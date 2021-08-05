using System;
using System.Collections.Generic;

using System.IdentityModel.Tokens.Jwt;

using System.Net;
//using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using Newtonsoft.Json.Linq;
using ClearSCADA.DBObjFramework;
using ClearSCADA.DriverFramework;
using SELogger;
using ClearScada.Client;
using System.Diagnostics;
using System.Net.Http;

namespace DriverSELogger
{
    class DriverSELogger
	{
		static void Main(string[] args)
		{
			// Debugger.Launch();

			using (DriverApp App = new DriverApp())
			{
				// Init the driver with the database module
				if (App.Init(new CSharpModule(), args))
				{
					// Do custom driver init here
					App.Log("SELogger driver started");

					// Start the driver MainLoop
					App.MainLoop();
				}
			}
		}
	}

	public static class RestAPITools
	{
		public static async Task<string> FetchOrValidateAccessToken(string AuthURL, string apiKey, string apiSecret, string tokenStr = null)
		{

			if (string.IsNullOrEmpty(tokenStr))
			{
				tokenStr = await GetAccessToken(AuthURL, apiKey, apiSecret);
			}


			if (DateTime.UtcNow > new JwtSecurityTokenHandler().ReadJwtToken(tokenStr).ValidTo)
			{
				tokenStr = await GetAccessToken(AuthURL, apiKey, apiSecret);
			}

			return tokenStr;
		}



		/// <summary>
		/// Method that will return an AccessToken from SE DataLogger REST-API oAuth2 Service
		/// </summary>
		/// <param name="apiKey">APIKey from SE DataLogger Platform</param>
		/// <param name="apiSecret">API Secret from SE DataLogger Platform </param>
		/// <returns>Returns access token</returns>
		/// <exception cref="Exception"></exception>
		private static async Task<string> GetAccessToken(string AuthURL, string apiKey, string apiSecret)
		{
			using (var client = new HttpClient())
			{

				client.BaseAddress = new Uri(AuthURL);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
					"Basic",
					Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}")));

				var content = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("grant_type", "client_credentials")
				});

				var result = await client.PostAsync(AuthURL, content);

				if (!result.IsSuccessStatusCode)
					throw new Exception($"Failed to receive access token: ret code {result.StatusCode}");

				var resultContent = await result.Content.ReadAsStringAsync();
				var json = JObject.Parse(resultContent);

				return (string)json["access_token"];
			}
		}

	}


	[Serializable]
	public class ConfigItem
	{
		public Int32 DeviceId;
		public StoredSiteResponse SiteConfig;
		public StoredDeviceResponse DeviceConfig;
		public StreamStoredStreamResponseCollection PointConfig;
		public ConfigItem(Int32 _DeviceId, StoredSiteResponse _siteConfig, StoredDeviceResponse _devconfig, StreamStoredStreamResponseCollection _pointconfig)
		{
			DeviceId = _DeviceId;
			SiteConfig = _siteConfig;
			DeviceConfig = _devconfig;
			PointConfig = _pointconfig;
		}

		// Ready to configure boolean - depends on all info present, which it should be for SELogger
		public bool Ready()
		{
			// We'll define birth readiness as having non-zero DeviceId
			if (DeviceId > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public class DrvSELoggerChannel : DriverChannel<SELoggerChannel>
    {
        // List of known devices
        public Dictionary<Int32, DrvCSScanner> DeviceIndex = new Dictionary<Int32, DrvCSScanner>();
        // List of unknown devices. Persisted to database module as a string of uuid and names.
        public Dictionary<Int32, ConfigItem> configBuf = new Dictionary<Int32, ConfigItem>();

		// Access token
		public string tokenStr = "";

		public void LogAndEvent( string Message)
		{
			Log(Message);
			if (this.DBChannel.EnhancedEvents)
			{
				App.SendReceiveObject(this.DBChannel.Id, OPCProperty.SendRecLogChannelEventText, Message);
			}
		}

		public override void OnDefine()
        {
            // Called (first) when enabled or config changed. Not on startup
            base.OnDefine();
            LogAndEvent("Defined channel with base URL: " + (string)DBChannel.BaseURL);

            // Clear the internal index of devices (scanners)
            DeviceIndex.Clear();
        }

        public override void OnConnect()
        {
            Log("Channel OnConnect(): Start, call base");
			base.OnConnect();

			// Make a connection to get the API Key
			ServerStatus ss = DoConnect(); 

            if (ss == ServerStatus.Offline)
            {
                LogAndEvent("Failed to connect.");
                throw new System.Runtime.InteropServices.COMException("Connection unsuccessful");
            }
            else
            {
                LogAndEvent("OnConnect Channel Online.");
            }
            Log("Channel OnConnect(): End");
        }


		public override void OnDisconnect()
        {
			// No actions to be taken for web api
            Log("Channel OnDisConnect(): Start");

            Log("Channel OnDisConnect(): Call base");
            base.OnDisconnect();

            Log("Channel OnDisConnect(): End");
        }

        public override void OnUnDefine()
        {
            Log("Channel OnUndefine(): Start");

            Log("Channel OnUndefine(): Call base");
            base.OnUnDefine();

            Log("Channel OnUndefine(): End");
        }

		// Last poll time defined as weeks ago, so first poll always happens
		private DateTime LastPollTime = DateTime.UtcNow.AddDays(-100);

		public override void OnPoll()
		{
			LogAndEvent("Channel OnPoll");
			// Defaults to 30 seconds, but we will use the channel ConfigReadRate property (seconds).
			if (LastPollTime.AddSeconds(DBChannel.ConfigReadRate) > DateTime.UtcNow)
			{
				return;
			}
			LastPollTime = DateTime.UtcNow;

			// No need to call base class. Return exception on failure of connection
			Configuration.Default.BasePath = DBChannel.BaseURL;

			try
			{
				var tokenTask = RestAPITools.FetchOrValidateAccessToken(DBChannel.AuthURL, DBChannel.APIKey, DBChannel.APISecret);
				tokenTask.Wait();
				tokenStr = tokenTask.Result;
			}
			catch (Exception e)
			{
				LogAndEvent("Poll token exception: " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""));
				tokenStr = "";
			}

			Log("Channel OnPoll(): " + tokenStr);
			if (tokenStr != "")
			{
				LogAndEvent("Channel OnPoll() set Connected");

				SetStatus(ServerStatus.Online, "Connected");

				// If an organisation is specified on the channel
				int organizationId = 0;
				if (DBChannel.OrganizationName != "")
				{
					// Read Organizations
					AccountStoredOrganizationResponseCollection organizations = null;
					try
					{ 
						var organizationsTask = new AccountApi().AccountGetAllOrganizationsAsync($"Bearer {tokenStr}");
						organizationsTask.Wait();
						organizations = organizationsTask.Result;
					}
					catch (Exception e)
					{
						LogAndEvent("Channel problem getting organizations: " + e.Message);
						SetFailReason("Could not find organizations.");
						SetStatus(ServerStatus.Offline, "Cannot find organizations");
						return;
					}

					if (organizations != null && organizations.Count > 0)
					{
						foreach (var organization in organizations)
						{
							LogAndEvent($"Found Organization Id: {organization.Id}, DisplayName: { organization.DisplayName}");
							if (organization.DisplayName == DBChannel.OrganizationName)
							{
								organizationId = (int)organization.Id;
							}
						}
					}
				}
				// Now if organizationId is non-zero then filter the sites by organisation, otherwise retrieve them all
				SiteStoredSiteResponseCollection sites = null;
				try
				{ 
					var sitesTask = new SiteApi().SiteGetAllSitesAsync($"Bearer {tokenStr}");
					sitesTask.Wait();
					sites = sitesTask.Result;
				}
				catch (Exception e)
				{
					LogAndEvent("Channel problem getting devices: " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""));
					// Some Cloud API configurations do not return any devices, but there may be sites with streams still.
					// We could just fail here and stop processing (uncomment next 3 lines and remove what is below).

					//SetFailReason("Could not find devices.");
					//SetStatus(ServerStatus.Offline, "Cannot find devices");
					//return;

					// Alternatively we could make up devices from the available site data.
					// And this is a substitute until we find out why the API does this.
					devices = new DeviceStoredDeviceResponseCollection();
					foreach (var site in sites)
					{
						StoredDeviceResponse NewDevice = new StoredDeviceResponse(
							"No device AKID",
							site.DisplayName,
							"", "", "", 
							site.Id, 
							"", 
							"No device Model", 
							"No device serial", 
							site.DisplayName, 
							(int?)site.Id );

						devices.Add(NewDevice);

						LogAndEvent("Created internal device: " + site.DisplayName + " Id: " + site.Id.ToString());
					}
				}

				if (sites != null && sites.Count > 0)
				{
					foreach (var site in sites)
					{
						LogAndEvent($"Found Site, Id: { site.Id}, Account Org Id: {site.AccountOrganizationId}, Status {site.Status}, DisplayName: { site.DisplayName}, Lat: {site.Latitude}, Long: {site.Longitude}");
					}
				}

				//Now look up all Devices
				DeviceStoredDeviceResponseCollection devices = null;
				try
				{ 
					var devicesTask = new DeviceApi().DeviceGetAllDevicesAsync($"Bearer {tokenStr}");
					devicesTask.Wait();
					devices = devicesTask.Result;
				}
				catch (Exception e)
				{
					LogAndEvent("Channel problem getting devices: " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""));
					SetFailReason("Could not find devices.");
					SetStatus(ServerStatus.Offline, "Cannot find devices");
					return;
				}

				if (devices != null && devices.Count > 0)
				{
					foreach (var device in devices)
					{
						LogAndEvent($"Found Device Id: {device.Id}, DisplayName {device.DisplayName}, Site Id: {device.SiteId}, Model: { device.ModelNumber}, Serial: { device.SerialNumber}");
					}
				}

				// We will select the device matching the site
				foreach (var site in sites)
				{
					// Ignore sites not matching this one
					if (organizationId == 0 || organizationId == site.AccountOrganizationId)
					{
						// Find matching Device to get the Id
						foreach (var device in devices)
						{
							if (device.SiteId == site.Id)
							{
								ReadDeviceConfig((int)device.Id, site, device);
							}
						}
					}
				}
			}
			else
			{
				LogAndEvent("Channel OnPoll(): Set Status Offline");
				SetFailReason("Could not open connection.");

				SetStatus(ServerStatus.Offline, "Cannot connect");
			}
		}

		// Called on (re)connection - get the access token into tokenStr
		private ServerStatus DoConnect()
        {
			// We connect up here to get the token, and fail the channel if we don't get it
			// We do it again and get actual data in the OnPoll
            Log("Channel doConnect(): Call Conn...");
			Configuration.Default.BasePath = DBChannel.BaseURL;

			try
			{
				var tokenTask = RestAPITools.FetchOrValidateAccessToken(DBChannel.AuthURL, DBChannel.APIKey, DBChannel.APISecret);
				tokenTask.Wait();
				tokenStr = tokenTask.Result;
			}
			catch (Exception e)
			{
				LogAndEvent("Connect fetch exception: " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""));
			}

            Log("Channel doConnect() token: " + tokenStr);
			if (tokenStr != "")
			{ 
                LogAndEvent("Channel doConnect() set Connected");
                SetStatus(ServerStatus.Online, "Connected");

				// Alarm Clear
				//Log("Channel doConnect(): SendReceive to Clear Alarm");
				//App.SendReceiveObject(DBChannel.Id, SendRecClearChannelAlarm, true);
				//if (!LastKnownOnline)
				//{
				//    Log("Channel doConnect(): Call SetScannersOnline");
				//    //SetScannersOnline();
				//    LastKnownOnline = true;
				//}
				return ServerStatus.Online;
            }
            else
            {
                LogAndEvent("Channel doConnect(): Set Status Offline");
                SetFailReason("Could not open connection.");

                SetStatus(ServerStatus.Offline, "Cannot connect");

                // Alarm Active
                //Log("Channel doConnect(): SendReceive to Activate Alarm");
                //App.SendReceiveObject(DBChannel.Id, OPCProperty.SendRecRaiseChannelAlarm, true);
                //if (LastKnownOnline)
                //{
                //    Log("Channel doConnect(): Call SetScannersOffline");
                //    //SetScannersOffline();
                //    LastKnownOnline = false;
                //}
                return ServerStatus.Offline;
            }
        }


		public void AddScannerToIndex(DrvCSScanner s)
        {
            // Check if channel already contains scanner before adding
            if (DeviceIndex.ContainsKey((Int32)s.DBScanner.DeviceId) == false)
            {
                DeviceIndex.Add(s.DBScanner.DeviceId, s);
                LogAndEvent("Add to Device Dictionary, total now: " + DeviceIndex.Count);
            }
            else
            {
				LogAndEvent("NOT Added to Device Dictionary - Duplicate. Total now: " + DeviceIndex.Count + " | scanner " + s.ToString());
            }
        }

        public void RemoveScannerFromIndex(DrvCSScanner s)
        {
            if (DeviceIndex.ContainsKey((Int32)s.DBScanner.DeviceId) == true)
            {
                DeviceIndex.Remove(s.DBScanner.DeviceId);
				LogAndEvent("Removed from Device Dictionary: " + DeviceIndex.Count);
            }
            else
            {
				LogAndEvent("NOT found for deletion in Device Dictionary. Total now: " + DeviceIndex.Count + " | scanner " + s.ToString());
            }
        }

		private void ReadDeviceConfig(int DeviceID, StoredSiteResponse site, StoredDeviceResponse device)
		{
			// 
			// Get streams configured for this device/site
			LogAndEvent("Get Streams");
			try
			{
				var streamsTask = new StreamApi().StreamGetStreamsBySiteAsync((int)site.Id, $"Bearer {tokenStr}");
				streamsTask.Wait();
				var streams = streamsTask.Result;

				if (streams != null && streams.Count > 0)
				{
					foreach (var stream in streams)
					{
						LogAndEvent($"Found Stream: Id: {stream.Id}, Site: {stream.SiteId}, DisplayName: { stream.DisplayName}, TypeId: {stream.TypeId}, TypeDisplayName: {stream.TypeDisplayName}, Unit: {stream.Units}, Scale: {stream.ValueScale}");
					}
					StartDevice(DeviceID, site, device, streams);
				}
			}
			catch (Exception e)
			{
				// No streams no device.
				LogAndEvent($"Cannot read streams for device {DeviceID} site {site.Id}: {e.Message}");
			}
		}

		// A device has been found
		private void StartDevice(Int32 DeviceId, StoredSiteResponse siteConfig, StoredDeviceResponse deviceConfig, StreamStoredStreamResponseCollection streamConfig)
        {

			LogAndEvent("Start Device: " + DeviceId.ToString());

			string ConfigChecksum = CalcConfigChecksum(siteConfig, deviceConfig, streamConfig);

			// A device has connected - existing or new?
			if (DeviceIndex.TryGetValue( DeviceId, out DrvCSScanner FD))
            {
				// Got the start message for a previously known device

				// Clear alarm
				Log("Clear Scanner (FD) Alarm.");
				App.SendReceiveObject(FD.DBScanner.Id, OPCProperty.SendRecClearScannerAlarm, null);
				// if ( (string)ReplyObject == "Yes")

				// Mark as Online!
				FD.SetStatus(SourceStatus.Online);

				// Existing device may need reconfiguration, so check and do
				if (ConfigChecksum != FD.DBScanner.ConfigChecksum)
				{
					FD.LogAndEvent("Received newer configuration");
					if (DBChannel.AutoReconfig)
					{
						FD.LogAndEvent("Update configuration");
						HandleNewConfig(siteConfig, deviceConfig, streamConfig, FD);
					}
					else
					{
						FD.LogAndEvent("Auto reconfiguration not enabled.");
					}
				}

				object ReplyObject = "";
				// Add Device and Point data into one for the SendReceive
				App.SendReceiveObject(FD.DBScanner.Id, OPCProperty.SendRecSetChecksum, ConfigChecksum, ref ReplyObject);

			}
			else
            {
                // Brand new device - not seen. 
                LogAndEvent("Device not in database.");
				if (!configBuf.TryGetValue(DeviceId, out ConfigItem thisItem))
				{
					// Save this device config for later
					LogAndEvent("Device not buffered, adding to memory.");
					thisItem = new ConfigItem(DeviceId, siteConfig, deviceConfig, streamConfig);
					configBuf.Add(DeviceId, thisItem);
				}
				// Later has arrived! If we are auto-configuring then process it, otherwise leave for later.
				CheckReadyInitiateConfig( DeviceId, thisItem);
			}
        }

		
		private string CalcConfigChecksum(StoredSiteResponse site, StoredDeviceResponse device, StreamStoredStreamResponseCollection streams)
		{
			string configData = ($"{site.Id},{site.AccountOrganizationId},{site.DisplayName}");
			// Add Lat and Long so changes will also cause reconfiguration - this could be more efficient
			configData += $"{site.Latitude},{site.Longitude}";
			// Can't find a way to set location in the DLL, so has to be done with the Client connection - could at least be separated into a different change mechanism

			configData += $"{device.Id},{device.DisplayName},{device.SiteId},{device.ModelNumber},{device.SerialNumber},{device.FirmwareVersion},{device.Akid},{device.Iccid},{device.Iccid2},{device.Meid}";

			if (streams != null && streams.Count > 0)
			{
				foreach (var stream in streams)
				{
					configData += $"{stream.Id},{stream.SiteId},{stream.DisplayName},{stream.TypeId},{stream.TypeDisplayName},{stream.Units},{stream.ValueScale}";
				}

			}
			return Util.GetHashString(configData);
		}


		private void HandleNewConfig(StoredSiteResponse S, StoredDeviceResponse D, StreamStoredStreamResponseCollection P, DrvCSScanner FD)
		{
			FD.LogAndEvent("Request configuration update action within driver.");
			if (UpdateFieldDevice(S, D, P, FD, out string ErrorText))
			{
				FD.LogAndEvent("Success modifying: " + FD.FullName);
			}
			else
			{
				FD.LogAndEvent("Failed to modify: " + FD.FullName + " " + ErrorText);
				// Should send receive to raise alarm.
				App.SendReceiveObject(this.DBChannel.Id, OPCProperty.SendRecReportConfigError, "Failed to modify Configuration: " + FD.FullName + " " + ErrorText);
			}
		}

		private void CheckReadyInitiateConfig(Int32 DeviceId, ConfigItem thisItem)
        {
            // Do we have all properties? Only check when Config is received.
            if (thisItem.Ready())
            {
                // Allow this to be auto-configured if the broker configuration permits it.
                // Or raise an alert/notification so the user can manually initiate the process
                if (DBChannel.AutoConfig)
                {
                    // Send request to configure. Ends up calling a method with the arguments in this driver exe.
                    LogAndEvent("Request auto configuration action within driver.");
					if (CreateFieldDevice(DeviceId, out string ErrorText))
					{
						LogAndEvent("Success creating: " + DeviceId);
					}
					else
					{
						LogAndEvent("Failed to create: " + DeviceId + " " + ErrorText);
						// Should send receive to raise alarm.
						App.SendReceiveObject(this.DBChannel.Id, OPCProperty.SendRecReportConfigError, "Failed to create FD: " + DeviceId + " " + ErrorText);
					}
					// Remove from queue
					configBuf.Remove(DeviceId);
                }
                else
                {
                    // Send request to alert user of config need. Ends up calling same CreateFieldDevice method as above
                    LogAndEvent("Request configuration request alarm for: " + DeviceId );
                    App.SendReceiveObject(this.DBChannel.Id, OPCProperty.SendRecRequestConfiguration, DeviceId);
                }
                // Update servers list of items waiting for config.
                RefreshPendingQueue();
            }
        }


		public override void OnExecuteAction(ClearSCADA.DriverFramework.DriverTransaction Transaction)
        {
			LogAndEvent("Driver Action - channel.");
			switch (Transaction.ActionType)
			{
				case OPCProperty.DriverActionInitiateConfig:
					{
						// Configure a device with this uuid
						Int32 DeviceId = (Int32)Transaction.get_Args(0);
						if (CreateFieldDevice(DeviceId, out string ErrorText))
						{
							this.CompleteTransaction(Transaction, 0, "Successfully created device: " + DeviceId);
						}
						else
						{
							LogAndEvent("Failed to create: " + DeviceId.ToString() + " " + ErrorText);
							// Should send receive to raise alarm.
							App.SendReceiveObject(this.DBChannel.Id, OPCProperty.SendRecReportConfigError, "Failed to create: " + DeviceId.ToString() + " " + ErrorText);

							this.CompleteTransaction(Transaction, 0, "Failed to create device: " + DeviceId + " " + ErrorText);
						}
					}
					break;

				case OPCProperty.DriverActionRetrieveConfig:
					{
						// Cause a scheduled retrieval in the next poll make making the last poll time old.
						LastPollTime = DateTime.UtcNow.AddDays(-100);
						this.CompleteTransaction(Transaction, 0, "Successfully scheduled retrieval" );
					}
					break;

				default:
					base.OnExecuteAction(Transaction);
					break;
			}
		}

		public bool UpdateFieldDevice(StoredSiteResponse S, StoredDeviceResponse D, StreamStoredStreamResponseCollection P, DrvCSScanner FD, out string ErrorText)
		{
			// Find the device
			// Need to connect to the database as a client
			if (!Connect2Net(DBChannel.ConfigUserName, DBChannel.ConfigPass, out ClearScada.Client.Simple.Connection connection, out ClearScada.Client.Advanced.IServer AdvConnection))
			{
				ErrorText = ("Driver cannot connect client to server.");
				return false;
			}

			ClearScada.Client.Simple.DBObject FieldDevice;
			try
			{
				ObjectId FieldDeviceId = new ObjectId(unchecked((int)FD.DBScanner.Id));
				FieldDevice = connection.GetObject(FieldDeviceId);
			}
			catch (Exception Fail)
			{
				ErrorText = "Cannot get device from its Id? " + Fail.Message;
				return false;
			}

			// Get parent group - not required yet.
			//ClearScada.Client.Simple.DBObject ParentGroup;
			//ParentGroup = FieldDevice.Parent; // this variable is 'Instance' in CreateFieldDevice()

			// Call function to do this all, starting with the UUID
			LogAndEvent("Reconfigure Device And Children");
			return ReconfigureDeviceAndChildren( S, D, P, FieldDevice, connection, AdvConnection, out ErrorText);
		}

		public bool CreateFieldDevice(Int32 DeviceId, out string ErrorText)
		{
			// Need to connect to the database as a client
			if (!Connect2Net(DBChannel.ConfigUserName, DBChannel.ConfigPass, out ClearScada.Client.Simple.Connection connection, out ClearScada.Client.Advanced.IServer AdvConnection))
			{
				ErrorText = ("Driver cannot connect client to server.");
				return false;
			}

			if (!configBuf.TryGetValue(DeviceId, out ConfigItem thisItem))
			{
				ErrorText = ("Cannot find device config message in configuration buffer.");
				return false;
			}

			bool status =  CreateFieldDeviceObjects(DeviceId, thisItem.SiteConfig, thisItem.DeviceConfig, thisItem.PointConfig, connection, AdvConnection, out ErrorText);
			// Should disconnect everywhere it goes wrong too!!
			DisconnectNet(connection, AdvConnection);

			// Finish by remove 
			configBuf.Remove(DeviceId);
			LogAndEvent("Removed from config buffer");
			RefreshPendingQueue();
			LogAndEvent("Config buffer refreshed.");

			return status;
		}

		Int32 QueryDatabaseForId(ClearScada.Client.Advanced.IServer AdvConnection, string sql)
		{
			Int32 result = 0;

			// Find the database reference of the Node
			ClearScada.Client.Advanced.IQuery serverQuery = AdvConnection.PrepareQuery(sql, new ClearScada.Client.Advanced.QueryParseParameters());
			ClearScada.Client.Advanced.QueryResult queryResult = serverQuery.ExecuteSync(new ClearScada.Client.Advanced.QueryExecuteParameters());

			if (queryResult.Status == ClearScada.Client.Advanced.QueryStatus.Succeeded || queryResult.Status == ClearScada.Client.Advanced.QueryStatus.NoDataFound)
			{
				if (queryResult.Rows.Count > 0)
				{
					// Found
					IEnumerator<ClearScada.Client.Advanced.QueryRow> e = queryResult.Rows.GetEnumerator();
					while (e.MoveNext())
					{
						result = (Int32)e.Current.Data[0];
					}
				}
			}
			serverQuery.Dispose();

			return result;
		}

		public bool CreateFieldDeviceObjects(	Int32 DeviceId,
												StoredSiteResponse Site,
												StoredDeviceResponse Dev,
												StreamStoredStreamResponseCollection Points,
												ClearScada.Client.Simple.Connection connection, 
												ClearScada.Client.Advanced.IServer AdvConnection, 
												out string ErrorText)
		{
			// Device object
			string DeviceName = Dev.DisplayName;

			// SCADA object names may not contain invalid chars
			string GSDeviceName = Logger2GSname(DeviceName).Replace('.', '_');

			// Define FieldDevice here
			ClearScada.Client.Simple.DBObject FieldDevice = null;
			// The group our Field Device sits in
			ClearScada.Client.Simple.DBObject Instance;


			//////////////////////////////////////////////////////////////////////////////////
			// Check Logger Device with this Id does not exist
			//////////////////////////////////////////////////////////////////////////////////
			// Need to query all devices on this channel by DeviceId
			// Enclose scope so the same names can be used
			string sql = "SELECT Id, Fullname FROM SELoggerDevice WHERE ChannelId = " + DBChannel.Id.ToString() + 
							" AND DeviceId = " + DeviceId + "";
			int DBDeviceId = QueryDatabaseForId(AdvConnection, sql);
			if (DBDeviceId > 0)
			{
				LogAndEvent( $"A device with this Device Id exists: {DeviceId} Row: {DBDeviceId}" );
				// Perhaps should not be here, but continue to find device
				try
				{
					FieldDevice = connection.GetObject((ClearScada.Client.ObjectId)(int)DBChannel.ConfigGroupId.Id);
				}
				catch
				{
					LogAndEvent("Can not (re)find device.");
					ErrorText = "Can not find a device but query did";
					return false;
				}
				LogAndEvent($"Found existing device, AutoReconfig={DBChannel.AutoReconfig}");
				if (!DBChannel.AutoReconfig)
				{
					ErrorText = "";
					return true;
				}
			}
			else
			{
				LogAndEvent("No existing device found - proceeding to create: " + DeviceId);


				//////////////////////////////////////////////////////////////////////////////////
				// Construct name of instance from the Configuration
				// If this is a device, then name is <ConfigGroupId>.<GSDeviceName>.Device

				// Could also filter other invalid characters. Used for devices too, if no template.
				LogAndEvent("Configure device: " + DeviceId + " as " + GSDeviceName);

				//////////////////////////////////////////////////////////////////////////////////
				// Group to contain instances or field device object groups
				ClearScada.Client.Simple.DBObject InstanceGroup;
				if (DBChannel.ConfigGroupId.Id <= 0)
				{
					ErrorText = ("Invalid channel instance group configuration.");
					return false;
				}
				try
				{
					InstanceGroup = connection.GetObject((ClearScada.Client.ObjectId)(int)DBChannel.ConfigGroupId.Id);
					LogAndEvent("Instance group. " + InstanceGroup.FullName);
				}
				catch
				{
					ErrorText = ("Invalid Configuration group. ");
					return false;
				}

				//////////////////////////////////////////////////////////////////////////////////
				// Read the template information from the channel
				// Use unchecked type conversion as there are differences in DDK library vs Client library in handling object ids
				Int32 TemplateId = unchecked((int)DBChannel.TemplateId.Id);
				if (TemplateId > 0)
				{
					try
					{
						// Read the Template
						ClearScada.Client.Simple.DBObject DeviceTemplate = connection.GetObject(TemplateId);
						LogAndEvent("Device Template: " + DeviceTemplate.FullName);
					}
					catch
					{
						LogAndEvent("Cannot read Device Template.");
						TemplateId = 0;
					}
				}
				else
				{
					LogAndEvent("No Device template configured.");
					TemplateId = 0;
				}

				//////////////////////////////////////////////////////////////////////////////////
				// Create instance or group with suggested name
				LogAndEvent("Trying to create instance or group.");
				if (TemplateId > 0)
				{
					LogAndEvent("Valid TemplateId");
					ObjectId CSTemplateId = new ObjectId(TemplateId);

					// Create Instance
					try
					{
						Instance = connection.CreateInstance(CSTemplateId, InstanceGroup.Id, GSDeviceName);
					}
					catch (Exception Fail)
					{
						ErrorText = ("Error creating device template instance. " + Fail.Message);
						// This could raise an alarm - there should not be an object with this name.
						return false;
					}
					// Now find the FD associated with this template
					// Find the FD - assumed to be a direct child of the instance
					ClearScada.Client.Simple.DBObjectCollection fds = Instance.GetChildren("", "");
					bool Found = false;
					foreach (ClearScada.Client.Simple.DBObject o in fds)
					{
						if (o.ClassDefinition.Name == "SELoggerDevice")
						{
							// This is an FD - assume only one
							Found = true;
							LogAndEvent("Found FD: " + o.FullName);
							FieldDevice = o;
							break;
						}
					}
					if (!Found)
					{
						ErrorText = ("No FD found as child of Template instance.");
						return false;
					}
				}
				//////////////////////////////////////////////////////////////////////////////////
				// Not templated
				else
				{
					// There is no template for this profile, so create FD in a group directly
					// Not an instance, but using that object ref
					LogAndEvent("Create group directly.");
					try
					{
						Instance = connection.CreateObject("CGroup", InstanceGroup.Id, GSDeviceName);
					}
					catch (Exception Failure)
					{
						// Possible to consider continuing here, keep this group and add FD to it.
						ErrorText = "Error creating Group, may already exist - cannot create field device. " + Failure.Message;
						return false;
					}
					// Create field device
					try
					{
						string FDName = "Logger";
						FieldDevice = connection.CreateObject("SELoggerDevice", Instance.Id, FDName);
					}
					catch (Exception Failure)
					{
						ErrorText = "Error creating Logger, may already exist. " + Failure.Message;
						return false;
					}
				}
				// Done Creating Field Device etc.

				//////////////////////////////////////////////////////////////////////////////////
				// Set field device properties
				// ChannelId - ignore if set already - may be already set appropriately in the template
				Int32 ChanId = (Int32)FieldDevice.GetProperty("ChannelId");
				if (ChanId == DBChannel.Id)
				{
					LogAndEvent("Field Device has correct channel.");
				}
				else
				{
					if (!CheckSet(FieldDevice, "ChannelId", this.DBChannel.Id))
					{
						ErrorText = "Error writing device ChannelId.";
						return false;
					}
				}
				// DeviceId. Checkset returns false if failed, 
				if (!CheckSet(FieldDevice, "DeviceId", DeviceId))
				{
					ErrorText = "Error writing device DeviceId.";
					return false;
				}
			}

			// Set properties function
			bool status = ReconfigureDeviceAndChildren(Site, Dev, Points, FieldDevice, connection, AdvConnection, out ErrorText);
			if (!status)
			{
				return false;
			}

			// Use DLL to write checksum config field
			string Reply = "";
			object ReplyObject = Reply;
			LogAndEvent("Send/receive on-Connection config info");
			string ConfigChecksum = CalcConfigChecksum( Site, Dev, Points);
			App.SendReceiveObject((uint)FieldDevice.Id.ToInt32(), OPCProperty.SendRecSetChecksum, ConfigChecksum, ref ReplyObject); // Birth data.

			//if good
			ErrorText = "";
            return true;                  
        }


		private bool ReconfigureDeviceAndChildren(  StoredSiteResponse site,
													StoredDeviceResponse device, 
													StreamStoredStreamResponseCollection Points, 
													ClearScada.Client.Simple.DBObject FieldDevice,
													ClearScada.Client.Simple.Connection connection,
													ClearScada.Client.Advanced.IServer AdvConnection,
													out string ErrorText)
		{
			ErrorText = "";

			// Set device properties
			LogAndEvent($"Found Site, Id: { site.Id}, Account Org Id: {site.AccountOrganizationId}, Status {site.Status}, DisplayName: { site.DisplayName}");
			CheckSet(FieldDevice, "SiteId", site.Id);
			CheckSet(FieldDevice, "SiteDisplayName", site.DisplayName);
			if (site.AccountOrganizationId is null)
			{
				Log("AccountOrganizationId is null");
			}
			else
			{
				CheckSet(FieldDevice, "OrganizationId", (Int32)site.AccountOrganizationId);
			}

			LogAndEvent($"Found Device Id: {device.Id}, DisplayName {device.DisplayName}, Site Id: {device.SiteId}, Model: { device.ModelNumber}, Serial: { device.SerialNumber}");
			CheckSet(FieldDevice, "DeviceDisplayName", device.DisplayName);
			CheckSet(FieldDevice, "ModelNumber", device.ModelNumber);
			CheckSet(FieldDevice, "SerialNumber", device.SerialNumber);

			CheckSet(FieldDevice, "FirmwareVersion", device.FirmwareVersion);
			CheckSet(FieldDevice, "AKID", device.Akid);
			CheckSet(FieldDevice, "ICCID", device.Iccid);
			CheckSet(FieldDevice, "ICCID2", device.Iccid2);
			CheckSet(FieldDevice, "MEID", device.Meid);

			// Set location properties from site Lat and Long
			// Alternatively this could be modified to set the location of the parent group
			// The location is only set when devices are created - this could instead be updated each poll by getting site info,
			// which would be needed if the location of loggers changes such as when on vehicles.
			// Or by retrieving full history data and populating to related point values or a Data Table if history is desired.

			// First set the location to be dynamic:
			var GeoAgg = FieldDevice.Aggregates["GISLocationSource"];
			bool ContinueGeo = true;
			if (!GeoAgg.Enabled || GeoAgg.ClassName != "CGISLocationSrcDynamic")
			{
				// May fail if templated
				try
				{
					GeoAgg.ClassName = "CGISLocationSrcDynamic";
				}
				catch (Exception e)
				{
					LogAndEvent("Cannot set device location aggregate. " + e.Message);
					ContinueGeo = false;
				}
			}
			if (ContinueGeo)
			{
				try
				{
					// Set location
					GeoAgg["Latitude"] = site.Latitude;
					GeoAgg["Longitude"] = site.Longitude;
					GeoAgg["UpdateTime"] = DateTime.UtcNow;
				}
				catch (Exception e)
				{
					LogAndEvent("Cannot set device location aggregate values. " + e.Message);
				}
			}

			// Create points
			// 
			if (Points != null && Points.Count > 0)
			{
				foreach (var p in Points) 
				{
					// Available types for auto create will only be analogues
					// In single quotes as this is used in SQL
					string csType = "'SELoggerPointAg'";

					LogAndEvent($"Found Stream: Id: {p.Id}, Site: {p.SiteId}, DisplayName: { p.DisplayName}, TypeId: {p.TypeId}, TypeDisplayName: {p.TypeDisplayName}, Unit: {p.Units}, Scale: {p.ValueScale}");
					//"Find/Create: Type: " + csType + ", " + ", Name: '" + p.Name + "', StreamId: " + p.Alias.ToString() + ", Parent: " + FieldDevice.FullName);

					// If there is a point of the correct type and SELoggerName linked to this device, use it
					// or
					// If there is a point of the correct type and SELoggerName not linked to this device, link and use it
					// or
					// If there is a point with name <ConfigGroupId>.<Device/Instance Group>.<Name/path>, use it (it could have been created in a previous template)
					// or
					//   Create standalone point
					//     <ConfigGroupId>.<Device/Instance Group>.<Name/path>

					// Logger names may include . which will be used to create subfolder names
					// We will try to replace . with _ and then replace / with . (allows
					// But we also use a function to replace other non-allowed characters with _
					string GSPointPath = Logger2GSname(p.DisplayName);
					string pointFullName = FieldDevice.Parent.FullName + "." + GSPointPath; // 'Ideal' point name
					// Include all point types
					string PointTables = "SELoggerPointAg UNION SELoggerPointDg UNION SELoggerPointCi UNION SELoggerPointTm";

					// The point we are looking for
					Int32 pointId = 0;

					if (pointId == 0)
					{
						// If there is a point of the correct scanner and stream linked to this device, use it
						// Look for a point of correct stream Id linked to this FieldDevice, so it could be in a different folder
						string sql = "SELECT id, Fullname, ScannerId, StreamId FROM " + PointTables + " WHERE " +
										"ScannerId = " + FieldDevice.Id.ToString() + " and " +
										"StreamId = " + p.Id;
						pointId = QueryDatabaseForId(AdvConnection, sql);
						if (pointId != 0)
						{
							LogAndEvent("Found point 1st time");
						}
					}

					// Not found a point with the right stream, so look in folder
					// If there is a point and SELoggerName not linked to this device, link and use it
					if (pointId == 0)
					{
						// Look for a point of p.DisplayName = DisplayName in FieldDevice folder/subfolders
						string sql = "SELECT id, Fullname, ScannerId, DisplayName, StreamId FROM " + PointTables + " WHERE " +
										"FullName LIKE '" + FieldDevice.Parent.FullName + ".%' and " +
										"DisplayName = '" + p.DisplayName + "'";
						pointId = QueryDatabaseForId(AdvConnection, sql);
						if (pointId != 0)
						{
							LogAndEvent("Found point 2nd time");
						}
					}

					// Still not found,
					// If there is a point with name <ConfigGroupId>.<Device>.<Name/path>, use it 
					// (it could have been created in a previous template and the DisplayName not set)
					if (pointId == 0)
					{
						// Look for a point of type csType = SPtype and p.name = SELoggerName in FieldDevice folder/subfolders
						string sql = "SELECT id, Fullname, ScannerId, DisplayName, StreamId FROM " + PointTables + " WHERE " +
										"FullName = '" + pointFullName + "'";

						pointId = QueryDatabaseForId(AdvConnection, sql);
						if (pointId != 0)
						{
							LogAndEvent("Found point 3rd time");
						}
					}

					// Last chance - create point
					// First work out the desired class name - first of the comma sep quoted list in csType
					string[] PointTypeList = csType.Split(',');
					string DesiredType = PointTypeList[0].Replace("'","");
					ClearScada.Client.Simple.DBObject PointObject;
					if (pointId == 0)
					{
						// First create any parent subfolders specified by the Name property
						string[] GSPointPathList = GSPointPath.Split('.');
						ClearScada.Client.Simple.DBObject ParentGroup = FieldDevice.Parent;
						// Process each element except the last, which is the point name
						for (int i = 0; i < GSPointPathList.Length - 1; i++)
						{
							// Does it exist already?
							ClearScada.Client.Simple.DBObject ChildGroup;
							try
							{
								ChildGroup = ParentGroup.GetChild(GSPointPathList[i]);
							}
							catch (Exception e)
							{
								LogAndEvent("Error finding child group: " + GSPointPathList[i] + ", " + e.Message);
								break;
							}
							if (ChildGroup == null)
							{
								try
								{
									// Create group
									ChildGroup = ParentGroup.CreateObject("CGroup", GSPointPathList[i]);
								}
								catch (Exception e)
								{
									LogAndEvent("Error creating child group: " + GSPointPathList[i] + ", " + e.Message);
									break;
								}
							}
							// Rinse and repeat
							ParentGroup = ChildGroup;
						}

						// No valid Parent
						if (ParentGroup == null)
						{
							LogAndEvent("Cannot find parent group of point.");
							continue;
						}

						// Create the point and get its Id
						try
						{
							LogAndEvent("Create point");
							PointObject = connection.CreateObject(DesiredType, ParentGroup.Id, GSPointPathList[GSPointPathList.Length-1]);
							pointId = PointObject.Id.ToInt32();
							LogAndEvent("Created point: " + PointObject.FullName);
						}
						catch (Exception Fail)
						{
							LogAndEvent("Cannot create point: " + GSPointPathList[GSPointPathList.Length - 1] + " Error: " + Fail.Message);
							continue;
						}
					}
					else
					{
						LogAndEvent("Get point from reference");
						ObjectId CSPointId = new ObjectId(pointId);
						PointObject = connection.GetObject(CSPointId);
					}

					// Update the point properties
					LogAndEvent("Set point properties");
					CheckSet( PointObject, "ScannerId", FieldDevice.Id.ToInt32());
					CheckSet( PointObject, "StreamId", p.Id);

					CheckSet(PointObject, "SiteId", p.SiteId);
					CheckSet(PointObject, "DisplayName", p.DisplayName);
					CheckSet(PointObject, "TypeId", p.TypeId);
					CheckSet(PointObject, "LoggerTypeName", p.TypeDisplayName);

					// Set historic filters and enable history
					// TODO - here and elsewhere only set these on first create?
					CheckSet(PointObject,"HistoricFilterValue", true);
					CheckSet(PointObject,"HistoricFilterState", true);
					CheckSet(PointObject,"HistoricFilterReport", true);
					CheckSet(PointObject,"HistoricFilterEOP", true);
					
					// Historic - enable by default
					try
					{
						// Set historic
						PointObject.Aggregates["Historic"].Enabled = true;
					}
					catch (Exception Failure)
					{
						LogAndEvent("Error writing point history aggregate. " + Failure.Message);
					}

					CheckSet(PointObject, "InService", true);

					// Other properties (analogue)
					CheckSet(PointObject, "Units", p.Units);
					// Don't know what this property is yet: 
					//CheckSet(PointObject, "FullScale", p.ValueScale);


				}
			}
			LogAndEvent("All points done");

			// Return from doing device fields and points

			//if good
			if (ErrorText == "")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		// Generic way to set all object properties
		private bool CheckSet( ClearScada.Client.Simple.DBObject CSObj, string FieldName, ValueType PropValue)
		{
			if (PropValue != null)
			{
				try
				{
					CSObj.SetProperty(FieldName, PropValue);
					return true;
				}
				catch (Exception Failure)
				{
					LogAndEvent("Error writing field " + FieldName + " " + Failure.Message);
				}
			}
			return false;
		}

		private bool CheckSet(ClearScada.Client.Simple.DBObject CSObj, string FieldName, string PropValue)
		{
			if (PropValue != null)
			{
				try
				{
					CSObj.SetProperty(FieldName, PropValue);
					return true;
				}
				catch (Exception Failure)
				{
					LogAndEvent("Error writing field " + FieldName + " " + Failure.Message);
				}
			}
			return false;
		}

		// Alternative function to set a field property - only modify if zero.
		// Could be used to set alarm limits only if they are unset.
#pragma warning disable IDE0051 // Remove unused private members
		private bool CheckSetIfZero(ClearScada.Client.Simple.DBObject CSObj, string FieldName, ValueType PropValue)
#pragma warning restore IDE0051 // Remove unused private members
		{
			if (PropValue != null)
			{
				try
				{
					if ((int)CSObj.GetProperty(FieldName) == 0)
					{
						CSObj.SetProperty(FieldName, PropValue);
					}
					return true;
				}
				catch (Exception Failure)
				{
					LogAndEvent("Error writing field " + FieldName + " " + Failure.Message);
				}
			}
			return false;
		}

		private string Logger2GSname( string name)
        {
			return name.Replace('+', '-').Replace('=', '_').Replace('#', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_').Replace('@', '_').Replace(':', '_').Replace(';', '_');
		}

		public void RefreshPendingQueue()
        {
            // Build an array of uuid and device information
            Int32 [] pendingQueue = new Int32[configBuf.Count];
            int i = 0;
            foreach ( var c in configBuf)
            {
                pendingQueue[i] = c.Value.DeviceId;
                i++;
            }
            App.SendReceiveObject(DBChannel.Id, OPCProperty.SendRecUpdateConfigQueue, pendingQueue);
        }

        public bool Connect2Net(string user, string password, out ClearScada.Client.Simple.Connection connection, out ClearScada.Client.Advanced.IServer AdvConnection)
        {
            var node = new ClearScada.Client.ServerNode(ClearScada.Client.ConnectionType.Standard, "127.0.0.1", 5481);
            connection = new ClearScada.Client.Simple.Connection("SELogger");
            try
            {
                connection.Connect(node);
                AdvConnection = node.Connect("SELogger-Adv", false);
            }
            catch (CommunicationsException)
            {
                LogAndEvent("Unable to communicate with ClearSCADA server.");
                AdvConnection = null;
                return false;
            }

            if (connection.IsConnected)
            {
                Log("Connected to database.");

                using (var spassword = new System.Security.SecureString())
                {
                    foreach (var c in password)
                    {
                        spassword.AppendChar(c);
                    }

                    try
                    {
                        connection.LogOn(user, spassword);
                        AdvConnection.LogOn(user, spassword);
                    }
                    catch (AccessDeniedException)
                    {
                        LogAndEvent("Access denied, please check username and password. Check CAPS LOCK is off.");
                        return false;
                    }
                    catch (PasswordExpiredException)
                    {
                        LogAndEvent("Password is expired. Please reset with ViewX or WebX.");
                        return false;
                    }

                    Log("Logged In.");
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DisconnectNet( ClearScada.Client.Simple.Connection connection,  ClearScada.Client.Advanced.IServer AdvConnection)
        {
            try
            {
                connection.LogOff();
                connection.Disconnect();
                connection.Dispose();

                AdvConnection.LogOff();
                AdvConnection.Dispose();
            }
            catch (Exception Failure)
            {
                LogAndEvent("Disconnect Error: " + Failure.Message);
                return false;
            }
            return true;
        }
    }

    public class DrvCSScanner : DriverScanner<SELoggerDevice>
    {
		// Logging, including server-side debug logging (can add considerable server load)
		public void LogAndEvent(string Message)
		{
			Log(Message);
			// Channel/broker property used to enable enhanced logging
			DrvSELoggerChannel broker = (DrvSELoggerChannel)Channel;
			if (broker.DBChannel.EnhancedEvents)
			{
				App.SendReceiveObject(this.DBScanner.Id, OPCProperty.SendRecLogFDEventText, Message);
			}
		}

		public override SourceStatus OnDefine()
        {
			SetScanRate(	DBScanner.NormalScanRate * 1000,
							DBScanner.NormalScanOffset);

			LogAndEvent("Scanner defined");

            ((DrvSELoggerChannel)Channel).AddScannerToIndex(this);

			// Read last sample id and retrieval time here from a data item on the server.
			// Date/time is for future reference and does not affect retrieval
			//object[] LastDataParams = new object[2];
			object DataRef = null;

			App.SendReceiveObject(this.DBScanner.Id, OPCProperty.SendRecGetLastDataTime, "", ref DataRef);
			if (DataRef != null)
			{
				object[] LastDataParams = (object[])DataRef;
				DBScanner.LastDataTime = (DateTime)LastDataParams[0];
				DBScanner.LastSampleId = (long)LastDataParams[1];
			}
			return SourceStatus.Online;
        }

		public override void OnScan()
		{
			DrvSELoggerChannel channel = (DrvSELoggerChannel)Channel;

			var tokenTask = RestAPITools.FetchOrValidateAccessToken(channel.DBChannel.AuthURL, channel.DBChannel.APIKey, channel.DBChannel.APISecret);
			tokenTask.Wait();
			channel.tokenStr = tokenTask.Result;

			// This is used to pick up and process data
			LogAndEvent("OnScan.");

			SampleStoredSampleScalarResponseCollection response;
			do
			{
				response = null;

				// We now use a SampleId number which the server uses to keep track of what has been downloaded per site
				try
				{
					long? LastSampleId = this.DBScanner.LastSampleId;
					// Database object initialises this to 0
					if (LastSampleId == 0)
					{
						// If zero then we should call with NULL which tells the server we have not got data for this site before.
						// But API declares the minimum to be 1, could be an error in the API definition, or intentional.
						LastSampleId = 1;
					}
					response = new SampleApi().SampleGetSamplesScalarBySite(DBScanner.SiteId, $"Bearer {channel.tokenStr}", LastSampleId);
				}
				catch (ApiException e)
				{
					LogAndEvent("API Exception: " + e.ErrorCode.ToString() + " " + e.Message);
					return;
				}

				if (response != null)
				{
					long LastSampleId;
					DateTime LastDataTime = ProcessMetrics(response, out LastSampleId);
					// Write to the scanner and send to the server
					this.DBScanner.LastDataTime = LastDataTime;
					this.DBScanner.LastSampleId = (long)LastSampleId;
					object[] LastDataParams = new object[2];
					LastDataParams[0] = LastDataTime;
					LastDataParams[1] = LastSampleId;
					App.SendReceiveObject(this.DBScanner.Id, OPCProperty.SendRecUpdateLastDataTime, LastDataParams);
				}

				/*
				 * If the batch consists of over 10,000 samples, it is broken down into chunks of no more than 10,000
				 * samples each, sent consecutively.
				 * API Documentation states 100 though.
				 * So we cause another read if there are more than 99 samples in the last call.
				 */
			}
			while (response != null && response.Count > 99);
		}


		public override void OnUnDefine()
        {
            ((DrvSELoggerChannel)Channel).RemoveScannerFromIndex(this);
            base.OnUnDefine();
        }

		public override void OnExecuteAction(DriverTransaction Transaction)
		{
			LogAndEvent("Driver Action - scanner.");
			switch (Transaction.ActionType)
			{

				default:
					base.OnExecuteAction(Transaction);
					break;
			}
		}

		public DateTime ProcessMetrics(SampleStoredSampleScalarResponseCollection response, out long LastSampleId)
		{
			bool DataReceived = false;
			LastSampleId = 0;
			DateTime LastDataTime = DateTime.UtcNow.AddDays(-365); // Default oldest date of data

			if (response != null)
			{
				/*
				 * Printing Out the new Samples
				 */
				foreach (var sample in response )
				{
					// Change to log
					LogAndEvent($"Found New Sample: ID: { sample.Id}, Value: { sample.Value}, SampleDate: {sample.SampleDate}, StreamId: {sample.StreamId}");
					DataReceived = true;

					// Process
					ProcessPointByNumber( (int)sample.StreamId, sample.Value, sample.SampleDate);
					if (sample.SampleDate > LastDataTime)
					{
						LastDataTime = (sample.SampleDate is null) ? DateTime.UtcNow : (DateTime)sample.SampleDate;
					}
					LastSampleId = (long)sample.Id;
				}
			}

			if (DataReceived)
			{
				LogAndEvent("Flushing buffered updates.");
				this.FlushUpdates(); // Write to server - buffering this so only called once per device/time
			}
			return LastDataTime;
		}


		// Point data processing
		// By type/number
		public void ProcessPointByNumber(int pointnum, string Value, DateTime? DataTime)
        {
            // Found point is:
            PointSourceEntry FoundPoint = null;

            // Find point by WITS ID
            foreach (PointSourceEntry Entry in Points)
            {
                if (Entry.PointType == typeof(SELoggerPointDg) )
                {
                    SELoggerPointDg PointDg = (SELoggerPointDg)Entry.DatabaseObject;
                    if (PointDg.StreamId == (int)pointnum )
                    {
                        FoundPoint = Entry;
                        break;
                    }
                }
				if (Entry.PointType == typeof(SELoggerPointAg) )
				{
					SELoggerPointAg PointAg = (SELoggerPointAg)Entry.DatabaseObject;
					if (PointAg.StreamId == (int)pointnum )
					{
						FoundPoint = Entry;
						break;
					}
				}
				if (Entry.PointType == typeof(SELoggerPointCi) )
				{
					SELoggerPointCi PointCi = (SELoggerPointCi)Entry.DatabaseObject;
					if (PointCi.StreamId == (int)pointnum)
					{
						FoundPoint = Entry;
						break;
					}
				}
				if (Entry.PointType == typeof(SELoggerPointTm))
				{
					SELoggerPointTm PointTm = (SELoggerPointTm)Entry.DatabaseObject;
					if (PointTm.StreamId == (int)pointnum)
					{
						FoundPoint = Entry;
						break;
					}
				}
			}

			if (FoundPoint != null)
            {
                ProcessPointData(FoundPoint, Value, DataTime);
            }
        }


        public void ProcessPointData(PointSourceEntry FoundPoint, string Value, DateTime? DataTime)
        {
			string ProcLogText = "";

			ProcLogText += "Point: " + FoundPoint.FullName + " value: " + Value;

			// Quality
			PointSourceEntry.Quality quality = PointSourceEntry.Quality.Good;

            // Reason
            PointSourceEntry.Reason reason = PointSourceEntry.Reason.Report;

			LogAndEvent( ProcLogText);

			DateTime DataTimeValue = (DataTime is null) ? DateTime.UtcNow : (DateTime) DataTime;

			// Pick value type appropriate to point
			FoundPoint.SetValue( DataTimeValue,
								 quality,
								 reason,
								 Value);
			// this.FlushUpdates(); // Write to server - could defer this until all points have been processed - more efficient.
		}
	}
}

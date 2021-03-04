using System;
using ClearSCADA.DBObjFramework;


[assembly:Category("SE Logger")]
[assembly:Description("SE Logger Driver")]
[assembly:DriverTask("DriverSELogger.exe")]

[System.ComponentModel.RunInstaller(true)]
public class CSInstaller :  DriverInstaller
{
}

namespace SELogger
{
    public class CSharpModule : DBModule
    {
    }

    [Table("SE Logger Connection", "SELogger")]
	[EventCategory("SELoggerConnection", "SE Logger Connection", OPCProperty.Base + 0)]
    public class SELoggerChannel : Channel
    {
        [AlarmCondition("SELoggerChannelConnectionAlarm", "SELoggerConnection", 0x03505041)]
        [AlarmSubCondition("SELoggerChannelCommError")]
        [AlarmSubCondition("SELoggerChannelNewDevice")]
        [AlarmSubCondition("SELoggerChannelDeviceConfig")]
        public Alarm SELoggerChannelAlarm;

        [Label("In Service", 1, 1)]
        [ConfigField("In Service", "Controls whether the channel is active.", 1, 2, 0x0350501B)]
        public override Boolean InService
        {
            get
            {
                return base.InService;
            }
            set
            {
                base.InService = value;
            }
        }

        [Label("Enhanced Events", 1, 3)]
		[ConfigField("Enhanced Events", "Controls whether debug messages are sent to the event log. (Do not use long-term).", 1, 4, OPCProperty.Base + 83)]
		public Boolean EnhancedEvents = false;

		[Label("Severity", 2, 1)]
        [ConfigField("Severity", "Severity", 2, 2, 0x0350501C)]
        public override ushort Severity
        {
            get
            {
                return base.Severity;
            }
            set
            {
                base.Severity = value;
            }
        }

		[Label("Area Of Interest", 3, 1, AreaOfInterest = true)]
        [ConfigField("Area Of Interest", "Reference to the Area Of Interest in which alarms & events on this object occur.", 3, 2, 0x03600D00)]
        public override AOIReference AreaOfInterestIdBase
        {
            get
            {
                return base.AreaOfInterestIdBase;
            }
            set
            {
                base.AreaOfInterestIdBase = value;
            }
        }

        [ConfigField("Area Of Interest", "Name of the Area Of Interest in which alarms & events on this object occur.", 3, 4, 0x03600D04, ReadOnly = true, Length = 48, Flags = FormFlags.Hidden)]
        public String AreaOfInterestBase
        {
            get { return AreaOfInterestIdBase.Name; }
        }

        [Label("Read Interval", 2, 3)]
        [ConfigField("ScanRate", "Configuration read interval.", 2, 4, 0x03505045)]
        [Interval(IntervalType.Seconds)]
        public UInt32 ConfigReadRate = 3600;

        [Label("Base URL", 4, 1)]
        [ConfigField("BaseURL",
                     "The address of the web service.",
                     4, 2, OPCProperty.Base + 1, Length = 120)]
        public String BaseURL = "https://ecostruxure-process-instrument-datalogger-api.se.app/v2.0";

        [Label("Authorization URL", 4, 3)]
        [ConfigField("AuthURL",
                     "The address of the authorization service.",
                     4, 4, OPCProperty.Base + 53, Length = 120)]
        public String AuthURL = "https://ecostruxure-process-instrument-datalogger-api.se.app/auth/token"; 

		[Label("API Key", 5, 1)]
        [ConfigField("APIKey",
                     "The Key for the API, retrieved from the web UI.",
                     5, 2, OPCProperty.Base + 54, Length = 120)]
        public String APIKey;

        [Label("API Secret", 5, 3)]
        [ConfigField("APISecret",
                     "The secret for the API key.",
                     5, 4, OPCProperty.Base + 55, Length = 120, Flags = FormFlags.Password)]
        public String APISecret;

        // Need to modify the below if there is a change to the API version
        [Label("API Version", 7, 1)]
        [ConfigField("APIVersion",
                        "The version of the SELogger protocol.",
                        7, 2, OPCProperty.Base + 110)]
        [Enum(new String[] { "2.0" })]
        public Byte APIVersion = 0;

        [Label("Organization Name", 8, 1)]
        [ConfigField("OrganizationName",
                        "The name of the organization (Leave blank to retrieve all available).",
                        8, 2, OPCProperty.Base + 111, Length = 80)]
        public String OrganizationName;

        [Label("Maximum Days to Retrieve", 9, 1)]
        [ConfigField("MaxRetrieveDays",
                     "Maximum days to retrieve data.",
                     9, 2, OPCProperty.Base + 112)]
        public UInt32 MaxRetrieveDays = 28;
        
        [Label("Automatic Configure", 10, 1)]
        [ConfigField("AutoConfig",
                     "Automatically configure unknown devices and their points.",
                     10, 2, OPCProperty.Base + 2)]
        public Boolean AutoConfig;

        [Label("Automatic Reconfigure", 10, 3)]
        [ConfigField("AutoReconfig",
                     "Automatically reconfigure known devices and point properties and new points.",
                     10, 4, OPCProperty.Base + 92)]
        public Boolean AutoReconfig;

        [Label("Server Port", 11, 1)]
        [ConfigField("ServerPort",
                     "Port for client to make configuration changes.",
                     11, 2, OPCProperty.Base + 34)]
        public UInt32 ServerPort = 5481;

        [Label("Configuration User Name", 12, 1)]
        [ConfigField("ConfigUserName",
                     "Geo SCADA User needed to log in and make configuration change.",
                     12, 2, OPCProperty.Base + 38, Length = 32)]
		public String ConfigUserName;

        [Label("Configuration Password", 13, 1)]
        [ConfigField("ConfigPass",
                     "Get SCADA User credential to log in and make configuration change.",
                     13, 2, OPCProperty.Base + 35, Length = 40, Flags = FormFlags.Password)]
        public String ConfigPass;

        [Label("Create Devices or Instances in Group", 14, 1)]
        [ConfigField("ConfigGroupId",
                     "Folder in which new sites are created automatically (you may wish this to be the Group Name).",
                     14, 2, OPCProperty.Base + 36, RefTables = "CGroup")]
        public Reference<DBObject> ConfigGroupId;

		[Label("Device Template", 15, 1)]
		[ConfigField("TemplateId",
			 "Device Template reference (optional). New device configurations will use this template",
			 15, 2, OPCProperty.Base + 37, RefTables = "CTemplate")]
		public Reference<DBObject> TemplateId;

        // To be maintained as a database item. This is an array, so can be understood by ViewX
        [DataField("ConfigBuf", "Buffer of pending device configuration", OPCProperty.Base + 9, ReadOnly = true)]
        public Int32[] ConfigBuf = new Int32[0];

        [DataField("PendingCount", "Count of pending devices for configuration", OPCProperty.Base + 41, ReadOnly =true, ViewInfoTitle = "Pending Devices for Configuration")]
        public Int32 PendingCount
        {
            get
            {
                return ConfigBuf.Length;
            }
        }

        [DataField("LastConfigId", "Next unconfigured device Device Id", OPCProperty.Base + 42, ReadOnly = true, ViewInfoTitle = "Next Device for Configuration")]
        public Int32 LastConfigId
        {
            get
            {
                if (ConfigBuf.Length > 0)
                {
                    return ConfigBuf[0];
                }
                else
                {
                    return 0;
                }
            }
        }


        public override void OnValidateConfig(MessageInfo Errors)
        {
            // Add more validations here
            if (APIKey == "")
            {
                Errors.Add(this, "APIKey", "API Key is blank.");
            }
            if (APISecret == "")
            {
                Errors.Add(this, "APISecret", "API Secret is blank.");
            }
            base.OnValidateConfig(Errors);
        }

        public override void OnReceive(uint Type, object Data, ref object Reply)
        {

            if (Type == OPCProperty.SendRecClearChannelAlarm)
            {
                if (SELoggerChannelAlarm.ActiveSubCondition == "SELoggerChannelCommError")
                {
                    SELoggerChannelAlarm.Clear();
                    SetDataModified(true);
                }
            }
            else if (Type == OPCProperty.SendRecRaiseChannelAlarm)
            {
                // If not already active subcondition AND uncleared.
                if ((SELoggerChannelAlarm.ActiveSubCondition != "SELoggerChannelCommError") && (SELoggerChannelAlarm.State != 4) && (SELoggerChannelAlarm.State != 2))
                {
                    SELoggerChannelAlarm.Raise("SELoggerChannelCommError", "SELogger Error: Web API Offline", Severity, true);
                    SetDataModified(true);
                }
            }
            else if (Type == OPCProperty.SendRecRequestConfiguration)
            {
                Int32 DeviceId = (Int32)Data;
                LogSystemEvent("SELoggerChannel", Severity, $"Request for configuration initiation from: {DeviceId}");
                SELoggerChannelAlarm.Raise("SELoggerChannelNewDevice", $"SELogger New Device Connected: {DeviceId}", Severity, true);
                // Raises an alarm which 'tells' user to check and then request config as a method action on the broker ConfigurePending
                SetDataModified(true);
            }
            else if (Type == OPCProperty.SendRecUpdateConfigQueue)
            {
				// All q items are refreshed by driver in one action whenever list changes.
				ConfigBuf = (Int32[])Data;
                SetDataModified(true);
            }
            else if (Type == OPCProperty.SendRecReportConfigError)
            {
                String Err = (String)Data;
                SELoggerChannelAlarm.Raise("SELoggerChannelDeviceConfig", "SELogger Error configuring device: " + Err, Alarm.AlarmType.Fleeting, Severity, true);
                SetDataModified(true);
            }
            else if (Type == OPCProperty.SendRecLogChannelEventText)
			{
				String Message = (String)Data;
				LogSystemEvent("SELoggerConnection", Severity, Message);
			}
			else
			{
                base.OnReceive(Type, Data, ref Reply);
            }
        }

		[Method("Configure Pending Devices", "Configure all devices which are pending", OPCProperty.Base + 32)]
        public void ConfigurePending()
        {
            // Queue a config request for each device, then the driver will delete the queue contents
            for (int i=0; i < ConfigBuf.Length; i++ )
            {
                object[] ArgObject = new Object[1];
                ArgObject[0] = ConfigBuf[i];
                DriverAction(OPCProperty.DriverActionInitiateConfig, ArgObject, "Initiate configuration of new Device: " + ConfigBuf[i]);
            }
        }

        [Method("Configure Next Pending Device", "Configure next devices which is pending", OPCProperty.Base + 96)]
        public void ConfigureNextPending()
        {
            if (ConfigBuf.Length > 0)
            {
                object[] ArgObject = new Object[1];
                ArgObject[0] = ConfigBuf[0];
                DriverAction(OPCProperty.DriverActionInitiateConfig, ArgObject, "Initiate configuration of new Device: " + ConfigBuf[0]);
            }
        }

        [Method("Retrieve Configuration", "Get latest configuration from API in the next connection poll.", OPCProperty.Base + 113)]
        public void RetrieveConfiguration()
        {
                object[] ArgObject = new Object[1];
                ArgObject[0] = "";
                DriverAction(OPCProperty.DriverActionRetrieveConfig, ArgObject, "Retrieve Configuration" );
        }

    }

    [Table("SE Logger Device", "SELogger")]
    [EventCategory("SELoggerDevice", "SE Logger Device", OPCProperty.Base + 3)]
	[EventCategory("SELoggerDeviceDbg", "SELogger Device Debug", OPCProperty.Base + 82)]
	public class SELoggerDevice : Scanner
    {
        [AlarmCondition("SELoggerDeviceScannerAlarm", "SELoggerDevice", 0x0350532F)]
		[AlarmSubCondition("SELoggerCommSeq")]
		[AlarmSubCondition("SELoggerCommLWT")]
        public Alarm SELoggerScannerAlarm;

        [AlarmCondition("SELoggerDeviceConfigAlarm", "SELoggerDevice", OPCProperty.Base + 19)]
        [AlarmSubCondition("SELoggerNodeIdError")]
        [AlarmSubCondition("SELoggerConfigVerError")]
        public Alarm SELoggerConfigAlarm;

        [Label("Enabled", 1, 1)]
        [ConfigField("In Service", 
                     "Controls whether service connection is active.",
                     1, 2, 0x350501B, DefaultOverride =true)]
        public override Boolean InService 
        {
            get 
            { 
                return base.InService;
            }
            set 
            { 
                base.InService = value;
            }
        }

        [Label("Severity", 2, 1)]
        [ConfigField("Severity", "Severity", 2, 2, 0x0350501C)]
        public override ushort Severity
        {
            get
            {
                return base.Severity;
            }
            set
            {
                base.Severity = value;
            }
		}

		[Label("Read Interval", 1, 3)] 
		[ConfigField("ScanRate", "Data Read Interval.", 1, 4, 0x03505045)] 
		[Interval(IntervalType.Seconds)]
		public UInt32 NormalScanRate = 600;

		[Label("Read Offset", 2, 3)]
		[ConfigField("ScanOffset", "Data read offset.", 2, 4, 0x0350504D, Length = 32)]
		public String NormalScanOffset = "M";

        [Label("Connection", 3, 1)]
        [ConfigField("Channel", "Connection Reference.", 3, 2, 0x03505041, DefaultOverride = true)]
        public Reference<SELoggerChannel> ChannelId;

        [Label("Device Id", 4, 1)]
        [ConfigField("DeviceId",
                     "The device identification number.",
                     4, 2, OPCProperty.Base + 4, AlwaysOverride =true)]
        public Int32 DeviceId;

        [Label("Site Id", 5, 1)]
        [ConfigField("SiteId",
                     "The site identification number.",
                     5, 2, OPCProperty.Base + 93, AlwaysOverride = true)]
        public Int32 SiteId;

        [Label("Area of Interest", 6, 1, AreaOfInterest = true)]
        [ConfigField("AOI Ref", "A reference to an AOI.", 6, 2, 0x0465700E)]
        public AOIReference AOIRef;

        [ConfigField("AOI Name", "A reference to an AOI.", 6, 3, 0x0465700F,
                     ReadOnly = true, Length = 48, Flags = FormFlags.Hidden)]
        public String AOIName
        {
            get { return AOIRef.Name; }
        }

        [Label("Organization Id", 7, 1)]
        [ConfigField("OrganizationId",
                     "The site organization number.",
                     7, 2, OPCProperty.Base + 84, AlwaysOverride = true)]
        public Int32 OrganizationId;

        [Label("Site Display Name", 8, 1)]
        [ConfigField("SiteDisplayName",
                     "The site's display name.",
                     8, 2, OPCProperty.Base + 85, Length = 80, AlwaysOverride = true)]
        public string SiteDisplayName;

        [Label("Device Display Name", 9, 1)]
        [ConfigField("DeviceDisplayName",
                     "The device's display name.",
                     9, 2, OPCProperty.Base + 86, Length = 80, AlwaysOverride = true)]
        public string DeviceDisplayName;

        [Label("Model Number", 10, 1)]
        [ConfigField("ModelNumber",
                     "The device's model number.",
                     10, 2, OPCProperty.Base + 87, Length = 80, AlwaysOverride = true)]
        public string ModelNumber;

        [Label("Serial Number", 11, 1)]
        [ConfigField("SerialNumber",
                     "The device's serial number.",
                     11, 2, OPCProperty.Base + 88, Length = 80, AlwaysOverride = true)]
        public string SerialNumber;

        [Label("Firmware Version", 12, 1)]
        [ConfigField("FirmwareVersion",
                     "The device's firmware version.",
                     12, 2, OPCProperty.Base + 100, Length = 80, AlwaysOverride = true)]
        public string FirmwareVersion;

        [Label("AKID", 13, 1)]
        [ConfigField("AKID",
                     "The device's AKID.",
                     13, 2, OPCProperty.Base + 101, Length = 80, AlwaysOverride = true)]
        public string AKID;

        [Label("ICCID", 14, 1)]
        [ConfigField("ICCID",
                     "The device's ICCID.",
                     14, 2, OPCProperty.Base + 102, Length = 80, AlwaysOverride = true)]
        public string ICCID;

        [Label("ICCID2", 15, 1)]
        [ConfigField("ICCID2",
                     "The device's ICCID2.",
                     15, 2, OPCProperty.Base + 103, Length = 80, AlwaysOverride = true)]
        public string ICCID2;

        [Label("MEID", 16, 1)]
        [ConfigField("MEID",
                     "The device's MEID.",
                     16, 2, OPCProperty.Base + 104, Length = 80, AlwaysOverride = true)]
        public string MEID;


        [DataField("Last Data Time",
           "The date/time of the last retrieved value, values older than this will not be retrieved.",
           OPCProperty.Base + 97, ViewInfoTitle = "Last retrieved date")]
        public DateTime LastDataTime = DateTime.UtcNow.AddDays(-366); // Default to a year ago, this is brought forward by the channel property MaxRetrieveDays.

        [DataField("Last Error",
                   "The text of the last error.",
                   OPCProperty.Base + 7, ViewInfoTitle = "Last Error")]
        public String ErrMessage;

		[ConfigField("ConfigChecksum", "Checksum/hash of the Device Config", 1, 1, OPCProperty.Base + 96, Flags = FormFlags.Hidden)]
		public string ConfigChecksum = "";

		public override void OnValidateConfig(MessageInfo Errors)
        {
			// ToDo - e.g. check DeviceId 
			if (DeviceId == 0)
			{
				Errors.Add(this, "DeviceId", "Device Id must not be zero.");
			}
            base.OnValidateConfig(Errors);
        }

		public override void OnReceive(uint Type, object Data, ref object Reply)
        {
            // Clear scanner alarm
            if (Type == OPCProperty.SendRecClearScannerAlarm)
            {
                SELoggerScannerAlarm.Clear();
                SetDataModified(true);
            }
            // Set scanner alarm
            else if (Type == OPCProperty.SendRecRaiseScannerAlarm)
            {
                SELoggerScannerAlarm.Raise("SELoggerCommLWT", "SELoggerCommError: Offline Received", Severity, true);
                SetDataModified(true);
            }
			// Recieve checksum data (configuration) for known device
			else if (Type == OPCProperty.SendRecSetChecksum)
            {
				// Data is a config string
				ConfigChecksum = (String)(Data);
				LogSystemEvent("SELoggerDevice", Severity, "Received device and point configuration checksum.");

				// Scanner good, Clear alarm
				SELoggerScannerAlarm.Clear();
				SetDataModified(true);
			}
			else if (Type == OPCProperty.SendRecFDProtocolError)
            {
                // Error interpreting a message - Log Event (may upgrade to alarm later?)
                LogSystemEvent("SELoggerDevice", Severity, (string)Data);
            }
			else if (Type == OPCProperty.SendRecLogFDEventText)
			{
				// General debug message raised as event
				String Message = (String)Data;
				LogSystemEvent("SELoggerDevice", Severity, Message);
			}
            else if (Type == OPCProperty.SendRecUpdateLastDataTime)
            {
                LastDataTime = (DateTime)Data;
                SetDataModified(true);
            }
            else if (Type == OPCProperty.SendRecGetLastDataTime)
            {
                Reply = LastDataTime;
            }
            else
                base.OnReceive(Type, Data, ref Reply);
        }

	}

    [Table("SE Logger Digital", "SELogger")]
	public class SELoggerPointDg : DigitalPoint
	{
        // Derived configuration field
        // This flag is stored and saved in the base class
        // To configure it it needs to be exposed by an override
        [Label("In Service", 1, 1)]
        [ConfigField("In Service",
        "Controls whether the point is active.",
        1, 2, 0x0350501B)]
        public override bool InService
        {
            get
            {
                return base.InService;
            }
            set
            {
                base.InService = value;
            }
        }
        
        [Label("Device", 2, 1)]
        [ConfigField("Scanner", "Scanner", 2, 2, 0x0350532F)]
        public new Reference<SELoggerDevice> ScannerId
		{
			get
			{
                return new Reference<SELoggerDevice>(base.ScannerId);
			}
			set
			{
				base.ScannerId = new Reference<Scanner>(value);
			}
		}

        // Common per type
        [Label("Display Name", 3, 1)]
        [ConfigField("DisplayName",
                     "The point's display name.",
                     3, 2, OPCProperty.Base + 89, Length = 80, AlwaysOverride = true)]
        public string DisplayName;

		[Label("StreamId", 3, 3)]
		[ConfigField("StreamId",
					 "The stream identification number.",
					 3, 4, OPCProperty.Base + 6, DefaultOverride = true)]
		public Int32 StreamId;

        [Label("SiteId", 3, 5)]
        [ConfigField("SiteId",
                     "The site identification number.",
                     3, 6, OPCProperty.Base + 90, DefaultOverride = true)]
        public Int32 SiteId;

        // Read-only data field containing a type Name
        [Label("Type Id", 10, 1)]
        [ConfigField("TypeId", "Point Type Number", 10, 2, OPCProperty.Base + 72, DefaultOverride = true)]
        public Int32 TypeId;

        [Label("Type Name", 10, 3)]
        [ConfigField("LoggerTypeName", "Point Type Name", 10, 4, OPCProperty.Base + 91, Length = 25, DefaultOverride = true)]
        public string LoggerTypeName;


        // Group of configuration fields
        // Normally an attribute on the first config field of the group
        [Label("Historic Data Filter", 4, 1, Width=2, Height=4)]
        // Historic filter is indexed property
        // Expose each item as a single tick box
        [Label("Significant Change", 5, 1)]
        [ConfigField("Report Historic Filter",
                    "Controls whether Significant Value Changes are logged historically",
                    5, 2, 0x3506913 )]
        public bool HistoricFilterValue
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.ValueChange);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.ValueChange, value);
            }
        }
        [Label("State Change", 6, 1)]
        [ConfigField("Report Historic Filter",
                    "Controls whether State Changes are logged historically",
                    6, 2, 0x3506914 )]
        public bool HistoricFilterState
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.StateChange);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.StateChange, value);
            }
        }
        [Label("Report", 7, 1)]
        [ConfigField("Report Historic Filter",
                "Controls whether Timed Report values are logged historically",
                7, 2, 0x3506915 )]
        public bool HistoricFilterReport
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.Report);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.Report, value);
            }
        }
        [Label("End of Period Report", 8, 1)]
        [ConfigField("EOP Historic Filter",
                "Controls whether End of Period values are logged historically",
                8, 2, 0x3506916 )]
        public bool HistoricFilterEOP
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.EndofPeriod);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.EndofPeriod, value);
            }
        }


		public override void OnConfigChange(ConfigEvent Event, MessageInfo Errors, DBObject OldObject)
		{
			base.OnConfigChange(Event, Errors, OldObject);
		}

		public override void OnValidateConfig(MessageInfo Errors)
		{
			//ToDo Check topics not empty

			//Errors.Add("A config error messsage.");
			base.OnValidateConfig(Errors);
		}
	}


	[Table("SE Logger Analogue", "SELogger")]
	public class SELoggerPointAg : AnaloguePoint
	{
        // Derived configuration field
        // This flag is stored and saved in the base class
        // To configure it it needs to be exposed by an override
        [Label("In Service", 1, 1)]
        [ConfigField("In Service",
        "Controls whether the point is active.",
        1, 2, 0x0350501B)]
        public override bool InService
        {
            get
            {
                return base.InService;
            }
            set
            {
                base.InService = value;
            }
        }

        [Label("Device", 2, 1)]
        [ConfigField("Scanner", "Scanner", 2, 2, 0x0350532F)]
        public new Reference<SELoggerDevice> ScannerId
        {
            get
            {
                return new Reference<SELoggerDevice>(base.ScannerId);
            }
            set
            {
                base.ScannerId = new Reference<Scanner>(value);
            }
        }

        // Common per type
        [Label("Display Name", 3, 1)]
        [ConfigField("DisplayName",
                     "The point's display name.",
                     3, 2, OPCProperty.Base + 89, Length = 80, AlwaysOverride = true)]
        public string DisplayName;

        [Label("StreamId", 3, 3)]
        [ConfigField("StreamId",
                     "The stream identification number.",
                     3, 4, OPCProperty.Base + 6, DefaultOverride = true)]
        public Int32 StreamId;

        [Label("SiteId", 3, 5)]
        [ConfigField("SiteId",
                     "The site identification number.",
                     3, 6, OPCProperty.Base + 90, DefaultOverride = true)]
        public Int32 SiteId;

        // Read-only data field containing a type Name
        [Label("Type Id", 10, 1)]
        [ConfigField("TypeId", "Point Type Number", 10, 2, OPCProperty.Base + 72, DefaultOverride = true)]
        public Int32 TypeId;

        [Label("Type Name", 10, 3)]
        [ConfigField("LoggerTypeName", "Point Type Name", 10, 4, OPCProperty.Base + 91, Length = 25, DefaultOverride = true)]
        public string LoggerTypeName;

        // Group of configuration fields
        // Normally an attribute on the first config field of the group
        [Label("Historic Data Filter", 4, 1, Width = 2, Height = 4)]
        // Historic filter is indexed property
        // Expose each item as a single tick box
        [Label("Significant Change", 5, 1)]
        [ConfigField("Report Historic Filter",
                    "Controls whether Significant Value Changes are logged historically",
                    5, 2, 0x3506913)]
        public bool HistoricFilterValue
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.ValueChange);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.ValueChange, value);
            }
        }
        [Label("State Change", 6, 1)]
        [ConfigField("Report Historic Filter",
                    "Controls whether State Changes are logged historically",
                    6, 2, 0x3506914)]
        public bool HistoricFilterState
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.StateChange);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.StateChange, value);
            }
        }
        [Label("Report", 7, 1)]
        [ConfigField("Report Historic Filter",
                "Controls whether Timed Report values are logged historically",
                7, 2, 0x3506915)]
        public bool HistoricFilterReport
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.Report);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.Report, value);
            }
        }
        [Label("End of Period Report", 8, 1)]
        [ConfigField("EOP Historic Filter",
                "Controls whether End of Period values are logged historically",
                8, 2, 0x3506916)]
        public bool HistoricFilterEOP
        {
            get
            {
                return base.get_HistoricFilter(PointSourceEntry.Reason.EndofPeriod);
            }
            set
            {
                base.set_HistoricFilter(PointSourceEntry.Reason.EndofPeriod, value);
            }
        }

		public override void OnConfigChange(ConfigEvent Event, MessageInfo Errors, DBObject OldObject)
		{
			base.OnConfigChange(Event, Errors, OldObject);
		}

		public override void OnValidateConfig(MessageInfo Errors)
		{
			//ToDo Check topics not empty

			//Errors.Add("A config error messsage.");
			base.OnValidateConfig(Errors);
		}
	}


	[Table("SE Logger Time", "SELogger")]
	public class SELoggerPointTm : TimePoint
	{
		// Derived configuration field
		// This flag is stored and saved in the base class
		// To configure it it needs to be exposed by an override
		[Label("In Service", 1, 1)]
		[ConfigField("In Service",
		"Controls whether the point is active.",
		1, 2, 0x0350501B)]
		public override bool InService
		{
			get
			{
				return base.InService;
			}
			set
			{
				base.InService = value;
			}
		}

		[Label("Device", 2, 1)]
		[ConfigField("Scanner", "Scanner", 2, 2, 0x0350532F)]
		public new Reference<SELoggerDevice> ScannerId
		{
			get
			{
				return new Reference<SELoggerDevice>(base.ScannerId);
			}
			set
			{
				base.ScannerId = new Reference<Scanner>(value);
			}
		}

        // Common per type
        [Label("Display Name", 3, 1)]
        [ConfigField("DisplayName",
                     "The point's display name.",
                     3, 2, OPCProperty.Base + 89, Length = 80, AlwaysOverride = true)]
        public string DisplayName;

        [Label("StreamId", 3, 3)]
        [ConfigField("StreamId",
                     "The stream identification number.",
                     3, 4, OPCProperty.Base + 6, DefaultOverride = true)]
        public Int32 StreamId;

        [Label("SiteId", 3, 5)]
        [ConfigField("SiteId",
                     "The site identification number.",
                     3, 6, OPCProperty.Base + 90, DefaultOverride = true)]
        public Int32 SiteId;

        // Read-only data field containing a type Name
        [Label("Type Id", 10, 1)]
        [ConfigField("TypeId", "Point Type Number", 10, 2, OPCProperty.Base + 72, DefaultOverride = true)]
        public Int32 TypeId;

        [Label("Type Name", 10, 3)]
        [ConfigField("LoggerTypeName", "Point Type Name", 10, 4, OPCProperty.Base + 91, Length = 25, DefaultOverride = true)]
        public string LoggerTypeName;

        // Group of configuration fields
        // Normally an attribute on the first config field of the group
        [Label("Historic Data Filter", 4, 1, Width = 2, Height = 4)]
		// Historic filter is indexed property
		// Expose each item as a single tick box
		[Label("Significant Change", 5, 1)]
		[ConfigField("Report Historic Filter",
					"Controls whether Significant Value Changes are logged historically",
					5, 2, 0x3506913)]
		public bool HistoricFilterValue
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.ValueChange);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.ValueChange, value);
			}
		}
		[Label("State Change", 6, 1)]
		[ConfigField("Report Historic Filter",
					"Controls whether State Changes are logged historically",
					6, 2, 0x3506914)]
		public bool HistoricFilterState
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.StateChange);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.StateChange, value);
			}
		}
		[Label("Report", 7, 1)]
		[ConfigField("Report Historic Filter",
				"Controls whether Timed Report values are logged historically",
				7, 2, 0x3506915)]
		public bool HistoricFilterReport
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.Report);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.Report, value);
			}
		}
		[Label("End of Period Report", 8, 1)]
		[ConfigField("EOP Historic Filter",
				"Controls whether End of Period values are logged historically",
				8, 2, 0x3506916)]
		public bool HistoricFilterEOP
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.EndofPeriod);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.EndofPeriod, value);
			}
		}

		public override void OnConfigChange(ConfigEvent Event, MessageInfo Errors, DBObject OldObject)
		{
			base.OnConfigChange(Event, Errors, OldObject);
		}

		public override void OnValidateConfig(MessageInfo Errors)
		{
			//ToDo Check topics not empty

			//Errors.Add("A config error messsage.");
			base.OnValidateConfig(Errors);
		}
	}

	[Table("SE Logger Counter", "SELogger")]
	public class SELoggerPointCi : CounterPoint
	{
		// Derived configuration field
		// This flag is stored and saved in the base class
		// To configure it it needs to be exposed by an override
		[Label("In Service", 1, 1)]
		[ConfigField("In Service",
		"Controls whether the point is active.",
		1, 2, 0x0350501B)]
		public override bool InService
		{
			get
			{
				return base.InService;
			}
			set
			{
				base.InService = value;
			}
		}

		[Label("Device", 2, 1)]
		[ConfigField("Scanner", "Scanner", 2, 2, 0x0350532F)]
		public new Reference<SELoggerDevice> ScannerId
		{
			get
			{
				return new Reference<SELoggerDevice>(base.ScannerId);
			}
			set
			{
				base.ScannerId = new Reference<Scanner>(value);
			}
		}

        // Common per type
        [Label("Display Name", 3, 1)]
        [ConfigField("DisplayName",
                     "The point's display name.",
                     3, 2, OPCProperty.Base + 89, Length = 80, AlwaysOverride = true)]
        public string DisplayName;

        [Label("StreamId", 3, 3)]
        [ConfigField("StreamId",
                     "The stream identification number.",
                     3, 4, OPCProperty.Base + 6, DefaultOverride = true)]
        public Int32 StreamId;

        [Label("SiteId", 3, 5)]
        [ConfigField("SiteId",
                     "The site identification number.",
                     3, 6, OPCProperty.Base + 90, DefaultOverride = true)]
        public Int32 SiteId;

        // Read-only data field containing a type Name
        [Label("Type Id", 10, 1)]
        [ConfigField("TypeId", "Point Type Number", 10, 2, OPCProperty.Base + 72, DefaultOverride = true)]
        public Int32 TypeId;

        [Label("Type Name", 10, 3)]
        [ConfigField("LoggerTypeName", "Point Type Name", 10, 4, OPCProperty.Base + 91, Length = 25, DefaultOverride = true)]
        public string LoggerTypeName;


        // Group of configuration fields
        // Normally an attribute on the first config field of the group
        [Label("Historic Data Filter", 4, 1, Width = 2, Height = 4)]
		// Historic filter is indexed property
		// Expose each item as a single tick box
		[Label("Significant Change", 5, 1)]
		[ConfigField("Report Historic Filter",
					"Controls whether Significant Value Changes are logged historically",
					5, 2, 0x3506913)]
		public bool HistoricFilterValue
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.ValueChange);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.ValueChange, value);
			}
		}
		[Label("State Change", 6, 1)]
		[ConfigField("Report Historic Filter",
					"Controls whether State Changes are logged historically",
					6, 2, 0x3506914)]
		public bool HistoricFilterState
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.StateChange);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.StateChange, value);
			}
		}
		[Label("Report", 7, 1)]
		[ConfigField("Report Historic Filter",
				"Controls whether Timed Report values are logged historically",
				7, 2, 0x3506915)]
		public bool HistoricFilterReport
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.Report);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.Report, value);
			}
		}
		[Label("End of Period Report", 8, 1)]
		[ConfigField("EOP Historic Filter",
				"Controls whether End of Period values are logged historically",
				8, 2, 0x3506916)]
		public bool HistoricFilterEOP
		{
			get
			{
				return base.get_HistoricFilter(PointSourceEntry.Reason.EndofPeriod);
			}
			set
			{
				base.set_HistoricFilter(PointSourceEntry.Reason.EndofPeriod, value);
			}
		}

		public override void OnConfigChange(ConfigEvent Event, MessageInfo Errors, DBObject OldObject)
		{
			base.OnConfigChange(Event, Errors, OldObject);
		}

		public override void OnValidateConfig(MessageInfo Errors)
		{
			//ToDo Check topics not empty

			//Errors.Add("A config error messsage.");
			base.OnValidateConfig(Errors);
		}
	}
}

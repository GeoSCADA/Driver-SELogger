using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using IO.Swagger.Api;
using IO.Swagger.Client;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO.Swagger.Model;
using Newtonsoft.Json.Linq;

namespace RestAPIExample
{
    internal static class Program
    {
        private const string AUTH_URL = "https://ecostruxure-process-instrument-datalogger-api.se.app/auth/token";
        private const string BASE_URL = "https://ecostruxure-process-instrument-datalogger-api.se.app/v2.0";
        /// <summary>
        /// Fill here the right API key and secret
        /// </summary>
        private const string API_KEY = "903C5C7B32AA492DAD680DF2A7D4897F";
        private const string API_SECRET = "1ZgMTrPUQN+L+/WwOaCiPYRSmeVfr7UHkWp3xtbDCls=";



        public static async Task Main()
        {
            Configuration.Default.BasePath = BASE_URL;

            try
            {
                var tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET);

                // Allow a (Geo SCADA) channel to filter sites by Organisation
                await Console.Out.WriteLineAsync("Organizations");

                var organizations = await new AccountApi().AccountGetAllOrganizationsAsync($"Bearer {tokenStr}");

                if (organizations != null && organizations.Count > 0)
                {
                    foreach (var organization in organizations)
                    {
                        await Console.Out.WriteLineAsync($"Found Organization Id: {organization.Id}, DisplayName: { organization.DisplayName}");
                    }
                }

                // Get as extra info for 'scanner'/site
                // 
                await Console.Out.WriteLineAsync("Devices");
                var devices = await new DeviceApi().DeviceGetAllDevicesAsync($"Bearer {tokenStr}");

                if (devices != null && devices.Count > 0)
                {
                    foreach (var device in devices)
                    {
                        await Console.Out.WriteLineAsync($"Found Device Id: {device.Id}, DisplayName {device.DisplayName}, Site Id: {device.SiteId}, Model: { device.ModelNumber}, Serial: { device.SerialNumber}");

                        // What are health counterTypeId ?
                        /*
                        int DeviceChildIdDefunct = (int)device.Id;
                        var devicehealths = await new DevicehealthApi().DevicehealthGetLastDeviceHealthAsync(DeviceChildIdDefunct, $"Bearer {tokenStr}");

                        if (devicehealths != null && devicehealths.Count > 0)
                        {
                            foreach (var devicehealth in devicehealths)
                            {
                                await Console.Out.WriteLineAsync($"Found Device Health Id: {devicehealth.Id}, CounterTypeId {devicehealth.CounterTypeId}, Date: {devicehealth.SampleDate}, Value: { devicehealth.Value} ");
                            }
                        }
                        */
                    }
                }



                // Use these for the 'Scanner'
                await Console.Out.WriteLineAsync("Sites");
                var sites = await new SiteApi().SiteGetAllSitesAsync($"Bearer {tokenStr}");

                if (sites != null && sites.Count > 0)
                {
                    foreach (var site in sites)
                    {
                        await Console.Out.WriteLineAsync($"Found Site, Id: { site.Id}, Account Org Id: {site.AccountOrganizationId}, Status {site.Status}, DisplayName: { site.DisplayName}");
                    }
                }

                // Channels - probably not needed?
                await Console.Out.WriteLineAsync("Channels");
                var channels = await new ChannelApi().ChannelGetAllChannelsAsync($"Bearer {tokenStr}");

                if (channels != null && channels.Count > 0)
                {
                    foreach (var channel in channels)
                    {
                        await Console.Out.WriteLineAsync($"Found Channel Id: {channel.Id}, DisplayName: { channel.DisplayName}, Device ID: {channel.DeviceChildIdDefunct}, Type: {channel.TypeDisplayName}, Status: {channel.DisplayStatusName}");
                    }
                }

                // Stream - subset of channel, use for 'Point'
                await Console.Out.WriteLineAsync("Streams");
                var streams = await new StreamApi().StreamGetAllStreamsAsync($"Bearer {tokenStr}");

                if (streams != null && streams.Count > 0)
                {
                    foreach (var stream in streams)
                    {
                        await Console.Out.WriteLineAsync($"Found Stream: Id: {stream.Id}, Site: {stream.SiteId}, DisplayName: { stream.DisplayName}, TypeId: {stream.TypeId}, TypeDisplayName: {stream.TypeDisplayName}, Unit: {stream.Units}, Scale: {stream.ValueScale}");
                    }
                    
                }

                await Console.Out.WriteLineAsync("Samples");
                while (true)
                {
                    // ***********************************************************************************************
                    // SAMPLES
                    SampleGetSamplesScalarBatchResponseBody response = null;

                    var tokenRefreshed = false;
                    try
                    {
                        /*
                         * The first time you call this API, specify a sampleID or backfillHours to define the starting
                         * point from which to provide the batch of samples. In addition, set enableAck to true so that the
                         * system will not update its internal last delivered sample id field based on the last sample
                         * sent in this batch.
                         */
                        await Console.Out.WriteLineAsync($"Request Samples");
                        response = await new SampleApi().SampleGetSamplesScalarBatchAsync($"Bearer {tokenStr}", false, null, 2);
                    }
					catch (ApiException e)
                    {
                        if (e.ErrorCode == (int)HttpStatusCode.Unauthorized)
                        {

                            tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET, tokenStr);
                            response = null;
                            tokenRefreshed = true;
                        }
                        else
                        {
                            throw;
                        }
                        
                    }


                    if (response?.Samples != null)
                    {
                        /*
                         * Printing Out the new Samples
                         */
                        foreach (var sample in response.Samples)
                        {
                            await Console.Out.WriteLineAsync($"Found New Sample: ID: { sample.Id}, Value: { sample.Value}, SampleDate: {sample.SampleDate}, StreamId: {sample.StreamId}");
                        }
                    }
                    
                    /*
                     * If the batch consists of over 10,000 samples, it is broken down into chunks of no more than 10,000
                     * samples each, sent consecutively. The hasMore return flag indicates whether a given chunk is the
                     * last chunk.
                     */
                    if (tokenRefreshed || response == null || (response.HasMore.HasValue && !response.HasMore.Value))
                    {
                        //Going to sleep
                        Thread.Sleep(new TimeSpan(0,15,0));
                    }
                    

                    tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET, tokenStr);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception when calling RestAPI: " + e.Message );
                Environment.Exit(1);
            }
        }

        private static async Task<string> FetchOrValidateAccessToken(string apiKey, string apiSecret, string tokenStr = null)
        {
            
            if (string.IsNullOrEmpty(tokenStr))
            {
                tokenStr = await GetAccessToken(apiKey, apiSecret);
            }

            
            if (DateTime.UtcNow > new JwtSecurityTokenHandler().ReadJwtToken(tokenStr).ValidTo)
            {
                tokenStr = await GetAccessToken(apiKey, apiSecret);
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
        private static async Task<string> GetAccessToken(string apiKey, string apiSecret)
        {
            using (var client = new HttpClient())
            {
                
                client.BaseAddress = new Uri(AUTH_URL);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var result = await client.PostAsync(AUTH_URL, content);

                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed to receive access token: ret code {result.StatusCode}");
                
                var resultContent = await result.Content.ReadAsStringAsync();
                var json = JObject.Parse(resultContent);

                return (string) json["access_token"];
            }
        }
    }
}
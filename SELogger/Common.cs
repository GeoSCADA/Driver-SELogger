﻿using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace SELogger
{
	public class OPCProperty
	{
		public const UInt32 Base = 0x04691000;

		// Channel / Broker
		// RendRecs
		public const UInt32 SendRecClearChannelAlarm = OPCProperty.Base + 10;
		public const UInt32 SendRecRaiseChannelAlarm = OPCProperty.Base + 11;
		public const UInt32 SendRecRequestConfiguration = OPCProperty.Base + 31;
		public const UInt32 SendRecUpdateConfigQueue = OPCProperty.Base + 40;
		public const UInt32 SendRecReportConfigError = OPCProperty.Base + 56;
		public const UInt32 SendRecLogChannelEventText = OPCProperty.Base + 79;
		// Actions
		public const UInt32 DriverActionInitiateConfig = OPCProperty.Base + 33;
		public const UInt32 DriverActionRetrieveConfig = OPCProperty.Base + 113;

		// Scanner / FD
		// RendRecs
		public const UInt32 SendRecClearScannerAlarm = OPCProperty.Base + 12;
		public const UInt32 SendRecRaiseScannerAlarm = OPCProperty.Base + 13;
		public const UInt32 SendRecSetChecksum = OPCProperty.Base + 14;
		public const UInt32 SendRecProcessDeviceConfig = OPCProperty.Base + 30;
		public const UInt32 SendRecFDProtocolError = OPCProperty.Base + 43;
		public const UInt32 SendRecLogFDEventText = OPCProperty.Base + 80;
		public const UInt32 SendRecUpdateLastDataTime = OPCProperty.Base + 98;
		public const UInt32 SendRecGetLastDataTime = OPCProperty.Base + 99;
	}


	public class Util
	{

		// Convert an object to a byte array
		public static byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		// Convert a byte array to an Object
		public static Object ByteArrayToObject(byte[] arrBytes)
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binForm = new BinaryFormatter();
			memStream.Write(arrBytes, 0, arrBytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			Object obj = (Object)binForm.Deserialize(memStream);

			return obj;
		}


		private static byte[] GetHash(string inputString)
		{
			HashAlgorithm algorithm = SHA256.Create();
			return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		public static string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}


		public static double DateTimeToUnixTimeStamp(DateTime input)
		{
			// to seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			return input.Subtract(dtDateTime).TotalSeconds;
		}
		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}

		public static DateTime UnixTimeStampMillisToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
			return dtDateTime;
		}

	}
}

/* 
 * RESTAPI Service
 *
 * RESTful API
 *
 * OpenAPI spec version: 2.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// StoredDeviceLogResponse result type (default view)
    /// </summary>
    [DataContract]
    public partial class StoredDeviceLogResponse :  IEquatable<StoredDeviceLogResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredDeviceLogResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected StoredDeviceLogResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredDeviceLogResponse" /> class.
        /// </summary>
        /// <param name="deviceId">Device ID of device log (required).</param>
        /// <param name="id">ID is the unique id of the device log. (required).</param>
        /// <param name="logDate">Log of sample (required).</param>
        /// <param name="message">Message (required).</param>
        /// <param name="source">Source (required).</param>
        public StoredDeviceLogResponse(int? deviceId = default(int?), long? id = default(long?), DateTime? logDate = default(DateTime?), string message = default(string), string source = default(string))
        {
            // to ensure "deviceId" is required (not null)
            if (deviceId == null)
            {
                throw new InvalidDataException("deviceId is a required property for StoredDeviceLogResponse and cannot be null");
            }
            else
            {
                this.DeviceId = deviceId;
            }
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new InvalidDataException("id is a required property for StoredDeviceLogResponse and cannot be null");
            }
            else
            {
                this.Id = id;
            }
            // to ensure "logDate" is required (not null)
            if (logDate == null)
            {
                throw new InvalidDataException("logDate is a required property for StoredDeviceLogResponse and cannot be null");
            }
            else
            {
                this.LogDate = logDate;
            }
            // to ensure "message" is required (not null)
            if (message == null)
            {
                throw new InvalidDataException("message is a required property for StoredDeviceLogResponse and cannot be null");
            }
            else
            {
                this.Message = message;
            }
            // to ensure "source" is required (not null)
            if (source == null)
            {
                throw new InvalidDataException("source is a required property for StoredDeviceLogResponse and cannot be null");
            }
            else
            {
                this.Source = source;
            }
        }
        
        /// <summary>
        /// Device ID of device log
        /// </summary>
        /// <value>Device ID of device log</value>
        [DataMember(Name="device_id", EmitDefaultValue=false)]
        public int? DeviceId { get; set; }

        /// <summary>
        /// ID is the unique id of the device log.
        /// </summary>
        /// <value>ID is the unique id of the device log.</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long? Id { get; set; }

        /// <summary>
        /// Log of sample
        /// </summary>
        /// <value>Log of sample</value>
        [DataMember(Name="log_date", EmitDefaultValue=false)]
        public DateTime? LogDate { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        /// <value>Message</value>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        /// <value>Source</value>
        [DataMember(Name="source", EmitDefaultValue=false)]
        public string Source { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StoredDeviceLogResponse {\n");
            sb.Append("  DeviceId: ").Append(DeviceId).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  LogDate: ").Append(LogDate).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as StoredDeviceLogResponse);
        }

        /// <summary>
        /// Returns true if StoredDeviceLogResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of StoredDeviceLogResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StoredDeviceLogResponse input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.DeviceId == input.DeviceId ||
                    (this.DeviceId != null &&
                    this.DeviceId.Equals(input.DeviceId))
                ) && 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.LogDate == input.LogDate ||
                    (this.LogDate != null &&
                    this.LogDate.Equals(input.LogDate))
                ) && 
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) && 
                (
                    this.Source == input.Source ||
                    (this.Source != null &&
                    this.Source.Equals(input.Source))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.DeviceId != null)
                    hashCode = hashCode * 59 + this.DeviceId.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.LogDate != null)
                    hashCode = hashCode * 59 + this.LogDate.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.Source != null)
                    hashCode = hashCode * 59 + this.Source.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            // DeviceId (int?) minimum
            if(this.DeviceId < (int?)1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for DeviceId, must be a value greater than or equal to 1.", new [] { "DeviceId" });
            }

            // Message (string) maxLength
            if(this.Message != null && this.Message.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Message, length must be less than 256.", new [] { "Message" });
            }

            // Source (string) maxLength
            if(this.Source != null && this.Source.Length > 10)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Source, length must be less than 10.", new [] { "Source" });
            }

            yield break;
        }
    }

}

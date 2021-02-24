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
    /// StoredSampleRFIDResponseBody result type (default view)
    /// </summary>
    [DataContract]
    public partial class StoredSampleRFIDResponseBody :  IEquatable<StoredSampleRFIDResponseBody>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSampleRFIDResponseBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected StoredSampleRFIDResponseBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSampleRFIDResponseBody" /> class.
        /// </summary>
        /// <param name="endDate">End date (required).</param>
        /// <param name="gpsLocationInfo">gpsLocationInfo (required).</param>
        /// <param name="id">ID is the unique id of the sample RFID. (required).</param>
        /// <param name="sampleDate">Sample date (required).</param>
        /// <param name="startDate">Start date (required).</param>
        /// <param name="streamId">Stream ID (required).</param>
        /// <param name="tagIds">Tag IDs (required).</param>
        public StoredSampleRFIDResponseBody(DateTime? endDate = default(DateTime?), GPSResponseBody gpsLocationInfo = default(GPSResponseBody), long? id = default(long?), DateTime? sampleDate = default(DateTime?), DateTime? startDate = default(DateTime?), int? streamId = default(int?), string tagIds = default(string))
        {
            // to ensure "endDate" is required (not null)
            if (endDate == null)
            {
                throw new InvalidDataException("endDate is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.EndDate = endDate;
            }
            // to ensure "gpsLocationInfo" is required (not null)
            if (gpsLocationInfo == null)
            {
                throw new InvalidDataException("gpsLocationInfo is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.GpsLocationInfo = gpsLocationInfo;
            }
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new InvalidDataException("id is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.Id = id;
            }
            // to ensure "sampleDate" is required (not null)
            if (sampleDate == null)
            {
                throw new InvalidDataException("sampleDate is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.SampleDate = sampleDate;
            }
            // to ensure "startDate" is required (not null)
            if (startDate == null)
            {
                throw new InvalidDataException("startDate is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.StartDate = startDate;
            }
            // to ensure "streamId" is required (not null)
            if (streamId == null)
            {
                throw new InvalidDataException("streamId is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.StreamId = streamId;
            }
            // to ensure "tagIds" is required (not null)
            if (tagIds == null)
            {
                throw new InvalidDataException("tagIds is a required property for StoredSampleRFIDResponseBody and cannot be null");
            }
            else
            {
                this.TagIds = tagIds;
            }
        }
        
        /// <summary>
        /// End date
        /// </summary>
        /// <value>End date</value>
        [DataMember(Name="end_date", EmitDefaultValue=false)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or Sets GpsLocationInfo
        /// </summary>
        [DataMember(Name="gps_location_info", EmitDefaultValue=false)]
        public GPSResponseBody GpsLocationInfo { get; set; }

        /// <summary>
        /// ID is the unique id of the sample RFID.
        /// </summary>
        /// <value>ID is the unique id of the sample RFID.</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long? Id { get; set; }

        /// <summary>
        /// Sample date
        /// </summary>
        /// <value>Sample date</value>
        [DataMember(Name="sample_date", EmitDefaultValue=false)]
        public DateTime? SampleDate { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        /// <value>Start date</value>
        [DataMember(Name="start_date", EmitDefaultValue=false)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Stream ID
        /// </summary>
        /// <value>Stream ID</value>
        [DataMember(Name="stream_id", EmitDefaultValue=false)]
        public int? StreamId { get; set; }

        /// <summary>
        /// Tag IDs
        /// </summary>
        /// <value>Tag IDs</value>
        [DataMember(Name="tag_ids", EmitDefaultValue=false)]
        public string TagIds { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StoredSampleRFIDResponseBody {\n");
            sb.Append("  EndDate: ").Append(EndDate).Append("\n");
            sb.Append("  GpsLocationInfo: ").Append(GpsLocationInfo).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  SampleDate: ").Append(SampleDate).Append("\n");
            sb.Append("  StartDate: ").Append(StartDate).Append("\n");
            sb.Append("  StreamId: ").Append(StreamId).Append("\n");
            sb.Append("  TagIds: ").Append(TagIds).Append("\n");
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
            return this.Equals(input as StoredSampleRFIDResponseBody);
        }

        /// <summary>
        /// Returns true if StoredSampleRFIDResponseBody instances are equal
        /// </summary>
        /// <param name="input">Instance of StoredSampleRFIDResponseBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StoredSampleRFIDResponseBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EndDate == input.EndDate ||
                    (this.EndDate != null &&
                    this.EndDate.Equals(input.EndDate))
                ) && 
                (
                    this.GpsLocationInfo == input.GpsLocationInfo ||
                    (this.GpsLocationInfo != null &&
                    this.GpsLocationInfo.Equals(input.GpsLocationInfo))
                ) && 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.SampleDate == input.SampleDate ||
                    (this.SampleDate != null &&
                    this.SampleDate.Equals(input.SampleDate))
                ) && 
                (
                    this.StartDate == input.StartDate ||
                    (this.StartDate != null &&
                    this.StartDate.Equals(input.StartDate))
                ) && 
                (
                    this.StreamId == input.StreamId ||
                    (this.StreamId != null &&
                    this.StreamId.Equals(input.StreamId))
                ) && 
                (
                    this.TagIds == input.TagIds ||
                    (this.TagIds != null &&
                    this.TagIds.Equals(input.TagIds))
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
                if (this.EndDate != null)
                    hashCode = hashCode * 59 + this.EndDate.GetHashCode();
                if (this.GpsLocationInfo != null)
                    hashCode = hashCode * 59 + this.GpsLocationInfo.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.SampleDate != null)
                    hashCode = hashCode * 59 + this.SampleDate.GetHashCode();
                if (this.StartDate != null)
                    hashCode = hashCode * 59 + this.StartDate.GetHashCode();
                if (this.StreamId != null)
                    hashCode = hashCode * 59 + this.StreamId.GetHashCode();
                if (this.TagIds != null)
                    hashCode = hashCode * 59 + this.TagIds.GetHashCode();
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
            // StreamId (int?) minimum
            if(this.StreamId < (int?)1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for StreamId, must be a value greater than or equal to 1.", new [] { "StreamId" });
            }

            yield break;
        }
    }

}

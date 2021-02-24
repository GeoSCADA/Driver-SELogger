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
    /// StoredSampleScalarResponse result type (default view)
    /// </summary>
    [DataContract]
    public partial class StoredSampleScalarResponse :  IEquatable<StoredSampleScalarResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSampleScalarResponse" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected StoredSampleScalarResponse() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSampleScalarResponse" /> class.
        /// </summary>
        /// <param name="id">ID is the unique id of the sample. (required).</param>
        /// <param name="sampleDate">Sample date (required).</param>
        /// <param name="streamId">Stream ID (required).</param>
        /// <param name="value">Value (required).</param>
        public StoredSampleScalarResponse(long? id = default(long?), DateTime? sampleDate = default(DateTime?), int? streamId = default(int?), string value = default(string))
        {
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new InvalidDataException("id is a required property for StoredSampleScalarResponse and cannot be null");
            }
            else
            {
                this.Id = id;
            }
            // to ensure "sampleDate" is required (not null)
            if (sampleDate == null)
            {
                throw new InvalidDataException("sampleDate is a required property for StoredSampleScalarResponse and cannot be null");
            }
            else
            {
                this.SampleDate = sampleDate;
            }
            // to ensure "streamId" is required (not null)
            if (streamId == null)
            {
                throw new InvalidDataException("streamId is a required property for StoredSampleScalarResponse and cannot be null");
            }
            else
            {
                this.StreamId = streamId;
            }
            // to ensure "value" is required (not null)
            if (value == null)
            {
                throw new InvalidDataException("value is a required property for StoredSampleScalarResponse and cannot be null");
            }
            else
            {
                this.Value = value;
            }
        }
        
        /// <summary>
        /// ID is the unique id of the sample.
        /// </summary>
        /// <value>ID is the unique id of the sample.</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long? Id { get; set; }

        /// <summary>
        /// Sample date
        /// </summary>
        /// <value>Sample date</value>
        [DataMember(Name="sample_date", EmitDefaultValue=false)]
        public DateTime? SampleDate { get; set; }

        /// <summary>
        /// Stream ID
        /// </summary>
        /// <value>Stream ID</value>
        [DataMember(Name="stream_id", EmitDefaultValue=false)]
        public int? StreamId { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        /// <value>Value</value>
        [DataMember(Name="value", EmitDefaultValue=false)]
        public string Value { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StoredSampleScalarResponse {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  SampleDate: ").Append(SampleDate).Append("\n");
            sb.Append("  StreamId: ").Append(StreamId).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
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
            return this.Equals(input as StoredSampleScalarResponse);
        }

        /// <summary>
        /// Returns true if StoredSampleScalarResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of StoredSampleScalarResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StoredSampleScalarResponse input)
        {
            if (input == null)
                return false;

            return 
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
                    this.StreamId == input.StreamId ||
                    (this.StreamId != null &&
                    this.StreamId.Equals(input.StreamId))
                ) && 
                (
                    this.Value == input.Value ||
                    (this.Value != null &&
                    this.Value.Equals(input.Value))
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
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.SampleDate != null)
                    hashCode = hashCode * 59 + this.SampleDate.GetHashCode();
                if (this.StreamId != null)
                    hashCode = hashCode * 59 + this.StreamId.GetHashCode();
                if (this.Value != null)
                    hashCode = hashCode * 59 + this.Value.GetHashCode();
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

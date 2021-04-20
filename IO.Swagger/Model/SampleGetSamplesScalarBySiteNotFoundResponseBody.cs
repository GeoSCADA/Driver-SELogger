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
    /// Sample not found
    /// </summary>
    [DataContract]
    public partial class SampleGetSamplesScalarBySiteNotFoundResponseBody :  IEquatable<SampleGetSamplesScalarBySiteNotFoundResponseBody>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleGetSamplesScalarBySiteNotFoundResponseBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected SampleGetSamplesScalarBySiteNotFoundResponseBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleGetSamplesScalarBySiteNotFoundResponseBody" /> class.
        /// </summary>
        /// <param name="message">Message of error (required).</param>
        /// <param name="requestId">Request ID (required).</param>
        public SampleGetSamplesScalarBySiteNotFoundResponseBody(string message = default(string), string requestId = default(string))
        {
            // to ensure "message" is required (not null)
            if (message == null)
            {
                throw new InvalidDataException("message is a required property for SampleGetSamplesScalarBySiteNotFoundResponseBody and cannot be null");
            }
            else
            {
                this.Message = message;
            }
            // to ensure "requestId" is required (not null)
            if (requestId == null)
            {
                throw new InvalidDataException("requestId is a required property for SampleGetSamplesScalarBySiteNotFoundResponseBody and cannot be null");
            }
            else
            {
                this.RequestId = requestId;
            }
        }
        
        /// <summary>
        /// Message of error
        /// </summary>
        /// <value>Message of error</value>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }

        /// <summary>
        /// Request ID
        /// </summary>
        /// <value>Request ID</value>
        [DataMember(Name="request_id", EmitDefaultValue=false)]
        public string RequestId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SampleGetSamplesScalarBySiteNotFoundResponseBody {\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  RequestId: ").Append(RequestId).Append("\n");
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
            return this.Equals(input as SampleGetSamplesScalarBySiteNotFoundResponseBody);
        }

        /// <summary>
        /// Returns true if SampleGetSamplesScalarBySiteNotFoundResponseBody instances are equal
        /// </summary>
        /// <param name="input">Instance of SampleGetSamplesScalarBySiteNotFoundResponseBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SampleGetSamplesScalarBySiteNotFoundResponseBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) && 
                (
                    this.RequestId == input.RequestId ||
                    (this.RequestId != null &&
                    this.RequestId.Equals(input.RequestId))
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
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.RequestId != null)
                    hashCode = hashCode * 59 + this.RequestId.GetHashCode();
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
            yield break;
        }
    }

}

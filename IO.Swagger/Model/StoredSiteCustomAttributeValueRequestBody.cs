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
    /// StoredSiteCustomAttributeValueRequestBody result type (default view)
    /// </summary>
    [DataContract]
    public partial class StoredSiteCustomAttributeValueRequestBody :  IEquatable<StoredSiteCustomAttributeValueRequestBody>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSiteCustomAttributeValueRequestBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected StoredSiteCustomAttributeValueRequestBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredSiteCustomAttributeValueRequestBody" /> class.
        /// </summary>
        /// <param name="displayName">Display name (required).</param>
        /// <param name="id">ID is the unique id of the site custom attribute value. (required).</param>
        /// <param name="parentId">Parent ID (required).</param>
        /// <param name="value">Value (required).</param>
        public StoredSiteCustomAttributeValueRequestBody(string displayName = default(string), long? id = default(long?), int? parentId = default(int?), string value = default(string))
        {
            // to ensure "displayName" is required (not null)
            if (displayName == null)
            {
                throw new InvalidDataException("displayName is a required property for StoredSiteCustomAttributeValueRequestBody and cannot be null");
            }
            else
            {
                this.DisplayName = displayName;
            }
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new InvalidDataException("id is a required property for StoredSiteCustomAttributeValueRequestBody and cannot be null");
            }
            else
            {
                this.Id = id;
            }
            // to ensure "parentId" is required (not null)
            if (parentId == null)
            {
                throw new InvalidDataException("parentId is a required property for StoredSiteCustomAttributeValueRequestBody and cannot be null");
            }
            else
            {
                this.ParentId = parentId;
            }
            // to ensure "value" is required (not null)
            if (value == null)
            {
                throw new InvalidDataException("value is a required property for StoredSiteCustomAttributeValueRequestBody and cannot be null");
            }
            else
            {
                this.Value = value;
            }
        }
        
        /// <summary>
        /// Display name
        /// </summary>
        /// <value>Display name</value>
        [DataMember(Name="display_name", EmitDefaultValue=false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// ID is the unique id of the site custom attribute value.
        /// </summary>
        /// <value>ID is the unique id of the site custom attribute value.</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long? Id { get; set; }

        /// <summary>
        /// Parent ID
        /// </summary>
        /// <value>Parent ID</value>
        [DataMember(Name="parent_id", EmitDefaultValue=false)]
        public int? ParentId { get; set; }

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
            sb.Append("class StoredSiteCustomAttributeValueRequestBody {\n");
            sb.Append("  DisplayName: ").Append(DisplayName).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  ParentId: ").Append(ParentId).Append("\n");
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
            return this.Equals(input as StoredSiteCustomAttributeValueRequestBody);
        }

        /// <summary>
        /// Returns true if StoredSiteCustomAttributeValueRequestBody instances are equal
        /// </summary>
        /// <param name="input">Instance of StoredSiteCustomAttributeValueRequestBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StoredSiteCustomAttributeValueRequestBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.DisplayName == input.DisplayName ||
                    (this.DisplayName != null &&
                    this.DisplayName.Equals(input.DisplayName))
                ) && 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.ParentId == input.ParentId ||
                    (this.ParentId != null &&
                    this.ParentId.Equals(input.ParentId))
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
                if (this.DisplayName != null)
                    hashCode = hashCode * 59 + this.DisplayName.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.ParentId != null)
                    hashCode = hashCode * 59 + this.ParentId.GetHashCode();
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
            // DisplayName (string) maxLength
            if(this.DisplayName != null && this.DisplayName.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for DisplayName, length must be less than 256.", new [] { "DisplayName" });
            }

            // ParentId (int?) minimum
            if(this.ParentId < (int?)1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ParentId, must be a value greater than or equal to 1.", new [] { "ParentId" });
            }

            // Value (string) maxLength
            if(this.Value != null && this.Value.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Value, length must be less than 256.", new [] { "Value" });
            }

            yield break;
        }
    }

}

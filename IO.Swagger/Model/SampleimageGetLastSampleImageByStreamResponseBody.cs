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
    /// GetLastSampleImageByStreamResponseBody result type (default view)
    /// </summary>
    [DataContract]
    public partial class SampleimageGetLastSampleImageByStreamResponseBody :  IEquatable<SampleimageGetLastSampleImageByStreamResponseBody>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleimageGetLastSampleImageByStreamResponseBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected SampleimageGetLastSampleImageByStreamResponseBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleimageGetLastSampleImageByStreamResponseBody" /> class.
        /// </summary>
        /// <param name="height">Height (required).</param>
        /// <param name="id">ID is the unique id of the sample. (required).</param>
        /// <param name="sampleDate">Sample date (required).</param>
        /// <param name="size">Size (required).</param>
        /// <param name="streamId">Stream ID (required).</param>
        /// <param name="url">URL (required).</param>
        /// <param name="width">Width (required).</param>
        public SampleimageGetLastSampleImageByStreamResponseBody(long? height = default(long?), long? id = default(long?), DateTime? sampleDate = default(DateTime?), long? size = default(long?), int? streamId = default(int?), string url = default(string), long? width = default(long?))
        {
            // to ensure "height" is required (not null)
            if (height == null)
            {
                throw new InvalidDataException("height is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.Height = height;
            }
            // to ensure "id" is required (not null)
            if (id == null)
            {
                throw new InvalidDataException("id is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.Id = id;
            }
            // to ensure "sampleDate" is required (not null)
            if (sampleDate == null)
            {
                throw new InvalidDataException("sampleDate is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.SampleDate = sampleDate;
            }
            // to ensure "size" is required (not null)
            if (size == null)
            {
                throw new InvalidDataException("size is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.Size = size;
            }
            // to ensure "streamId" is required (not null)
            if (streamId == null)
            {
                throw new InvalidDataException("streamId is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.StreamId = streamId;
            }
            // to ensure "url" is required (not null)
            if (url == null)
            {
                throw new InvalidDataException("url is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.Url = url;
            }
            // to ensure "width" is required (not null)
            if (width == null)
            {
                throw new InvalidDataException("width is a required property for SampleimageGetLastSampleImageByStreamResponseBody and cannot be null");
            }
            else
            {
                this.Width = width;
            }
        }
        
        /// <summary>
        /// Height
        /// </summary>
        /// <value>Height</value>
        [DataMember(Name="height", EmitDefaultValue=false)]
        public long? Height { get; set; }

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
        /// Size
        /// </summary>
        /// <value>Size</value>
        [DataMember(Name="size", EmitDefaultValue=false)]
        public long? Size { get; set; }

        /// <summary>
        /// Stream ID
        /// </summary>
        /// <value>Stream ID</value>
        [DataMember(Name="stream_id", EmitDefaultValue=false)]
        public int? StreamId { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        /// <value>URL</value>
        [DataMember(Name="url", EmitDefaultValue=false)]
        public string Url { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        /// <value>Width</value>
        [DataMember(Name="width", EmitDefaultValue=false)]
        public long? Width { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SampleimageGetLastSampleImageByStreamResponseBody {\n");
            sb.Append("  Height: ").Append(Height).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  SampleDate: ").Append(SampleDate).Append("\n");
            sb.Append("  Size: ").Append(Size).Append("\n");
            sb.Append("  StreamId: ").Append(StreamId).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
            sb.Append("  Width: ").Append(Width).Append("\n");
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
            return this.Equals(input as SampleimageGetLastSampleImageByStreamResponseBody);
        }

        /// <summary>
        /// Returns true if SampleimageGetLastSampleImageByStreamResponseBody instances are equal
        /// </summary>
        /// <param name="input">Instance of SampleimageGetLastSampleImageByStreamResponseBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SampleimageGetLastSampleImageByStreamResponseBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Height == input.Height ||
                    (this.Height != null &&
                    this.Height.Equals(input.Height))
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
                    this.Size == input.Size ||
                    (this.Size != null &&
                    this.Size.Equals(input.Size))
                ) && 
                (
                    this.StreamId == input.StreamId ||
                    (this.StreamId != null &&
                    this.StreamId.Equals(input.StreamId))
                ) && 
                (
                    this.Url == input.Url ||
                    (this.Url != null &&
                    this.Url.Equals(input.Url))
                ) && 
                (
                    this.Width == input.Width ||
                    (this.Width != null &&
                    this.Width.Equals(input.Width))
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
                if (this.Height != null)
                    hashCode = hashCode * 59 + this.Height.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.SampleDate != null)
                    hashCode = hashCode * 59 + this.SampleDate.GetHashCode();
                if (this.Size != null)
                    hashCode = hashCode * 59 + this.Size.GetHashCode();
                if (this.StreamId != null)
                    hashCode = hashCode * 59 + this.StreamId.GetHashCode();
                if (this.Url != null)
                    hashCode = hashCode * 59 + this.Url.GetHashCode();
                if (this.Width != null)
                    hashCode = hashCode * 59 + this.Width.GetHashCode();
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

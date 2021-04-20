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
    /// ThresholdLevelRequestBody
    /// </summary>
    [DataContract]
    public partial class ThresholdLevelRequestBody :  IEquatable<ThresholdLevelRequestBody>, IValidatableObject
    {
        /// <summary>
        /// Transmission device report interval
        /// </summary>
        /// <value>Transmission device report interval</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TransmissionIntervalEnum
        {
            
            /// <summary>
            /// Enum Normal for value: normal
            /// </summary>
            [EnumMember(Value = "normal")]
            Normal = 1,
            
            /// <summary>
            /// Enum Event for value: event
            /// </summary>
            [EnumMember(Value = "event")]
            Event = 2,
            
            /// <summary>
            /// Enum Emergency for value: emergency
            /// </summary>
            [EnumMember(Value = "emergency")]
            Emergency = 3
        }

        /// <summary>
        /// Transmission device report interval
        /// </summary>
        /// <value>Transmission device report interval</value>
        [DataMember(Name="transmission_interval", EmitDefaultValue=false)]
        public TransmissionIntervalEnum TransmissionInterval { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdLevelRequestBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ThresholdLevelRequestBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdLevelRequestBody" /> class.
        /// </summary>
        /// <param name="comment">Comment for the threshold&#39;s level (required).</param>
        /// <param name="displayName">Name of the threshold&#39;s level (required).</param>
        /// <param name="falseAlarmFilter">Threshold triggered interval in seconds (required).</param>
        /// <param name="notificationActions">notificationActions.</param>
        /// <param name="outputActions">outputActions.</param>
        /// <param name="sampleActions">sampleActions.</param>
        /// <param name="sampleInterval">Sample interval in seconds how often the device streams (required).</param>
        /// <param name="transmissionInterval">Transmission device report interval (required) (default to TransmissionIntervalEnum.Normal).</param>
        /// <param name="transmitImmediately">When a device reached threshold zone does he need to transmit immediately (required) (default to false).</param>
        /// <param name="upperLimit">Upper limit of the threshold range (required).</param>
        public ThresholdLevelRequestBody(string comment = default(string), string displayName = default(string), int? falseAlarmFilter = default(int?), List<ThresholdActionNotificationRequestBody> notificationActions = default(List<ThresholdActionNotificationRequestBody>), List<ThresholdActionOutputRequestBody> outputActions = default(List<ThresholdActionOutputRequestBody>), List<ThresholdActionSampleRequestBody> sampleActions = default(List<ThresholdActionSampleRequestBody>), int? sampleInterval = default(int?), TransmissionIntervalEnum transmissionInterval = TransmissionIntervalEnum.Normal, bool? transmitImmediately = false, double? upperLimit = default(double?))
        {
            // to ensure "comment" is required (not null)
            if (comment == null)
            {
                throw new InvalidDataException("comment is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.Comment = comment;
            }
            // to ensure "displayName" is required (not null)
            if (displayName == null)
            {
                throw new InvalidDataException("displayName is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.DisplayName = displayName;
            }
            // to ensure "falseAlarmFilter" is required (not null)
            if (falseAlarmFilter == null)
            {
                throw new InvalidDataException("falseAlarmFilter is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.FalseAlarmFilter = falseAlarmFilter;
            }
            // to ensure "sampleInterval" is required (not null)
            if (sampleInterval == null)
            {
                throw new InvalidDataException("sampleInterval is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.SampleInterval = sampleInterval;
            }
            // to ensure "transmissionInterval" is required (not null)
            if (transmissionInterval == null)
            {
                throw new InvalidDataException("transmissionInterval is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.TransmissionInterval = transmissionInterval;
            }
            // to ensure "transmitImmediately" is required (not null)
            if (transmitImmediately == null)
            {
                throw new InvalidDataException("transmitImmediately is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.TransmitImmediately = transmitImmediately;
            }
            // to ensure "upperLimit" is required (not null)
            if (upperLimit == null)
            {
                throw new InvalidDataException("upperLimit is a required property for ThresholdLevelRequestBody and cannot be null");
            }
            else
            {
                this.UpperLimit = upperLimit;
            }
            this.NotificationActions = notificationActions;
            this.OutputActions = outputActions;
            this.SampleActions = sampleActions;
        }
        
        /// <summary>
        /// Comment for the threshold&#39;s level
        /// </summary>
        /// <value>Comment for the threshold&#39;s level</value>
        [DataMember(Name="comment", EmitDefaultValue=false)]
        public string Comment { get; set; }

        /// <summary>
        /// Name of the threshold&#39;s level
        /// </summary>
        /// <value>Name of the threshold&#39;s level</value>
        [DataMember(Name="display_name", EmitDefaultValue=false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Threshold triggered interval in seconds
        /// </summary>
        /// <value>Threshold triggered interval in seconds</value>
        [DataMember(Name="false_alarm_filter", EmitDefaultValue=false)]
        public int? FalseAlarmFilter { get; set; }

        /// <summary>
        /// Gets or Sets NotificationActions
        /// </summary>
        [DataMember(Name="notification_actions", EmitDefaultValue=false)]
        public List<ThresholdActionNotificationRequestBody> NotificationActions { get; set; }

        /// <summary>
        /// Gets or Sets OutputActions
        /// </summary>
        [DataMember(Name="output_actions", EmitDefaultValue=false)]
        public List<ThresholdActionOutputRequestBody> OutputActions { get; set; }

        /// <summary>
        /// Gets or Sets SampleActions
        /// </summary>
        [DataMember(Name="sample_actions", EmitDefaultValue=false)]
        public List<ThresholdActionSampleRequestBody> SampleActions { get; set; }

        /// <summary>
        /// Sample interval in seconds how often the device streams
        /// </summary>
        /// <value>Sample interval in seconds how often the device streams</value>
        [DataMember(Name="sample_interval", EmitDefaultValue=false)]
        public int? SampleInterval { get; set; }


        /// <summary>
        /// When a device reached threshold zone does he need to transmit immediately
        /// </summary>
        /// <value>When a device reached threshold zone does he need to transmit immediately</value>
        [DataMember(Name="transmit_immediately", EmitDefaultValue=false)]
        public bool? TransmitImmediately { get; set; }

        /// <summary>
        /// Upper limit of the threshold range
        /// </summary>
        /// <value>Upper limit of the threshold range</value>
        [DataMember(Name="upper_limit", EmitDefaultValue=false)]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ThresholdLevelRequestBody {\n");
            sb.Append("  Comment: ").Append(Comment).Append("\n");
            sb.Append("  DisplayName: ").Append(DisplayName).Append("\n");
            sb.Append("  FalseAlarmFilter: ").Append(FalseAlarmFilter).Append("\n");
            sb.Append("  NotificationActions: ").Append(NotificationActions).Append("\n");
            sb.Append("  OutputActions: ").Append(OutputActions).Append("\n");
            sb.Append("  SampleActions: ").Append(SampleActions).Append("\n");
            sb.Append("  SampleInterval: ").Append(SampleInterval).Append("\n");
            sb.Append("  TransmissionInterval: ").Append(TransmissionInterval).Append("\n");
            sb.Append("  TransmitImmediately: ").Append(TransmitImmediately).Append("\n");
            sb.Append("  UpperLimit: ").Append(UpperLimit).Append("\n");
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
            return this.Equals(input as ThresholdLevelRequestBody);
        }

        /// <summary>
        /// Returns true if ThresholdLevelRequestBody instances are equal
        /// </summary>
        /// <param name="input">Instance of ThresholdLevelRequestBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ThresholdLevelRequestBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Comment == input.Comment ||
                    (this.Comment != null &&
                    this.Comment.Equals(input.Comment))
                ) && 
                (
                    this.DisplayName == input.DisplayName ||
                    (this.DisplayName != null &&
                    this.DisplayName.Equals(input.DisplayName))
                ) && 
                (
                    this.FalseAlarmFilter == input.FalseAlarmFilter ||
                    (this.FalseAlarmFilter != null &&
                    this.FalseAlarmFilter.Equals(input.FalseAlarmFilter))
                ) && 
                (
                    this.NotificationActions == input.NotificationActions ||
                    this.NotificationActions != null &&
                    this.NotificationActions.SequenceEqual(input.NotificationActions)
                ) && 
                (
                    this.OutputActions == input.OutputActions ||
                    this.OutputActions != null &&
                    this.OutputActions.SequenceEqual(input.OutputActions)
                ) && 
                (
                    this.SampleActions == input.SampleActions ||
                    this.SampleActions != null &&
                    this.SampleActions.SequenceEqual(input.SampleActions)
                ) && 
                (
                    this.SampleInterval == input.SampleInterval ||
                    (this.SampleInterval != null &&
                    this.SampleInterval.Equals(input.SampleInterval))
                ) && 
                (
                    this.TransmissionInterval == input.TransmissionInterval ||
                    (this.TransmissionInterval != null &&
                    this.TransmissionInterval.Equals(input.TransmissionInterval))
                ) && 
                (
                    this.TransmitImmediately == input.TransmitImmediately ||
                    (this.TransmitImmediately != null &&
                    this.TransmitImmediately.Equals(input.TransmitImmediately))
                ) && 
                (
                    this.UpperLimit == input.UpperLimit ||
                    (this.UpperLimit != null &&
                    this.UpperLimit.Equals(input.UpperLimit))
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
                if (this.Comment != null)
                    hashCode = hashCode * 59 + this.Comment.GetHashCode();
                if (this.DisplayName != null)
                    hashCode = hashCode * 59 + this.DisplayName.GetHashCode();
                if (this.FalseAlarmFilter != null)
                    hashCode = hashCode * 59 + this.FalseAlarmFilter.GetHashCode();
                if (this.NotificationActions != null)
                    hashCode = hashCode * 59 + this.NotificationActions.GetHashCode();
                if (this.OutputActions != null)
                    hashCode = hashCode * 59 + this.OutputActions.GetHashCode();
                if (this.SampleActions != null)
                    hashCode = hashCode * 59 + this.SampleActions.GetHashCode();
                if (this.SampleInterval != null)
                    hashCode = hashCode * 59 + this.SampleInterval.GetHashCode();
                if (this.TransmissionInterval != null)
                    hashCode = hashCode * 59 + this.TransmissionInterval.GetHashCode();
                if (this.TransmitImmediately != null)
                    hashCode = hashCode * 59 + this.TransmitImmediately.GetHashCode();
                if (this.UpperLimit != null)
                    hashCode = hashCode * 59 + this.UpperLimit.GetHashCode();
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
            // Comment (string) maxLength
            if(this.Comment != null && this.Comment.Length > 512)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Comment, length must be less than 512.", new [] { "Comment" });
            }

            // DisplayName (string) maxLength
            if(this.DisplayName != null && this.DisplayName.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for DisplayName, length must be less than 256.", new [] { "DisplayName" });
            }

            // FalseAlarmFilter (int?) maximum
            if(this.FalseAlarmFilter > 4294967300)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FalseAlarmFilter, must be a value less than or equal to 4294967300.", new [] { "FalseAlarmFilter" });
            }

            // FalseAlarmFilter (int?) minimum
            if(this.FalseAlarmFilter < (int?)0)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FalseAlarmFilter, must be a value greater than or equal to 0.", new [] { "FalseAlarmFilter" });
            }

            // SampleInterval (int?) maximum
            if(this.SampleInterval > 4294967300)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SampleInterval, must be a value less than or equal to 4294967300.", new [] { "SampleInterval" });
            }

            // SampleInterval (int?) minimum
            if(this.SampleInterval < (int?)30)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SampleInterval, must be a value greater than or equal to 30.", new [] { "SampleInterval" });
            }

            yield break;
        }
    }

}

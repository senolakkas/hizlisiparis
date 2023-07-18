using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI
{
    public class ClassificationStatus
    {
        /// <summary>
        /// The status value
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
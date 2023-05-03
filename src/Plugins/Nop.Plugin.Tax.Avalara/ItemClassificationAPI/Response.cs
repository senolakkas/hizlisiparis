using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI
{
    /// <summary>
    /// Represents response from the service
    /// </summary>
    public class Response : HSClassificationModel
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}

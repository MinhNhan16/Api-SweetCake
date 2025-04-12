using Newtonsoft.Json;

namespace ASM_NhomSugar_SD19311.DTO
{
    public class GoogleTokenInfo
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }
    }
}

using Newtonsoft.Json;

namespace AmouxBot.Model
{
    public class LoginResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("idPessoa")]
        public string IdPessoa { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonIgnore]
        public string Login { get; set; }

        [JsonIgnore]
        public bool Sucesso { get; set; }
    }
}

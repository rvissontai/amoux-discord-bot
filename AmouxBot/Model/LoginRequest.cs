using Newtonsoft.Json;

namespace AmouxBot.Model
{
    public class LoginRequest
    {
        [JsonProperty("usuario")]
        public string Login { get; set; }

        [JsonProperty("senha")]
        public string Senha { get; set; }
    }
}

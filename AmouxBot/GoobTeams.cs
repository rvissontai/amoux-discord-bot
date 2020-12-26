using AmouxBot.Entidades;
using AmouxBot.Model;
using Database;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace AmouxBot
{
    public class GoobTeams
    {
        private IConfigurationRoot _configuration { get; set; }

        public GoobTeams()
        {
        }

        public GoobTeams(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public LoginResponse Login(string login, string senha)
        {
            var url = "https://apiteams.goobee.com.br/api/Token"; //_configuration.GetValue<string>("goobeteams:urlLogin");
            var requestModel = new LoginRequest()
            {
                Login = login,
                Senha = senha
            };

            var response = new LoginResponse();

            try
            {
                using (var wb = new WebClient())
                {
                    wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    response = JsonConvert.DeserializeObject<LoginResponse>(wb.UploadString(url, "POST", JsonConvert.SerializeObject(requestModel)));
                    response.Login = login;
                    response.Sucesso = true;

                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
            }

            return response;   
        }

        public void MudarHumor(string login, string senha, Sentimento sentimento)
        {
            var loginResponse = Login(login, senha);

            using (var db = new SqliteContext())
            {
                var usuarioSentimento = db.UsuarioSentimento
                    .AsQueryable()
                    .Where(o => o.LoginUsuario == loginResponse.Login && o.DataCadastro == DateTime.Today)
                    .FirstOrDefault();

                if (usuarioSentimento == null)
                    Adicionar(loginResponse, sentimento);
                else
                    Alterar(loginResponse, usuarioSentimento, sentimento);
            }
        }

        private void Adicionar(LoginResponse loginResponse, Sentimento sentimento)
        {
            var urlAdicionar = "https://apiteams.goobee.com.br/api/Home/AdicionarHumor";
            var paramAdicionar = new
            {
                idPessoa = loginResponse.IdPessoa,
                idResponsavelCriacao = loginResponse.Id,
                sentimento = (int)sentimento
            };

            using (var wb = new WebClient())
            {
                wb.Headers[HttpRequestHeader.Authorization] = "Bearer " + loginResponse.Token;
                wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wb.Headers[HttpRequestHeader.Accept] = "application/json";

                var response = wb.UploadString(urlAdicionar, "POST", JsonConvert.SerializeObject(paramAdicionar));

                using (var db = new SqliteContext())
                {
                    var user = db.Usuarios.AsQueryable().Where(o => o.Login == loginResponse.Login).FirstOrDefault();

                    db.UsuarioSentimento.Add(new UsuarioSentimento() { 
                        IdUsuario = user.Id, 
                        IdSentimentoPessoa = response.ToString().Replace("\"", ""),
                        LoginUsuario = loginResponse.Login, 
                        DataCadastro = DateTime.Today 
                    });
                    db.SaveChanges();
                }
            };
        }

        private void Alterar(LoginResponse loginResponse, UsuarioSentimento usuarioSentimento, Sentimento sentimento)
        {
            var url = "https://apiteams.goobee.com.br/api/Home/EditarHumor/" + usuarioSentimento.IdSentimentoPessoa;

            var requestModel = new
            {
                idSentimentoPessoa = usuarioSentimento.IdSentimentoPessoa,
                idResponsavelCriacao = loginResponse.Id,
                sentimento = (int)sentimento
            };

            using (var wb = new WebClient())
            {
                wb.Headers[HttpRequestHeader.Authorization] = "Bearer " + loginResponse.Token;
                wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wb.Headers[HttpRequestHeader.Accept] = "application/json";

                wb.UploadString(url, "PUT", JsonConvert.SerializeObject(requestModel));
            };

        }
    }
}

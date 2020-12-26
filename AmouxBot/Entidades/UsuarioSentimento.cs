using System;

namespace AmouxBot.Entidades
{
    public class UsuarioSentimento
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }

        public string LoginUsuario { get; set; }

        public string IdSentimentoPessoa { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}

using Database;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace AmouxBot.Module.Comandos.Humor
{
    public class Irritado : ModuleBase<SocketCommandContext>
    {
        [Command("irritado")]
        public async Task AlterarParaIrritado()
        {
            using (var db = new SqliteContext())
            {
                var user = db.Usuarios.AsQueryable().Where(o => o.IdDiscord == Context.User.Id.ToString()).FirstOrDefault();

                if (user == null)
                {
                    await Context.User.SendMessageAsync("Agora é só me falar seu email e senha em uma mensagem só beleza? é só separar por espaço, fica tranquilo que não sou x9.");
                    await ReplyAsync("Não tenho suas credenciais, te mandei uma mensagem privada, me passa seus dados lá.");
                }
                else
                {
                    new GoobTeams().MudarHumor(user.Login, user.Senha, Sentimento.Irritado);

                    await ReplyAsync("Humor alterado para irrado =/ mano vai com calma ai véi, tudo nessa vida passa.");
                }
            }


        }
    }
}

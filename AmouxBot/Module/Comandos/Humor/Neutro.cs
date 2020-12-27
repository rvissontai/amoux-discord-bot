using Database;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace AmouxBot.Module.Comandos.Humor
{
    public class Neutro : ModuleBase<SocketCommandContext>
    {
        [Command("neutro")]
        public async Task AlterarParaNeutro()
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
                    var response = new GoobTeams().MudarHumor(user.Login, user.Senha, Sentimento.Neutro);

                    if (response.Sucesso)
                        await ReplyAsync("Humor alterado para neutro!");
                    else
                        await ReplyAsync(response.Mensagem);
                }
            }


        }
    }
}

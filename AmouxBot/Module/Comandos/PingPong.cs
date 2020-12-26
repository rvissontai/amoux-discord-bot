using Discord.Commands;
using System.Threading.Tasks;

namespace AmouxBot.Module.Comandos
{
    public class PingPong : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }
    }
}

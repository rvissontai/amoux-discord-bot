﻿using Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AmouxBot
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _cliente;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            var serviceCollection = new ServiceCollection();

            _cliente = new DiscordSocketClient();
            _commands = new CommandService();
            _services = serviceCollection.AddSingleton(_cliente).AddSingleton(_commands).BuildServiceProvider();

            ConfigureServices(serviceCollection);

            _cliente.Ready += Cliente_Ready;
            _cliente.Log += Cliente_Log;

            await _cliente.LoginAsync(TokenType.Bot, Util.Decrypt(configuration.GetValue<string>("token")));
            await Cliente_Ready();
            await ComandosBot();

            await _cliente.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ComandosBot()
        {
            _cliente.MessageReceived += Cliente_MessageReceived;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task Cliente_MessageReceived(SocketMessage arg)
        {
            var mensagem = arg as SocketUserMessage;

            if (mensagem == null || mensagem.Author.IsBot)
                return;

            var context = new SocketCommandContext(_cliente, mensagem);
            int argPost = 0;

            //Se for mensagem privada para o bot
            if (mensagem.Channel is SocketDMChannel)
            {
                var array = mensagem.Content.Split(" ");

                if (array.Length < 2)
                    await mensagem.Author.SendMessageAsync("Cara alguma coisa errada não ta certa, confere o que você digitou ai.");

                var response = new GoobTeams().Login(array[0], array[1]);

                if (response != null && response.Sucesso && !string.IsNullOrWhiteSpace(response.Token))
                {
                    using (var db = new SqliteContext())
                    {
                        db.Usuarios.Add(new Usuario() { Login = array[0], Senha = array[1], IdDiscord = mensagem.Author.Id.ToString() });
                        db.SaveChanges();
                    }

                    await mensagem.Author.SendMessageAsync("Boaaa, consegui autenticar você, volta no chat lá e manda o comando pra modificar o humor, valeu!");
                }
                else
                {
                    await mensagem.Author.SendMessageAsync("Não consegui fazer o login com a info que você me passou, tem certeza que passou os dados certos?");
                }
            }
            else if(mensagem.HasStringPrefix("?", ref argPost))
            {
                var result = await _commands.ExecuteAsync(context, argPost, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }

        private Task Cliente_Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        private async Task Cliente_Ready()
        {
            await _cliente.SetGameAsync("Jogo da vida", "https://www.google.com.br", ActivityType.Playing);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            // Add app
            //serviceCollection.AddTransient<App>();
        }
    }
}

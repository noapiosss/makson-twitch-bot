using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Domain.Commands;
using Domain.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Web.Configuration;
using Web.Services.Interfaces;

namespace Web.Services
{

    public class TwitchBotService : IHostedService, IDisposable
    {
        private readonly ISpotifyAPI _spotifyApiClient;
        private readonly ITwitchAPI _twitchApiClient;
        private readonly TwitchClient _twitchBotClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string PeriodicMessage = "Yo guys and gals! If you want to support streamer and get more content follow his social medias:" +
                "\nhttps://twitter.com/MaksonGospodar/" +
                "\nhttps://www.instagram.com/2ez4makson/";
        public Dictionary<string, DateTime> Viewers = new();
        public Dictionary<string, string> Commands = new();
        public Timer PeriodicMessageTimer;

        public TwitchBotService(IOptionsMonitor<TwitchBotConfiguration> twitchBotConfiguration,
            IServiceScopeFactory serviceScopeFactory,
            // OptionsMonitor<SpotifyApiConfiguration> spotifyApiConfiguration,
            ISpotifyAPI spotifyApiClient,
            ITwitchAPI twitchApiClient)
        {
            _twitchApiClient = twitchApiClient;
            _spotifyApiClient = spotifyApiClient;
            _serviceScopeFactory = serviceScopeFactory;


            ConnectionCredentials credentials = new(twitchBotConfiguration.CurrentValue.BotUsername, twitchBotConfiguration.CurrentValue.BotPassword);
            ClientOptions clientOptions = new()
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new(clientOptions);
            _twitchBotClient = new(customClient);
            _twitchBotClient.Initialize(credentials, twitchBotConfiguration.CurrentValue.Channel);

            _twitchBotClient.OnLog += Client_OnLog;
            _twitchBotClient.OnWhisperReceived += Client_OnWhisperReceived;
            _twitchBotClient.OnConnected += Client_OnConnected;
            _twitchBotClient.OnMessageReceived += ClientOnMessageReceived;
            _twitchBotClient.OnUserJoined += Client_OnUserJoin;
            _twitchBotClient.OnUserLeft += Client_OnUserLeft;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            GetAllCommandsQuery getAllCommandsQuery = new() { };
            GetAllCommandsQueryResult getAllCommandsQueryResult = await mediator.Send(getAllCommandsQuery, cancellationToken);
            foreach (Command command in getAllCommandsQueryResult.Commands)
            {
                Commands.Add(command.CommandName, command.CommandOutput);
            }

            _ = _twitchBotClient.Connect();

            PeriodicMessageTimer = new(SendPeriodicMessage, null, TimeSpan.FromHours(30), TimeSpan.FromHours(1));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _twitchBotClient.Disconnect();
            Dispose();
            return Task.CompletedTask;
        }

        private void SendPeriodicMessage(object state)
        {
            _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, PeriodicMessage);
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            _twitchBotClient.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private static string GetFollowAgeMessage(string requestFrom, string requestAbout, DateTime followedDate)
        {
            int totalDays = (int)Math.Floor((DateTime.UtcNow - followedDate).TotalDays);
            int years = totalDays / 365;
            int months = (totalDays - (years * 365)) / 30;
            int days = totalDays - (years * 365) - (months * 30);

            string message = requestFrom == requestAbout ?
                 $"@{requestFrom} You are following the channel for " :
                 $"@{requestFrom} {requestAbout} is following the channel for ";

            if (years > 0)
            {
                message += years == 1 ? $"{years} year " : $"{years} years ";
            }
            if (months > 0)
            {
                message += months == 1 ? $"{months} month " : $"{months} months ";
            }
            if (days > 0)
            {
                message += days == 1 ? $"{days} day" : $"{days} days";
            }

            return message;
        }

        private string GetWatchTimeMessage(string requestFrom, string requestAbout)
        {
            if (Viewers.TryGetValue(requestAbout, out DateTime startWatchingAt))
            {
                return requestFrom == requestAbout ?
                    $"@{requestFrom} You are not watching stream" :
                    $"@{requestFrom} {requestAbout} is not watching stream";
            }

            TimeSpan watchtime = DateTime.UtcNow - startWatchingAt;

            string message = requestFrom == requestAbout ?
                $"@{requestFrom} You are watching a stream for " :
                $"@{requestFrom} {requestAbout} is watching a stream for ";

            if (watchtime.Hours > 0)
            {
                message += watchtime.Hours == 1 ? $"{watchtime.Hours} hour " : $"{watchtime.Hours} hours ";
            }
            if (watchtime.Minutes > 0)
            {
                message += watchtime.Minutes == 1 ? $"{watchtime.Minutes} minute " : $"{watchtime.Minutes} minutes ";
            }
            if (watchtime.Seconds > 0)
            {
                message += watchtime.Seconds == 1 ? $"{watchtime.Seconds} second" : $"{watchtime.Seconds} seconds";
            }

            return message;
        }

        public virtual async void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!followage"))
            {
                string[] split = e.ChatMessage.Message.Split(" ");
                GetUsersFollowsResponse follows;
                string askAbout;
                GetUsersResponse users;

                if (split.Length == 1)
                {
                    askAbout = e.ChatMessage.Username;
                    users = await _twitchApiClient.Helix.Users.GetUsersAsync(logins: new List<string>() { e.ChatMessage.Username });
                }
                else
                {
                    askAbout = split[1];
                    users = await _twitchApiClient.Helix.Users.GetUsersAsync(logins: new List<string>() { split[1] });
                }

                if (!users.Users.Any())
                {
                    _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{e.ChatMessage.Username} there is no user with such name");
                }

                string fromId = users.Users.FirstOrDefault().Id;

                if (fromId == null)
                {
                    _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{e.ChatMessage.Username} {askAbout} is not follower of this channel");
                }

                follows = await _twitchApiClient.Helix.Users.GetUsersFollowsAsync(
                    fromId: fromId,
                    toId: e.ChatMessage.RoomId);

                Follow follow = follows.Follows.FirstOrDefault();
                string message = follow != null
                    ? GetFollowAgeMessage(e.ChatMessage.Username, askAbout, follow.FollowedAt)
                    : e.ChatMessage.Username == askAbout ?
                        $"@{e.ChatMessage.Username} You are not follower of this channel" :
                        $"@{e.ChatMessage.Username} {askAbout} is not follower of this channel";

                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, message);

                return;
            }

            if (e.ChatMessage.Message.StartsWith("!watchtime"))
            {
                string[] split = e.ChatMessage.Message.Split(" ");
                string askAbout = split.Length == 1 ? e.ChatMessage.Username : split[1];
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, GetWatchTimeMessage(e.ChatMessage.Username, askAbout));
                return;
            }

            if (e.ChatMessage.Message.StartsWith("!song"))
            {
                string song = await _spotifyApiClient.GetCurrentPlayingTrack();
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{e.ChatMessage.Username} {song}");
                return;
            }

            if (e.ChatMessage.Message.StartsWith("!addCommand"))
            {
                if (!e.ChatMessage.IsModerator)
                {
                    return;
                }

                string[] split = e.ChatMessage.Message.Split(" ");
                string commandName = split[1];
                string commandOutput = split[2];

                await AddCustomCommand(e.ChatMessage.Username, commandName, commandOutput);

                return;
            }

            if (e.ChatMessage.Message.StartsWith("!editCommand"))
            {
                if (!e.ChatMessage.IsModerator)
                {
                    return;
                }

                string[] split = e.ChatMessage.Message.Split(" ");
                string commandName = split[1];
                string commandOutput = split[2];

                await EditCustomCommand(e.ChatMessage.Username, commandName, commandOutput);

                return;
            }

            if (e.ChatMessage.Message.StartsWith("!deleteCommand"))
            {
                if (!e.ChatMessage.IsModerator)
                {
                    return;
                }

                string[] split = e.ChatMessage.Message.Split(" ");
                string commandName = split[1];

                await DeleteCustomCommand(e.ChatMessage.Username, commandName);

                return;
            }

            if (e.ChatMessage.Message.StartsWith("!"))
            {
                TryExecuteCustomCommand(e.ChatMessage);
                return;
            }
        }

        private async Task AddCustomCommand(string moderatorName, string commandName, string commandOutput)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (Commands.TryAdd(commandName, commandOutput))
            {
                Command command = new()
                {
                    CommandName = commandName,
                    CommandOutput = commandOutput
                };

                AddCommand addCommand = new() { Command = command };
                _ = await mediator.Send(addCommand);

                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} Command was successfully added");
            }
            else
            {
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} Command is already exists. Try !editCommand [command] [commandOutput]");
            }
        }

        private async Task EditCustomCommand(string moderatorName, string commandName, string commandOutput)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (Commands.ContainsKey(commandName))
            {
                EditCommand editCommand = new()
                {
                    CommandName = commandName,
                    CommandOutput = commandOutput
                };
                _ = await mediator.Send(editCommand);

                Commands[commandName] = commandOutput;
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} Command was successfully edited");
            }
            else
            {
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} There is no such a command. Try !addCommand [command] [commandOutput]");
            }
        }

        private async Task DeleteCustomCommand(string moderatorName, string commandName)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (Commands.Remove(commandName))
            {
                DeleteCommand deleteCommand = new() { CommandName = commandName };
                _ = await mediator.Send(deleteCommand);

                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} Command was successfully deleted");
            }
            else
            {
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{moderatorName} There is no such a command");
            }
        }

        private void TryExecuteCustomCommand(ChatMessage chatMessage)
        {
            if (Commands.TryGetValue(chatMessage.Message, out string commandOutput))
            {
                _twitchBotClient.SendMessage(_twitchBotClient.JoinedChannels[0].Channel, $"@{chatMessage.Username} {commandOutput}");
            }
        }

        private void Client_OnUserJoin(object sender, OnUserJoinedArgs e)
        {
            Viewers.Add(e.Username, DateTime.UtcNow);
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            _ = Viewers.Remove(e.Username);
        }

        public void Dispose()
        {
            PeriodicMessageTimer?.Dispose();
        }
    }
}
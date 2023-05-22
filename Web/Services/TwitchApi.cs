using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.Undocumented;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.ThirdParty;
using Web.Configuration;

namespace Web.Services
{
    public class TwitchApiClient : ITwitchAPI
    {
        private readonly TwitchAPI _apiClient;
        private readonly ILogger<TwitchApiClient> _logger;
        private readonly string _channel;
        private DateTime ExpiresAt;
        private bool AccessTokenIsDeprecated => ExpiresAt > DateTime.UtcNow;

        public IApiSettings Settings
        {
            get
            {
                if (AccessTokenIsDeprecated)
                {
                    _ = GetAccessTokenAsync();
                }

                return _apiClient.Settings;
            }
        }

        public Helix Helix
        {
            get
            {
                if (AccessTokenIsDeprecated)
                {
                    _ = GetAccessTokenAsync();
                }

                return _apiClient.Helix;
            }
        }

        public ThirdParty ThirdParty
        {
            get
            {
                if (AccessTokenIsDeprecated)
                {
                    _ = GetAccessTokenAsync();
                }

                return _apiClient.ThirdParty;
            }
        }

        public Undocumented Undocumented
        {
            get
            {
                if (AccessTokenIsDeprecated)
                {
                    _ = GetAccessTokenAsync();
                }

                return _apiClient.Undocumented;
            }
        }

        public TwitchApiClient(IOptionsMonitor<TwitchApiConfiguration> twitchApiConfiguration, ILogger<TwitchApiClient> logger)
        {
            _apiClient = new();
            _apiClient.Settings.ClientId = twitchApiConfiguration.CurrentValue.ClientId;
            _apiClient.Settings.Secret = twitchApiConfiguration.CurrentValue.ClientSecret;
            _channel = twitchApiConfiguration.CurrentValue.Channel;
            _logger = logger;
        }

        private async Task GetAccessTokenAsync()
        {
            _logger.LogInformation("Updating bot access token", DateTime.UtcNow.ToLongDateString());
            using HttpClient httpClient = new();

            Dictionary<string, string> parameters = new()
            {
                { "client_id", _apiClient.Settings.ClientId },
                { "client_secret", _apiClient.Settings.Secret },
                { "grant_type", "client_credentials" },
            };

            HttpResponseMessage response = await httpClient.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(parameters));
            string responseJson = await response.Content.ReadAsStringAsync();

            _apiClient.Settings.AccessToken = Newtonsoft.Json.Linq.JObject.Parse(responseJson)["access_token"].ToString();
            ExpiresAt = DateTime.UtcNow.AddSeconds((int)Newtonsoft.Json.Linq.JObject.Parse(responseJson)["expires_in"] - 10);
        }
    }
}
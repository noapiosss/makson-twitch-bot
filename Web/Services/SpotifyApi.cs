using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Web.Configuration;
using Web.Services.Interfaces;

namespace Web.Services
{
    public class SpotifyApiClient : ISpotifyAPI
    {
        private DateTime ExpiresAt;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private string AccessToken;
        public SpotifyApiClient(IOptionsMonitor<SpotifyApiConfiguration> spotifyApiConfiguration)
        {
            _clientId = spotifyApiConfiguration.CurrentValue.ClientId;
            _clientSecret = spotifyApiConfiguration.CurrentValue.ClientSecret;
        }

        private async Task GetAccessTokenAsync()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}")));

            FormUrlEncodedContent body = new(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });

            HttpResponseMessage response = await client.PostAsync("https://accounts.spotify.com/api/token", body);
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseContent);

            AccessToken = json["access_token"].ToString();
            ExpiresAt = DateTime.UtcNow.AddSeconds((int)json["expires_in"] - 10);
        }

        public async Task<string> GetCurrentPlayingTrack()
        {
            if (AccessToken == null || ExpiresAt < DateTime.UtcNow)
            {
                await GetAccessTokenAsync();
            }

            if (!await IsCurrenltyPlaying())
            {
                return "Currently, nothing playing";
            }

            using HttpClient client = new();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/me/player/currently-playing");
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseContent);

            string trackName = json["item"]["name"].ToString();
            string artistName = json["item"]["artists"][0]["name"].ToString();

            return $"Currently playing: {trackName} by {artistName}";
        }

        private async Task<bool> IsCurrenltyPlaying()
        {
            if (AccessToken == null || ExpiresAt < DateTime.UtcNow)
            {
                await GetAccessTokenAsync();
            }

            using HttpClient client = new();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/me/player");

            string responseContent = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseContent);

            return (bool)json["is_playing"];
        }
    }
}
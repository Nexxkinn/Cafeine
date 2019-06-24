using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services.Api
{
    internal class AuthenticationModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }

        [JsonProperty("created_at")]
        public ulong? CreatedAt { get; private set; }

        [JsonProperty("expires_in")]
        public ulong? ExpiresIn { get; private set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }

        [JsonProperty("scope")]
        public string Scope { get; private set; }

        [JsonProperty("token_type")]
        public string TokenType { get; private set; }
    }

    public static class Kitsu
    {
        //Grant access(?) but unused(??), but required(???) :
        private static readonly Uri OauthURI = new Uri("https://kitsu.io/api/oauth");

        private static readonly Uri BaseURI = new Uri("https://kitsu.io/api/edge");

        private static HttpClient KitsuAuthClient = new HttpClient();

        public static int ID { get; private set; }

        public static string UserName { get; private set; }

        public static async Task Authenticate(string username, string password)
        {
            StringContent content = new StringContent(
                $"{{\"grant_type\": \"password\", \"username\": \"{username}\", \"password\": \"{password}\"}}",
                Encoding.UTF8,
                "application/vnd.api+json");
            var AuthPostAsync = await KitsuAuthClient.PostAsync($"{OauthURI}/token", content);
            AuthPostAsync.EnsureSuccessStatusCode();
            var AuthJson = await AuthPostAsync.Content.ReadAsStringAsync();
            var AuthResponse = JsonConvert.DeserializeObject<AuthenticationModel>(AuthJson);

            KitsuAuthClient.DefaultRequestHeaders.Add("Authorization", $"{AuthResponse.TokenType} {AuthResponse.AccessToken}");
            var UserResponse = await KitsuAuthClient.GetAsync($"{BaseURI}/users?filter[self]=true");
            UserResponse.EnsureSuccessStatusCode();
            var UserJson = await UserResponse.Content.ReadAsStringAsync();
            var UserInfo = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(UserJson);
            UserName = UserInfo["data"][0]["attributes"]["name"];
            ID = UserInfo["data"][0]["id"];
        }
    }
}

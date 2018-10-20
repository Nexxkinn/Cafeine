using NetJSON;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    internal class AuthenticationModel
    {
        [NetJSONProperty("access_token")]
        public string AccessToken { get; private set; }

        [NetJSONProperty("created_at")]
        public ulong? CreatedAt { get; private set; }

        [NetJSONProperty("expires_in")]
        public ulong? ExpiresIn { get; private set; }

        [NetJSONProperty("refresh_token")]
        public string RefreshToken { get; private set; }

        [NetJSONProperty("scope")]
        public string Scope { get; private set; }

        [NetJSONProperty("token_type")]
        public string TokenType { get; private set; }
    }
    public static class KitsuApi
    {
        #region setup member variables
        //Grant access(?) but unused(??), but required(???) : 
        private static readonly Uri OauthURI = new Uri("https://kitsu.io/api/oauth");
        private static readonly Uri BaseURI = new Uri("https://kitsu.io/api/edge");

        private static HttpClient KitsuAuthClient = new HttpClient();
        public static int ID { get; private set; }
        public static string UserName { get; private set; }
        #endregion

        public static async Task Authenticate(string username, SecureString password)
        {
            StringContent content = new StringContent(
                $"{{\"grant_type\": \"password\", \"username\": \"{username}\", \"password\": \"{password}\"}}",
                Encoding.UTF8,
                "application/vnd.api+json");
            var AuthPostAsync = await KitsuAuthClient.PostAsync($"{OauthURI}/token", content);
            AuthPostAsync.EnsureSuccessStatusCode();
            var AuthJson = await AuthPostAsync.Content.ReadAsStringAsync();
            var AuthResponse = NetJSON.NetJSON.Deserialize<AuthenticationModel>(AuthJson);
            
            KitsuAuthClient.DefaultRequestHeaders.Add("Authorization", $"{AuthResponse.TokenType} {AuthResponse.AccessToken}");
            var UserResponse = await KitsuAuthClient.GetAsync($"{BaseURI}/users?filter[self]=true");
            UserResponse.EnsureSuccessStatusCode();
            var UserJson = await UserResponse.Content.ReadAsStringAsync();
            var UserInfo = NetJSON.NetJSON.Deserialize<Dictionary<string,dynamic>>(UserJson);
            UserName = UserInfo["data"][0]["attributes"]["name"];
            ID = UserInfo["data"][0]["id"];
        }
        //TODO : create AddCredentials method
    }
}

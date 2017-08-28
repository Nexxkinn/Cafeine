using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Cafeine.Design.RemoteTorrent.qBittorent {
    class CoreApi {
        public static async Task<bool> Authentication(int port, string username, string password) {
            Uri uri = new Uri("http://localhost:" + port.ToString() + "/login");
            try {
                using (var client = new HttpClient()) {
                    Dictionary<string, string> authentication = new Dictionary<string, string>();
                    authentication.Add("username", username);
                    authentication.Add("password", password);

                    HttpFormUrlEncodedContent x = new HttpFormUrlEncodedContent(authentication);
                    var result = await client.PostAsync(uri, x);

                    //store user credential and port
                    var vault = new PasswordVault();
                    var cred = new PasswordCredential("qBittorent", username, password);
                    vault.Add(cred);
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["localport"] = port.ToString();
                    GC.Collect();
                }
                return await Task.FromResult(true);
            }
            catch(Exception) {
                return await Task.FromResult(false);
            }
        }
        private static string GetKey() {
            string skey;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string port = (string)localSettings.Values["localport"];
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter()) {
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri("http://localhost:" + port + "/login"));
                skey = cookieCollection.First().Value;
            }
            return skey;
        }
        public static async Task<string> GetASync(string path) {
            string key = GetKey();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string port = (string)localSettings.Values["localport"];
            Uri uri = new Uri(new Uri("http://localhost:"+port),path);
            try {
                using (HttpClient client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Cookie", "SID=" + key);
                    var result = await client.GetAsync(uri);
                    string output = result.Content.ToString();
                    return await Task.FromResult(output);
                }
            }
            catch(Exception) {
                return null;
            }
        }
        public static async Task<bool> PostAsync(string path,IHttpContent content) {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string port = (string)localSettings.Values["localport"];
            Uri uri = new Uri(new Uri("http://localhost:"+port), path);
            try {
                using (var client = new HttpClient()) {
                    var result = await client.PostAsync(uri, content);
                    GC.Collect();
                }
                return await Task.FromResult(true);
            }
            catch (Exception) {
                return await Task.FromResult(false);
            }
        }
    }
}

using System;
using Windows.Security.Credentials;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Threading.Tasks;
namespace Cafeine.Services
{
    class Logincredentials
    {
        private string using_service = null;
        /// <summary>
        /// Check user's credentials
        /// </summary>
        /// <param name="username">user's username</param>
        /// <param name="password">user's password</param>
        /// <param name="_using_service">1 -> Myanimelist, 2-> Hummingbird, 3-> Anico</param>
        /// <returns></returns>
        public async Task<bool> logincredential(string username, string password, int _using_service)
        {
            switch (_using_service)
            {
                case 1: using_service = "MAL"; break;
                case 2: using_service = "Hummingbird"; break;
                case 3: using_service = "Anico"; break;
            }
            try
            {
                var url = new Uri("https://myanimelist.net/api/account/verify_credentials.xml");
                var clientHeader = new HttpBaseProtocolFilter();
                clientHeader.ServerCredential = new PasswordCredential(using_service, username, password);
                clientHeader.AllowUI = false;
                using (var client = new HttpClient(clientHeader))
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string getrespond = response.Content.ReadAsStringAsync().ToString();
                    //store password
                    var vault = new PasswordVault();
                    var cred = clientHeader.ServerCredential;
                    vault.Add(cred);
                    return true;
                }
                #region oldcode_but_works
                // problem : can't disable basic authentication pop-up when failed to login.
                /*using (HttpClient client = new HttpClient()) {
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("basic",Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        string getresponse = await response.Content.ReadAsStringAsync();
                        test.Content = getresponse;
                    }
                  }
                */
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string getusername(int status)
        {
            var get_user = new Logincredentials().getcredentialfromlocker(1);
            return get_user.UserName;
        }
        public PasswordCredential getcredentialfromlocker(int srvc)
        {
            switch (srvc)
            {
                case 1: using_service = "MAL"; break;
                case 2: using_service = "Hummingbird"; break;
                case 3: using_service = "Anico"; break;
            }
            PasswordCredential credential = null;
            var vault = new PasswordVault();
            try
            {
                var thelist = vault.FindAllByResource(using_service);
                credential = thelist[0];
                return credential;
            }
            catch( Exception e)
            {
                return null;
            }
        }
    }
}
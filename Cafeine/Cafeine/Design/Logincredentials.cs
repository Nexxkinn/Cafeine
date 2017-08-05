using System;
using Windows.Security.Credentials;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
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
                byte[] bytes = Encoding.UTF8.GetBytes(username + ":" + password);
                string LoginToBase64 = Convert.ToBase64String(bytes);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + LoginToBase64);
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    //store password
                    var vault = new PasswordVault();
                    var cred = new PasswordCredential(using_service, username, password);
                    vault.Add(cred);
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }
        public static PasswordCredential getuser(int status)
        {
            var get_user = new Logincredentials().getcredentialfromlocker(1);
            if(get_user != null) {
                get_user.RetrievePassword();
            };
            return get_user;
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}
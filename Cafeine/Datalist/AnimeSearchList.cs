using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Data;

namespace Cafeine.Datalist
{
    class AnimeSearchList
    {
        public string username = "todata";
        public string password = "pass";
        private string responseBody;
        private async void connecttosearchdata()
        {
            try
            {
                
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));

                    using (HttpResponseMessage response = client.GetAsync("http://myanimelist.net/api/anime/search.xml?q=fullmetal+alchemist").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        responseBody = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private string getresult()
        {
            return responseBody;
        }
    }
}

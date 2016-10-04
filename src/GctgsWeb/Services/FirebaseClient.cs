using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GctgsWeb.Models;
using Newtonsoft.Json;

namespace GctgsWeb.Services
{
    public class FirebaseClient
    {
        private readonly FirebaseSettings _firebaseSettings;

        public FirebaseClient(FirebaseSettings firebaseSettings)
        {
            _firebaseSettings = firebaseSettings;
        }

        public async Task SendRequestNotification(string token, User requester, BoardGame boardGame)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + _firebaseSettings.ApiKey);
                var notification = JsonConvert.SerializeObject(new
                {
                    to = token,
                    notification = new
                    {
                        title = "Board Game Request",
                        body = requester.Name + " would like to play " + boardGame.Name + "!",
                        color = "#4CAF50"
                    },
                    data = new
                    {
                        requester,
                        boardGame
                    }
                });
                var content = new StringContent(notification, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(_firebaseSettings.BaseUri, content);

            }
        }
    }
}

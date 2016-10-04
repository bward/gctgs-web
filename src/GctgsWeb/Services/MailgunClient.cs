using GctgsWeb.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GctgsWeb.Services
{
    public class MailgunClient
    {
        private readonly EmailSettings _emailSettings;

        public MailgunClient(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task SendNotificationEmail(string email, string to, string from, string boardGame)
        {
            await SendEmailAsync(email,
                from + " would like to play " + boardGame + "!",
                "Hi " + to + "!\n\n"
                + from + " has asked for a game of " +  boardGame + ". Why not bring it to the next meeting?"
                + "\n\nHave fun,\nGCTGS Bot xoxo");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(_emailSettings.BaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_emailSettings.ApiKey)));
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", _emailSettings.From),
                    new KeyValuePair<string, string>("to", email),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", message)
                });

                await client.PostAsync(_emailSettings.RequestUri, content).ConfigureAwait(false);
            }
        }
    }
}

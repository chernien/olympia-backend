using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Olympia.Services
{
    public class SmsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _senderId;

        public SmsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WinSms:ApiKey"];
            _senderId = configuration["WinSms:SenderId"];
        }

        public async Task<string> SendSmsAsync(string to, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(message))
                    return "Numéro ou message vide.";

                // Nettoyage du numéro de téléphone
                to = to.Replace("+", "").Replace(" ", "").Replace("00", "");

                string url = $"https://www.winsmspro.com/sms/sms/api?action=send-sms" +
                             $"&api_key={_apiKey}" +
                             $"&to={to}" +
                             $"&from={_senderId}" +
                             $"&sms={Uri.EscapeDataString(message)}";

                var response = await _httpClient.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return $"Erreur lors de l'envoi du SMS : {ex.Message}";
            }
        }
    }
}

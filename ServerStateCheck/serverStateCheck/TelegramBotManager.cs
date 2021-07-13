using System;
using System.Net;
using System.IO;

namespace supermango
{
    public class TelegramBotManager
    {

        private static readonly string _baseUrl = @"https://api.telegram.org/bot";
        private static readonly string _token = "Input Telegram Room Token";//Room Token
        public static string _chatID = "Input Bot Chat ID"; //BotID
        static SecurityProtocolType securityDefault = ServicePointManager.SecurityProtocol;

        public static void SendMessage(string text)
        {
            securityDefault = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                text = text.Replace('#', '@');
                text = text.Replace('&', '@');//텔레그램
                string Url = string.Format("{0}{1}/sendMessage?chat_id={2}&text={3}", _baseUrl, _token, _chatID, text);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.GetResponse();
                ServicePointManager.SecurityProtocol = securityDefault;
            }
            catch (Exception e)
            {
                ServicePointManager.SecurityProtocol = securityDefault;
            }

        }

    }
}
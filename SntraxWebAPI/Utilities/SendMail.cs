using EnttlOrchestrationLayer.Utilities;
using System.Net.Mail;


namespace SntraxWebAPI.Utilities
{
    public class SendMail
    {

        private static string _function = string.Empty;
        private static string _server = string.Empty;

        public SendMail()
        { }

        public SendMail(string function, string server)
        {
            _function = function;
            _server = server;
        }


        public void SendEmail(string body)
        {
            string recipent = AppConstants.EMAIL_RECIPENT;
            string sender = AppConstants.EMAIL_SENDER.ToString();
            string mailServer = AppConstants.EMAIL_SERVER;
            string subject = "Error in SNTrax service";

            if (_function != "")
                subject = subject + " - " + _function + " @ " + _server;
            else
                subject = subject + " @ " + _server;

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(new MailAddress(recipent));
                message.Subject = subject;
                message.Body = body;

                var client = new SmtpClient();
                client.Host = mailServer;
                client.Port = 465;
                client.EnableSsl = true;
               // client.Credentials = new NetworkCredential("your_username", "your_password");
                client.Send(message);

            }
            catch (Exception ex)
            {

            }
        }
    }

}

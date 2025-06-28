
using System.Net;
using MimeKit;
using System.Net.Mail;
using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Common;
using AIBDecrypt;
using DocumentFormat.OpenXml.Wordprocessing;
//using EASendMail;
namespace Operational_Risk_Management.Models.Services
{
    public interface IEmailSender
    {
        public Task<bool> SendAsync_SMTP(string toMail, string subject, string text,bool isBodyHtml=false, string? cc = null);
    }
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _emailSettings;
        private string smtpServer;
        private string senderEmail;
        private string password;
        private byte[] _iv = [206, 13, 15, 80, 97, 175, 4, 89, 174, 180, 38, 33, 44, 74, 70, 69];
        private string _secret = "G*7N;{$-s`ZqXE_n&N:X,65;";
        public EmailSender(ILogger<EmailSender> logger, IConfiguration emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings;
            smtpServer = _emailSettings["EmailSettings:Host"];
            senderEmail = _emailSettings["EmailSettings:SenderEmail"];
            password = Decryptor.Decrypt(_emailSettings["EmailSettings:Password"]!,_secret,_iv );
        }
        public async Task<bool> SendAsync_SMTP(string toMail, string subject, string text,bool isBodyHtml= false, string? cc=null)
        {
            _logger.LogInformation("{0}: sending email over smtp server {1} started...", DateTime.Now,smtpServer);
            try
            {
                // Create a new mail message
                MailMessage mail = new MailMessage(); 
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(toMail); 
                if(cc != null)
                {
                    mail.CC.Add(cc);
                }
                mail.Subject = subject; 
                mail.Body = text; 
                mail.IsBodyHtml = isBodyHtml; 
                // Set up the SMTP server configuration
                SmtpClient smtpClient = new SmtpClient(smtpServer) 
                { 
                    Port = 587, 
                    // or your specific SMTP server port
                    Credentials = new NetworkCredential(senderEmail, password), 
                    EnableSsl = true ,
                    
                };
                // Send the email
                await smtpClient.SendMailAsync(mail);
                
                _logger.LogInformation("{0}: email sent successfully from {1} to {2}", DateTime.Now, senderEmail, toMail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("{0}: error during sendind email from {1} to {2}", DateTime.Now, senderEmail, toMail);
                _logger.LogError(ex.Message);
                return false;

            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BookShopApp.webui.EmailServices
{
    public class SmtpEmailSender : IEmailSender
    {
        private string _host;
        private int _port;
        private bool _enableSSL;
        private string _userName;
        private string _password;

        public SmtpEmailSender(string host, int port, bool enableSSLL, string username, string password)
        {
            //string host -> server'ın bir adresi olacak
            //hangi port uzerınden bılgı gonderıldıgını anlamak ıcın port bılgısı -> int port
            //enableSSL-> şifreleme olayının true/false olması
            this._host = host;//this._host bilgisi dısarıdan gonderılen host bılgısıyle set edilir
            this._port = port;
            this._enableSSL = enableSSLL;
            this._userName = username;
            this._password = password;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //client-> müsteri
            var client = new SmtpClient(this._host, this._port)
            {
                //credentials -> kimlik bilgileri
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = this._enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(this._userName, email, subject, htmlMessage)
                {
                    IsBodyHtml = true
                }
            );
        }  
    }    
}

using MailBee.ImapMail;
using System;

namespace MailHelperLib
{
    public class IMAPMails
    {
        public void IMAPListMailDownLoad(string account,string password,string serverName,int serverPort=993)
        {
            MailBee.Global.LicenseKey = LicenseKey.Key;//插件注册
            
            Imap imap = new Imap();
            imap.SslProtocol = MailBee.Security.SecurityProtocol.Tls11;
            try
            {
                imap.Connect(serverName, serverPort);
                imap.Login(account, password);
            }
            catch
            {
                throw new Exception("登陆失败");
            }
            if (!imap.SelectFolder("Inbox")) throw new Exception("收件箱查找错误");

            var i = imap.MessageCount;
            var Mes= imap.DownloadEntireMessages(string.Format("{0}:1",i), false);
        }
        public void IMAPMailDownLoad(string account, string password, string serverName, int serverPort=993)
        {
            MailBee.Global.LicenseKey = LicenseKey.Key;//插件注册

            Imap imap = new Imap();
            imap.SslProtocol = MailBee.Security.SecurityProtocol.Tls11;
            try
            {
                imap.Connect(serverName, serverPort);
                imap.Login(account, password);
            }
            catch
            {
                throw new Exception("登陆失败");
            }
            if (!imap.SelectFolder("Inbox")) throw new Exception("收件箱查找错误");
            
            var Mes = imap.DownloadEntireMessages(string.Format("{0}:1", 1), false);
        }
    }
}

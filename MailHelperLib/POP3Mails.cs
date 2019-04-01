using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailBee.Pop3Mail;

namespace MailHelperLib
{
    public class POP3Mails
    {
        public void POPListMailDownLoad(string account, string password, string serverName, int serverPort=995)
        {
            MailBee.Global.LicenseKey = LicenseKey.Key;

            Pop3 pop = new Pop3();
            try {
                pop.Connect(serverName, serverPort);
                pop.Login(account, password);
            }
            catch {
                throw new Exception("登陆失败");
            }
            var ss = pop.DownloadEntireMessages();
        }
        public void POPMailDownLoad(string account, string password, string serverName, int serverPort=995)
        {
            MailBee.Global.LicenseKey = LicenseKey.Key;

            Pop3 pop = new Pop3();
            try
            {
                pop.Connect(serverName, serverPort);
                pop.Login(account, password);
            }
            catch {
                throw new Exception("登陆失败");
            }
            var i = pop.InboxMessageCount;
            var ss = pop.DownloadEntireMessages(i, 1);
        }
    }
}

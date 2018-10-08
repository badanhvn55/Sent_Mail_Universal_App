using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Limilabs.Mail;
using Limilabs.Client.IMAP;
using Windows.UI.Popups;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App_Send_Mail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Inbox : Page
    {

        string USER = "badanhht@gmail.com";
        string PASSWORD = "nbdvn12345";

        public Inbox()
        {
            this.InitializeComponent();
            receiveMail();
            list.Items.Add("One");
            list.Items.Add("Two");
            list.Items.Add("Three");

        }

        private async void receiveMail()
        {
            using (Imap imap = new Imap())
            {
                await imap.ConnectSSL("imap.gmail.com");
                await imap.UseBestLoginAsync(USER, PASSWORD);
                await imap.SelectInboxAsync();
                List<long> uids = await imap.SearchAsync(Flag.Unseen);
                foreach (long uid in uids)
                {
                    var eml = await imap.GetMessageByUIDAsync(uid);
                    IMail mail = new MailBuilder().CreateFromEml(eml);
                    //Console.WriteLine(mail.Subject);
                    //Console.WriteLine(mail.Text);
                    string content = mail.Subject + " - " + mail.Text;
                    list.Items.Add(content);
                }
                await imap.CloseAsync();
                MessageDialog msg = new MessageDialog("Mail has been received");

                msg.ShowAsync();
            }

        }
    }
}

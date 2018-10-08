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

using Windows.Storage;

using Windows.UI.Popups;

using Limilabs.Client.SMTP;
using Limilabs.Client.IMAP;

using Limilabs.Mail.Headers;

using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App_Send_Mail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //THAY USER VỚI PASSWORD CỦA EMAIL VÀO DƯỚI ĐÂY NHA
        string USER = "nguyengoc.195@gmail.com";
        string PASSWORD = "futurefight";

        FileOpenPicker FilePick;

        StorageFile AttachFile;

        public MainPage()
        {
            this.InitializeComponent();
            receiveMail();
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
                    string content = mail.Subject;
                    list.Items.Add(content);
                }
                await imap.CloseAsync();
            }

        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["Detail"];
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["Normal"];
            }
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)

        {



        }

        private async void AttachFileBtn_Click(object sender, RoutedEventArgs e)

        {

            FilePick = new FileOpenPicker();

            FilePick.FileTypeFilter.Clear();

            FilePick.FileTypeFilter.Add(".doc");

            FilePick.FileTypeFilter.Add(".png");

            FilePick.FileTypeFilter.Add(".txt");

            FilePick.FileTypeFilter.Add(".jpg");

            AttachFile = await FilePick.PickSingleFileAsync();

        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)

        {

            MailBuilder myMail = new MailBuilder();

            myMail.Html = MsgText.Text;

            myMail.Subject = SubText.Text;

            await myMail.AddAttachment(AttachFile);

            myMail.To.Add(new MailBox(ToText.Text));

            myMail.From.Add(new MailBox(FromText.Text));



            IMail email = myMail.Create();



            try

            {

                using (Smtp smtp = new Smtp())

                {

                    await smtp.Connect("smtp.gmail.com", 587);

                    await smtp.UseBestLoginAsync(USER, PASSWORD);

                    await smtp.SendMessageAsync(email);

                    await smtp.CloseAsync();

                    MessageDialog msg = new MessageDialog("Mail has been sucessfully sent");

                    msg.ShowAsync();

                }

            }

            catch (Exception ex)

            {

                MessageDialog msg = new MessageDialog("Error in mail send");

                msg.ShowAsync();

            }

        }

    }
}

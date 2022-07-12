using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Mime;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace Post
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        string Login;
        string Pass;
        List<Message> messages;

        public Window1(string Login, string Pass)
        {
            InitializeComponent();
            this.Login = Login;
            this.Pass = Pass;
            messages = new List<Message>();
        }
        void GetMail()
        {
            Pop3Client pop3Client = new Pop3Client();
            pop3Client.Connect("pop.yandex.ru", 995, true);
            pop3Client.Authenticate(Login, Pass);
            Message msg = pop3Client.GetMessage(1);
            LB.Items.Clear();
            for (int i=0; i<10; i++)
            {
                messages.Add(pop3Client.GetMessage(i+1));
                LB.Items.Add(messages[i].MessagePart.GetBodyAsText());

            }

            foreach (Message msg1 in messages)
            {
                LB.Items.Add(msg1.Headers.Subject);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MailAddress mailAddressTo = new MailAddress("mail");
            MailAddress mailAddressFrom = new MailAddress("mail");
            MailMessage mail = new MailMessage(mailAddressFrom, mailAddressTo);
            mail.Subject = Subject.Text;

            FlowDocument flow = Body.Document;
            TextRange textRange = new TextRange(flow.ContentStart, flow.ContentEnd);

            mail.Body = textRange.Text;
            SmtpClient smtpClient = new SmtpClient("smt.yandex.ru", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(Login, Pass);
            smtpClient.Send(mail);

            To.Text = "";
            From.Text = "";
            Subject.Text = "";
            Body.Document = new FlowDocument();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetMail();
        }

        private void LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB.SelectedIndex>=0)
            {
                string text = messages[LB.SelectedIndex].MessagePart.GetBodyAsText();
                Body.AppendText(text);
            }
        }
    }
}

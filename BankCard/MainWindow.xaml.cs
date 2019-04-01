using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankCard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TaskCB();
            //new LoginWindow().ShowDialog();
            //new MailHelperLib.POP3Mails().POPMailDownLoad("laio45@163.com","chendaigao4","pop.163.com",995);
        }
        private void TaskCB()
        {
            Task.Factory.StartNew(() =>
            {
                new MailHelperLib.CreateButtons().CreateButton(rv =>
                {
                    CreateButton(rv.ToString());
                });
            });
        }

        public void CreateButton(string content)
        {
            stackPanelLeftDateTimeCard.Dispatcher.Invoke(() =>
            {
                Button button = new Button();
                button.ToolTip = content;
                if (content.Contains(DateTime.Now.Year.ToString()))
                    content = content.Split('-')[1];

                button.Content = content;
                button.Height = 30;
                button.Margin = new Thickness(0, 3, 0, 3);
                button.Click += Button_Click;
                stackPanelLeftDateTimeCard.Children.Add(button);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanelRightDetails.Children.Clear();
            new MailHelperLib.CreateButtons().SelectYearsMonth(button.ToolTip.ToString(), rv =>
            {
                Button btn = new Button();
                btn.Content = rv;
                btn.Height = 30;
                StackPanelRightDetails.Children.Add(btn);
            });
        }

    }
}

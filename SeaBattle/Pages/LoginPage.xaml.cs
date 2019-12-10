using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleships.Model;
using Battleships.Extensions;

namespace Battleships.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    /// 

    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ipAdress in localIP)
                if (ipAdress.AddressFamily == AddressFamily.InterNetwork) ServerIpAdress.Text = ipAdress.ToString();
            ConnectIpAdress.Text = ServerIpAdress.Text;
        }

        private async void ServerStartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await Battle.ServerManager.StartServer(IPAddress.Parse(ServerIpAdress.Text),ServerPort.Text))
                    ConnectionLog.AppendText("Server started! Waiting for users...", Brushes.Green);

                CreateServerButton.IsEnabled = false;
                ConnectButton.IsEnabled = false;

                await WaitForOpponent();

                Battle.Info.IsHost = true;
                Battle.Info.IsMyTurn = true;

                Battle.ServerManager.RunRecive();

                ContinueButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CreateServerButton.IsEnabled = true;
                ConnectButton.IsEnabled = true;

                ConnectionLog.AppendText(ex);
            }
        }

        private async Task WaitForOpponent()
        {
            try
            {
                await Battle.ServerManager.WaitServerForUsers();
                ConnectionLog.AppendText($"User connected!\n" +
                                             $"Local end point : {Battle.ServerManager.Client.LocalEndPoint}\n" +
                                             $"Remote end point: {Battle.ServerManager.Listener.RemoteEndPoint}", Brushes.Green);
                ConnectionLog.ScrollToEnd();
            }
            catch (Exception ex)
            {
                ConnectionLog.AppendText(ex);
            }
        }

        #region Buttons events

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Battle.ServerManager.ConnectTo(IPAddress.Parse(ConnectIpAdress.Text), Convert.ToInt32(ConnectionPort.Text));

                ConnectionLog.AppendText($"Connected!\n" +
                                         $"Local end point : {Battle.ServerManager.Client.LocalEndPoint}\n" +
                                         $"Remote end point: {Battle.ServerManager.Listener.RemoteEndPoint}", Brushes.Green);

                Battle.Info.IsHost = false;
                Battle.Info.IsMyTurn = false;

                Battle.ServerManager.RunRecive();

                CreateServerButton.IsEnabled = false;
                ConnectButton.IsEnabled = false;

                ContinueButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CreateServerButton.IsEnabled = true;
                ConnectButton.IsEnabled = true;

                ConnectionLog.AppendText(ex);
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.GoToPage(PageType.SetUp);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Battle.ServerManager.CloseAllConnections();
            PageHelper.GoToPage(PageType.MainMenu);
        }

        #endregion

        private bool IsValidIp(string ip)
        {
            Regex ipPattern = new Regex(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$");
            return ipPattern.IsMatch(ip);
        }

        private void ConnectIpAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidIp(ConnectIpAdress.Text))
            {
                ConnectIpAdress.Foreground = Brushes.DarkBlue;
                if (ConnectButton != null) ConnectButton.IsEnabled = true;
            }
            else
            {
                ConnectIpAdress.Foreground = Brushes.Red;
                if (ConnectButton != null) ConnectButton.IsEnabled = false;
            }
        }

        private void ServerIpAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidIp(ServerIpAdress.Text))
            {
                ServerIpAdress.Foreground = Brushes.DarkBlue;
                if (CreateServerButton != null) CreateServerButton.IsEnabled = true;
            }
            else
            {
                ServerIpAdress.Foreground = Brushes.Red;
                if (CreateServerButton != null) CreateServerButton.IsEnabled = false;
            }
        }
    }
}

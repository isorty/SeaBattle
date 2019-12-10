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
using Battleships.Model;
using Battleships.Extensions;

namespace Battleships.Pages
{
    /// <summary>
    /// Interaction logic for Fight.xaml
    /// </summary>
    public partial class Fight : Page
    {
        public Fight()
        {
            InitializeComponent();
            Battle.ServerManager.ChatStream = ChatWindow;
        }

        public Fight(PlayerField f): this()
        {
            myField.CopyInfo(f);

            Battle.Player.PlayerField = myField;
            Battle.Player.EnemyField = EnemyField;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextToSend.Text.Length == 0) return;

            Battle.Enemy.SendChatMessage(TextToSend.Text);

            ChatWindow.AppendText("Me: " + TextToSend.Text, Brushes.Green);
            TextToSend.Clear();
            TextToSend.Focus();
        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendButton_Click(null, null);
        }
    }
}

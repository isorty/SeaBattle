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
using Battleships.Core;
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
            Game.ConnectionManager.ChatStream = ChatWindow;
        }

        public Fight(MyField f): this()
        {
            myField.CopyInfo(f);

            Game.me.myField = myField;
            Game.me.enemyField = EnemyField;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextToSend.Text.Length == 0) return;

            Game.opponent.SendChatMessage(TextToSend.Text);

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

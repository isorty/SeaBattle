using System.Windows;
using System.Windows.Controls;
using Battleships.Model;
using Battleships.Extensions;

namespace Battleships.Pages
{
    /// <summary>
    /// Interaction logic for SetUp.xaml
    /// </summary>
    /// 
    public partial class SetUp : Page
    {
        public SetUp()
        {
            InitializeComponent();
            DataContext = this;
        }
      
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            myField.Reset();
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            myField.SetRandom();
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            await Battle.Enemy.SendGameMessage(GameMessageType.End);
            Battle.ServerManager.CloseAllConnections();
            PageHelper.GoToPage(PageType.MainMenu);
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Battle.Info.IsIReady = true;
            await Battle.Enemy.SendGameMessage(GameMessageType.Ready);
            PageHelper.GoToPage(PageType.Fight, myField);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            myField.DeleteSelectedShip();
        }
    }  
}

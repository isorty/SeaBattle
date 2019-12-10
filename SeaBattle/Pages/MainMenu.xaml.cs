using System.Windows;
using System.Windows.Controls;
using Battleships.Model;
using Battleships.Extensions;

namespace Battleships.Pages
{
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void PlayVsHuman_Click(object sender, RoutedEventArgs e)
        {
            Battle battle = new Battle(BattleType.Online);
            PageHelper.GoToPage(PageType.Login);
        }

        private void PlayVsAI_Click(object sender, RoutedEventArgs e)
        {
            Battle battle = new Battle(BattleType.Offline);
            PageHelper.GoToPage(PageType.SetUp);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (Battle.ServerManager != null)
                Battle.ServerManager.CloseAllConnections();
            Application.Current.Shutdown();
        }
    }
}

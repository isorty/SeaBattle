using System.Windows;
using System.Windows.Controls;
using Battleships.Core;
using Battleships.Extensions;

namespace Battleships.Pages
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void PlayVsHuman_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsHuman);
            PageHelper.GoToPage(PageType.Login);
        }

        private void PlayVsAI_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(GameType.VsAI);
            PageHelper.GoToPage(PageType.SetUp);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (Game.ConnectionManager != null)
                Game.ConnectionManager.CloseAllConnections();
            Application.Current.Shutdown();
        }
    }
}

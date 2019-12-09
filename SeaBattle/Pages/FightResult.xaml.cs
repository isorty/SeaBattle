using System.Windows;
using System.Windows.Controls;
using Battleships.Core;
using Battleships.Extensions;

namespace Battleships.Pages
{
    /// <summary>
    /// Interaction logic for FightResult.xaml
    /// </summary>
    public partial class FightResult : Page
    {
        public FightResult()
        {
            InitializeComponent();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Game.ConnectionManager.CloseAllConnections();

            PageHelper.GoToPage(PageType.MainMenu);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }


}

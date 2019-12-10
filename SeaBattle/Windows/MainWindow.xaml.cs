using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Battleships.Extensions;

namespace Battleships.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer backMusic;
        private bool isBackMusic = false;

        public MainWindow()
        {
            InitializeComponent();

            PageHelper.Frame = Main;
            PageHelper.GoToPage(PageType.MainMenu);

            backMusic = new MediaPlayer();
            backMusic.Open(new System.Uri(@"..\..\Resources\battleground.wav", System.UriKind.Relative));
            backMusic.Volume = 0.3;
            backMusic.Play();
            isBackMusic = true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void Minimaze_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BackMusic_Click(object sender, RoutedEventArgs e)
        {
            if (isBackMusic)
            {
                backMusic.Pause();
                isBackMusic = false;
            }
            else
            {
                backMusic.Play();
                isBackMusic = true;
            }
        }
    }
}

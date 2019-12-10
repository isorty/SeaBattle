using System;
using System.Windows;
using System.Windows.Controls;
using Battleships.Model;
using Battleships.Pages;

namespace Battleships.Extensions
{
    public class PageHelper
    {
        private static Frame mainWindowFrame;
        public static Frame Frame
        {
            get
            {
                return mainWindowFrame;
            }
            set
            {
                if (value is Frame) mainWindowFrame = value;
            }
        }
        

        public static void GoToPage(PageType pageType, object Parameters = null)
        {
            if (mainWindowFrame == null)
            {
                MessageBox.Show("Frame not assigned!");
                throw new Exception("Frame not assigned!");
            }

            switch (pageType)
            {
                case PageType.MainMenu:
                    {
                        mainWindowFrame.Content = new MainMenu();
                        break;
                    }
                case PageType.Login:
                    {
                        mainWindowFrame.Content  = new LoginPage();
                        break;
                    }
                case PageType.SetUp:
                    {
                        mainWindowFrame.Content = new SetUp();
                        break;
                    }
                case PageType.Fight:
                    {
                        mainWindowFrame.Content = new Fight((PlayerField)Parameters);
                        break;
                    }
                case PageType.FightResult:
                    {
                        mainWindowFrame.Content = new FightResult();
                        break;
                    }
                default:
                    {
                        mainWindowFrame.Content = new MainMenu();
                        MessageBox.Show("Wrorg page type!");
                        throw new Exception("Wrorg page type!");
                    }
            }
        }
    }

    public enum PageType
    {
        MainMenu,
        Login,
        SetUp,
        Fight,
        FightResult
    }
}

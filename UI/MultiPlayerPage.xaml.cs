using System;
using ChessGame8.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для MultiPlayerPage.xaml
    /// </summary>
    public partial class MultiPlayerPage : Page
    {
       private GameController _gameController;
       
        public MultiPlayerPage()
        {
            InitializeComponent();
            _gameController = new GameController();
            UpdateIndicators();

        }
        private void UpdateIndicators()
        {
            PlayersCountLabel.Text = $"Количество игроков: {_gameController.Players.Count}";
            CurrentPlayerLabel.Text = $"Текущий игрок: {_gameController.CurrentPlayer.Name}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

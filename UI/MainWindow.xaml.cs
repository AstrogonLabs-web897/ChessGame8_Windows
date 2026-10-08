using ChessGame8.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using wpfMessageBox = System.Windows.MessageBox;
namespace ChessGame8.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Settings Settings { get;  set; }
        public  GameController controller;
        //private readonly BitmapImage? defaultImage;
        public Frame MainFrame { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Settings = new Settings();
            if (controller != null) {controller = new GameController();}
            else { wpfMessageBox.Show("Контроллер не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);}
            if (MainFrame != null)
            {
                // Создаем пустые данные или дефолтные значения
                List<Player> players = new List<Player>(); // Пустой список игроков
                GameModeEnum selectedMode = GameModeEnum.Unknown; // Отсутствие выбора режима

                // Сразу переходим на страницу StartMenu с пустыми данными
                MainFrame.Navigate(new StartMenu(players, selectedMode));
            }
            else { wpfMessageBox.Show("Фрейм не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);}
        }
        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
        //private void SettingsButton_Click(object sender, RoutedEventArgs e)
        //{
            //MainFrame.NavigationService.Navigate(new SettingsPage(Settings));
        //}

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем пустые данные или дефолтные значения
            List<Player> players = new List<Player>(); // Пустой список игроков
            GameModeEnum selectedMode = GameModeEnum.Unknown; // Отсутствие выбора режима

            // Переводим пользователя на страницу StartMenu
            MainFrame.Navigate(new StartMenu(players, selectedMode));
        }
    }
}
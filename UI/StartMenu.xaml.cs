using System;
using ChessGame8.WaitingRoom;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using wpfMessageBox = System.Windows.MessageBox;

namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для StartMenu.xaml
    /// </summary>
    public partial class StartMenu : Page
    {
        // Списки игроков и выбранный режим
        private List<Player> _players;
        private GameModeEnum _selectedMode;
        public StartMenu(List<Player> players, GameModeEnum mode)
        {
            InitializeComponent();
            _players = players;
            _selectedMode = mode;
            // Логика обработки принятых данных
            ProcessReceivedData();
        }

        // Метод обработки полученных данных
        private void ProcessReceivedData()
        {
            // Например, выводим настройки в лог
            wpfMessageBox.Show($"Имя игрока: {_players.First().Name}\nРежим игры: {_selectedMode}\nЦвет игрока: {_players.First().Color}");
        }

        // Обработчик события нажатия кнопки "Начать игру"
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = PlayerNameInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                wpfMessageBox.Show("Пожалуйста, введите своё имя.");
                return;
            }
            // Создаем экземпляр Settings
            var settings = new Settings();
            // Создаем список игроков (берем первого из пресетов)
            var players = new List<Player> { Settings.PresetPlayers[0] };

            // Получаем выбранный режим игры
            var gameMode = settings.SelectedGameMode;
            // Выбор режима игры
            var selectedMode = GameMode.SelectedMode;
            if (selectedMode == GameModeEnum.SinglePlayerVsComputer)
            {
                // Одинокий игрок против ИИ
                NavigateToGameScreen(new[] { new Player(playerName, PlayerColor_1.White,0) }, selectedMode);
            }
            else if (selectedMode == GameModeEnum.Multiplayer2v2 || selectedMode == GameModeEnum.Multiplayer4v4 || selectedMode == GameModeEnum.Miltiplayer2VsComputer)
            {
                // Несколько игроков против ИИ
                EnterMultiplayerNames(); // Вызываем дополнительный шаг для ввода имён игроков
            }
            else
            {
                //Во всех остальных случаях selectedMode = GameModeEnum.Unknown
                selectedMode = GameModeEnum.Unknown;
                // Далее ничего не делаем 
   
            }
        }
        // Например, можно перейти на другую страницу или открыть новую форму
        private void NavigateToGameScreen(Player[] Players, GameModeEnum mode)
        {
            // Создаем список игроков (берем первого из пресетов)
            var players = new List<Player> { Settings.PresetPlayers[0] };
            NavigationService.Navigate(new GameScreen(players, mode));
        }
        private void EnterMultiplayerNames()
        {
            // Логика для ввода имён нескольких игроков
            // Здесь можно добавить дополнительное окно или страницу для ввода имён
            // Пока просто для примера
            var players = new[]
            {
                new Player(PlayerNameInput.Text, PlayerColor_1.White, 0),
                new Player("Second Player", PlayerColor_1.Black, 0) // Можно предложить ввести второе имя
            };
            StartMultiplayerGame_Checked(null, null);
        }
        private void StartMultiplayerGame_Checked(object? sender, RoutedEventArgs? e)
        {
            // Пользователь выбрал мультиплеер, переходим на страницу ожидания
            NavigationService.Navigate(new WaitingRoomPage());
        }
        private void  GameOptionsButton_Click(object sender, RoutedEventArgs e) 
        {
            NavigationService.Navigate(new GameOptions());
        }
        #region Comments
        //private void StartGameSinglePlayer_Click(object sender, RoutedEventArgs e)
        //{
        // Создаем список игроков (берем первого из пресетов)
        //var players = new List<Player> { Settings.PresetPlayers[0] };
        // Создаем экземпляр Settings
        //var settings = new Settings();
        //var gameMode = settings.SelectedGameMode;
        // Пользователь выбрал одиночную игру, переходим на страницу игры
        //NavigationService.Navigate(new GameScreen(players, gameMode ));
        //}
        //private void PlayerNameInput_TextChanged(object sender, TextChangedEventArgs e)
        //{
        // Обрабатываем изменение текста в реальном времени
        //UpdatePlayerName(PlayerNameInput.Text);
        //}

        //private void PlayerNameInput_LostFocus(object sender, RoutedEventArgs e)
        //{
        // Обрабатываем фокусировку вне текстового поля
        //UpdatePlayerName(PlayerNameInput.Text);
        //}

        //private void PlayerNameInput_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //if (e.Key == Key.Enter)
        //{
        // Обрабатываем ввод имени игрока
        //UpdatePlayerName(PlayerNameInput.Text);

        // Переводим фокус на кнопку "Start Game"
        //StartGameButton.Focus();

        // Блокируем дальнейшее распространение события
        //e.Handled = true;
        //}
        //}
        //private void UpdatePlayerName(string playerName)
        //{
        // Здесь можно обновить имя игрока в модели или где угодно
        // Например, сохраним имя в настройках
        //Settings.Instance.PlayerName = playerName;
        // Здесь можно добавить дополнительную логику, например, проверку имени
        //}
        #endregion
    }
}

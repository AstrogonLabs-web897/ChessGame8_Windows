using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using  wpfMessageBox = System.Windows.MessageBox;
namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для GameOptions.xaml
    /// </summary>
    public partial class GameOptions : Page
    {
        private List<Player> _players;
        // Имя игрока
        private string _playerName;
      

        // Выбранный режим игры
        private GameModeEnum _selectedMode;
       

        // Выбранный цвет игрока
        private ChessGame8.UI.PlayerColor_1 _selectedColor;



        public GameOptions()
        {
            InitializeComponent();
            // Присвоение начальных значений
            _playerName = "DefaultPlayer";
            _selectedMode = GameModeEnum.SinglePlayerVsComputer;
            _selectedColor = PlayerColor_1.White;
        }
        // Обработчик сохранения настроек
        private void SaveOptionsButton_Click(object? sender, RoutedEventArgs? e)
        {
            // Получаем имя игрока
            string playerName = PlayerNameInput.Text.Trim();
            // Получаем выбранный режим игры
            _selectedMode = (GameModeEnum)GameModeComboBox.SelectedItem;
            string selectedModeStr = _selectedMode.ToString();
            // Получаем выбранный цвет игрока
            _selectedColor = (PlayerColor_1)ColorComboBox.SelectedItem;
            string selectedColorStr = _selectedColor.ToString();
            // Создаем список игроков
            _players = new List<Player> { new Player(playerName, _selectedColor, 0) };
            // Тут можно записать настройки или перейти на другую страницу
            wpfMessageBox.Show($"Имя игрока: {playerName}\nРежим игры: {selectedModeStr}\nЦвет игрока: {selectedColorStr}");
            // Переходим на следующую страницу
            NavigateToStartMenu();
        }
        // Переход на страницу начала игры
        private void NavigateToStartMenu()
        {
            NavigationService.Navigate(new StartMenu(_players, _selectedMode));
        }
        

        //private void Multiplayer_Checked(object sender, RoutedEventArgs e)
        //{
            // Логика выбора режима "Multiplayer"
            //if (sender is System.Windows.Controls.RadioButton radioButton && radioButton.IsChecked == true)
            //{
                // Сохраняем выбор режима
                //GameMode.SelectedMode = GameModeEnum.Miltiplayer2VsComputer;

                // Дальнейшие действия после выбора (например, переход на другую страницу)
                // Например, можно перенести пользователя на страницу подбора оппонента
                //NavigationService.Navigate(new WaitingRoom.WaitingRoomPage());
                //wpfMessageBox.Show("Выбран режим 'Multiplayer 2 VS Computer'");
            //}
        //}
        private void MoreSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу настроек
            NavigationService.Navigate(new SettingsPage(Settings.Instance));
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся к предыдущему окну
            NavigationService.GoBack();
        }

        private void PlayerNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обрабатываем изменение текста в реальном времени
            UpdatePlayerName(PlayerNameInput.Text);
        }

        private void PlayerNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            // Обрабатываем фокусировку вне текстового поля
            UpdatePlayerName(PlayerNameInput.Text);
        }

        private void PlayerNameInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Обрабатываем ввод имени игрока
                UpdatePlayerName(PlayerNameInput.Text);

                // Переводим фокус на кнопку "Save Game Options "
                SaveGameOptions.Focus();

                // Блокируем дальнейшее распространение события
                e.Handled = true;
            }
        }
        private void UpdatePlayerName(string playerName)
        {
            // Здесь можно обновить имя игрока в модели или где угодно
            // Например, сохраним имя в настройках
            Settings.Instance.PlayerName = playerName;
            // Здесь можно добавить дополнительную логику, например, проверку имени
        }

        private void GameModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный элемент
            ComboBox? comboBox = sender as ComboBox;

            if (comboBox != null)
            {
                if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    if (selectedItem != null)
                    {
                        // Если выбрана какая-то позиция, выполняем общую логику
                        if (e.AddedItems.Count > 0)
                        {
                            ProcessModeSelection(((ComboBoxItem)e.AddedItems[0]).Content.ToString());
                        }
                    }
                     else if (selectedItem == null) {return; }
                    }
                }
                else if (comboBox == null) { return; }
            }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            // Получаем выбранный элемент
            ComboBox? comboBox = sender as ComboBox;

            if (comboBox != null)
            {
                if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    if (selectedItem != null)
                    {
                        // Если выбрана какая-то позиция, выполняем общую логику
                        if (e.AddedItems.Count > 0)
                        {
                            ProcessColorSelection(((ComboBoxItem)e.AddedItems[0]).Content.ToString());
                        }
                    }
                    else if (selectedItem == null) { return; }
                }
            }
            else if (comboBox == null) { return; }
        }
        // Обработчик выбора цвета
        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Передаем управление общему методу
            ProcessColorSelection(sender);
           
        }
        // Обработчик выбора режима игры
        private void ModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Передаем управление общему методу
            ProcessModeSelection(sender);
        }
        // Общий метод для обработки выбора цвета
        private void ProcessColorSelection(object senderOrContent)
        {
            string? content;
            if (senderOrContent is RadioButton rb)
            {
                content = rb.Content.ToString();
            }
            else
            {
                content = senderOrContent.ToString();
            }

            switch (content)
            {
                case "Белые":
                    _selectedColor = ChessGame8.UI.PlayerColor_1.White;
                    wpfMessageBox.Show("Вы выбрали игру за белых.");
                    break;
                case "Черные":
                    _selectedColor = ChessGame8.UI.PlayerColor_1.Black;
                    wpfMessageBox.Show("Вы выбрали игру за черных.");
                    break;
                default:
                    // Действие по умолчанию
                    wpfMessageBox.Show("Не выбран ни один цвет.");
                    break;
            }
        }
        // Общий метод для обработки выбора режима
        private void ProcessModeSelection(object senderOrContent)
        {
            string? content;
            if (senderOrContent is RadioButton rb && rb.IsChecked == true)
            {
                content = rb.Content.ToString();
            }
            else
            {
                content = senderOrContent.ToString();
            }

            switch (content)
            {
                case "Одиночная игра против компьютера":
                    _selectedMode = GameModeEnum.SinglePlayerVsComputer;
                        // Сохраняем выбор режима
                        GameMode.SelectedMode = (GameModeEnum)_selectedMode;

                    // Дальнейшие действия после выбора (например, запуск игры против AI)
                    SaveOptionsButton_Click(null, null);
                    wpfMessageBox.Show("Выбран режим 'Single Player vs Computer'");
                    break;
                case "Многопользовательская игра 2 на 2":
                    _selectedMode = GameModeEnum.Multiplayer2v2;
                    GameMode.SelectedMode = ( GameModeEnum)_selectedMode;
                    // Дальнейшие действия после выбора (например, переход на другую страницу)
                    SaveOptionsButton_Click(null, null);
                    wpfMessageBox.Show("Выбран режим 'Multiplayer2v2'");
                    break;
                case "Многопользовательская игра 2 против компьютера":
                    _selectedMode = GameModeEnum.Miltiplayer2VsComputer;
                    GameMode.SelectedMode = (GameModeEnum)_selectedMode;
                    // Дальнейшие действия после выбора (например, переход на другую страницу)
                    SaveOptionsButton_Click(null, null);
                    wpfMessageBox.Show("Выбран режим 'Multiplayer 2 vs Computer '");
                    break;
                case "Многопользовательская игра 4 на 4":
                    _selectedMode = GameModeEnum.Multiplayer4v4;
                    GameMode.SelectedMode =  (GameModeEnum)_selectedMode;
                    // Дальнейшие действия после выбора (например, переход на другую страницу)
                   SaveOptionsButton_Click(null, null);
                    wpfMessageBox.Show("Выбран режим 'Multiplayer4v4'");
                    break;
                default:
                    wpfMessageBox.Show("Не выбран ни один из режимов игры. ");
                    break;
            }
        }
        // Обработчик кнопки "Отмена"
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем выбор в ComboBox
            GameModeComboBox.SelectedIndex = -1; // Очистка выбора режима
            ColorComboBox.SelectedIndex = -1; // Очистка выбора цвета

            // Очищаем имя игрока
            PlayerNameInput.Clear();

            // Любые другие необходимые действия для сброса
        }
        // Обработчик сохранения настроек
        private void SaveOptionsButton_Click_Handler(object sender, RoutedEventArgs e)
        {
            SaveOptionsButton_Click(null, null); // Явно вызываем основной метод сохранения
        }
    }
}

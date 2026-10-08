using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using wpfMessageBox = System.Windows.MessageBox;

namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public Settings Settings { get; set; }
        public SettingsPage(Settings settings)
        {
            InitializeComponent();
            Settings = settings;
            DataContext = Settings.Instance;
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Вернуться на предыдущую страницу
        }
        private void OnThemeSelected(object sender, RoutedEventArgs e)
        {
            // Получаем текущий ресурсный словарь
            ResourceDictionary currentResources = System.Windows.Application.Current.Resources.MergedDictionaries.First();
            // Определим выбранную тему
            if (sender is System.Windows.Controls.RadioButton selectedButton && selectedButton.Content.ToString() == "Светлая тема")
            {
                Properties.Settings.Default.Theme1 = "White";
            }
            else
            {
                Properties.Settings.Default.Theme2 = "Dark";
            }
            Properties.Settings.Default.Save();
            // Производим переключение темы
            ApplyTheme();
        }
        // Метод для применения выбранной темы
        private void ApplyTheme()
        {
            ResourceDictionary currentResources = System.Windows.Application.Current.Resources.MergedDictionaries.First();
            if (Properties.Settings.Default.Theme1 == "White")
            {
                currentResources.Source = new Uri("/res/Thems/WhiteTheme.xaml", UriKind.Relative);
            }
            else
            {
                currentResources.Source = new Uri("/res/Thems/DarkTheme.xaml", UriKind.Relative);
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем настройки

            // Звук и громкость
            Settings.Instance.SoundEnabled = soundCheckBox.IsChecked == true;
            Settings.Instance.Volume = (int)volumeSlider.Value;

            // Сложность
            Settings.Instance.DifficultyLevel = difficultyComboBox.SelectedItem?.ToString();

            // Показывать подсказки
            Settings.Instance.ShowHints = showHintsCheckBox.IsChecked == true;

            // Тема
            Settings.Instance.Theme = lightThemeRadio.IsChecked == true ? "White" : "Dark";

            // Сохраняем настройки в постоянное хранилище
            Properties.Settings.Default.Save();

            wpfMessageBox.Show("Настройки сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

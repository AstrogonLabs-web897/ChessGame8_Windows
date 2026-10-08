using System.ComponentModel;
using System.Runtime.CompilerServices;
using static ChessGame8.UI.GameScreenViewModel;

namespace ChessGame8.UI
{
    public class Settings: INotifyPropertyChanged
    {
        private  string _theme = "White";
        private bool _soundEnabled = true;
        private bool _showHints = true;
        private string _difficultyLevel = "легкий";
        private int _volume = 40;
        private string _playerName = "Alex123456";
        public GameModeEnum _gameMode = GameModeEnum.SinglePlayerVsComputer;
        public GameModeEnum SelectedGameMode = GameModeEnum.SinglePlayerVsComputer;
        public static List<Player> PresetPlayers { get; } = new List<Player> // Пресеты игроков
        {
        new Player("Alex123456", PlayerColor_1.White, 100),
        new Player("JohnDoe",   PlayerColor_1.White,500),
        new Player("JaneSmith",PlayerColor_1.White,1000)
        };
        public GameModeEnum GameMode2
        {
            get => _gameMode;
            set
            {
                _gameMode = value;
                OnPropertyChanged(nameof(GameMode2));
            }
        }
        
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }
        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                _soundEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool ShowHints
        {
            get => _showHints;
            //get => _showHintsOff;
            set
            {
                _showHints = value;
                OnPropertyChanged();
            }
        }
        public string DifficultyLevel
        {
            get => _difficultyLevel;
            set
            {
                _difficultyLevel = value;
                OnPropertyChanged();
            }
        }

        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnPropertyChanged();
            }
        }
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }
        // Синглтон для глобального доступа к настройкам
        private static Settings _instance = new Settings();
        public static Settings Instance => _instance ??= new Settings();
        // Конструктор с начальными значениями
        public Settings()
        {
            Theme = "White";
            SoundEnabled = true;
            ShowHints = true;
            DifficultyLevel = "легкий";
            Volume = 40;
            PlayerName = "Alex123456";
            GameMode2 = GameModeEnum.SinglePlayerVsComputer;
        }
        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

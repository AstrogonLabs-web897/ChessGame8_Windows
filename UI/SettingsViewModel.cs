using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame8.UI
{
    public class SettingsViewModel
    {
        public string Theme1 { get; set; } = "White";
        public string Theme2 { get; set; } = "Dark";

        public bool SoundEnabled { get; set; } = true;
        public bool SoundDisabled { get; set; } = false;
        public bool ShowHintsOn { get; set; } = true;
        public bool ShowHintsOff { get; set; } = false;
        public string DifficultyLevel { get; set; } = "легкий";
        public string DifficultyLevel2 { get; set; } = "средний";
        public string DifficultyLevel3 { get; set; } = "сложный";
        public int Volume { get; set; } = 40;
    }
}

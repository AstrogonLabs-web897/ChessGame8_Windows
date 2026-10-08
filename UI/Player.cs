using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame8.UI
{
    public class Player
    {
        public string Name { get; set; }
        public PlayerColor_1 Color { get; set; }
        public DateTime JoinDate { get; set; }
        public int Rating { get; set; }


        public Player(string name, PlayerColor_1 color,int rating)
        {
#pragma warning disable
            Name = name;
            Color = color;
            Rating = rating;
            JoinDate = DateTime.Now;
        }
        // Метод для увеличения рейтинга
        public void IncreaseRating(int points)
        {
            Rating += points;
        }

        // Метод для уменьшения рейтинга
        public void DecreaseRating(int points)
        {
            Rating -= points;
        }

        // Метод для получения информации о игроке
        public string GetInfo()
        {
            return $"{Name} ({Color}), рейтинг: {Rating}";
        }
       
        
    } 
    public enum PlayerColor_1
    {
            White,
            Black,
            Human,
            Computer,
            Trasparent
            
    }
    // Перечисление режимов игры
   public enum GameMode_2
   {
     SinglePlayerVsComputer,
      Multiplayer2v2,
      Multiplayer4v4
   }
}



using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColorCodeBreaker.Models
{
    public class Game
    {
        public enum GameStatus
        {
            None = 0,
            Won = 1,
            Lost = 2
        }

        public class GameResult
        {
            public GameStatus Status { get; set; }
            public string Result { get; set; }
        }

        public static string RED = "RED";
        public static string BLUE = "BLUE";
        public static string GREEN = "GREEN";
        public static string YELLOW = "YELLOW";

        private List<string> ColorOptions = new List<string>() { RED, BLUE, GREEN, YELLOW };
        private Random random = new Random();

        public Game(string playerId)
        {
            PlayerId = playerId;
            for (var i = 0; i < 4; i++)
            {
                var index = random.Next(4);
                Colors.Add(ColorOptions[index]);
                Choices.Add(string.Empty);
            }

            System.Diagnostics.Trace.WriteLine(Colors);
        }

        public string PlayerId { get; set; }
        public List<string> Colors { get; set; } = new List<string>();
        public List<string> Choices { get; set; } = new List<string>();
        public int Runs { get; set; }
        public int SelectedColorIndex { get; set; }

        public GameResult EvaluateSelection()
        {
            int correctPosition = 0;
            int wrongPosition = 0;
            var colors = new List<string>(Colors);
            var choices = new List<string>(Choices);

            for (var i = 0; i < 4; i++)
            {
                if (choices[i] == colors[i])
                {
                    correctPosition++;
                    choices[i] = string.Empty;
                    colors[i] = string.Empty;
                }
            }

            for (var j = 0; j < 4; j++)
            {
                if (choices[j] != string.Empty && colors.IndexOf(choices[j]) != -1)
                {
                    wrongPosition++;
                    colors[colors.IndexOf(choices[j])] = string.Empty;
                }
            }

            this.Runs++;

            if (this.Runs > 9)
            {
                return new GameResult()
                {
                    Status = GameStatus.Lost,
                    Result = $"Try {this.Runs}: GAME OVER the correct combination was {Colors[0]}, {Colors[1]}, {Colors[2]}, {Colors[3]}"
                };
            }
            else if (correctPosition == 4)
            {
                return new GameResult()
                {
                    Status = GameStatus.Won,
                    Result = $"Try {this.Runs}: YOU WON"
                };
            }
            else
            {
                return new GameResult()
                {
                    Status = GameStatus.None,
                    Result = $"Try {this.Runs}: Your choices were {Choices[0]}, {Choices[1]}, {Choices[2]}, {Choices[3]}. You have {correctPosition} color in correct position and {wrongPosition} colors in wrong position"
                };

            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Сheckers
{
    internal static class Program
    {
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public static void Main(string[] args)
        {
            // -- Очень низкий уровень сложности --
            // StartGame(false, 1);
            // StartGame(true, 1);
            
            // -- Низкий уровень сложности --
            // StartGame(false, 2);
            // StartGame(true, 2);
                
            // -- Средний уровень сложности --
            // StartGame(false, 4);
            // StartGame(true, 4);
            
            // -- Высокий уровень сложности --
            // StartGame(false, 6);
            // StartGame(true, 6);
            
            // -- Невероятно высокий уровень сложности --
            // StartGame(false, 8);
            // StartGame(true, 8);
            
            // -- Невозможный уровень сложности --
            // StartGame(false, 10);
            StartGame(true, 10);
            
            // -- Тестовая игра --
            // TestGame();
        }

        private static void StartGame(bool userIsWhite, int searchDepth = 10)
        {
            var game = new Game();
            game.PrintDesk();
            
            while (game.GetWinner() == 0)
            {
                // Совершение хода
                Console.WriteLine();
                Console.WriteLine("Ходят " + (game.PlayerIsWhite ? "белые" : "чёрные") + ".");

                var startTime = DateTime.Now;
                
                List<int[]> moving;
                if (game.PlayerIsWhite == userIsWhite)
                {
                    moving = GetUserMoving(game);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Компьютер думает...");
                    moving = game.GetBestMoving(searchDepth);
                    Thread.Sleep(400);
                }
                
                game.PrintDesk(moving); // Вывод доски до хода
                game.Move(moving);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Thread.Sleep(400); // Задержка между выводами доски
                Console.WriteLine($"Совершён ход {ConvNumbersToString(moving)}. " +
                                  $"Затраченное время: {(DateTime.Now - startTime).TotalSeconds} с.");
                game.PrintDesk(moving); // Вывод доски после хода
            }
            
            
            // Определение победителя
            Console.WriteLine("Победели " + (game.GetWinner() == 1 ? "белые" : "чёрные") + ".");
        }

        private static List<int[]> GetUserMoving(Game game)
        {
            var allMoveVariants = game.GetAllMoves();
            while (true)
            {
                Console.WriteLine("Введите, как вы хотите сходить");
                var userMoving = DoMovingInput();
                
                if (allMoveVariants.Any(movingVariant => 
                    // Совпадают длины ходов
                    userMoving.Count == movingVariant.Count &&
                    // Совпадают ходы
                    !userMoving.Where((t, i) => !movingVariant[i].SequenceEqual(t)).Any()))
                {
                    return userMoving;
                }

                Console.WriteLine("Ошибка: неправильный ход!");
                Console.WriteLine("Вот все возможные варианты хода:");
                foreach (var variant in allMoveVariants)
                    Console.WriteLine(ConvNumbersToString(variant));
            }
        }

        private const string Letters = "abcdefgh";
        private const string Numbers = "12345678";
        private static string ConvNumbersToString(IEnumerable<int[]> moving)
        {
            var result = new StringBuilder();
            foreach (int[] coords in moving)
                result.Append($"{Letters[coords[0]]}{Numbers[coords[1]]} ");
            return result.ToString().Trim();
        }

        private static List<int[]> ConvStringToNumbers(string moving)
        {
            moving = moving.Replace(" ", "");
            
            var result = new List<int[]>();
            
            // Преобразование хода
            for (var i = 0; i < moving.Length - 1; i += 2)
                result.Add(new[] {Letters.IndexOf(moving[i]), Numbers.IndexOf(moving[i + 1])});
            
            return result;
        }
        
        public static void TestGame()
        {
            Console.WriteLine();
            
            var game = new Game();
            var random = new Random();

            // var dateTime = DateTime.Now;
            // Console.WriteLine("START!!!!!!!!!!");
            // for (var i = 0; i < 5; i++) game.GetBestMoving(10);
            // Console.WriteLine($"END!!!!!!!!!! {(DateTime.Now - dateTime).TotalSeconds}");
            //
            // return;
            game.PrintDesk();
            
            while (game.GetWinner() == 0)
            {
                // Совершение хода
                Console.WriteLine("Ходят " + (game.PlayerIsWhite ? "белые" : "чёрные") + ".");

                // var dateTime = DateTime.Now;
                // List<int[]> bestMoving = null;
                // Console.WriteLine("START!!!!!!!!!!");
                // for (var i = 0; i < 5; i++)
                // {
                //     bestMoving = game.GetBestMoving(10);
                // }
                // Console.WriteLine($"END!!!!!!!!!! {(DateTime.Now - dateTime).TotalSeconds}");
                
                var bestMoving = game.GetBestMoving(8);
                
                // List<int[]> bestMoving;
                // if (game.PlayerIsWhite)
                // {
                //     var allMoves = game.GetAllMoves();
                //     bestMoving = allMoves[random.Next(0, allMoves.Count)];
                //     // bestMoving = game.GetBestMoving(7);
                // }
                // else
                // {
                //     bestMoving = game.GetBestMoving(9);
                // }

                Console.WriteLine(ConvNumbersToString(bestMoving));
                game.Move(bestMoving);
                
                game.PrintDesk(bestMoving);
                Console.WriteLine();
            }

            // Определение победителя
            string winner = game.GetWinner() == 1 ? "белые" : "чёрные";
            Console.WriteLine($"Победели {winner}.");
        }

        private static List<int[]> DoMovingInput()
        {
            while (true)
            {
                Console.Write(">>> ");
                string userInput = Console.ReadLine();
                try
                {
                    return ConvStringToNumbers(userInput);
                }
                catch (Exception)
                {
                    Console.WriteLine("Некорректный ввод, попробуйте ещё раз");
                }
            }
        }
    }
}
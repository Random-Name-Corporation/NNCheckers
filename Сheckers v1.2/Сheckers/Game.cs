using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Сheckers
{
    internal class Game
    {
        private const int Infinity = 1000000000;

        private static readonly string[] InitialDesk =
        {
            "-b-b-b-b",
            "b-b-b-b-",
            "-b-b-b-b",
            ".-.-.-.-",
            "-.-.-.-.",
            "w-w-w-w-",
            "-w-w-w-w",
            "w-w-w-w-"
        };
        
        // private static readonly string[] InitialDesk =
        // {
        //     "-b-.-b-.",
        //     ".-.-b-b-",
        //     "-.-b-b-b",
        //     ".-.-.-b-",
        //     "-.-.-.-w",
        //     "b-.-w-w-",
        //     "-w-.-.-.",
        //     "w-w-w-w-"
        // };
        
        private int[,] Board { get; set; }
        private Checker[] Figures { get; set; }
        private List<Checker> WhiteFigures { get; set; }
        private List<Checker> BlackFigures { get; set; }
        public bool PlayerIsWhite { get; private set; }

        public Game(bool playerIsWhite = true)
        {
            Board = new int[8, 8];
            PlayerIsWhite = playerIsWhite;
            
            var lastFigureNumber = 0; // Номер последней добавленной в список фигуры

            // Заполнение массивов с фигурами
            var whiteFigures = new List<Checker>();
            var blackFigures = new List<Checker>();
            var figures = new List<Checker>();

            for (var y = 0; y < InitialDesk.Length; y++)
            for (var x = 0; x < InitialDesk[y].Length; x++)
            {
                var cell = InitialDesk[y][x].ToString();
                bool isQueen = cell == cell.ToUpper();
                cell = cell.ToLower();
                switch (cell)
                {
                    // Чёрные клетки
                    case "b":
                    case "w":
                        var figure = new Checker(x, y, cell == "w", isQueen);
                        figures.Add(figure);
                        (figure.IsWhite ? whiteFigures : blackFigures).Add(figure);
                        Board[x, y] = lastFigureNumber;
                        lastFigureNumber++;
                        break;
                    // Белые клетки
                    case "-":
                        Board[x, y] = -2;
                        break;
                    // Пустые чёрные клетки
                    case ".":
                        Board[x, y] = -1;
                        break;
                    default:
                        throw new Exception($"Error: unknown char {cell} on {x}, {y}");
                }
            }

            WhiteFigures = whiteFigures;
            BlackFigures = blackFigures;
            Figures = figures.ToArray();
        }

        public Game(bool playerIsWhite, int[,] board, Checker[] figures,
            List<Checker> whiteFigures, List<Checker> blackFigures)
        {
            PlayerIsWhite = playerIsWhite;
            Board = board;
            Figures = figures;
            WhiteFigures = whiteFigures;
            BlackFigures = blackFigures;
        }
        
        private void ChangePlayer()
        {
            PlayerIsWhite = !PlayerIsWhite;
        }

        public void PrintDesk(List<int[]> highlight = null)
        {
            Console.WriteLine("  a b c d e f g h");
            
            for (var y = 0; y < Board.GetLength(0); y++)
            {
                Console.Write(y + 1);
                for (var x = 0; x < Board.GetLength(1); x++)
                {
                    Console.Write(" ");

                    int i = Board[x, y];
                    string cellText = i switch { -2 => "-", -1 => "#", _ => Figures[i].ToString() };
                    
                    // Подсветка хода
                    bool highlightThisCell = highlight != null && highlight.Any(a => a[0] == x && a[1] == y);

                    // Цвет фигуры
                    if (i >= 0)
                        Console.ForegroundColor = Figures[i].IsWhite ? ConsoleColor.Red : ConsoleColor.DarkBlue;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    // У дамок другой фон
                    if (i >= 0 && Figures[i].IsQueen)
                    {
                        Console.BackgroundColor = highlightThisCell ? ConsoleColor.Cyan : ConsoleColor.White;
                    }
                    else if (highlightThisCell) Console.BackgroundColor = ConsoleColor.DarkCyan;
                    
                    // Вывод клетки в консоль
                    Console.Write(cellText);
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }

        public int GetWinner(List<List<int[]>> allMovesList = null)
        {
            allMovesList ??= GetAllMoves();
            
            /*  0 - Победителя ещё нет
             *  1 - Белые победили
             *  2 - Чёрные победили    */
            
            if (WhiteFigures.Count == 0) return 2;
            if (BlackFigures.Count == 0) return 1;
            if (allMovesList.Count == 0) return PlayerIsWhite ? 2 : 1;
            return 0;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        internal void Move(List<int[]> moving, bool changePlayer = true)
        {
            for (var i = 0; i < moving.Count - 1; i++)
            {
                int x1 = moving[i][0];
                int y1 = moving[i][1];
                int x2 = moving[i + 1][0];
                int y2 = moving[i + 1][1];
            
                int directionX = x2 > x1 ? 1 : -1;
                int directionY = y2 > y1 ? 1 : -1;
                
                // Шашка, перемещение которой происходит во время данного хода
                var figure = Figures[Board[x1, y1]];
                
                // Обновление информации о шашке
                figure.SetPosition(x2, y2);
                if ((figure.IsWhite ? 0 : 7) == y2) figure.IsQueen = true;
                
                // Обновление игровой доски
                Board[x2, y2] = Board[x1, y1];
                Board[x1, y1] = -1;
                x1 += directionX;
                y1 += directionY;

                while (x1 != x2)
                {
                    int j = Board[x1, y1];

                    // Удаление срубленных шашек
                    if (j >= 0)
                    {
                        (Figures[j].IsWhite ? WhiteFigures : BlackFigures).Remove(Figures[j]);
                        Figures[j].IsEnabled = false;
                    }
                    Board[x1, y1] = -1;
                    
                    x1 += directionX;
                    y1 += directionY;
                }
            }
            if (changePlayer) ChangePlayer();
        }

        private Game GetGameCopy()
        {
            var newFigures = new Checker[Figures.Length];
            for (var i = 0; i < Figures.Length; i++)
                newFigures[i] = Figures[i].Copy();
            
            var newWhiteFigures = new List<Checker>();
            var newBlackFigures = new List<Checker>();
            
            foreach (var figure in newFigures)
                if (figure.IsEnabled)
                    (figure.IsWhite ? newWhiteFigures : newBlackFigures).Add(figure);
            
            return new Game(PlayerIsWhite, (int[,]) Board.Clone(), newFigures,
                newWhiteFigures, newBlackFigures);
        }

        private void SupplementCuttingList(List<List<int[]>> cuttingList, 
            bool checkerIsQueen, int x1, int y1)
        {
            if (Board[x1, y1] < 0 || !Figures[Board[x1, y1]].IsEnabled) return;
            if (checkerIsQueen)
            {
                foreach (int directionY in new[] {-1, 1})
                foreach (int directionX in new[] {-1, 1})
                {
                    for (var delta = 2;; delta++)
                    {
                        int x2 = x1 + directionX * delta;
                        int y2 = y1 + directionY * delta;

                        // Клетка за пределами доски
                        if (x2 is < 0 or > 7 || y2 is < 0 or > 7) break;

                        int cuttingI = Board[x2 - directionX, y2 - directionY];

                        if (cuttingI < 0) continue; // Нельзя рубить пустую клетку

                        if (Figures[cuttingI].IsWhite == PlayerIsWhite)
                            break; // Нельзя рубить свою шашку

                        // Клетка занята
                        if (Board[x2, y2] >= 0) break;

                        // Если получается срубить, добавляем ходы
                        while (x2 is >= 0 and <= 7 && y2 is >= 0 and <= 7 && Board[x2, y2] < 0)
                        {
                            cuttingList.Add(new List<int[]> {new[] {x1, y1}, new[] {x2, y2}});
                            x2 += directionX;
                            y2 += directionY;
                        }

                        break;
                    }
                }
            }
            else
            {
                foreach (int directionY in new[] {-1, 1})
                {
                    int y2 = y1 + directionY * 2;
                    // Клетка за пределами доски по y
                    if (y2 is < 0 or > 7) continue;
                    foreach (int directionX in new[] {-1, 1})
                    {
                        int x2 = x1 + directionX * 2;

                        // Клетка за пределами доски по x
                        if (x2 is < 0 or > 7) continue;

                        int cuttingI = Board[x1 + directionX, y1 + directionY];

                        // Нельзя ходить на занятую клетку, рубить пустую клетку или свою шашку
                        if (cuttingI < 0 || Figures[cuttingI].IsWhite == PlayerIsWhite
                                         || Board[x2, y2] >= 0) continue;

                        // Добавление нового хода
                        cuttingList.Add(new List<int[]> {new[] {x1, y1}, new[] {x2, y2}});
                    }
                }
            }
        }
        
        public List<List<int[]>> GetCuttingList(int[] firstCell = null)
        {
            var cuttingList = new List<List<int[]>>();
            var checkers = PlayerIsWhite ? WhiteFigures : BlackFigures;
            
            // Поиск одинарных рубок

            if (firstCell == null)
            {
                foreach (var checker in checkers)
                {
                    int x1 = checker.X;
                    int y1 = checker.Y;
                    SupplementCuttingList(cuttingList, checker.IsQueen, x1, y1);
                }
            }
            else
            {
                int x1 = firstCell[0], y1 = firstCell[1];
                if (Board[x1, y1] >= 0)
                    SupplementCuttingList(cuttingList, Figures[Board[x1, y1]].IsQueen, x1, y1);
            }

            // Добавление ходов с двойной рубкой
            foreach (var firstCuttingVariant in cuttingList.ToList())
            {
                var gameCopy = GetGameCopy();
                gameCopy.Move(firstCuttingVariant, false);
                
                foreach (var secondCuttingVariant in gameCopy.GetCuttingList(firstCuttingVariant[1]))
                {
                    secondCuttingVariant.Insert(0, (int[]) firstCuttingVariant[0].Clone());
                    cuttingList.Add(secondCuttingVariant);
                }
            }
            return cuttingList;
        }
        
        public List<List<int[]>> GetAllMoves(List<List<int[]>> allMovesList = null)
        {
            allMovesList = allMovesList == null ? GetCuttingList() : allMovesList.ToList();
            
            // Рубить нужно обязательно
            if (allMovesList.Count > 0) return allMovesList;

            var checkers = PlayerIsWhite ? WhiteFigures : BlackFigures;
            
            foreach (var checker in checkers)
            {
                int x1 = checker.X, y1 = checker.Y;
                
                if (Board[x1, y1] < 0) continue;
                if (!Figures[Board[x1, y1]].IsEnabled) continue;

                if (checker.IsQueen)
                {
                    foreach (int directionY in new[] {-1, 1})
                    foreach (int directionX in new[] {-1, 1})
                        for (var delta = 1;; delta++)
                        {
                            // Куда ходит шашка
                            int x2 = x1 + directionX * delta;
                            int y2 = y1 + directionY * delta;
                            
                            // Клетка не может находиться за пределами доски или быть занята
                            if (x2 is < 0 or > 7 || y2 is < 0 or > 7 || Board[x2, y2] >= 0) break; 

                            // Добавление нового хода
                            allMovesList.Add(new List<int[]> {new[] {x1, y1}, new[] {x2, y2}});
                        }
                }
                else
                {
                    int y2 = PlayerIsWhite ? y1 - 1 : y1 + 1;
                    foreach (int x2 in new[] {x1 - 1, x1 + 1})
                    {
                        if (x2 is >= 0 and <= 7 && y2 is >= 0 and <= 7 && Board[x2, y2] < 0)
                            allMovesList.Add(new List<int[]> {new[] {x1, y1}, new[] {x2, y2}});
                    }
                }
            }

            return allMovesList;
        }

        private static readonly int[,] CheckersEvaluation = {
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 5, 6, 7, 8, 8, 7, 6, 5},
            { 4, 5, 6, 7, 7, 6, 5, 4},
            { 3, 4, 5, 6, 6, 5, 4, 3},
            { 2, 3, 4, 5, 5, 4, 3, 2},
            { 1, 2, 3, 4, 4, 3, 2, 1},
            { 0, 1, 2, 3, 3, 2, 1, 0},
            {-1, 0, 1, 2, 2, 1, 0,-1},
        };

        private static readonly int[,] QueensEvaluation = {
            {0, 1, 2, 3, 3, 2, 1, 0},
            {1, 2, 3, 4, 4, 3, 2, 1},
            {2, 3, 4, 5, 5, 4, 3, 2},
            {3, 4, 5, 6, 6, 5, 4, 3},
            {3, 4, 5, 6, 6, 5, 4, 3},
            {2, 3, 4, 5, 5, 4, 3, 2},
            {1, 2, 3, 4, 4, 3, 2, 1},
            {0, 1, 2, 3, 3, 2, 1, 0}
        };
        
        private int Evaluate()
        {
            var score = 0;
            foreach (var figure in Figures)
            {
                if (!figure.IsEnabled) continue;
                
                int figureY = figure.IsWhite ? figure.Y : 7 - figure.Y;
                int figureDirectionY = figure.IsWhite ? -1 : 1;
                
                // Оценка позиций по таблицам
                int figureScore = figure.IsQueen ? 
                    3000 + QueensEvaluation[figureY, figure.X] : 
                    1000 + CheckersEvaluation[figureY, figure.X];
                
                // Если фигура защищена, +2 за каждую защиту
                foreach (int x in new[] {figure.X - 1, figure.X + 1})
                {
                    int y = figure.Y - figureDirectionY;
                    if (x is >= 0 and <= 7 && y is >= 0 and <= 7 && Board[x, y] >= 0 &&
                        Figures[Board[x, y]].IsWhite == figure.IsWhite) figureScore += 2;
                }
                score += figure.IsWhite == PlayerIsWhite ? figureScore : -figureScore;
            }
            
            return score;
        }

        private int AlphaBeta(int alpha, int beta, int depth)
        {
            var cuttingList = GetCuttingList();
            var movesList = GetAllMoves(cuttingList);
            
            int winner = GetWinner(movesList);
            if (winner != 0)
            {
                // Конец игры
                bool winnerIsWhite = winner == 1;
                return winnerIsWhite == PlayerIsWhite ? Infinity : -Infinity;
            }
            
            switch (depth)
            {
                // Выход из рекурсии, когда глубина заканчивается
                case <= 0 when cuttingList.Count == 0:
                    return Evaluate();
                // Сортировка перемещений
                case >= 4:
                    movesList = movesList.OrderBy(moving =>
                    {
                        var gameCopy = GetGameCopy();
                        gameCopy.Move(moving);
                        return gameCopy.Evaluate();
                    }).ToList();
                    break;
            }

            foreach (var moving in movesList)
            {
                var gameCopy = GetGameCopy();
                gameCopy.Move(moving);
                int score = -gameCopy.AlphaBeta(-beta, -alpha, depth - 1);
                
                // 
                if (score > alpha) alpha = score;
                if (alpha >= beta) return alpha;
            }
            
            return alpha;
        }

        public List<int[]> GetBestMoving(int depth)
        {
            var bestMoving = new List<int[]> {new[] {0, 0}, new[] {0, 0}};
            var movesList = GetAllMoves();
            
            int maxResult = -Infinity;
            if (movesList.Count == 1) return movesList[0];
            foreach (var moving in movesList)
            {
                var gameCopy = GetGameCopy();
                gameCopy.Move(moving);
                int result = -gameCopy.AlphaBeta(-Infinity, Infinity, depth - 1);

                if (result < maxResult) continue;
                
                maxResult = result;
                bestMoving = moving;

                if (maxResult == Infinity) break;
            }

            return bestMoving;
        }
    }
}
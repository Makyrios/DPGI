namespace TicTacToe
{
    public class GameState
    {
        public Player[,] GameGrid { get; set; }
        public Player CurrentPlayer { get; set; }
        public int TurnsPassed { get; set; }
        public bool IsFinished { get; set; }

        public event Action<int, int> MoveMade;
        public event Action<GameResult> GameFinished;
        public event Action GameRestarted;

        public GameState()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            IsFinished = false;
        }

        public bool CanMakeMove(int row, int column)
        {
            return !IsFinished && GameGrid[row, column] == Player.None;
        }

        public void MakeMove(int row, int column)
        {
            if (!CanMakeMove(row, column))
            {
                return;
            }

            GameGrid[row, column] = CurrentPlayer;
            MoveMade?.Invoke(row, column);

            if (CheckForWin(row, column, CurrentPlayer, out EndType endType, out int index))
            {
                HandleWinGame(endType, index);
                return;
            }

            TurnsPassed++;
            if (TurnsPassed == 9)
            {
                HandleTieGame();
                return;
            }

            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool CheckForWin(int r, int c, Player currentPlayer, out EndType endType, out int index)
        {
            int[,] row = new int[,] { { r, 0 }, { r, 1 }, { r, 2 } };
            int[,] column = new int[,] { { 0, c }, { 1, c }, { 2, c } };
            int[,] majorDiagonal = new int[,] { { 0, 0 }, { 1, 1 }, { 2, 2 } };
            int[,] minorDiagonal = new int[,] { { 0, 2 }, { 1, 1 }, { 2, 0 } };

            if (CheckLine(row, currentPlayer))
            {
                endType = EndType.Row;
                index = r;
                return true;
            }
            else if (CheckLine(column, currentPlayer))
            {
                endType = EndType.Column;
                index = c;
                return true;
            }
            else if (r == c && CheckLine(majorDiagonal, currentPlayer))
            {
                endType = EndType.MajorDiagonal;
                index = 0;
                return true;
            }
            else if ((r == 1 && c == 1 || r + c == 2) && CheckLine(minorDiagonal, currentPlayer))
            {
                endType = EndType.MinorDiagonal;
                index = 2;
                return true;
            }
            endType = EndType.Tie;
            index = -1;
            return false;
        }

        private void HandleWinGame(EndType endType, int index)
        {
            IsFinished = true;
            GameFinished?.Invoke(new GameResult
            {
                Winner = CurrentPlayer,
                EndInfo = new EndInfo
                {
                    EndType = endType,
                    Index = index
                }
            });
        }

        private void HandleTieGame()
        {
            IsFinished = true;
            GameFinished?.Invoke(new GameResult
            {
                Winner = Player.None,
                EndInfo = new EndInfo
                {
                    EndType = EndType.Tie,
                    Index = -1
                }
            });
        }

        private bool CheckLine(int[,] row, Player currentPlayer)
        {
            return GameGrid[row[0, 0], row[0, 1]] == currentPlayer &&
                GameGrid[row[1, 0], row[1, 1]] == currentPlayer &&
                GameGrid[row[2, 0], row[2, 1]] == currentPlayer;
        }
        
        public void Reset()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            IsFinished = false;

            GameRestarted?.Invoke();
        }
    }
}

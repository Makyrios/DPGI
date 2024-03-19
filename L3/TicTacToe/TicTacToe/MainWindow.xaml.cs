using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imagesSource = new Dictionary<Player, ImageSource>
        {
            { Player.X, new BitmapImage(new Uri("Images/X.png", UriKind.Relative)) },
            { Player.O, new BitmapImage(new Uri("Images/O.png", UriKind.Relative)) }
        };

        private readonly GameState gameState = new GameState();
        private Image[,] images = new Image[3, 3];
        
        public MainWindow()
        {
            InitializeComponent();

            gameState.GameRestarted += OnGameRestarted;
            gameState.GameFinished += OnGameFinished;

            StartNewGame();
        }

        private async void OnGameFinished(GameResult obj)
        {
            await Task.Delay(1000);

            if (obj.Winner != Player.None)
            {
                ShowEndLine(obj.EndInfo);
                await Task.Delay(1000);
                HandleEndScreen("Winner: ", imagesSource[obj.Winner]);
            }
            else
            {
                HandleEndScreen("It's a draw!", null);
            }
        }

        private void ShowEndLine(EndInfo endInfo)
        {
            (Point start, Point end) = FindEndLinePoints(endInfo);

            GameOverLine.X1 = start.X;
            GameOverLine.X2 = end.X;
            GameOverLine.Y1 = start.Y;
            GameOverLine.Y2 = end.Y;
            GameOverLine.Visibility = Visibility.Visible;
        }

        (Point, Point) FindEndLinePoints(EndInfo endInfo)
        {
            float squareSize = (float)GameGridCanvas.ActualWidth / 3;
            float squareMargin = squareSize / 2;

            if (endInfo.EndType == EndType.Tie)
            {
                return (new Point(0, 0), new Point(0, 0));
            }
            if (endInfo.EndType == EndType.Row)
            {
                float y = endInfo.Index * squareSize + squareMargin;
                return (new Point(0, y), new Point(GameGrid.Width, y));
            }
            if (endInfo.EndType == EndType.Column)
            {
                float x = endInfo.Index * squareSize + squareMargin;
                return (new Point(x, 0), new Point(x, GameGrid.Height));
            }
            if (endInfo.EndType == EndType.MajorDiagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }
            if (endInfo.EndType == EndType.MinorDiagonal)
            {
                return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
            }
            return (new Point(0, 0), new Point(0, 0));
        }

        private void HandleEndScreen(string text, ImageSource imageSource)
        {
            CurrentTurnPanel.Visibility = Visibility.Hidden;
            GameGridCanvas.Visibility = Visibility.Hidden;
            EndScreen.Visibility = Visibility.Visible;
            EndScreenText.Text = text;
            EndScreenImage.Source = imageSource;
        }

        private void OnGameRestarted()
        {
            CurrentTurnPanel.Visibility = Visibility.Visible;
            GameGridCanvas.Visibility = Visibility.Visible;
            EndScreen.Visibility = Visibility.Hidden;
            StartNewGame();
        }

        public void StartNewGame()
        {
            GameOverLine.Visibility = Visibility.Hidden;
            GameGrid.Children.Clear();
            images = new Image[3, 3];
            PlayerImage.Source = new BitmapImage(new Uri("Images/X.png", UriKind.Relative));
            SetupGameGrid();
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseClickLocation = Mouse.GetPosition(GameGridCanvas);
            int row = (int)(mouseClickLocation.Y / (GameGridCanvas.ActualHeight / 3));
            int column = (int)(mouseClickLocation.X / (GameGridCanvas.ActualWidth / 3));

            if (gameState.CanMakeMove(row, column))
            {
                gameState.MakeMove(row, column);
                ChangeSquareImage(row, column);
            }
        }

        private void ChangeSquareImage(int row, int column)
        {
            Player player = gameState.GameGrid[row, column];
            images[row, column].Source = imagesSource[player];
            PlayerImage.Source = imagesSource[gameState.CurrentPlayer];
        }

        private void SetupGameGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Image image = new Image();
                    GameGrid.Children.Add(image);
                    images[r, c] = image;
                }
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameState.Reset();
        }
    }
}
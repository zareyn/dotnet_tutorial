using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleGame
{
    public partial class MainWindow : Window
    {
        private double playerSpeed = 5;
        private bool gameCompleted = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateStatus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameCompleted) return;

            double newX = System.Windows.Controls.Canvas.GetLeft(Player);
            double newY = System.Windows.Controls.Canvas.GetTop(Player);

            // Движение игрока
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    newY -= playerSpeed;
                    break;
                case Key.S:
                case Key.Down:
                    newY += playerSpeed;
                    break;
                case Key.A:
                case Key.Left:
                    newX -= playerSpeed;
                    break;
                case Key.D:
                case Key.Right:
                    newX += playerSpeed;
                    break;
            }

            // Проверка границ канваса
            if (newX < 0) newX = 0;
            if (newY < 0) newY = 0;
            if (newX > GameCanvas.ActualWidth - Player.Width) 
                newX = GameCanvas.ActualWidth - Player.Width;
            if (newY > GameCanvas.ActualHeight - Player.Height) 
                newY = GameCanvas.ActualHeight - Player.Height;

            // Создаем временный Rect для проверки коллизий
            Rect playerRect = new Rect(newX, newY, Player.Width, Player.Height);

            // Проверка коллизий со стенами
            if (!CheckCollision(playerRect, Wall1) &&
                !CheckCollision(playerRect, Wall2) &&
                !CheckCollision(playerRect, Wall3))
            {
                // Если нет коллизий - обновляем позицию
                System.Windows.Controls.Canvas.SetLeft(Player, newX);
                System.Windows.Controls.Canvas.SetTop(Player, newY);
            }

            // Проверка достижения выхода
            if (CheckCollision(playerRect, Exit))
            {
                gameCompleted = true;
                StatusText.Text = "ПОБЕДА! Вы достигли выхода!";
                Player.Fill = Brushes.Gold;
            }

            UpdateStatus();
        }

        private bool CheckCollision(Rect rect1, Rectangle rect2)
        {
            double left2 = System.Windows.Controls.Canvas.GetLeft(rect2);
            double top2 = System.Windows.Controls.Canvas.GetTop(rect2);
            
            Rect rect2Bounds = new Rect(left2, top2, rect2.Width, rect2.Height);
            
            return rect1.IntersectsWith(rect2Bounds);
        }

        private void UpdateStatus()
        {
            if (!gameCompleted)
            {
                double playerX = System.Windows.Controls.Canvas.GetLeft(Player);
                double playerY = System.Windows.Controls.Canvas.GetTop(Player);
                
                StatusText.Text = $"Позиция: X={playerX:F0}, Y={playerY:F0}\n" +
                                 $"Управление: WASD или стрелки\n" +
                                 $"Цель: дойти до зеленого квадрата";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls; // Добавляем это!

namespace SmoothGame
{
    public partial class MainWindow : Window
    {
        private double playerSpeed = 0.3;
        private double friction = 0.85;
        private double maxSpeed = 8.0;
        
        private double velocityX = 0;
        private double velocityY = 0;
        
        private bool isMovingUp = false;
        private bool isMovingDown = false;
        private bool isMovingLeft = false;
        private bool isMovingRight = false;
        
        private bool gameCompleted = false;
        private int coinsCollected = 0;
        private int totalCoins = 3;
        
        private DispatcherTimer? gameTimer; // Делаем nullable
        private List<Ellipse>? coins; // Делаем nullable

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            StartGameLoop();
        }

        private void InitializeGame()
        {
            coins = new List<Ellipse> { Coin1, Coin2, Coin3 };
            UpdateStatus();
        }

        private void StartGameLoop()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object? sender, EventArgs e) // Добавляем nullable
        {
            if (gameCompleted) return;

            HandleInput();
            ApplyPhysics();
            CheckCollisions();
            UpdateStatus();
        }

        private void HandleInput()
        {
            if (isMovingUp) velocityY -= playerSpeed;
            if (isMovingDown) velocityY += playerSpeed;
            if (isMovingLeft) velocityX -= playerSpeed;
            if (isMovingRight) velocityX += playerSpeed;
        }

        private void ApplyPhysics()
        {
            velocityX = Math.Max(Math.Min(velocityX, maxSpeed), -maxSpeed);
            velocityY = Math.Max(Math.Min(velocityY, maxSpeed), -maxSpeed);
            
            velocityX *= friction;
            velocityY *= friction;
            
            if (Math.Abs(velocityX) < 0.1) velocityX = 0;
            if (Math.Abs(velocityY) < 0.1) velocityY = 0;
            
            double currentX = Canvas.GetLeft(Player);
            double currentY = Canvas.GetTop(Player);
            
            double newX = currentX + velocityX;
            double newY = currentY + velocityY;
            
            newX = Math.Max(0, Math.Min(newX, GameCanvas.ActualWidth - Player.Width));
            newY = Math.Max(0, Math.Min(newY, GameCanvas.ActualHeight - Player.Height));
            
            Canvas.SetLeft(Player, newX);
            Canvas.SetTop(Player, newY);
        }

        private void CheckCollisions()
        {
            Rect playerRect = new Rect(
                Canvas.GetLeft(Player), 
                Canvas.GetTop(Player), 
                Player.Width, 
                Player.Height
            );

            if (CheckCollision(playerRect, Wall1) ||
                CheckCollision(playerRect, Wall2) ||
                CheckCollision(playerRect, Wall3) ||
                CheckCollision(playerRect, Wall4))
            {
                velocityX = -velocityX * 0.5;
                velocityY = -velocityY * 0.5;
                
                double currentX = Canvas.GetLeft(Player);
                double currentY = Canvas.GetTop(Player);
                Canvas.SetLeft(Player, currentX - velocityX);
                Canvas.SetTop(Player, currentY - velocityY);
            }

            // Проверяем что coins не null
            if (coins != null)
            {
                for (int i = coins.Count - 1; i >= 0; i--)
                {
                    if (CheckCollision(playerRect, coins[i]))
                    {
                        coinsCollected++;
                        GameCanvas.Children.Remove(coins[i]);
                        coins.RemoveAt(i);
                        
                        Player.Fill = Brushes.Orange;
                        DispatcherTimer tempTimer = new DispatcherTimer();
                        tempTimer.Interval = TimeSpan.FromMilliseconds(200);
                        tempTimer.Tick += (s, e) => 
                        { 
                            Player.Fill = Brushes.Red; 
                            tempTimer.Stop(); 
                        };
                        tempTimer.Start();
                    }
                }
            }

            if (CheckCollision(playerRect, Exit))
            {
                gameCompleted = true;
                Player.Fill = Brushes.Gold;
                gameTimer?.Stop();
            }
        }

        private bool CheckCollision(Rect rect1, Shape shape)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            
            Rect shapeRect = new Rect(left, top, shape.Width, shape.Height);
            return rect1.IntersectsWith(shapeRect);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    isMovingUp = true;
                    break;
                case Key.S:
                case Key.Down:
                    isMovingDown = true;
                    break;
                case Key.A:
                case Key.Left:
                    isMovingLeft = true;
                    break;
                case Key.D:
                case Key.Right:
                    isMovingRight = true;
                    break;
                case Key.R:
                    ResetGame();
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    isMovingUp = false;
                    break;
                case Key.S:
                case Key.Down:
                    isMovingDown = false;
                    break;
                case Key.A:
                case Key.Left:
                    isMovingLeft = false;
                    break;
                case Key.D:
                case Key.Right:
                    isMovingRight = false;
                    break;
            }
        }

        private void ResetGame()
        {
            Canvas.SetLeft(Player, 50);
            Canvas.SetTop(Player, 50);
            
            velocityX = 0;
            velocityY = 0;
            
            isMovingUp = isMovingDown = isMovingLeft = isMovingRight = false;
            gameCompleted = false;
            coinsCollected = 0;
            
            // Восстанавливаем монеты
            if (!GameCanvas.Children.Contains(Coin1))
            {
                GameCanvas.Children.Add(Coin1);
                Canvas.SetLeft(Coin1, 200);
                Canvas.SetTop(Coin1, 300);
            }
            if (!GameCanvas.Children.Contains(Coin2))
            {
                GameCanvas.Children.Add(Coin2);
                Canvas.SetLeft(Coin2, 450);
                Canvas.SetTop(Coin2, 250);
            }
            if (!GameCanvas.Children.Contains(Coin3))
            {
                GameCanvas.Children.Add(Coin3);
                Canvas.SetLeft(Coin3, 600);
                Canvas.SetTop(Coin3, 400);
            }
            
            coins = new List<Ellipse> { Coin1, Coin2, Coin3 };
            Player.Fill = Brushes.Red;
            
            if (gameTimer != null && !gameTimer.IsEnabled)
                gameTimer.Start();
                
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            double playerX = Canvas.GetLeft(Player);
            double playerY = Canvas.GetTop(Player);
            
            if (gameCompleted)
            {
                StatusText.Text = $"ПОБЕДА! Собрано монет: {coinsCollected}/{totalCoins}\n" +
                                 $"Нажми R для перезапуска";
            }
            else
            {
                StatusText.Text = $"Позиция: X={playerX:F0}, Y={playerY:F0}\n" +
                                 $"Скорость: X={velocityX:F1}, Y={velocityY:F1}\n" +
                                 $"Монеты: {coinsCollected}/{totalCoins}\n" +
                                 $"Управление: WASD/Стрелки | R - перезапуск";
            }
        }
    }
}
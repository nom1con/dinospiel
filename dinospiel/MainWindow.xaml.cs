using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Windows.Markup;

namespace dinospiel {

    public partial class MainWindow : Window {

        private DispatcherTimer timer = new DispatcherTimer();
        private List<Obstacle> obstacles = new List<Obstacle>();
        Random rng = new Random();

        public MainWindow() {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += playing;
        }

        private void playing(object sender, EventArgs e) {
            if ( rng.Next(1, 200) == 1 ) {
                Obstacle obstacle = new Obstacle(rng.Next(2) == 1);
                obstacles.Add(obstacle);
                GameCanvas.Children.Add(obstacle.Img);
                Canvas.SetLeft(obstacle.Img, obstacle.Pos.X + 1);
                Canvas.SetTop(obstacle.Img, obstacle.Pos.Y + 1);
            }

            Move();


        }

        public void Move() {
            foreach ( var obstacle in obstacles ) {
                if ( GameCanvas.Children.Contains(obstacle.Img) ) {
                    GameCanvas.Children.Remove(obstacle.Img);
                }
            }

            foreach ( Obstacle obstacle in obstacles ) {
                if ( GameCanvas.Children.Contains(obstacle.Img) )
                    continue;
                obstacle.Pos = new Point(obstacle.Pos.X-3, 0);
                GameCanvas.Children.Add(obstacle.Img);
            }

            if ( HitCheck() )
                GameOver();
        }

        public bool HitCheck() {
            double dinoX = Canvas.GetLeft(dino);
            double dinoY = Canvas.GetTop(dino);
            double dinoWidth = dino.Width;
            double dinoHeight = dino.Height;

            Rect dinoRect = new Rect(dinoX, dinoY, dinoWidth, dinoHeight);

            foreach ( Obstacle obstacle in obstacles ) {
                Rect obsRect = new Rect(obstacle.Pos.X, obstacle.Pos.Y, obstacle.Width, obstacle.Height);
                if ( dinoRect.IntersectsWith(obsRect) ) {
                    return true;
                }
            }

            return false;
        }

        private void GameOver() {
            timer.Stop();
            foreach ( var obstacle in obstacles ) {
                if ( GameCanvas.Children.Contains(obstacle.Img) ) {
                    GameCanvas.Children.Remove(obstacle.Img);
                }
            }   

            obstacles.Clear();
            BTNplay.Visibility = Visibility.Visible;
        }

        private void BTNplay_Click(object sender, RoutedEventArgs e) {
            BTNplay.Visibility = Visibility.Hidden;
            timer.Start();
        }
    }

    
}

public class Obstacle {
    public bool Type { get; set; }
    public string Path { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Point Pos { get; set; } = new Point();
    public Image Img { get; set; }


    public Obstacle(bool type) {
        this.Type = type;

        switch ( type ) {
            case true:
            this.Path = "img\\cactus.png";
            this.Width = 100;
            this.Height = 100;
            this.Pos = new Point(20, 0);
            break;
            case false:
            this.Path = "img\\ptero.png";
            this.Width = 70;
            this.Height = 70;
            this.Pos = new Point(20, -100);
            break;
        }

        if ( !File.Exists(Path) ) {
            MessageBox.Show($"Warning: Image file not found at path '{Path}'", "Missing Image", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        Img = new Image {
            Width = this.Width,
            Height = this.Height,
            Source = new BitmapImage(new Uri(Path, UriKind.Relative))
        };
    }
}


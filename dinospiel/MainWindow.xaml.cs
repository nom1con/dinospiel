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
using System.Windows.Shell;
using System.Security.Cryptography;

namespace dinospiel {

    public partial class MainWindow : Window {

        private DispatcherTimer timer = new DispatcherTimer();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private Random rng = new Random();
        private int jumping = 0, score = 0;

        public MainWindow() {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += playing;
        }

        private void playing(object sender, EventArgs e) {
            if (( rng.Next(1, 200) == 1 || obstacles.Count<1 ) && obstacles.Count < 2 ) {
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

            List<Obstacle> RMV_obstacles = new List<Obstacle>();

            foreach ( Obstacle obstacle in obstacles ) {
                if ( GameCanvas.Children.Contains(obstacle.Img) )
                    continue;
                obstacle.Pos = new Point(obstacle.Pos.X-10, obstacle.Pos.Y);

                GameCanvas.Children.Add(obstacle.Img);
                
                if ( obstacle.Pos.X < -800 ) {
                    RMV_obstacles.Add(obstacle);
                    score++;
                    lblScore.Content = score;
                }

                Canvas.SetLeft(obstacle.Img, 800 + obstacle.Pos.X);
                Canvas.SetTop(obstacle.Img, 295 - obstacle.Pos.Y);
            }


            foreach( Obstacle obstacle in RMV_obstacles ) {
                obstacles.Remove(obstacle);
                GameCanvas.Children.Remove(obstacle.Img );
            }




            if ( HitCheck() )
                GameOver();

            if(Canvas.GetTop(dino) != 295 || jumping <0 ) {
                double newTop;
                if ( jumping < -250) {
                    newTop = Canvas.GetTop(dino) - 10;
                    jumping += 10;
                } else {
                    newTop = 295 + jumping;
                    jumping = jumping < 0 ? jumping + 10 : 0;
                }
                Canvas.SetTop(dino, newTop);
            }
        }

        public bool HitCheck() {
            double dinoX = Canvas.GetLeft(dino);
            double dinoY = Canvas.GetTop(dino);
            double dinoWidth = dino.Width;
            double dinoHeight = dino.Height;

            Rect dinoRect = new Rect(dinoX, dinoY, dinoWidth, dinoHeight);

            foreach ( Obstacle obstacle in obstacles ) {
                Rect obsRect = new Rect(Canvas.GetLeft(obstacle.Img), Canvas.GetTop(obstacle.Img), obstacle.Width, obstacle.Height);
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

            score = 0;
            obstacles.Clear();
            BTNplay.Visibility = Visibility.Visible;
            lblTutorial.Visibility = Visibility.Visible;
        }

        private void BTNplay_Click(object sender, RoutedEventArgs e) {
            BTNplay.Visibility = Visibility.Hidden;
            lblTutorial.Visibility = Visibility.Hidden;
            lblScore.Content = "0";
            jumping = 0;
            Canvas.SetTop(dino, 295);
            timer.Start();
        }

        private void jump(object sender, KeyEventArgs e) {
            if ( jumping >= 0 ) {
                jumping -= 500;
            }
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
            this.Pos = new Point(20, (new Random().Next(0, 300)));
            break;
        }

        if ( !File.Exists(Path) ) {
            MessageBox.Show($"Warning: Image file not found at path '{Path}'", "Missing Image", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        Img = new Image {
            Width = this.Width,
            Height = this.Height,
            Stretch = Stretch.Fill,
            Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(Path)))

            //Source = new BitmapImage(new Uri(Path, UriKind.Relative))
        };
    }

}



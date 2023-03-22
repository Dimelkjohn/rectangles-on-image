using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPicasso
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Upload image handler
        private void Upload_Img(object sender, RoutedEventArgs e)
        {
            //create a new filedialog obj
            OpenFileDialog openDialog = new OpenFileDialog();

            //filter selection to images
            openDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            openDialog.FilterIndex = 1;

            //if the dialog gets opened
            if(openDialog.ShowDialog() == true)
            {
                //whatever's selected gets passed to uploaded_img
                Uploaded_img.Source = new BitmapImage(new Uri(openDialog.FileName));
            }

        }

        private void Save_Img(object sender, RoutedEventArgs e)
        {
            //create a new savedialog obj
            SaveFileDialog saveDialog = new SaveFileDialog();

            //set the ext to .jpeg
            saveDialog.DefaultExt = ".png";
            saveDialog.Filter = "Image files|*.bmp;*.jpg;*.png";

            //if the dialog gets opened
            if (saveDialog.ShowDialog() == true)
            {
                /*save edited image by creating a render target
                bitmap the same size of the canvas, rendering it,
                creating a new pngBitmapEncoder, adding the render to it,
                then writing to file*/
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)Uploaded_img.ActualWidth, (int)Uploaded_img.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                renderTargetBitmap.Render(content_grid);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                using(FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }

        }

        //Clear canvas space
        private void Clear_Canvas(object sender, RoutedEventArgs e)
        {
            Uploaded_img.Source = null;
            canvas.Children.Clear();
        }

        //close window
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Drawing a rectangle
        private Point startPoint;
        private Rectangle rect;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(canvas); //set start point on mouse down

            rect = new Rectangle //create a rect
            {
                Stroke = new SolidColorBrush(Color.FromArgb(50, 0, 0, 77)),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(80, 0, 35, 102)),
            };

            //add right click to open menu
            rect.MouseRightButtonUp += Open_Menu;
            //rect.MouseLeftButtonDown += Move_Rectangle;

            Canvas.SetLeft(rect, startPoint.X); //set the left property of the rect to our startpoint x-axis 
            Canvas.SetTop(rect, startPoint.Y); //set the top property of the rect to our startpoint y-axis 
            
            canvas.Children.Add(rect); //adds rect to our xaml as a child to canvas
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || rect == null) //if mouse up, stop
                return;

            var pos = e.GetPosition(canvas); //get the current mouse position

            // get the top left corner of the rectangle b/w the current mouse postion and the start point
            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            // get the width and height by calculating the space between the top left and the bottom right
            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            //set the width and height to those values
            rect.Width = w;
            rect.Height = h;

            //set rect in canvas left and top to x and y
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(rect != null)
            {
                rect = null; //make rect null, stopping the drawing of the rect, also serves as an err handler
            }
        }

        // handles menus
        private void Open_Menu(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender; //cast sender to a rectangle

            //Context menu
            rectangle.ContextMenu = new ContextMenu(); //enabling right click on the rectangle

            var color = new MenuItem(); //color as a menu item ***NOTE: YOU CAN ONLY CHANGE BETWEEN 3 COLORS
            color.Header = "Color";

            var delete = new MenuItem(); //delete as a menu item
            delete.Header = "Delete";

            delete.Click += (s, ea) => //delete rectangle
            {
                Canvas canvas = (Canvas)rectangle.Parent; //tap into the parent canvas
                canvas.Children.Remove(rectangle); //remove that rectangle as it's child
            }; 

            rectangle.ContextMenu.Items.Add(color); //add color as a menu item
            rectangle.ContextMenu.Items.Add(delete); //add delete as a menu item

            //add color red; it's header and a click event
            var color_red = new MenuItem();
            color_red.Header = "Red";
            color_red.Click += (s, ea) =>
            {
                rectangle.Stroke = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                rectangle.Fill = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
            };

            color.Items.Add(color_red);

            //add color yellow; it's header and a click event
            var color_yellow = new MenuItem();
            color_yellow.Header = "Yellow";
            color_yellow.Click += (s, ea) =>
            {
                rectangle.Stroke = new SolidColorBrush(Color.FromArgb(50, 255, 255, 0));
                rectangle.Fill = new SolidColorBrush(Color.FromArgb(80, 255, 255, 0));
            };

            color.Items.Add(color_yellow);

            //add color green; it's header and a click event
            var color_green = new MenuItem();
            color_green.Header = "Green";
            color_green.Click += (s, ea) =>
            {
                rectangle.Stroke = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                rectangle.Fill = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
            };

            color.Items.Add(color_green);
        }


        //Move Rectangle (Not Completed)
        /*
        Point offset;
        private void Move_Rectangle(object sender, EventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender; //cast sender as a rectangle

            rectangle.MouseDown += (s, ea) =>
            {
                offset = ea.GetPosition(canvas); //get the current position of the mouse
                offset.Y += Canvas.GetTop(rectangle);  //set the y position = y position - top of rect
                offset.X += Canvas.GetLeft(rectangle); //set the x position = x position - left of rect
                canvas.CaptureMouse();
            };

            rectangle.MouseMove += (s, ea) =>
            {
                if (rectangle == null) //if there's no object to drag, do nothing.
                    return;

                var position = ea.GetPosition(rectangle);

                Canvas.SetTop(rectangle, position.Y - offset.Y); //sets the top while mouse move
                Canvas.SetLeft(rectangle, position.X - offset.X); //sets the left while mouse move
            };

            rectangle.MouseUp += (s, ea) =>
            {
                rectangle = null;

                canvas.ReleaseMouseCapture();
            };
        } */
    }
}

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class NinePanel : Grid
    {
        Image image1 = new Image() { Stretch = Stretch.Uniform };
        Image image2 = new Image() { Stretch = Stretch.Fill };
        Image image3 = new Image() { Stretch = Stretch.Uniform };
        Image image4 = new Image() { Stretch = Stretch.Fill };
        Image image5 = new Image() { Stretch = Stretch.Fill };
        Image image6 = new Image() { Stretch = Stretch.Fill };
        Image image7 = new Image() { Stretch = Stretch.Uniform };
        Image image8 = new Image() { Stretch = Stretch.Fill };
        Image image9 = new Image() { Stretch = Stretch.Uniform };
        public NinePanel()
        {
            ColumnDefinition[] col = new ColumnDefinition[3];
            for (int i = 0; i < 3; i++)
            {
                col[i] = new ColumnDefinition();
                this.ColumnDefinitions.Add(col[i]);
            }

            RowDefinition[] row = new RowDefinition[3];

            for (int i = 0; i < 3; i++)
            {
                row[i] = new RowDefinition();
                this.RowDefinitions.Add(row[i]);
            }

            this.Children.Add(image1);
            this.Children.Add(image2);
            this.Children.Add(image3);
            this.Children.Add(image4);
            this.Children.Add(image5);
            this.Children.Add(image6);
            this.Children.Add(image7);
            this.Children.Add(image8);
            this.Children.Add(image9);

            image1.SetValue(ColumnProperty, 0);
            image2.SetValue(ColumnProperty, 1);
            image3.SetValue(ColumnProperty, 2);
            image4.SetValue(ColumnProperty, 0);
            image5.SetValue(ColumnProperty, 1);
            image6.SetValue(ColumnProperty, 2);
            image7.SetValue(ColumnProperty, 0);
            image8.SetValue(ColumnProperty, 1);
            image9.SetValue(ColumnProperty, 2);

            image4.SetValue(RowProperty, 1);
            image5.SetValue(RowProperty, 1);
            image6.SetValue(RowProperty, 1);

            image7.SetValue(RowProperty, 2);
            image8.SetValue(RowProperty, 2);
            image9.SetValue(RowProperty, 2);
        }

        public void NinePanl_Loaded(object sender, RoutedEventArgs e)
        {
            //Background.ToString() == "System.Windows.Media.ImageBrush"
            UpDateImage();
        }

        [Browsable(false)]
        public new Brush Background
        {
            get { return base.Background; }
            set { base.Background = value; }
        }

        [Bindable(true), Category("自定义属性")]
        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(NinePanel), new PropertyMetadata(new Thickness(15), new PropertyChangedCallback(NineGridPropertyChangedCallback)));

        private static void NineGridPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as NinePanel;
            obj.UpDateImage();
        }
        [Bindable(true), Category("自定义属性")]
        public Thickness NineGrid
        {
            get { return (Thickness)GetValue(NineGridProperty); }
            set { SetValue(NineGridProperty, value); }
        }

        [Bindable(true), Category("自定义属性")]
        public static readonly DependencyProperty NineImageProperty = DependencyProperty.Register("NineImage", typeof(ImageSource), typeof(NinePanel), new PropertyMetadata(new PropertyChangedCallback(NineImagePropertyChangedCallback)));

        private static void NineImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as NinePanel;
            obj.UpDateImage();


        }
        [Bindable(true), Category("自定义属性")]
        public ImageSource NineImage
        {
            get { return (ImageSource)GetValue(NineImageProperty); }
            set { SetValue(NineImageProperty, value); }
        }


        private void DrawNineImage()
        {
            BitmapImage im = new BitmapImage();
            im.BeginInit();
            im.UriSource = new Uri(NineImage.ToString());
            BitmapSource image = im;
            im.EndInit();

            int left = (int)NineGrid.Left;
            int top = (int)NineGrid.Top;
            int right = (int)NineGrid.Right;
            int bottom = (int)NineGrid.Bottom;

            double drawWidth = ActualWidth - left - right;
            double drawHeight = ActualHeight - top - bottom;

            int centerWidth = (int)image.PixelWidth - left - right;
            int centerHeight = (int)image.PixelHeight - top - bottom;


            image1.Source = new CroppedBitmap(image, new Int32Rect(0, 0, left, top));
            image2.Source = new CroppedBitmap(image, new Int32Rect(left, 0, centerWidth, top));
            image3.Source = new CroppedBitmap(image, new Int32Rect(left + centerWidth, 0, right, top));
            image4.Source = new CroppedBitmap(image, new Int32Rect(0, top, left, centerHeight));
            image5.Source = new CroppedBitmap(image, new Int32Rect(left, top, centerWidth, centerHeight));
            image6.Source = new CroppedBitmap(image, new Int32Rect(left + centerWidth, top, right, centerHeight));
            image7.Source = new CroppedBitmap(image, new Int32Rect(0, top + centerHeight, left, bottom));
            image8.Source = new CroppedBitmap(image, new Int32Rect(left, top + centerHeight, centerWidth, bottom));
            image9.Source = new CroppedBitmap(image, new Int32Rect(left + centerWidth, top + centerHeight, right, bottom));
        }

        private void UpDateImage()
        {
            if (NineImage == null || NineGrid.Top == 0 || NineGrid.Bottom == 0 || NineGrid.Left == 0 || NineGrid.Right == 0)
            {
                return;
            }
            else
            {
                this.ColumnDefinitions[0].Width = new GridLength(NineGrid.Left);
                this.ColumnDefinitions[2].Width = new GridLength(NineGrid.Right);
                //this.ColumnDefinitions[1].Width = GridLength.Auto;
                this.RowDefinitions[0].Height = new GridLength(NineGrid.Top);
                this.RowDefinitions[2].Height = new GridLength(NineGrid.Bottom);
                //this.RowDefinitions[1].Height = GridLength.Auto;
                this.MinWidth = NineGrid.Left + NineGrid.Right;
                this.MinHeight = NineGrid.Top + NineGrid.Bottom;

                DrawNineImage();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 游标面板
    /// </summary>
    public partial class CursorPanel : UserControl
    {
        private string _cursorImage;

        public CursorPanel()
        {
            InitializeComponent();
            LineColor = Color.FromArgb(0xFF, 0x74, 0x74, 0x74);
            HorizontalLineVisibility = Visibility.Collapsed;
            VerticalLineVisibility = Visibility.Collapsed;
            DataContext = this;
            SizeChanged += OnCursorPanelSizeChanged;
        }

        private void OnCursorPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitEvents();
        }

        private void InitEvents()
        {
            var parentElement = Parent as FrameworkElement;
            if (parentElement == null)
                return;
            parentElement.MouseEnter -= OnCursorPanelMouseEnter;
            parentElement.MouseLeave -= OnCursorPanelMouseLeave;
            parentElement.MouseMove -= OnCursorPanelMouseMove;
            parentElement.MouseEnter += OnCursorPanelMouseEnter;
            parentElement.MouseLeave += OnCursorPanelMouseLeave;
            parentElement.MouseMove += OnCursorPanelMouseMove;
        }

        private void OnCursorPanelMouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);
            UpdateArrowPoint(pt);
            var cpArgs = new CursorPointEventArgs();
            cpArgs.OldPoint = pt;
            OnCursorPoint(cpArgs);
            if (cpArgs.NewPoint != null)
            {
                //UpdateArrowPoint(new Point(cpArgs.NewPoint.Value.X, pt.Y));
            }
        }

        private void OnCursorPanelMouseLeave(object sender, MouseEventArgs e)
        {
            xSpectrumArrow.Visibility = xLineX1.Visibility = xLineY1.Visibility = Visibility.Collapsed;
            xLineX2.Visibility = xLineY2.Visibility = Visibility.Collapsed;
            //xLineX1.Visibility = xLineX2.Visibility = Visibility.Visible;
            //xLineY1.Visibility = xLineY2.Visibility = Visibility.Visible;
            Cursor = Cursors.Arrow;
            OnCursorVisibilityChanged(false, e.GetPosition(this));
        }

        private void OnCursorPanelMouseEnter(object sender, MouseEventArgs e)
        {
            xSpectrumArrow.Visibility = Visibility.Visible;
            //xLineX1.Visibility = xLineX2.Visibility = Visibility.Visible;
            //xLineY1.Visibility = xLineY2.Visibility = Visibility.Visible;
            Cursor = Cursors.None;
            OnCursorVisibilityChanged(true, e.GetPosition(this));
            //this.HorizontalAlignment
            //this.VerticalAlignment
        }

        public string CursorImage
        {
            get { return _cursorImage; }
            set
            {
                _cursorImage = value;
                xSpectrumArrow.Source = new BitmapImage(new Uri(_cursorImage, UriKind.RelativeOrAbsolute));
            }
        }

        public Color LineColor { get; set; }

        public Visibility HorizontalLineVisibility { get; set; }

        public Visibility VerticalLineVisibility { get; set; }

        private void UpdateArrowPoint(Point pt)
        {
            xLineX1.X1 = pt.X;
            xLineX1.X2 = pt.X;
            xLineX1.Y1 = 0;
            xLineX1.Y2 = pt.Y - 5;

            xLineX2.X1 = pt.X;
            xLineX2.X2 = pt.X;
            xLineX2.Y1 = pt.Y + 5;
            xLineX2.Y2 = ActualHeight;

            xLineY1.X1 = 0;
            xLineY1.X2 = pt.X - 5;
            xLineY1.Y1 = pt.Y;
            xLineY1.Y2 = pt.Y;

            xLineY2.X1 = pt.X + 5;
            xLineY2.X2 = ActualWidth;
            xLineY2.Y1 = pt.Y;
            xLineY2.Y2 = pt.Y;

            var ct = xSpectrumArrow.RenderTransform as ScaleTransform;
            ct.ScaleX = pt.X - 5;
            ct.ScaleY = pt.Y - 5;
        }

        public event Action<CursorPointEventArgs> CursorPoint;
        public event Action<bool, Point> CursorVisibilityChanged;

        public void OnCursorVisibilityChanged(bool pVisibility, Point pt)
        {
            Action<bool, Point> handler = CursorVisibilityChanged;
            if (handler != null) handler(pVisibility, pt);
        }

        public void OnCursorPoint(CursorPointEventArgs pPt)
        {
            Action<CursorPointEventArgs> handler = CursorPoint;
            if (handler != null) handler(pPt);
        }
    }

    public class CursorPointEventArgs
    {
        public Point OldPoint { get; set; }
        public Point? NewPoint { get; set; }
    }
}

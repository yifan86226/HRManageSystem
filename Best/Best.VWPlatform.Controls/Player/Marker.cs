using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Player
{
    //public delegate void MarkerEventHandler(Object sender, TimelineMarker m);
    [TemplatePart(Name = ArrowElement, Type = typeof(Ellipse))]
    [TemplatePart(Name = MarkerGridElement, Type = typeof(Grid))]
    [TemplatePart(Name = AnimateElement, Type = typeof(Storyboard))]
    [TemplatePart(Name = tbMarkerElement, Type = typeof(TextBlock))]
    public class Marker : Control
    {
        internal const string ArrowElement = "Arrow";
        internal const string MarkerGridElement = "markerGrid";
        internal const string tbMarkerElement = "tbMarker";
        internal const string AnimateElement = "Animate";

        //public static readonly DependencyProperty TimelineMarkerProperty =
        //    DependencyProperty.Register("TimelineMarker", typeof(TimelineMarker), typeof(Marker), null);

        //public TimelineMarker TimelineMarker
        //{
        //    get { return (TimelineMarker)GetValue(TimelineMarkerProperty); }
        //    set { SetValue(TimelineMarkerProperty, value); }
        //}

        private Ellipse _arrow;
        private Grid _markerGrid;
        private TextBlock _tbMarker;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Marker), null);
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty ArrowPositionProperty =
            DependencyProperty.Register("ArrowPosition", typeof(double), typeof(Marker), null);
        public double ArrowPosition
        {
            get { return (double)GetValue(ArrowPositionProperty); }
            set { SetValue(ArrowPositionProperty, value); }
        }
        private Storyboard _animate;
        public Storyboard Animate
        {
            get { return _animate; }
            set { _animate = value; }
        }
        //public event MarkerEventHandler Clicked;

        static Marker()
        {
            //base.DefaultStyleKey = typeof(Marker);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Marker), new FrameworkPropertyMetadata(typeof(Marker)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._arrow = base.GetTemplateChild(ArrowElement) as Ellipse;
            this._markerGrid = base.GetTemplateChild(MarkerGridElement) as Grid;
            this._tbMarker = base.GetTemplateChild(tbMarkerElement) as TextBlock;
            this.Animate = base.GetTemplateChild(AnimateElement) as Storyboard;
            if (this._arrow != null)
            {
                this._arrow.MouseLeftButtonUp += new MouseButtonEventHandler(Arrow_MouseLeftButtonUp);
                UpdateArrowPosition();
            }
            if (_tbMarker != null)
                _tbMarker.Text = this.Text;
        }


        public void UpdateArrowPosition(double position)
        {
            this.ArrowPosition = position;
            this.UpdateArrowPosition();
        }
        public void UpdateArrowPosition()
        {
            _arrow.Margin = new Thickness((this.ArrowPosition / 2) + 2, 4.5, 0, 0);
            this.Margin = new Thickness((this.ArrowPosition / 2), 0, 0, 0);
        }
        void Arrow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //OnMarkerClicked();
        }

        //private void OnMarkerClicked()
        //{
        //    if (Clicked != null)
        //    {
        //        Clicked(this, this.TimelineMarker);
        //    }
        //}
    }
}

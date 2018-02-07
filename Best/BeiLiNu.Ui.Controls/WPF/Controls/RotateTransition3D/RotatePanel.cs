using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class RotatePanel : Control
    {
        public Border FrontWarpper;
        public Border BackWarpper;
        private Viewport3D Viewport;
        private GeometryModel3D GeometryModel;
        private Storyboard FrontToBack;
        private Storyboard BackToFront;


        static RotatePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RotatePanel), new FrameworkPropertyMetadata(typeof(RotatePanel)));
        }


        public void Rotate(double angle)
        {
            FrontToBack = new Storyboard()
            {
                Children = new TimelineCollection()
                    {
                        GetShowHideAnimation(Viewport, 0, 1100),
                        GetShowHideAnimation(BackWarpper, 1000, -1),
                        GetShowHideAnimation(FrontWarpper, -1, 50),
                        GetFadeAnimation(FrontWarpper, 0, -1, 50),
                        GetFadeAnimation(BackWarpper, 1, 1050, 50),
                        GetCameraMoveAnimation(0, 0, 0.5, 0, 0, 1.1, 0, 500),
                        GetRotateAnimation(0,angle, 0.3, 0.3, 0, 1000)
                    }
            };

            BackToFront = new Storyboard()
            {
                Children = new TimelineCollection()
                    {
                        GetShowHideAnimation(Viewport, 0, 1100),
                        GetShowHideAnimation(FrontWarpper, 1000, -1),
                        GetShowHideAnimation(BackWarpper, -1, 50),
                        GetFadeAnimation(BackWarpper, 0, -1, 50),
                        GetFadeAnimation(FrontWarpper, 1, 1050, 50),
                        GetCameraMoveAnimation(0, 0, 0.5, 0, 0, 1.1, 0, 500),
                        GetRotateAnimation(angle,angle+angle, 0.3, 0.3, 0, 1000)
                    }
            };

            if (FrontWarpper.Opacity != 0)
            {
                FrontToBack.Begin();
            }
            else
            {
                BackToFront.Begin();
            }

        }
        public override void OnApplyTemplate()
        {
            FrontWarpper = Template.FindName("PART_FrontWarpper", this) as Border;
            BackWarpper = Template.FindName("PART_BackWarpper", this) as Border;
            GeometryModel = Template.FindName("PART_GeometryModel3D", this) as GeometryModel3D;
            Viewport = Template.FindName("PART_Viewport", this) as Viewport3D;

            this.Loaded += new RoutedEventHandler(PanelControl_Loaded);
        }

        private void PanelControl_Loaded(object sender, RoutedEventArgs e)
        {
            VisualBrush f = new VisualBrush(FrontWarpper.Child) { Stretch = Stretch.Uniform };
            VisualBrush b = new VisualBrush(BackWarpper.Child) { Stretch = Stretch.Uniform, RelativeTransform = new ScaleTransform(-1, 1, 0.5, 0) };

            GeometryModel.Material = new DiffuseMaterial(f);
            GeometryModel.BackMaterial = new DiffuseMaterial(b);
        }



        #region 动画

        private DoubleAnimation GetFadeAnimation(UIElement target, int toOpacity, int beginTime, int duration)
        {
            DoubleAnimation result = new DoubleAnimation(toOpacity, new Duration(TimeSpan.FromMilliseconds(duration)));
            if (beginTime >= 0)
            {
                result.BeginTime = TimeSpan.FromMilliseconds(beginTime);
            }
            Storyboard.SetTarget(result, target);
            Storyboard.SetTargetProperty(result, new PropertyPath("Opacity"));
            return result;
        }

        private Point3DAnimation GetCameraMoveAnimation(double x1, double y1, double z1, double x2, double y2, double z2, int beginTime, int duration)
        {
            Point3DAnimation result = new Point3DAnimation(new Point3D(x1, y1, z1), new Point3D(x2, y2, z2), new Duration(TimeSpan.FromMilliseconds(duration)))
            {
                AutoReverse = true,
                BeginTime = TimeSpan.FromMilliseconds(beginTime),
                DecelerationRatio = 0.3
            };
            Storyboard.SetTarget(result, this);
            Storyboard.SetTargetProperty(result, new PropertyPath("CameraPosition"));
            return result;
        }

        private DoubleAnimation GetRotateAnimation(double fromDegree, double toDegree, double accelerationRatio, double decelerationRatio, int beginTime, int duration)
        {
            DoubleAnimation result = new DoubleAnimation(fromDegree, toDegree, new Duration(TimeSpan.FromMilliseconds(duration)), FillBehavior.HoldEnd)
            {
                AccelerationRatio = accelerationRatio,
                DecelerationRatio = decelerationRatio,
                BeginTime = TimeSpan.FromMilliseconds(beginTime)
            };
            Storyboard.SetTarget(result, this);
            Storyboard.SetTargetProperty(result, new PropertyPath("Angle"));
            return result;
        }

        private ObjectAnimationUsingKeyFrames GetShowHideAnimation(UIElement element, int showTime, int hideTime)
        {
            ObjectAnimationUsingKeyFrames frame = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(frame, element);
            Storyboard.SetTargetProperty(frame, new PropertyPath("Visibility"));
            if (showTime >= 0)
            {
                frame.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.FromMilliseconds(showTime)));
            }
            if (hideTime >= 0)
            {
                frame.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden, TimeSpan.FromMilliseconds(hideTime)));
            }
            return frame;
        }

        #endregion

        #region 属性


        public Point3D CameraPosition
        {
            get { return (Point3D)GetValue(CameraPositionProperty); }
            set { SetValue(CameraPositionProperty, value); }
        }


        public static readonly DependencyProperty CameraPositionProperty =
            DependencyProperty.Register("CameraPosition", typeof(Point3D), typeof(RotatePanel), new PropertyMetadata(new Point3D(0, 0, 0.5)));




        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }


        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RotatePanel), new PropertyMetadata(0d));


        public Vector3D Axis
        {
            get { return (Vector3D)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }


        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(Vector3D), typeof(RotatePanel), new PropertyMetadata(new Vector3D(0d, 1d, 0d)));


        #endregion
    }
}

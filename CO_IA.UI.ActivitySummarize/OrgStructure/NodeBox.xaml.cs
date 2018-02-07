using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    public partial class NodeBox : UserControl
    {
        private double _fontSize = 12;

        public NodeBox(double scale)
        {
            _Scale = scale;
            InitializeComponent();

            tbEmployeeName.FontSize = _fontSize * Scale;
            tbEmployeeName.SetValue(Canvas.TopProperty, 5 * scale);
            //tbEmployeeName.SetValue(Canvas.LeftProperty, 5 * scale);
            tbEmployeeName.Height = 20 * scale;
            tbEmployeeName.Width = 170 * scale;
            tbEmployeeName.TextWrapping = TextWrapping.NoWrap;
            tbEmployeeName.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 170 * scale, 20 * scale) };

            tbTitle.FontSize = _fontSize * Scale;
            tbTitle.SetValue(Canvas.TopProperty, 25 * scale);
            tbTitle.SetValue(Canvas.LeftProperty, 5 * scale);
            tbTitle.Height = 20 * scale;
            tbTitle.Width = 130 * scale;
            tbTitle.TextWrapping = TextWrapping.NoWrap;
            tbTitle.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 130 * scale, 20 * scale) };

            tbSubLeader.FontSize = _fontSize * Scale;
            tbSubLeader.SetValue(Canvas.TopProperty, 45 * scale);
            tbSubLeader.SetValue(Canvas.LeftProperty, 5 * scale);
            tbSubLeader.Height = 20 * scale;
            tbSubLeader.TextWrapping = TextWrapping.NoWrap;
            tbSubLeader.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 130 * scale, 20 * scale) };



            tbDepartment.FontSize = _fontSize * Scale;
            tbDepartment.SetValue(Canvas.TopProperty, 65 * scale);
            tbDepartment.SetValue(Canvas.LeftProperty, 5 * scale);
            tbDepartment.Height = 20 * scale;
            tbDepartment.TextWrapping = TextWrapping.NoWrap;
            tbDepartment.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 130 * scale, 20 * scale) };


            tbEquiplist.FontSize = _fontSize * Scale;
            tbEquiplist.SetValue(Canvas.TopProperty, 85 * scale);
            tbEquiplist.SetValue(Canvas.LeftProperty, 5 * scale);
            tbEquiplist.Height = 20 * scale;
            tbEquiplist.TextWrapping = TextWrapping.NoWrap;
            tbEquiplist.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 130 * scale, 20 * scale) };

            tbVehicle.FontSize = _fontSize * Scale;
            tbVehicle.SetValue(Canvas.TopProperty, 100 * scale);
            tbVehicle.SetValue(Canvas.LeftProperty, 5 * scale);
            tbVehicle.Height = 20 * scale;
            tbVehicle.TextWrapping = TextWrapping.NoWrap;
            tbVehicle.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 130 * scale, 20 * scale) };

            recBorder.StrokeThickness = 2 * scale;
            recBorder.RadiusX = 5 * scale;
            recBorder.RadiusY = 5 * scale;
            recBorder.Width = this.Width * scale;
            recBorder.Height = this.Height * scale;
        }

        private double _Scale = 1;
        public double Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
            }
        }

        private string _ID;
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private string _EmployeeName;
        public string EmployeeName
        {
            get
            {
                return _EmployeeName;
            }
            set
            {
                _EmployeeName = value;
                if (!string.IsNullOrEmpty(_EmployeeName))
                {
                    tbEmployeeName.Text = _EmployeeName;
                }
                else
                {
                    tbEmployeeName.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                if (!string.IsNullOrEmpty(_Title))
                {
                    tbTitle.Text = _Title;
                }
                else
                {
                    tbTitle.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }


        private string _SubLeader;
        public string SubLeader
        {
            get
            {
                return _SubLeader;
            }
            set
            {
                _SubLeader = value;
                if (!string.IsNullOrEmpty(_SubLeader))
                {
                    tbSubLeader.Text = _SubLeader;
                }
                else
                {
                    tbSubLeader.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }


        private string _Department;
        public string Department
        {
            get
            {
                return _Department;
            }
            set
            {
                _Department = value;
                if (!string.IsNullOrEmpty(_Department))
                {
                    tbDepartment.Text = _Department;
                }
                else
                {
                    tbDepartment.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private string _Equiplist;
        public string Equiplist
        {
            get
            {
                return _Equiplist;
            }
            set
            {
                _Equiplist = value;

                if (!string.IsNullOrEmpty(_Equiplist))
                {
                    tbEquiplist.Text = _Equiplist;
                }
                else
                {
                    tbEquiplist.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }



        private string _Vehicle;
        public string Vehicle
        {
            get
            {
                return _Vehicle;
            }
            set
            {
                _Vehicle = value;

                if (!string.IsNullOrEmpty(_Vehicle))
                {
                    tbVehicle.Text = _Vehicle;
                }
                else
                {
                    tbVehicle.Visibility = System.Windows.Visibility.Collapsed;
                }

            }
        }
        private void canvMain_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.mouseEnter.Begin();
        }

        private void canvMain_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.mouseLeave.Begin();
        }
    }
}

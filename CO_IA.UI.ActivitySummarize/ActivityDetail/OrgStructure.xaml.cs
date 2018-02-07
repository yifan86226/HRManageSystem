using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using System.Xml.Linq;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// OrgStructure.xaml 的交互逻辑
    /// </summary>
    public partial class OrgStructure : UserControl
    {

        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;


        public List<Person> persons = new List<Person>();
        public double totalWidth;
        public double totalHight;
        public double buttonWidth;
        public double buttonHeight;
        public double plusButtonWidth;
        public double plusButtonHeight;
        public double levelHight;
        public double minHorizontalSpace;
        public double minVerticalSpace;
        public double fontSize;
        public SolidColorBrush LinesStroke;
        public double LinesStrokeThickness;
        public DispatcherTimer _doubleClickTimer;
        public double drawingScale = 1;


        public OrgStructure()
        {
            InitializeComponent();
            MyScoller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            MyScoller.PanningMode = PanningMode.HorizontalOnly;
            LoadXMLFile();
            MyCanvas.Margin = new Thickness(this.ActualWidth / 2, this.ActualHeight / 2, 0, 0);
        }

        private void Rescale()
        {
            MyCanvas.Children.Clear();
            buttonWidth = 170 * drawingScale;
            buttonHeight = 120 * drawingScale;
            minHorizontalSpace = 20 * drawingScale;
            minVerticalSpace = 20 * drawingScale;
            plusButtonWidth = 12 * drawingScale;
            plusButtonHeight = 12 * drawingScale;
            fontSize = 10 * drawingScale;
            LinesStrokeThickness = 1 * drawingScale;
            levelHight = buttonHeight + minVerticalSpace * 2;
            Refresh();
        }

        private void Refresh()
        {
            MyCanvas.Width = buttonWidth;
            MyCanvas.Height = buttonHeight;
            totalWidth = MyCanvas.Width;
            totalHight = MyCanvas.Height;
            persons[0].MinChildWidth = buttonWidth + minHorizontalSpace;
            persons[0].StartX = 0;
            persons[0].X = persons[0].MinChildWidth / 2;
            SetLevel(persons[0], 1);
            CalculateWidth(persons[0]);
            CalculateX(persons[0]);
            DrawNode(persons[0]);
        }

        #region Load XML
        private void LoadXMLFile()
        {



            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

            //新建还是读取
            if (activity == null || string.IsNullOrEmpty(activity.Guid) == true)
            {
                //创建默认的组织结构组
                //CreateAndSaveDefaultOrgInfos();
            }
            else
            {
                //所得所有已存的人员组织结构
                List<PP_OrgInfo> orgList = new List<PP_OrgInfo>();



                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    orgList = channel.GetPP_OrgInfos(activity.Guid);
                });

                //新建还是读取
                if (orgList.Count == 0)
                {
                    //创建默认的组织结构组
                    //CreateAndSaveDefaultOrgInfos();
                }
                else
                {
                    List<Person> tempPersonList = new List<Person>();
                    Person tempPerson = new Person();
                    foreach (PP_OrgInfo oinfo in orgList)
                    {
                        tempPerson = new Person();

                        tempPerson.ID = oinfo.GUID;
                        tempPerson.Opened = true;

                        /// <summary>
                        /// 每个节点获取的人员列表
                        /// </summary>
                        List<PP_PersonInfo> itemPersonList = new List<PP_PersonInfo>();


                        /// <summary>
                        /// 当前组员列表
                        /// </summary>
                        List<PP_PersonInfo> itemGrouperList = new List<PP_PersonInfo>();

                        /// <summary>
                        /// 每个节点获取的车辆信息
                        /// </summary>
                        List<MonitorStationEquInfo> itemEquipList = new List<MonitorStationEquInfo>();

                        /// <summary>
                        /// 每个节点获取的设备列表
                        /// </summary>
                        PP_VehicleInfo itemVehicle = new PP_VehicleInfo();

                        //读取人员信息
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                        {
                            //更新当前节点
                            itemPersonList = channel.GetPP_PersonInfos(oinfo.GUID);
                        });

                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                        {
                            //更新当前节点
                            itemEquipList = channel.GetPP_EqupInfos(oinfo.GUID);
                        });
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                        {
                            //更新当前节点
                            itemVehicle = channel.GetPP_VehicleInfo(oinfo.GUID);
                        });

                        if (1 == 1)
                        {
                            tempPerson.Name = oinfo.NAME + "(监测组)";
                        }
                        else
                        {
                            tempPerson.Name = oinfo.NAME;
                        }
                        //根节点
                        if (oinfo.PARENT_GUID == "")
                        {
                            //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                            foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                            {
                                if (tempPersonInfo.PERSON_TYPE == 4)
                                {
                                    itemGrouperList.Add(tempPersonInfo);
                                }
                                else if (tempPersonInfo.PERSON_TYPE == 0)
                                {
                                    tempPerson.Title = "主   任： " + tempPersonInfo.NAME;

                                }
                                else if (tempPersonInfo.PERSON_TYPE == 3)
                                {
                                    tempPerson.SubLeader = "协调员： " + tempPersonInfo.NAME;
                                }
                            }


                        }
                        //其他组织节点
                        else
                        {
                            //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                            foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                            {
                                if (tempPersonInfo.PERSON_TYPE == 4)
                                {
                                    itemGrouperList.Add(tempPersonInfo);

                                }
                                else if (tempPersonInfo.PERSON_TYPE == 1 && !String.IsNullOrEmpty(tempPersonInfo.NAME))
                                {
                                    tempPerson.Title = "组   长： " + tempPersonInfo.NAME;
                                }
                                else if (tempPersonInfo.PERSON_TYPE == 2 && !String.IsNullOrEmpty(tempPersonInfo.NAME))
                                {
                                    tempPerson.SubLeader = "副组长： " + tempPersonInfo.NAME;
                                }
                            }
                        }
                        if (itemGrouperList != null && itemGrouperList.Count > 0)
                        {
                            tempPerson.Department = "组   员： " + itemGrouperList.Count + "  名 ";
                        }
                        if (itemEquipList != null && itemEquipList.Count > 0)
                        {
                            tempPerson.Equiplist = "设   备： " + itemEquipList.Count + "  台";
                        }
                        if (itemVehicle != null && !String.IsNullOrEmpty(itemVehicle.VEHICLE_NUMB))
                        {
                            tempPerson.Vehicle = "车牌号： " + itemVehicle.VEHICLE_NUMB;
                        }
                        if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
                        {
                            tempPerson.ManagerID = "";
                            tempPerson.MinChildWidth = totalWidth;
                            persons.Add(tempPerson);
                        }
                        else
                        {
                            tempPerson.ManagerID = oinfo.PARENT_GUID;
                            tempPersonList.Add(tempPerson);
                        }
                    }
                    persons.AddRange(tempPersonList);
                    Rescale();
                }
            }
        }

        #endregion

        #region Calculate Values
        private void SetLevel(Person parent, int level)
        {
            // Set the node level
            parent.Level = level;

            // Calculate the total height based on the number of levels
            if (totalHight < levelHight * (level + 2))
            {
                totalHight = levelHight * (level + 2);
                MyCanvas.Height = totalHight;
            }

            // Select the closed items under this parent
            var resultN = from n in persons
                          where n.ManagerID == parent.ID && n.Opened == false
                          select n;

            // Get the closed nodes number
            parent.HiddenSubNodes = resultN.Count();

            // Select the opend nodes under this parent
            var result = from n in persons
                         where n.ManagerID == parent.ID && n.Opened == true
                         select n;

            Person[] nodes = result.ToArray();

            // Get the Opend nodes number
            parent.SubNodes = nodes.Length;

            // Call the child nodes
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].NodeOrder = i + 1;
                nodes[i].MinChildWidth = buttonWidth + minHorizontalSpace;
                SetLevel(nodes[i], parent.Level + 1);
            }
        }

        private void CalculateWidth(Person parent)
        {
            if (parent.SubNodes > 0)
            {
                var result = from n in persons
                             where n.ManagerID == parent.ID && n.Opened == true
                             orderby n.NodeOrder
                             select n;

                Person[] nodes = result.ToArray();
                double width = 0;

                for (int i = 0; i < nodes.Length; i++)
                {
                    // calculate the width of the child before adding it to the parent
                    CalculateWidth(nodes[i]);

                    // calculate the new width
                    width = width + nodes[i].MinChildWidth;
                }

                if (width > parent.MinChildWidth)
                {
                    parent.MinChildWidth = width;
                    if (MyCanvas.Width < width)
                    {
                        MyCanvas.Width = width;
                        //totalWidth = width;
                    }
                }
            }
        }

        private void CalculateX(Person parent)
        {
            if (parent.SubNodes > 0)
            {
                var result = from n in persons
                             where n.ManagerID == parent.ID && n.Opened == true
                             orderby n.NodeOrder
                             select n;

                Person[] nodes = result.ToArray();

                // Calculate the startX for each node
                double start = parent.StartX;
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].StartX = start;
                    nodes[i].X = nodes[i].StartX + nodes[i].MinChildWidth / 2;
                    CalculateX(nodes[i]);
                    start = start + nodes[i].MinChildWidth;
                }

                // realign the parent node to the middle of the child nodes
                if (nodes.Length > 1)
                {
                    parent.X = (nodes[0].X + nodes[nodes.Length - 1].X) / 2;
                }
                else // root element
                {
                    parent.X = nodes[0].X;
                }
            }
        }
        #endregion

        #region Draw
        private void DrawNode(Person parent)
        {
            // Check if the current node is the parent node or not
            if (parent.ManagerID == "")
            {
                AddBox(MyCanvas, parent.X, parent.Level * levelHight, null, parent.ID, parent.Name, parent.Title, parent.SubLeader, parent.Department, parent.Equiplist, parent.Vehicle, false, true, parent.HiddenSubNodes > 0);
            }

            // Get the child nodes
            var results = from n in persons
                          where n.ManagerID == parent.ID && n.Opened == true
                          select n;

            foreach (Person p in results)
            {
                AddBox(MyCanvas, p.X, p.Level * levelHight, parent.X, p.ID, p.Name, p.Title, p.SubLeader, p.Department, p.Equiplist, p.Vehicle, true, p.SubNodes > 0, p.HiddenSubNodes > 0);
                DrawNode(p);
            }
        }

        public void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            //line.Stroke = Util.GetColorFromHex("#FF6495ed");
            line.Stroke = Util.GetColorFromHex("#FFFFFFFF");
            line.StrokeThickness = LinesStrokeThickness;
            canvas.Children.Add(line);
        }

        public void AddBox(Canvas canvas, double x, double y, double? parentX, string ID, string name, string title, string subLeader, string department, string equiplist, string vehicle, bool root, bool subNodes, bool hiddenSubNodes)
        {
            NodeBox nb = new NodeBox(drawingScale);
            //nb.Name = ID;
            if (name.Length > 12)
            {
                nb.EmployeeName = name.Substring(0, 12) + "...";
            }
            else
            {
                nb.EmployeeName = name;
            }

            nb.Title = title;
            nb.SubLeader = subLeader;
            nb.Department = department;
            nb.Equiplist = equiplist;
            nb.Vehicle = vehicle;
            nb.Width = buttonWidth;
            nb.Height = buttonHeight;
            nb.SetValue(Canvas.LeftProperty, x - nb.Width / 2);
            nb.SetValue(Canvas.TopProperty, y);

            // handle the double click actions
            _doubleClickTimer = new DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _doubleClickTimer.Tick += new EventHandler(delegate { _doubleClickTimer.Stop(); });
            nb.MouseLeftButtonDown += new MouseButtonEventHandler(delegate
            {
                if (_doubleClickTimer.IsEnabled)
                {
                    _doubleClickTimer.Stop();
                    btnClicked(ID);
                }
                else
                {
                    _doubleClickTimer.Start();
                }
            });
            nb.ToolTip = GetTooltip(name);

            canvas.Children.Add(nb);

            // The line above the box
            if (root)
                DrawLine(canvas, x, y - minVerticalSpace, x, y);


            // Draw the horizontal line to the parent
            if (parentX != null)
                DrawLine(canvas, x, y - minVerticalSpace, Convert.ToDouble(parentX), y - minVerticalSpace);


            // Draw the line under the box
            if (subNodes || hiddenSubNodes)
                DrawLine(canvas, x, y + buttonHeight, x, y + buttonHeight + minVerticalSpace);

            // display the small plus
            if (hiddenSubNodes)
            {
                //Button btn = new Button();
                //btn.Name = "plus" + ID;
                //btn.FontSize = fontSize / 2;
                //btn.Click += new RoutedEventHandler(btn_Click);
                //btn.Height = plusButtonHeight;
                //btn.Width = plusButtonWidth;
                //btn.Content = "+";
                //btn.SetValue(Canvas.LeftProperty, x - btn.Width / 2);
                //btn.SetValue(Canvas.TopProperty, y + buttonHeight + minVerticalSpace - minVerticalSpace / 2);
                //canvas.Children.Add(btn);
            }
        }
        #endregion

        #region Events
        void btn_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            btnClicked(clicked.Name.Replace("plus", ""));
        }

        public void btnClicked(string clickedButtonName)
        {

            //for (int i = 0; i < persons.Count; i++)
            //{
            //    if (persons[i].ID == clickedButtonName)
            //    {
            //        MyCanvas.Children.Clear();
            //        var results = from n in persons
            //                      where n.ManagerID == persons[i].ID
            //                      select n;

            //        persons[i].Collapsed = !persons[i].Collapsed;
            //        foreach (Person p in results)
            //        {
            //            p.Opened = !persons[i].Collapsed;
            //        }

            //        Refresh();
            //        return;
            //    }
            //}
        }
        #endregion
        private bool dragging;
        Point mousePoint;
        Rectangle shadow = new Rectangle();//显示控件阴影的矩形   
        //Control mouseCtrl = null;
        Canvas mouseCtrl = null;

        //被拖动的控件
        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragging = true;//标记鼠标按下   
                mousePoint = e.GetPosition(this.MyCanvas);//获取鼠标在但前canvas内的位置   
                //if (e.Source.GetType().Name == "NodeBox")
                //{
                //    return;
                //}
                //mouseCtrl = (Control)e.Source;   //获得事件触发的源，即哪个控件     
                //mouseCtrl = (Canvas)e.Source;   //获得事件触发的源，即哪个控件   
                mouseCtrl = (Canvas)sender;
                VisualBrush v;
                v = new VisualBrush(mouseCtrl);//利用VisualBrush得到控件的影像   
                shadow.Width = mouseCtrl.Width;
                shadow.Height = mouseCtrl.Height;
                shadow.Fill = v;//将影像填充给矩形   
                Canvas.SetLeft(shadow, Canvas.GetLeft(mouseCtrl));
                Canvas.SetTop(shadow, Canvas.GetTop(mouseCtrl));
                shadow.Visibility = Visibility.Visible;//使矩形可见   
                //Canvas.SetZIndex(shadow, 0);//可以通过SetZIndex设置阴影的z方向位置   
                MyCanvas.CaptureMouse();//强制捕获鼠标。这在对于背景透明的窗体里面是必须的   
            }
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point theMousePoint = e.GetPosition(this.MyCanvas);
                    Canvas.SetLeft(shadow, theMousePoint.X - (mousePoint.X - Canvas.GetLeft(shadow)));
                    Canvas.SetTop(shadow, theMousePoint.Y - (mousePoint.Y - Canvas.GetTop(shadow)));//简单的计算，只移动shadow   
                    mousePoint = theMousePoint;
                }
            }
        }

        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            Mouse.Capture(null);//取消强制捕获   
            shadow.Visibility = Visibility.Hidden;//隐藏shadow   
            if (mouseCtrl != null)
            {
                Canvas.SetLeft(mouseCtrl, Canvas.GetLeft(shadow));
                Canvas.SetTop(mouseCtrl, Canvas.GetTop(shadow));//将控件放到新的位置 
            }
        }

        private ToolTip GetTooltip(string msg)
        {
            StackPanel sp = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Padding = new Thickness(10);
            tb.TextWrapping = TextWrapping.WrapWithOverflow;
            tb.Width = 150;
            tb.Text = msg;
            sp.Children.Add(tb);

            ToolTip ttp = new ToolTip();
            ttp.Content = sp;

            return ttp;
        }

    }
}

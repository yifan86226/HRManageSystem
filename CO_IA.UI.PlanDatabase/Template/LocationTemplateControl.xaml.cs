using CO_IA.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CO_IA.Client;
using CO_IA.Data.Template;
namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// LocationTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class LocationTemplateControl : UserControl
    {
        private List<ActivityEquipment> listActivityEquipment = new List<ActivityEquipment>();
        public LocationTemplateControl()
        {
            InitializeComponent();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase>(channel =>
            {
                var orgs = channel.GetOrganizations();
                Equipments.EquipmentListControl.OrganizationList.Clear();
                if (orgs != null)
                {
                    Equipments.EquipmentListControl.OrganizationList.AddRange(orgs);
                }
            });
            this.DataContextChanged += LocationTemplateControl_DataContextChanged;
        }

        private void LocationTemplateControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var template = e.NewValue as CO_IA.Data.Template.ActivityTemplate;
            if (template != null)
            {
                List<TemplatePlace> list = new List<TemplatePlace>();
                listActivityEquipment.Clear();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                    {
                        var te=channel.GetTemplatePlaces(template.Guid);
                        list.AddRange(te);
                        this.listBoxPlace.ItemsSource = new ObservableCollection<TemplatePlace>(list);// channel.GetTemplatePlaces(template.Guid);
                        if (list.Count > 0)
                        {
                            this.listBoxPlace.SelectedIndex = 0;
                        }
                    });
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                {
                    listActivityEquipment.AddRange(channel.GetTemplateEuipments(template.Guid));
                    //this.planDtatabaseEquipmentList.DataContext = new ObservableCollection<ActivityEquipment>(channel.GetTemplateEuipments(template.Guid));
                });
            }
        }

        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var placeInfo = e.AddedItems[0] as TemplatePlace;
                if (placeInfo != null)
                {
                    placeInfo.IsChecked = false;
                    UpdateEquipmentList(placeInfo.Key);
                    //var view = System.Windows.Data.CollectionViewSource.GetDefaultView(this.planDtatabaseEquipmentList.DataContext);
                    //if (view.CanFilter)
                    //{
                    //    view.Filter = current =>
                    //    {
                    //        ActivityEquipment currentEquipment = current as ActivityEquipment;
                    //        if (currentEquipment != null)
                    //        {
                    //            return currentEquipment.PlaceGuid == placeInfo.Guid;
                    //        }
                    //        return false;
                    //    };
                    //}
                }
            }
        }

        private void UpdateEquipmentList(string placeGuid)
        {
            var equipmentList= from equipment in this.listActivityEquipment where equipment.PlaceGuid == placeGuid select equipment;
            //foreach (var equipment in equipmentList)
            //{
            //    equipment.IsChecked = false;
            //}
            this.planDtatabaseEquipmentList.DataContext = equipmentList;
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            var currentPlace = this.listBoxPlace.SelectedItem as TemplatePlace;
            if (currentPlace == null)
            {
                return;
            }
            List<string> ignoreEquipmentGuidList = null;
            if (this.listActivityEquipment.Count > 0)
            {
                ignoreEquipmentGuidList = (from equip in listActivityEquipment select equip.Key).ToList();
            }
            EquipmentSelectWindow wnd = new EquipmentSelectWindow(ignoreEquipmentGuidList);
            if (wnd.ShowDialog(this) == true)
            {
                var listEquipment = wnd.GetSelectedEquipmentList();
                if (listEquipment.Count > 0)
                {
                    ActivityEquipment[] equipments = new ActivityEquipment[listEquipment.Count];
                    for (int i = 0; i < equipments.Length; i++)
                    {
                        equipments[i] = new ActivityEquipment { ActivityGuid = currentPlace.TemplateGuid, PlaceGuid = currentPlace.Key };
                        equipments[i].CopyFrom(listEquipment[i]);
                    }
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.SaveTemplateEquipments(equipments);
                        });
                    this.listActivityEquipment.AddRange(equipments);
                    this.UpdateEquipmentList(currentPlace.Key);
                    //var collectionEquipment=this.planDtatabaseEquipmentList.DataContext as ObservableCollection<ActivityEquipment>;
                    //if (collectionEquipment != null)
                    //{
                    //    foreach (var equipment in equipments)
                    //    {
                    //        collectionEquipment.Add(equipment);
                    //    }
                    //}
                }
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentPlace = this.listBoxPlace.SelectedItem as TemplatePlace;
            if (currentPlace == null)
            {
                MessageBox.Show("请先选择活动区域");
                return;
            }
            ActivityEquipment[] deleteEquipments=null;
            var equipmentList = this.planDtatabaseEquipmentList.DataContext as IEnumerable<ActivityEquipment>;
            if (equipmentList != null)
            {
                deleteEquipments=(from equipment in equipmentList where equipment.IsChecked && equipment.PlaceGuid==currentPlace.Key select equipment).ToArray();
            }
            if (deleteEquipments != null && deleteEquipments.Length>0)
            {
                if (MessageBox.Show("确实要删除选中的设备吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string[] equipmentGuids = (from equipment in deleteEquipments select equipment.Key).ToArray();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.DeleteTemplateEquipments(currentPlace.TemplateGuid, equipmentGuids);
                        });
                    foreach (var equip in deleteEquipments)
                    {
                        this.listActivityEquipment.Remove(equip);
                    }
                    this.UpdateEquipmentList(currentPlace.Key);
                }
            }
            else
            {
                MessageBox.Show("没有需要删除的设备!");
            }
        }

        private void buttonAddPlace_Click(object sender, RoutedEventArgs e)
        {
            var template=this.DataContext as ActivityTemplate;
            if (template != null)
            {
                TemplatePlace placeInfo = new TemplatePlace();
                placeInfo.Key = Utility.NewGuid();
                placeInfo.Name = "新增区域";
                placeInfo.TemplateGuid = template.Guid;
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                {
                    channel.SaveTemplatePlaces(new TemplatePlace[] { placeInfo });
                });
                (this.listBoxPlace.ItemsSource as ObservableCollection<TemplatePlace>).Add(placeInfo);
            }
        }

        private void buttonRenameOk_Click(object sender, RoutedEventArgs e)
        {
            var placeInfo = (sender as Button).DataContext as TemplatePlace;
            if (placeInfo != null)
            {
                placeInfo.IsChecked = false;
                TextBox editTextBox=(sender as Button).Tag as TextBox;
                if (editTextBox != null && !string.IsNullOrWhiteSpace(editTextBox.Text))
                {
                    placeInfo.Name = editTextBox.Text;
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.SaveTemplatePlaces(new TemplatePlace[] { placeInfo });
                        });
                }
            }
        }

        private void buttonLocationEdit_Click(object sender, RoutedEventArgs e)
        {
            var placeInfo = (sender as Button).DataContext as TemplatePlace;
            if (placeInfo != null)
            {
                placeInfo.IsChecked = true;
            }
        }

        private void listBoxPlace_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void buttonLocationDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要删除选中的活动区域吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var placeInfo = (sender as Button).DataContext as TemplatePlace;
                if (placeInfo != null)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.DeleteTemplatePlace(placeInfo.Key);
                        });
                    var equipments = (from data in this.listActivityEquipment where data.PlaceGuid == placeInfo.Key select data).ToArray();
                    foreach (var equipment in equipments)
                    {
                        this.listActivityEquipment.Remove(equipment);
                    }
                    var index=this.listBoxPlace.SelectedIndex;
                    var placeList=this.listBoxPlace.ItemsSource as ObservableCollection<TemplatePlace>;
                    placeList.Remove(placeInfo);
                    if (index<placeList.Count)
                    {
                        this.listBoxPlace.SelectedIndex=index;
                    }
                    else
                    {
                        this.listBoxPlace.SelectedIndex=placeList.Count-1;
                    }
                    if (this.listBoxPlace.SelectedIndex ==-1)
                    {
                        this.planDtatabaseEquipmentList.DataContext=new ObservableCollection<ActivityEquipment>();
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CO_IA.Data.FileManage;

namespace CO_IA.UI.ActivityManage.FileManage
{

    public class CatalogTreeNodess : INotifyPropertyChanged
    {
        //public CatalogTreeNode()
        //{
        //    Children = new ObservableCollection<CatalogTreeNode>();
        //}

        public int Level { get; set; }

        private CatalogInfo catalog;
        public CatalogInfo Catalog
        {
            get 
            {
                return catalog;
            }
            set 
            {
                catalog = value;
                NotifyPropertyChanged("Catalog");
            }
        }


        //private ObservableCollection<CatalogTreeNode> children;
        //public ObservableCollection<CatalogTreeNode> Children
        //{
        //    get { return children; }
        //    set { children = value; }
        //}


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

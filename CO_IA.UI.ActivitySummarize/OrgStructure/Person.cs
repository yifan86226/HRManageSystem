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

namespace CO_IA.UI.ActivitySummarize
{
    public class Person
    {
        public string ID;
        public string Name;
        public string ManagerID;
        public string Title;
        public string SubLeader;
        public string Department;
        public string Equiplist;
        public string Vehicle;

        #region More Fileds
        public int Level = 1;
        public int SubNodes = 0;
        public int HiddenSubNodes = 0;
        public int NodeOrder = 1;
        public double MinChildWidth;
        public double X;
        public double StartX;
        public Boolean Opened = false;        
        public Boolean Collapsed = true;
        #endregion

        //public static Person GetPerson(string id, string name, string managerID, string title, string department, string extension, string email)
        //{
        //    Person p = new Person();
        //    p.ID = id;
        //    p.Name = name;
        //    p.ManagerID = managerID;
        //    p.Title = title;
        //    p.SubLeader = department;
        //    p.Extension = extension;
        //    p.Email = email;
        //    return p;
        //}

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CO_IA.UI.MonitorTask
{
    public partial class PdfReader : UserControl
    {
        public PdfReader()
        {
            InitializeComponent();
            //this.OpenFile(filePath);
        }

        public void OpenFile(string filePath)
        {
            this.axAcroPDF.LoadFile(filePath);
        }
    }
}

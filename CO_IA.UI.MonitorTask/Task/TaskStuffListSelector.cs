using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.MonitorTask.Task
{
    public class TaskStuffListSelector : DataTemplateSelector
    {
        private readonly static AT_BC.Data.FileTypeGetter fileTypeGetter = new AT_BC.Data.FileTypeGetter();
        private DataTemplate audioStuffTemplate = null;

        public DataTemplate AudioStuffTemplate
        {
            get { return audioStuffTemplate; }
            set { audioStuffTemplate = value; }
        }

        private DataTemplate fileStuffTemplate = null;
        public DataTemplate FileStuffTemplate
        {
            get { return fileStuffTemplate; }
            set { fileStuffTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AT_BC.Data.IFileDescription)
            {
                var fileDesc=item as AT_BC.Data.IFileDescription;
                if (fileTypeGetter.GetFileType(fileDesc.Extension) == AT_BC.Data.FileType.Sound)
                {
                    return this.AudioStuffTemplate;
                }
                else
                {
                    return this.FileStuffTemplate;
                }
                //if ((item as CO_IA.Data.TaskStuff). == AT_BC.Data.FormState.Check)
                //{
                //    return this.AudioStuffTemplate;
                //}
                //else
                //{
                //    return this.FileStuffTemplate;
                //}
            }

            return base.SelectTemplate(item, container);
        }

    }
}

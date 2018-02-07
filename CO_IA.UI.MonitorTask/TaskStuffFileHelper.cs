using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using AT_BC.Offices;

namespace CO_IA.UI.MonitorTask
{
    internal static class TaskStuffFileHelper
    {
        private static string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaskStuffs");
        public static string SaveStuffFile(TaskStuff taskStuff)
        {
            string filePath = System.IO.Path.Combine(basePath, string.Format("{0}.{1}", taskStuff.Key, ((AT_BC.Data.IFileDescription)taskStuff).Extension));
            if (!System.IO.File.Exists(filePath))
            {
                if (!taskStuff.IsLoaded)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        var result = channel.GetTaskStuffByGuid(taskStuff.Key);
                        if (result.Content != null && result.Content.Length > 0)
                        {
                            taskStuff.Content = result.Content;
                        }
                    });
                }
                using (var fs = System.IO.File.Create(filePath))
                {
                    fs.Write(taskStuff.Content, 0, taskStuff.Content.Length);
                    //fs.Close();
                }
            }
            return filePath;
        }

        public static string GetDisplayFile(TaskStuff taskStuff)
        {
            string filePath = SaveStuffFile(taskStuff);
            if (!OfficeConverter.IsValidOfficeFile(filePath))
            {
                return filePath;
            }
            else
            {
                string xpsFilePath = System.IO.Path.Combine(basePath, string.Format("{0}.xps", taskStuff.Key));
                OfficeConverter.ConvertToXps(filePath, xpsFilePath);
                return xpsFilePath;
            }
        }

        private static string[] imageExts = new string[] { "png", "jpg", "jpeg", "bmp", "tiff" };

        public static bool IsImage(AT_BC.Data.IFileDescription fileDescription)
        {
            string extension = fileDescription.Extension;
            foreach (string ext in imageExts)
            {
                if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static BitmapImage GetImage(TaskStuff taskStuff)
        {
            if (!taskStuff.IsLoaded)
            {
                if (!taskStuff.IsLoaded)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        var result = channel.GetTaskStuffByGuid(taskStuff.Key);
                        if (result.Content != null && result.Content.Length > 0)
                        {
                            taskStuff.Content = result.Content;
                        }
                    });
                }
            }
            MemoryStream stream = new MemoryStream(taskStuff.Content);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();//初始化
            bmp.StreamSource = stream;//设置源
            bmp.EndInit();//初始化结束
            return bmp;
        }

        public static void OpenFile(object sender, ExecutedRoutedEventArgs e,System.Windows.Window ownerWindow)
        {
            if (e.Parameter is TaskStuff)
            {
                var taskStuff = e.Parameter as TaskStuff;
                if (TaskStuffFileHelper.IsImage(taskStuff))
                {
                    var image = TaskStuffFileHelper.GetImage(taskStuff);
                    ImageWindow wnd = new ImageWindow();
                    wnd.LoadImage(image);
                    wnd.ShowDialog(ownerWindow);
                }
                else
                {
                    string displayFile = TaskStuffFileHelper.GetDisplayFile(taskStuff);
                    if (displayFile.EndsWith(".xps", StringComparison.OrdinalIgnoreCase))
                    {
                        XpsDocumentWindow wnd = new XpsDocumentWindow();
                        wnd.OpenXpsDocument(displayFile);
                        wnd.ShowDialog(ownerWindow);
                    }
                    else if (displayFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || displayFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        var player = CO_IA.Client.Utility.AudioPlayer;
                        player.Stop();
                        player.Open(new Uri(displayFile, UriKind.RelativeOrAbsolute));
                        player.Play();
                    }
                    else
                    {
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo { FileName = displayFile };
                        p.Start();
                    }
                }
            }
        }
    }
}

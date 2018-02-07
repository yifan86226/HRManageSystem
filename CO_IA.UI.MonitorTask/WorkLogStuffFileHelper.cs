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
    internal static class WorkLogStuffFileHelper
    {
        private static string[] imageExts = new string[] { "png", "jpg", "jpeg", "bmp", "tiff" };
        private static string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkLogStuffs");
        static WorkLogStuffFileHelper()
        {
            if (!System.IO.Directory.Exists(basePath))
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
        }
        public static string SaveStuffFile(WorkLogStuff logStuff)
        {
            string filePath = System.IO.Path.Combine(basePath, string.Format("{0}.{1}", logStuff.Key, ((AT_BC.Data.IFileDescription)logStuff).Extension));
            if (!System.IO.File.Exists(filePath))
            {
                if (!logStuff.IsLoaded)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        byte[] result = channel.GetWorkLogStuffByID(logStuff.Key);
                        if (result != null && result.Length > 0)
                        {
                            logStuff.Content = result;
                        }
                    });
                }
                using (var fs = System.IO.File.Create(filePath))
                {
                    fs.Write(logStuff.Content, 0, logStuff.Content.Length);
                    //fs.Close();
                }
            }
            return filePath;
        }

        public static string GetDisplayFile(WorkLogStuff logStuff)
        {
            string filePath = SaveStuffFile(logStuff);
            if (!OfficeConverter.IsValidOfficeFile(filePath))
            {
                return filePath;
            }
            else
            {
                string xpsFilePath = System.IO.Path.Combine(basePath, string.Format("{0}.xps", logStuff.Key));
                OfficeConverter.ConvertToXps(filePath, xpsFilePath);
                return xpsFilePath;
            }
        }

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

        public static BitmapImage GetImage(WorkLogStuff logStuff)
        {
            if (!logStuff.IsLoaded)
            {
                if (!logStuff.IsLoaded)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        byte[] result = channel.GetWorkLogStuffByID(logStuff.Key);
                        if (result != null && result.Length > 0)
                        {
                            logStuff.Content = result;
                        }
                    });
                }
            }

            MemoryStream stream = new MemoryStream(logStuff.Content);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();//初始化
            bmp.StreamSource = stream;//设置源
            bmp.EndInit();//初始化结束
            return bmp;
        }

        public static void OpenFile(object sender, ExecutedRoutedEventArgs e, System.Windows.Window ownerWindow)
        {
            if (e.Parameter is WorkLogStuff)
            {
                var logStuff = e.Parameter as WorkLogStuff;
                if (TaskStuffFileHelper.IsImage(logStuff))
                {
                    var image = WorkLogStuffFileHelper.GetImage(logStuff);
                    ImageWindow wnd = new ImageWindow();
                    wnd.LoadImage(image);
                    wnd.ShowDialog(ownerWindow);
                }
                else
                {
                    string displayFile = WorkLogStuffFileHelper.GetDisplayFile(logStuff);
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

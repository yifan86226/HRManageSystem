using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Utility
{
    public class PackTool
    {        
        class Header
        {
            public long length;
            public string filename;

            public void WriteTo(Stream fs)
            {
                byte[] len = BitConverter.GetBytes(length);
                byte[] buf = new byte[256];

                byte[] str = Encoding.Unicode.GetBytes(filename);
                str.CopyTo(buf, 0);

                fs.Write(len, 0, len.Length);
                fs.Write(buf, 0, buf.Length);
            }
            public bool ReadFrom(Stream fs)
            {
                byte[] len = BitConverter.GetBytes(length);
                byte[] buf = new byte[256];

                if (len.Length != fs.Read(len, 0, len.Length)) return false;
                if (buf.Length != fs.Read(buf, 0, buf.Length)) return false;

                length = BitConverter.ToInt64(len, 0);
                filename = Encoding.Unicode.GetString(buf).Trim(new char[] { '\0' });
                return true;
            }
        }

        public static void Pack(string resultFilename, params string[] filenames)
        {
            using (FileStream fout = new FileStream(resultFilename, FileMode.Create, FileAccess.Write))
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                    using (FileStream fin = new FileStream(filenames[i], FileMode.Open))
                    {
                        Header header = new Header();
                        header.length = fin.Length;
                        header.filename = Path.GetFileName(filenames[i]);
                        header.WriteTo(fout);

                        byte[] buf = new byte[header.length];
                        fin.Read(buf, 0, buf.Length);
                        fout.Write(buf, 0, buf.Length);
                    }
                }
            }
        }

        public static void Pack(string pTargetPath, List<string> filenames, string pId)
        {
            if (!File.Exists(pTargetPath))
            {
                var fs = File.Create(pTargetPath);
                fs.Close();
            }

            using (FileStream fout = new FileStream(pTargetPath, FileMode.Create, FileAccess.Write))
            {
                for (int i = 0; i < filenames.Count; i++)
                {
                    using (FileStream fin = new FileStream(filenames[i], FileMode.Open))
                    {
                        Header header = new Header();
                        header.length = fin.Length;
                        string n = Path.GetFileName(filenames[i]);
                        header.filename = n.Equals("f.xml")? n : n.Replace(".", "_" + pId + ".");
                        header.WriteTo(fout);

                        byte[] buf = new byte[header.length];
                        fin.Read(buf, 0, buf.Length);
                        fout.Write(buf, 0, buf.Length);
                    }
                }
            }
        }

        public static string[] Unpack(string filename, string pTargetPath)
        {
            List<string> unpackedFiles = new List<string>();

            using (FileStream fin = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                Header header = new Header();
                while (header.ReadFrom(fin))
                {
                    unpackedFiles.Add(header.filename);
                    byte[] buf = new byte[header.length];
                    fin.Read(buf, 0, buf.Length);

                    using (FileStream fout = new FileStream(pTargetPath +"\\" + header.filename, FileMode.Create, FileAccess.Write))
                    {
                        fout.Write(buf, 0, buf.Length);
                    }
                }
            }
            return unpackedFiles.ToArray();
        }
    }
}

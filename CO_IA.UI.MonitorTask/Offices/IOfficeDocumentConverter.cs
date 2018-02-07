using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Offices
{
    public interface IOfficeDocumentConverter
    {
        void ConvertFromWord(string sourcePath, string targetPath);
        
        void ConvertFromExcel(string sourcePath, string targetPath);

        void ConvertFromPowerPoint(string sourcePath, string targetPath);
    }
}

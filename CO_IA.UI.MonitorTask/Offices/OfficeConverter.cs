using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Offices
{
    public static class OfficeConverter
    {
        public static void ConvertToXps(string srcFilePath, string destFilePath)
        {
            switch (GetPathType(srcFilePath))
            {
                case OfficePathType.PowerPoint:
                    OfficeToXpsConverter.ConvertFromPowerPoint(srcFilePath, destFilePath);
                    break;
                case OfficePathType.Word:
                    OfficeToXpsConverter.ConvertFromWord(srcFilePath, destFilePath);
                    break;
                case OfficePathType.Excel:
                    OfficeToXpsConverter.ConvertFromExcel(srcFilePath, destFilePath);
                    break;
                default:
                    break;
            }
        }

        public static void ConvertToPdf(string srcFilePath, string destFilePath)
        {
            switch (GetPathType(srcFilePath))
            {
                case OfficePathType.PowerPoint:
                    OfficeToPdfConverter.ConvertFromPowerPoint(srcFilePath, destFilePath);
                    break;
                case OfficePathType.Word:
                    OfficeToPdfConverter.ConvertFromWord(srcFilePath, destFilePath);
                    break;
                case OfficePathType.Excel:
                    OfficeToPdfConverter.ConvertFromExcel(srcFilePath, destFilePath);
                    break;
                default:
                    break;
            }
        }

        private static OfficePathType GetPathType(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return OfficePathType.FileNotExist;
            }
            string fileExtension = System.IO.Path.GetExtension(filePath);
            if (".xls".Equals(fileExtension, StringComparison.OrdinalIgnoreCase) || ".xlsx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return OfficePathType.Excel;
            }
            if (".doc".Equals(fileExtension, StringComparison.OrdinalIgnoreCase) || ".docx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return OfficePathType.Word;
            }
            if (".ppt".Equals(fileExtension, StringComparison.OrdinalIgnoreCase) || ".pptx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return OfficePathType.PowerPoint;
            }
            return OfficePathType.InvalidFile;
        }

        public static bool IsValidOfficeFile(string filePath)
        {
            OfficePathType pathType = GetPathType(filePath);
            bool result = false;
            switch (pathType)
            {
                case OfficePathType.Excel:
                case OfficePathType.Word:
                case OfficePathType.PowerPoint:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        internal enum OfficePathType
        {
            FileNotExist,
            InvalidFile,
            Word,
            Excel,
            PowerPoint,
        }
    }
}

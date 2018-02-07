using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    public static class StructClone
    {
        public static void ClassClone<T>(T pDataSource, ref T pDataTarget) //where T : new()
        {
            if (pDataTarget == null) return;
            System.Reflection.PropertyInfo[] properties1 = pDataSource.GetType().GetProperties();
            System.Reflection.PropertyInfo[] properties2 = pDataTarget.GetType().GetProperties();
            for (int i = 0; i < properties1.Length; i++)
            {
                for (int j = 0; j < properties2.Length; j++)
                {
                    if (properties2[j].Name == properties1[i].Name && properties1[i].PropertyType == properties2[j].PropertyType)
                    {
                        object obj = properties1[i].GetValue(pDataSource, null);
                        properties2[j].SetValue(pDataTarget, obj, null);
                    }
                }
            }
        }
    }
}

using CO_IA.Data.PlanDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataManager.Public
{
    public class CodeDicModel
    {

        private string CodeDicItemListSql = @"select * FROM CodeDic   where  1=1  ";



        /// <summary>
        /// 获取基础表信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="code">code值</param>
        /// <returns>基础表信息</returns>
        public List<CodeDicItem> CodeDicItemList(string type, string code)
        {


            if (!string.IsNullOrEmpty(type))
            {
                CodeDicItemListSql += "and  type = '" + type + "'  ";
            }


            if (!string.IsNullOrEmpty(code))
            {
                CodeDicItemListSql += "and  code = '" + code + "'  ";
            }



            DataSet ds = DbHelperACE.Query(CodeDicItemListSql);

            List<CodeDicItem> list = new List<CodeDicItem>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {


                CodeDicItem item = new CodeDicItem();


                item.CODE = drs[i]["CODE"].ToString().Trim();
                item.NAME = drs[i]["NAME"].ToString().Trim();
                item.TYPE = drs[i]["TYPE"].ToString().Trim();
                item.DSC = drs[i]["DSC"].ToString().Trim();

                item.A1 = drs[i]["A1"].ToString().Trim();
                item.A2 = drs[i]["A2"].ToString().Trim();
                item.A3 = drs[i]["A3"].ToString().Trim();



                list.Add(item);
            }
            return list;
        }
    }
}

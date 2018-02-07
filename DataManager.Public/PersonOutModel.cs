using CO_IA.Data.PlanDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DataManager.Public
{
  public  class PersonOutModel
    {



        private string UpdatePersonOutInfoSql = @"Update PersonOutInfo SET   ";


        private string DeletePersonOutInfoSql = @"Delete FROM PersonOutInfo    Where 1=1  ";


        private string SelectPersonOutInfoListSql = @"select * FROM PersonOutInfo   where  1=1  ";

         
        /// <summary>
        /// 插入人员外出信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>

        public bool InsertPersoOutInfo(PersonOutInfo info)
        {
            bool isresult = false;
            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = @"Insert INTO PersonOutInfo (NAMEID,NAME,INCIDENT,OUTTIME,BACKTIME,BZ,OPERATOR,OPERATORTIME,OPERATORID,A1,A2,A3,OUTTIMEHOUR,OUTTIMEMINUTE,BACKTIMEHOUR,BACKTIMEMINUTE ) values ('" + info.NAMEID + "','" + info.NAME + "','" + info.INCIDENT + "','" + info.OUTTIME + "','" + info.BACKTIME + "','" + info.BZ + "','" + info.OPERATOR + "','" + info.OPERATORTIME + "','" + info.OPERATORID + "','"  + info.A1 + "','" + info.A2 + "','" + info.A3 + "','" + info.OUTTIMEHOUR + "','" + info.OUTTIMEMINUTE + "','" + info.BACKTIMEHOUR + "','" + info.BACKTIMEMINUTE + "' )";
                command.CommandType = CommandType.Text; 
                try
                {
                    connection.Open();
                    int rows = command.ExecuteNonQuery();
                    isresult = true;
                }
                catch (System.Data.OleDb.OleDbException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }

            return isresult;

        }

        /// <summary>
        /// 删除人员信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool DeletePersonOutInfo(string id)
        {


            DeletePersonOutInfoSql += " and  id = " + id + "";



            int i = DbHelperACE.ExecuteSql(DeletePersonOutInfoSql);


            return i > 0;
        }



        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ModifyPersonOutInfo(PersonOutInfo info)
        {
            bool isresult = false;
            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = UpdatePersonOutInfoSql + @"  NAMEID = '" + info.NAMEID + "'"
                     + @" , NAME = '" + info.NAME + "'"
                     + @" , INCIDENT = '" + info.INCIDENT + "'"
                     + @" , OUTTIME = '" + info.OUTTIME + "'"
                     + @" , BACKTIME = '" + info.BACKTIME + "'"
                     + @" , BZ = '" + info.BZ + "'"
                     + @" , OPERATOR = '" + info.OPERATOR + "'"
                     + @" , OPERATORTIME = '" + info.OPERATORTIME + "'"
                     + @" , OPERATORID = '" + info.OPERATORID + "'"

                     + @" , A1 = '" + info.A1 + "'"
                     + @" , A2 = '" + info.A2 + "'"
                     + @" , A3 = '" + info.A3 + "'"
                     + @" , OUTTIMEHOUR = '" + info.OUTTIMEHOUR + "'"
                     + @" , OUTTIMEMINUTE = '" + info.OUTTIMEMINUTE + "'"
                     + @" , BACKTIMEHOUR = '" + info.BACKTIMEHOUR + "'"
                     + @" , BACKTIMEMINUTE = '" + info.BACKTIMEMINUTE + "'"
                     + @" where   ID =" + info.ID + "";
                  


                command.CommandType = CommandType.Text;


                try
                {
                    connection.Open();
                    int rows = command.ExecuteNonQuery();
                    isresult = true;
                }
                catch (System.Data.OleDb.OleDbException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }

            return isresult;

        }



        /// <summary>
        /// 获取人员外出信息
        /// </summary>
        /// <returns>所有人员外出信息</returns>

        public List<PersonOutInfo> GetPersonOutInfos(string nameid, string incident, string fromdate, string todate)
        {

          
            if (!string.IsNullOrEmpty(incident))
            {
                SelectPersonOutInfoListSql += "and  INCIDENT = '" + incident + "'  ";
            }




            if (!string.IsNullOrEmpty(nameid))
            {
                SelectPersonOutInfoListSql += "and  NAMEID = '" + nameid + "'  ";
            }




            if (!string.IsNullOrEmpty(fromdate))
            {
                SelectPersonOutInfoListSql += "and   Format(OUTTIME,\"yyyy-mm-dd\") >= '" + fromdate + "'  ";
            }

            if (!string.IsNullOrEmpty(todate))
            {
                SelectPersonOutInfoListSql += "and   Format(BACKTIME,\"yyyy-mm-dd\") <= '" + todate + "'  ";
            }


            SelectPersonOutInfoListSql += " order by OPERATORTIME desc";


            DataSet ds = DbHelperACE.Query(SelectPersonOutInfoListSql);

            List<PersonOutInfo> list = new List<PersonOutInfo>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {


                PersonOutInfo item = new PersonOutInfo();


                item.ID = drs[i]["ID"].ToString().Trim();
                item.NAMEID = drs[i]["NAMEID"].ToString().Trim();
                item.NAME = drs[i]["NAME"].ToString().Trim();
                item.INCIDENT = drs[i]["INCIDENT"].ToString().Trim();


                item.OUTTIMEHOUR = drs[i]["OUTTIMEHOUR"].ToString().Trim();
                if (string.IsNullOrEmpty(drs[i]["OUTTIMEMINUTE"].ToString()))
                {
                    item.OUTTIMEMINUTE = "00";
                }
                else
                {
                    item.OUTTIMEMINUTE = drs[i]["OUTTIMEMINUTE"].ToString().Trim();
                }
              
                item.BACKTIMEHOUR = drs[i]["BACKTIMEHOUR"].ToString().Trim();
               

                if (string.IsNullOrEmpty(drs[i]["BACKTIMEMINUTE"].ToString()))
                {
                    item.BACKTIMEMINUTE = "00";
                }
                else
                {
                    item.BACKTIMEMINUTE = drs[i]["BACKTIMEMINUTE"].ToString().Trim();

                }

                try
                {
                    if (drs[i]["OUTTIME"] != null && drs[i]["OUTTIME"].ToString() != string.Empty)
                    {
                        item.OUTTIME = Convert.ToDateTime(drs[i]["OUTTIME"]).ToString("yyyy-MM-dd")+" " + item.OUTTIMEHOUR + "时" + item.OUTTIMEMINUTE + "分";
                    }
                }
                catch
                {
                    item.OUTTIME = drs[i]["OUTTIME"].ToString().Trim();
                }

             


                try
                {
                    if (drs[i]["BACKTIME"] != null && drs[i]["BACKTIME"].ToString() != string.Empty)
                    {
                        item.BACKTIME = Convert.ToDateTime(drs[i]["BACKTIME"]).ToString("yyyy-MM-dd") + " " + item.BACKTIMEHOUR + "时" + item.BACKTIMEMINUTE + "分";
                    }
                }
                catch
                {
                    item.BACKTIME = drs[i]["BACKTIME"].ToString().Trim();
                }



           
                item.BZ = drs[i]["BZ"].ToString().Trim();
                item.OPERATOR = drs[i]["OPERATOR"].ToString().Trim();

                if (drs[i]["OPERATORTIME"] != null && drs[i]["OPERATORTIME"].ToString() != string.Empty)
                {
                    item.OPERATORTIME = Convert.ToDateTime(drs[i]["OPERATORTIME"]);
                }


                try
                {
                    if (drs[i]["OPERATORTIME"] != null && drs[i]["OPERATORTIME"].ToString() != string.Empty)
                    {
                        item.OPERATORTIME = Convert.ToDateTime(drs[i]["OPERATORTIME"]).ToLocalTime();
                    }
                }
                catch
                {
                    //item.OPERATORTIME =null;
                }




                item.OPERATORID = drs[i]["OPERATORID"].ToString().Trim();


                item.A1 = drs[i]["A1"].ToString().Trim();
                item.A2 = drs[i]["A2"].ToString().Trim();
                item.A3 = drs[i]["A3"].ToString().Trim();

          

                list.Add(item);
            }
            return list;
        }



    }
}

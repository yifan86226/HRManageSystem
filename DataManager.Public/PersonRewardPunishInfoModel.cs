using CO_IA.Data.PlanDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DataManager.Public
{
  public   class PersonRewardPunishInfoModel
    {


        private string UpdatePersonRewardPunishInfoSql = @"Update PersonRewardPunishInfo SET   ";


        private string DeletePersonRewardPunishInfoSql = @"Delete FROM PersonRewardPunishInfo    Where 1=1  ";


        private string SelectPersonRewardPunishInfoListSql = @"select * FROM PersonRewardPunishInfo   where  1=1  ";


        /// <summary>
        /// 插入人员信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>

        public bool InsertPersonRewardPunishInfo(PersonRewardPunishInfo info)
        {
            bool isresult = false;
            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = @"Insert INTO PersonRewardPunishInfo (NAMEID,NAME,INCIDENT,FRACTION,RPTYPE,RPTIME,RPREPORTOR,OPERATOR,OPERATORTIME,OPERATORID,A1,A2,A3,BZ ) values ('" + info.NAMEID + "','" + info.NAME + "','" + info.INCIDENT + "','" + info.FRACTION + "','" + info.RPTYPE + "','" + info.RPTIME + "','" + info.RPREPORTOR + "','" + info.OPERATOR + "','" + info.OPERATORTIME + "','" + info.OPERATORID+ "','" + info.A1 + "','" + info.A2 + "','" + info.A3 + "','" + info.BZ+ "')";
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
        public bool DeletePersonRewardPunishInfo(string id)
        {


            DeletePersonRewardPunishInfoSql += " and  ID = " + id + "";



            int i = DbHelperACE.ExecuteSql(DeletePersonRewardPunishInfoSql);


            return i > 0;
        }



        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ModifyPersonRewardPunishInfo(PersonRewardPunishInfo info)
        {
            bool isresult = false;
            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = UpdatePersonRewardPunishInfoSql + @"  NAMEID = '" + info.NAMEID + "'"
                      + @" , NAME = '" + info.NAME + "'"
                      + @" , INCIDENT = '" + info.INCIDENT + "'"
                      + @" , FRACTION = '" + info.FRACTION + "'"
                      + @" , RPTYPE = '" + info.RPTYPE + "'"
                      + @" , RPTIME = '" + info.RPTIME + "'"
                      + @" , RPREPORTOR = '" + info.RPREPORTOR + "'"
                      + @" , OPERATOR = '" + info.OPERATOR + "'"
                      + @" , OPERATORTIME = '" + info.OPERATORTIME + "'"
                      + @" , OPERATORID = '" + info.OPERATORID + "'"
                      + @" , BZ = '" + info.BZ + "'"
                      + @" , A1 = '" + info.A1 + "'"
                      + @" , A2 = '" + info.A2 + "'"
                      + @" , A3 = '" + info.A3 + "'"
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
        /// 获取 所有人员
        /// </summary>
        /// <returns>所有人员信息</returns>

        public List<PersonRewardPunishInfo> GetPersonRewardPunishInfos(string type, string incident, string nameid, string fromdate, string todate ,string  orderstr)
        {

            //if (!string.IsNullOrEmpty(type))
            //{
            //    SelectPersonRewardPunishInfoListSql += "and  RPTYPE = '" + type + "'  ";
            //}

            //if (!string.IsNullOrEmpty(incident))
            //{
            //    SelectPersonRewardPunishInfoListSql += "and  INCIDENT = '" + incident + "'  ";
            //}




            //if (!string.IsNullOrEmpty(nameid))
            //{
            //    SelectPersonRewardPunishInfoListSql += "and  NAMEID = '" + nameid + "'  ";
            //}




            //if (!string.IsNullOrEmpty(fromdate))
            //{
            //    SelectPersonRewardPunishInfoListSql += "and   Format(RPTIME,\"yyyy-mm-dd\") >= '" + fromdate + "'  ";
            //}

            //if (!string.IsNullOrEmpty(todate))
            //{
            //    SelectPersonRewardPunishInfoListSql += "and   Format(RPTIME,\"yyyy-mm-dd\") <= '" + todate + "'  ";
            //}

            //if (!string.IsNullOrEmpty(orderstr))
            //{
            //    SelectPersonRewardPunishInfoListSql += orderstr ;
            //}
            //else
            //{

            //    SelectPersonRewardPunishInfoListSql += " order by RPTIME desc ,OPERATORTIME desc";

            //}



            //DataSet ds = DbHelperACE.Query(SelectPersonRewardPunishInfoListSql);


            DataTable dt = GetPersonRewardPunishDataTable(type, incident, nameid, fromdate, todate, orderstr);



            List <PersonRewardPunishInfo> list = new List<PersonRewardPunishInfo>();
            DataRowCollection drs = dt.Rows;
            for (int i = 0; i < drs.Count; i++)
            {


                PersonRewardPunishInfo item = new PersonRewardPunishInfo();


                item.ID = drs[i]["ID"].ToString().Trim();
                item.NAMEID = drs[i]["NAMEID"].ToString().Trim();
                item.NAME = drs[i]["NAME"].ToString().Trim();
                item.INCIDENT = drs[i]["INCIDENT"].ToString().Trim();
                try
                {
                    item.FRACTION = Convert.ToDouble(drs[i]["FRACTION"].ToString());
                }
                catch
                { }
                item.RPTYPE = drs[i]["RPTYPE"].ToString().Trim();


                try
                {
                    if (drs[i]["RPTIME"] != null && drs[i]["RPTIME"].ToString() != string.Empty)
                    {
                        item.RPTIME = Convert.ToDateTime(drs[i]["RPTIME"]).ToString("yyyy-MM-dd") ;
                    }
                }
                catch
                {
                    item.RPTIME = drs[i]["RPTIME"].ToString().Trim();
                }





                item.RPREPORTOR = drs[i]["RPREPORTOR"].ToString().Trim();
                item.OPERATOR = drs[i]["OPERATOR"].ToString().Trim();

                try
                {
                    if (drs[i]["OPERATORTIME"] != null && drs[i]["OPERATORTIME"].ToString() != string.Empty)
                    {
                        item.OPERATORTIME = Convert.ToDateTime(drs[i]["OPERATORTIME"]);
                    }

                }
                catch { }
                item.OPERATORID = drs[i]["OPERATORID"].ToString().Trim();
                

                item.A1 = drs[i]["A1"].ToString().Trim();
                item.A2 = drs[i]["A2"].ToString().Trim();
                item.A3 = drs[i]["A3"].ToString().Trim();
                item.BZ = drs[i]["BZ"].ToString().Trim();

 

                list.Add(item);
            }
            return list;
        }



        public DataTable GetPersonRewardPunishDataTable(string type, string incident, string nameid, string fromdate, string todate, string orderstr)
        {

            if (!string.IsNullOrEmpty(type))
            {
                SelectPersonRewardPunishInfoListSql += "and  RPTYPE = '" + type + "'  ";
            }

            if (!string.IsNullOrEmpty(incident))
            {
                SelectPersonRewardPunishInfoListSql += "and  INCIDENT = '" + incident + "'  ";
            }




            if (!string.IsNullOrEmpty(nameid))
            {
                SelectPersonRewardPunishInfoListSql += "and  NAMEID = '" + nameid + "'  ";
            }




            if (!string.IsNullOrEmpty(fromdate))
            {
                SelectPersonRewardPunishInfoListSql += "and   Format(RPTIME,\"yyyy-mm-dd\") >= '" + fromdate + "'  ";
            }

            if (!string.IsNullOrEmpty(todate))
            {
                SelectPersonRewardPunishInfoListSql += "and   Format(RPTIME,\"yyyy-mm-dd\") <= '" + todate + "'  ";
            }

            if (!string.IsNullOrEmpty(orderstr))
            {
                SelectPersonRewardPunishInfoListSql += orderstr;
            }
            else
            {

                SelectPersonRewardPunishInfoListSql += " order by RPTIME desc ,OPERATORTIME desc";

            }



            DataSet ds = DbHelperACE.Query(SelectPersonRewardPunishInfoListSql);
            return ds.Tables[0];
           
        }





    }
}

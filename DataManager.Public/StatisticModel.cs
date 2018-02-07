using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DataManager.Public
{
    public class StatisticModel
    {


        string Getpersonstat2017 = "SELECT name, type, count(leaveday) as statisticcount   FROM LeaveDay2017  group by name, type  order by name, type";


   
        string GetPersonRPStatByDateSql = "SELECT nameid, name, INCIDENT as type, sum(FRACTION) as statisticcount FROM  PersonRewardPunishInfo where 1=1 ";

        string GetPersonOutStatByDateSql = "SELECT nameid, name, INCIDENT as type, count(INCIDENT) as statisticcount FROM  PersonOutInfo where 1=1";



        string GetPersonRPStatByDateHJSql = @"SELECT '合计' as name, type, sum(sumcount) as statisticcount
  FROM(

        SELECT nameid, name, INCIDENT as type, sum(FRACTION) as sumcount
           FROM PersonRewardPunishInfo
 where  1=1
 and Format(RPTIME,'yyyy-mm-dd')  >= @fromDate
and Format(RPTIME,'yyyy-mm-dd')  <= @toDate
          group by nameid, name, INCIDENT
         order by name, INCIDENT)

 group by name, type
 order by name, type";



        string GetPersonOutStatByDateHJSql = @"SELECT '合计' as name, type, sum(sumcount) as statisticcount
  FROM(

        SELECT nameid, name, INCIDENT as type, count(INCIDENT) as sumcount
           FROM PersonOutInfo
 where  1=1
 and Format(OUTTIME,'yyyy-mm-dd')  >= @fromDate
and Format(OUTTIME,'yyyy-mm-dd')  <= @toDate
          group by nameid, name, INCIDENT
         order by name, INCIDENT)

 group by name, type
 order by name, type";



        string GetPersonRPScoreByDateSql = @"SELECT name, sum(FRACTION) as score
from PersonRewardPunishInfo

where  1=1
 and Format(RPTIME,'yyyy-mm-dd')  >= @fromDate
and Format(RPTIME,'yyyy-mm-dd')  <= @toDate

group by name 
order by sum(FRACTION)  desc
";




        string GetpersonstatHJ2017 = @"
SELECT '合计' as name, type, sum(sumcount) as statisticcount
  FROM (
        
        SELECT name, type, count(leaveday) as sumcount
          FROM LeaveDay2017
         group by name, type
         order by name, type)
 group by name, type
 order by name, type";



        string GetpersonLeaveInfos2017 = "SELECT *  FROM LeaveDay2017  where 1=1 ";



        /// <summary>
        /// 返回人员统计列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<PersonPlanStatisticData> GetPersonRPStatByDate(string fromDate, string  toDate)
        {


            //            SELECT nameid, name, INCIDENT as type, sum(FRACTION) as statisticcount FROM PersonRewardPunishInfo

            //where Format(RPTIME,"yyyy-mm-dd")  <= "2012/10/12"


            //group by NAMEID, NAME, INCIDENT order by NAME, INCIDENT

            if (string.IsNullOrEmpty(fromDate) == false)
            {
                GetPersonRPStatByDateSql += " and  Format(RPTIME,\"yyyy-mm-dd\")  >= \'" + fromDate +"'";
            }

            if (string.IsNullOrEmpty(toDate) == false)
            {
                GetPersonRPStatByDateSql += " and  Format(RPTIME,\"yyyy-mm-dd\")  <= \'" + toDate + "'";

            }

            GetPersonRPStatByDateSql += " group by NAMEID, NAME, INCIDENT order by NAME, INCIDENT ";


            DataSet ds = DbHelperACE.Query(GetPersonRPStatByDateSql);

            List<PersonPlanStatisticData> PersonPlanList = new List<PersonPlanStatisticData>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {
                PersonPlanStatisticData item = new PersonPlanStatisticData();
                item.AddressGuid = drs[i]["nameid"].ToString().Trim();
                item.Address = drs[i]["name"].ToString().Trim();
                item.Type = drs[i]["type"].ToString().Trim();
                if (drs[i]["statisticcount"] != DBNull.Value)
                {
                    item.Count = Double.Parse(drs[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }



//            and Format(RPTIME,'yyyy - mm - dd')  >= @fromDate
//and Format(RPTIME,'yyyy - mm - dd')  <= @toDate
            List<OleDbParameter> paramList = new List<OleDbParameter>();
            OleDbParameter param = new OleDbParameter("@fromDate", OleDbType.LongVarWChar);
            param.Value = fromDate;

            paramList.Add(param);


            OleDbParameter toDateparam = new OleDbParameter("@toDate", OleDbType.LongVarWChar);
            toDateparam.Value = toDate;

            paramList.Add(toDateparam);

            DataSet dshj = DbHelperACE.Query(GetPersonRPStatByDateHJSql, paramList.ToArray());
            DataRowCollection drshj = dshj.Tables[0].Rows;
            for (int i = 0; i < drshj.Count; i++)
            {
                PersonPlanStatisticData item = new PersonPlanStatisticData();
                item.AddressGuid = drshj[i]["name"].ToString().Trim();
                item.Address = drshj[i]["name"].ToString().Trim();
                item.Type = drshj[i]["type"].ToString().Trim();
                if (drshj[i]["statisticcount"] != DBNull.Value)
                {
                    item.Count = Double.Parse(drshj[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }

            return PersonPlanList;
        }


        /// <summary>
        /// 返回人员统计列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<NameValuePair<double>> GetPersonRPScoreByDate(string fromDate, string toDate)
        {


            List<NameValuePair<double>> PersonPlanList = new List<NameValuePair<double>>();
            List<OleDbParameter> paramList = new List<OleDbParameter>();
            OleDbParameter param = new OleDbParameter("@fromDate", OleDbType.LongVarWChar);
            param.Value = fromDate;

            paramList.Add(param);


            OleDbParameter toDateparam = new OleDbParameter("@toDate", OleDbType.LongVarWChar);
            toDateparam.Value = toDate;

            paramList.Add(toDateparam);

            DataSet dshj = DbHelperACE.Query(GetPersonRPScoreByDateSql, paramList.ToArray());
            DataRowCollection drshj = dshj.Tables[0].Rows;
            for (int i = 0; i < drshj.Count; i++)
            {
                NameValuePair<double> item = new NameValuePair<double>();
                item.Name = drshj[i]["name"].ToString().Trim();


                if (drshj[i]["score"] != DBNull.Value)
                {
                    try
                    {

                        item.Value = Convert.ToDouble(drshj[i]["score"].ToString().Trim());
                    }
                    catch
                    { }
                }

                PersonPlanList.Add(item);
            }

            return PersonPlanList;
        }




        /// <summary>
        /// 返回人员统计列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<PersonPlanStatisticData> GetPersonOutStatsByDate(string fromDate, string toDate)
        {

            //            SELECT nameid, name, INCIDENT as type, sum(FRACTION) as statisticcount FROM PersonRewardPunishInfo

            //where Format(RPTIME,"yyyy-mm-dd")  <= "2012/10/12"


            //group by NAMEID, NAME, INCIDENT order by NAME, INCIDENT

            if (string.IsNullOrEmpty(fromDate) == false)
            {
                GetPersonOutStatByDateSql += " and  Format(OUTTIME,\"yyyy-mm-dd\")  >= \'" + fromDate + "'";
            }

            if (string.IsNullOrEmpty(toDate) == false)
            {
                GetPersonOutStatByDateSql += " and  Format(OUTTIME,\"yyyy-mm-dd\")  <= \'" + toDate + "'";

            }

            GetPersonOutStatByDateSql += " group by NAMEID, NAME, INCIDENT order by NAME, INCIDENT ";


            DataSet ds = DbHelperACE.Query(GetPersonOutStatByDateSql);

            List<PersonPlanStatisticData> PersonPlanList = new List<PersonPlanStatisticData>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {
                PersonPlanStatisticData item = new PersonPlanStatisticData();
                item.AddressGuid = drs[i]["nameid"].ToString().Trim();
                item.Address = drs[i]["name"].ToString().Trim();
                item.Type = drs[i]["type"].ToString().Trim();
                if (drs[i]["statisticcount"] != DBNull.Value)
                {
                    item.Count = Double.Parse(drs[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }



            //            and Format(RPTIME,'yyyy - mm - dd')  >= @fromDate
            //and Format(RPTIME,'yyyy - mm - dd')  <= @toDate
            List<OleDbParameter> paramList = new List<OleDbParameter>();
            OleDbParameter param = new OleDbParameter("@fromDate", OleDbType.LongVarWChar);
            param.Value = fromDate;

            paramList.Add(param);


            OleDbParameter toDateparam = new OleDbParameter("@toDate", OleDbType.LongVarWChar);
            toDateparam.Value = toDate;

            paramList.Add(toDateparam);

            DataSet dshj = DbHelperACE.Query(GetPersonOutStatByDateHJSql, paramList.ToArray());
            DataRowCollection drshj = dshj.Tables[0].Rows;
            for (int i = 0; i < drshj.Count; i++)
            {
                PersonPlanStatisticData item = new PersonPlanStatisticData();
                item.AddressGuid = drshj[i]["name"].ToString().Trim();
                item.Address = drshj[i]["name"].ToString().Trim();
                item.Type = drshj[i]["type"].ToString().Trim();
                if (drshj[i]["statisticcount"] != DBNull.Value)
                {
                    item.Count = Double.Parse(drshj[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }

            return PersonPlanList;
        }





        #region  2017人员统计相关

        /// <summary>
        /// 返回人员统计列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<PersonPlanStatisticData>  GetPersonPlanStats2017(string obj)
        {

            DataSet ds = DbHelperACE.Query(Getpersonstat2017);

            List<PersonPlanStatisticData> PersonPlanList = new List<PersonPlanStatisticData>() ;
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {
                PersonPlanStatisticData  item = new PersonPlanStatisticData();
                item.AddressGuid = drs[i]["name"].ToString().Trim();
                item.Address = drs[i]["name"].ToString().Trim();
                item.Type = drs[i]["type"].ToString().Trim();
                if (drs[i]["statisticcount"] != DBNull.Value)
                { 
                    item.Count = Double.Parse(drs[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }


            DataSet dshj = DbHelperACE.Query(GetpersonstatHJ2017);
            DataRowCollection drshj = dshj.Tables[0].Rows;
            for (int i = 0; i < drshj.Count; i++)
            {
                PersonPlanStatisticData item = new PersonPlanStatisticData();
                item.AddressGuid = drshj[i]["name"].ToString().Trim();
                item.Address = drshj[i]["name"].ToString().Trim();
                item.Type = drshj[i]["type"].ToString().Trim();
                if (drshj[i]["statisticcount"] != DBNull.Value)
                {
                    item.Count = Double.Parse(drshj[i]["statisticcount"].ToString().Trim());
                }
                PersonPlanList.Add(item);
            }

            return PersonPlanList;
        }



        /// <summary>
        /// 获取活动下所有人员
        /// </summary>
        /// <param name="orgGuid">组织ID</param>
        /// <returns>所有人员信息</returns>

        public List<PP_PersonLeaveInfo> GetPP_PersonInfos(string name, string type)
        {

            if (!string.IsNullOrEmpty(name))
            {
                GetpersonLeaveInfos2017 += "and  name = '" + name + "'  ";
            }


            if (!string.IsNullOrEmpty(type))
            {
                GetpersonLeaveInfos2017 += "and  type = '" + type + "'  ";
            }



            DataSet ds = DbHelperACE.Query(GetpersonLeaveInfos2017);

            List<PP_PersonLeaveInfo> list = new List<PP_PersonLeaveInfo>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {
                //没有请假日期不算
                if (string.IsNullOrEmpty(drs[i]["LeaveDay"].ToString()))
                {
                    continue;
                }
                PP_PersonLeaveInfo item = new PP_PersonLeaveInfo();
                //list[i]. = drs[i]["name"].ToString().Trim();
                item.NAME = drs[i]["name"].ToString().Trim();

                
                item.LEAVE_DAY = drs[i]["LeaveDay"].ToString().Trim();
                item.LEAVE_TYPE = drs[i]["type"].ToString().Trim();

                list.Add(item);
            }
            return list;
        }

        #endregion
    }
}

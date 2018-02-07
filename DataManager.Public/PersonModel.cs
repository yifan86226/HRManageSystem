using CO_IA.Data.PlanDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataManager.Public
{
 public   class PersonModel
    { 

        private string UpdatePersonInfoSql = @"Update PersonBasicInfo SET   ";


        private string DeletePersonInfoSql = @"Delete FROM PersonBasicInfo    Where 1=1  ";
 

        private string SelectPersonInfoListSql = @"select * FROM PersonBasicInfo   where  1=1  ";






        /// <summary>
        /// 插入人员信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>

        public bool InsertPersonBasicInfo(PersonBasicInfo info)
        {
            bool isresult = false;

            if (info.PHOTO != null && info.PHOTO.Length > 0)
            {
     
                info.PHOTO = ImageHelper.YaSuo(info.PHOTO);

                ////保存图片
                //string filename = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + info.NAMEID;
                //string fileFullname = ImageHelper.CreateImageFromBytes(filename, info.PHOTO);

                //string md5Code = ImageHelper.GetMD5HashFromByte(info.PHOTO);

                //info.A1 = md5Code;

                //info.A2 = fileFullname.Replace(AppDomain.CurrentDomain.BaseDirectory,"");

            }



            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = @"Insert INTO PersonBasicInfo (NAMEID,NAME,SEX,BIRTHDATE,NATION,ENLISTMENTDATE,MILITARYRANK,ORIGINPLACE,ARMYSEAT,MAJOR,EDUCATION,POLITICAL,PARTYTIME,HJQK,BLOODTYPE,IDCARD,HOBBY,CHARACTERTYPE,QQID,HOMEADDRESS,PHONE,SPOUSENAME,SPOUSEMARRIAGETIME,SPOUSESUNIT,SPOUSESHOMEADDRESS,SPOUSESPHONE,CHILDRENNAME,CHILDRENSEX,CHILDRENBIRTH,ENLISTINGRESUME,TRAININGSITUATION,REWARDSPUNISHMENTS,FAMILYMEMBER,A1,A2,A3 ,PHOTO) values ('" + info.NAMEID + "','" + info.NAME + "','" + info.SEX + "','" + info.BIRTHDATE + "','" + info.NATION + "','" + info.ENLISTMENTDATE + "','" + info.MILITARYRANK + "','" + info.ORIGINPLACE + "','" + info.ARMYSEAT + "','" + info.MAJOR + "','" + info.EDUCATION + "','" + info.POLITICAL + "','" + info.PARTYTIME + "','" + info.HJQK + "','" + info.BLOODTYPE + "','" + info.IDCARD + "','" + info.HOBBY + "','" + info.CHARACTERTYPE + "','" + info.QQID + "','" + info.HOMEADDRESS + "','" + info.PHONE + "','" + info.SPOUSENAME + "','" + info.SPOUSEMARRIAGETIME + "','" + info.SPOUSESUNIT + "','" + info.SPOUSESHOMEADDRESS + "','" + info.SPOUSESPHONE + "','" + info.CHILDRENNAME + "','" + info.CHILDRENSEX + "','" + info.CHILDRENBIRTH + "','" + info.ENLISTINGRESUME + "','" + info.TRAININGSITUATION + "','" + info.REWARDSPUNISHMENTS + "','" + info.FAMILYMEMBER + "','" + info.A1 + "','" + info.A2 + "','" + info.A3 + "' , @photo1)";
                command.CommandType = CommandType.Text;
                OleDbParameter imageType = new OleDbParameter("@photo1", OleDbType.VarBinary);
                imageType.Value = info.PHOTO;
                command.Parameters.Add(imageType).Value = info.PHOTO;



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
        public bool DeletePersonInfo(string guid)
        {


            DeletePersonInfoSql += " and  NAMEID = '" + guid + "'";



            int i = DbHelperACE.ExecuteSql(DeletePersonInfoSql);


            return i > 0;
        }



        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ModifyPersonBasicInfo(PersonBasicInfo info)
        {
            bool isresult = false;


            if (info.PHOTO != null && info.PHOTO.Length > 0)
            {



                //if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + info.A2) == false)
                //{


                //    info.PHOTO = ImageHelper.YaSuo(info.PHOTO);

                //    //保存图片
                //    string filename = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + info.NAMEID;
                //    string fileFullname = ImageHelper.CreateImageFromBytes(filename, info.PHOTO);

                //    string md5Code = ImageHelper.GetMD5HashFromByte(info.PHOTO);

                //    info.A1 = md5Code;

                //    info.A2 = fileFullname.Replace(AppDomain.CurrentDomain.BaseDirectory, "");



                //}
                //else
                //{

                //    string md5file = DataManager.Public.ImageHelper.GetMD5HashFromFile(AppDomain.CurrentDomain.BaseDirectory + info.A2);

                //    string md5PhotoCode = ImageHelper.GetMD5HashFromByte(info.PHOTO);




                //    if (md5file.Equals(md5PhotoCode) == false)
                //    {
                   
                        info.PHOTO = ImageHelper.YaSuo(info.PHOTO);

                //        //保存图片
                //        string filename = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + info.NAMEID;
                //        string fileFullname = ImageHelper.CreateImageFromBytes(filename, info.PHOTO);

                //        string md5Code = ImageHelper.GetMD5HashFromByte(info.PHOTO);

                //        info.A1 = md5Code;

                //        info.A2 = fileFullname.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
                //    }
                //}
            }




            using (OleDbConnection connection = new OleDbConnection(DbHelperACE.connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = UpdatePersonInfoSql + @"  NAMEID = '" + info.NAMEID + "'"
                     + @" , NAME = '" + info.NAME + "'"
                     + @" ,  SEX = '" + info.SEX + "'"
                     + @" ,  BIRTHDATE = '" + info.BIRTHDATE + "'"
                     + @" ,  NATION = '" + info.NATION + "'"
                     + @" ,  ENLISTMENTDATE = '" + info.ENLISTMENTDATE + "'"
                     + @" ,  MILITARYRANK = '" + info.MILITARYRANK + "'"
                     + @" ,  ORIGINPLACE = '" + info.ORIGINPLACE + "'"
                     + @" ,  ARMYSEAT = '" + info.ARMYSEAT + "'"
                     + @" ,  MAJOR = '" + info.MAJOR + "'"
                     + @" ,  EDUCATION = '" + info.EDUCATION + "'"
                     + @" ,  POLITICAL = '" + info.POLITICAL + "'"
                     + @" ,  PARTYTIME = '" + info.PARTYTIME + "'"
                     + @" ,  BLOODTYPE = '" + info.BLOODTYPE + "'"
                     + @" ,  HJQK = '" + info.HJQK + "'"
                     + @" ,  IDCARD = '" + info.IDCARD + "'"
                     + @" ,  HOBBY = '" + info.HOBBY + "'"
                     + @" ,  CHARACTERTYPE = '" + info.CHARACTERTYPE + "'"
                     + @" ,  QQID = '" + info.QQID + "'"
                     + @" ,  HOMEADDRESS = '" + info.HOMEADDRESS + "'"
                     + @" ,  PHONE = '" + info.PHONE + "'"
                     + @" ,  SPOUSENAME = '" + info.SPOUSENAME + "'"
                     + @" ,  SPOUSEMARRIAGETIME = '" + info.SPOUSEMARRIAGETIME + "'"
                     + @" ,  SPOUSESUNIT = '" + info.SPOUSESUNIT + "'"
                     + @" ,  SPOUSESHOMEADDRESS = '" + info.SPOUSESHOMEADDRESS + "'"
                     + @" ,  SPOUSESPHONE = '" + info.SPOUSESPHONE + "'"
                     + @" ,  CHILDRENNAME = '" + info.CHILDRENNAME + "'"
                     + @" ,  CHILDRENSEX = '" + info.CHILDRENSEX + "'"
                     + @" ,  CHILDRENBIRTH = '" + info.CHILDRENBIRTH + "'"
                     + @" ,  ENLISTINGRESUME = '" + info.ENLISTINGRESUME + "'"
                     + @" ,  TRAININGSITUATION = '" + info.TRAININGSITUATION + "'"
                     + @" ,  REWARDSPUNISHMENTS = '" + info.REWARDSPUNISHMENTS + "'"


                     + @" ,  FAMILYMEMBER = '" + info.FAMILYMEMBER + "'"
                     + @" ,  A1 = '" + info.A1 + "'"
                     + @" ,  A2 = '" + info.A2 + "'"
                     + @" ,  A3 = '" + info.A3 + "'"
                     + @" ,  PHOTO = @photo1"  
                     + @" where    NAMEID = '" + info.NAMEID + "'";

                command.CommandType = CommandType.Text;
                OleDbParameter imageType = new OleDbParameter("@photo1", OleDbType.VarBinary);
                imageType.Value = info.PHOTO;
                command.Parameters.Add(imageType).Value = info.PHOTO;



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
        /// <param name="militaryrank">军衔</param>
        /// <param name="nameid">名称id</param>
        /// <param name="orderstr">排序</param>
        /// <returns>所有人员信息</returns>
        public List<PersonBasicInfo> GetPersonBasicInfos(string militaryrank, string nameid, string orderstr, bool isShowPic = true)
        {

            if (!string.IsNullOrEmpty(militaryrank))
            {
                SelectPersonInfoListSql += "and  MILITARYRANK = '" + militaryrank + "'  ";
            }


            if (!string.IsNullOrEmpty(nameid))
            {
                SelectPersonInfoListSql += "and  NAMEID = '" + nameid + "'  ";
            }



            if (!string.IsNullOrEmpty(orderstr))
            {
                SelectPersonInfoListSql += orderstr;
            }
            else
            {
                SelectPersonInfoListSql += " order by MILITARYRANK,POLITICAL";
            }


            DataSet ds = DbHelperACE.Query(SelectPersonInfoListSql);

            List<PersonBasicInfo> list = new List<PersonBasicInfo>();
            DataRowCollection drs = ds.Tables[0].Rows;
            for (int i = 0; i < drs.Count; i++)
            {


                PersonBasicInfo item = new PersonBasicInfo();


                item.NAMEID = drs[i]["NAMEID"].ToString().Trim();
                item.NAME = drs[i]["NAME"].ToString().Trim();
                item.SEX = drs[i]["SEX"].ToString().Trim();
                item.BIRTHDATE = drs[i]["BIRTHDATE"].ToString().Trim();
                item.NATION = drs[i]["NATION"].ToString().Trim();
                item.ENLISTMENTDATE = drs[i]["ENLISTMENTDATE"].ToString().Trim();
                item.MILITARYRANK = drs[i]["MILITARYRANK"].ToString().Trim();
                item.ORIGINPLACE = drs[i]["ORIGINPLACE"].ToString().Trim();
                item.ARMYSEAT = drs[i]["ARMYSEAT"].ToString().Trim();
                item.MAJOR = drs[i]["MAJOR"].ToString().Trim();
                item.EDUCATION = drs[i]["EDUCATION"].ToString().Trim();
                item.POLITICAL = drs[i]["POLITICAL"].ToString().Trim();
                item.PARTYTIME = drs[i]["PARTYTIME"].ToString().Trim();
                item.HJQK = drs[i]["HJQK"].ToString().Trim();
                item.BLOODTYPE = drs[i]["BLOODTYPE"].ToString().Trim();
                item.IDCARD = drs[i]["IDCARD"].ToString().Trim();
                item.HOBBY = drs[i]["HOBBY"].ToString().Trim();
                item.CHARACTERTYPE = drs[i]["CHARACTERTYPE"].ToString().Trim();
                item.QQID = drs[i]["QQID"].ToString().Trim();
                item.HOMEADDRESS = drs[i]["HOMEADDRESS"].ToString().Trim();
                item.PHONE = drs[i]["PHONE"].ToString().Trim();
                item.SPOUSENAME = drs[i]["SPOUSENAME"].ToString().Trim();
                item.SPOUSEMARRIAGETIME = drs[i]["SPOUSEMARRIAGETIME"].ToString().Trim();
                item.SPOUSESUNIT = drs[i]["SPOUSESUNIT"].ToString().Trim();
                item.SPOUSESHOMEADDRESS = drs[i]["SPOUSESHOMEADDRESS"].ToString().Trim();
                item.SPOUSESPHONE = drs[i]["SPOUSESPHONE"].ToString().Trim();
                item.CHILDRENNAME = drs[i]["CHILDRENNAME"].ToString().Trim();
                item.CHILDRENSEX = drs[i]["CHILDRENSEX"].ToString().Trim();
                item.CHILDRENBIRTH = drs[i]["CHILDRENBIRTH"].ToString().Trim();
                item.ENLISTINGRESUME = drs[i]["ENLISTINGRESUME"].ToString().Trim();
                item.TRAININGSITUATION = drs[i]["TRAININGSITUATION"].ToString().Trim();
                item.REWARDSPUNISHMENTS = drs[i]["REWARDSPUNISHMENTS"].ToString().Trim();
                item.FAMILYMEMBER = drs[i]["FAMILYMEMBER"].ToString().Trim();
                item.A1 = drs[i]["A1"].ToString().Trim();
                item.A2 = drs[i]["A2"].ToString().Trim();
                item.A3 = drs[i]["A3"].ToString().Trim();


                if (isShowPic == true)
                {
                    if (drs[i]["PHOTO"] != null && !drs[i]["PHOTO"].ToString().Equals(""))
                    {

                        item.PHOTO = (byte[])drs[i]["PHOTO"];
                    }

                }
                list.Add(item);
            }
            return list;
        }
    }
}

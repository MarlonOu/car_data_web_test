using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using car_test.Models;
using System;

namespace car_test.Models
{
    public class databaseProcess
    {
        public static MySqlConnection conn = new MySqlConnection();

        public static void Connect(string databaseName)
        {
            string connString = "server=120.117.72.61;port=3306;user id=J1939;password='P502_J1939_User';database=" + databaseName + ";charset=utf8;";
            conn.ConnectionString = connString;
            if (conn.State != ConnectionState.Open)
                conn.Open();
        }

        public static void Disconnect()
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }
        
        public List<carRegistrationInformation> getSdCardNum()
        {
            try
            {
                string databaseName = "J1939_Car_Informations";
                Connect(databaseName);
                List<carRegistrationInformation> CarDataList = new List<carRegistrationInformation>();
                string sql = @"SELECT `primary_value`, `SD_card_number` FROM `j1939_car_registration_information`";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        carRegistrationInformation data = new carRegistrationInformation();
                        data.primaryValue_SdCardNum = dr["primary_value"].ToString();
                        data.SdCardNum = dr["SD_card_number"].ToString();
                        CarDataList.Add(data);
                    }
                }
                return CarDataList;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public List<carRegistrationInformation> getCarDriverList(string carSDCardNumSelected)
        {
            try
            {
                string databaseName = "J1939_Car_Informations";
                Connect(databaseName);
                string sql = " SELECT `driver_name` FROM `j1939_car_driver_registration_information` WHERE `SD_card_number` = '" + carSDCardNumSelected + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                List<carRegistrationInformation> carDriverList = new List<carRegistrationInformation>();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        carRegistrationInformation data = new carRegistrationInformation();
                        data.carDriver = dr["driver_name"].ToString();
                        //data.DateTime = dr["Village"].ToString();
                        carDriverList.Add(data);
                    }
                }
                Disconnect();
                
                return carDriverList;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public List<carStartDateTime> getCarStartDateTime(string carSDCardNumSelected, string carDriverSelectd)
        {
            try
            {
                string databaseName = "J1939_Car_Event_Car_Start";
                Connect(databaseName);
                string sql = " SELECT `j1939_car_data_compare_table_name`, `wrong_data` FROM `" + carSDCardNumSelected + "` WHERE j1939_car_driver = '" + carDriverSelectd + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                List<carStartDateTime> carStartDateTimeList = new List<carStartDateTime>();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        carStartDateTime data = new carStartDateTime();
                        data.compareTableName = dr["j1939_car_data_compare_table_name"].ToString();
                        data.wrongData = bool.Parse(dr["wrong_data"].ToString());
                        //data.DateTime = dr["Village"].ToString();
                        if (!data.wrongData)
                            carStartDateTimeList.Add(data);
                    }
                }
                Disconnect();

                databaseName = "J1939_Car_Data";
                Connect(databaseName);

                for (int i = 0; i < carStartDateTimeList.Count; i++)
                {
                    sql = " SELECT `Date / Time` FROM `" + carStartDateTimeList[i].compareTableName + "` WHERE `primary_value` = 1";
                    cmd = new MySqlCommand(sql, conn);
                    
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string[] DateTimeSplitList = dr["Date / Time"].ToString().Split('.');

                            carStartDateTimeList[i].carStartTime = DateTimeSplitList[0];
                        }
                    }
                }
                return carStartDateTimeList;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public List<carStartDateTime> getCarStartupTime(string carSDCardNumSelected, string carDriverSelectd)
        {
            try
            {
                string databaseName = "J1939_Car_Event_Car_Start";
                Connect(databaseName);
                string sql = " SELECT `j1939_car_data_compare_table_name`, `wrong_data` FROM `" + carSDCardNumSelected + "` WHERE j1939_car_driver = '" + carDriverSelectd + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                List<carStartDateTime> carStartDateTimeList = new List<carStartDateTime>();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        carStartDateTime data = new carStartDateTime();
                        data.compareTableName = dr["j1939_car_data_compare_table_name"].ToString();
                        data.wrongData = bool.Parse(dr["wrong_data"].ToString());
                        //data.DateTime = dr["Village"].ToString();
                        if (!data.wrongData)
                            carStartDateTimeList.Add(data);
                    }
                }
                Disconnect();

                databaseName = "J1939_Car_Data";
                Connect(databaseName);

                for (int i = 0; i < carStartDateTimeList.Count; i++)
                {
                    sql = "SELECT `Date / Time` FROM `" + carStartDateTimeList[i].compareTableName + "` WHERE `primary_value` = 1 OR `primary_value` = (SELECT max(`primary_value`) FROM `" + carStartDateTimeList[i].compareTableName + "`)";
                    cmd = new MySqlCommand(sql, conn);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        List<string> DateTimeSplitList = new List<string>();
                        while (dr.Read())
                        {
                            string[] data = dr["Date / Time"].ToString().Split('.');
                            DateTimeSplitList.Add(data[0]);
                        }

                        carStartDateTimeList[i].carStartTime = DateTimeSplitList[0];
                        carStartDateTimeList[i].carEndTime = DateTimeSplitList[1];
                    }
                }
                return carStartDateTimeList;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public List<carData> getCarDataList(string carStartupSelected, string[] carDataTypeAllSelected, int startPrimaryValue = 0, int endPrimaryValue = 0)
        {
            try
            {
                string databaseName = "J1939_Car_Data";
                Connect(databaseName);
                List<carData> carDataList = new List<carData>();


                string sql = @"SELECT `Date / Time`, ";
                for (int i=0; i < carDataTypeAllSelected.Length; i++)
                {
                    if (carDataTypeAllSelected[i] == "primaryValue")
                        sql = sql + "`primary_value`";
                    else if (carDataTypeAllSelected[i] == "SPD")
                        sql = sql + "`SPD`";
                    else if (carDataTypeAllSelected[i] == "engineSpeed")
                        sql = sql + "`Engine Speed`";
                    else if (carDataTypeAllSelected[i] == "instantFuel")
                        sql = sql + "`Instant Fuel`";
                    else if (carDataTypeAllSelected[i] == "averageFuel")
                        sql = sql + "`Average Fuel`";
                    else if (carDataTypeAllSelected[i] == "ODO")
                        sql = sql + "`ODO`";
                    else if (carDataTypeAllSelected[i] == "idleHours")
                        sql = sql + "`Idle Hours`";
                    else if (carDataTypeAllSelected[i] == "idleFuel")
                        sql = sql + "`Idle Fuel`";
                    if (i != carDataTypeAllSelected.Length - 1)
                        sql = sql + ", ";
                }
                if (startPrimaryValue == 0 || startPrimaryValue == 0)
                    sql = sql + " FROM `" + carStartupSelected + "` WHERE 1";
                else
                    //SELECT `Date / Time`, `Instant Fuel`, `ODO` FROM `346102085_2022-09-01 00:00:08` WHERE `primary_value` > 134788 and `primary_value` < 161054;;
                    sql = sql + " FROM `" + carStartupSelected + "` WHERE `primary_value` > " + startPrimaryValue + " and `primary_value` <= " + endPrimaryValue;
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        carData data = new carData();
                        data.DateTime = dr["Date / Time"].ToString();
                        for (int i=0; i < carDataTypeAllSelected.Length; i++)
                        {
                            switch (carDataTypeAllSelected[i])//switch (比對的運算式)
                            {
                                case "primaryValue":
                                    data.primaryValue = int.Parse(dr["primary_value"].ToString());
                                    break;

                                case "SPD":
                                    data.SPD = float.Parse(dr["SPD"].ToString());
                                    break;

                                case "engineSpeed":
                                    data.engineSpeed = float.Parse(dr["Engine Speed"].ToString());
                                    break;

                                case "instantFuel":
                                    data.instantFuel = float.Parse(dr["Instant Fuel"].ToString());
                                    break;

                                case "averageFuel":
                                    data.averageFuel = float.Parse(dr["Average Fuel"].ToString());
                                    break;

                                case "ODO":
                                    data.ODO = float.Parse(dr["ODO"].ToString());                                             
                                    break;

                                case "idleHours":
                                    data.idleHours = float.Parse(dr["Idle Hours"].ToString());
                                    break;

                                case "idleFuel":
                                    data.idleFuel = float.Parse(dr["Idle Fuel"].ToString());
                                    break;

                                default:
                                    break;
                            }
                        }                        
                        //data.APP1 = float.Parse(dr["APP1"].ToString());
                        //data.BPP = float.Parse(dr["BPP"].ToString());
                        //data.Torque = dr["Torque"].ToString();
                        //data.brakeState = dr["Brake State"].ToString();
                        //data.AP1_LIS = dr["AP1 LIS"].ToString();
                        //data.CANDateTime = dr["CAN Date / Time"].ToString();
                        carDataList.Add(data);
                    }
                }
                return carDataList;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        /*Hommy:Login*/
        public static string getDBToken(string id)
        {
            try
            {
                string databaseName = "J1939_Car_User";
                Connect(databaseName);
                List<carRegistrationInformation> CarDataList = new List<carRegistrationInformation>();
                string sql = @"SELECT `password` FROM `j1939_car_user_registration_information` where account = '" + id + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        return dr["password"].ToString();
                    }
                }
                return "none";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
            finally
            {
                Disconnect();
            }
        }
    }
}

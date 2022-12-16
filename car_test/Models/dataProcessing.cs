using car_test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;
using car_test.Models;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Runtime.ConstrainedExecution;

namespace car_test.Functions
{
    public class dataProcessing
    {
        public static List<carData> getDataByNoRepeating(List<carData> carDataList)
        {
            List<carData> NewcarDataList = new List<carData>();
            carData TempcarData = carDataList[0];

            foreach (carData carData in carDataList)
            {
                if (carData == TempcarData) NewcarDataList.Add(carData); ;//第一筆不執行
                if (((carData.engineSpeed - TempcarData.engineSpeed) / ((carData.engineSpeed + TempcarData.engineSpeed) / 2)) > 0.06 ||
                    carData.SPD != TempcarData.SPD ||
                    carData.APP1 != TempcarData.APP1 ||
                    carData.BPP != TempcarData.BPP ||
                    carData.Torque != TempcarData.Torque ||
                    carData.instantFuel != TempcarData.instantFuel ||
                    carData.averageFuel != TempcarData.averageFuel ||
                    carData.ODO != TempcarData.ODO ||
                    carData.idleHours != TempcarData.idleHours ||
                    carData.idleFuel != TempcarData.idleFuel ||
                    carData.AP1_LIS != TempcarData.AP1_LIS)//先不考慮擴充性
                {
                    NewcarDataList.Add(carData);
                }
                TempcarData = carData;
            }
            return NewcarDataList;
        }
        public static List<carData> getDataByAveragePerSecond(List<carData> carDataList)
        {
            List<carData> carDataBySecList = new List<carData>();
            int primaryValue;
            string time;
            string timeNext;
            string[] timeSplit;
            float SPDSum;
            float engineSpeedSum;
            float instantFuelSum;
            float averageFuelSum;
            float ODOSum;
            float idleHoursSum;
            float idleFuelSum;
            int j, count;

            foreach (carData carData in carDataList)
            {
                timeSplit = carData.DateTime.Split(".");
                carData.DateTime = timeSplit[0];
            }

            for (int i = 0; i < carDataList.Count;)
            {
                carData carDataBySec = new carData();
                time = carDataList[i].DateTime;
                SPDSum = carDataList[i].SPD;
                engineSpeedSum = carDataList[i].engineSpeed;
                instantFuelSum = carDataList[i].instantFuel;
                averageFuelSum = carDataList[i].averageFuel;
                ODOSum = carDataList[i].ODO;
                idleHoursSum = carDataList[i].idleHours;
                idleFuelSum = carDataList[i].idleFuel;
                count = 1;
                for (j = i + 1; j < carDataList.Count; j++)
                {
                    timeNext = carDataList[j].DateTime;
                    if (time == timeNext)
                    {
                        SPDSum += carDataList[i].SPD;
                        engineSpeedSum += carDataList[i].engineSpeed;
                        instantFuelSum += carDataList[i].instantFuel;
                        averageFuelSum += carDataList[i].averageFuel;
                        ODOSum += carDataList[i].ODO;
                        idleHoursSum += carDataList[i].idleHours;
                        idleFuelSum += carDataList[i].idleFuel;
                        count++;   
                    }
                    else
                        break;
                }
                carDataBySec.DateTime = time;
                carDataBySec.SPD = (float)Math.Round(SPDSum / count, 2);
                carDataBySec.engineSpeed = (float)Math.Round(engineSpeedSum / count, 3);
                carDataBySec.instantFuel = (float)Math.Round(instantFuelSum / count, 3);
                carDataBySec.averageFuel = (float)Math.Round(averageFuelSum / count, 4);
                carDataBySec.ODO = (float)Math.Round(ODOSum / count, 2);
                carDataBySec.idleHours = (float)Math.Round(idleHoursSum / count, 1);
                carDataBySec.idleFuel = (float)Math.Round(idleFuelSum / count, 1);
                carDataBySecList.Add(carDataBySec);
                i = j;
            }

            return carDataBySecList;
        }
        public static List<carData> getValueOfMaxAndMin(List<carData> carDataList, string[] carDataTypeSelected)
        {
            carData carDataListMax = new carData();
            carData carDataListMin = new carData();
            List<float> dataList = new List<float>();

            foreach (string carDataType in carDataTypeSelected)
            {
                if (carDataType == "SPD")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.SPD);
                    }
                    carDataListMax.SPD = (float)Math.Round((dataList.Max() * 1.1));
                    carDataListMin.SPD = -10;
                }
                else if (carDataType == "engineSpeed")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.engineSpeed);
                    }
                    carDataListMax.engineSpeed = (float)Math.Round((dataList.Max() * 1.1));
                    carDataListMin.engineSpeed = -10;
                }
                else if (carDataType == "instantFuel")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.instantFuel);
                    }
                    carDataListMax.instantFuel = (float)Math.Round((dataList.Max() * 1.1));
                    carDataListMin.instantFuel = -10;
                }
                else if (carDataType == "averageFuel")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.averageFuel);
                    }
                    carDataListMax.averageFuel = (float)Math.Round((dataList.Max() * 1.0005), 2);
                    carDataListMin.averageFuel = (float)Math.Round((dataList.Min() / 1.0005), 2);
                }
                else if (carDataType == "ODO")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.ODO);
                    }
                    carDataListMax.ODO = (float)Math.Round((dataList.Max() * 1.00005));
                    carDataListMin.ODO = (float)Math.Round((dataList.Min() / 1.00005));
                }
                else if (carDataType == "idleHours")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.idleHours);
                    }
                    carDataListMax.idleHours = (float)Math.Round((dataList.Max() * 1.0005));
                    carDataListMin.idleHours = (float)Math.Round((dataList.Min() / 1.0005));
                }
                else if (carDataType == "idleFuel")
                {
                    dataList.Clear();
                    foreach (carData carData in carDataList)
                    {
                        dataList.Add(carData.idleFuel);
                    }
                    carDataListMax.idleFuel = (float)Math.Round((dataList.Max() * 1.0005));
                    carDataListMin.idleFuel = (float)Math.Round((dataList.Min() / 1.0005));
                }
            }
            carDataList.Add(carDataListMax);
            carDataList.Add(carDataListMin);
            return carDataList;
        }
        public static List<carData> getCarDataBySec(List<carData> carDataList, string dataParameterRequire = "")
        {
            List<carData> carDataBySecList = new List<carData>();
            int primaryValue;
            string time;
            string timeNext;
            string[] timeSplit;
            float instantFuelSum;
            int j, count;

            foreach (carData carData in carDataList)
            {
                timeSplit = carData.DateTime.Split(".");
                carData.DateTime = timeSplit[0];
            }

            for (int i = 0; i < carDataList.Count;)
            {
                carData carDataBySec = new carData();
                time = carDataList[i].DateTime;
                instantFuelSum = carDataList[i].instantFuel;
                count = 1;
                for (j = i + 1; j < carDataList.Count;)
                {
                    timeNext = carDataList[j].DateTime;
                    if (time == timeNext)
                    {
                        if (dataParameterRequire == "CO2")
                        {
                            instantFuelSum += carDataList[j].instantFuel;
                            count++;
                        }
                        j++;
                    }
                    else
                        break;
                }
                carDataBySec.DateTime = time;
                carDataBySec.instantFuel = instantFuelSum / count;
                if (dataParameterRequire == "")
                {
                    primaryValue = carDataList[i].primaryValue;
                    carDataBySec.primaryValue = primaryValue;
                }
                carDataBySecList.Add(carDataBySec);
                i = j;
            }
            if (dataParameterRequire == "CO2")
            {
                carDataBySecList[0].ODO = carDataList[0].ODO;
                carDataBySecList[carDataBySecList.Count - 1].ODO = carDataList[carDataList.Count - 1].ODO;
            }

            return carDataBySecList;
        }
        public static carCalculatedData computeCO2ByCarStartup(List<carData> carDataBySecList)
        {
            carCalculatedData carCalculatedData = new carCalculatedData();
            float instantFuelSum = 0;

            for (int i = 0; i < carDataBySecList.Count; i++)
            {
                instantFuelSum += carDataBySecList[i].instantFuel;
            }
            //平均油耗 instantFuelAverage km/L 
            float instantFuelAverage = instantFuelSum / carDataBySecList.Count;
            //Mileage during driving (最後一筆ODO - 第一筆ODO)   mileage km
            float mileage = carDataBySecList[carDataBySecList.Count - 1].ODO - carDataBySecList[0].ODO;
            //耗油量 mileage-instantFuelAverage = FuelConsumption L
            float FuelConsumption = mileage / instantFuelAverage;
            //消耗柴油重量 FuelConsumption * 柴油密度0.85 =  dieselWeight kg
            float dieselWeight = (float)(FuelConsumption * 0.85);
            //CO2 dieselWeight * 3.16 kg/kg fuel = co2 kg
            float co2 = (float)Math.Round((dieselWeight * 3.16), 2);

            carCalculatedData.CO2 = co2;
            carCalculatedData.mileage = (float)Math.Round(mileage, 2);

            return carCalculatedData;
        }
    }
}
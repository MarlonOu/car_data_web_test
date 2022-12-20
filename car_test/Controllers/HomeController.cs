using car_test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;
using car_test.Functions;
using Newtonsoft.Json.Linq;

namespace car_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            databaseProcess db = new databaseProcess();
            List<carRegistrationInformation> carSdCardNumList = db.getSdCardNum();

            ViewBag.carSdCardNumList = carSdCardNumList;
            return View();
        }

        public IActionResult getCarDriverList(string carSDCardNumSelected)
        {
            databaseProcess db = new databaseProcess();
            List<carRegistrationInformation> carDriverList = db.getCarDriverList(carSDCardNumSelected);
            string result = "";
            if (carDriverList == null)
            {
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                result = JsonConvert.SerializeObject(carDriverList);
                return Json(result);
            }
        }

        public IActionResult carStartDateTime(string carSDCardNumSelected, string carDriverSelectd)
        {
            databaseProcess db = new databaseProcess();
            List<carStartDateTime> carStartDateTimeList = db.getCarStartDateTime(carSDCardNumSelected, carDriverSelectd);
            string result = "";
            if (carStartDateTimeList == null)
            {
                ViewBag.zz = carStartDateTimeList;
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                result = JsonConvert.SerializeObject(carStartDateTimeList);
                return Json(result);
            }
        }

        public IActionResult carStartupTime(string carSDCardNumSelected, string carDriverSelectd)
        {
            databaseProcess db = new databaseProcess();
            List<carStartDateTime> carStartDateTimeList = db.getCarStartupTime(carSDCardNumSelected, carDriverSelectd);
            string result = "";
            if (carStartDateTimeList == null)
            {
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                result = JsonConvert.SerializeObject(carStartDateTimeList);
                return Json(result);
            }
        }

        public IActionResult getCarDataJson(string carStartupSelected, string[] carDataTypeAllSelected)
        {
            databaseProcess db = new databaseProcess(); 
            List<carData> carDataList = db.getCarDataList(carStartupSelected, carDataTypeAllSelected);

            string result = "";
            if (carDataList == null)
            {
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                //carDataList = dataProcessing.getDataByNoRepeating(carDataList);
                carDataList = dataProcessing.getDataByAveragePerSecond(carDataList);
                carDataList = dataProcessing.getValueOfMaxAndMin(carDataList, carDataTypeAllSelected);

                result = JsonConvert.SerializeObject(carDataList);

                //string to class property.
                //System.Reflection.PropertyInfo prop = typeof(carData).GetProperty("SPD");
                //object value = prop.GetValue(carDataList);
                return Json(result);
            }
        }

        public IActionResult getCarStartupAllTime(string carStartupSelected)
        {
            databaseProcess db = new databaseProcess();
            string[] carDataTypeAllSelected = new string[] { "primaryValue"};
            //DateTime start = DateTime.Now;
            List<carData> carDataList = db.getCarDataList(carStartupSelected, carDataTypeAllSelected);
            //DateTime end = DateTime.Now;
            //DateTime start1 = DateTime.Now;
            List<carData> carDataBySecList = dataProcessing.getCarDataBySec(carDataList);
            //DateTime end1 = DateTime.Now;



            string result = "";
            if (carDataBySecList == null)
            {
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                result = JsonConvert.SerializeObject(carDataBySecList);
                return Json(result);
            }
        }

        public IActionResult getCO2ByCarStartup(string carStartupSelected, int startPrimaryValue, int endPrimaryValue)
        {
            databaseProcess db = new databaseProcess();
            string[] carDataTypeAllSelected = new string[] {"instantFuel", "ODO"};
            List<carData> carDataList = db.getCarDataList(carStartupSelected, carDataTypeAllSelected, startPrimaryValue, endPrimaryValue);
            List<carData> carDataBySecList = dataProcessing.getCarDataBySec(carDataList, dataParameterRequire: "CO2");
            carCalculatedData carCalculatedData = dataProcessing.computeCO2ByCarStartup(carDataBySecList);



            string result = "";
            if (carCalculatedData == null)
            {
                //讀取資料庫錯誤
                return Json(result);
            }
            else
            {
                result = JsonConvert.SerializeObject(carCalculatedData);
                return Json(result);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
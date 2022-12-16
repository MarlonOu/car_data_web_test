using Microsoft.AspNetCore.Mvc;

namespace car_test.Models
{
    public class carStartDateTime
    {
        public string compareTableName { get; set; }
        public string carStartTime { get; set; }
        public string carEndTime { get; set; }
        public bool wrongData { get; set; }

    }
}

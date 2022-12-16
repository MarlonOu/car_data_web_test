using Microsoft.AspNetCore.Mvc;

namespace car_test.Models
{
    public class carData
    {
        public int primaryValue { get; set; }
        public string DateTime { get; set; }
        public float SPD { get; set; }
        public float engineSpeed { get; set; }
        public float APP1 { get; set; }
        public float BPP { get; set; }
        public string Torque { get; set; }
        public float instantFuel { get; set; }
        public float averageFuel { get; set; }
        public float ODO { get; set; }
        public float idleHours { get; set; }
        public float idleFuel { get; set; }
        public string brakeState { get; set; }
        public string AP1_LIS { get; set; }
        public string CANDateTime { get; set; }
    }
}

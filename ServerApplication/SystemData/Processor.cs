using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ServerApplication.SystemData
{
    public class Processor
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public string Manufacturer { get; set; }
        public string CurrentClockSpeed { get; set; }
        public string Caption { get; set; }
        public string NumberOfCores { get; set; }
        public string NumberOfEnabledCores { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string Architecture { get; set; }
        public string Family { get; set; }
        public string ProcessorType { get; set; }
        public string Characteristics { get; set; }
        public string AddressWidth  { get; set; }
    }
}

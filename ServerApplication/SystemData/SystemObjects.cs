using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.SystemData
{
    public class SystemObjects
    {
        public HardDisk Hdd { get; set; }
        public Processor Processor { get; set; }
        public Ram Ram { get; set; }
        public OperatingSystem OperatingSystem { get; set; }
    }
}

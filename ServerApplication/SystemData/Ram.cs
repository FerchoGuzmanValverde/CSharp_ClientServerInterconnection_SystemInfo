using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.SystemData
{
    public class Ram
    {
        public string TotalVisibleMemory { get; set; }
        public string TotalPhysicalMemory { get; set; }
        public string TotalVirtualMemory { get; set; }
        public string FreeVirtualMemory { get; set; }
    }
}

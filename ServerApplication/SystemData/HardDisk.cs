using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.SystemData
{
    public class HardDisk
    {
        public string DriveName { get; set; }
        public string DriveType { get; set; }
        public string VolumeLabel { get; set; }
        public string FileSystem { get; set; }
        public string AvailableSpaceCurrentUser { get; set; }
        public string TotalAvailableSpace { get; set; }
        public string TotalSizeDrive { get; set; }
        public string RootDirectory { get; set; }
    }
}

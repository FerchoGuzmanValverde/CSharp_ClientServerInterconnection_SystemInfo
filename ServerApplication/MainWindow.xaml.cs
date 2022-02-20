using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Windows.Threading;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint localEndPoint;

        SystemData.Processor processorObj;
        SystemData.HardDisk hardDiskObj;
        SystemData.Ram ramObj;
        SystemData.OperatingSystem osObj;

        Socket clientSocket;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void GetProcessorInfo()
        {
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                processorObj = new SystemData.Processor();

                processorObj.Name = (string)obj["Name"];
                processorObj.DeviceID = (string)obj["DeviceID"];
                processorObj.Manufacturer = (string)obj["Manufacturer"];
                processorObj.CurrentClockSpeed = obj["CurrentClockSpeed"].ToString();
                processorObj.Caption = (string)obj["Caption"];
                processorObj.NumberOfCores = obj["NumberOfCores"].ToString();
                processorObj.NumberOfEnabledCores = obj["NumberOfEnabledCore"].ToString();
                processorObj.NumberOfLogicalProcessors = obj["NumberOfLogicalProcessors"].ToString();
                processorObj.Architecture = obj["Architecture"].ToString();
                processorObj.Family = obj["Family"].ToString();
                processorObj.ProcessorType = obj["ProcessorType"].ToString();
                processorObj.Characteristics = obj["Characteristics"].ToString();
                processorObj.AddressWidth = obj["AddressWidth"].ToString();
            }
        }

        public void GetHDDInfor()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            hardDiskObj = new SystemData.HardDisk();

            hardDiskObj.DriveName = allDrives[0].Name.ToString();
            hardDiskObj.DriveType = allDrives[0].DriveType.ToString();
            hardDiskObj.VolumeLabel = allDrives[0].VolumeLabel;
            hardDiskObj.FileSystem = allDrives[0].DriveFormat;
            hardDiskObj.AvailableSpaceCurrentUser = allDrives[0].AvailableFreeSpace.ToString();
            hardDiskObj.TotalAvailableSpace = allDrives[0].TotalFreeSpace.ToString();
            hardDiskObj.TotalSizeDrive = allDrives[0].TotalSize.ToString();
            hardDiskObj.RootDirectory = allDrives[0].RootDirectory.ToString();

            /*foreach (DriveInfo d in allDrives)
            {
                hardDiskObj.DriveName = d.Name.ToString();
                hardDiskObj.DriveType = d.DriveType.ToString();
                hardDiskObj.VolumeLabel = d.VolumeLabel;
                hardDiskObj.FileSystem = d.DriveFormat;
                hardDiskObj.AvailableSpaceCurrentUser = d.AvailableFreeSpace.ToString();
                hardDiskObj.TotalAvailableSpace = d.TotalFreeSpace.ToString();
                hardDiskObj.TotalSizeDrive = d.TotalSize.ToString();
                hardDiskObj.RootDirectory = d.RootDirectory.ToString();
            }*/
        }

        public void GetRamInfo()
        {
            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                ramObj = new SystemData.Ram();

                ramObj.TotalVisibleMemory = result["TotalVisibleMemorySize"].ToString();
                ramObj.TotalPhysicalMemory = result["FreePhysicalMemory"].ToString();
                ramObj.TotalVirtualMemory = result["TotalVirtualMemorySize"].ToString();
                ramObj.FreeVirtualMemory = result["FreeVirtualMemory"].ToString();
            }
        }

        public void GetOperatingSystemInfo()
        {
            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                osObj = new SystemData.OperatingSystem();

                osObj.Caption = (string)obj["Caption"];
                osObj.WindowsDirectory = obj["WindowsDirectory"].ToString();
                osObj.ProductType = obj["ProductType"].ToString();
                osObj.SerialNumber = obj["SerialNumber"].ToString();
                osObj.SystemDirectory = (string)obj["SystemDirectory"];
                osObj.CountryCode = (string)obj["CountryCode"];
                osObj.CurrentTimeZone = obj["CurrentTimeZone"].ToString();
                osObj.EncryptionLevel = obj["EncryptionLevel"].ToString();
                osObj.OSType = obj["OSType"].ToString();
                osObj.Version = (string)obj["Version"];
            }
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            host = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = host.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddr, 11111);
            byte timesListen = 0;
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    lblStatus.Content = "";
                    lblStatus.Content = "Waiting connection... ";
                    clientSocket = listener.Accept();
                    lblStatus.Content = "A device has connected!!";

                    // Data buffer
                    byte[] bytes = new byte[1024];
                    string data = null;

                    while (timesListen < 10)
                    {
                        while (true)
                        {
                            int numByte = clientSocket.Receive(bytes);

                            data += Encoding.ASCII.GetString(bytes, 0, numByte);

                            if (data.IndexOf("<EOF>") > -1)
                                break;
                        }

                        //Extracting Info and sending
                        GetProcessorInfo();
                        GetRamInfo();
                        GetHDDInfor();
                        GetOperatingSystemInfo();

                        SystemData.SystemObjects dataObj = new SystemData.SystemObjects();
                        dataObj.Processor = processorObj;
                        dataObj.Ram = ramObj;
                        dataObj.Hdd = hardDiskObj;
                        dataObj.OperatingSystem = osObj;

                        string jsonData = JsonSerializer.Serialize(dataObj);
                        byte[] message = Encoding.ASCII.GetBytes(jsonData);

                        // Send a message to Client
                        // using Send() method
                        clientSocket.Send(message);
                        timesListen += 1;
                    }

                    

                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
            

        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            // Close client Socket using the
            // Close() method. After closing,
            // we can use the closed Socket
            // for a new Client Connection
            //clientSocket.Shutdown(SocketShutdown.Both);
            //clientSocket.Close();
        }
    }
}

using System;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.Json;
using System.IO;
using ServerApplication.SystemData;
using System.Collections.Generic;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPHostEntry ipHost;
        IPAddress ipAddr;
        IPEndPoint localEndPoint;
        Socket send;

        SystemObjects systemObj;
        List<Processor> listProcessors = new List<Processor>();
        List<HardDisk> listHardDisk = new List<HardDisk>();
        List<Ram> listRam = new List<Ram>();
        List<ServerApplication.SystemData.OperatingSystem> listOS = new List<ServerApplication.SystemData.OperatingSystem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ipHost = Dns.GetHostEntry(Dns.GetHostName());
                ipAddr = ipHost.AddressList[0];
                localEndPoint = new IPEndPoint(ipAddr, 11111);

                send = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                send.Connect(localEndPoint);

                lblMesagge.Content = "Socket connected to -> " + send.RemoteEndPoint.ToString();
            }

            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void ShowData()
        {
            dg_Processor.ItemsSource = "";
            dg_hdd.ItemsSource = "";
            dg_ram.ItemsSource = "";
            dg_OS.ItemsSource = "";
            listProcessors.Add(systemObj.Processor);
            listHardDisk.Add(systemObj.Hdd);
            listOS.Add(systemObj.OperatingSystem);
            listRam.Add(systemObj.Ram);
            dg_Processor.ItemsSource = listProcessors;
            dg_hdd.ItemsSource = listHardDisk;
            dg_OS.ItemsSource = listOS;
            dg_ram.ItemsSource = listRam;
        }

        private void btnStartRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte timeRequest = 0;
                while(timeRequest < 10)
                {
                    // Creation of message that
                    // we will send to Server
                    byte[] messageSent = Encoding.ASCII.GetBytes("Request<EOF>");
                    int byteSent = send.Send(messageSent);

                    // Data buffer
                    byte[] messageReceived = new byte[2048];
                    int byteRecv = send.Receive(messageReceived);

                    //string fileMsg = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                    string jsonString = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                    //string jsonString = File.ReadAllText(fileMsg);
                    systemObj = new SystemObjects();
                    systemObj = JsonSerializer.Deserialize<SystemObjects>(Encoding.ASCII.GetString(messageReceived, 0, byteRecv))!;


                    ShowData();
                    timeRequest += 1;
                    Thread.Sleep(5000);
                    //Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));
                    //send.Shutdown(SocketShutdown.Both);
                    //send.Close();
                }
                // Close Socket using
                // the method Close()
                send.Shutdown(SocketShutdown.Both);
                send.Close();
            }

            // Manage of Socket's Exceptions
            catch (ArgumentNullException ane) { MessageBox.Show(ane.Message); }

            catch (SocketException se) { MessageBox.Show(se.Message); }

            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}

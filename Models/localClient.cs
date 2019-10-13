using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Exersice3.Models
{
    public class localClient
    {
        public int indexer = 0;
        public List<CalculatePos> fileByLines = new List<CalculatePos>();
        public object lockAction = new object();
        static object lockMethod = new object();
        static object lockMethod2 = new object();
        public event PropertyChangedEventHandler propChanged;
        public string FileToRead;
        public string FileToWrite;
        public static Socket mySocket;
        public static  string ip { get; set; } = "127.0.0.1";
        public static  int port { get; set; } = 5400;
        private static localClient s_instace = null;
        public bool changeIndicator { get; set; } = false;
        public static localClient Instance
        {
            get
            {
                if (s_instace == null)
                {
                    s_instace = new localClient();
                    // open stream socket with tcp protocol on the same computer.
                     mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // connect to local host port number 5400
                    while (true)
                    {
                        try
                        {
                            mySocket.Connect(new IPEndPoint((IPAddress.Parse(ip)), port));
                            break;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return s_instace;
            }
        }
        // request function. longitude and latitude.
        public double[] Request(string ip, int port)
        {
            lock(lockMethod){
                // send get request specifeid by the value path.
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /position/longitude-deg\r\n"));
                byte[] messege = new byte[512];
                Array.Clear(messege, 0, 512);
                string longi = System.Text.Encoding.ASCII.GetString(messege, 0, mySocket.Receive(messege));
                double longii = Double.Parse(((Regex.Match(longi, @"'(.*?[^\\])'")).Value).Trim('\''));
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /position/latitude-deg\r\n"));
                byte[] messege2 = new byte[512];
                Array.Clear(messege2, 0, 512);
                string lati = System.Text.Encoding.ASCII.GetString(messege2, 0, mySocket.Receive(messege2));
                double latii = Double.Parse(((Regex.Match(lati, @"'(.*?[^\\])'")).Value).Trim('\''));
                changeIndicator = !changeIndicator;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(longii + "," + latii));
                double[] vals = new double[2];
                vals[0] = longii;
                vals[1] = latii;
                return vals;
            }
            
        }
        // request function. altimeter, speed, heading, longitude and latitude.
        public string RequestAdditional()
        {
            lock (lockMethod2)
            {
                // send get request specifeid by the value path.
                byte[] messege = new byte[512];
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /position/longitude-deg\r\n"));
                Array.Clear(messege, 0, 512);
                string longi = System.Text.Encoding.ASCII.GetString(messege, 0, mySocket.Receive(messege));
                double longii = Double.Parse(((Regex.Match(longi, @"'(.*?[^\\])'")).Value).Trim('\''));
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /position/latitude-deg\r\n"));
                Array.Clear(messege, 0, 512);
                string lati = System.Text.Encoding.ASCII.GetString(messege, 0, mySocket.Receive(messege));
                double latii = Double.Parse(((Regex.Match(lati, @"'(.*?[^\\])'")).Value).Trim('\''));
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /controls/engines/current-engine/throttle\r\n"));
                Array.Clear(messege, 0, 512);
                string throttle = System.Text.Encoding.ASCII.GetString(messege, 0, mySocket.Receive(messege));
                double throttllee = Double.Parse(((Regex.Match(throttle, @"'(.*?[^\\])'")).Value).Trim('\''));
                mySocket.Send(System.Text.Encoding.ASCII.GetBytes("get /controls/flight/rudder\r\n"));
                Array.Clear(messege, 0, 512);
                string rudder = System.Text.Encoding.ASCII.GetString(messege, 0, mySocket.Receive(messege));
                double rudderr = Double.Parse(((Regex.Match(rudder, @"'(.*?[^\\])'")).Value).Trim('\''));
                changeIndicator = !changeIndicator;
                string args = (throttllee.ToString()) + "," + (rudderr.ToString()) + "," + (longii.ToString()) + "," + (latii.ToString());
                return args;
               
            }
        }
    }
}
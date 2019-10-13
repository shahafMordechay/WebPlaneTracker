using System;
using System.IO;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Exersice3.Models;
using System.Runtime.Serialization.Formatters.Binary;

namespace Exersice3.Controllers
{
    public class HomeController : Controller
    {
        private double lon;
        private double lat;
        private bool readerAlreadyWorking = false;
        private bool firstRead = false;

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult animation(string fileName, int frequancy)
        {
            // clean;
            localClient.Instance.indexer = 0;
            localClient.Instance.fileByLines.Clear();
            localClient.Instance.FileToRead = fileName;
            // make sure start from the begginig.
            string path = @"~/App_Data/";
            path += localClient.Instance.FileToRead;
            FileStream fsin = new FileStream(Server.MapPath(path), FileMode.Open, FileAccess.Read, FileShare.None);
            fsin.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(fsin);
            while (!sr.EndOfStream)
            {
                string [] val = (sr.ReadLine()).Split(',');
                if (val.Length == 4)
                {
                    CalculatePos pos = new CalculatePos(Double.Parse(val[2]), Double.Parse(val[3]));
                    localClient.Instance.fileByLines.Add(pos);
                }
            }
            fsin.Close();
            ViewBag.interval = (1000 / frequancy);
            return View("animation");
        }
        [HttpGet]
        public ActionResult display(string ip, int port)
        {
            // check if animation URL or display.
            if(ip.CompareTo(localClient.ip) != 0)
            {
                return animation(ip,port);
            }
            if (!readerAlreadyWorking)
            {
                // run this function when parameters updated.
                localClient.Instance.propChanged += Handler;
            }
            localClient.ip = ip;
            localClient.port = port;
            localClient.Instance.Request(ip, port);
            readerAlreadyWorking = true;
            // wait until first update.(just for the first time.)
            while (!firstRead) ;
            return View();
        }
        [HttpGet]
        public ActionResult Refresh(string ip, int port, int timeSlice)
        {
            localClient.ip = ip;
            localClient.port = port;
            ViewBag.interval = (1000 * timeSlice);
            if (!readerAlreadyWorking)
            {
                // run this function when parameters updated.
                localClient.Instance.propChanged += Handler;
     
            }
            localClient.Instance.Request(ip, port);
            readerAlreadyWorking = true;
            // wait until first update.(just for the first time.)
            while (!firstRead) ;
            return View();
        }

        [HttpGet]
        public ActionResult save(string ip, int port,int timeSlice,int timePeriod, string file)
        {
            ViewBag.period = timePeriod * 1000;
            ViewBag.interval = timeSlice / 1000;
            ViewBag.calls = (timePeriod * timeSlice);
            localClient.Instance.FileToWrite = file;
            if (!readerAlreadyWorking)
            {
                // run this function when parameters updated.
                localClient.Instance.propChanged += Handler;
            }
            localClient.Instance.Request(ip, port);
            readerAlreadyWorking = true;
            // wait until first update.(just for the first time.)
            while (!firstRead) ;
            return View();
        }


        // run this function when property changed.
        public void Handler(object sender, PropertyChangedEventArgs args)
        {
            string[] vals = (args.PropertyName).Split(',');
            // update view bag.
            ViewBag.longitude = Double.Parse(vals[0]);
            ViewBag.latitude = Double.Parse(vals[1]);
            firstRead = true;
        }

        [HttpPost]
        public string Position()
        {
            double[] vals = localClient.Instance.Request("127.0.0.1", 5400);
            CalculatePos position = new CalculatePos(vals[0],vals[1]);
            var json = new JavaScriptSerializer().Serialize(position);
            return json;

        }

        [HttpPost]
        public string WriteData(string fileName)
        {
            // get data.
            string data = localClient.Instance.RequestAdditional();
            string[] vals = data.Split(',');
            CalculatePos position = new CalculatePos(Double.Parse(vals[0]), Double.Parse(vals[1]));
            data += "\r\n";
            byte[] info = System.Text.Encoding.ASCII.GetBytes(data);
            //write data
            BinaryFormatter bf = new BinaryFormatter();
            string path = @"~/App_Data/";
            path += localClient.Instance.FileToWrite;
            FileStream fsout = new FileStream(Server.MapPath(path), FileMode.Append, FileAccess.Write, FileShare.None);
            bf.Serialize(fsout, info);
            fsout.Close();
            var json = new JavaScriptSerializer().Serialize(position);
            return json;
        }

        [HttpPost]
        public string ReadLine(string fileName)
        {
            try
            {
                CalculatePos position = localClient.Instance.fileByLines[localClient.Instance.indexer];
                localClient.Instance.indexer += 1;
                var json = new JavaScriptSerializer().Serialize(position);
                return json;
            }
            catch (Exception)
            {
                return "";
            }
           
            
        }

    }
}
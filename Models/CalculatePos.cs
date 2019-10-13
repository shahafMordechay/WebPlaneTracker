using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exersice3.Models
{
    public class CalculatePos
    {
        // getters and setters.
        public double longi {get; set;}
        public double lati { get; set;}
        // constructor.
        public CalculatePos(double longitude, double latitude)
        {
            longi = longitude;
            lati = latitude;
        }
    }
}
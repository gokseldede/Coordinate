using System;
using System.Data;

namespace GeoLocation
{
    public class Coordinate
    {
       public int Id {get;set;}
       public string Latitude {get;set;}
       public string Longitude {get;set;}     
    }
    public class Location
    {
       public int Id {get;set;}
       public string Address {get;set;} 
    }
}

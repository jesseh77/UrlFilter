using System;

namespace DemoApi.data
{
    public class HockeyStat
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string ShootCatch { get; set; }
        public string Pos { get; set; }
        public DateTime Birthday { get; set; }
        public string BirthCountry { get; set; }
        public int Year { get; set; }
        public int Stint { get; set; }
        public string Team { get; set; }
        public string League { get; set; }
        public int? GP { get; set; }
        public int? G { get; set; }
        public int? A { get; set; }
        public int? Pts { get; set; }
        public int? Pim { get; set; }
        public int? PM { get; set; }
        public int? PPG { get; set; }
        public int? PPA { get; set; }
        public int? SHG { get; set; }
        public int? SHA { get; set; }
        public int? GWG { get; set; }
        public int? GTG { get; set; }
        public int? SOG { get; set; }
    }
}
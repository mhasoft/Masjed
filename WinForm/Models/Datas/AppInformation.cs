using System;
using System.Collections.Generic;
using System.Text;

namespace WinForm.Models.Datas
{
    public static class AppInformation
    {
        public static int ProgramID { get; set; } = 1;
        public static string Owner { get; set; } = "none";
        public static double Latitude { get; set; } = 37.2518402;
        public static double Longitude { get; set; } = 55.1544687;
        public static int MapZoom { get; set; } = 13;
        public static string Counrty { get; set; } = "ایران";
        public static string State { get; set; } = "گلستان";
        public static string City { get; set; } = "گنبدکاووس";
        public static string DatabasePrefix { get; set; } = "dbo";
    }
}

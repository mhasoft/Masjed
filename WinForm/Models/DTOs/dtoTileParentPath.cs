using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WinForm.Models.DTOs
{
    public class dtoTileParentPath
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? SubCode { get; set; } // SubCode ممکن است NULL باشد
        public string Title { get; set; }
        public string ItemType { get; set; }
        public int LevelFromCurrent { get; set; }
        public int LevelFromBase { get; set; }
        public string DisplayTitle { get; set; }
    }
}

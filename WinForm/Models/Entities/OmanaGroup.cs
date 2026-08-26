using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("OmanaGroup")]
    public class OmanaGroup
    {
        [Key]
        [PersianName("شناسه")]
        public int Id { get; set; }

        [PersianName("کد مسجد")]
        public int MasjedId { get; set; }

        [PersianName("کد شخص")]
        public int PersonId { get; set; }

        [PersianName("جایگاه شخص")]
        public int OmanaTypeId { get; set; }

        [PersianName("توضیحات")]
        public string? Describe { get; set; }

        [PersianName("تاریخ شروع حکم")]
        public DateTime StartDate { get; set; }

        [PersianName("تاریخ پایان حکم")]
        public DateTime? EndDate { get; set; }
    }
}

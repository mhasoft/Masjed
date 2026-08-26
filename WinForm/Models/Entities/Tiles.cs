using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Controls.AnimatedTilePanel.Enums;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("Tiles")]
    public class Tiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [PersianName("کد")]
        public string Code { get; set; }

        [PersianName("زیرکد")]
        public string? SubCode { get; set; }

        [PersianName("نوع آیتم")]
        public DashboardTileItemType ItemType { get; set; }

        [PersianName("عنوان")]
        public string? Title { get; set; }

        [PersianName("توضیحات")]
        public string? Describe { get; set; }

        [PersianName("آیکن")]
        public string? Icon { get; set; }

        [PersianName("نام فرم")]
        public string? FormName { get; set; }

        [PersianName("کد فعال‌ساز")]
        public string? ActivatorCode { get; set; }

        [PersianName("کد مجوز")]
        public string? LicenseCode { get; set; }

        [NotMapped]
        [PersianName("شناسه")]
        public int? Id { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("Masjed")]
    public class Masjed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [PersianName("شناسه")]
        public int Id { get; set; }

        [Required]
        [PersianName("نام مسجد")]
        public string Name { get; set; } = string.Empty;

        [PersianName("شناسه محله")]
        public int Mahale_Id { get; set; }

        [PersianName("آدرس")]
        public string? Address { get; set; }

        [PersianName("شماره تماس")]
        public string? PhoneNumber { get; set; }

        [PersianName("مختصات JSON")]
        public string? CoordinatesJson { get; set; }

        [PersianName("عرض جغرافیایی")]
        public string? Latitude { get; set; }

        [PersianName("طول جغرافیایی")]
        public string? Longitude { get; set; }

        [PersianName("تاریخ ایجاد")]
        public DateTime CreateAt { get; set; }
    }
}

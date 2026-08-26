using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("Amaken")]
    public class Amaken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [PersianName("شناسه")]
        public int Id { get; set; }

        [Required]
        [PersianName("شناسه محله")]
        public int Mahale_Id { get; set; }

        [Required]
        [PersianName("نام مکان")]
        public string Name { get; set; } = string.Empty;

        [PersianName("نام مسئول / مالک")]
        public string? Owner { get; set; }

        [PersianName("آدرس")]
        public string? Address { get; set; }

        [PersianName("توضیحات")]
        public string? Description { get; set; }

        [PersianName("شماره تماس")]
        public string? PhoneNumber { get; set; }

        [PersianName("عرض جغرافیایی")]
        public string? Latitude { get; set; }

        [PersianName("طول جغرافیایی")]
        public string? Longitude { get; set; }

        [PersianName("نام آیکون")]
        public string? IconName { get; set; }
    }
}

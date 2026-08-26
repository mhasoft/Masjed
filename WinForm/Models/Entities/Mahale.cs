using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("Mahale")]
    public class Mahale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [PersianName("شناسه")]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [PersianName("نام محله")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [PersianName("مختصات محدوده (JSON)")]
        public string CoordinatesJson { get; set; } = string.Empty;

        [PersianName("تاریخ ایجاد")]
        public DateTime CreateAt { get; set; }
    }
}

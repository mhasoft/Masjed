using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("Persons")]
    public class Persons
    {
        [Key]
        [PersianName("شناسه")]
        public int Id { get; set; }

        [Required]
        [PersianName("نام")]
        public string FName { get; set; }

        [Required]
        [PersianName("نام خانوادگی")]
        public string LName { get; set; }

        /// <summary>
        /// این فیلد در دیتابیس دارای Constraint JSON است.
        /// می توانید لیست آدرس ها یا اطلاعات تکمیلی مکان را در آن ذخیره کنید.
        /// </summary>
        [PersianName("آدرس (JSON)")]
        public string? Address { get; set; }

        /// <summary>
        /// این فیلد در دیتابیس دارای Constraint JSON است.
        /// مناسب برای ذخیره چندین شماره تماس (موبایل، ثابت، تلگرام و ...).
        /// </summary>
        [PersianName("شماره تماس (JSON)")]
        public string? PhoneNumber { get; set; }

        [PersianName("مسیر تصویر")]
        public string? PicturePath { get; set; }

        [PersianName("تاریخ ایجاد")]
        public DateTime CreateAt { get; set; }

        // ویژگی محاسباتی برای نمایش نام کامل در UI
        [NotMapped]
        [PersianName("نام و نام خانوادگی")]
        public string FullName => $"{FName} {LName}";
    }
}

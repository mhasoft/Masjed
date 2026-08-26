using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinForm.Utility;

namespace WinForm.Models.Entities
{
    [Table("PictureType")]
    public class PictureType
    {
        [Key]
        [PersianName("شناسه")]
        public int Id { get; set; }

        [Required]
        [PersianName("عنوان")]
        public string Title { get; set; }

        [PersianName("توضیحات")]
        public string? Describe { get; set; }
    }
}

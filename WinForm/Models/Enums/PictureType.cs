using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Utility;

namespace WinForm.Models.Enums
{
    public enum PictureType
    {
        [PersianName("سردر")] Masjed_Sardab,
        [PersianName("محراب")] Masjed_Mehrab,
        [PersianName("خادم")] Masjed_Khadem,
        [PersianName("امام جماعت")] Masjed_EmamJamat,
        [PersianName("رئیس هیئت امنا")] Omana_Raeis,
        [PersianName("دبیر هیئت امنا")] Omana_Dabir,
        [PersianName("اعضای هیئت امنا")] Omana,
        [PersianName("رئیس هلال احمر")] HelalAhmar_Raeis,
        [PersianName("دبیر هلال احمر")] HelalAhmar_Dabir,
        [PersianName("عمومی")] General,
        [PersianName("متفرقه")] Others
    }
}

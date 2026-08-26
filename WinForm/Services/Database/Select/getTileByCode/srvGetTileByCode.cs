using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Controls.AnimatedTilePanel.Entities;
using WinForm.Models.Entities;

namespace WinForm.Services.Database.Select.getTileByCode
{
    public static class srvGetTileByCode
    {
        //private readonly string _connectionString;

        // constructor برای دریافت رشته اتصال
        //public srvGetTileByCode()
        //{
        //}

        /// <summary>
        /// متدی برای فراخوانی تابع SQL dbo.fn_GetTileParentPath
        /// </summary>
        /// <param name="startCode">کدی که میخواهیم مسیر والد آن را بگیریم.</param>
        /// <returns>لیستی از اطلاعات سلسله مراتب.</returns>
        public static IEnumerable<Models.Entities.Tiles> Execute(string Code)
        {
            IEnumerable<Models.Entities.Tiles> result = Enumerable.Empty<Models.Entities.Tiles>();

            // اطمینان از معتبر بودن کد ورودی
            if (String.IsNullOrWhiteSpace(Code))
            {
                // می‌توانید اینجا exception پرتاب کنید یا یک لیست خالی برگردانید
                return result;
            }

            // کوئری SQL برای فراخوانی تابع
            // توجه: نام تابع در SQL Server باید دقیقاً مطابقت داشته باشد
            string query = @$"
                SELECT
                    *
                FROM Tiles
                WHERE Code={Code}
            ";

            try
            {
                // اجرای کوئری و دریافت نتایج با Dapper
                result = Program.Database.Query<Models.Entities.Tiles>(
                    query,
                    new { Code = Code } // پارامتر ورودی تابع SQL
                ).ToList(); // .ToList() باعث اجرای فوری کوئری می‌شود

                // اگر می‌خواهید تابع را مرتب کنید (چون خود تابع LevelFromBase دارد، ممکن است لازم نباشد)
                // result = result.OrderBy(t => t.LevelFromBase).ToList();
            }
            catch (Exception ex)
            {
                // مدیریت خطاهای SQL
                // مثلاً لاگ کردن خطا
                Console.WriteLine($"SQL Error: {ex.Message}");
                // می‌توانید exception را دوباره پرتاب کنید یا خطا را به کاربر نمایش دهید
                throw; // یا return Enumerable.Empty<TileParentPathInfo>();
            }

            return result;
        }
    }
}

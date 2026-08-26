using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Controls.AnimatedTilePanel.Entities;
using WinForm.Models.Entities;

namespace WinForm.Services.Database.Select.getTilesBySubCode
{
    public class srvGetTileBySubCode
    {
        private readonly string _connectionString;

        // constructor برای دریافت رشته اتصال
        public srvGetTileBySubCode()
        {
        }

        /// <summary>
        /// متدی برای فراخوانی تابع SQL dbo.fn_GetTileParentPath
        /// </summary>
        /// <param name="startCode">کدی که میخواهیم مسیر والد آن را بگیریم.</param>
        /// <returns>لیستی از اطلاعات سلسله مراتب.</returns>
        public IEnumerable<Models.Entities.Tiles> Execute(string SubCode)
        {
            IEnumerable<Models.Entities.Tiles> result = Enumerable.Empty<Models.Entities.Tiles>();

            // اطمینان از معتبر بودن کد ورودی
            if (String.IsNullOrWhiteSpace(SubCode))
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
                WHERE SubCode={SubCode}
            ";

            try
            {
                // اجرای کوئری و دریافت نتایج با Dapper
                result = Program.Database.Query<Models.Entities.Tiles>(
                    query,
                    new { Code = SubCode } // پارامتر ورودی تابع SQL
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

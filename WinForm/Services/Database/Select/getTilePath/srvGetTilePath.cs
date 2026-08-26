using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using WinForm.Models.DTOs;
using WinForm.Services.License.getPermission;
using WinForm.Services.ShowMessage.getShowMessage.Model;

namespace WinForm.Services.Database.Select.getTilePath
{
    public class srvGetTilePath
    {
        private readonly string _connectionString;

        // constructor برای دریافت رشته اتصال
        public srvGetTilePath()
        {
        }

        /// <summary>
        /// متدی برای فراخوانی تابع SQL dbo.fn_GetTileParentPath
        /// </summary>
        /// <param name="startCode">کدی که میخواهیم مسیر والد آن را بگیریم.</param>
        /// <returns>لیستی از اطلاعات سلسله مراتب.</returns>
        public IEnumerable<dtoTileParentPath> Execute(string Code)
        {
            

            IEnumerable<dtoTileParentPath> result = Enumerable.Empty<dtoTileParentPath>();

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
                    Id,
                    Code,
                    SubCode,
                    Title,
                    ItemType,
                    LevelFromCurrent,
                    LevelFromBase,
                    DisplayTitle
                FROM dbo.fn_GetTileParentPathByCode(@Code)
                ORDER BY LevelFromBase; -- اطمینان از ترتیب نمایش برای ساختار درختی
            ";

            try
            {
                // اجرای کوئری و دریافت نتایج با Dapper
                result = Program.Database.Query<dtoTileParentPath>(
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

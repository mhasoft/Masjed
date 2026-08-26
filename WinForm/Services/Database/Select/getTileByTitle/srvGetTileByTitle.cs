using Dapper;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using WinForm.Models.Entities;
using WinForm.Services.License.getPermission;
using WinForm.Services.ShowMessage.getShowMessage.Model;

namespace WinForm.Services.Database.Select.getTileByTitle
{
    public class srvGetTileByTitle
    {
        public srvGetTileByTitle()
        {
            
        }
        /// <summary>
        /// متدی برای فراخوانی تابع SQL dbo.fn_GetTileParentPath
        /// </summary>
        /// <param name="startCode">کدی که میخواهیم مسیر والد آن را بگیریم.</param>
        /// <returns>لیستی از اطلاعات سلسله مراتب.</returns>
        public IEnumerable<Models.Entities.Tiles> Execute(string Title)
        {
            IEnumerable<Models.Entities.Tiles> result = Enumerable.Empty<Models.Entities.Tiles>();

            // اطمینان از معتبر بودن کد ورودی
            if (String.IsNullOrWhiteSpace(Title))
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
                WHERE Title like N'%{Title}%' and Code<>'0'
                ORDER BY Title;
            ";

            Debug.WriteLine(query);

            try
            {
                // اجرای کوئری و دریافت نتایج با Dapper
                result = Program.Database.Query<Models.Entities.Tiles>(
                    query,
                    new { Code = Title } // پارامتر ورودی تابع SQL
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

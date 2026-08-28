using System;
using System.Collections.Generic;
using System.Linq;
using WinForm.Database;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Insert.setMasjedAllRecord
{
    public class srvSetMasjedAllRecord
    {
        public static Dictionary<string, Masjed> Execute(
            Dictionary<string, Masjed> itemsToSave)
        {
            if (itemsToSave == null || itemsToSave.Count == 0)
            {
                return new Dictionary<string, Masjed>();
            }

            try
            {
                // =========================================================
                // دریافت تمام Idهای موجود در دیتابیس
                // =========================================================

                string getAllIdsSql = $@"
                    SELECT
                        {srvEntityTableName.FieldName<Masjed>(x => x.Id)}
                    FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Masjed>()};";

                List<int> databaseIds =
                    Program.Database.Query<int>(getAllIdsSql);

                HashSet<int> databaseIdSet =
                    databaseIds.ToHashSet();


                // =========================================================
                // لیست رکوردهای معتبر ورودی
                // =========================================================

                List<Masjed> itemsList =
                    itemsToSave.Values
                        .Where(x => x != null)
                        .ToList();


                // =========================================================
                // Id رکوردهای موجود در DB که باید حفظ شوند
                // =========================================================

                HashSet<int> existingIdsToKeep =
                    new HashSet<int>();


                // =========================================================
                // تمام دستورات INSERT / UPDATE / DELETE
                // =========================================================

                List<DatabaseInsertCommand> commands =
                    new List<DatabaseInsertCommand>();


                // =========================================================
                // نگهداری INSERTها برای دریافت Id جدید
                //
                // کلید:
                // شماره دستور در commands
                //
                // مقدار:
                // شیء Masjed مربوط به همان INSERT
                // =========================================================

                Dictionary<int, Masjed> insertedMasjedByCommandIndex =
                    new Dictionary<int, Masjed>();


                // =========================================================
                // SQL - INSERT
                //
                // نکته مهم:
                // Mahale_Id نیز همراه مسجد ذخیره می‌شود.
                // =========================================================

                string insertSql = $@"
                    INSERT INTO
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Masjed>()}
                    (
                        {srvEntityTableName.FieldName<Masjed>(x => x.Name)},
                        {srvEntityTableName.FieldName<Masjed>(x => x.Address)},
                        {srvEntityTableName.FieldName<Masjed>(x => x.PhoneNumber)},
                        {srvEntityTableName.FieldName<Masjed>(x => x.CoordinatesJson)},
                        {srvEntityTableName.FieldName<Masjed>(x => x.CreateAt)},
                        {srvEntityTableName.FieldName<Masjed>(x => x.Mahale_Id)}
                    )
                    VALUES
                    (
                        @Name,
                        @Address,
                        @PhoneNumber,
                        @CoordinatesJson,
                        @CreateAt,
                        @Mahale_Id
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";


                // =========================================================
                // SQL - UPDATE
                //
                // Mahale_Id نیز در صورت تغییر والد به‌روزرسانی می‌شود.
                // =========================================================

                string updateSql = $@"
                    UPDATE
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Masjed>()}
                    SET
                        {srvEntityTableName.FieldName<Masjed>(x => x.Name)} =
                            @Name,

                        {srvEntityTableName.FieldName<Masjed>(x => x.Address)} =
                            @Address,

                        {srvEntityTableName.FieldName<Masjed>(x => x.PhoneNumber)} =
                            @PhoneNumber,

                        {srvEntityTableName.FieldName<Masjed>(x => x.CoordinatesJson)} =
                            @CoordinatesJson,

                        {srvEntityTableName.FieldName<Masjed>(x => x.Mahale_Id)} =
                            @Mahale_Id
                    WHERE
                        {srvEntityTableName.FieldName<Masjed>(x => x.Id)} =
                            @Id;";


                // =========================================================
                // SQL - DELETE
                // =========================================================

                string deleteSql = $@"
                    DELETE FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Masjed>()}
                    WHERE
                        {srvEntityTableName.FieldName<Masjed>(x => x.Id)}
                        IN @Ids;";


                // =========================================================
                // پردازش تمام رکوردهای ورودی
                // =========================================================

                foreach (
                    KeyValuePair<string, Masjed> item
                    in itemsToSave)
                {
                    string mapKey =
                        item.Key;

                    Masjed masjed =
                        item.Value;


                    // -----------------------------------------------------
                    // بررسی Null
                    // -----------------------------------------------------

                    if (masjed == null)
                    {
                        throw new Exception(
                            $"اطلاعات مسجد با کلید «{mapKey}» معتبر نیست."
                        );
                    }


                    // -----------------------------------------------------
                    // بررسی نام مسجد
                    // -----------------------------------------------------

                    if (string.IsNullOrWhiteSpace(masjed.Name))
                    {
                        throw new Exception(
                            $"نام مسجد با کلید «{mapKey}» خالی است."
                        );
                    }


                    // -----------------------------------------------------
                    // بررسی Mahale_Id
                    //
                    // هر مسجد باید به یک محله معتبر متصل باشد.
                    //
                    // این مقدار باید قبل از اجرای این سرویس توسط ucMap
                    // تعیین شده باشد.
                    // -----------------------------------------------------

                    if (masjed.Mahale_Id <= 0)
                    {
                        throw new Exception(
                            $"مسجد «{masjed.Name.Trim()}» " +
                            $"با کلید «{mapKey}» فاقد Mahale_Id معتبر است."
                        );
                    }


                    // -----------------------------------------------------
                    // مقداردهی CreateAt در صورت خالی بودن
                    // -----------------------------------------------------

                    masjed.CreateAt =
                        masjed.CreateAt == default(DateTime)
                            ? DateTime.Now
                            : masjed.CreateAt;


                    // -----------------------------------------------------
                    // Trim نام
                    // -----------------------------------------------------

                    masjed.Name =
                        masjed.Name.Trim();


                    // =====================================================
                    // UPDATE
                    //
                    // اگر Id معتبر باشد و رکورد در DB وجود داشته باشد.
                    // =====================================================

                    if (
                        masjed.Id > 0 &&
                        databaseIdSet.Contains(masjed.Id)
                    )
                    {
                        commands.Add(
                            new DatabaseInsertCommand
                            {
                                SqlScript =
                                    updateSql,

                                Parameters =
                                    new
                                    {
                                        Id =
                                            masjed.Id,

                                        Name =
                                            masjed.Name,

                                        Address =
                                            masjed.Address,

                                        PhoneNumber =
                                            masjed.PhoneNumber,

                                        CoordinatesJson =
                                            masjed.CoordinatesJson,

                                        Mahale_Id =
                                            masjed.Mahale_Id
                                    },

                                HasOutput =
                                    false
                            });


                        // -------------------------------------------------
                        // این رکورد در DB وجود دارد و باید حفظ شود.
                        // -------------------------------------------------

                        existingIdsToKeep.Add(
                            masjed.Id);


                        continue;
                    }


                    // =====================================================
                    // INSERT
                    //
                    // حالت اول:
                    // Id <= 0
                    //
                    // حالت دوم:
                    // Id > 0 ولی در DB وجود ندارد.
                    // =====================================================

                    int commandIndex =
                        commands.Count;


                    commands.Add(
                        new DatabaseInsertCommand
                        {
                            SqlScript =
                                insertSql,

                            Parameters =
                                new
                                {
                                    Name =
                                        masjed.Name,

                                    Address =
                                        masjed.Address,

                                    PhoneNumber =
                                        masjed.PhoneNumber,

                                    CoordinatesJson =
                                        masjed.CoordinatesJson,

                                    CreateAt =
                                        masjed.CreateAt,

                                    Mahale_Id =
                                        masjed.Mahale_Id
                                },

                            HasOutput =
                                true
                        });


                    // -----------------------------------------------------
                    // بعد از اجرای Transaction، Id جدید دریافت می‌شود.
                    // -----------------------------------------------------

                    insertedMasjedByCommandIndex.Add(
                        commandIndex,
                        masjed);
                }


                // =========================================================
                // تشخیص رکوردهایی که باید حذف شوند
                //
                // مثال:
                //
                // DB:
                // 1
                // 2
                // 3
                //
                // ورودی:
                // 1
                // 2
                //
                // نتیجه:
                // 3 حذف می‌شود.
                // =========================================================

                List<int> idsToDelete =
                    databaseIds
                        .Where(
                            id =>
                                !existingIdsToKeep.Contains(id))
                        .ToList();


                // =========================================================
                // DELETE
                // =========================================================

                if (idsToDelete.Count > 0)
                {
                    commands.Add(
                        new DatabaseInsertCommand
                        {
                            SqlScript =
                                deleteSql,

                            Parameters =
                                new
                                {
                                    Ids =
                                        idsToDelete
                                },

                            HasOutput =
                                false
                        });
                }


                // =========================================================
                // اگر هیچ تغییری وجود نداشته باشد
                // =========================================================

                if (commands.Count == 0)
                {
                    return itemsList.ToDictionary(
                        x => x.Id.ToString(),
                        x => x);
                }


                // =========================================================
                // اجرای تمام عملیات
                //
                // INSERT
                // UPDATE
                // DELETE
                //
                // Program.Database.Insert باید این دستورات را
                // در Transaction اجرا کند.
                // =========================================================

                List<object> results =
                    Program.Database.Insert(
                        commands);


                // =========================================================
                // دریافت Id رکوردهای جدید
                // =========================================================

                foreach (
                    KeyValuePair<int, Masjed> item
                    in insertedMasjedByCommandIndex)
                {
                    int commandIndex =
                        item.Key;

                    Masjed masjed =
                        item.Value;


                    // -----------------------------------------------------
                    // بررسی خروجی
                    // -----------------------------------------------------

                    if (
                        commandIndex < 0 ||
                        commandIndex >= results.Count
                    )
                    {
                        throw new Exception(
                            "تعداد خروجی‌های دیتابیس با تعداد دستورات " +
                            "هم‌خوانی ندارد."
                        );
                    }


                    object result =
                        results[commandIndex];


                    // -----------------------------------------------------
                    // Id باید از SCOPE_IDENTITY دریافت شده باشد.
                    // -----------------------------------------------------

                    if (
                        result == null ||
                        result == DBNull.Value
                    )
                    {
                        throw new Exception(
                            $"شناسه مسجد جدید «{masjed.Name}» " +
                            "پس از ثبت دریافت نشد."
                        );
                    }


                    masjed.Id =
                        Convert.ToInt32(result);


                    // -----------------------------------------------------
                    // Mahale_Id از قبل روی همان شیء قرار گرفته است.
                    //
                    // بنابراین بعد از دریافت Id مسجد، اطلاعات کامل
                    // همان شیء در mapMasjed باقی می‌ماند.
                    // -----------------------------------------------------
                }


                // =========================================================
                // ساخت Dictionary نهایی
                //
                // نکته:
                // itemsList شامل همان Referenceهای اصلی اشیاء است؛
                // بنابراین Idهای دریافت‌شده در خود mapMasjed نیز
                // قابل مشاهده خواهند بود.
                // =========================================================

                return itemsList.ToDictionary(
                    x => x.Id.ToString(),
                    x => x);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SQL Error in srvSetMasjedAllRecord: {ex.Message}"
                );

                throw new Exception(
                    $"خطا هنگام ذخیره‌سازی اطلاعات مسجدها." +
                    Environment.NewLine +
                    $"جزئیات: {ex.Message}",
                    ex
                );
            }
        }
    }
}

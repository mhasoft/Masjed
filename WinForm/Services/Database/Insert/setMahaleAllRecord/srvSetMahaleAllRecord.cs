using System;
using System.Collections.Generic;
using System.Linq;
using WinForm.Database;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Insert.setMahaleAllRecord
{
    public class srvSetMahaleAllRecord
    {
        public static Dictionary<string, Mahale> Execute(
            Dictionary<string, Mahale> itemsToSave)
        {
            if (itemsToSave == null || itemsToSave.Count == 0)
            {
                return new Dictionary<string, Mahale>();
            }

            try
            {
                // =========================================================
                // دریافت تمام رکوردهای موجود در دیتابیس
                // =========================================================
                string getAllSql = $@"
                    SELECT
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)},
                        {srvEntityTableName.FieldName<Mahale>(x => x.Name)}
                    FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()};";

                List<Mahale> databaseItems =
                    Program.Database.Query<Mahale>(getAllSql);

                HashSet<int> databaseIdSet =
                    databaseItems
                        .Select(x => x.Id)
                        .ToHashSet();


                // =========================================================
                // لیست رکوردهای ورودی
                // =========================================================
                List<Mahale> itemsList =
                    itemsToSave.Values
                        .Where(x => x != null)
                        .ToList();


                // =========================================================
                // Id رکوردهایی که باید حفظ شوند
                // =========================================================
                HashSet<int> existingIdsToKeep =
                    new HashSet<int>();


                // =========================================================
                // دستورات دیتابیس
                // =========================================================
                List<DatabaseInsertCommand> commands =
                    new List<DatabaseInsertCommand>();


                // =========================================================
                // نگهداری INSERTها برای دریافت Id
                // =========================================================
                Dictionary<int, Mahale> insertedMahaleByCommandIndex =
                    new Dictionary<int, Mahale>();


                // =========================================================
                // SQL - INSERT
                // =========================================================
                string insertSql = $@"
                    INSERT INTO
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    (
                        {srvEntityTableName.FieldName<Mahale>(x => x.Name)},
                        {srvEntityTableName.FieldName<Mahale>(x => x.CoordinatesJson)},
                        {srvEntityTableName.FieldName<Mahale>(x => x.CreateAt)}
                    )
                    VALUES
                    (
                        @Name,
                        @CoordinatesJson,
                        @CreateAt
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";


                // =========================================================
                // SQL - UPDATE
                // =========================================================
                string updateSql = $@"
                    UPDATE
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    SET
                        {srvEntityTableName.FieldName<Mahale>(x => x.Name)} = @Name,
                        {srvEntityTableName.FieldName<Mahale>(x => x.CoordinatesJson)} = @CoordinatesJson
                    WHERE
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)} = @Id;";


                // =========================================================
                // SQL - DELETE
                // =========================================================
                string deleteSql = $@"
                    DELETE FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    WHERE
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)} IN @Ids;";


                // =========================================================
                // پردازش رکوردهای ورودی
                // =========================================================
                foreach (KeyValuePair<string, Mahale> item in itemsToSave)
                {
                    string mapKey = item.Key;
                    Mahale mahale = item.Value;


                    // -----------------------------------------------------
                    // Null
                    // -----------------------------------------------------
                    if (mahale == null)
                    {
                        throw new Exception(
                            $"اطلاعات محله با کلید «{mapKey}» معتبر نیست."
                        );
                    }


                    // -----------------------------------------------------
                    // اعتبارسنجی نام
                    // -----------------------------------------------------
                    if (string.IsNullOrWhiteSpace(mahale.Name))
                    {
                        throw new Exception(
                            $"نام محله با کلید «{mapKey}» خالی است."
                        );
                    }


                    // -----------------------------------------------------
                    // اعتبارسنجی مختصات
                    // -----------------------------------------------------
                    if (string.IsNullOrWhiteSpace(mahale.CoordinatesJson))
                    {
                        throw new Exception(
                            $"مختصات محله «{mahale.Name}» خالی است."
                        );
                    }


                    mahale.Name =
                        mahale.Name.Trim();


                    mahale.CreateAt =
                        mahale.CreateAt == default(DateTime)
                            ? DateTime.Now
                            : mahale.CreateAt;


                    // =====================================================
                    // ابتدا بررسی Id
                    // =====================================================
                    if (mahale.Id > 0 &&
                        databaseIdSet.Contains(mahale.Id))
                    {
                        // =================================================
                        // رکورد موجود است => UPDATE
                        // =================================================
                        commands.Add(new DatabaseInsertCommand
                        {
                            SqlScript = updateSql,

                            Parameters = new
                            {
                                Id = mahale.Id,
                                Name = mahale.Name,
                                CoordinatesJson = mahale.CoordinatesJson
                            },

                            HasOutput = false
                        });

                        existingIdsToKeep.Add(mahale.Id);

                        continue;
                    }


                    // =====================================================
                    // اگر Id معتبر نبود، بررسی نام موجود در DB
                    //
                    // این قسمت مشکل ویرایش محدوده را حل می‌کند.
                    // =====================================================
                    Mahale? existingByName =
                        databaseItems.FirstOrDefault(x =>
                            string.Equals(
                                x.Name?.Trim(),
                                mahale.Name,
                                StringComparison.OrdinalIgnoreCase));


                    if (existingByName != null)
                    {
                        // =================================================
                        // نام قبلاً وجود دارد.
                        //
                        // بنابراین رکورد جدید نیست؛
                        // همان رکورد موجود را UPDATE می‌کنیم.
                        // =================================================
                        mahale.Id =
                            existingByName.Id;

                        mahale.CreateAt =
                            existingByName.CreateAt;

                        commands.Add(new DatabaseInsertCommand
                        {
                            SqlScript = updateSql,

                            Parameters = new
                            {
                                Id = mahale.Id,
                                Name = mahale.Name,
                                CoordinatesJson = mahale.CoordinatesJson
                            },

                            HasOutput = false
                        });

                        existingIdsToKeep.Add(
                            mahale.Id);

                        continue;
                    }


                    // =====================================================
                    // INSERT
                    //
                    // فقط زمانی انجام می‌شود که:
                    //
                    // 1. Id وجود نداشته باشد
                    // 2. Id در DB وجود نداشته باشد
                    // 3. نام نیز در DB وجود نداشته باشد
                    // =====================================================
                    int commandIndex =
                        commands.Count;


                    commands.Add(new DatabaseInsertCommand
                    {
                        SqlScript = insertSql,

                        Parameters = new
                        {
                            Name = mahale.Name,
                            CoordinatesJson = mahale.CoordinatesJson,
                            CreateAt = mahale.CreateAt
                        },

                        HasOutput = true
                    });


                    insertedMahaleByCommandIndex.Add(
                        commandIndex,
                        mahale);
                }


                // =========================================================
                // رکوردهای حذف‌شدنی
                // =========================================================
                List<int> idsToDelete =
                    databaseItems
                        .Select(x => x.Id)
                        .Where(id =>
                            !existingIdsToKeep.Contains(id))
                        .ToList();


                // =========================================================
                // DELETE
                // =========================================================
                if (idsToDelete.Count > 0)
                {
                    commands.Add(new DatabaseInsertCommand
                    {
                        SqlScript = deleteSql,

                        Parameters = new
                        {
                            Ids = idsToDelete
                        },

                        HasOutput = false
                    });
                }


                // =========================================================
                // اگر تغییری وجود نداشت
                // =========================================================
                if (commands.Count == 0)
                {
                    return itemsList.ToDictionary(
                        x => x.Id.ToString(),
                        x => x);
                }


                // =========================================================
                // اجرای Transaction
                // =========================================================
                List<object> results =
                    Program.Database.Insert(commands);


                // =========================================================
                // دریافت Id رکوردهای جدید
                // =========================================================
                foreach (
                    KeyValuePair<int, Mahale> item
                    in insertedMahaleByCommandIndex)
                {
                    int commandIndex = item.Key;
                    Mahale mahale = item.Value;


                    if (commandIndex < 0 ||
                        commandIndex >= results.Count)
                    {
                        throw new Exception(
                            "تعداد خروجی‌های دیتابیس با تعداد دستورات " +
                            "هم‌خوانی ندارد."
                        );
                    }


                    object result =
                        results[commandIndex];


                    if (result == null ||
                        result == DBNull.Value)
                    {
                        throw new Exception(
                            $"شناسه محله جدید «{mahale.Name}» " +
                            "پس از ثبت دریافت نشد."
                        );
                    }


                    mahale.Id =
                        Convert.ToInt32(result);
                }


                // =========================================================
                // ساخت Dictionary نهایی
                // =========================================================
                return itemsList.ToDictionary(
                    x => x.Id.ToString(),
                    x => x);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SQL Error in srvSetMahaleAllRecord: {ex.Message}"
                );

                throw new Exception(
                    $"خطا هنگام ذخیره‌سازی اطلاعات محله‌ها.{Environment.NewLine}" +
                    $"جزئیات: {ex.Message}",
                    ex
                );
            }
        }
    }
}
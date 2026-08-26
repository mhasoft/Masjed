using System;
using System.Collections.Generic;
using System.Linq;
using WinForm.Database;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Insert.setAmakenAllRecord
{
    public static class srvSetAmakenAllRecord
    {
        public static Dictionary<string, Amaken> Execute(
            Dictionary<string, Amaken> itemsToSave)
        {
            if (itemsToSave == null || itemsToSave.Count == 0)
            {
                return new Dictionary<string, Amaken>();
            }

            try
            {
                // =========================================================
                // دریافت تمام Idهای موجود در جدول Amaken
                // =========================================================
                string getAllAmakenIdsSql = $@"
                    SELECT
                        {srvEntityTableName.FieldName<Amaken>(x => x.Id)}
                    FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Amaken>()};";

                List<int> databaseAmakenIds =
                    Program.Database.Query<int>(getAllAmakenIdsSql);

                HashSet<int> databaseAmakenIdSet =
                    databaseAmakenIds.ToHashSet();


                // =========================================================
                // لیست رکوردهای ورودی
                // =========================================================
                List<Amaken> itemsList =
                    itemsToSave.Values
                        .Where(x => x != null)
                        .ToList();


                // =========================================================
                // اعتبارسنجی اولیه رکوردهای ورودی
                // =========================================================
                foreach (KeyValuePair<string, Amaken> item in itemsToSave)
                {
                    string mapKey = item.Key;
                    Amaken amaken = item.Value;

                    // -----------------------------------------------------
                    // بررسی Null
                    // -----------------------------------------------------
                    if (amaken == null)
                    {
                        throw new Exception(
                            $"اطلاعات مکان با کلید «{mapKey}» معتبر نیست."
                        );
                    }

                    // -----------------------------------------------------
                    // بررسی Mahale_Id
                    // -----------------------------------------------------
                    if (amaken.Mahale_Id <= 0)
                    {
                        throw new Exception(
                            $"شناسه محله برای مکان «{amaken.Name}» " +
                            $"معتبر نیست."
                        );
                    }

                    // -----------------------------------------------------
                    // بررسی Name
                    // -----------------------------------------------------
                    if (string.IsNullOrWhiteSpace(amaken.Name))
                    {
                        throw new Exception(
                            $"نام مکان با کلید «{mapKey}» خالی است."
                        );
                    }

                    // -----------------------------------------------------
                    // Trim نام مکان
                    // -----------------------------------------------------
                    amaken.Name = amaken.Name.Trim();
                }


                // =========================================================
                // دریافت تمام Mahale_Idهای مورد نیاز
                //
                // فقط Idهای یکتا خوانده می‌شوند.
                // =========================================================
                HashSet<int> requestedMahaleIds =
                    itemsList
                        .Select(x => x.Mahale_Id)
                        .Distinct()
                        .ToHashSet();


                // =========================================================
                // دریافت Mahaleهای موجود در دیتابیس
                // =========================================================
                string getMahaleIdsSql = $@"
                    SELECT
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)}
                    FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    WHERE
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)}
                        IN @Ids;";

                List<int> existingMahaleIds =
                    Program.Database.Query<int>(
                        getMahaleIdsSql,
                        new
                        {
                            Ids = requestedMahaleIds.ToList()
                        });


                HashSet<int> existingMahaleIdSet =
                    existingMahaleIds.ToHashSet();


                // =========================================================
                // پیدا کردن Mahale_Idهای نامعتبر
                // =========================================================
                List<int> invalidMahaleIds =
                    requestedMahaleIds
                        .Where(id => !existingMahaleIdSet.Contains(id))
                        .OrderBy(id => id)
                        .ToList();


                // =========================================================
                // اگر حتی یک Mahale وجود نداشته باشد، هیچ عملیات
                // INSERT / UPDATE / DELETE اجرا نمی‌شود.
                // =========================================================
                if (invalidMahaleIds.Count > 0)
                {
                    List<string> invalidMahaleMessages =
                        itemsList
                            .Where(x =>
                                invalidMahaleIds.Contains(x.Mahale_Id))
                            .Select(x =>
                                $"مکان «{x.Name}» دارای شناسه محله " +
                                $"«{x.Mahale_Id}» است که در جدول محله‌ها وجود ندارد.")
                            .Distinct()
                            .ToList();

                    throw new Exception(
                        "برخی از شناسه‌های محله معتبر نیستند."
                        + Environment.NewLine
                        + string.Join(
                            Environment.NewLine,
                            invalidMahaleMessages)
                    );
                }


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
                // نگهداری رکوردهای جدید برای دریافت Id
                //
                // کلید:
                // شماره دستور در commands
                //
                // مقدار:
                // شیء Amaken
                // =========================================================
                Dictionary<int, Amaken> insertedAmakenByCommandIndex =
                    new Dictionary<int, Amaken>();


                // =========================================================
                // SQL - INSERT
                // =========================================================
                string insertSql = $@"
                    INSERT INTO
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Amaken>()}
                    (
                        {srvEntityTableName.FieldName<Amaken>(x => x.Mahale_Id)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Name)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Owner)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Address)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.PhoneNumber)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.IconName)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Description)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Latitude)},
                        {srvEntityTableName.FieldName<Amaken>(x => x.Longitude)}
                    )
                    VALUES
                    (
                        @Mahale_Id,
                        @Name,
                        @Owner,
                        @Address,
                        @PhoneNumber,
                        @IconName,
                        @Description,
                        @Latitude,
                        @Longitude
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";


                // =========================================================
                // SQL - UPDATE
                // =========================================================
                string updateSql = $@"
                    UPDATE
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Amaken>()}
                    SET
                        {srvEntityTableName.FieldName<Amaken>(x => x.Mahale_Id)} = @Mahale_Id,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Name)} = @Name,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Owner)} = @Owner,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Address)} = @Address,
                        {srvEntityTableName.FieldName<Amaken>(x => x.PhoneNumber)} = @PhoneNumber,
                        {srvEntityTableName.FieldName<Amaken>(x => x.IconName)} = @IconName,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Description)} = @Description,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Latitude)} = @Latitude,
                        {srvEntityTableName.FieldName<Amaken>(x => x.Longitude)} = @Longitude
                    WHERE
                        {srvEntityTableName.FieldName<Amaken>(x => x.Id)} = @Id;";


                // =========================================================
                // SQL - DELETE
                // =========================================================
                string deleteSql = $@"
                    DELETE FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Amaken>()}
                    WHERE
                        {srvEntityTableName.FieldName<Amaken>(x => x.Id)}
                        IN @Ids;";


                // =========================================================
                // پردازش تمام رکوردهای ورودی
                // =========================================================
                foreach (KeyValuePair<string, Amaken> item in itemsToSave)
                {
                    string mapKey = item.Key;
                    Amaken amaken = item.Value;


                    // =====================================================
                    // UPDATE
                    //
                    // اگر Id معتبر باشد و در DB وجود داشته باشد.
                    // =====================================================
                    if (amaken.Id > 0 &&
                        databaseAmakenIdSet.Contains(amaken.Id))
                    {
                        commands.Add(new DatabaseInsertCommand
                        {
                            SqlScript = updateSql,

                            Parameters = new
                            {
                                Id = amaken.Id,
                                Mahale_Id = amaken.Mahale_Id,
                                Name = amaken.Name,
                                Owner = amaken.Owner,
                                Address = amaken.Address,
                                PhoneNumber = amaken.PhoneNumber,
                                IconName = amaken.IconName,
                                Description = amaken.Description,
                                Latitude = amaken.Latitude,
                                Longitude = amaken.Longitude
                            },

                            HasOutput = false
                        });


                        // -------------------------------------------------
                        // این Id باید حفظ شود.
                        // -------------------------------------------------
                        existingIdsToKeep.Add(amaken.Id);
                    }


                    // =====================================================
                    // INSERT
                    //
                    // Id <= 0
                    //
                    // یا:
                    //
                    // Id > 0 ولی در DB وجود ندارد.
                    // =====================================================
                    else
                    {
                        int commandIndex =
                            commands.Count;


                        commands.Add(new DatabaseInsertCommand
                        {
                            SqlScript = insertSql,

                            Parameters = new
                            {
                                Mahale_Id = amaken.Mahale_Id,
                                Name = amaken.Name,
                                Owner = amaken.Owner,
                                Address = amaken.Address,
                                PhoneNumber = amaken.PhoneNumber,
                                IconName = amaken.IconName,
                                Description = amaken.Description,
                                Latitude = amaken.Latitude,
                                Longitude = amaken.Longitude
                            },

                            HasOutput = true
                        });


                        // -------------------------------------------------
                        // برای دریافت Id جدید
                        // -------------------------------------------------
                        insertedAmakenByCommandIndex.Add(
                            commandIndex,
                            amaken
                        );
                    }
                }


                // =========================================================
                // تشخیص رکوردهایی که باید حذف شوند
                //
                // مثال:
                //
                // DB:
                // Id = 1
                // Id = 2
                // Id = 3
                //
                // ورودی:
                // Id = 1
                // Id = 2
                //
                // نتیجه:
                // Id = 3 حذف می‌شود.
                // =========================================================
                List<int> idsToDelete =
                    databaseAmakenIds
                        .Where(id => !existingIdsToKeep.Contains(id))
                        .ToList();


                // =========================================================
                // افزودن دستور DELETE
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
                // اگر هیچ تغییری وجود ندارد
                // =========================================================
                if (commands.Count == 0)
                {
                    return itemsList.ToDictionary(
                        x => x.Id.ToString(),
                        x => x
                    );
                }


                // =========================================================
                // اجرای INSERT / UPDATE / DELETE
                //
                // انتظار می‌رود Insert این دستورات را داخل Transaction
                // اجرا کند.
                // =========================================================
                List<object> results =
                    Program.Database.Insert(commands);


                // =========================================================
                // دریافت Id رکوردهای جدید
                // =========================================================
                foreach (
                    KeyValuePair<int, Amaken> item
                    in insertedAmakenByCommandIndex)
                {
                    int commandIndex = item.Key;
                    Amaken amaken = item.Value;


                    // -----------------------------------------------------
                    // بررسی شماره دستور
                    // -----------------------------------------------------
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


                    // -----------------------------------------------------
                    // بررسی Id دریافت شده
                    // -----------------------------------------------------
                    if (result == null ||
                        result == DBNull.Value)
                    {
                        throw new Exception(
                            $"شناسه مکان جدید «{amaken.Name}» " +
                            $"پس از ثبت دریافت نشد."
                        );
                    }


                    // -----------------------------------------------------
                    // ثبت Id جدید روی شیء
                    // -----------------------------------------------------
                    amaken.Id =
                        Convert.ToInt32(result);
                }


                // =========================================================
                // ساخت Dictionary نهایی
                // =========================================================
                return itemsList.ToDictionary(
                    x => x.Id.ToString(),
                    x => x
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SQL Error in srvSetAmakenAllRecord: {ex.Message}"
                );

                throw new Exception(
                    $"خطا هنگام ذخیره‌سازی اطلاعات اماکن."
                    + Environment.NewLine +
                    $"جزئیات: {ex.Message}",
                    ex
                );
            }
        }
    }
}
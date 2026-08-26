using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WinForm.Database
{
    public class DatabaseContext
    {
        // توسط LoadConnectionString مقداردهی می‌شود.
        private string _connectionString = "";

        // مسیر پوشه‌ای که فایل اجرایی برنامه در آن قرار دارد.
        private string basePath = AppDomain.CurrentDomain.BaseDirectory;

        // اتصال مرکزی برنامه به پایگاه داده.
        private IDbConnection connection;

        #region Connection

        private string LoadConnectionString()
        {
            string filePath = Path.Combine(basePath, "connection.txt");

            if (!File.Exists(filePath))
            {
                Debug.WriteLine("فایل connection.txt پیدا نشد:");
                Debug.WriteLine(filePath);

                return null;
            }

            string connectionString = File.ReadAllText(filePath).Trim();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Debug.WriteLine("فایل connection.txt خالی است.");

                return null;
            }

            connectionString = connectionString.Replace(
                "{APP}",
                basePath.TrimEnd('\\')
            );

            return connectionString;
        }

        /// <summary>
        /// اتصال مرکزی برنامه به دیتابیس را ایجاد و باز می‌کند.
        /// </summary>
        public bool CreateConnection()
        {
            // اگر اتصال قبلاً باز است، دوباره اتصال جدید ایجاد نمی‌کنیم.
            if (connection != null &&
                connection.State == ConnectionState.Open)
            {
                return true;
            }

            _connectionString = LoadConnectionString();

            Debug.WriteLine("====================================");
            Debug.WriteLine(_connectionString);

            if (string.IsNullOrWhiteSpace(_connectionString))
                return false;

            try
            {
                connection?.Dispose();
                connection = null;

                connection = new SqlConnection(_connectionString);
                connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("====================================");
                Debug.WriteLine(ex.ToString());

                connection?.Dispose();
                connection = null;

                return false;
            }
        }

        /// <summary>
        /// اطمینان از باز و آماده‌بودن اتصال دیتابیس.
        /// </summary>
        private void EnsureConnection()
        {
            if (connection == null ||
                connection.State != ConnectionState.Open)
            {
                bool isConnected = CreateConnection();

                if (!isConnected)
                {
                    throw new Exception(
                        "اتصال به دیتابیس برقرار نشد. " +
                        "فایل connection.txt و وضعیت SQL Server را بررسی کنید."
                    );
                }
            }
        }

        /// <summary>
        /// بستن و آزادسازی اتصال مرکزی دیتابیس.
        /// مناسب برای زمان خروج از برنامه.
        /// </summary>
        public void CloseConnection()
        {
            if (connection == null)
                return;

            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            finally
            {
                connection.Dispose();
                connection = null;
            }
        }

        #endregion

        #region Read - SELECT

        /// <summary>
        /// اجرای SELECT و بازگرداندن لیستی از نتایج.
        /// </summary>
        public List<T> Query<T>(string sqlQuery, object parameters = null)
        {
            EnsureConnection();
            Debug.WriteLine(sqlQuery);
            Debug.WriteLine(parameters);
            return connection
                .Query<T>(sqlQuery, parameters)
                .ToList();
        }

        #endregion

        #region Create - INSERT

        /// <summary>
        /// اجرای یک دستور SQL بدون خروجی خاص.
        /// قابل استفاده برای INSERT، UPDATE و DELETE.
        /// خروجی: تعداد ردیف‌های تحت تأثیر.
        /// </summary>
        public int Insert(string insertCommand, object parameters = null)
        {
            EnsureConnection();

            return connection.Execute(insertCommand, parameters);
        }

        /// <summary>
        /// اجرای چند دستور SQL در یک Transaction واحد.
        ///
        /// ترتیب خروجی‌ها دقیقاً مانند ترتیب دستورات ورودی است:
        /// - HasOutput = true  => اولین مقدار خروجی SQL
        /// - HasOutput = false => null
        ///
        /// در صورت خطای هر دستور، تمام عملیات Rollback می‌شوند.
        /// </summary>
        public List<object> Insert(List<DatabaseInsertCommand> insertCommands)
        {
            if (insertCommands == null || insertCommands.Count == 0)
            {
                return new List<object>();
            }

            EnsureConnection();

            List<object> results = new List<object>();

            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < insertCommands.Count; i++)
                    {
                        DatabaseInsertCommand command = insertCommands[i];

                        if (command == null)
                        {
                            // حفظ ترتیب ورودی‌ها
                            results.Add(null);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(command.SqlScript))
                        {
                            throw new ArgumentException(
                                $"متن SQL دستور شماره {i + 1} خالی است."
                            );
                        }

                        try
                        {
                            if (command.HasOutput)
                            {
                                object result = connection.ExecuteScalar(
                                    command.SqlScript,
                                    command.Parameters,
                                    transaction: transaction
                                );

                                results.Add(result);
                            }
                            else
                            {
                                connection.Execute(
                                    command.SqlScript,
                                    command.Parameters,
                                    transaction: transaction
                                );

                                results.Add(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("========== Database Batch Error ==========");
                            Debug.WriteLine($"Command Index: {i}");
                            Debug.WriteLine($"HasOutput: {command.HasOutput}");
                            Debug.WriteLine("SQL:");
                            Debug.WriteLine(command.SqlScript);
                            Debug.WriteLine("Error:");
                            Debug.WriteLine(ex.ToString());

                            throw new Exception(
                                $"خطا در اجرای دستور دیتابیس شماره {i + 1}.{Environment.NewLine}" +
                                $"متن خطا: {ex.Message}{Environment.NewLine}{Environment.NewLine}" +
                                $"SQL:{Environment.NewLine}{command.SqlScript}",
                                ex
                            );
                        }
                    }

                    transaction.Commit();

                    return results;
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // اگر خود Rollback هم خطا داشت،
                        // خطای اصلی را پنهان نمی‌کنیم.
                    }

                    throw;
                }
            }
        }

        #endregion

        #region Update - UPDATE

        /// <summary>
        /// اجرای دستور UPDATE.
        /// </summary>
        public int Update(string updateCommand, object parameters = null)
        {
            EnsureConnection();

            return connection.Execute(updateCommand, parameters);
        }

        #endregion

        #region Delete - DELETE

        /// <summary>
        /// اجرای دستور DELETE.
        /// </summary>
        public int Delete(string deleteCommand, object parameters = null)
        {
            EnsureConnection();

            return connection.Execute(deleteCommand, parameters);
        }

        #endregion
    }

    /// <summary>
    /// مشخصات یک دستور SQL در عملیات گروهی.
    /// با وجود نام فعلی، این کلاس می‌تواند INSERT، UPDATE یا DELETE باشد.
    /// </summary>
    public class DatabaseInsertCommand
    {
        /// <summary>
        /// متن SQL دستور.
        /// </summary>
        public string SqlScript { get; set; }

        /// <summary>
        /// پارامترهای Dapper.
        /// </summary>
        public object Parameters { get; set; }

        /// <summary>
        /// اگر true باشد، دستور SQL باید خروجی داشته باشد
        /// (مثل SELECT یا OUTPUT INSERTED.Id).
        /// </summary>
        public bool HasOutput { get; set; }
    }
}

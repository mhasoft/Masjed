using System;
using System.Collections.Generic;
using System.Linq;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Select.getMahaleAllRecords
{
    public static class srvGetMahaleAllRecords
    {
        public static Dictionary<string, Mahale> Execute()
        {
            try
            {
                string query = $@"
                    SELECT
                        {srvEntityTableName.AllFields()}
                    FROM {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    ORDER BY {srvEntityTableName.FieldName<Mahale>(x => x.Id)};";

                var mahaleList = Program.Database
                    .Query<Mahale>(query)
                    .ToList();

                return mahaleList
                    .Where(x => x != null)
                    .ToDictionary(
                        x => x.Id.ToString(),
                        x => x
                    );
            }
            catch (Exception ex)
            {

                Console.WriteLine($"SQL Error in srvGetMahaleAllRecords: {ex.Message}");
                throw;
            }
        }
    }
}

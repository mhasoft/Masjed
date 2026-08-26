using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Select.getAmakenAllRecords
{
    public static class srvGetAmakenAllRecords
    {
        public static Dictionary<string, Amaken> Execute()
        {
            try
            {


                string query = $@"
                                    SELECT
                                        {srvEntityTableName.AllFields()}
                                    FROM {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Amaken>()}
                                    ORDER BY {srvEntityTableName.FieldName<Amaken>(x => x.Id)};";

                var amakenList = Program.Database
                    .Query<Amaken>(query)
                    .ToList();

                return amakenList
                    .Where(x => x != null)
                    .ToDictionary(
                        x => x.Id.ToString(),
                        x => x
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL Error in srvGetAmakenAllRecords: {ex.Message}");
                throw;
            }
        }
    }
}

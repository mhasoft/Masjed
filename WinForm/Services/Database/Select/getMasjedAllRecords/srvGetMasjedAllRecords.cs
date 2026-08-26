using System;
using System.Collections.Generic;
using System.Linq;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Select.getMasjedAllRecords
{
    public class srvGetMasjedAllRecords
    {
        public static Dictionary<string, Masjed> Execute()
        {
            try
            {
                string query = $@"
                    SELECT
                        {srvEntityTableName.AllFields()}
                    FROM {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Masjed>()}
                    ORDER BY {srvEntityTableName.FieldName<Masjed>(x => x.Id)};";

                var masjedList = Program.Database
                    .Query<Masjed>(query)
                    .ToList();

                return masjedList
                    .Where(x => x != null)
                    .ToDictionary(
                        x => x.Id.ToString(),
                        x => x
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL Error in srvGetMasjedAllRecords: {ex.Message}");
                throw;
            }
        }
    }
}

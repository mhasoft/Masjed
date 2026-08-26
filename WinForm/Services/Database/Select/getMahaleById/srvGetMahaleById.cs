using System;
using WinForm.Models.Datas;
using WinForm.Models.Entities;
using WinForm.Services.Utility.Databse;

namespace WinForm.Services.Database.Select.getMahaleById
{
    public static class srvGetMahaleById
    {
        public static Mahale? Execute(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null;
                }


                string query = $@"
                    SELECT
                        {srvEntityTableName.AllFields()}
                    FROM
                        {AppInformation.DatabasePrefix}.{srvEntityTableName.TableName<Mahale>()}
                    WHERE
                        {srvEntityTableName.FieldName<Mahale>(x => x.Id)} = @Id;";


                Mahale? mahale =
                    Program.Database
                        .Query<Mahale>(
                            query,
                            new
                            {
                                Id = id
                            })
                        .FirstOrDefault();


                return mahale;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SQL Error in srvGetMahaleById: {ex.Message}"
                );

                throw;
            }
        }
    }
}
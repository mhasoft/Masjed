using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace WinForm.Services.Database
{
    public static class TableNameHelper
    {
        public static string GetTableName<T>()
        {
            TableAttribute? attr = typeof(T).GetCustomAttribute<TableAttribute>();
            return attr?.Name ?? typeof(T).Name;
        }
    }
}

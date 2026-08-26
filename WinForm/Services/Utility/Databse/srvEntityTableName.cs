using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace WinForm.Services.Utility.Databse
{
    public static class srvEntityTableName
    {
        /// <summary>
        /// نام جدول دیتابیس را از Attribute [Table("...")] دریافت می‌کند.
        /// مثال: TableName<Amaken>() -> "TblAmaken"
        /// </summary>
        public static string TableName<T>()
        {
            return TableName(typeof(T));
        }

        /// <summary>
        /// نام جدول دیتابیس را از Attribute [Table("...")] نوع داده دریافت می‌کند.
        /// </summary>
        public static string TableName(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
            {
                throw new InvalidOperationException(
                    $"کلاس '{entityType.FullName}' دارای Attribute از نوع [Table(...)] نیست.");
            }

            return tableAttribute.Name;
        }

        /// <summary>
        /// نام فیلد را به صورت Entity.Property برمی‌گرداند.
        /// مثال:
        /// FieldName<Amaken>(x => x.Id)   -> "Amaken.Id"
        /// FieldName<Amaken>(x => x.Name) -> "Amaken.Name"
        /// </summary>
        public static string FieldName<T>(
            Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            MemberExpression? memberExpression = propertyExpression.Body switch
            {
                MemberExpression member => member,

                // برای Propertyهای Value Type مثل int، long، bool و ...
                UnaryExpression
                {
                    Operand: MemberExpression member
                } => member,

                _ => null
            };

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    $"Expression باید یک Property از نوع '{typeof(T).Name}' باشد.",
                    nameof(propertyExpression));
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException(
                    $"'{memberExpression.Member.Name}' یک Property نیست.",
                    nameof(propertyExpression));
            }

            return $"{typeof(T).Name}.{propertyInfo.Name}";
        }

        /// <summary>
        /// تمام فیلدها را برمی‌گرداند.
        /// </summary>
        public static string AllFields()
        {
            return "*";
        }
    }
}
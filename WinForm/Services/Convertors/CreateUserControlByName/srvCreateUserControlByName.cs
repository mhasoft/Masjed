using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WinForm.Services.Convertors.CreateUserControlByName
{
    public class srvCreateUserControlByName
    {


        /// <summary>
        /// نمونه‌سازی پویا از یک UserControl با استفاده از نام کلاس آن به صورت رشته متنی.
        /// </summary>
        /// <param name="controlClassName">نام کلاس کنترل (بدون پسوند .cs و بدون نیاز به ذکر حتمی Namespace)</param>
        /// <returns>یک نمونه جدید از UserControl یا null در صورت عدم یافتن نوع مشخص شده</returns>
        public static UserControl Execute(string controlClassName)
        {
            if (string.IsNullOrWhiteSpace(controlClassName))
            {
                throw new ArgumentException("نام کنترل نمی‌تواند خالی باشد.", nameof(controlClassName));
            }

            // تمیز کردن نام در صورتی که پسوند فایل همراه آن فرستاده شده باشد
            if (controlClassName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                controlClassName = controlClassName.Substring(0, controlClassName.Length - 3);
            }

            Type targetType = null;

            // ۱. ابتدا جستجو در اسمبلی جاری
            var executingAssembly = Assembly.GetExecutingAssembly();
            targetType = FindTypeInAssembly(executingAssembly, controlClassName);

            // ۲. اگر در اسمبلی جاری پیدا نشد، جستجو در تمام اسمبلی‌های لود شده در دامنه‌ی برنامه (AppDomain)
            if (targetType == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    targetType = FindTypeInAssembly(assembly, controlClassName);
                    if (targetType != null)
                    {
                        break;
                    }
                }
            }

            // ۳. در صورتی که کلاس مورد نظر پیدا شد، نمونه‌سازی انجام می‌شود
            if (targetType != null)
            {
                try
                {
                    // ساخت شیء به صورت پویا با استفاده از کلاس Activator
                    object instance = Activator.CreateInstance(targetType);
                    return instance as UserControl;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"خطا در نمونه‌سازی کنترل '{controlClassName}': {ex.Message}", ex);
                }
            }

            // اگر هیچ کلاسی با این مشخصات یافت نشد
            return null;
        }

        /// <summary>
        /// متد کمکی برای یافتن نوع داده‌ای (Type) درون یک اسمبلی مشخص بر اساس نام کلاس و ارث‌بری از UserControl
        /// </summary>
        private static Type FindTypeInAssembly(Assembly assembly, string className)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    // بررسی مطابقت نام کلاس و اینکه حتماً از UserControl ارث‌بری کرده باشد
                    if (type.Name.Equals(className, StringComparison.OrdinalIgnoreCase) &&
                        typeof(UserControl).IsAssignableFrom(type))
                    {
                        return type;
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // در صورت لود نشدن برخی انواع در اسمبلی، نادیده گرفته می‌شود
            }
            return null;
        }


    }
}

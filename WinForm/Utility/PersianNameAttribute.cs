using System;

namespace WinForm.Utility
{
    // اضافه کردن Property به Targetها
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PersianNameAttribute : Attribute
    {
        public string Name { get; }
        public PersianNameAttribute(string name)
        {
            Name = name;
        }
    }
}

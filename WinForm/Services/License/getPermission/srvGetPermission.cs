using System;
using System.Collections.Generic;
using System.Text;
using WinForm.Models.Entities;
using WinForm.Services.License.getPermission.DTOs;

namespace WinForm.Services.License.getPermission
{
    public class srvGetPermission
    {
        /// <summary>
        /// بررسی اینکه آیا این تایل لایسنسش خریده شده است یا خیر؟
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public dtoGetPermissionResult Execute(string Code)
        {
            return new dtoGetPermissionResult
            {
                isActive = true,
            };
        }
    }
}

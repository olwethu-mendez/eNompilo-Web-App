﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace eNompilo.v3._0._1.Constants
{
    public static class EnumExtensions
    {
        //CALLING THIS ALLOWS YOU TO DISPLAY THE SPECIFIED DISPLAY NAME ON YOUR ENUM INSTEAD OF ITS DEFAULT NAME!
        public static string? GetDisplayName(this Enum enumValue)
        {
            if (enumValue == null)
                return "";

            try
            {
                var returnValue = enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
                return returnValue;
            }
            catch (NullReferenceException)
            {
                return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First().Name.ToString();
            }
        }
    }
}

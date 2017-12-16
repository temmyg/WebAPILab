using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebAPIComprehensive.Attributes
{
    public class HasProvinceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string homeTown = (string)value;
            if (HomeProvince.FindProvince(homeTown) == null)
                return new ValidationResult("Hometown Not in Australia");
            else
                return ValidationResult.Success;
        }
    }

    struct HomeProvince{
        public static string FindProvince(string hometown)
        {
            if (hometown.IndexOf("NSW")!=-1)
                return "New South Wales";
            else
                return null;
        }
    }
}
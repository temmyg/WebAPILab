using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using WebAPIComprehensive.Models;

namespace WebAPIComprehensive.Attributes
{
    public class NameDifferenceAttribute : ValidationAttribute 
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Author auth = (Author)value;
            if (auth.FirstName == auth.LastName)
            {
                return new ValidationResult("FirstName and LastName equality error");
            }
            else
                return ValidationResult.Success;
        }
    }
}
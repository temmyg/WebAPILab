using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPIComprehensive.Attributes;

namespace WebAPIComprehensive.Models
{
    [NameDifference]
    public partial class Author
    {
        // public string Industry { get; set; }
        [HasProvince]
        public string HomeTown { get; set; }
        public Author(string Name)
        {

        }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAPIComprehensive.Models
{
    using System;
    using System.Collections.Generic;
    
    //using System.Runtime.Serialization;
    
    //[DataContract(IsReference=true)]
    public partial class Author
    {
        public Author()
        {
            this.Blogs = new HashSet<Blog>();
        }
    
    	
        //[DataMember]
        public string LastName { get; set; }
    	
        //[DataMember]
        public long Id { get; set; }
    	
        //[DataMember]
        public Nullable<int> Age { get; set; }
    	
        //[DataMember]
        public string FirstName { get; set; }
    	
        //[DataMember]
        public string Industry { get; set; }
    	
        //[DataMember]
        public Nullable<System.DateTime> RegisteredTime { get; set; }
    
    	//[DataMember]
        public virtual ICollection<Blog> Blogs { get; set; }
    }
}

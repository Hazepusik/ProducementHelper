//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ContosoUniversity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tender
    {
        public Tender()
        {
            this.Bid = new HashSet<Bid>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<double> minPrice { get; set; }
        public Nullable<double> maxPrice { get; set; }
    
        public virtual ICollection<Bid> Bid { get; set; }
    }
}

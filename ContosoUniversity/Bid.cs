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
    
    public partial class Bid
    {
        public int id { get; set; }
        public int tenderId { get; set; }
        public int propertyId { get; set; }
        public int participantId { get; set; }
        public double value { get; set; }
    
        public virtual Participant Participant { get; set; }
        public virtual Property Property { get; set; }
        public virtual Tender Tender { get; set; }
    }
}

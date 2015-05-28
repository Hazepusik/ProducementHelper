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
    
    public partial class Property
    {
        public Property()
        {
            this.DefaultProperty = new HashSet<DefaultProperty>();
            this.Property1 = new HashSet<Property>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int importance { get; set; }
        public Nullable<int> isSubpropertyOf { get; set; }
        public Nullable<double> minValue { get; set; }
        public Nullable<double> maxValue { get; set; }
        public Nullable<double> step { get; set; }
        public bool toMax { get; set; }
        public Nullable<bool> isDefault { get; set; }
        public int functionId { get; set; }
        public Nullable<bool> isPrice { get; set; }
    
        public virtual ICollection<DefaultProperty> DefaultProperty { get; set; }
        public virtual Function Function { get; set; }
        public virtual ICollection<Property> Property1 { get; set; }
        public virtual Property Property2 { get; set; }
    }
}

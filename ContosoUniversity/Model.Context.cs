﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProcurementEntities : DbContext
    {
        public ProcurementEntities()
            : base("name=ProcurementEntities")
        {
            this.Configuration.ProxyCreationEnabled = false; 
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bid> Bid { get; set; }
        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<Property> Property { get; set; }
        public virtual DbSet<Tender> Tender { get; set; }
    }
}

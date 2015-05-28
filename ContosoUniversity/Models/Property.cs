using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System;

namespace ContosoUniversity
{
    [MetadataType(typeof(PropertyMetaData))]
    public partial class Property
    {

        public static Property byId(int id)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Property.FirstOrDefault(x => x.id == id);
            }
        }

        public static List<Property> byTender(int tId)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Property.ToList();
            }
        }

        public static List<Property> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Property.ToList();
            }
        }

        public Property Clone()
        {
            Property property = new Property();
            property.functionId = this.functionId;
            property.importance = this.importance;
            property.isPrice = this.isPrice;
            property.isDefault = false;
            property.isSubpropertyOf = null;
            property.maxValue = this.maxValue;
            property.minValue = this.minValue;
            property.name = this.name;
            property.step = this.step;
            property.toMax = this.toMax;
            using (ProcurementEntities db = new ProcurementEntities())
            {
                db.Property.Add(property);
                db.SaveChanges();
            }
            return property;
        }
    }

    public class PropertyMetaData
    {
        [Display(Name = "Важность")]
        public int importance { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string name { get; set; }

        [Display(Name = "Минимальное значение критерия")]
        public Nullable<double> minValue { get; set; }

        [Display(Name = "Максимальное значение критерия")]
        public Nullable<double> maxValue { get; set; }

        [Display(Name = "Шаг изменения значений критерия")]
        public Nullable<double> step { get; set; }

        [Display(Name = "Лучше большее значение критерия?")]
        public Nullable<bool> toMax { get; set; }

        [Display(Name = "Критерий по умолчанию")]
        public Nullable<bool> isDefault { get; set; }
    }



}
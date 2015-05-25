using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

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

        public static List<Property> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Property.ToList();
            }
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
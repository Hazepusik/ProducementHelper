using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;

namespace ContosoUniversity
{
    [MetadataType(typeof(TenderMetaData))]
    public partial class Tender
    {
        public static Tender byId(int id)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Tender.FirstOrDefault(x => x.id == id);
            }
        }

        public static List<Tender> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Tender.ToList();
            }
        }
    }

    public class TenderMetaData
    {
        [Required]
        [Display(Name = "Имя")]
        public string name { get; set; }

        [Display(Name = "Подробная информация")]
        public string description { get; set; }

        [Display(Name = "Минимальная ставка")]
        public Nullable<double> minPrice { get; set; }

        [Display(Name = "Максимальная ставка")]
        public Nullable<double> maxPrice { get; set; }
    }


}
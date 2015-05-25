using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity
{
    [MetadataType(typeof(FunctionMetaData))]
    public partial class Function
    {

        public static Function byId(int id)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Function.FirstOrDefault(x => x.id == id);
            }
        }

        public static List<Function> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Function.ToList();
            }
        }
    }

    public class FunctionMetaData
    {

        [Required]
        [Display(Name = "Имя функции")]
        public string name { get; set; }

    }



}
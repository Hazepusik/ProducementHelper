using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

        public static List<SelectListItem> ToSelectList(int selected)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Function f in QueryAll())
            {
                list.Add(new SelectListItem
                {
                    Value = f.id.ToString(),
                    Text = f.name,
                    Selected = f.id == selected
                });
            }
            return list;
        }
    }

    public class FunctionMetaData
    {

        [Required]
        [Display(Name = "Имя функции")]
        public string name { get; set; }

    }



}
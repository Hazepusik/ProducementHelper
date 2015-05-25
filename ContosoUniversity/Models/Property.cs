using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity
{
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



}
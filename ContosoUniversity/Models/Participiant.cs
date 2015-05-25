using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity
{
    public partial class Participant
    {
        public static Participant byId(int id)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Participant.FirstOrDefault(x => x.id == id);
            }
        }

        public static List<Participant> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Participant.ToList();
            }
        }
    }



}
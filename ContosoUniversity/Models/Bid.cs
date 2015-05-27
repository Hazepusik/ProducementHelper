using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity
{
    [MetadataType(typeof(BidMetaData))]
    public partial class Bid
    {
        public static Bid byId(int id)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Bid.FirstOrDefault(x => x.id == id);
            }
        }

        public static List<Bid> QueryAll()
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                return db.Bid.ToList();
            }
        }
    }

    public class BidMetaData
    {
        [Required]
        [Display(Name = "Valllll")]
        public string value { get; set; }
    }

    public class BidContainer
    {
        public List<string> participants = null;
        public List<string> properties = null;
        public Dictionary<string, Dictionary<string, Bid>> bid = null;
        List<Bid> bids = null;
        public BidContainer(int? _tenderId)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                int tenderId = _tenderId ?? 9; // TODO: fixit
                this.bids = Bid.QueryAll().Where(b => b.tenderId == tenderId).ToList();
                List<int> propertyIds = bids.Where(b => b.propertyId != null).Select(b => b.propertyId.Value).Distinct().ToList() ?? new List<int>();
                this.properties = db.Property.Where(p => propertyIds.Contains(p.id)).Select(p => p.name).ToList();
                List<int> participantIds = bids.Select(b => b.participantId).Distinct().ToList() ?? new List<int>();
                this.participants = db.Participant.Where(p => participantIds.Contains(p.id)).Select(p => p.name).ToList();
                bid = new Dictionary<string, Dictionary<string, Bid>>();
                foreach (int pt in participantIds)
                {
                    Dictionary<string, Bid> prtyDict = new Dictionary<string,Bid>();
                    foreach (int py in propertyIds)
                        prtyDict[Property.byId(py).name] = bids.Where(b => b.propertyId != null).First(b => b.participantId == pt && b.propertyId.Value == py);
                    bid[Participant.byId(pt).name] = prtyDict;
                }
                    
            }


        }
    }

}
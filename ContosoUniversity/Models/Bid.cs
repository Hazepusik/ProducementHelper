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
        public string defaultValue { get; set; }
    }

    public class BidContainer
    {
        //public List<string> participants = null;
        //public List<string> properties = null;
        public Dictionary<int, Dictionary<int, Bid>> bid = null;
        List<Bid> bids = null;
        public int tenderId = 0;
        public List<int> propertyIds;
        public List<int> participantIds; 

        public BidContainer() { }
        public BidContainer(int? _tenderId)
        {
            using (ProcurementEntities db = new ProcurementEntities())
            {
                this.tenderId = _tenderId ?? 0;
                this.bids = Bid.QueryAll().Where(b => b.tenderId == tenderId).ToList();
                propertyIds = bids.Where(b => b.propertyId != null).Select(b => b.propertyId.Value).Distinct().ToList() ?? new List<int>();
                //this.properties = db.Property.Where(p => propertyIds.Contains(p.id)).Select(p => p.name).ToList();
                participantIds = bids.Select(b => b.participantId).Distinct().ToList() ?? new List<int>();
                //this.participants = db.Participant.Where(p => participantIds.Contains(p.id)).Select(p => p.name).ToList();
                bid = new Dictionary<int, Dictionary<int, Bid>>();
                foreach (int pt in participantIds)
                {
                    Dictionary<int, Bid> prtyDict = new Dictionary<int, Bid>();
                    foreach (int py in propertyIds)
                        prtyDict[py] = bids.Where(b => b.propertyId != null).First(b => b.participantId == pt && b.propertyId.Value == py);
                    bid[pt] = prtyDict;
                }
                    
            }


        }
    }

}
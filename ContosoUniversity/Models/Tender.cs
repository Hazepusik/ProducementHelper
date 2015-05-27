using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;

namespace ContosoUniversity
{
    [MetadataType(typeof(TenderMetaData))]
    public partial class Tender
    {
        private List<int> _participantIds = null;
        public List<int> participantIds
        {
            get
            {
                if (_participantIds == null)
                {
                    using (ProcurementEntities db = new ProcurementEntities())
                    {
                        _participantIds = db.Bid.Where(b => b.tenderId == this.id).Select(b => b.participantId).Distinct().ToList() ?? new List<int>();
                    }
                }
                return _participantIds;
            }
            set
            {
                this._participantIds = value;
            }
        }
        [Display(Name = "Уастники торгов")]
        public IEnumerable<SelectListItem> participants
        {
            get
            {
                return Participant.QueryAll()
                    .Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name, }).ToList();
            }
            set { }
        }

        private List<int> _propertyIds = null;
        public List<int> propertyIds
        {
            get
            {
                if (_propertyIds == null)
                {
                    using (ProcurementEntities db = new ProcurementEntities())
                    {
                        _propertyIds = db.Bid.Where(b => b.tenderId == this.id && b.propertyId != null).Select(b => b.propertyId.Value).Distinct().ToList() ?? new List<int>();
                    }
                }
                return _propertyIds;
            }
            set
            {
                this._propertyIds = value;
            }
        }

        [Display(Name = "Критерии торгов")]
        public IEnumerable<SelectListItem> properties
        {
            get
            {
                return Property.QueryAll().Where(p => p.isDefault ?? false)
                    .Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name, }).ToList();
            }
            set { }
        }

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
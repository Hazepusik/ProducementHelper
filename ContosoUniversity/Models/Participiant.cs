﻿using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity
{
    [MetadataType(typeof(ParticipantMetaData))]
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

    public class ParticipantMetaData
    {
        [Required]
        [Display(Name = "Имя")]
        public string name { get; set; }
    }

}
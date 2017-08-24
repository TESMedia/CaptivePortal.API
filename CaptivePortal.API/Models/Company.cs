using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        // Foreign key 
        public  int ? OrganisationId { get; set; }
        public string CompanyIcon { get; set; }


        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
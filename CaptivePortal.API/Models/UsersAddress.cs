using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class UsersAddress
    {
        
            [Key]
            public int AddressId { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string PostTown { get; set; }
            public string County { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
            public string Notes { get; set; }

            // Foreign key 
            public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual WifiUser WifiUsers { get; set; }

    }
}
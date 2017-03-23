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
            public string Addresses { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string Zip { get; set; }
            public string Notes { get; set; }

            // Foreign key 
            public int UserId { get; set; }

            [ForeignKey("UserId")]
            public virtual Users Users { get; set; }
      
    }
}
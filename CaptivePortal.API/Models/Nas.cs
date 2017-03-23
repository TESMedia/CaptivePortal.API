using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Nas
    {
        [Key]
        public int NasId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(128)]
        public string Nasname { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string Shortname { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string Type { get; set; }

        public int Ports { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(60)]
        public string Secret { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string Server { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Community { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(500)]
        public string Description { get; set; }
    }
}
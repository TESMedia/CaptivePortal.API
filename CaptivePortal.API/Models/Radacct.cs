using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Radacct
    {
        [Key]
        public long RadacctId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string AcctsessionId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string AcctuniqueId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string UserName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string GroupName { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string NasipAddress { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(15)]
        public string NasportId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string NasPortType { get; set; }

        public string AcctstopTime { get; set; }
        public string AcctstartTime { get; set; }

        public int AcctsessionTime { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string Acctauthentic { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Connectinfo_Start { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string Connectinfo_Stop { get; set; }
        public long AcctinputOctets { get; set; }
        public long AcctoutputOctets { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string CalledstationId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string CallingstationId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string AcctterminateCause { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string ServiceType { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string FramedProtocol { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string FramedIpAddress { get; set; }

        public int AcctStartDelay { get; set; }

        public int AcctStopDelay { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string XascendSessionsvrKey { get; set; }

    }
}
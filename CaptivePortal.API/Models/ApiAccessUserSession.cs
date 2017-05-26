using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptivePortal.API.Models
{
    public class ApiAccessUserSession
    {

        [Key]
        public int UserSessionId { get; set; }
        public int UserId { get; set; }
        public string SessionId { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }
    }
}
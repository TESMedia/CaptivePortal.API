using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class ManagePromotion
    {
        [Key]
        public int ManagePromotionId { get; set; }

        //Foreign key
        public int SiteId { get; set; }

        public string SuccessPageOption { get; set; }
        public string WebPageURL { get; set; }
        public string OptionalPictureForSuccessPage { get; set; }

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
    }
}
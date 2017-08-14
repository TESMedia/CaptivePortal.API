using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class LocationIndicatorViewModel
    {
        public int AreaOfInterestId { get; set; }
        [Required(ErrorMessage = "Enter the Area Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter the LocationIndicator Name")]
        public string LoctionIndicator { get; set; }
        public string NeighBourName { get; set; }
    }
}
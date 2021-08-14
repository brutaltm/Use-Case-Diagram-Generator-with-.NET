using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.Models
{
    public abstract class RegularFields
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }
    }
}
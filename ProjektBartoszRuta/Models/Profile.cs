using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.Models
{
    public class Profile
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Username")]
        [StringLength(30, ErrorMessage = "Username cannot be longer than 30 characters.")]
        public string UserName { get; set; }
        public string Image { get; set; }
        public virtual List<UseCaseDiagram> UseCaseDiagrams { get; set; }
    }
}
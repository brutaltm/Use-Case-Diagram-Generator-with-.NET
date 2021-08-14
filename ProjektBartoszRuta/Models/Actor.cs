using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.Models
{
    public class Actor : RegularFields
    {
        //public int ID { get; set; }
        [Required]
        [Display(Name = "Diagram")]
        public int UseCaseDiagramID { get; set; }
        //[Required]
        //[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        //public string Name { get; set; }
        [Display(Name = "Diagram")]
        public virtual UseCaseDiagram UseCaseDiagram { get; set; }
        public virtual ICollection<UseCaseActorJoin> UseCaseActorJoins { get; set; }
    }
}
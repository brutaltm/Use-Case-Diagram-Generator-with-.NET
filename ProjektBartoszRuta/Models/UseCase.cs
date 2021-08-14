using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.Models
{
    public class UseCase : RegularFields
    {
        //public int ID { get; set; }
        //[Required]
        [Display(Name = "Diagram")]
        public int? UseCaseDiagramID { get; set; }
        //[Required]
        //[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        //public string Name { get; set; }
        [StringLength(150, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string Description { get; set; }
        [Display(Name = "Diagram")]
        public virtual UseCaseDiagram UseCaseDiagram { get; set; }
        public virtual ICollection<UseCaseActorJoin> UseCaseActorJoins { get; set; }
    }
}
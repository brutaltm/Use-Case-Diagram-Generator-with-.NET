using System.ComponentModel.DataAnnotations;

namespace ProjektBartoszRuta.Models
{
    public class UseCaseActorJoin
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Actor")]
        public int ActorID { get; set; }
        [Required]
        [Display(Name = "Use Case")]
        public int UseCaseID { get; set; }
        public virtual Actor Actor { get; set; }
        public virtual UseCase UseCase { get; set; }
    }
}
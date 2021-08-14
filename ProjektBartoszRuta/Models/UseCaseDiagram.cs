using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ProjektBartoszRuta.Models
{
    public delegate void DiagramDelegate(UseCaseDiagram ucd);
    public class UseCaseDiagram : RegularFields
    {
        public event DiagramDelegate OnDescriptionChanged, OnDescriptionChanging;
        //public int ID { set; get; }
        [Required]
        [Display(Name = "Author")]
        public int ProfileID { set; get; }
        //[Required]
        //[Column("Name")]
        //[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        //public string Name { set; get; }
        private string description;
        [StringLength(150, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string Description { 
            get
            {
                return description;
            }
            set
            {
                OnDescriptionChanging?.Invoke(this);
                description = value;
                OnDescriptionChanged?.Invoke(this);
            }
        }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created")]
        public DateTime? CreatedAt { set; get; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Modified")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Actors")]
        public int ActorCount
        {
            get { return Actors == null ? 0 : Actors.Count; }
        }

        [Display(Name = "Use Cases")]
        public int UseCaseCount
        {
            get { return UseCases == null ? 0 : UseCases.Count; }
        }

        public bool IsGenerated { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual ICollection<Actor> Actors { get; set; }
        public virtual ICollection<UseCase> UseCases { get; set; }

    }
}
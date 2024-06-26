﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
namespace SafeWheel3.Models
{
    public class ApplicationUser : IdentityUser 
    {
        //public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Anunt>? Anunturi{ get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }


        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        //public virtual ICollection<Anunt>? AnunturiFav { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public virtual List<Plata> Plati { get; set; }


        public int Tokens { get; set; } = 100;
     }
}

﻿using System.ComponentModel.DataAnnotations;

namespace SafeWheel3.Models
{
    public class Comment
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? AnuntId { get; set; }

        public string? UserId { get; set; }
       public string? UserName { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

        public virtual Anunt? Anunt { get; set; }
    }
}

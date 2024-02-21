using SafeWheel3.Models;
using SafeWheel3.Models;
using System.ComponentModel.DataAnnotations;

namespace SafeWheel3.Models
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele dealer-ului este obligatoriu.")]
        public string Nume { get; set; }

        public virtual ICollection<Anunt>? Cars { get; set; }
    }
}

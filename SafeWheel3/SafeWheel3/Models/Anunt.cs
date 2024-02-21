
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SafeWheel3.Models;

public class Anunt
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Marca mașinii este obligatorie")]
    [MaxLength(100, ErrorMessage = "Marca nu poate avea mai mult de 100 de caractere.")]
    public string Marca { get; set; }


    [Required(ErrorMessage = "Pretul este obligatoriu.")]

    [Range(1, 10000000000, ErrorMessage = "Pretul trebuie sa fie mai mare ca 0.")]
    public int Pret { get; set; }


    [Required(ErrorMessage = " Data fabricației este obligatorie.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? DataFabricatiei { get; set; }



    [Required(ErrorMessage = "Dealer-ul trebuie selectat")]

    public int? DealerId { get; set; }

    public virtual Marca? Dealer { get; set; }

    [NotMapped]
    public IEnumerable<SelectListItem>? Dealers
    {
        get; set;
    }

    public virtual ApplicationUser User { get; set; }

    public string Image { get; set; }

    public string Description { get; set; }
}



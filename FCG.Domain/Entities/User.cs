using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FCG.Domain.Entities;

public abstract class User
{
    [Required]
    [DisplayName("Data de Nascimento")]
    public DateTime DateOfBirth{ get; set; }
}
using System.ComponentModel.DataAnnotations;
using Backend.Models.Domain;

namespace Backend.Models.DTOs;

public class MoveCardRequest
{
    [Required]
    public required Card SelectedCard { get; set; }

    [Required]
    public required Pile SourcePile { get; set; }

    [Required]
    public required TableauPile TargetPile { get; set; }
}

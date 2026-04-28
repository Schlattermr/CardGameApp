using Backend.Models.Domain;

namespace Backend.Models.DTOs;

public class MoveCardResponse
{
    public required TableauPile Tableau1 { get; set; }

    public required TableauPile Tableau2 { get; set; }

    public required TableauPile Tableau3 { get; set; }

    public required TableauPile Tableau4 { get; set; }

    public required TableauPile Tableau5 { get; set; }

    public required TableauPile Tableau6 { get; set; }

    public required TableauPile Tableau7 { get; set; }

    public required FoundationPile FoundationClubs { get; set; }

    public required FoundationPile FoundationDiamonds { get; set; }

    public required FoundationPile FoundationHearts { get; set; }

    public required FoundationPile FoundationSpades { get; set; }

    public required Pile Stock { get; set; }

    public required Pile Discard { get; set; }
}

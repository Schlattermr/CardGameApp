using Backend.Models.Enums;

namespace Backend.Models.Domain;

/// <summary>
/// Foundation pile index increments starting from Ace, opposite of tableau pile
/// 0: Ace
/// 1: 1
/// 2: 2
/// etc.
/// </summary>
public class FoundationPile : Pile, IFoundationPile
{
    public Suit acceptedSuit { get; set; }

    public bool IsComplete()
    {
        return Cards.Count == 13;
    }
}

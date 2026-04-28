namespace Backend.Models.Domain;

public interface ICard
{
    /// <summary>
    /// Returns value of card
    /// </summary>
    int GetCardValue();

    /// <summary>
    /// Flips card over
    /// </summary>
    void FlipCard();

    /// <summary>
    /// Determines if card is spades or clubs or neither
    /// </summary>
    bool IsBlack();
}

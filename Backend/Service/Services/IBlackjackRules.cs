using Backend.Models.Domain;

namespace Backend.Services;

public interface IBlackjackRules
{
    Deck Deck { get; }

    List<Card> PlayerHand { get; }

    List<Card> DealerHand { get; }

    bool RoundOver { get; }

    /// <summary>
    /// Shuffles a fresh deck and deals the opening two cards to the player and dealer,
    /// with the dealer's second card dealt face down
    /// </summary>
    void StartRound();

    /// <summary>
    /// Draws a card into the player's hand, ending the round if it results in a bust
    /// </summary>
    void Hit();

    /// <summary>
    /// Reveals the dealer's hidden card and draws until the dealer's hand is 17 or higher
    /// </summary>
    void Stand();

    /// <summary>
    /// Describes the outcome of the round; only meaningful once RoundOver is true
    /// </summary>
    string GetResult();
}

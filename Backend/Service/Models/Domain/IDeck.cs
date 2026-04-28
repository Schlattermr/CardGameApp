using Backend.Models.Enums;

namespace Backend.Models.Domain;

public interface IDeck
{
    /// <summary>
    /// Creates deck based on the game that it will be used for
    /// </summary>
    List<Card> CreateDeck(GameType game);

    /// <summary>
    /// Counts jokers in a deck
    /// </summary>
    int CountJokers(List<Card> deck);

    /// <summary>
    /// Adds cards to a users hand
    /// </summary>
    List<Card> AddCardToDeck(List<Card> roundCards);

    /// <summary>
    /// Pulls top card from the deck
    /// </summary>
    Card PullTopCard();

    /// <summary>
    /// Shuffles cards in the deck
    /// </summary>
    List<Card> Shuffle(List<Card> deck);
}

namespace Engines;

public interface IDeck
{
    /*
     *  Creates deck based on the game that it will be used for
     */
    List<Card> CreateDeck(GameType game);

    /*
     *  Counts jokers in a deck
     */
    int CountJokers(List<Card> deck);

    /*
     *  Adds cards to a users hand
     */
    List<Card> AddCardToDeck(List<Card> roundCards);

    /*
     *  Pulls top card from the deck
     */
    Card PullTopCard();

    /*
     *  Shuffles cards in the deck
     */
    List<Card> Shuffle(List<Card> deck);
}
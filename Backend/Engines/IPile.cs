namespace Engines;

public interface IPile
{
    /// <summary>
    /// Checks size of pile for win conditions and interactability 
    /// </summary>
    /// <returns>Current size of a pile</returns>
    int Count();


    /// <summary>
    /// Returns properties of first card and helps determine valid moves
    /// </summary>
    /// <returns>First card of a pile, null otherwise</returns>
    Card? TopCard();

    /// <summary>
    /// Returns properties of last card and helps determine valid moves
    /// </summary>
    /// <returns>Last card of a pile, null otherwise</returns>
    Card? LastCard();

    /// <summary>
    /// Checks if a pile is empty
    /// </summary>
    bool IsEmpty();

    /// <summary>
    /// Finds the index of a given card in a pile
    /// </summary>
    /// <returns> Index of a card, -1 otherwise</returns>
    int IndexCard(Card card);
}

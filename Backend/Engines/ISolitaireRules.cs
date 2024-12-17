namespace Engines;

public interface ISolitaireRules
{
    /// <summary>
    /// Forms the Solitaire board by separating cards into tableau and foundation
    /// </summary>
    void CreateBoard();

    /// <summary>
    /// Cycles the stock to find a card to use. Clicking the stock moves a card
    /// to the discard, where the top card can be used for play
    /// </summary>
    void DrawFromStockpile();

    /// <summary>
    /// Attempts to move part of the pile to a tableau spot
    /// </summary>
    void MoveToTableau(Card selectedCard, Pile chosenPile, TableauPile addingPile);

    /// <summary>
    /// Attempts to move part of the pile to a foundation spot
    /// </summary>
    void MoveToFoundation(Card selectedCard, Pile chosenPile, FoundationPile addingPile);

    /// <summary>
    /// Checks if all foundation piles have all cards; if yes, then Solitaire is won
    /// </summary>
    bool WinSolitaire();
}
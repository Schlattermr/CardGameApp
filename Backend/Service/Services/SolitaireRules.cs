namespace Backend.Services;

public class SolitaireRules : ISolitaireRules
{
    public Deck Deck { get; private set; } = new();
    public TableauPile Tableau1 { get; private set; } = new();
    public TableauPile Tableau2 { get; private set; } = new();
    public TableauPile Tableau3 { get; private set; } = new();
    public TableauPile Tableau4 { get; private set; } = new();
    public TableauPile Tableau5 { get; private set; } = new();
    public TableauPile Tableau6 { get; private set; } = new();
    public TableauPile Tableau7 { get; private set; } = new();
    public FoundationPile FoundationClubs { get; private set; } = new();
    public FoundationPile FoundationDiamonds { get; private set; } = new();
    public FoundationPile FoundationHearts { get; private set; } = new();
    public FoundationPile FoundationSpades { get; private set; } = new();
    public Pile Stock { get; private set; } = new();
    public Pile Discard { get; private set; } = new(); // Can only select the discard's last card for play

    public void CreateBoard()
    {
        // Use Deck's public API to obtain a List<Card> instead of accessing a non-existent 'cards' field.
        List<Card> deckList = Deck.Shuffle(Deck.CreateDeck(GameType.Solitaire));

        List<FoundationPile> foundationPiles =
            [FoundationClubs, FoundationDiamonds, FoundationHearts, FoundationSpades];
        List<TableauPile> tableauPiles =
            [Tableau1, Tableau2, Tableau3, Tableau4, Tableau5, Tableau6, Tableau7];

        // Assigns each foundation pile their suits and empty lists
        var i = 0;
        foreach (FoundationPile pile in foundationPiles)
        {
            pile.acceptedSuit = (Suit)i;
            pile.Cards.Clear();
            i++;
        }

        // Assigns each tableau pile an increasing amount of cards and flips the last card
        i = 0;
        var index = 0;

        foreach (TableauPile pile in tableauPiles)
        {
            // Loop iterates 1, 2, 3, ..., 7 in subsequent foreach calls to give each tableau the correct amount
            for (var j = 0; j <= i; j++)
            {
                pile.Cards.Add(deckList[index]);
                index++;
            }

            pile.LastCard()!.FacingUp = true;
            i++;
        }

        // Assigns the stock the remaining cards
        Stock.Cards.Clear();
        Stock.Cards.AddRange(deckList.GetRange(index, deckList.Count - index));
        Discard.Cards.Clear();
    }

    public void DrawFromStockpile()
    {
        // Case: Stock is empty, refill with cards from discard
        if (Stock.Count() == 0)
        {
            for (var i = 0; i < Discard.Count(); i++)
            {
                Discard.Cards[i].FacingUp = false;
                Stock.Cards.Add(Discard.Cards[i]);
            }

            Discard.Cards.Clear();
        }
        // Case: Stock is nonempty, move latest card to discard where it can then be used for play
        else
        {
            Stock.TopCard()!.FacingUp = true; // Nonempty piles won't have a null top card
            Discard.Cards.Add(Stock.TopCard()!);
            Stock.Cards.Remove(Stock.TopCard()!);
        }
    }

    public void MoveToTableau(Card selectedCard, Pile chosenPile, TableauPile addingPile)
    {
        int selectedIndex = chosenPile.IndexCard(selectedCard);
        if (selectedIndex != -1 && selectedCard.FacingUp) // The selected card is valid
        {
            var isValidMove = (selectedCard.CardNumber == Number.King && addingPile.IsEmpty()) ||
                (!addingPile.IsEmpty() && addingPile.LastCard()!.FacingUp &&
                 (selectedCard.IsBlack() ^ addingPile.LastCard()!.IsBlack()) && // Colors must alternate
                 (Math.Abs(selectedCard.CardNumber - addingPile.LastCard()!.CardNumber) == 1)); // Difference must be 1

            if (isValidMove)
            {
                // Add all cards for a tableau
                if (chosenPile.GetType() == typeof(TableauPile))
                {
                    var count = chosenPile.Count(); // Count must be evaluated once at the start, not during each loop iteration
                    for (var i = selectedIndex; i < count; i++)
                    {
                        addingPile.Cards.Add(chosenPile.Cards[selectedIndex]);
                        chosenPile.Cards.RemoveAt(selectedIndex);
                    }
                }
                // Add only one card for a discard which must be the last
                else if(chosenPile.LastCard() == selectedCard)
                {
                    addingPile.Cards.Add(selectedCard);
                    chosenPile.Cards.Remove(selectedCard);
                }
            }
        }
    }

    public void MoveToFoundation(Card selectedCard, Pile chosenPile, FoundationPile addingPile)
    {
        if (chosenPile.IndexCard(selectedCard) != -1 && selectedCard.FacingUp) // The selected card is valid
        {
            var isValidMove = (chosenPile.IndexCard(selectedCard) != -1 && addingPile.IsEmpty() &&
                addingPile.acceptedSuit == selectedCard.CardSuit && selectedCard.CardNumber == Number.Ace) ||
                (addingPile.LastCard() != null && addingPile.acceptedSuit == selectedCard.CardSuit &&
                Math.Abs(addingPile.LastCard()!.CardNumber - selectedCard.CardNumber) == 1 &&
                selectedCard == chosenPile.LastCard()); // Verifies the selected card is the last card

            if (isValidMove)
            {
                addingPile.Cards.Add(selectedCard);
                chosenPile.Cards.Remove(selectedCard);
            }
        }
    }

    public bool WinSolitaire()
    {
        if (FoundationClubs.IsComplete() && FoundationDiamonds.IsComplete() &&
            FoundationHearts.IsComplete() && FoundationSpades.IsComplete())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

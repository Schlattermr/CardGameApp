using System.Runtime.CompilerServices;
using static System.Reflection.Metadata.BlobBuilder;

namespace Engines;

public class SolitaireRules : ISolitaireRules
{
    protected Deck Deck = new Deck();
    protected TableauPile Tableau1 = new TableauPile(), Tableau2 = new TableauPile(), Tableau3 = new TableauPile(), 
        Tableau4 = new TableauPile(), Tableau5 = new TableauPile(), Tableau6 = new TableauPile(), Tableau7 = new TableauPile();
    protected FoundationPile FoundationClubs = new FoundationPile(), FoundationDiamonds = new FoundationPile(), 
        FoundationHearts = new FoundationPile(), FoundationSpades = new FoundationPile();
    public Pile Stock = new Pile(), Discard = new Pile(); // Can only select the discard's last card for play

    public void CreateBoard()
    {
        Deck.cards = Deck.Shuffle(Deck.CreateDeck(GameType.Solitaire));

        List<FoundationPile> foundationPiles =
            [FoundationClubs, FoundationDiamonds, FoundationHearts, FoundationSpades];
        List<TableauPile> tableauPiles =
            [Tableau1, Tableau2, Tableau3, Tableau4, Tableau5, Tableau6, Tableau7];

        // Assigns each foundation pile their suits and empty lists
        var i = 0;
        foreach(FoundationPile pile in foundationPiles)
        {
            pile.acceptedSuit = (Suit)i;
            pile.cards = new List<Card>();
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
                pile.cards.Add(Deck.cards[index]);
                index++;
            }

            pile.LastCard()!.FacingUp = true;
            i++;
        }

        // Assigns the stock the remaining cards
        Stock.cards = Deck.cards.GetRange(index, 24);
        Discard.cards = new List<Card>();
    }

    public void DrawFromStockpile()
    {
        // Case: Stock is empty, refill with cards from discard
        if (Stock.Count() == 0)
        {
            for (var i = 0; i < Discard.Count(); i++)
            {
                Discard.cards[i].FacingUp = false;
                Stock.cards.Add(Discard.cards[i]);
            }

            Discard.cards.Clear();
        }
        // Case: Stock is nonempty, move latest card to discard where it can then be used for play
        else
        {
            Stock.TopCard()!.FacingUp = true; // Nonempty piles won't have a null top card
            Discard.cards.Add(Stock.TopCard()!);
            Stock.cards.Remove(Stock.TopCard()!);
        }
    }

    public void MoveToTableau(Card selectedCard, Pile chosenPile, TableauPile addingPile)
    {
        int selectedIndex = chosenPile.IndexCard(selectedCard);
        if (selectedIndex != -1 && selectedCard.FacingUp) // The selected card is valid
        {
            // Case: Moving a king into an empty tableau spot
            if (selectedCard.CardNumber == Number.King && addingPile.IsEmpty())
            {
                // Add all cards for a tableau
                if (chosenPile.GetType() == typeof(TableauPile))
                {
                    var count = chosenPile.Count(); // Count must be evaluated once at the start, not during each loop iteration
                    for (var i = selectedIndex; i < count; i++)
                    {
                        addingPile.cards.Add(chosenPile.cards[selectedIndex]);
                        chosenPile.cards.RemoveAt(selectedIndex);
                    }
                }
                // Add only one card for a discard which must be the last
                else if(chosenPile.LastCard() == selectedCard)
                {
                    addingPile.cards.Add(selectedCard);
                    chosenPile.cards.Remove(selectedCard);
                }

            }
            // Case: Normally moving a pile into another pile
            else if (!addingPile.IsEmpty() && addingPile.LastCard()!.FacingUp &&
                     (selectedCard.IsBlack() ^ addingPile.LastCard()!.IsBlack()) && // Colors must alternate
                     (Math.Abs(selectedCard.CardNumber - addingPile.LastCard()!.CardNumber) == 1)) // Difference must be 1
            {
                if (chosenPile.GetType() == typeof(TableauPile))
                {
                    var count = chosenPile.Count();
                    for (var i = selectedIndex; i < count; i++)
                    {
                        addingPile.cards.Add(chosenPile.cards[selectedIndex]);
                        chosenPile.cards.RemoveAt(selectedIndex);
                    }
                }
                else if (chosenPile.LastCard() == selectedCard)
                {
                    addingPile.cards.Add(selectedCard);
                    chosenPile.cards.Remove(selectedCard);
                }
            }

        }

    }

    public void MoveToFoundation(Card selectedCard, Pile chosenPile, FoundationPile addingPile)
    {
        if (chosenPile.IndexCard(selectedCard) != -1 && selectedCard.FacingUp) // The selected card is valid
        {
            // Case: Moving an ace into an empty foundation spot
            if (chosenPile.IndexCard(selectedCard) != -1 && addingPile.IsEmpty() &&
                addingPile.acceptedSuit == selectedCard.CardSuit && selectedCard.CardNumber == Number.Ace)
            {
                addingPile.cards.Add(selectedCard);
                chosenPile.cards.Remove(selectedCard);
            }
            // Case: Normally moving a card into a foundation
            else if (addingPile.LastCard() != null && addingPile.acceptedSuit == selectedCard.CardSuit &&
                Math.Abs(addingPile.LastCard()!.CardNumber - selectedCard.CardNumber) == 1 &&
                selectedCard == chosenPile.LastCard()) // Verifies the selected card is the last card
            {
                addingPile.cards.Add(selectedCard);
                chosenPile.cards.Remove(selectedCard);
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

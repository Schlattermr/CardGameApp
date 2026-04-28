namespace Backend.Models.Domain;

public class Pile : IPile
{
    public List<Card> Cards { get; set; } = new();

    // Provide lowercase alias for backward compatibility with tests
    public List<Card> cards
    {
        get => Cards;
        set => Cards = value ?? new();
    }

    public int Count()
    {
        return Cards.Count;
    }

    public Card? TopCard()
    {
        return Cards.Count > 0 ? Cards[0] : null;
    }

    public Card? LastCard()
    {
        return Cards.Count > 0 ? Cards[^1] : null;
    }

    public bool IsEmpty()
    {
        return Cards.Count == 0;
    }

    public int IndexCard(Card card)
    {
        return Cards.IndexOf(card); // returns -1 if not found
    }
}

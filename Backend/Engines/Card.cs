namespace Engines;

public class Card : ICard
{
    public required Number CardNumber { get; set; }

    public required Suit? CardSuit { get; set; }

    public required bool FacingUp { get; set; }

    public required GameType Game { get; set; }

    public int GetCardValue()
    {
        return (int)CardNumber;
    }

    public void FlipCard()
    {
        FacingUp = !FacingUp;
    }

    public bool IsBlack()
    {
        return (CardSuit == Suit.Clubs || CardSuit == Suit.Spades);
    }
}

public enum Suit
{
    Clubs = 0,
    Diamonds = 1,
    Hearts = 2,
    Spades = 3,
    Joker = 4
}

public enum Number
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Joker = 14
}

public enum GameType
{
    War = 0,
    Solitaire = 1
}

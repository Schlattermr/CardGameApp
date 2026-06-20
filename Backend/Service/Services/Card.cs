using Backend.Models.Enums;

namespace Backend.Services;

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

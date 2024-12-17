namespace Engines;

public interface ICard
{
    /*
     *  Returns value of card
     */
    public int GetCardValue();

    /*
     *  Flips card over
     */ 
    public void FlipCard();

    /*
     *  Determines if card is spades or clubs or neither
     */
    public bool IsBlack();
}
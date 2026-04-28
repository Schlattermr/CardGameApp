namespace Backend.Services;

/*
 * Foundation pile index increments starting from King or the highest card, opposite of foundation pile
 * 0: King
 * 1: Queen
 * 2: Jack
 * etc.
 */
public class TableauPile : Pile, ITableauPile
{
    public bool ValidatePile()
    {
        // Iterates through cards until reaching two facing up cards
        var i = 0;
        do
        {
            if (i >= Cards.Count - 1) // Doesn't have enough cards to compare against
                return true;
            else
            {
                i++;
            }
        } while ((!Cards[i - 1].FacingUp || !Cards[i].FacingUp));


        while (i < Cards.Count)
        {
            if(!(Cards[i - 1].IsBlack() ^ Cards[i].IsBlack() &&  // Colors must alternate
               Math.Abs(Cards[i-1].CardNumber - Cards[i].CardNumber) == 1)) // Card numbers must be sequential
            {
                return false;
            }

            i++;
        }

        return true; // All conditions hold
    }
}

using System;
using System.Collections.Generic;

namespace Engines;

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
            if (i >= cards.Count() - 1) // Doesn't have enough cards to compare against
                return true;
            else
            {
                i++;
            }
        } while ((!cards[i - 1].FacingUp || !cards[i].FacingUp));


        while (i < cards.Count())
        {
            if(!(cards[i - 1].IsBlack() ^ cards[i].IsBlack() &&  // Colors must alternate
               Math.Abs(cards[i-1].CardNumber - cards[i].CardNumber) == 1)) // Card numbers must be sequential
            {
                return false;
            }

            i++;
        }

        return true; // All conditions hold
    }
}

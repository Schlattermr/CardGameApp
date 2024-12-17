using System;
using System.Collections.Generic;

namespace Engines;

/*
 * Foundation pile index increments starting from Ace, opposite of tableau pile
 * 0: Ace
 * 1: 1
 * 2: 2
 * etc.
 */
public class FoundationPile : Pile, IFoundationPile
{
    public Suit acceptedSuit { get; set; }

    public bool IsComplete()
    {
        return cards.Count() == 13;
    }
}
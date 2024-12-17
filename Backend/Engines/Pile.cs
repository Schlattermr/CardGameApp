using System;
using System.Collections.Generic;

namespace Engines;

public class Pile() : IPile
{
    public List<Card> cards = new List<Card>();

    public int Count()
    {
        return cards.Count;
    }

    public Card? TopCard()
    {
        try
        {
            return cards.First();
        }
        catch (ArgumentNullException) // If a pile is empty
        {
            return null;
        }
    }

    public Card? LastCard()
    {
        try
        {
            return cards.Last();
        }
        catch (ArgumentNullException) // If a pile is empty
        {
            return null;
        }

    }

    public bool IsEmpty()
    {
        return cards.Count == 0;
    }

    public int IndexCard(Card card)
    {
        return cards.IndexOf(card); // returns -1 if not found
    }
}

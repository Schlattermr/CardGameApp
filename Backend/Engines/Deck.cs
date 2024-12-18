using System.Runtime.CompilerServices;

namespace Engines;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Deck : IDeck
{
    public List<Card> cards = new List<Card>();

    public List<Card> CreateDeck(GameType game)
{
    // Error handling for invalid GameType
    if (game != GameType.War && game != GameType.Solitaire)
    {
            return [];
    }

    // Reset deck
    this.cards = new List<Card>();

    // Array of suit names
    var suits = new[] { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };

    for (var suitIndex = 0; suitIndex < suits.Length; suitIndex++)
    {
        for (var numberIndex = 1; numberIndex <= 13; numberIndex++)
        {
            // Create a card for each number and suit
            cards.Add(new Card
            {
                CardNumber = (Number)numberIndex,
                CardSuit = suits[suitIndex], // Assign suit
                FacingUp = false,
                Game = game
            });
        }
    }

    // Add two jokers for War
    if (game == GameType.War)
    {
        cards.Add(new Card { CardNumber = (Number)14, CardSuit = Suit.Joker, FacingUp = false, Game = game });
        cards.Add(new Card { CardNumber = (Number)14, CardSuit = Suit.Joker, FacingUp = false, Game = game });
    }
    

    return cards;
}

    public int CountJokers(List<Card> deck)
    {
        var jokerCount = 0;

        foreach (var c in deck)
        {
            if (c.CardNumber == Number.Joker)
            {
                jokerCount++;
            }
        }
        return jokerCount;
    }

    public List<Card> AddCardToDeck(List<Card> roundCards)
    {
        foreach (var roundCard in roundCards)
        {
            this.cards.Add(roundCard);
        }
        return this.cards;
    }

    public Card PullTopCard()
    {
        if (this.cards.Count == 0)
        {
            throw new InvalidOperationException("No Cards in Deck");
        }

        var pulledCard = this.cards[0];    // Get the top card
        this.cards.RemoveAt(0);      // Remove it from the deck
        return pulledCard;
    }

    public List<Card> Shuffle(List<Card> deck)
    {
        if(deck.Count == 0)
        {
            throw new InvalidOperationException("No Cards in Deck");
        }

        Random seed = new Random(); // creates a randomized seed to determine order
        for(int i = deck.Count - 1; i > 0; --i)
        {
            int randomPosition = seed.Next(i + 1);

            // Swaps cards to a random position
            (deck[i], deck[randomPosition]) = (deck[randomPosition], deck[i]);
        }

        return deck;
    }
}

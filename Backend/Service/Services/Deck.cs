using Backend.Models.Enums;

namespace Backend.Services;

public class Deck : IDeck
{
    public List<Card> Cards { get; set; } = new List<Card>();

    public List<Card> CreateDeck(GameType game)
    {
        if (game != GameType.War && game != GameType.Solitaire)
            return [];

        Cards = new List<Card>();

        var suits = new[] { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };

        for (var suitIndex = 0; suitIndex < suits.Length; suitIndex++)
        {
            for (var numberIndex = 1; numberIndex <= 13; numberIndex++)
            {
                Cards.Add(new Card
                {
                    CardNumber = (Number)numberIndex,
                    CardSuit = suits[suitIndex],
                    FacingUp = false,
                    Game = game
                });
            }
        }

        if (game == GameType.War)
        {
            Cards.Add(new Card { CardNumber = (Number)14, CardSuit = Suit.Joker, FacingUp = false, Game = game });
            Cards.Add(new Card { CardNumber = (Number)14, CardSuit = Suit.Joker, FacingUp = false, Game = game });
        }

        return Cards;
    }

    public int CountJokers(List<Card> deck)
    {
        var jokerCount = 0;
        foreach (var c in deck)
        {
            if (c.CardNumber == Number.Joker)
                jokerCount++;
        }
        return jokerCount;
    }

    public List<Card> AddCardToDeck(List<Card> roundCards)
    {
        foreach (var roundCard in roundCards)
            Cards.Add(roundCard);
        return Cards;
    }

    public Card PullTopCard()
    {
        if (Cards.Count == 0)
            throw new InvalidOperationException("No Cards in Deck");

        var pulledCard = Cards[0];
        Cards.RemoveAt(0);
        return pulledCard;
    }

    public List<Card> Shuffle(List<Card> deck)
    {
        if (deck.Count == 0)
            throw new InvalidOperationException("No Cards in Deck");

        Random seed = new Random();
        for (int i = deck.Count - 1; i > 0; --i)
        {
            int randomPosition = seed.Next(i + 1);
            (deck[i], deck[randomPosition]) = (deck[randomPosition], deck[i]);
        }

        return deck;
    }
}

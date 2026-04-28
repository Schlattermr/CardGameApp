using Backend.Models.Enums;

namespace Backend.Models.Domain;

public class Deck : IDeck
{
    private List<Card> _cards = new();

    // Expose read-only view of the internal list
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

    // Public property for backward compatibility with tests
    public List<Card> cards
    {
        get => _cards;
        set => _cards = value ?? new();
    }

    // Internal method to populate the deck (used for testing)
    internal void SetCards(List<Card> cards)
    {
        _cards = cards;
    }

    public List<Card> CreateDeck(GameType game)
    {
        // Error handling for invalid GameType
        if (game != GameType.War && game != GameType.Solitaire)
        {
            return [];
        }

        // Reset deck
        _cards = new();

        // Array of suit names
        var suits = new[] { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };

        for (var suitIndex = 0; suitIndex < suits.Length; suitIndex++)
        {
            for (var numberIndex = 1; numberIndex <= 13; numberIndex++)
            {
                // Create a card for each number and suit
                _cards.Add(new Card
                {
                    CardNumber = (Number)numberIndex,
                    CardSuit = suits[suitIndex],
                    FacingUp = false,
                    Game = game
                });
            }
        }

        // Add two jokers for War
        if (game == GameType.War)
        {
            _cards.Add(new Card { CardNumber = Number.Joker, CardSuit = Suit.Joker, FacingUp = false, Game = game });
            _cards.Add(new Card { CardNumber = Number.Joker, CardSuit = Suit.Joker, FacingUp = false, Game = game });
        }

        return _cards;
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
        _cards.AddRange(roundCards);
        return _cards;
    }

    public Card PullTopCard()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException("No Cards in Deck");
        }

        var pulledCard = _cards[0];
        _cards.RemoveAt(0);
        return pulledCard;
    }

    public List<Card> Shuffle(List<Card> deck)
    {
        if (deck.Count == 0)
        {
            throw new InvalidOperationException("No Cards in Deck");
        }

        var seed = new Random();
        for (int i = deck.Count - 1; i > 0; --i)
        {
            int randomPosition = seed.Next(i + 1);

            // Swaps cards to a random position
            (deck[i], deck[randomPosition]) = (deck[randomPosition], deck[i]);
        }

        return deck;
    }
}

using Backend.Models.Domain;
using Backend.Models.Enums;

namespace Backend.Services;

public class BlackjackRules : IBlackjackRules
{
    public Deck Deck { get; private set; } = new();

    public List<Card> PlayerHand { get; private set; } = new();

    public List<Card> DealerHand { get; private set; } = new();

    public bool RoundOver { get; private set; }

    public void StartRound()
    {
        Deck = new Deck();
        Deck.Shuffle(Deck.CreateDeck(GameType.Blackjack));

        PlayerHand = new List<Card>();
        DealerHand = new List<Card>();
        RoundOver = false;

        PlayerHand.Add(DrawCard(faceUp: true));
        DealerHand.Add(DrawCard(faceUp: true));
        PlayerHand.Add(DrawCard(faceUp: true));
        DealerHand.Add(DrawCard(faceUp: false));

        if (IsBlackjack(PlayerHand) || GetHandValue(DealerHand) == 21)
        {
            EndRound();
        }
    }

    public void Hit()
    {
        if (RoundOver)
        {
            throw new InvalidOperationException("Round is already over");
        }

        PlayerHand.Add(DrawCard(faceUp: true));

        if (IsBust(PlayerHand))
        {
            EndRound();
        }
    }

    public void Stand()
    {
        if (RoundOver)
        {
            throw new InvalidOperationException("Round is already over");
        }

        while (GetHandValue(DealerHand) < 17)
        {
            DealerHand.Add(DrawCard(faceUp: true));
        }

        EndRound();
    }

    public string GetResult()
    {
        if (!RoundOver)
        {
            return "In Progress";
        }

        if (IsBust(PlayerHand))
        {
            return "Dealer Wins - Player Bust";
        }

        if (IsBlackjack(PlayerHand) && !IsBlackjack(DealerHand))
        {
            return "Player Wins - Blackjack";
        }

        if (IsBlackjack(DealerHand) && !IsBlackjack(PlayerHand))
        {
            return "Dealer Wins - Blackjack";
        }

        if (IsBust(DealerHand))
        {
            return "Player Wins - Dealer Bust";
        }

        var playerValue = GetHandValue(PlayerHand);
        var dealerValue = GetHandValue(DealerHand);

        if (playerValue > dealerValue)
        {
            return "Player Wins";
        }

        if (dealerValue > playerValue)
        {
            return "Dealer Wins";
        }

        return "Push";
    }

    public static int GetHandValue(List<Card> hand)
    {
        var value = 0;
        var aceCount = 0;

        foreach (var card in hand)
        {
            if (card.CardNumber == Number.Ace)
            {
                aceCount++;
                value += 11;
            }
            else if (card.CardNumber >= Number.Ten)
            {
                value += 10;
            }
            else
            {
                value += (int)card.CardNumber;
            }
        }

        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }

        return value;
    }

    public static bool IsBust(List<Card> hand)
    {
        return GetHandValue(hand) > 21;
    }

    public static bool IsBlackjack(List<Card> hand)
    {
        return hand.Count == 2 && GetHandValue(hand) == 21;
    }

    private void EndRound()
    {
        if (DealerHand.Count > 1)
        {
            DealerHand[1].FacingUp = true;
        }

        RoundOver = true;
    }

    private Card DrawCard(bool faceUp)
    {
        var card = Deck.PullTopCard();
        card.FacingUp = faceUp;
        return card;
    }
}

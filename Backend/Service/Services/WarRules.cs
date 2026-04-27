using Backend.Models;

namespace Backend.Services;

public class WarRules : IWarRules
{
    private readonly Deck deck;
    private User? player1, player2, player3, player4, player5, player6;

    public WarRules()
    {
        deck = new Deck();
    }

    public void CreateWarGame(User p1, User p2, User p3, User p4, User p5, User p6)
    {
        var cards = deck.CreateDeck(GameType.War);
        var shuffledCards = deck.Shuffle(cards);
        deck.cards = shuffledCards;

        player1 = p1;
        player2 = p2;
        player3 = p3;
        player4 = p4;
        player5 = p5;
        player6 = p6;
    }

    public void PlayWar()
    {
        var warDeck = new Deck();
        var cards = warDeck.CreateDeck(GameType.War);
        warDeck.cards = cards;

        for (var i = 0; i < 54; i += 6)
        {
            player1?.SetWarDeck(warDeck.PullTopCard(), i);
            player2?.SetWarDeck(warDeck.PullTopCard(), i + 1);
            player3?.SetWarDeck(warDeck.PullTopCard(), i + 2);
            player4?.SetWarDeck(warDeck.PullTopCard(), i + 3);
            player5?.SetWarDeck(warDeck.PullTopCard(), i + 4);
            player6?.SetWarDeck(warDeck.PullTopCard(), i + 5);
        }

        List<User> players = new List<User> { player1!, player2!, player3!, player4!, player5!, player6! };
        var roundWinner = GetWinner(players);
    }

    public User GetWinner(List<User> players)
    {
        var winner = players[0];
        var highestValue = 0;

        foreach (var player in players)
        {
            var playerCardValue = player.GetWarCard((int)GameType.War).GetCardValue();
            if (playerCardValue > highestValue)
            {
                winner = player;
                highestValue = playerCardValue;
            }
        }
        return winner;
    }
}

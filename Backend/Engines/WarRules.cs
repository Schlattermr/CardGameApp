using System.Dynamic;
using System.Runtime.InteropServices;
using BackendAPI.Models;

namespace Engines;

public class WarRules : IWarRules
{
    private Deck deck;
    private User player1, player2, player3, player4, player5, player6;

    public void CreateWarGame(User p1, User p2, User p3, User p4, User p5, User p6)
    {

        var deck = new Deck();
        var cards = deck.CreateDeck((int)GameType.War);
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
        // Pull card for each player

        var warDeck = new Deck();
        var cards = deck.CreateDeck((int)GameType.War);
        deck.cards = cards;

        for (var i = 0; i < 54; i += 6)
        {
            player1.SetWarDeck(deck.PullTopCard(), i);
            player2.SetWarDeck(deck.PullTopCard(), i + 1);
            player3.SetWarDeck(deck.PullTopCard(), i + 2);
            player4.SetWarDeck(deck.PullTopCard(), i + 3);
            player5.SetWarDeck(deck.PullTopCard(), i + 4);
            player6.SetWarDeck(deck.PullTopCard(), i + 5);
        }

        List<User> players = [player1, player2, player3, player4, player5, player6];
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
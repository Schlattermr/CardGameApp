using Microsoft.AspNetCore.SignalR;
using Backend.Services;
using Backend.Managers;
using Backend.Models.Enums;

namespace Backend.Hubs
{
    public class WarGameHub : Hub
    {
        // ── In-memory state (single-room, max 2 players) ─────────────────────
        static readonly object _lock = new();
        static readonly List<WarPlayer> _players = new();

        // ── JoinGame ──────────────────────────────────────────────────────────
        public async Task JoinGame(string username)
        {
            // Decide outcome inside the lock, then act outside (locks can't be async).
            JoinOutcome outcome = JoinOutcome.First;
            string p1ConnId = "";
            string p1Username = "";
            string p2ConnId = "";
            string p2Username = "";
            int p1CardCount = 0;
            int p2CardCount = 0;

            lock (_lock)
            {
                if (_players.Count >= 2)
                {
                    outcome = JoinOutcome.Full;
                }
                else
                {
                    var player = new WarPlayer
                    {
                        ConnectionId = Context.ConnectionId,
                        Username = username
                    };
                    _players.Add(player);

                    if (_players.Count == 1)
                    {
                        outcome = JoinOutcome.First;
                    }
                    else
                    {
                        outcome = JoinOutcome.Second;

                        var hands = DeckManager.InitializeAndDistributeDeck(
                            new List<string> { _players[0].Username, _players[1].Username });

                        _players[0].Hand = hands[_players[0].Username];
                        _players[1].Hand = hands[_players[1].Username];

                        p1ConnId      = _players[0].ConnectionId;
                        p1Username    = _players[0].Username;
                        p2ConnId      = _players[1].ConnectionId;
                        p2Username    = _players[1].Username;
                        p1CardCount   = _players[0].Hand.Count;
                        p2CardCount   = _players[1].Hand.Count;
                    }
                }
            }

            switch (outcome)
            {
                case JoinOutcome.Full:
                    await Clients.Caller.SendAsync("GameFull");
                    break;

                case JoinOutcome.First:
                    await Clients.Caller.SendAsync("WaitingForOpponent",
                        new { yourUsername = username });
                    break;

                case JoinOutcome.Second:
                    await Clients.Client(p1ConnId).SendAsync("GameStarted", new
                    {
                        yourUsername      = p1Username,
                        opponentUsername  = p2Username,
                        yourCardCount     = p1CardCount,
                        opponentCardCount = p2CardCount
                    });
                    await Clients.Client(p2ConnId).SendAsync("GameStarted", new
                    {
                        yourUsername      = p2Username,
                        opponentUsername  = p1Username,
                        yourCardCount     = p2CardCount,
                        opponentCardCount = p1CardCount
                    });
                    break;
            }
        }

        // ── FlipCard ──────────────────────────────────────────────────────────
        public async Task FlipCard()
        {
            // Capture all state inside the lock, then send messages outside.
            FlipOutcome outcome = FlipOutcome.NotFound;
            string? otherConnId = null;

            string myConnId              = "";
            string otherConnIdForResult  = "";
            Card?  myCard                = null;
            Card?  otherCard             = null;
            string winnerConnId          = "";
            bool   isTie                 = false;
            bool   gameOver              = false;
            int    myScore               = 0;
            int    otherScore            = 0;
            int    myCardsRemaining      = 0;
            int    otherCardsRemaining   = 0;

            lock (_lock)
            {
                var me = _players.Find(p => p.ConnectionId == Context.ConnectionId);
                if (me == null || me.HasFlipped)
                {
                    outcome = FlipOutcome.NotFound;
                }
                else
                {
                    me.HasFlipped  = true;
                    me.FlippedCard = me.Hand[0];
                    me.Hand.RemoveAt(0);

                    var other = _players.Find(p => p.ConnectionId != Context.ConnectionId);

                    if (other == null || !other.HasFlipped)
                    {
                        outcome     = FlipOutcome.WaitingForOther;
                        otherConnId = other?.ConnectionId;
                    }
                    else
                    {
                        outcome = FlipOutcome.BothFlipped;

                        myCard    = me.FlippedCard;
                        otherCard = other.FlippedCard;
                        myConnId             = me.ConnectionId;
                        otherConnIdForResult = other.ConnectionId;

                        // Determine round winner
                        int myNum    = (int)myCard!.CardNumber;
                        int otherNum = (int)otherCard!.CardNumber;

                        if (myNum > otherNum)
                        {
                            winnerConnId = me.ConnectionId;
                            me.Score++;
                        }
                        else if (otherNum > myNum)
                        {
                            winnerConnId = other.ConnectionId;
                            other.Score++;
                        }
                        else
                        {
                            // Tie-break by suit (higher int wins)
                            int mySuit    = (int)(myCard.CardSuit ?? Suit.Clubs);
                            int otherSuit = (int)(otherCard.CardSuit ?? Suit.Clubs);

                            if (mySuit > otherSuit)
                            {
                                winnerConnId = me.ConnectionId;
                                me.Score++;
                            }
                            else if (otherSuit > mySuit)
                            {
                                winnerConnId = other.ConnectionId;
                                other.Score++;
                            }
                            else
                            {
                                isTie = true;
                            }
                        }

                        // Reset flip state for next round
                        me.HasFlipped    = false; me.FlippedCard    = null;
                        other.HasFlipped = false; other.FlippedCard = null;

                        myScore              = me.Score;
                        otherScore           = other.Score;
                        myCardsRemaining     = me.Hand.Count;
                        otherCardsRemaining  = other.Hand.Count;

                        gameOver = myCardsRemaining == 0 || otherCardsRemaining == 0;
                    }
                }
            }

            switch (outcome)
            {
                case FlipOutcome.NotFound:
                    // Nothing to do
                    break;

                case FlipOutcome.WaitingForOther:
                    // Notify the other player that the caller has flipped
                    if (otherConnId != null)
                        await Clients.Client(otherConnId).SendAsync("OpponentFlipped");
                    break;

                case FlipOutcome.BothFlipped:
                    if (myCard == null || otherCard == null) break;

                    string myWinLabel;
                    string otherWinLabel;
                    if (isTie)
                    {
                        myWinLabel    = "tie";
                        otherWinLabel = "tie";
                    }
                    else if (winnerConnId == myConnId)
                    {
                        myWinLabel    = "me";
                        otherWinLabel = "opponent";
                    }
                    else
                    {
                        myWinLabel    = "opponent";
                        otherWinLabel = "me";
                    }

                    if (gameOver)
                    {
                        await Clients.Client(myConnId).SendAsync("GameOver", new
                        {
                            winner        = myWinLabel,
                            myScore       = myScore,
                            opponentScore = otherScore
                        });
                        await Clients.Client(otherConnIdForResult).SendAsync("GameOver", new
                        {
                            winner        = otherWinLabel,
                            myScore       = otherScore,
                            opponentScore = myScore
                        });
                    }
                    else
                    {
                        await Clients.Client(myConnId).SendAsync("RoundResult", new
                        {
                            myCard = new
                            {
                                cardNumber = (int)myCard.CardNumber,
                                cardSuit   = (int)(myCard.CardSuit ?? Suit.Clubs)
                            },
                            opponentCard = new
                            {
                                cardNumber = (int)otherCard.CardNumber,
                                cardSuit   = (int)(otherCard.CardSuit ?? Suit.Clubs)
                            },
                            roundWinner            = myWinLabel,
                            myScore                = myScore,
                            opponentScore          = otherScore,
                            myCardsRemaining       = myCardsRemaining,
                            opponentCardsRemaining = otherCardsRemaining
                        });

                        await Clients.Client(otherConnIdForResult).SendAsync("RoundResult", new
                        {
                            myCard = new
                            {
                                cardNumber = (int)otherCard.CardNumber,
                                cardSuit   = (int)(otherCard.CardSuit ?? Suit.Clubs)
                            },
                            opponentCard = new
                            {
                                cardNumber = (int)myCard.CardNumber,
                                cardSuit   = (int)(myCard.CardSuit ?? Suit.Clubs)
                            },
                            roundWinner            = otherWinLabel,
                            myScore                = otherScore,
                            opponentScore          = myScore,
                            myCardsRemaining       = otherCardsRemaining,
                            opponentCardsRemaining = myCardsRemaining
                        });
                    }
                    break;
            }
        }

        // ── PlayAgain ─────────────────────────────────────────────────────────
        public async Task PlayAgain()
        {
            bool ready = false;
            string p1ConnId   = "";
            string p2ConnId   = "";
            string p1Username = "";
            string p2Username = "";
            int p1CardCount   = 0;
            int p2CardCount   = 0;

            lock (_lock)
            {
                if (_players.Count != 2) return;

                var hands = DeckManager.InitializeAndDistributeDeck(
                    new List<string> { _players[0].Username, _players[1].Username });

                foreach (var p in _players)
                {
                    p.Hand        = hands[p.Username];
                    p.Score       = 0;
                    p.HasFlipped  = false;
                    p.FlippedCard = null;
                }

                p1ConnId   = _players[0].ConnectionId;
                p1Username = _players[0].Username;
                p2ConnId   = _players[1].ConnectionId;
                p2Username = _players[1].Username;
                p1CardCount = _players[0].Hand.Count;
                p2CardCount = _players[1].Hand.Count;
                ready = true;
            }

            if (!ready) return;

            await Clients.Client(p1ConnId).SendAsync("GameStarted", new
            {
                yourUsername      = p1Username,
                opponentUsername  = p2Username,
                yourCardCount     = p1CardCount,
                opponentCardCount = p2CardCount
            });
            await Clients.Client(p2ConnId).SendAsync("GameStarted", new
            {
                yourUsername      = p2Username,
                opponentUsername  = p1Username,
                yourCardCount     = p2CardCount,
                opponentCardCount = p1CardCount
            });
        }

        // ── OnDisconnectedAsync ───────────────────────────────────────────────
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? remainingConnId = null;

            lock (_lock)
            {
                _players.RemoveAll(p => p.ConnectionId == Context.ConnectionId);

                if (_players.Count == 1)
                    remainingConnId = _players[0].ConnectionId;
            }

            if (remainingConnId != null)
                await Clients.Client(remainingConnId).SendAsync("OpponentDisconnected");

            await base.OnDisconnectedAsync(exception);
        }
    }

    // ── Supporting types ──────────────────────────────────────────────────────

    class WarPlayer
    {
        public string     ConnectionId { get; set; } = "";
        public string     Username     { get; set; } = "";
        public List<Card> Hand         { get; set; } = new();
        public int        Score        { get; set; } = 0;
        public bool       HasFlipped   { get; set; } = false;
        public Card?      FlippedCard  { get; set; } = null;
    }

    enum JoinOutcome { Full, First, Second }
    enum FlipOutcome { NotFound, WaitingForOther, BothFlipped }
}

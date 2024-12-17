using Engines;

namespace Managers
{
    public class DeckManager
    {
        public void Test()
        {
            Console.WriteLine("DeckManager is accessible and working.");
        }
        public Dictionary<string, List<Card>> InitializeAndDistributeDeck(List<string> playerNames)
        {
            if (playerNames == null || playerNames.Count == 0)
            {
                throw new ArgumentException("Player names cannot be null or empty.");
            }

            Deck deck = new Deck();
            List<Card> shuffledDeck = deck.Shuffle(deck.CreateDeck(GameType.War));

            Dictionary<string, List<Card>> playerHands = new Dictionary<string, List<Card>>();
            int cardsPerPlayer = shuffledDeck.Count / playerNames.Count;

            for (int i = 0; i < playerNames.Count; i++)
            {
                string playerName = playerNames[i];
                playerHands[playerName] = shuffledDeck.Skip(i * cardsPerPlayer).Take(cardsPerPlayer).ToList();
            }

            return playerHands;
        }
    }
}

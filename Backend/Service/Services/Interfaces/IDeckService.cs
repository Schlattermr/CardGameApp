using Backend.Models.Domain;

namespace Backend.Services.Interfaces;

public interface IDeckService
{
    /// <summary>
    /// Initializes a deck and distributes cards to players
    /// </summary>
    Dictionary<string, List<Card>> InitializeAndDistributeDeck(List<string> playerNames);
}

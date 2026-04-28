using Backend.Data.Entities;

namespace Backend.Services.Interfaces;

public interface IWarService
{
    /// <summary>
    /// Forms the War board by separating cards into piles for each player
    /// </summary>
    void CreateWarGame(User p1, User p2, User p3, User p4, User p5, User p6);

    /// <summary>
    /// Logic for handing out cards and playing war
    /// </summary>
    void PlayWar();

    /// <summary>
    /// Logic to get winner of each round
    /// </summary>
    User GetWinner(List<User> players);
}

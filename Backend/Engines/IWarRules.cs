using BackendAPI.Models;

namespace Engines;

public interface IWarRules
{
    /*
     *  Forms the War board by separating cards into piles for each player
     */
    void CreateWarGame(User p1, User p2, User p3, User p4, User p5, User p6);

    /*
     *  Logic for handing out cards and playing war
     */
    void PlayWar();

    /*
     *  Logic to get winner of each round
     */
    User GetWinner(List<User> players);
}
using Backend.Services;

namespace Backend.Models
{
    public interface IUser
    {

        public void SetWarDeck(Card card, int i);

        public Card GetWarCard(int i);

    }
}

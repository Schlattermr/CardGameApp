import React, { useState, useEffect } from 'react';
import '../styles/SolitaireGame.css'; // Import Solitaire game-specific styles

let selected_card = null;

const SolitaireGame = () => {
  const [deckId, setDeckId] = useState(null);
  const [cards, setCards] = useState([]);
  const [stacks, setStacks] = useState([
    [],
    [],
    [],
    [],
    [],
    [],
    [],
  ]);
  const [drawStack, setDrawStack] = useState([]);

  useEffect(() => {
    // Fetch a new deck of cards and shuffle it
    const fetchDeck = async () => {
      try {
        const response = await fetch('https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1');
        const data = await response.json();
        setDeckId(data.deck_id);
        await dealCards(data.deck_id);
      } catch (err) {
        console.error('Error fetching deck:', err);
      }
    };

    fetchDeck();
  }, []);

  const dealCards = async (deckId) => {
    try {
      const response = await fetch(`https://deckofcardsapi.com/api/deck/${deckId}/draw/?count=52`);
      const data = await response.json();
      const shuffledCards = data.cards;
      setCards(shuffledCards);
      distributeCards(shuffledCards);
    } catch (err) {
      console.error('Error dealing cards:', err);
    }
  };

  const distributeCards = (shuffledCards) => {
    const newStacks = Array(7).fill([]);
  
    // Distribute cards into the first 7 stacks
    let index = 0;
    for (let i = 0; i < 7; i++) {
      newStacks[i] = shuffledCards.slice(index, index + i + 1); 
      index += i + 1;
    }

    // The remaining cards go into a separate stack (after the first 7 stacks)
    const remainingCards = shuffledCards.slice(index);

    setDrawStack(remainingCards);
    setStacks(newStacks.reverse());
  };

  const HandleSlotClick = () => {
    if (selected_card) {
      selected_card.classList.remove('selected');
      selected_card = null;
    }
  };

  const HandleCardClick = (card) => {
    if (selected_card === null) {
      selected_card = card;
      card.classList.add('selected');
    } else {
      const sourceStack = selected_card.parentElement;
      const targetStack = card.parentElement;

      if (selected_card === card) {
        selected_card.classList.remove('selected');
        selected_card = null;
      } else {
        if (sourceStack === targetStack) {
          selected_card.classList.remove('selected');
          card.classList.add('selected');
          selected_card = card;
        } else {
          if (targetStack.lastElementChild === card) {
            MoveCards(sourceStack, targetStack, selected_card);
            selected_card.classList.remove('selected');
            selected_card = null;
          } else {
            selected_card.classList.remove('selected');
            card.classList.add('selected');
            selected_card = card;
          }
        }
      }
    }
  };

  const MoveCards = (sourceStack, targetStack, selected_card) => {
    const cardIndex = Array.from(sourceStack.children).indexOf(selected_card);
    const cardsToMove = Array.from(sourceStack.children).slice(cardIndex, sourceStack.children.length);

    setStacks((prevStacks) => {
      const newStacks = [...prevStacks];
      return newStacks;
    });

    cardsToMove.forEach((card) => {
      targetStack.appendChild(card);
      card.style.setProperty('--card-index', targetStack.children.length - 1);
    });
  };

  return (
    <div className="solitaire-container">
      <button className="home-button" onClick={() => window.location.href = "/homepage"}>Go to Homepage</button>
      <div className="card-row">
        <div className="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
        <div className="card-filler" id="card-filler"></div>
        <div className="card" id="card" onClick={() => {HandleSlotClick()}}>
          <img
            src={drawStack[0].image}
            alt="Card Face"
            className="card-face"
          />
        </div>
        <div className="card" id="card" onClick={() => {
          if (drawStack.length > 0) {
            const topCard = drawStack[0];
            setDrawStack(drawStack.slice(1));
            console.log("Drawn card:", topCard);
          }
        }}>
          <img
            src={"https://deckofcardsapi.com/static/img/back.png"}
            alt="Card Face"
            className="card-face"
          />
        </div>
      </div>
      <div className="card-row">
        {stacks.map((stack, stackIndex) => (
          <div className="card-stack" key={stackIndex}>
            {stack.length === 0 ? (
              <div className="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
            ) : (
              stack.map((card, cardIndex) => (
                <div
                  className="card"
                  key={cardIndex}
                  style={{ '--card-index': cardIndex }}
                  onClick={(e) => HandleCardClick(e.currentTarget)}
                >
                  <img
                    src={card.image}
                    alt="Card Face"
                    className="card-face"
                  />
                </div>
              ))
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default SolitaireGame;

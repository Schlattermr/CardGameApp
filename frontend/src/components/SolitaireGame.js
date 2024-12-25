import React, { useState, useEffect } from 'react';
import '../styles/SolitaireGame.css'; // Import Solitaire game-specific styles

let selected_card = null;

const SolitaireGame = () => {
  const [deckId, setDeckId] = useState(null);
  const [cards, setCards] = useState([]);
  const [stacks, setStacks] = useState([[], [], [], [], [], [], []]);
  const [drawStack, setDrawStack] = useState([]);
  const [topCard, setTopCard] = useState(null);
  const [revealedCards, setRevealedCards] = useState([]);
  const suitNames = ['C', 'D', 'H', 'S']; 
  const cardNumberNames = [
    'A', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K' // 0 is for 10
  ];

  useEffect(() => {
    // Fetch a new deck of cards and shuffle it
    const fetchDeck = async () => {
      try {
        const response = await fetch('https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1');
        const data = await response.json();
        setDeckId(data.deck_id);
        await dealCards(data.deck_id);
      } catch (e) {
        console.error('Error fetching deck:', e);
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
    } catch (e) {
      console.error('Error dealing cards:', e);
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

    // Add bottom card of each stack to revealed cards
    const bottomCards = newStacks.map((stack) => stack[stack.length - 1]);
    setRevealedCards(bottomCards);
  };

  const handleSlotClick = () => {
    if (selected_card) {
      selected_card.classList.remove('selected');
      selected_card = null;
    }
  };  

  const handleCardClick = (card) => {
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
            moveCards(sourceStack, targetStack, selected_card);
            // Ensure selected_card is valid before accessing its classList
            if (selected_card && selected_card.classList) {
              selected_card.classList.remove('selected');
            }
            selected_card = null;
          } else {
            // Deselect the current selected card
            if (selected_card && selected_card.classList) {
              selected_card.classList.remove('selected');
            }
            card.classList.add('selected');
            selected_card = card;
          }
        }
      }
    }
  };

  const handleDrawStackClick = () => {
    if (drawStack.length > 0) {
      const newTopCard = drawStack[0];
      setTopCard(newTopCard); // Update the top card
      setDrawStack(drawStack.slice(1)); // Remove the card from the draw stack
      console.log("Drawn card:", newTopCard);
    }
  };

  const moveCards = (sourceStack, targetStack, selectedCard) => {
    if (!selectedCard) {
      console.error("No card selected to move.");
      return;
    }
  
    setStacks((prevStacks) => {
      const sourceStackIndex = Array.from(sourceStack.parentElement.children).indexOf(sourceStack);
      const targetStackIndex = Array.from(targetStack.parentElement.children).indexOf(targetStack);
  
      const newStacks = [...prevStacks];
      const sourceCards = [...newStacks[sourceStackIndex]];
      const targetCards = [...newStacks[targetStackIndex]];
  
      // Find the index of the selected card in the source stack
      const cardIndex = Array.from(sourceStack.children).indexOf(selectedCard);
  
      // Move the selected card and any above it
      const cardsToMove = sourceCards.splice(cardIndex);
      targetCards.push(...cardsToMove);
  
      // Update stacks
      newStacks[sourceStackIndex] = sourceCards;
      newStacks[targetStackIndex] = targetCards;
  
      return newStacks;
    });
  
    // Safely handle deselecting the card (only if selected_card is valid)
    if (selected_card && selected_card.classList) {
      selected_card.classList.remove('selected');
    }
    selected_card = null;
  };  

  const navigateToHome = () => {
    window.location.href = "/"; 
  };

  return (
    <div className="solitaire-container">
      <button className="home-button" onClick={() => navigateToHome()}>Go to Homepage</button>
      <div className="card-row">
        <div className="card-slot" id="card-slot" onClick={(e) => handleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => handleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => handleSlotClick()}></div>
        <div className="card-slot" id="card-slot" onClick={(e) => handleSlotClick()}></div>
        <div className="card-filler" id="card-filler"></div>
        <div className={topCard ? "card" : "card-slot"} id={topCard ? "card" : "card-slot"} onClick={() => {}}>
          <noscript>
            Only display first image if draw pile is selected, otherwise display card slot
          </noscript>
          {topCard && <img src={topCard.image} alt="Card Face" className="card-face" draggable="false"/>}
        </div>
        <div className="card" id="card" onClick={handleDrawStackClick}>
          {drawStack.length > 0 ? (
            <img src={"https://deckofcardsapi.com/static/img/back.png"} alt="Card Back" className="card-face" draggable="false"/>
          ) : (
            <img src={"https://as1.ftcdn.net/v2/jpg/09/88/84/70/1000_F_988847079_yMrhhzz9kO1Nu1SjkECqxyNfHGd4BD0O.jpg"} alt="Reset Deck" className="reset-deck" draggable="false"/>
          )}
        </div>
      </div>
      <div className="card-row">
        {stacks.map((stack, stackIndex) => (
          <div className="card-stack" key={stackIndex}>
            {stack.length === 0 ? (
              <div className="card-slot" id="card-slot" onClick={(e) => handleSlotClick()}></div>
            ) : (
              stack.map((card, cardIndex) => (
                <div
                  className="card"
                  key={cardIndex}
                  style={{ '--card-index': cardIndex }}
                  onClick={(e) => handleCardClick(e.currentTarget)}
                >
                  <img
                    src={cardIndex === stack.length - 1 ? card.image : "https://deckofcardsapi.com/static/img/back.png"}
                    alt={cardIndex === stack.length - 1 ? "Card Face" : "Card Back"}
                    className="card-face"
                    draggable="false"
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

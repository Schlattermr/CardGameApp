import React, { useState } from 'react';
import '../styles/SolitaireGame.css'; // Import Solitaire game-specific styles

let selected_card = null;

const SolitaireGame = () => {
  // SOLITAIRE SETUP
  const [stacks, setStacks] = useState([
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/English_pattern_queen_of_diamonds.svg/1280px-English_pattern_queen_of_diamonds.svg.png"
    ],
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/d/da/English_pattern_6_of_hearts.svg/1024px-English_pattern_6_of_hearts.svg.png",
    ],
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0f/English_pattern_3_of_hearts.svg/80px-English_pattern_3_of_hearts.svg.png"
    ],
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/6/60/English_pattern_7_of_clubs.svg/80px-English_pattern_7_of_clubs.svg.png"
    ],
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/c/cb/English_pattern_7_of_hearts.svg/80px-English_pattern_7_of_hearts.svg.png"
    ],
    [
      "https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png",
      "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f1/English_pattern_king_of_spades.svg/80px-English_pattern_king_of_spades.svg.png"
    ],
    [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f0/English_pattern_9_of_spades.svg/80px-English_pattern_9_of_spades.svg.png"
    ],
  ]);

  const HandleSlotClick = () => {
    selected_card.classList.remove('selected');
    selected_card = null;
  }

  const HandleCardClick = (card) => {
    if (selected_card === null){ // No card is selected
      selected_card = card;
      card.classList.add('selected');
    } else {
      const sourceStack = selected_card.parentElement;
      const targetStack = card.parentElement;
  
      if (selected_card === card) { // Same card is selected
        selected_card.classList.remove('selected');
        selected_card = null;
      } else {
        if (sourceStack === targetStack) { // if the card is in the same stack , just change selection
          selected_card.classList.remove('selected');
          card.classList.add('selected');
          selected_card = card;
        } else {
          if (targetStack.lastElementChild === card){ // if the card is the last card in the stack

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
  }  

  const MoveCards = (sourceStack, targetStack, selected_card) => {
    const cardIndex = Array.from(sourceStack.children).indexOf(selected_card);
    const cardsToMove = Array.from(sourceStack.children).slice(cardIndex, sourceStack.children.length);
  
    setStacks((prevStacks) => {
      const newStacks = [...prevStacks];
      
      return newStacks;
    });
  
    cardsToMove.forEach(card => {
      targetStack.appendChild(card);
      card.style.setProperty('--card-index', targetStack.children.length - 1);
    });
  };

 return (
   <div className="solitaire-container">
    <button className="home-button" onClick={() => window.location.href = "/homepage"}>Go to Homepage</button>
      <div class="card-row">
       <div class="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
       <div class="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
       <div class="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
       <div class="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
       <div class="card-filler" id="card-filler"></div>
       <div class="card-slot" id="card-slot" onClick={(e) => HandleSlotClick()}></div>
       <div class="card" id="card">
         <img src="https://tekeye.uk/playing_cards/images/svg_playing_cards/backs/png_96_dpi/red.png" alt="Card Face" class="card-face" />
       </div>
     </div>
     <div class="card-row">
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
                  src={card}
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
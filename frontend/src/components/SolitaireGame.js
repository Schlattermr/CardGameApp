import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/SolitaireGame.css';

const SUIT_COLOR = { HEARTS: 'red', DIAMONDS: 'red', CLUBS: 'black', SPADES: 'black' };
const RANK_ORDER = ['ACE', '2', '3', '4', '5', '6', '7', '8', '9', '10', 'JACK', 'QUEEN', 'KING'];

const rankValue = (value) => RANK_ORDER.indexOf(value) + 1;

const canPlaceOnFoundation = (card, foundation) => {
  if (foundation.length === 0) return card.value === 'ACE';
  const top = foundation[foundation.length - 1];
  return card.suit === top.suit && rankValue(card.value) === rankValue(top.value) + 1;
};

const canPlaceOnStack = (card, stack) => {
  if (stack.length === 0) return card.value === 'KING';
  const top = stack[stack.length - 1];
  if (!top.faceUp) return false;
  return (
    SUIT_COLOR[card.suit] !== SUIT_COLOR[top.suit] &&
    rankValue(card.value) === rankValue(top.value) - 1
  );
};

const SolitaireGame = () => {
  const [stacks, setStacks] = useState(() => Array.from({ length: 7 }, () => []));
  const [foundations, setFoundations] = useState(() => Array.from({ length: 4 }, () => []));
  const [drawPile, setDrawPile] = useState([]);
  const [waste, setWaste] = useState([]);
  const [selectedCard, setSelectedCard] = useState(null);
  const [won, setWon] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    startGame();
  }, []);

  const startGame = async () => {
    try {
      const deckRes = await fetch('https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1');
      const deckData = await deckRes.json();
      const drawRes = await fetch(`https://deckofcardsapi.com/api/deck/${deckData.deck_id}/draw/?count=52`);
      const drawData = await drawRes.json();
      distributeCards(drawData.cards);
    } catch (e) {
      console.error('Error fetching deck:', e);
    }
  };

  const distributeCards = (cards) => {
    const newStacks = Array.from({ length: 7 }, () => []);
    let idx = 0;
    for (let col = 0; col < 7; col++) {
      for (let row = 0; row <= col; row++) {
        newStacks[col].push({ ...cards[idx], faceUp: row === col });
        idx++;
      }
    }
    setStacks(newStacks);
    setDrawPile(cards.slice(idx).map(c => ({ ...c, faceUp: false })));
    setWaste([]);
    setFoundations(Array.from({ length: 4 }, () => []));
    setSelectedCard(null);
    setWon(false);
  };

  const autoMoveToFoundation = (card, source, stackIdx, cardIdx) => {
    const targetIdx = foundations.findIndex(f => canPlaceOnFoundation(card, f));
    if (targetIdx === -1) return;

    const newFoundations = foundations.map((f, i) =>
      i === targetIdx ? [...f, { ...card, faceUp: true }] : f
    );

    if (source === 'waste') {
      setWaste(w => w.slice(0, -1));
    } else if (source === 'tableau') {
      setStacks(prev => prev.map((s, i) => {
        if (i !== stackIdx) return s;
        const newS = s.slice(0, cardIdx);
        if (newS.length > 0 && !newS[newS.length - 1].faceUp) {
          newS[newS.length - 1] = { ...newS[newS.length - 1], faceUp: true };
        }
        return newS;
      }));
    }

    setFoundations(newFoundations);
    setSelectedCard(null);
    if (newFoundations.every(f => f.length === 13)) setWon(true);
  };

  const executeTableauMove = (cardsToMove, targetStackIdx, selection) => {
    const newStacks = stacks.map((s, i) => {
      if (i === targetStackIdx) return [...s, ...cardsToMove.map(c => ({ ...c, faceUp: true }))];
      if (selection.source === 'tableau' && i === selection.stackIndex) {
        const newS = s.slice(0, selection.cardIndex);
        if (newS.length > 0 && !newS[newS.length - 1].faceUp) {
          newS[newS.length - 1] = { ...newS[newS.length - 1], faceUp: true };
        }
        return newS;
      }
      return s;
    });

    if (selection.source === 'waste') setWaste(w => w.slice(0, -1));
    if (selection.source === 'foundation') {
      setFoundations(prev => prev.map((f, i) =>
        i === selection.foundationIndex ? f.slice(0, -1) : f
      ));
    }

    setStacks(newStacks);
    setSelectedCard(null);
  };

  const handleDrawClick = () => {
    setSelectedCard(null);
    if (drawPile.length > 0) {
      setWaste(prev => [...prev, { ...drawPile[drawPile.length - 1], faceUp: true }]);
      setDrawPile(prev => prev.slice(0, -1));
    } else {
      setDrawPile(waste.map(c => ({ ...c, faceUp: false })).reverse());
      setWaste([]);
    }
  };

  const handleWasteClick = () => {
    if (waste.length === 0) return;
    if (selectedCard && selectedCard.source === 'waste') {
      setSelectedCard(null);
    } else {
      setSelectedCard({ source: 'waste', card: waste[waste.length - 1] });
    }
  };

  const handleWasteDoubleClick = () => {
    if (waste.length === 0) return;
    autoMoveToFoundation(waste[waste.length - 1], 'waste', null, null);
  };

  const handleTableauClick = (stackIdx, cardIdx) => {
    const stack = stacks[stackIdx];
    const card = stack[cardIdx];

    if (!selectedCard) {
      if (!card.faceUp) return;
      setSelectedCard({ source: 'tableau', stackIndex: stackIdx, cardIndex: cardIdx, card });
      return;
    }

    if (selectedCard.source === 'tableau' && selectedCard.stackIndex === stackIdx && selectedCard.cardIndex === cardIdx) {
      setSelectedCard(null);
      return;
    }

    // Prevent same-stack moves
    if (selectedCard.source === 'tableau' && selectedCard.stackIndex === stackIdx) {
      if (card.faceUp) {
        setSelectedCard({ source: 'tableau', stackIndex: stackIdx, cardIndex: cardIdx, card });
      } else {
        setSelectedCard(null);
      }
      return;
    }

    const cardsToMove = selectedCard.source === 'tableau'
      ? stacks[selectedCard.stackIndex].slice(selectedCard.cardIndex)
      : [selectedCard.card];

    if (cardIdx === stack.length - 1 && card.faceUp && canPlaceOnStack(cardsToMove[0], stack)) {
      executeTableauMove(cardsToMove, stackIdx, selectedCard);
      return;
    }

    if (card.faceUp) {
      setSelectedCard({ source: 'tableau', stackIndex: stackIdx, cardIndex: cardIdx, card });
    } else {
      setSelectedCard(null);
    }
  };

  const handleTableauDoubleClick = (stackIdx, cardIdx) => {
    const stack = stacks[stackIdx];
    if (cardIdx !== stack.length - 1) return;
    const card = stack[cardIdx];
    if (!card.faceUp) return;
    autoMoveToFoundation(card, 'tableau', stackIdx, cardIdx);
  };

  const handleEmptyStackClick = (stackIdx) => {
    if (!selectedCard) return;
    const cardsToMove = selectedCard.source === 'tableau'
      ? stacks[selectedCard.stackIndex].slice(selectedCard.cardIndex)
      : [selectedCard.card];
    if (cardsToMove[0].value !== 'KING') {
      setSelectedCard(null);
      return;
    }
    executeTableauMove(cardsToMove, stackIdx, selectedCard);
  };

  const handleFoundationClick = (foundIdx) => {
    const foundation = foundations[foundIdx];

    if (!selectedCard) {
      if (foundation.length === 0) return;
      const card = foundation[foundation.length - 1];
      setSelectedCard({ source: 'foundation', foundationIndex: foundIdx, card });
      return;
    }

    if (selectedCard.source === 'foundation' && selectedCard.foundationIndex === foundIdx) {
      setSelectedCard(null);
      return;
    }

    const sourceCard = selectedCard.card;

    // Only the top card of a tableau run can go to foundation
    if (selectedCard.source === 'tableau' && selectedCard.cardIndex !== stacks[selectedCard.stackIndex].length - 1) {
      setSelectedCard(null);
      return;
    }

    if (!canPlaceOnFoundation(sourceCard, foundation)) {
      setSelectedCard(null);
      return;
    }

    if (selectedCard.source === 'foundation') {
      const newFoundations = foundations.map((f, i) => {
        if (i === selectedCard.foundationIndex) return f.slice(0, -1);
        if (i === foundIdx) return [...f, { ...sourceCard, faceUp: true }];
        return f;
      });
      setFoundations(newFoundations);
      setSelectedCard(null);
      if (newFoundations.every(f => f.length === 13)) setWon(true);
      return;
    }

    const newFoundations = foundations.map((f, i) =>
      i === foundIdx ? [...f, { ...sourceCard, faceUp: true }] : f
    );

    if (selectedCard.source === 'waste') {
      setWaste(w => w.slice(0, -1));
    } else if (selectedCard.source === 'tableau') {
      setStacks(prev => prev.map((s, i) => {
        if (i !== selectedCard.stackIndex) return s;
        const newS = s.slice(0, selectedCard.cardIndex);
        if (newS.length > 0 && !newS[newS.length - 1].faceUp) {
          newS[newS.length - 1] = { ...newS[newS.length - 1], faceUp: true };
        }
        return newS;
      }));
    }

    setFoundations(newFoundations);
    setSelectedCard(null);
    if (newFoundations.every(f => f.length === 13)) setWon(true);
  };

  const isSelected = (source, stackIdx, cardIdx, foundIdx) => {
    if (!selectedCard) return false;
    if (source === 'waste') return selectedCard.source === 'waste';
    if (source === 'tableau') {
      return (
        selectedCard.source === 'tableau' &&
        selectedCard.stackIndex === stackIdx &&
        selectedCard.cardIndex <= cardIdx
      );
    }
    if (source === 'foundation') {
      return selectedCard.source === 'foundation' && selectedCard.foundationIndex === foundIdx;
    }
    return false;
  };

  return (
    <div className="solitaire-container">
      <button className="home-button" onClick={() => navigate('/')}>Go to Homepage</button>

      <div className="card-row">
        {foundations.map((foundation, foundIdx) => (
          <div
            key={foundIdx}
            className={`card-slot foundation${isSelected('foundation', null, null, foundIdx) ? ' selected' : ''}`}
            onClick={() => handleFoundationClick(foundIdx)}
          >
            {foundation.length > 0 && (
              <img
                src={foundation[foundation.length - 1].image}
                alt="Foundation card"
                className="card-face"
                draggable="false"
              />
            )}
          </div>
        ))}
        <div className="card-filler"></div>
        <div
          className={`card-slot${waste.length > 0 ? ' card' : ''}${isSelected('waste') ? ' selected' : ''}`}
          onClick={handleWasteClick}
          onDoubleClick={handleWasteDoubleClick}
        >
          {waste.length > 0 && (
            <img
              src={waste[waste.length - 1].image}
              alt="Waste card"
              className="card-face"
              draggable="false"
            />
          )}
        </div>
        <div className="card" onClick={handleDrawClick}>
          {drawPile.length > 0 ? (
            <img src="https://deckofcardsapi.com/static/img/back.png" alt="Card Back" className="card-face" draggable="false" />
          ) : (
            <span className="recycle-icon">↺</span>
          )}
        </div>
      </div>

      <div className="card-row">
        {stacks.map((stack, stackIdx) => (
          <div className="card-stack" key={stackIdx}>
            {stack.length === 0 ? (
              <div className="card-slot" onClick={() => handleEmptyStackClick(stackIdx)}></div>
            ) : (
              stack.map((card, cardIdx) => (
                <div
                  key={cardIdx}
                  className={`card${!card.faceUp ? ' face-down' : ''}${isSelected('tableau', stackIdx, cardIdx) ? ' selected' : ''}`}
                  style={{ '--card-index': cardIdx }}
                  onClick={() => handleTableauClick(stackIdx, cardIdx)}
                  onDoubleClick={() => handleTableauDoubleClick(stackIdx, cardIdx)}
                >
                  <img
                    src={card.faceUp ? card.image : 'https://deckofcardsapi.com/static/img/back.png'}
                    alt={card.faceUp ? `${card.value} of ${card.suit}` : 'Card Back'}
                    className="card-face"
                    draggable="false"
                  />
                </div>
              ))
            )}
          </div>
        ))}
      </div>

      {won && (
        <div className="win-overlay">
          <div className="win-message">
            <h2>You Win!</h2>
            <button onClick={startGame}>Play Again</button>
          </div>
        </div>
      )}
    </div>
  );
};

export default SolitaireGame;

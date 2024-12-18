import React, { useState, useEffect } from "react";
import "../styles/WarGame.css";

const WarGame = () => {
  const [players, setPlayers] = useState([]);
  const [roundWinner, setRoundWinner] = useState("");
  const [allRevealed, setAllRevealed] = useState(false);
  const [gameOver, setGameOver] = useState(false);
  const [finalWinner, setFinalWinner] = useState(""); 


  const getCardImage = (cardNumber, cardSuit) => {
    const suitNames = ['C', 'D', 'H', 'S']; 
    const cardNumberNames = [
      'A', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K' // 0 is for 10
    ];

    if (cardNumber === 14) { // Joker image from web
      return 'https://imgs.search.brave.com/dj6Wo1AUZXzlqN_smDDVScCdyjzZHdH4g7_aRCjoqe4/rs:fit:500:0:0:0/g:ce/aHR0cHM6Ly9tZWRp/YS5nZXR0eWltYWdl/cy5jb20vaWQvNDU4/OTQ1OTcxL3Bob3Rv/L2pva2VyLW9sZC1w/bGF5aW5nLWNhcmQt/ZnJvbS0xOTQwcy5q/cGc_cz02MTJ4NjEy/Jnc9MCZrPTIwJmM9/eDBJWmlNbU12ME5G/ZkNZbjJLdXZLRnRN/b0hkem5Wb1lWQ2dr/QzRlVUtpcz0'; 
    }
  
    const suitName = suitNames[cardSuit];
    const cardNumberName = cardNumberNames[cardNumber - 1];
  
    return `https://deckofcardsapi.com/static/img/${cardNumberName}${suitName}.png`;
  };

  const fetchPlayers = async () => {
    console.log("Fetching players...");
    try {
      const response = await fetch("http://localhost:5013/api/game/players", {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      });

      if (response.ok) {
        const data = await response.json();
        setPlayers(
          data.map((player) => ({
            name: player.username,
            score: 0,
            hand: [], 
          }))
        );
        console.log("Players loaded:", data);
      } else {
        console.error("Failed to fetch players:", response.statusText);
      }
    } catch (err) {
      console.error("Error fetching players:", err);
    }
  };

  const initializeGame = async () => {
    console.log("Initializing game...");
    try {
      const response = await fetch("http://localhost:5013/api/game/initialize", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(players.map((player) => player.name)), 
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Game initialized. Distributed deck:", data);

        setPlayers((prevPlayers) =>
          prevPlayers.map((player) => ({
            ...player,
            hand: data[player.name], 
          }))
        );
      } else {
        console.error("Failed to initialize game:", response.statusText);
      }
    } catch (err) {
      console.error("Error initializing game:", err);
    }
  };


const playRound = () => {
  console.log("Playing round...");
  if (players.every((player) => player.hand.length === 0)) {
    console.log("Game over! No cards left.");
    setRoundWinner("Game Over");
    setGameOver(true); 
    determineFinalWinner();
    return;
  }

  const playedCards = players
    .filter((player) => player.hand.length > 0) 
    .map((player) => player.hand[0]);

  const winningCard = playedCards.reduce((highest, card, idx) => {
 
    if (!highest || card.cardNumber > highest.card.cardNumber) {
      return { card, playerIndex: idx };
    }
  
    if (card.cardNumber === highest.card.cardNumber) {
      const suitRank = [ 'C', 'D', 'H', 'S' ]; 
      const currentCardSuitIndex = suitRank.indexOf(card.cardSuit);
      const highestCardSuitIndex = suitRank.indexOf(highest.card.cardSuit);
      if (currentCardSuitIndex > highestCardSuitIndex) {
        return { card, playerIndex: idx };
      }
    }
    return highest;
  }, null);

  const winnerName = players[winningCard.playerIndex].name;
  console.log(`${winnerName} wins the round!`);

  setPlayers((prevPlayers) =>
    prevPlayers.map((player, idx) => ({
      ...player,
      hand: player.hand.slice(1), 
      score: idx === winningCard.playerIndex ? player.score + 1 : player.score, 
    }))
  );

  setRoundWinner(`${winnerName} won this round!`);
  setAllRevealed(true);
};

  const resetGame = () => {
    console.log("Resetting entire game...");
    setPlayers([]);
    setRoundWinner("");
    setFinalWinner(""); 
    setAllRevealed(false);
    setGameOver(false); 
    fetchPlayers().then(() => initializeGame(true)); 
  };

  const continueGame = () => {
    console.log("Continuing the game...");
    setRoundWinner(""); 
    setAllRevealed(false); 
  };

  const determineFinalWinner = () => {
    const highestScorer = players.reduce(
      (highest, player) =>
        !highest || player.score > highest.score ? player : highest,
      null
    );

    if (highestScorer) {
      console.log(`${highestScorer.name} is the winner with ${highestScorer.score} points!`);
      setFinalWinner(`${highestScorer.name} is the winner with ${highestScorer.score} points!`);
    }
  };

  /* TODO: Finish updating leaderboard */
  const getUserWins = async() => {

  };

  const updateLeaderboard = async(username, gameId, wins) => {
    const requestPayload = {
      username: username,
      gameId: gameId,
      wins: wins
    };

    try {
      const response = await fetch('http://localhost:5013/api/leaderboard/update', {
        method: 'POST',
        body: JSON.stringify(requestPayload)
      });
    } catch (error) {
      console.error('An error occurred while updating the leaderboard:', error);
    }
  };
  /* Done here */
  
  useEffect(() => {
    fetchPlayers(); 
  }, []);

  const navigateToHome = () => {
    window.location.href = "/"; 
  };

  return (
    <div className="war-game">
      {/* Button to navigate back to homepage */}
      <button className="home-button" onClick={navigateToHome}>Go to Homepage</button>
      <div className="player-section">
        {players.map((player, index) => (
          <div key={index} className="player">
            <h2>{player.name}</h2>
            <p>Score: {player.score}</p>
            <p>Cards Remaining: {player.hand.length}</p>
            <div className="card">
              {/* Display the top card image */}
              {player.hand.length > 0 ? (
                <img
                  src={getCardImage(player.hand[0].cardNumber, player.hand[0].cardSuit)}
                  alt={`Card: ${player.hand[0].cardNumber} of ${player.hand[0].cardSuit}`}
                  className="card-image"
                />
              ) : (
                <p>No Cards</p>
              )}
            </div>
          </div>
        ))}
      </div>

      <div className="game-board">
        {roundWinner && <p className="round-result">{roundWinner}</p>}
        {gameOver && finalWinner && (
          <p className="round-result">{finalWinner}</p>
        )}
      </div>

      <div className="game-controls">
        {players.length === 0 ? (
          <button onClick={fetchPlayers}>Fetch Players</button>
        ) : gameOver ? (
          <button onClick={resetGame}>Reset Game</button>
        ) : allRevealed ? (
          <button onClick={continueGame}>Continue Game</button>
        ) : (
          <button onClick={playRound}>Play Round</button>
        )}
      </div>
    </div>
  );
};

export default WarGame;

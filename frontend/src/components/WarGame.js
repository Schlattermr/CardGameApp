import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import "../styles/WarGame.css";

const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5013";

const getCardImage = (cardNumber, cardSuit) => {
  if (cardNumber === 14) return "https://deckofcardsapi.com/static/img/X1.png";
  const suitNames = ["C", "D", "H", "S"];
  const cardNumberNames = ["A", "2", "3", "4", "5", "6", "7", "8", "9", "0", "J", "Q", "K"];
  return `https://deckofcardsapi.com/static/img/${cardNumberNames[cardNumber - 1]}${suitNames[cardSuit]}.png`;
};

const getCardName = (cardNumber, cardSuit) => {
  if (cardNumber === 14) return "Joker";
  const numbers = ["Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King"];
  const suits = ["Clubs", "Diamonds", "Hearts", "Spades"];
  return `${numbers[cardNumber - 1]} of ${suits[cardSuit]}`;
};

const WarGame = () => {
  const navigate = useNavigate();

  const [phase, setPhase] = useState("connecting");
  const [myUsername, setMyUsername] = useState("");
  const [opponentUsername, setOpponentUsername] = useState("");
  const [myCardCount, setMyCardCount] = useState(0);
  const [opponentCardCount, setOpponentCardCount] = useState(0);
  const [myScore, setMyScore] = useState(0);
  const [opponentScore, setOpponentScore] = useState(0);
  const [myCard, setMyCard] = useState(null);
  const [opponentCard, setOpponentCard] = useState(null);
  const [roundWinner, setRoundWinner] = useState(null);
  const [gameWinner, setGameWinner] = useState(null);
  const [roundHistory, setRoundHistory] = useState([]);
  const [iHaveFlipped, setIHaveFlipped] = useState(false);
  const [opponentHasFlipped, setOpponentHasFlipped] = useState(false);
  const [errorMsg, setErrorMsg] = useState("");

  const connectionRef = useRef(null);
  const roundNumberRef = useRef(1);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/wargamehub`)
      .withAutomaticReconnect()
      .build();

    connection.on("WaitingForOpponent", () => {
      setPhase("waiting");
    });

    connection.on("GameStarted", (data) => {
      setMyUsername(data.yourUsername);
      setOpponentUsername(data.opponentUsername);
      setMyCardCount(data.yourCardCount);
      setOpponentCardCount(data.opponentCardCount);
      setMyScore(0);
      setOpponentScore(0);
      setMyCard(null);
      setOpponentCard(null);
      setRoundWinner(null);
      setIHaveFlipped(false);
      setOpponentHasFlipped(false);
      roundNumberRef.current = 1;
      setPhase("playing");
    });

    connection.on("OpponentFlipped", () => {
      setOpponentHasFlipped(true);
    });

    connection.on("RoundResult", (data) => {
      setMyCard(data.myCard);
      setOpponentCard(data.opponentCard);
      setMyCardCount(data.myCardsRemaining);
      setOpponentCardCount(data.opponentCardsRemaining);
      setMyScore(data.myScore);
      setOpponentScore(data.opponentScore);
      setRoundWinner(data.roundWinner);

      setRoundHistory((prev) => [
        ...prev,
        {
          round: roundNumberRef.current,
          winner: data.roundWinner,
          myCard: data.myCard,
          opponentCard: data.opponentCard,
        },
      ]);

      setPhase("result");

      setTimeout(() => {
        setIHaveFlipped(false);
        setOpponentHasFlipped(false);
        setMyCard(null);
        setOpponentCard(null);
        setRoundWinner(null);
        roundNumberRef.current += 1;
        setPhase("playing");
      }, 2500);
    });

    connection.on("GameOver", (data) => {
      setGameWinner(data.winner);
      setMyScore(data.myScore);
      setOpponentScore(data.opponentScore);
      setPhase("gameover");
    });

    connection.on("OpponentDisconnected", () => {
      setPhase("disconnected");
    });

    connection.on("GameFull", () => {
      setErrorMsg("Game is already in progress. Try again later.");
    });

    connectionRef.current = connection;

    connection
      .start()
      .then(() => {
        const username = localStorage.getItem("username") || "Player";
        setMyUsername(username);
        connection.invoke("JoinGame", username);
      })
      .catch(() => setErrorMsg("Could not connect to server."));

    return () => {
      connection.stop();
    };
  }, []);

  const handleFlip = () => {
    if (iHaveFlipped || phase !== "playing") return;
    setIHaveFlipped(true);
    connectionRef.current?.invoke("FlipCard");
  };

  const handlePlayAgain = () => {
    setRoundHistory([]);
    roundNumberRef.current = 1;
    setMyScore(0);
    setOpponentScore(0);
    setGameWinner(null);
    connectionRef.current?.invoke("PlayAgain");
  };

  return (
    <div className="war-game">
      <button className="home-button" onClick={() => navigate("/")}>
        Go to Homepage
      </button>

      {phase === "connecting" && (
        <div className="overlay">
          <p>Connecting...</p>
        </div>
      )}

      {phase === "waiting" && (
        <div className="overlay">
          <p>Waiting for opponent...</p>
        </div>
      )}

      {phase === "disconnected" && (
        <div className="overlay">
          <p>Opponent disconnected.</p>
          <button onClick={() => window.location.reload()}>Play Again</button>
        </div>
      )}

      {errorMsg && (
        <div className="overlay error">
          <p>{errorMsg}</p>
        </div>
      )}

      {phase === "gameover" && (
        <div className="overlay">
          <div className="gameover-box">
            <h2>{gameWinner === "me" ? "🏆 You Win!" : "You Lose"}</h2>
            <p>
              {myScore} – {opponentScore}
            </p>
            <button onClick={handlePlayAgain}>Play Again</button>
            <button onClick={() => navigate("/")}>Home</button>
          </div>
        </div>
      )}

      {["playing", "flipping", "result"].includes(phase) && (
        <>
          <div className="score-bar">
            <span>
              {opponentUsername}: {opponentScore}
            </span>
            <span>vs</span>
            <span>
              {myUsername}: {myScore}
            </span>
          </div>

          <div className="player-area opponent-area">
            <div className="player-label">
              {opponentUsername} — {opponentCardCount} cards
            </div>
            <div
              className={`war-card ${
                opponentHasFlipped && phase !== "result" ? "flipped" : ""
              }`}
            >
              <img
                src={
                  opponentCard
                    ? getCardImage(opponentCard.cardNumber, opponentCard.cardSuit)
                    : "https://deckofcardsapi.com/static/img/back.png"
                }
                alt={
                  opponentCard
                    ? getCardName(opponentCard.cardNumber, opponentCard.cardSuit)
                    : "Card Back"
                }
                draggable="false"
              />
              {opponentHasFlipped && !opponentCard && (
                <div className="flip-indicator">Flipped!</div>
              )}
            </div>
          </div>

          {phase === "result" && (
            <div
              className={`round-banner ${
                roundWinner === "me"
                  ? "win"
                  : roundWinner === "opponent"
                  ? "lose"
                  : "tie"
              }`}
            >
              {roundWinner === "me"
                ? "You win this round!"
                : roundWinner === "opponent"
                ? "Opponent wins this round!"
                : "Tie!"}
            </div>
          )}

          <div className="player-area my-area">
            <div className={`war-card ${iHaveFlipped ? "flipped" : ""}`}>
              <img
                src={
                  myCard
                    ? getCardImage(myCard.cardNumber, myCard.cardSuit)
                    : "https://deckofcardsapi.com/static/img/back.png"
                }
                alt={
                  myCard
                    ? getCardName(myCard.cardNumber, myCard.cardSuit)
                    : "Card Back"
                }
                draggable="false"
              />
            </div>
            <div className="player-label">
              {myUsername} — {myCardCount} cards
            </div>
            <button
              className="flip-button"
              onClick={handleFlip}
              disabled={iHaveFlipped || phase !== "playing"}
            >
              {iHaveFlipped ? "Waiting..." : "Flip!"}
            </button>
          </div>

          <div className="round-history">
            <h4>Round History</h4>
            <ul>
              {[...roundHistory]
                .reverse()
                .slice(0, 8)
                .map((r, i) => (
                  <li
                    key={i}
                    className={
                      r.winner === "me"
                        ? "win"
                        : r.winner === "opponent"
                        ? "lose"
                        : "tie"
                    }
                  >
                    R{r.round}:{" "}
                    {r.winner === "me"
                      ? "You"
                      : r.winner === "opponent"
                      ? opponentUsername
                      : "Tie"}
                    {r.myCard &&
                      ` (${getCardName(
                        r.myCard.cardNumber,
                        r.myCard.cardSuit
                      )} vs ${getCardName(
                        r.opponentCard.cardNumber,
                        r.opponentCard.cardSuit
                      )})`}
                  </li>
                ))}
            </ul>
          </div>
        </>
      )}
    </div>
  );
};

export default WarGame;

import React, { useState } from 'react';
import '../styles/Blackjack.css';

const CARD_BACK_IMAGE = 'https://deckofcardsapi.com/static/img/back.png';

const Blackjack = () => {
    const username = localStorage.getItem('username');

    const [playerHand, setPlayerHand] = useState([]);
    const [dealerHand, setDealerHand] = useState([]);
    const [playerValue, setPlayerValue] = useState(0);
    const [dealerValue, setDealerValue] = useState(0);
    const [roundOver, setRoundOver] = useState(false);
    const [result, setResult] = useState('');
    const [gameStarted, setGameStarted] = useState(false);
    const [message, setMessage] = useState('');

    const getCardImage = (cardNumber, cardSuit, facingUp) => {
        if (!facingUp) {
            return CARD_BACK_IMAGE;
        }

        const suitNames = ['C', 'D', 'H', 'S'];
        const cardNumberNames = [
            'A', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K'
        ];

        const suitName = suitNames[cardSuit];
        const cardNumberName = cardNumberNames[cardNumber - 1];

        return `https://deckofcardsapi.com/static/img/${cardNumberName}${suitName}.png`;
    };

    const applyState = (data) => {
        setPlayerHand(data.playerHand);
        setDealerHand(data.dealerHand);
        setPlayerValue(data.playerValue);
        setDealerValue(data.dealerValue);
        setRoundOver(data.roundOver);
        setResult(data.result);
    };

    const getUserWins = async (name) => {
        try {
            const response = await fetch(`http://localhost:5013/api/leaderboard/wins?username=${name}`, {
                method: 'GET',
            });

            if (response.ok) {
                const data = await response.json();
                return data[0].Wins;
            }
            return null;
        } catch (err) {
            console.error('Error fetching user wins:', err);
            return null;
        }
    };

    const updateLeaderboard = async (name) => {
        const wins = await getUserWins(name);

        const requestPayload = {
            username: name,
            wins: wins != null ? wins + 1 : 1
        };

        try {
            const response = await fetch('http://localhost:5013/api/leaderboard/update', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(requestPayload)
            });

            if (!response.ok) {
                console.error('Failed to update leaderboard:', response.statusText);
            }
        } catch (err) {
            console.error('An error occurred while updating the leaderboard:', err);
        }
    };

    const dealHand = async () => {
        if (!username) {
            setMessage('Please log in to play Blackjack.');
            return;
        }

        setMessage('');
        try {
            const response = await fetch('http://localhost:5013/api/blackjack/start', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(username),
            });

            if (response.ok) {
                const data = await response.json();
                applyState(data);
                setGameStarted(true);
            } else {
                setMessage(await response.text());
            }
        } catch (err) {
            console.error('Error starting Blackjack round:', err);
            setMessage('Unable to start a new round.');
        }
    };

    const hit = async () => {
        try {
            const response = await fetch('http://localhost:5013/api/blackjack/hit', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(username),
            });

            if (response.ok) {
                const data = await response.json();
                applyState(data);
                if (data.roundOver && data.result.startsWith('Player Wins')) {
                    updateLeaderboard(username);
                }
            } else {
                setMessage(await response.text());
            }
        } catch (err) {
            console.error('Error hitting:', err);
            setMessage('Unable to hit.');
        }
    };

    const stand = async () => {
        try {
            const response = await fetch('http://localhost:5013/api/blackjack/stand', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(username),
            });

            if (response.ok) {
                const data = await response.json();
                applyState(data);
                if (data.roundOver && data.result.startsWith('Player Wins')) {
                    updateLeaderboard(username);
                }
            } else {
                setMessage(await response.text());
            }
        } catch (err) {
            console.error('Error standing:', err);
            setMessage('Unable to stand.');
        }
    };

    const navigateToHome = () => {
        window.location.href = '/';
    };

    const renderHand = (hand) => (
        <div className="hand">
            {hand.map((card, index) => (
                <img
                    key={index}
                    src={getCardImage(card.cardNumber, card.cardSuit, card.facingUp)}
                    alt={card.facingUp ? `Card: ${card.cardNumber} of ${card.cardSuit}` : 'Face-down card'}
                    className="card-image"
                    draggable="false"
                />
            ))}
        </div>
    );

    return (
        <div className="blackjack-container">
            <button className="home-button" onClick={navigateToHome}>Go to Homepage</button>
            <h1>Blackjack</h1>

            {!gameStarted ? (
                <div className="game-controls">
                    <button onClick={dealHand}>Deal</button>
                </div>
            ) : (
                <div className="table">
                    <div className="hand-section">
                        <h2>Dealer {roundOver ? `(${dealerValue})` : ''}</h2>
                        {renderHand(dealerHand)}
                    </div>

                    <div className="hand-section">
                        <h2>You ({playerValue})</h2>
                        {renderHand(playerHand)}
                    </div>

                    {result && <p className="round-result">{result}</p>}

                    <div className="game-controls">
                        {roundOver ? (
                            <button onClick={dealHand}>Play Again</button>
                        ) : (
                            <>
                                <button onClick={hit}>Hit</button>
                                <button onClick={stand}>Stand</button>
                            </>
                        )}
                    </div>
                </div>
            )}

            {message && <p className="blackjack-message">{message}</p>}
        </div>
    );
};

export default Blackjack;

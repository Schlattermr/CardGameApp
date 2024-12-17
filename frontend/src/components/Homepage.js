import React, { useState, useEffect } from 'react';
import '../styles/Homepage.css'; // Import the CSS file

const HomePage = () => {
  const [leaderboard, setLeaderboard] = useState([]);

  useEffect(() => {
    async function fetchLeaderboard() {
      try {
        const response = await fetch('http://localhost:5013/api/Leaderboard');
        const data = await response.json();
        setLeaderboard(data);
      } catch (err) {
        console.error('Unable to load leaderboard data', err);
      }
    }
  
    fetchLeaderboard();
  }, []);

  return (
    <div className="homepage-container">
      {/* Left side - Leaderboard */}
      <div className="leaderboard">
        <h2 className="leaderboard-title">Leaderboard</h2>
        <ul className="leaderboard-table">
          {leaderboard.length > 0 ? (
            leaderboard.map((player, index) => (
              <li className="leaderboard-row" key={index}>
                <span className="leaderboard-rank">{index + 1}</span>
                <span className="leaderboard-player">{player.Username}</span>
                <span className="leaderboard-score">{player.Wins} Wins</span>
              </li>
            ))
          ) : (
            <li>No Leaderboard Data Available</li>
          )}
        </ul>
      </div>

      {/* Right side - Play Buttons */}
      <div className="main-content">
        <h1 className="title">Procrastination Pastimes</h1>
        <div className="login-button-container">
          <button className="play-button" onClick={() => window.location.href = "/login"}>
            Login
          </button>
        </div>
        <div className="button-container">
          <button className="play-button" onClick={() => window.location.href = "/waiting-room"}>
            Play War
          </button>
          <button className="play-button" onClick={() => window.location.href = "/solitaire"}>
            Play Solitaire
          </button>
        </div>
      </div>
    </div>
  );
};

export default HomePage;

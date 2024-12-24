import React, { useState, useEffect } from 'react';
import '../styles/Homepage.css';

const HomePage = ({ auth, onLogout }) => {
  const [leaderboard, setLeaderboard] = useState([]);

  useEffect(() => {
    async function fetchLeaderboard() {
      try {
        const response = await fetch('http://localhost:5013/api/Leaderboard', {
          headers: {
            "Authorization": `Bearer ${auth.token}`,
          },
        });
        const data = await response.json();
        setLeaderboard(data);
      } catch (err) {
        console.error('Unable to load leaderboard data', err);
      }
    }
  
    fetchLeaderboard();
  }, [auth.token]);

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

      <div className="login-button-container">
          {auth.token ? (
            <>
              <span className="welcome-text">Welcome, {auth.username}!</span>
              <noscript>Profile Photo for potential profile page: https://www.kohls.com/cnc/media/1.0.46/images/dgsImages/ic-user-profile@3x.png</noscript>
              <button className="play-button" onClick={onLogout}>
                Logout
              </button>
            </>
          ) : (
            <button className="play-button" onClick={() => window.location.href = "/login"}>
              Login
            </button>
          )}
        </div>

      {/* Right side - Play Buttons */}
      <div className="main-content">
        <img className="img" src="procrastinationpastimes.png" alt="Procrastination Pastimes Logo" />
        <div className="button-container">
          <button className="play-button" onClick={() => window.location.href = "/waiting-room"}>
            Play War
          </button>
          <button className="play-button" onClick={() => window.location.href = "/solitaire"}>
            Play Solitaire
          </button>
          <button className="play-button" onClick={() => window.location.href = "/blackjack"}>
            Play Blackjack
          </button>
        </div>
      </div>
    </div>
  );
};

export default HomePage;

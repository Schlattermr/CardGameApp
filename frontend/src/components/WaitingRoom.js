import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Login from "./Login";
import "../styles/WaitingRoom.css";

const WaitingRoom = () => {
  const [players, setPlayers] = useState([]); 
  const maxPlayers = 6; 
  const navigate = useNavigate(); 
  let loadingGif = "https://media.tenor.com/wpSo-8CrXqUAAAAi/loading-loading-forever.gif";

  const fetchPlayers = async () => {
    try {
      const response = await fetch("http://localhost:5013/api/game/players");
      if (response.ok) {
        const data = await response.json();
        setPlayers(data); 
      } else {
        console.error("Failed to fetch players");
      }
    } catch (e) {
      console.error("Error fetching players:", e);
    }
  };

  const addPlayer = async (username) => {
    try {
      const response = await fetch("http://localhost:5013/api/game/addPlayer", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(username),
      });

      if (response.ok) {
        fetchPlayers(); 
      } else if (response.status === 409) {
        console.error("Player is already logged in.");
      } else {
        console.error("Failed to add player");
      }
    } catch (e) {
      console.error("Error adding player:", e);
    }
  };

  useEffect(() => {
    if (players.length === maxPlayers) {
      navigate("/war", { state: { players } }); 
    }
  }, [players, navigate]);

  useEffect(() => {
    fetchPlayers();

    const interval = setInterval(fetchPlayers, 5000);

    return () => clearInterval(interval); 
  }, []);

  return (
    <div className="waiting-room">
      <div className="waiting-area">
        <h2 className="waiting-text">Lobby</h2>
        <img className ="loading-gif" src={loadingGif} alt="Loading..." draggable="false"/>
        <ul>
          {players.map((player, index) => (
            <li key={index}>{player.username}</li>
          ))}
        </ul>
        {players.length < maxPlayers && <p className="waiting-text">Waiting for more players...</p>}
      </div>

      <div className="auth-section">
        <Login onLogin={addPlayer} /> {/* Pass addPlayer as a prop */}
      </div>
    </div>
  );
};

export default WaitingRoom;

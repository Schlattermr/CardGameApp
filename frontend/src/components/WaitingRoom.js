import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Login from "./Login";
import "../styles/WaitingRoom.css";

const WaitingRoom = () => {
  const [players, setPlayers] = useState([]); 
  const maxPlayers = 6; 
  const navigate = useNavigate(); 

 
  const fetchPlayers = async () => {
    try {
      const response = await fetch("http://localhost:5013/api/game/players");
      if (response.ok) {
        const data = await response.json();
        setPlayers(data); 
      } else {
        console.error("Failed to fetch players");
      }
    } catch (error) {
      console.error("Error fetching players:", error);
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
    } catch (error) {
      console.error("Error adding player:", error);
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
        <h2>Waiting Area</h2>
        <ul>
          {players.map((player, index) => (
            <li key={index}>{player.username}</li>
          ))}
        </ul>
        {players.length < maxPlayers && <p>Waiting for more players...</p>}
      </div>

      <div className="auth-section">
        <h2>Login</h2>
        <Login onLogin={addPlayer} /> {/* Pass addPlayer as a prop */}
      </div>
    </div>
  );
};

export default WaitingRoom;

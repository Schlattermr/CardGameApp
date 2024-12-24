import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Login from "./Login";
import "../styles/WaitingRoom.css";

const WaitingRoom = () => {
  const [players, setPlayers] = useState([]); 
  const maxPlayers = 6; 
  const navigate = useNavigate(); 
  const loadingGif = "data:image/svg+xml;charset=utf-8;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPHN2ZyBjbGFzcz0ibGRzLXNwaW5uZXIiIHdpZHRoPSI1MHB4IiBoZWlnaHQ9IjUwcHgiIHN0eWxlPSJiYWNrZ3JvdW5kOm5vbmUiIHByZXNlcnZlQXNwZWN0UmF0aW89InhNaWRZTWlkIiB2aWV3Qm94PSIwIDAgMTAwIDEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGcgdHJhbnNmb3JtPSJyb3RhdGUoMCA1MCA1MCkiPgo8cmVjdCB4PSI0OCIgeT0iMjQiIHdpZHRoPSI0IiBoZWlnaHQ9IjEyIiByeD0iNC44IiByeT0iMi40IiBmaWxsPSIjMzMzIj4KPGFuaW1hdGUgYXR0cmlidXRlTmFtZT0ib3BhY2l0eSIgYmVnaW49Ii0wLjkxNjY2NjY2NjY2NjY2NjZzIiBkdXI9IjFzIiBrZXlUaW1lcz0iMDsxIiByZXBlYXRDb3VudD0iaW5kZWZpbml0ZSIgdmFsdWVzPSIxOzAiLz4KPC9yZWN0Pgo8L2c+CjxnIHRyYW5zZm9ybT0icm90YXRlKDMwIDUwIDUwKSI+CjxyZWN0IHg9IjQ4IiB5PSIyNCIgd2lkdGg9IjQiIGhlaWdodD0iMTIiIHJ4PSI0LjgiIHJ5PSIyLjQiIGZpbGw9IiMzMzMiPgo8YW5pbWF0ZSBhdHRyaWJ1dGVOYW1lPSJvcGFjaXR5IiBiZWdpbj0iLTAuODMzMzMzMzMzMzMzMzMzNHMiIGR1cj0iMXMiIGtleVRpbWVzPSIwOzEiIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiB2YWx1ZXM9IjE7MCIvPgo8L3JlY3Q+CjwvZz4KPGcgdHJhbnNmb3JtPSJyb3RhdGUoNjAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC43NXMiIGR1cj0iMXMiIGtleVRpbWVzPSIwOzEiIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiB2YWx1ZXM9IjE7MCIvPgo8L3JlY3Q+CjwvZz4KPGcgdHJhbnNmb3JtPSJyb3RhdGUoOTAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC42NjY2NjY2NjY2NjY2NjY2cyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgxMjAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC41ODMzMzMzMzMzMzMzMzM0cyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgxNTAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC41cyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgxODAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC40MTY2NjY2NjY2NjY2NjY3cyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgyMTAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC4zMzMzMzMzMzMzMzMzMzMzcyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgyNDAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSItMC4yNXMiIGR1cj0iMXMiIGtleVRpbWVzPSIwOzEiIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiB2YWx1ZXM9IjE7MCIvPgo8L3JlY3Q+CjwvZz4KPGcgdHJhbnNmb3JtPSJyb3RhdGUoMjcwIDUwIDUwKSI+CjxyZWN0IHg9IjQ4IiB5PSIyNCIgd2lkdGg9IjQiIGhlaWdodD0iMTIiIHJ4PSI0LjgiIHJ5PSIyLjQiIGZpbGw9IiMzMzMiPgo8YW5pbWF0ZSBhdHRyaWJ1dGVOYW1lPSJvcGFjaXR5IiBiZWdpbj0iLTAuMTY2NjY2NjY2NjY2NjY2NjZzIiBkdXI9IjFzIiBrZXlUaW1lcz0iMDsxIiByZXBlYXRDb3VudD0iaW5kZWZpbml0ZSIgdmFsdWVzPSIxOzAiLz4KPC9yZWN0Pgo8L2c+CjxnIHRyYW5zZm9ybT0icm90YXRlKDMwMCA1MCA1MCkiPgo8cmVjdCB4PSI0OCIgeT0iMjQiIHdpZHRoPSI0IiBoZWlnaHQ9IjEyIiByeD0iNC44IiByeT0iMi40IiBmaWxsPSIjMzMzIj4KPGFuaW1hdGUgYXR0cmlidXRlTmFtZT0ib3BhY2l0eSIgYmVnaW49Ii0wLjA4MzMzMzMzMzMzMzMzMzMzcyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8ZyB0cmFuc2Zvcm09InJvdGF0ZSgzMzAgNTAgNTApIj4KPHJlY3QgeD0iNDgiIHk9IjI0IiB3aWR0aD0iNCIgaGVpZ2h0PSIxMiIgcng9IjQuOCIgcnk9IjIuNCIgZmlsbD0iIzMzMyI+CjxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9Im9wYWNpdHkiIGJlZ2luPSIwcyIgZHVyPSIxcyIga2V5VGltZXM9IjA7MSIgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIHZhbHVlcz0iMTswIi8+CjwvcmVjdD4KPC9nPgo8L3N2Zz4K";

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
        <h2 className="waiting-text">Lobby</h2>
        <img className ="loading-gif" src={loadingGif} alt="Loading..." />
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

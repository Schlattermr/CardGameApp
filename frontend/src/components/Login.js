import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/Login.css";

const Login = ({ onLogin }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [isFading, setIsFading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch("http://localhost:5013/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (response.ok) {
        const data = await response.json();
        localStorage.setItem("token", data.token);
        localStorage.setItem("username", data.username);

        setMessage("Login successful!");
        if (onLogin) {
          onLogin(data.username, data.token); 
        }

        setIsFading(true);

        setTimeout(() => { // Redirect login to homepage after 1 second, not waiting-room
          if(window.location.href === "http://localhost:3000/login") {
            navigate("/homepage");
          }
        }, 800);
      }
    } catch (e) {
      setMessage("An error occurred. Please try again.");
    }
  };

  return (
    <div className={`auth-container ${isFading ? "fade-out" : ""}`}>
      <h1 className="auth-header">Login</h1>
      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Username:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Enter your username"
            required
          />
        </div>
        <div className="form-group">
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Enter your password"
            required
          />
        </div>
        <button type="submit">Log In</button>
      </form>
      {message && <p className="auth-message">{message}</p>}
      <div className="auth-footer">
        <p>
          Don't have an account? <a href="/register">Register here</a>
        </p>
      </div>
    </div>
  );
};

export default Login;

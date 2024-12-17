import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/Login.css";

const Login = ({ onLogin }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
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
        setMessage("Login successful!");
        if (onLogin) {
          onLogin(username); 
        } else {
          navigate("/homepage");
        }
      } else {
        const error = await response.text();
        setMessage(error || "Login failed.");
      }
    } catch (err) {
      setMessage("An error occurred. Please try again.");
    }
  };

  return (
    <div className="auth-container">
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

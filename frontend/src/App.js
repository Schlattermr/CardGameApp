import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import WarGame from './components/WarGame';
import SolitaireGame from './components/SolitaireGame';
import Homepage from './components/Homepage';
import WaitingRoom from './components/WaitingRoom';

const App = () => {
  const [auth, setAuth] = useState({
    token: localStorage.getItem('token'),
    username: localStorage.getItem('username'),
  });

  useEffect(() => {
    // Check for token in localStorage on initial load
    const token = localStorage.getItem('token');
    const username = localStorage.getItem('username');
    setAuth({ token, username });
  }, []);

  const handleLogin = (username, token) => {
    localStorage.setItem('token', token);
    localStorage.setItem('username', username);
    setAuth({ token, username });
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    setAuth({ token: null, username: null });
  };

  return (
    <div>
      <Routes>
        <Route path="/" element={<Navigate to={auth.token ? "/homepage" : "/login"} />} />
        <Route path="/login" element={<Login onLogin={handleLogin} />} />
        <Route path="/register" element={<Register />} />
        <Route path="/war" element={<WarGame />} />
        <Route path="/solitaire" element={<SolitaireGame />} />
        <Route path="/homepage" element={<Homepage auth={auth} onLogout={handleLogout} />} />
        <Route path="/waiting-room" element={<WaitingRoom />} />
      </Routes>
    </div>
  );
};

export default App;

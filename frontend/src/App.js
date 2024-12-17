import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login'; // Corrected import
import Register from './components/Register'; // Corrected import
import WarGame from './components/WarGame'; // Correct import
import SolitaireGame from './components/SolitaireGame'; // Correct import
import Homepage from './components/Homepage'; 
import WaitingRoom from './components/WaitingRoom'; 

const App = () => {
  return (
    <div>
      <Routes>
        {/* Redirect root to Login */}
        <Route path="/" element={<Navigate to="/homepage" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        {/* Add WarGame and SolitaireGame routes */}
        <Route path="/war" element={<WarGame />} />
        <Route path="/solitaire" element={<SolitaireGame />} />
        <Route path="/homepage" element={<Homepage />} />
        <Route path="/waiting-room" element={<WaitingRoom />} />
      </Routes>
    </div>
  );
};

export default App;

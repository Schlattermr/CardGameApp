import React, { useState, useEffect } from 'react';
import '../styles/Blackjack.css';

const Blackjack = () => {
    const moneyGif = "https://cdn.honey.io/images/findsavings/coin_excited_confetti.gif";

    const navigateToHome = () => { 
        window.location.href = "/";
    };

    return (
        <div className="blackjack-container">
            <button className="home-button" onClick={() => { navigateToHome(); }}>Go to Homepage</button>
            <img className="honey-gif" src={moneyGif} alt="Money Gif" draggable="false"/>
        </div>
    );
};

export default Blackjack;

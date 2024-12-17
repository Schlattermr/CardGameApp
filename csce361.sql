-- Use our csce361 azure db
USE CSCE361;
GO

-- users table creation
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL -- Store hashed passwords for security
);
GO

-- games table creation
CREATE TABLE Games (
    GameId INT PRIMARY KEY IDENTITY(1,1),
    GameName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- put solitaire and war into the games table
INSERT INTO Games (GameName) VALUES ('Solitaire'), ('War');
GO

-- userGames table creation
CREATE TABLE UserGames (
    UserGameId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    GameId INT FOREIGN KEY REFERENCES Games(GameId)
);
GO

-- leaderboards table creation
CREATE TABLE Leaderboards (
    LeaderboardId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    GameId INT FOREIGN KEY REFERENCES Games(GameId),
    Wins INT DEFAULT 0
);
GO

-- here are some test users
INSERT INTO Users (Username, PasswordHash) VALUES ('TestUser1', 'hashedpassword1'), ('TestUser2', 'hashedpassword2');
GO

-- initialize some userGames
INSERT INTO UserGames (UserId, GameId)
SELECT UserId, GameId FROM Users CROSS JOIN Games;
GO

--initialize some leaderboards
INSERT INTO Leaderboards (UserId, GameId, Wins)
SELECT UserId, GameId, 0 FROM Users CROSS JOIN Games;
GO

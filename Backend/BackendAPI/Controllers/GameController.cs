using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Engines;
using Managers;
using Accessors;
using BackendAPI.Models;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private static ConcurrentDictionary<string, bool> LoggedInUsers = new ConcurrentDictionary<string, bool>();

        [HttpGet("players")]
        public IActionResult GetLoggedInPlayers()
        {
            var players = LoggedInUsers.Keys.Select(username => new { Username = username }).ToList();
            return Ok(players);
        }

        [HttpPost("addPlayer")]
        public IActionResult AddLoggedInPlayer([FromBody] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Invalid username.");
            }

            if (LoggedInUsers.ContainsKey(username))
            {
                return Conflict("User is already logged in.");
            }

            LoggedInUsers[username] = true;
            Console.WriteLine($"[INFO] User '{username}' added to the waiting room.");
            return Ok($"User '{username}' added to the waiting room.");
        }

        [HttpPost("initialize")]
        public IActionResult InitializeGame([FromBody] List<string> playerNames)
        {
            if (playerNames == null || playerNames.Count == 0)
            {
                return BadRequest("Player names are required to initialize the game.");
            }
            try
            {
                DeckManager deckManager = new DeckManager();
                Dictionary<string, List<Card>> distributedDeck = deckManager.InitializeAndDistributeDeck(playerNames);

                return Ok(distributedDeck); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to initialize game: {ex.Message}");
                return StatusCode(500, "An error occurred while initializing the game.");
            }
        }

        [HttpPost("reset")]
        public IActionResult ResetGame([FromBody] List<string> playerNames)
        {
            if (playerNames == null || playerNames.Count == 0)
            {
                return BadRequest("Player names are required to reset the game.");
            }

            try
            {
                DeckManager deckManager = new DeckManager();
                Dictionary<string, List<Card>> newDeck = deckManager.InitializeAndDistributeDeck(playerNames);

                Console.WriteLine("[INFO] Game has been reset.");
                return Ok(newDeck); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to reset game: {ex.Message}");
                return StatusCode(500, "An error occurred while resetting the game.");
            }
        }
    }
}

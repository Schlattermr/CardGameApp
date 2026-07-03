using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Backend.Models.DTOs;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/blackjack")]
    public class BlackjackController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, BlackjackRules> Games = new();

        [HttpPost("start")]
        [ProducesResponseType(typeof(BlackjackStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Start([FromBody] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var rules = new BlackjackRules();
            rules.StartRound();
            Games[username] = rules;

            return Ok(BuildState(rules));
        }

        [HttpPost("hit")]
        [ProducesResponseType(typeof(BlackjackStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Hit([FromBody] string username)
        {
            if (!Games.TryGetValue(username, out var rules))
            {
                return NotFound("No active game found for this user. Start a new game first.");
            }

            try
            {
                rules.Hit();
                return Ok(BuildState(rules));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("stand")]
        [ProducesResponseType(typeof(BlackjackStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Stand([FromBody] string username)
        {
            if (!Games.TryGetValue(username, out var rules))
            {
                return NotFound("No active game found for this user. Start a new game first.");
            }

            try
            {
                rules.Stand();
                return Ok(BuildState(rules));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static BlackjackStateResponse BuildState(BlackjackRules rules)
        {
            return new BlackjackStateResponse
            {
                PlayerHand = rules.PlayerHand,
                DealerHand = rules.DealerHand,
                PlayerValue = BlackjackRules.GetHandValue(rules.PlayerHand),
                DealerValue = BlackjackRules.GetHandValue(rules.DealerHand),
                RoundOver = rules.RoundOver,
                Result = rules.GetResult()
            };
        }
    }
}

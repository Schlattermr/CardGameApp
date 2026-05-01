using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Models.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/solitaire")]
    public class SolitaireController : ControllerBase
    {
        private readonly SolitaireRules _solitaireRules;

        public SolitaireController()
        {
            _solitaireRules = new SolitaireRules();
        }

        [HttpPost("move")]
        [ProducesResponseType(typeof(MoveCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult MoveCard([FromBody] MoveCardRequest request)
        {
            try
            {
                _solitaireRules.MoveToTableau(request.SelectedCard, request.SourcePile, request.TargetPile);
                var result = new MoveCardResponse
                {
                    Tableau1 = _solitaireRules.Tableau1,
                    Tableau2 = _solitaireRules.Tableau2,
                    Tableau3 = _solitaireRules.Tableau3,
                    Tableau4 = _solitaireRules.Tableau4,
                    Tableau5 = _solitaireRules.Tableau5,
                    Tableau6 = _solitaireRules.Tableau6,
                    Tableau7 = _solitaireRules.Tableau7,
                    FoundationClubs = _solitaireRules.FoundationClubs,
                    FoundationDiamonds = _solitaireRules.FoundationDiamonds,
                    FoundationHearts = _solitaireRules.FoundationHearts,
                    FoundationSpades = _solitaireRules.FoundationSpades,
                    Stock = _solitaireRules.Stock,
                    Discard = _solitaireRules.Discard
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to move card: {ex.Message}");
            }
        }
    }
}

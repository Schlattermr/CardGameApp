using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Backend.Services;

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
        public IActionResult MoveCard([FromBody] MoveCardRequest request)
        {
            try
            {
                _solitaireRules.MoveToTableau(request.SelectedCard, request.SourcePile, request.TargetPile);
                var result = new
                {
                    _solitaireRules.Tableau1,
                    _solitaireRules.Tableau2,
                    _solitaireRules.Tableau3,
                    _solitaireRules.Tableau4,
                    _solitaireRules.Tableau5,
                    _solitaireRules.Tableau6,
                    _solitaireRules.Tableau7,
                    _solitaireRules.FoundationClubs,
                    _solitaireRules.FoundationDiamonds,
                    _solitaireRules.FoundationHearts,
                    _solitaireRules.FoundationSpades,
                    _solitaireRules.Stock,
                    _solitaireRules.Discard
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to move card: {ex.Message}");
            }
        }
    }

    public class MoveCardRequest
    {
        [Required]
        public required Card SelectedCard 
        { 
            get; set; 
        }

        [Required]
        public required Pile SourcePile 
        { 
            get; set; 
        }

        [Required]
        public required TableauPile TargetPile 
        { 
            get; set; 
        }
    }
}

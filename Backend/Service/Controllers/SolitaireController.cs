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

    public class MoveCardResponse
    {
        public required TableauPile Tableau1 
        { 
            get; set; 
        }

        public required TableauPile Tableau2 
        { 
            get; set; 
        }

        public required TableauPile Tableau3 
        { 
            get; set; 
        }

        public required TableauPile Tableau4 
        { 
            get; set; 
        }

        public required TableauPile Tableau5 
        { 
            get; set;
        }

        public required TableauPile Tableau6 
        { 
            get; set; 
        }

        public required TableauPile Tableau7 
        { 
            get; set; 
        }

        public required FoundationPile FoundationClubs 
        { 
            get; set;
        }

        public required FoundationPile FoundationDiamonds 
        { 
            get; set; 
        }

        public required FoundationPile FoundationHearts 
        { 
            get; set; 
        }

        public required FoundationPile FoundationSpades 
        { 
            get; set; 
        }

        public required Pile Stock 
        { 
            get; set; 
        }

        public required Pile Discard 
        {
                get; set; 
        }
    }
}

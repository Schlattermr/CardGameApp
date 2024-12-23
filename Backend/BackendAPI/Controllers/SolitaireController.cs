using Microsoft.AspNetCore.Mvc;
using Engines;

namespace BackendAPI.Controllers
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
        public Card SelectedCard { get; set; }
        public Pile SourcePile { get; set; }
        public TableauPile TargetPile { get; set; }
    }
}
using Backend.Models.Domain;

namespace Backend.Models.DTOs;

public class BlackjackStateResponse
{
    public required List<Card> PlayerHand { get; set; }

    public required List<Card> DealerHand { get; set; }

    public required int PlayerValue { get; set; }

    public required int DealerValue { get; set; }

    public required bool RoundOver { get; set; }

    public required string Result { get; set; }
}

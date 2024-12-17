namespace Engines;

public interface ITableauPile
{
    /// <summary>
    /// Validates whether a pile follows the tableau restrictions: adjacent numbers, alternating colors
    /// Used for testing/debugging purposes
    /// </summary>
    bool ValidatePile();
}
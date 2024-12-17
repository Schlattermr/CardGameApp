namespace Engines;

public interface IFoundationPile
{
    /// <summary>
    /// Checks if a foundation pile has all cards of its suit
    /// </summary>
    /// <returns>Checks if all 14 same suit cards exist</returns>
    protected bool IsComplete();
}
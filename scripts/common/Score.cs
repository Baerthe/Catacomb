namespace Common;

using Godot;
/// <summary>
/// Manages the score data for an IController player.
/// </summary>
public sealed class Score
{
    public uint CurrentScore { get; private set; }
    private readonly Label _scoreLabel;
    public Score(Label scoreLabel)
    {
        _scoreLabel = scoreLabel;
        CurrentScore = 0;
        UpdateLabel();
    }
    /// <summary>
    /// Adds a point to the current score and updates the label.
    /// </summary>
    public void AddPoint()
    {
        CurrentScore++;
        UpdateLabel();
    }
    public void AddPoints(uint points)
    {
        CurrentScore += points;
        UpdateLabel();
    }
    /// <summary>
    /// Resets the score to zero and updates the label.
    /// </summary>
    public void Reset()
    {
        CurrentScore = 0;
        UpdateLabel();
    }
    /// <summary>
    /// Removes points from the current score and updates the label. If points to remove exceed current score, score is set to zero.
    /// </summary>
    /// <param name="points"></param>
    public void RemovePoints(uint points)
    {
        if (points > CurrentScore)
            CurrentScore = 0;
        else
            CurrentScore -= points;
        UpdateLabel();
    }
    /// <summary>
    /// Updates the score label with the current points.
    /// </summary>
    private void UpdateLabel() =>_scoreLabel.Text = CurrentScore.ToString("D8");
}
namespace Common;

using Godot;
/// <summary>
/// Manages the score data for an IController player.
/// </summary>
public sealed class Score
{
    public byte CurrentScore { get { return _points; }}
    private readonly Label _scoreLabel;
    private byte _points;
    public Score(Label scoreLabel)
    {
        _scoreLabel = scoreLabel;
        _points = 0;
        UpdateLabel();
    }
    /// <summary>
    /// Adds a point to the current score and updates the label.
    /// </summary>
    public void AddPoint()
    {
        _points++;
        UpdateLabel();
    }
    /// <summary>
    /// Resets the score to zero and updates the label.
    /// </summary>
    public void Reset()
    {
        _points = 0;
        UpdateLabel();
    }
    /// <summary>
    /// Updates the score label with the current points.
    /// </summary>
    private void UpdateLabel() =>_scoreLabel.Text = _points.ToString("D8");
}
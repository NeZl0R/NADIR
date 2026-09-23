namespace NADIR.Diagnostics;

internal sealed class FpsCounter
{
    private const double RefreshIntervalSeconds = 0.5;
    private double _elapsedSeconds;
    private int _frameCount;

    public double FramesPerSecond { get; private set; }

    public void Update(double deltaTime)
    {
        if (!double.IsFinite(deltaTime) || deltaTime < 0)
            return;

        _elapsedSeconds += deltaTime;
        _frameCount++;

        if (_elapsedSeconds < RefreshIntervalSeconds)
            return;

        FramesPerSecond = _frameCount / _elapsedSeconds;
        _elapsedSeconds = 0;
        _frameCount = 0;
    }
}

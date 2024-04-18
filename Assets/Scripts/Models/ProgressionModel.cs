public class ProgressionModel
{
    private int _currentValue;
    private int _maxValue;
    private int _displayedValue;

    public int CurrentValue
    {
        get => _currentValue;
    }
    
    public int DisplayValue
    {
        get => _displayedValue;
    }
    
    public int MaXValue
    {
        get => _maxValue;
    }

    public ProgressionModel(int currentValue, int maxValue, int displayedValue)
    {
        _currentValue = currentValue;
        _maxValue = maxValue;
        _displayedValue = displayedValue;
    }
}

using MODBD_Common.NumericTypes.Positive;

namespace MODBD_Common.NumericTypes.Random;

public class Chance : IChance
{
    private readonly IRandom _random;

    public Chance(IRandom random)
    {
        _random = random;
    }

    public bool IsOneOf(StrictlyPositive<int> chancesNumber)
    {
        double threshold = (double) 1 / chancesNumber;
        return _random.NextDouble() < threshold;
    }
}


public interface IChance
{
    public bool IsOneOf(StrictlyPositive<int> chancesNumber);
}

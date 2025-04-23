namespace MODBD_Common.NullableTypes;

public static class CompareToResult
{
    /// <summary>
    /// When comparing not null value to null value ('when other is null' case), we return <see cref="int.MaxValue"/>, <br></br>
    /// so that null values will be displayed first. 
    /// </summary>
    public const int WhenOtherIsNull = int.MaxValue;
    /// <summary>
    /// When comparing null value to not null value ('when this is null case'), we return <see cref="int.MinValue"/>, <br></br>
    /// so that null values will be displayed first. 
    /// </summary>
    public const int WhenThisIsNull = int.MinValue;
}

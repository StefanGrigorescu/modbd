using MODBD_Api.Common.Abstractions.Entities;
using MODBD_Api.Common.NumericTypes.Random;
using MODBD_Api.Common.Text;
using MODBD_Api.Common.Time;

namespace MODBD_Api.Common.Abstractions;

public sealed class UidFactory
{
    /// <summary>
    /// Last two digits of the current year.
    /// </summary>
    private readonly int _yearDigits;
    private readonly int _monthDigits;
    private readonly int _dayDigits;
    private readonly int _hourDigits;
    private readonly IRandom _random;

    public UidFactory(Utc.Snapshot utcSnapshot, IRandom random)
    {
        DateTime utcSnapshotValue = utcSnapshot;
        _yearDigits = utcSnapshotValue.Year % 100;
        _monthDigits = utcSnapshotValue.Month;
        _dayDigits = utcSnapshotValue.Day;
        _hourDigits = utcSnapshotValue.Hour;
        _random = random;
    }

    public long CreateNextUid()
    {
        int random4Digits = _random.Next(1000, 10000); // Generates a random 4-digit number
        return long.Parse(
                    $"{_yearDigits:D2}{_monthDigits:D2}{_dayDigits:D2}{_hourDigits:D2}{random4Digits:D4}");
    }

    public long CreateNextUid(int additionalDigits)
    {
        int random4Digits = _random.Next(1000, 10000); // Generates a random 4-digit number
        return long.Parse(
                    $"{_yearDigits:D2}{_monthDigits:D2}{_dayDigits:D2}{_hourDigits:D2}{additionalDigits}{random4Digits:D4}");
    }
}


public static class CreateUid
{
    public static string FromName(
        string name,
        IReadOnlyList<EntityId<string>> previouslyCreatedIds
    ) {
        NormalizedText normalizedName = NormalizedText.From(name);

        char firstLetter = normalizedName.Value[0];
        string remainingLetters = normalizedName.Value.Substring(1);
        string remainingLettersWithoutVowels = RemoveVowels.Normalize(remainingLetters);

        // If there are at least two consonants, prefer them
        return remainingLettersWithoutVowels.Length >= 2 ?
            From(firstLetter, remainingLettersWithoutVowels, previouslyCreatedIds) :
            From(firstLetter, remainingLetters, previouslyCreatedIds);
    }

    private static string From(
        char firstLetter,
        string remainingLetters,
        IReadOnlyList<EntityId<string>> previouslyCreatedIds
    ) {
        string id = remainingLetters.Length >= 2 ?
            $"{firstLetter}{remainingLetters.Substring(0, 2)}" :    // Get the first two remaining letters
            $"{firstLetter}{remainingLetters.PadRight(2, '0')}";    // or pad with zeroes if there are not two remaining letters

        int otherSimilarIdsCount = previouslyCreatedIds.Count(
            previouslyCreatedId => previouslyCreatedId.Value.Contains(id)
        );

        return otherSimilarIdsCount > 0 ?
            $"{id}{1 + otherSimilarIdsCount}" :
            id;
    }
}

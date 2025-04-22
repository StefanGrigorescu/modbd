using System.Diagnostics.CodeAnalysis;

namespace MODBD_Api.Common.Time.Hours;

public static class TrimTimeUnitsSmallerThan
{
    public static class Minute
    {
        /// <summary>
        /// Creates a new <see cref="DateTime?"/>instance that has the same date as <paramref name="source"/>, 
        /// also same hour and minute, but all other smaller units are trimmed (set to 0). <br></br>
        /// If <paramref name="source"/> is <see langword="null"/>, it returns <see langword="null"/>.
        /// </summary>
        /// <param name="source">The source being copied.</param>
        /// <returns>
        /// A new <see cref="DateTime"/> instance with all time units smaller than minute trimmed (set to 0). <br></br>
        /// If <paramref name="source"/> is <see langword="null"/>, it returns <see langword="null"/>.
        /// </returns>
        public static DateTime? Normalize(
            [NotNullIfNotNull(nameof(source))] DateTime? source
         )
        {
            return source is null ?
                null :
                Normalize(source.Value);
        }

        /// <summary>
        /// Creates a new <see cref="DateTime?"/>instance that has the same date as <paramref name="source"/>, 
        /// also same hour and minute, but all other smaller units are trimmed (set to 0).
        /// </summary>
        /// <param name="source">The source being copied.</param>
        /// <returns>
        /// A new <see cref="DateTime"/> instance with all time units smaller than minute trimmed (set to 0).
        /// </returns>
        public static DateTime Normalize(DateTime source) => new(
            source.Year,
            source.Month,
            source.Day,
            source.Hour,
            source.Minute,
            0
        );
    }


    public static class Hour
    {
        /// <summary>
        /// Creates a new <see cref="DateTime?"/>instance that has the same date as <paramref name="source"/>, 
        /// also same hour, but all other smaller units are trimmed (set to 0). <br></br>
        /// If <paramref name="source"/> is <see langword="null"/>, it returns <see langword="null"/>.
        /// </summary>
        /// <param name="source">The source being copied.</param>
        /// <returns>
        /// A new <see cref="DateTime"/> instance with all time units smaller than minute trimmed (set to 0). <br></br>
        /// If <paramref name="source"/> is <see langword="null"/>, it returns <see langword="null"/>.
        /// </returns>
        public static DateTime? Normalize(
            [NotNullIfNotNull(nameof(source))] DateTime? source
         )
        {
            return source is null ?
                null :
                Normalize(source.Value);
        }

        /// <summary>
        /// Creates a new <see cref="DateTime?"/>instance that has the same date as <paramref name="source"/>, 
        /// also same hour, but all other smaller units are trimmed (set to 0).
        /// </summary>
        /// <param name="source">The source being copied.</param>
        /// <returns>
        /// A new <see cref="DateTime"/> instance with all time units smaller than minute trimmed (set to 0).
        /// </returns>
        public static DateTime Normalize(DateTime source) => new(
            source.Year,
            source.Month,
            source.Day,
            source.Hour,
            0,
            0
        );
    }
}

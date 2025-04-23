using MODBD_Common.Abstractions;

namespace MODBD_Common.Time;

/// <summary>
/// <see cref="DateTime"/> representation of LocalDateTime.<br></br>
/// </summary>
public sealed class LocalDateTime : ValueObject<DateTime>
{
    public static LocalDateTime Now => new() { Value = DateTime.Now };
    private LocalDateTime() : base() { }


    public sealed class Snapshot
    {
        public static implicit operator DateTime(Snapshot snapshot)
        {
            snapshot._capturedValue ??= snapshot.CaptureValue();
            return snapshot._capturedValue;
        }
        public static implicit operator LocalDateTime(Snapshot snapshot)
        {
            snapshot._capturedValue ??= snapshot.CaptureValue();
            return snapshot._capturedValue;
        }
        private LocalDateTime? _capturedValue = null;

        private readonly LocalDateTimeNow _LocalDateTimeNow;

        public Snapshot(LocalDateTimeNow LocalDateTimeNow)
        {
            _LocalDateTimeNow = LocalDateTimeNow;
        }

        /// <summary>
        /// Takes a snapshot of LocalDateTime now.
        /// </summary>
        /// <returns>Captured snapshot of LocalDateTime now.</returns>
        private LocalDateTime CaptureValue() => new()
        {
            Value = _LocalDateTimeNow(),
        };
    }
}


public delegate LocalDateTime LocalDateTimeNow();

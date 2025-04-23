using MODBD_Common.Abstractions;

namespace MODBD_Common.Time;

/// <summary>
/// <see cref="DateTime"/> representation of UTC. <br></br>
/// </summary>
public sealed class Utc : ValueObject<DateTime>
{
    public static Utc Now => new() { Value = DateTime.UtcNow };
    private Utc() : base() { }


    public sealed class Snapshot
    {
        public static implicit operator DateTime(Snapshot snapshot) =>
            snapshot.AsDateTime();
        public DateTime AsDateTime()
        {
            _capturedValue ??= CaptureValue();
            return _capturedValue;
        }
        public static implicit operator Utc(Snapshot snapshot)
        {
            snapshot._capturedValue ??= snapshot.CaptureValue();
            return snapshot._capturedValue;
        }
        private Utc? _capturedValue = null;
        
        private readonly UtcNow _utcNow;

        public Snapshot(UtcNow utcNow)
        {
            _utcNow = utcNow;
        }

        /// <summary>
        /// Takes a snapshot of UTC now.
        /// </summary>
        /// <returns>Captured snapshot of UTC now.</returns>
        private Utc CaptureValue() => new() 
        { 
            Value = _utcNow(),
        };
    }
}


public delegate Utc UtcNow();

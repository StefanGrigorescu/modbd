using MODBD_Core.Schema;

namespace MODBD_Core.Applications.SqlConditions; 

public interface ICondition : IEquatable<ICondition>
{
    IReadOnlyList<Column> Columns { get; }
    string Sql { get; }
    ICondition Not();
}

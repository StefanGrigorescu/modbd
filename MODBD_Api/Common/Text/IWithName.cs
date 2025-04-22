namespace MODBD_Api.Common.Text;

public interface IWithName<TText>
    where TText : Text
{
    TText Name { get; }
}

namespace PMS.Shared.Common.Exceptions;

public class NotFoundException : BaseBusinessException
{
    public override int StatusCode => 404;
    public NotFoundException(string message) : base(message) { }
}

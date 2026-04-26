using PMS.Shared.Common.Exceptions;

namespace Core.Domain.Exceptions;

public class CoreException : BaseBusinessException
{
    public override int StatusCode => 400;

    public CoreException(string message) : base(message) { }
}

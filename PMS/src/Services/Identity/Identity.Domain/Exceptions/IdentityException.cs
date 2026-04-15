using Microsoft.AspNetCore.Identity;
using PMS.Shared.Common.Exceptions;

namespace Identity.Domain.Exceptions;

public class IdentityException : BaseBusinessException
{
    public override int StatusCode => 400;

    public IdentityException(IEnumerable<IdentityError> errors)
        : base(string.Join(";", errors.Select(e => e.Description)))
    {
    }

    public IdentityException(string message)
        : base(message) 
    {
    }
}

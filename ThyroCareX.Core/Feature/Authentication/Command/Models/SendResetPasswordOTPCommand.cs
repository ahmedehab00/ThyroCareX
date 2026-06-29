using MediatR;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.Authentication.Command.Models
{
    public class SendResetPasswordOTPCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}

using MediatR;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.Authentication.Command.Models
{
    public class VerifyResetPasswordOTPCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}

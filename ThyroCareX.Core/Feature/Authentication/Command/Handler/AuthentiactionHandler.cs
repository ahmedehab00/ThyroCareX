using AutoMapper;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Authentication.Command.Models;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Models;
using ThyroCareX.Data.Models.Identity;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Core.Feature.Authentication.Command.Handler
{
    public class AuthentiactionHandler:ResponseHandler,IRequestHandler<RegisterDoctorCommand,Response<string>>
                                                      ,IRequestHandler<SignInCommand,Response<string>>
                                                      ,IRequestHandler<SendResetPasswordOTPCommand,Response<string>>
                                                      ,IRequestHandler<VerifyResetPasswordOTPCommand,Response<string>>
                                                      ,IRequestHandler<ResetPasswordWithOTPCommand,Response<string>>
    {

        #region Fields
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IAuthentcationService _authService;
        private readonly IImageService _imageService;
        private readonly IEmailService _emailService;
        #endregion
        #region Constructor
        public AuthentiactionHandler(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager
                                     , SignInManager<User> signInManager, IDoctorService doctorService,
                                       IMapper mapper, IAuthentcationService authService, IImageService imageService,
                                       IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _doctorService = doctorService;
            _mapper = mapper;
            _authService = authService;
            _imageService = imageService;
            _emailService = emailService;
        }


        #endregion
        #region Handle Functions
        public async Task<Response<string>> Handle(RegisterDoctorCommand request, CancellationToken cancellationToken)
        {

                if (request.Password != request.ConfirmPassword)
                return new Response<string>("Passwords do not match", false);
            
            var roleExists = await _roleManager.RoleExistsAsync("Doctor");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>("Doctor"));
                if (!roleResult.Succeeded)
                    return new Response<string>("Failed to create Doctor role", false);
            }
            if (request.IdentificationImage == null)
                return new Response<string>("Doctor must upload an image", false);

            string imagePath = await _imageService.UploadImageAsync(
                request.IdentificationImage.OpenReadStream(),
                request.IdentificationImage.FileName
            );

            var user = new User
            {
                Email = request.Email.Trim(),
                UserName = request.FullName.Trim(),
                //PasswordHash=request.Password.Trim(),
                //ConfirmPassword=request.ConfirmPassword.Trim(),
                Address=request.Address.Trim(),
                City= request.City.Trim(),
                ZipCode= request.ZipCode.Trim(),
                ImagePath= imagePath,
                Specialization=request.Specialization.Trim(),


            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return new Response<string>(string.Join(", ", result.Errors.Select(e => e.Description)), false);

            await _userManager.AddToRoleAsync(user, "Doctor");
            var doctor = _mapper.Map<Doctor>(request);
            doctor.UserId = user.Id;
            doctor.Status = Data.Enums.DoctorStatus.Pending;
            doctor.ImagePath = imagePath;

            var doctorResult = await _doctorService.AddAsync(doctor);

            await _userManager.AddClaimAsync(user,
               new Claim("UserType", "Doctor"));

            await _userManager.AddClaimAsync(user,
                new Claim("DoctorId", doctor.DoctorID.ToString()));

            return new Response<string>("Doctor Registered Successfully. Status: Pending Approval", true);
        }



        public async Task<Response<string>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {

            //Check If user is exist or not
            var user=await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null)
                return BadRequest<string>("Email Or Password Is Wrong");
            var passwordValid = await _userManager.CheckPasswordAsync(user,request.Password);

            if (!passwordValid)
                return BadRequest<string>("Email Or Password Is Wrong");

            // Check if user is a Doctor
            if (await _userManager.IsInRoleAsync(user, "Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(user.Id);
                if (doctor != null && doctor.Status != DoctorStatus.Approved)
                {
                    return BadRequest<string>($"Your account is currently {doctor.Status}. Please wait for admin approval.");
                }
            }
            //Generate Token
            var accesstoken= await _authService.GetJWTToken(user);
            //return the token
            return Success(accesstoken,"Login Successfully");

        }

        public async Task<Response<string>> Handle(SendResetPasswordOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
                return BadRequest<string>("User not found.");

            // Generate a 6-digit OTP
            var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send Email
            var message = $"<h3>ThyraX Password Reset</h3>" +
                          $"<p>Your OTP to reset your password is: <strong>{otp}</strong></p>" +
                          $"<p>If you didn't request this, please ignore this email.</p>";

            var isSent = await _emailService.SendEmailAsync(user.Email, "ThyraX - Password Reset OTP", message);
            if (!isSent)
                return BadRequest<string>("Failed to send OTP email.");

            return Success("OTP sent to your email successfully.");
        }

        public async Task<Response<string>> Handle(VerifyResetPasswordOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
                return BadRequest<string>("User not found.");

            var isOtpValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.OTP.Trim());
            if (!isOtpValid)
                return BadRequest<string>("Invalid or expired OTP.");

            return Success("OTP is valid.");
        }

        public async Task<Response<string>> Handle(ResetPasswordWithOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
                return BadRequest<string>("User not found.");

            var isOtpValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.OTP.Trim());
            if (!isOtpValid)
                return BadRequest<string>("Invalid or expired OTP.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest<string>(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Success("Password reset successfully.");
        }

        #endregion
    }
}

using System.Security.Cryptography;

namespace WanderoBackEnd.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<EmailDto>> Register(UserRegisterRequest request)
        {
            ServiceResponse<EmailDto> response = new ServiceResponse<EmailDto>();

            if (_context.Users.Any(u => u.Email == request.Email))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return  response;
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };

            EmailDto emailDto = new EmailDto();
            emailDto.To = request.Email;
            emailDto.Subject = "Email Address Verification - Wandero.eu";
            emailDto.Body = $"Verify your email address with this link: <a href=\"https://localhost:7046/api/User/verify?token={user.VerificationToken}\">https://localhost:7046/api/User/verify?token={user.VerificationToken}</a>";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Data = emailDto;
            response.Message = "User successfully created!";
            return response;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<ServiceResponse<String>> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            ServiceResponse<String> response = new ServiceResponse<string>();

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found!";
                return response;
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Password is incorrect!";
                return response;
            }

            if (user.VerifiedAt == null)
            {
                response.Success = false;
                response.Message = "Not verified!";
                return response;
            }

            response.Data = user.Email;
            response.Message = $"Welcome back, {user.Email}! :)";
            return response;
        }

        public async Task<ServiceResponse<String>> Verify(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            ServiceResponse<String> response = new ServiceResponse<string>();

            if (user == null)
            {
                response.Success=false;
                response.Message = "Invalid token!";
                return response;
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            response.Data = "User verified!";
            return response;
        }

        public async Task<ServiceResponse<String>> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            ServiceResponse<String> response = new ServiceResponse<string>();

            if (user == null)
            {
                response.Success = false;
                response.Message = "No user found with the given email address!";
                return response;
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddHours(3);
            await _context.SaveChangesAsync();

            response.Data = "You can reset your password in the next 3 hours!";
            return response;
        }

        public async Task<ServiceResponse<String>> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            ServiceResponse<String> response = new ServiceResponse<string>();

            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                response.Success = false;
                response.Message = "Invalid Token!";
                return response;
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            response.Data = "Password successfully reset!";
            return response;
        }
    }
}
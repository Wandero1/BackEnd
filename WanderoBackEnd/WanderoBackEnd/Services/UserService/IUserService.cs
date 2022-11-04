namespace WanderoBackEnd.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<EmailDto>> Register(UserRegisterRequest request);

        Task<ServiceResponse<String>> Login(UserLoginRequest request);

        Task<ServiceResponse<String>> Verify(string token);

        Task<ServiceResponse<String>> ForgotPassword(string email);

        Task<ServiceResponse<String>> ResetPassword(ResetPasswordRequest request);
    }
}

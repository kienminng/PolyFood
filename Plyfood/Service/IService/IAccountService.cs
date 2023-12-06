using System.Diagnostics.CodeAnalysis;
using Plyfood.Dto.Account;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IAccountService
{
    bool Register(RegisterForm registerForm,[NotNull] string AuthorName);

    AuthenticationResponse? Login(AuthenRequest authenRequest);

    bool GenericResetPasswordToken(string username);
    string? CreateAccessToken(Account account);
    Account? FindByUsername(string usn);

    bool ChangeStatus(string username);

    bool ChangePassword(string username,string oldPassword,string newPassword);
    
    bool ResetPassword(string username, string passwordToken, string newPassword);

    bool UpdateAccount(UpdateAccountApi api,string username);

    bool BandAccount(int id);
}
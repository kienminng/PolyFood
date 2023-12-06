using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.Account;
using Plyfood.Dto.SendMail;
using Plyfood.Entity;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;


namespace Plyfood.Service.Impl;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly IMailSender _mailSender;
    private readonly ITokenService _tokenService;

    public AccountService(AppDbContext context
        , IMailSender mailSender
        , ITokenService tokenService
    )
    {
        _context = context;
        _mailSender = mailSender;
        _mailSender = mailSender;
        _tokenService = tokenService;
        
    }

    public bool Register(RegisterForm registerForm,string AuthorName)
    {
        if (ValidateUsername(registerForm.UserName, registerForm.Email))
        {
            return false;
        }

        try
        {
            Decentralization? decentralization = GetAuthor(AuthorName);
            if (decentralization == null)
            {
                return false;
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    registerForm.Password = EncodeBase64(registerForm.Password);
                     Account account = CreateAccount(registerForm, decentralization);
                    _context.Accounts.Add(account);
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    transaction.Rollback();
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    public Account? FindByUsername(string usn)
    {
        return
            _context.Accounts
                .Include(x=> x.Users)
                .FirstOrDefault(x => x.User_name.Equals(usn));
    }

    public bool BandAccount(int id)
    {
        Account? account = _context.Accounts.Find(id);
        if (account is null)
        {
            throw new Exception("User not found");
        }

        account.Status = 0;
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Accounts.Update(account);
                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return false;
            }
        }
    }

    public bool ChangeStatus(string? username)
    {
        if (username is null)
        {
            return false;
        }

        Account? account = FindByUsername(username);
        if (account is null)
        {
            return false;
        }

        account.Status = 1;
        _context.Accounts.Update(account);
        _context.SaveChanges();
        return true;
    }

    public AuthenticationResponse? Login(AuthenRequest authenRequest)
    {
        authenRequest.Password = EncodeBase64(authenRequest.Password);
        Account? account = _context.Accounts
            .Include(x => x.Users)
            .Include(x=>x.Decentralization)
            .FirstOrDefault(x =>
                x.Password == authenRequest.Password && x.Status == 1 &&
                x.User_name == authenRequest.Username
                || x.Users.FirstOrDefault().Email ==authenRequest.Username);
        if (account == null)
        {
            Console.WriteLine
                ("AccountUser not found");
            return null;
        }

        var token = CreateAccessToken(account);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var AccountToken = _context.Tokens.FirstOrDefault(x => x.AccountId == account.Account_Id);
        if (AccountToken is null)
        {
            Token newRefreshToken = new Token()
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
            };
            AccountToken= newRefreshToken;
        }
        else
        {
            AccountToken.RefreshToken = refreshToken;
            AccountToken.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        }

        account.Token = AccountToken;
        _context.Accounts.Update(account);
        _context.SaveChanges();
        return new AuthenticationResponse()
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    public bool GenericResetPasswordToken(string username)
    {
        User? user = _context.Users.FirstOrDefault(x => x.User_Name.Equals(username) || x.Email.Equals(username));
        if (user is not null)
        {
            Account account = _context.Accounts.FirstOrDefault(x => x.Users.FirstOrDefault().Equals(user));
            account.ResestPasswordToken = GenerateRandomResetToken(20);
            account.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(1);
            MailForm mailForm = new MailForm()
            {
                To = user.Email,
                Subject = "Your password Token",
                Body = $"{account.ResestPasswordToken}"
            };
            _mailSender.SendMail(mailForm);
            _context.Accounts.Update(account);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    public bool UpdateAccount(UpdateAccountApi api,string username)
    {
        Account? account = FindByUsername(username);
        if (account is null)
        {
            return false;
        }

        if (api.Email is not null)
        {
            account.Users.FirstOrDefault().Email = api.Email;
        }

        if (api.Address is not null)
        {
            account.Users.FirstOrDefault().Address = api.Address;
        }

        if (api.Avatar is not null)
        {
            account.Avatar = api.Avatar;
        }

        if (api.Phone is not null)
        {
            account.Users.FirstOrDefault().Phone = api.Phone;
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Accounts.Update(account);
                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return false;
            }
        }
    }

    public bool ResetPassword(string username, string passwordToken, string newPassword)
    {
        Account? account = _context.Accounts
            .FirstOrDefault(x => x.User_name == username || x.Users.FirstOrDefault().Email.Equals(username)
                && x.ResestPasswordToken.Equals(passwordToken) && x.ResetPasswordTokenExpiry >= DateTime.Now);
        if (account is not null)
        {
            account.Password = EncodeBase64(newPassword);
            account.ResestPasswordToken = string.Empty;
            account.ResetPasswordTokenExpiry = null;
            _context.Accounts.Update(account);
            _context.SaveChanges();
            return true;
        }

        return false;
    }


    public bool ForgotPassword(string usernameOrEmail)
    {
        throw new NotImplementedException();
    }

    public bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        var account = FindByUsernameOrEmail(username);
        if (account is null)
        {
            return false;
        }

        if (account.Password.Equals(EncodeBase64(oldPassword)))
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    account.Password = EncodeBase64(newPassword);
                    _context.Accounts.Update(account);
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        return false;
    }


    public string? CreateAccessToken(Account account)
    {
        if (account is null)
        {
            return null;
        }

        Decentralization? Authority = _context.Decentralizations.Find(account.Decentralization_Id);
        string? Authority_name = Authority.Authority_name;
        string? Email = _context.Users.FirstOrDefault(x => x.Account_Id == account.Account_Id).Email;

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, Email),
            new Claim(ClaimTypes.Role, Authority_name),
            new Claim(ClaimTypes.Name, account.User_name)
        };

        var token = _tokenService.GenerateAccessToken(claims);
        return token;
    }


    private Account? FindByUsernameOrEmail(string str)
    {
        var user = _context.Users.FirstOrDefault(x => x.User_Name.Equals(str) || x.Email.Equals(str));
        if (user is not null)
        {
            return _context.Accounts.FirstOrDefault(x => x.Users.FirstOrDefault().Equals(user));
        }

        return null;
    }

    private bool ValidateUsername(string username, string email)
    {
        var User = _context.Users.FirstOrDefault(x => x.User_Name.Equals(username) || x.Email.Equals(email));
        if (User == null)
        {
            return false;
        }

        return true;
    }


    private Decentralization? GetAuthor(string name)
    {
        return
            _context.Decentralizations.FirstOrDefault(x => x.Authority_name.Equals(name));
    }

    private Account CreateAccount(RegisterForm registerForm, Decentralization decentralization)
    {
        return new Account()
        {
            User_name = registerForm.UserName,
            Users = new List<User>()
            {
                new User()
                {
                    User_Name = registerForm.UserName,
                    Phone = registerForm.Phone,
                    Email = registerForm.Email,
                    Address = registerForm.Address,
                    Create_At = DateTime.UtcNow,
                    Update_At = DateTime.UtcNow,
                    Carts = new List<Cart>()
                    {
                        new Cart()
                        {
                            Create_At = DateTime.Now
                        }
                    }
                }
            },
            Avatar = registerForm.Avatar,
            Status = 0,
            Password = registerForm.Password,
            Decentralization_Id = decentralization.Decentralization_Id,
            Decentralization = decentralization
        };
    }

    private string EncodeBase64(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        string encodedPassword = Convert.ToBase64String(passwordBytes);

        return encodedPassword;
    }

    private string GenerateRandomResetToken(int length)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
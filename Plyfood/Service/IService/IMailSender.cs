using Plyfood.Dto.SendMail;
using Plyfood.Entity;

namespace Plyfood.Service.IService;

public interface IMailSender
{
    void SendMail(MailForm mailForm);
    public bool ConfirmEmailByUrl(Account account,string Token);
}
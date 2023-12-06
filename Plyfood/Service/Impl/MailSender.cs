
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Plyfood.Dto.SendMail;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.Service.IService;


public class MailSender : IMailSender
{
    private readonly EmailConfiguration _emailConfig;
    private readonly EmailContent _emailContent;
    private readonly Html _html;

    public MailSender(EmailConfiguration emailConfiguration,EmailContent emailContent,Html html)
    {
        _emailConfig = emailConfiguration;
        _emailContent = emailContent;
        _html = html;
    }

    public void SendMail(MailForm mailForm)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("", _emailConfig.From));
        message.To.Add(new MailboxAddress("",mailForm.To ));
        message.Subject = mailForm.Subject;
        message.Body = new TextPart(TextFormat.Html) { Text = mailForm.Body };

        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port);
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                client.Disconnect(true);
            }


        }
    }
    
    public bool ConfirmEmailByUrl(Account account,string token)
    {
        
        var Url =  token;
        var message = new MimeMessage();
        var str = Uri.EscapeDataString(Url);

        message.From.Add(new MailboxAddress("", _emailConfig.From));
        message.To.Add(new MailboxAddress("",account.Users.FirstOrDefault().Email));
        message.Subject = "Welcome to Poly Food";
        message.Body = new TextPart(TextFormat.Html) { Text = "<h1> Welcome "+account.User_name +" to join with us </h1> " +
                                                              "<h2> Please click to the button to start</h2>" +
                                                              "<a href=\" "+ _emailContent.RegisterConfirmUrl+ $"?Token={str}" +" \"> " +
                                                              _html.Button + "</a>" + 
                                                              "<h3> This is confidential information. </h3>" +
                                                              "<h3> Do not share with anyone </h3>" };
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port);
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                client.Disconnect(true);
            }
        }
    }
}
namespace CleanArch.Application.Common.Interfaces.Services;

public interface IMailService
{
    Task Send(string emailAddressTo, string emailAddressFrom, string password,
        string subject, string body, string smtpHost, int port);
}

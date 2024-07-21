using SMS.App.Models.Entities;
using SMS.App.Models.Config;

namespace SMS.App.Services.Interfaces
{
    public interface IConfigurationService
    {
        IntervalSettings IntervalSettings { get; }
        SmtpSettings SmtpSettings { get; }
        Target[] Targets { get; }
        string SecretKey { get; }
    }
}
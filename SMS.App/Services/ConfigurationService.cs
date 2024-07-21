using Microsoft.Extensions.Configuration;
using SMS.App.Services.Interfaces;
using SMS.App.Models.Exceptions;
using SMS.App.Models.Entities;
using SMS.App.Models.Config;

namespace SMS.App.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Smtp connection settings.
        /// </summary>
        public SmtpSettings SmtpSettings { get; private set; }
        /// <summary>
        /// Check interval settings.
        /// </summary>
        public IntervalSettings IntervalSettings { get; private set; }
        /// <summary>
        /// Checking targets.
        /// </summary>
        public Target[] Targets { get; private set; }
        /// <summary>
        /// Encryption secrey key.
        /// </summary>
        public string SecretKey { get; private set; }

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;

            InitializeConfigValuesFromAppsettings();
        }

        /// <summary>
        /// Get config values from config file.
        /// </summary>
        /// <exception cref="MissingSmtpConfigurationException"></exception>
        /// <exception cref="MissingIntervalSettingsException"></exception>
        /// <exception cref="MissingTargetListException"></exception>
        private void InitializeConfigValuesFromAppsettings()
        {
            SmtpSettings =
                _configuration.GetSection("SmtpSettings").Get<SmtpSettings>()
                ?? throw new MissingSmtpConfigurationException();

            IntervalSettings =
                _configuration.GetSection("Intervals").Get<IntervalSettings>()
                ?? throw new MissingIntervalSettingsException();

            Targets =
                _configuration.GetSection("Targets").Get<Target[]>()
                ?? throw new MissingTargetListException();
               
            SecretKey =
                GetSecretKey();
        }

        /// <summary>
        /// Get secret key from env.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MissingSecretKeyException"></exception>
        private string GetSecretKey()
        {
            DotNetEnv.Env.Load();

            return Environment.GetEnvironmentVariable("SECRET_KEY")
            ?? throw new MissingSecretKeyException();
        }
            

    }
}

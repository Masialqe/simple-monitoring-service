﻿using SMS.App.Models.Enums;

namespace SMS.App.Models.Entities
{
    public record Target
    {
        /// <summary>
        /// Service name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL to call.
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Type of the check.
        /// </summary>
        public CheckTypes Type { get; set; }
    }
}

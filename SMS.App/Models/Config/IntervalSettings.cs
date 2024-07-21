namespace SMS.App.Models.Config
{
    public class IntervalSettings
    {
        /// <summary>
        /// Perform operation once per X time.
        /// </summary>
        public int CheckInterval { get; set; }
        /// <summary>
        /// When to start, 5 = 5:00, 17 = 17:00
        /// </summary>
        public int StartTime { get; set; }
        /// <summary>
        /// When to stop, 5 = 5:00, 17 = 17:00
        /// </summary>
        public int StopTime { get; set; }
    }
}

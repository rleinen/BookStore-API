using BookStore_API.Contracts;

namespace BookStore_API.Services
{
    public class LoggerService : ILoggerService
    {
        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        public void LogDebug(string msg)
        {
            logger.Debug(msg);
        }

        public void LogError(string msg)
        {
            logger.Error(msg);
        }

        public void LogInfo(string msg)
        {
            logger.Info(msg);
        }

        public void LogWarn(string msg)
        {
            logger.Warn(msg);
        }
    }
}

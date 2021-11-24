namespace BookStore_API.Contracts
{
    public interface ILoggerService
    {
        void LogInfo(string msg);
        void LogWarn(string msg);
        void LogDebug(string msg);
        void LogError(string msg);

    }
}

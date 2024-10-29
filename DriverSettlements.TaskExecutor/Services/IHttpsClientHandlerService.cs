namespace DriverSettlements.TaskExecutor.Services
{
    public interface IHttpsClientHandlerService
	{
        HttpMessageHandler GetPlatformMessageHandler();
    }
}


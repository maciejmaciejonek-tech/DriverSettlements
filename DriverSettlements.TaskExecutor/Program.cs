using DriverSettlements.TaskExecutor.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

ILogger<DriverSettlements.TaskExecutor.Task>? logger = null;

try
{
    var cultureInfo = new CultureInfo("en-US");
    Thread.CurrentThread.CurrentCulture = cultureInfo;
    Thread.CurrentThread.CurrentUICulture = cultureInfo;
    CultureInfo.CurrentCulture = cultureInfo;

    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    var config = builder.Build();

    using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .AddLog4Net());
    logger = loggerFactory.CreateLogger<DriverSettlements.TaskExecutor.Task>();

    var serviceConfiguration = new ServiceConfiguration();
    config.GetSection(ServiceConfiguration.SectionName).Bind(serviceConfiguration);
    DriverSettlements.TaskExecutor.Services.UriBuilder.RestUrl = serviceConfiguration.Uri;

    logger.LogInformation("Executing task");
    if (args == null || args.Length != 1)
        throw new Exception("Invalid number of parameters");
    var applicationInstanceId = args[0];
    var task = new DriverSettlements.TaskExecutor.Task(logger);
    await task.Execute(applicationInstanceId);
    logger.LogInformation("Task was executed sucessfully");
}
catch (Exception ex)
{
    if (logger != null)
        logger.LogError(ex, ex.Message);        
    Console.WriteLine(ex.Message);
}
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class RetryService : IRetryService
{
    private readonly ILogger<RetryService> _logger;

    public RetryService(ILogger<RetryService> logger)
    {
        _logger = logger;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, int maxRetries, TimeSpan delay, Func<Exception, bool>? shouldRetry = null)
    {
        if (maxRetries < 1)
        {
            ArgumentOutOfRangeException
                .ThrowIfLessThan(maxRetries, 1);
        }

        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "{Attempt} of {MaxRetries} failes",
                    attempt,
                    maxRetries);

                if (shouldRetry is not null && !shouldRetry(ex)) throw;

                if (attempt < maxRetries)
                {
                    var expBackoff = TimeSpan.FromMilliseconds(
                        delay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    await Task.Delay(expBackoff);
                    continue;
                }

                lastException = ex;
            }
        }

        _logger.LogError(lastException, "Operation failed after {MaxRetries} attempts", maxRetries);

        throw lastException!;
    }
}

using System;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IRetryService
{
    Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation, 
        int maxRetries, 
        TimeSpan delay, 
        Func<Exception, bool>? shouldRetry = null);
}

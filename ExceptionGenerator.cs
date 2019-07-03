namespace CancellationTokenTest
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal class ExceptionGenerator
    {
        public Task CreateTask<T>(string message) where T: Exception
        {
            return Task.Run(async () => {
                await Task.Delay(TimeSpan.FromSeconds(2));
                var messageParam = Expression.Parameter(typeof(string), "message");
                throw Expression.Lambda<Func<string, T>>(
                    Expression.New(
                        typeof(T).GetConstructor(new [] {typeof(string)}),
                        messageParam
                    ), 
                    messageParam
                ).Compile()(message);
            });
        }
    }
}
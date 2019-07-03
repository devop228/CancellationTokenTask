using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await tryMultipleTask(3);
        }

        private static async Task tryMultipleTask(int taskNumber)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.Register(() => {
                Console.WriteLine("Token cancled.");
            });

            // Monitor keyboard input to cancel the task.
            var taskMonitor = Task.Run(() => {
                Console.WriteLine("Press 'C' to cancel the task.");
                if (Console.ReadKey(true).KeyChar.ToString()
                        .ToUpperInvariant() == "C")
                    tokenSource.Cancel();
            });

            var tasks = new List<Task>();
            for (var i = 0; i < taskNumber; i++)
                tasks.Add(new RandomGenerator(i.ToString()).CreateTask(token));
            var exceptionCreator = new ExceptionGenerator();
            tasks.Add(exceptionCreator.CreateTask<ArgumentNullException>(nameof(taskNumber)));
            tasks.Add(exceptionCreator.CreateTask<ArgumentException>("okearg"));
            var allTasks = Task.WhenAll(tasks.ToArray());
            await tryCancellableTask(allTasks);
            Console.WriteLine($"Task end status : {allTasks.Status}");
        }
        private static async Task tryOneTask() 
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.Register(() => {
                Console.WriteLine("Token cancled.");
            });

            // Monitor keyboard input to cancel the task.
            var taskMonitor = Task.Run(() => {
                Console.WriteLine("Press 'C' to cancel the task.");
                if (Console.ReadKey(true).KeyChar.ToString()
                        .ToUpperInvariant() == "C")
                    tokenSource.Cancel();
            });

            var task = new RandomGenerator().CreateTask(token);
            await tryCancellableTask(task);
            Console.WriteLine($"Task end status : {task.Status}");
        }
        private static async Task tryCancellableTask(Task task) 
        {
            try {
                // This statement might raise exceptions.
                // When only one exception was raised, await throws the exception, in 
                // this sample TimeoutException.
                // When multiple exceptions were thrown, await throws the first exception
                // but set the Task.Exception to an AggregateException wrapping all the
                // exceptions thrown.
                await task;
            }
            catch (OperationCanceledException) {
                Console.WriteLine("OperationCanceledException caught...");
                Console.WriteLine($"Task.Exception is \"{task.Exception?.Message ?? "null"}\"");
            }
            catch (TimeoutException) {
                Console.WriteLine("TimeoutException caught...");
                Console.WriteLine($"Task.Exception is \"{task.Exception?.Message ?? "null"}\"");
            }
        }
    }
}

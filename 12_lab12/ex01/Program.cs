namespace ex01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parentTask = Task.Factory.StartNew
            (
                () =>
                {
                    var childTask = new Task
                    (
                        () =>
                        {
                            Thread.Sleep(2000);
                            Console.WriteLine("Child task has finished!");
                        }
                        //TaskCreationOptions.AttachedToParent
                    );
                    childTask.Start();

                    Console.WriteLine("Parent task has finished!");
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Current
            );

            parentTask.Wait();

            Console.WriteLine("Main thread exiting...");
        }
    }
}
namespace ex04
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var progress = new Progress<float>();
            progress.ProgressChanged += (sender, percent) =>
            {
                Console.WriteLine($"TID {Thread.CurrentThread.ManagedThreadId}: ProgressChanged => {percent}%");
            };

            Task<int> task_1 = DoSomeWorkAsync(progress);
            Task<string> task_2 = DoSomeStringWorkAsync(progress);
            Task task_3 = ThrowNotImplementedExceptionAsync();

            int result1 = 0;
            string result2 = string.Empty;

            try
            {
                await Task.WhenAll(task_3, task_1, task_2);

                result1 = await task_1;
                result2 = await task_2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong! => {ex.Message}");

                if (task_1.IsCompletedSuccessfully)
                    result1 = task_1.Result;

                if (task_2.IsCompletedSuccessfully)
                    result2 = task_2.Result;
            }

            // Afișăm rezultatele finale
            Console.WriteLine("\n=== Rezultate finale ===");
            Console.WriteLine($"Rezultatul task_1 (int): {result1}");
            Console.WriteLine($"Rezultatul task_2 (string): {result2}");
        }

        static async Task<int> DoSomeWorkAsync(IProgress<float> progress = null)
        {
            int result = 0;
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(i * 200);
                result += Random.Shared.Next(1, 10);
                Console.Write("Task1: ");
                progress?.Report(i * 10.0f);
            }
            return result;
        }

        static async Task<string> DoSomeStringWorkAsync(IProgress<float> progress = null)
        {
            string result = string.Empty;
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(i * 100);
                result += i;
                Console.Write("Task2: ");
                progress?.Report(i * 10.0f);
            }
            return result;
        }

        static async Task ThrowNotImplementedExceptionAsync()
        {
            await Task.Delay(100);
            Console.WriteLine("Task3 throwing exception...");
            throw new NotImplementedException("Funcționalitatea nu este implementată.");
        }
    }
}
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

            try
            {
                await Task.WhenAll(task_3, task_1, task_2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong! => {ex.Message}");
            }
        }

        static async Task<int> DoSomeWorkAsync(IProgress<float> progress = null)
        {
            int result = 0;
            for (int i = 1; i <= 10; i++)
            {
                // Simulate processing item i
                await Task.Delay(i * 200);

                result += Random.Shared.Next(1, 10);

                // Report progress
                progress?.Report(i * 10.0f);
            }

            return result;
        }

        static async Task<string> DoSomeStringWorkAsync(IProgress<float> progress = null)
        {
            string result = string.Empty;

            for (int i = 1; i <= 10; i++)
            {
                // Simulate processing item i
                await Task.Delay(i * 100);

                result += i;

                // Report progress
                progress?.Report(i * 10.0f);
            }

            return result;
        }

        static async Task ThrowNotImplementedExceptionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
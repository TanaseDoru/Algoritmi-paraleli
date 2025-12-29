namespace ex07
{
    internal class Program
    {
        private static int NUM_OF_ITERATIONS = 50;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Press any key to terminate the program...");

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                var progress_1 = new Progress<float>();
                progress_1.ProgressChanged += (sender, percent) =>
                {
                    Console.WriteLine($"DoWork_1_Async: {percent}%");
                };
                DoWork_1_Async(cts.Token, progress_1);

                var progress_2 = new Progress<float>();
                progress_2.ProgressChanged += (sender, percent) =>
                {
                    Console.WriteLine($"DoWork_2_Async: {percent}%");
                };
                DoWork_2_Async(cts.Token, progress_2);

                cts.CancelAfter(NUM_OF_ITERATIONS * 10 * 100);

                // Which task has finished first?
                Console.ReadKey();
            }
            catch (Exception)
            {
                throw;
            }

        }

        static async Task<int> DoWork_1_Async(CancellationToken cancellationToken, IProgress<float> progress = null)
        {
            int result = 0;
            Random random = new Random();

            for (int i = 1; i <= NUM_OF_ITERATIONS; i++)
            {
                progress?.Report((i * 100.0f) / NUM_OF_ITERATIONS);
                await Task.Delay(random.Next(1, 20) * 100);
                result += i;

                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        static async Task<int> DoWork_2_Async(CancellationToken cancellationToken, IProgress<float> progress = null)
        {
            int result = 0;
            Random random = new Random();

            for (int i = 1; i <= NUM_OF_ITERATIONS; i++)
            {
                progress?.Report((i * 100.0f) / NUM_OF_ITERATIONS);
                await Task.Delay(random.Next(1, 20) * 100);
                result += i;

                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }
    }
}
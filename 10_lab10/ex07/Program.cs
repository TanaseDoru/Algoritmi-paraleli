namespace ex07
{
    internal class Program
    {
        private const int NUM_OF_ITERATIONS = 50;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Astept primul task care termina...");

            using var cts = new CancellationTokenSource();

            var progress1 = new Progress<float>(p => Console.WriteLine($"DoWork_1_Async: {p}%"));
            var progress2 = new Progress<float>(p => Console.WriteLine($"DoWork_2_Async: {p}%"));

            Task<int> task1 = DoWork_1_Async(cts.Token, progress1);
            Task<int> task2 = DoWork_2_Async(cts.Token, progress2);

            Task<int> completedTask = await Task.WhenAny(task1, task2);

            cts.Cancel();

            int result = await completedTask;

            Console.WriteLine($"\nPrimul task terminat a returnat rezultatul: {result}");

            try
            {
                await Task.WhenAll(task1, task2);
            }
            catch (OperationCanceledException)
            {
            }
        }

        static async Task<int> DoWork_1_Async(CancellationToken cancellationToken, IProgress<float>? progress = null)
        {
            int result = 0;
            Random random = new Random();

            for (int i = 1; i <= NUM_OF_ITERATIONS; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(random.Next(1, 20) * 100, cancellationToken);

                result += i;

                progress?.Report((i * 100.0f) / NUM_OF_ITERATIONS);
            }

            return result;
        }

        static async Task<int> DoWork_2_Async(CancellationToken cancellationToken, IProgress<float>? progress = null)
        {
            int result = 0;
            Random random = new Random();

            for (int i = 1; i <= NUM_OF_ITERATIONS; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(random.Next(1, 20) * 100, cancellationToken);

                result += i;

                progress?.Report((i * 100.0f) / NUM_OF_ITERATIONS);
            }

            return result;
        }
    }
}
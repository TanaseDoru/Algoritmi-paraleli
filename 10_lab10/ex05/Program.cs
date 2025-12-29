namespace ex05
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string arg_1 = "1";
            Task<string> task_1 = DoSomeWorkAsync(arg_1);
            string arg_2 = "2";
            Task<string> task_2 = DoSomeWorkAsync(arg_2);
            Task<string> task_3 = DoSomeWorkAsync("3");

            Task<string> result = await Task.WhenAny(task_1, task_2, task_3);

            Console.WriteLine($"Final result: {await result}");

            //await Task.Delay(4000);
        }

        static async Task<string> DoSomeWorkAsync(string id)
        {
            Random random = new Random();
            string result = $"[{id}] => ";

            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(random.Next(1, 10) * 100);
                Console.WriteLine($"[{id}] working {i}...");
                result += i;
            }

            return result;
        }
    }
}
namespace ex06
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Task<string> task_1 = DoSomeWorkAsync(10);
            Task<string> task_2 = DoSomeWorkAsync(2);
            Task<string> task_3 = DoSomeWorkAsync(5);

            Task<string>[] tasks = new[] { task_1, task_2, task_3 };

            while (tasks.Length > 0)
            {
                Task<string> completedTask = await Task.WhenAny(tasks);

                string result = await completedTask;
                Console.WriteLine(result);

                tasks = tasks.Where(t => t != completedTask).ToArray();
            }
        }

        static async Task<string> DoSomeWorkAsync(int times)
        {
            string result = string.Empty;
            for (int i = 0; i < times; i++)
            {
                await Task.Delay(100);
                result += i;
            }
            return $"Task cu {times} iteratii => {result}";
        }
    }
}
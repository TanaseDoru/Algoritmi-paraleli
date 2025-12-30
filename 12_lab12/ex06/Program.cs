using System.Threading.Tasks.Dataflow;

namespace ex06
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            /// 3. Grouping blocks
            /// 3.1. BatchBlock<T>
            BatchBlock<string> batchBlock = new BatchBlock<string>(3);

            // Post items to the batch block
            List<string> persons = new List<string>()
            {
                "Emma Johnson",
                "William Thompson",
                "Mia Taylor",
                "Liam Adams",
                "Charlotte Lewis",
                "Noah Brooks",
                "Ethan Mitchell",
                "Emily Powell",
                "Lucas Reed",
                "Lily Simmons",
                "Daniel Hayes",
                "Isabella Wright",
                "Amelia Davis"
            };
            foreach (string person in persons)
            {
                await batchBlock.SendAsync(person);
            }
            // Signal the end of adding items to the batch block
            batchBlock.Complete();

            // Receive batches from the batch block
            string[] group;
            int groupNumber = 1;
            while (batchBlock.TryReceive(out group))
            {
                Console.WriteLine($"Group {groupNumber}: ");
                foreach (string person in group)
                {
                    Console.WriteLine($"\t{person}");
                }
                groupNumber++;
            }
            Console.WriteLine();

            /// 3.2. JoinBlock<T1, T2> and JoinBlock<T1, T2, T3>
            JoinBlock<string, int> joinBlock = new JoinBlock<string, int>();

            // Post items to the join block
            string[] cars = new string[]
            {
                "Toyota Camry",
                "Ford Mustang",
                "Honda Accord",
                "Chevrolet Silverado",
                "BMW 3 Series",
                "Nissan Rogue",
                "Tesla Model 3",
                "Volkswagen Golf"
            };
            int[] prices = new int[]
            {
                25000,
                35000,
                27000,
                40000,
                45000,
                30000,
                50000,
                22000
            };
            for (int i = 0; i < cars.Length; i++)
            {
                await joinBlock.Target1.SendAsync(cars[i]);
                await joinBlock.Target2.SendAsync(prices[i]);
            }

            // Receive items from the join block
            for (int i = 0; i < cars.Length; i++)
            {
                Tuple<string, int> pair = joinBlock.Receive();
                Console.WriteLine($"{pair.Item1}: {pair.Item2}$");
            }
            Console.WriteLine();

            /// 3.3. BatchedJoinBlock<T1, T2>
        }
    }
}
using System.Threading.Tasks.Dataflow;

namespace ex05
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            /// 2. Execution blocks 
            /// 2.1. ActionBlock<TInput>
            ActionBlock<int> actionBlock
                = new ActionBlock<int>
                (
                    str => Console.WriteLine(str)
                );

            // Post items to the action block
            for (int i = 0; i < 10; i++)
            {
                await actionBlock.SendAsync(i);
            }
            actionBlock.Complete();
            await actionBlock.Completion;

            /// 2.2. TransformBlock<TInput, TOutput>
            TransformBlock<string, string> transformBlock
                = new TransformBlock<string, string>
                (
                    input => input.ToUpper()
                );

            // Post items to the transform block
            string[] strings = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen", "twenty" };
            foreach (string s in strings)
            {
                await transformBlock.SendAsync(s);
            }

            // Receive items from the transform block
            for (int i = 0; i < strings.Length; i++)
            {
                Console.WriteLine(transformBlock.Receive());
            }

            /// 2.3. TransformManyBlock<TInput, TOutput>
            TransformManyBlock<int, int> transformManyBlock
                = new TransformManyBlock<int, int>
                (
                    input =>
                    {
                        List<int> digits = new List<int>();
                        if (input == 0)
                            return new int[] { 0 };
                        while (input > 0)
                        {
                            digits.Add(input % 10);
                            input /= 10;
                        }
                        digits.Reverse();
                        return digits;
                    }
                );

            // Post items to the transform many block
            await transformManyBlock.SendAsync(123456);
            await transformManyBlock.SendAsync(0);
            await transformManyBlock.SendAsync(10020);
            await transformManyBlock.SendAsync(123);
            //await transformManyBlock.SendAsync(-1234);

            // Receive items from the transform many block
            for (int i = 0; i < 6; i++)
            {
                Console.Write(transformManyBlock.Receive());
                Console.Write(" ");
            }
            Console.WriteLine();
            for (int i = 0; i < 1; i++)
            {
                Console.Write(transformManyBlock.Receive());
                Console.Write(" ");
            }
            Console.WriteLine();
            for (int i = 0; i < 5; i++)
            {
                Console.Write(transformManyBlock.Receive());
                Console.Write(" ");
            }
            Console.WriteLine();
            for (int i = 0; i < 3; i++)
            {
                Console.Write(transformManyBlock.Receive());
                Console.Write(" ");
            }
            Console.WriteLine();
            //for (int i = 0; i < 4; i++)
            //{
            //    Console.Write(transformManyBlock.Receive());
            //    Console.Write(" ");
            //}
            //Console.WriteLine();
        }
    }
}
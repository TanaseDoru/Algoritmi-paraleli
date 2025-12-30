using System.Threading.Tasks.Dataflow;

namespace ex07
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Sources
            BufferBlock<Wood> sourceWood = new BufferBlock<Wood>();
            BufferBlock<Stone> sourceStone = new BufferBlock<Stone>();
            BufferBlock<Iron> sourceIron = new BufferBlock<Iron>();

            // Joins
            JoinBlock<Wood, Stone> joinWoodStoneBlock = new JoinBlock<Wood, Stone>
            (
                new GroupingDataflowBlockOptions
                {
                    Greedy = true
                }
            );
            JoinBlock<Wood, Iron> joinWoodIronBlock = new JoinBlock<Wood, Iron>
            (
                new GroupingDataflowBlockOptions
                {
                    Greedy = true
                }
            );

            // Actions
            ActionBlock<Tuple<Wood, Stone>> actionWoodStone = new ActionBlock<Tuple<Wood, Stone>>
            (
                async resource =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(100);
                        Console.WriteLine($"Wood + Stone {i + 1}/10");
                    }
                }
            );
            ActionBlock<Tuple<Wood, Iron>> actionWoodIron = new ActionBlock<Tuple<Wood, Iron>>
            (
                async resource =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(100);
                        Console.WriteLine($"Wood + Iron {i + 1}/10");
                    }
                }
            );

            // Link sources to join blocks
            sourceWood.LinkTo(joinWoodStoneBlock.Target1);
            sourceWood.LinkTo(joinWoodIronBlock.Target1);
            sourceStone.LinkTo(joinWoodStoneBlock.Target2);
            sourceIron.LinkTo(joinWoodIronBlock.Target2);

            // Link join blocks to action blocks
            joinWoodStoneBlock.LinkTo(actionWoodStone);
            joinWoodIronBlock.LinkTo(actionWoodIron);

            Random random = new Random();

            // Feed data into sources
            for (int i = 0; i < 10; i++)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(random.Next(1, 10) * 100);
                    sourceStone.Post(new Stone());
                });
                Task.Run(async () =>
                {
                    await Task.Delay(random.Next(1, 10) * 100);
                    sourceIron.Post(new Iron());
                });
                Task.Run(async () =>
                {
                    await Task.Delay(random.Next(1, 10) * 100);
                    sourceWood.Post(new Wood());
                });
            }

            Console.ReadKey();
        }
    }
}
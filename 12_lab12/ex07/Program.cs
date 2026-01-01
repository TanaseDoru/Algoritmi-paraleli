using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ex07
{

    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Sources
            var sourceWood = new BufferBlock<Wood>();
            var sourceStone = new BufferBlock<Stone>();
            var sourceIron = new BufferBlock<Iron>();

            // Join blocks - 
            var joinWoodStone = new JoinBlock<Wood, Stone>(new GroupingDataflowBlockOptions { Greedy = false });
            var joinWoodIron = new JoinBlock<Wood, Iron>(new GroupingDataflowBlockOptions { Greedy = false });

            // Action blocks
            var actionWoodStone = new ActionBlock<Tuple<Wood, Stone>>(async _ =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    Console.WriteLine($"Wood + Stone {i + 1}/10");
                }
            });

            var actionWoodIron = new ActionBlock<Tuple<Wood, Iron>>(async _ =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    Console.WriteLine($"Wood + Iron {i + 1}/10");
                }
            });

            // Link-uri cu PropagateCompletion = true
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            sourceWood.LinkTo(joinWoodStone.Target1, linkOptions);
            sourceWood.LinkTo(joinWoodIron.Target1, linkOptions);
            sourceStone.LinkTo(joinWoodStone.Target2, linkOptions);
            sourceIron.LinkTo(joinWoodIron.Target2, linkOptions);

            joinWoodStone.LinkTo(actionWoodStone, linkOptions);
            joinWoodIron.LinkTo(actionWoodIron, linkOptions);

            var random = new Random();
            var producerTasks = new Task[30]; // 10 x 3

            for (int i = 0, idx = 0; i < 10; i++)
            {
                producerTasks[idx++] = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(50, 300));
                    sourceWood.Post(new Wood());
                });

                producerTasks[idx++] = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(50, 300));
                    sourceStone.Post(new Stone());
                });

                producerTasks[idx++] = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(50, 300));
                    sourceIron.Post(new Iron());
                });
            }

            // Așteptăm producătorii
            await Task.WhenAll(producerTasks);

            // Semnalăm finalul surselor
            sourceWood.Complete();
            sourceStone.Complete();
            sourceIron.Complete();

            // Așteptăm finalizarea acțiunilor
            await Task.WhenAll(actionWoodStone.Completion, actionWoodIron.Completion);

            Console.WriteLine("Procesare finalizată.");
        }
    }
}
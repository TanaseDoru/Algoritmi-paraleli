using System.Threading.Tasks.Dataflow;

namespace ex04
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            /// 1. Buffering blocks 
            /// 1.1. BufferBlock<T>
            BufferBlock<int> bufferBlock = new BufferBlock<int>();

            // Posting items to the buffer block
            for (int i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }
            // Receiving items from the buffer block
            for (int i = 0; i < 10; i++)
            {
                //int nr = bufferBlock.Receive(); // sync
                int nr = await bufferBlock.ReceiveAsync(); // async
            }

            /// 1.2. BroadcastBlock<T>
            BroadcastBlock<string> broadcastBlock = new BroadcastBlock<string>(null); // What it the cloning function

            // Post an item to the broadcast block
            broadcastBlock.Post("Low battery!");
            //broadcastBlock.Post("Battery sufficiently charged!");

            // Receive the item (multiple times) from the broadcast block
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(broadcastBlock.Receive());
            }

            /// 1.3. WriteOnceBlock<T>
            WriteOnceBlock<int> writeOnceBlock = new WriteOnceBlock<int>(null);

            // Post an item to the write-once block
            await writeOnceBlock.SendAsync(5);
            //await writeOnceBlock.SendAsync(6);

            // Receive the item from the write-once block
            Console.WriteLine(await writeOnceBlock.ReceiveAsync());
        }
    }
}
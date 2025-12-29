using System.Diagnostics;

namespace ex02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int numThreads = 8;

            string booksString = File.ReadAllText("booksLarge.txt");
            //string substring = "I need no medicine";//"This eBook is for the use of anyone anywhere";
            string substring = @"Here again: Mr. Lorry’s inquiries into Miss Pross’s personal history had
established the fact that her brother Solomon was a heartless scoundrel
who had stripped her of everything she possessed, as a stake to
speculate with, and had abandoned her in her poverty for evermore, with
no touch of compunction. Miss Pross’s fidelity of belief in Solomon
(deducting a mere trifle for this slight mistake) was quite a serious
matter with Mr. Lorry, and had its weight in his good opinion of her.

“As we happen to be alone for the moment, and are both people of
business,” he said, when they had got back to the drawing-room and had
sat down there in friendly relations, “let me ask you--does the Doctor,
in talking with Lucie, never refer to the shoemaking time, yet?”

“Never.”

“And yet keeps that bench and those tools beside him?”

“Ah!” returned Miss Pross, shaking her head. “But I don’t say he don’t
refer to it within himself.”

“Do you believe that he thinks of it much?”

“I do,” said Miss Pross.

“Do you imagine--” Mr. Lorry had begun, when Miss Pross took him up
short with:

“Never imagine anything. Have no imagination at all.”

“I stand corrected; do you suppose--you go so far as to suppose,
sometimes?”

“Now and then,” said Miss Pross.
";

            CancellationTokenSource cts = new CancellationTokenSource();
            Thread[] threads = new Thread[numThreads];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            /////////////////////////////////////////////////////////////////////////
            // Implement your solution here

            int foundIndex = -1;
            object lockObj = new object();

            void SearchWorker(int threadId, int startInclusive, int endExclusive, CancellationToken token)
            {
                int n = booksString.Length;
                int m = substring.Length;
                for (int i = startInclusive; i < endExclusive && i <= n - m; i++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    bool match = true;

                    


                    for (int j = 0; j < m; j++)
                    {
                        if (i + j >= endExclusive)
                        {
                            if (booksString[i + j] != substring[j])
                            {
                                match = false;
                                break;
                            }
                        }
                        else if (booksString[i + j] != substring[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        lock (lockObj)
                        {
                            if (foundIndex == -1)
                            {
                                foundIndex = i;
                                Console.WriteLine($"Thread {threadId}: Found at [{i}].");
                                cts.Cancel();
                            }
                        }
                        return;
                    }
                }
            }

            int chunkSize = booksString.Length / numThreads;
            int remainder = booksString.Length % numThreads;

            var threadsList = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int threadId = i;
                int start = i * chunkSize + Math.Min(i, remainder);
                int end = (i == numThreads - 1) ? booksString.Length : (i + 1) * chunkSize + Math.Min(i + 1, remainder);

                var thread = new Thread(() =>
                {
                    SearchWorker(threadId, start, end, cts.Token);
                });
                thread.IsBackground = true;
                threadsList.Add(thread);
                thread.Start();
            }

            foreach (var t in threadsList)
                t.Join();

            /////////////////////////////////////////////////////////////////////////

            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
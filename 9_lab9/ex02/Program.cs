using System.Diagnostics;

namespace ex02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int numThreads = 2;

            string booksString = File.ReadAllText("booksLarge.txt");
            //string substring = "I need no medicine";//"This eBook is for the use of anyone anywhere";
            string substring = @"Section 3. Information about the Project Gutenberg Literary
Archive Foundation

The Project Gutenberg Literary Archive Foundation is a non-profit
501(c)(3) educational corporation organized under the laws of the
state of Mississippi and granted tax exempt status by the Internal
Revenue Service. The Foundation's EIN or federal tax identification
number is 64-6221541. Contributions to the Project Gutenberg Literary
Archive Foundation are tax deductible to the full extent permitted by
U.S. federal laws and your state's laws.

The Foundation's business office is located at 809 North 1500 West,
Salt Lake City, UT 84116, (801) 596-1887. Email contact links and up
to date contact information can be found at the Foundation's website
and official page at www.gutenberg.org/contact

Section 4. Information about Donations to the Project Gutenberg
Literary Archive Foundation";

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
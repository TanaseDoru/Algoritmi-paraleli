using ex04;
using System.Diagnostics;

if (args.Length < 1)
{
    Console.WriteLine($"Not enough parameters: ./program N P");
    return;
}

try
{
    Console.WriteLine("====================Program Start====================");
    Console.WriteLine($"Current time: {DateTime.Now.ToString("HH:mm:ss:ms")}");
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    int N = int.Parse(args[0]);
    int P = int.Parse(args[1]);

    int[] v = new int[N];

    init(v);
    //print(v);

    // Write generated vector to file
    string filename = Constants.FILE_PREFIX + "_in." + Constants.FILE_SUFFIX;
    write(v, filename);

    ////////////////////////////////////////////////////////////////////////
    // TODO: Start P worker threads here
    Thread[] threads = new Thread[P];
    for (int tid = 0; tid < P; tid++)
    {
        int localTid = tid; // capture variable for closure
        threads[tid] = new Thread(() => threadFunction(v, localTid, P));
        threads[tid].Start();
    }

    foreach (var t in threads)
        t.Join();
    ////////////////////////////////////////////////////////////////////////

    stopwatch.Stop();
    Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
    Console.WriteLine($"Current time: {DateTime.Now.ToString("HH:mm:ss:ms")}");
    Console.WriteLine("====================Program END====================");

}
catch (Exception)
{
    Console.WriteLine("Something went wrong!");
    throw;
}

static void init(int[] v)
{
    for (int i = 0; i < v.Length; i++)
    {
        v[i] = i;
    }
}

static void print(int[] v)
{
    for (int i = 0; i < v.Length; i++)
    {
        Console.Write(v[i]);
        Console.Write(' ');
    }
    Console.WriteLine();
}

static void write(int[] v, string filename)
{
    File.WriteAllText(filename, string.Join(" ", v));
}

static void threadFunction(int[] v, int tid, int P)
{
    int N = v.Length;
    int start = tid * (int)Math.Ceiling((double)N / (double)P);
    int end = (int)Math.Min((double)N, (tid + 1) * Math.Ceiling((double)N / (double)P));
    int[] primes = Enumerable.Repeat<int>(-1, end - start).ToArray();
    int pi = 0;

    for (int vi = start; vi < end; vi++)
    {
        if (isPrime(v[vi]))
        {
            primes[pi++] = v[vi];
        }
    }

    Console.WriteLine($"Thread {tid}: [{start}, {end}] => {pi}");
    string filename = Constants.FILE_PREFIX + "_" + tid + "_out." + Constants.FILE_SUFFIX;
    write(primes.Where(p => p != -1).ToArray(), filename);
}

static bool isPrime(int n)
{
    if (n > 1)
    {
        return Enumerable.Range(1, n).Where(x => n % x == 0)
                         .SequenceEqual(new[] { 1, n });
    }

    return false;
}
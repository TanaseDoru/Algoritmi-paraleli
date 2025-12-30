using System.Collections.Concurrent;

namespace ex05
{
    internal class Program
    {
        static readonly int ARRAY_SIZE = 1000000;
        static int primeCount = 0;

        static void Main(string[] args)
        {
            int[] v = new int[ARRAY_SIZE];
            var primes = new ConcurrentBag<int>();

            init(v);
            //print(v);

            Parallel.ForEach(v, i =>
            {
                if (IsPrime(i))
                {
                    primes.Add(i);
                }
            });

            primeCount = primes.Count;
            File.WriteAllLines("primes_out.txt", primes.OrderBy(x => x).Select(x => x.ToString()));
            File.WriteAllText("primes_out_count.txt", primeCount.ToString());
            Console.WriteLine("S-a terminat executia");
            Console.ReadKey();

        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int j = 3; j <= limit; j += 2)
            {
                if (n % j == 0)
                    return false;
            }
            return true;
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
    }
}
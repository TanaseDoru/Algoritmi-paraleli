using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace ex11
{
    internal class Program
    {
        static readonly int ARRAY_SIZE = 100;
        static readonly int SECTION_COUNT = 4; // Numar parametrizabil de sectiuni

        static void Main(string[] args)
        {
            int[] v = new int[ARRAY_SIZE];
            init(v);

            var primes = new ConcurrentBag<int>();
            int primeCount = 0;

            int sectionSize = ARRAY_SIZE / SECTION_COUNT;
            Action[] actions = new Action[SECTION_COUNT];

            for (int i = 0; i < SECTION_COUNT; i++)
            {
                int start = i * sectionSize;
                int end = (i == SECTION_COUNT - 1) ? ARRAY_SIZE : (i + 1) * sectionSize;

                actions[i] = () => ProcessSection(v, start, end, primes);
            }

            Parallel.Invoke(actions);

            primeCount = primes.Count;

            File.WriteAllLines("primes_out.txt", primes.OrderBy(x => x).Select(x => x.ToString()));
            File.WriteAllText("primes_out_count.txt", primeCount.ToString());

            Console.WriteLine($"Gasite {primeCount} numere prime.");
            Console.ReadKey();
        }

        static void ProcessSection(int[] array, int start, int end, ConcurrentBag<int> primes)
        {
            for (int i = start; i < end; i++)
            {
                if (IsPrime(array[i]))
                {
                    primes.Add(array[i]);
                }
            }
        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int i = 3; i <= limit; i += 2)
            {
                if (n % i == 0) return false;
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
    }
}
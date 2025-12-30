using System;
using System.IO;
using System.Threading.Tasks;

namespace ex09
{
    internal class Program
    {
        static readonly int ARRAY_SIZE = 1000000;

        static void Main(string[] args)
        {
            int[] v = new int[ARRAY_SIZE];
            init(v);

            int primeCount = ParallelCountPrimes(v);

            File.WriteAllText("primes_out_count.txt", primeCount.ToString());

            Console.WriteLine("Numar nr prime: " + primeCount);
            Console.ReadKey();
        }

        static int ParallelCountPrimes(int[] numbers)
        {
            int totalCount = 0;
            object lockObj = new object();

            Parallel.ForEach(
                source: numbers,
                localInit: () => 0,
                body: (item, state, localCount) =>
                {
                    if (IsPrime(item))
                    {
                        return localCount + 1;
                    }
                    return localCount;
                },
                localFinally: localCount =>
                {
                    lock (lockObj)
                    {
                        totalCount += localCount;
                    }
                }
            );

            return totalCount;
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
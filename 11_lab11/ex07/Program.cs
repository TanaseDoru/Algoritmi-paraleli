using System;
using System.Threading.Tasks;

namespace ex07
{
    internal class Program
    {
        static readonly int ARRAY_SIZE = 1000000;

        static void Main(string[] args)
        {
            int[] v = new int[ARRAY_SIZE];
            init(v);

            int firstPrime = -1;

            Parallel.ForEach(v, (x, state) =>
            {
                if (x >= 2 && IsPrime(x))
                {
                    if (System.Threading.Interlocked.CompareExchange(ref firstPrime, x, -1) == -1)
                    {
                        Console.WriteLine($"Am gasit: {x}");
                        state.Stop();
                    }
                }
            });

            if (firstPrime == -1)
            {
                Console.WriteLine("Nu am gasit nr prim.");
            }

            Console.ReadKey();
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
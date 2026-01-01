using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ex03
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            string rootPath = "C:\\Users\\Tanase\\Desktop\\DotNet Projects\\Algoritmi-paraleli\\12_lab12\\ex03";

            if (!Directory.Exists(rootPath))
            {
                Console.WriteLine("Eroare: Directorul specificat nu exista.");
                return;
            }

            var results = new DirectoryResults();

            await Task.Run(() => ProcessDirectory(rootPath, results));

            var latest = results.LastWritten
                .OrderByDescending(x => x.Item2)
                .FirstOrDefault();

            Console.WriteLine($"Files count: {results.FileCount}");
            Console.WriteLine($"Folders count: {results.FolderCount}");
            Console.WriteLine($"Total file size: {results.TotalSize} bytes");

            if (results.FileCount > 0)
            {
                Console.WriteLine($"Last written file: {Path.GetFileName(latest.Item1)}");
                Console.WriteLine($"Last written file time: {latest.Item2:MM/dd/yyyy HH:mm:ss}");
            }
            else
            {
                Console.WriteLine("Last written file: none");
                Console.WriteLine("Last written file time: -");
            }
        }

        static void ProcessDirectory(string path, DirectoryResults results)
        {
            string[] subDirs;
            string[] files;

            try
            {
                subDirs = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            }
            catch
            {
                return;
            }

            Interlocked.Add(ref results.FolderCount, subDirs.Length);

            foreach (var file in files)
            {
                FileInfo fi;
                try
                {
                    fi = new FileInfo(file);
                }
                catch
                {
                    continue;
                }

                Interlocked.Increment(ref results.FileCount);
                Interlocked.Add(ref results.TotalSize, fi.Length);
                results.LastWritten.Add((fi.FullName, fi.LastWriteTime));
            }

            if (subDirs.Length > 0)
            {
                Parallel.ForEach(subDirs, subDir =>
                {
                    ProcessDirectory(subDir, results);
                });
            }
        }
    }

    class DirectoryResults
    {
        public long TotalSize = 0;
        public int FileCount = 0;
        public int FolderCount = 0;
        public ConcurrentBag<(string, DateTime)> LastWritten = new ConcurrentBag<(string, DateTime)>();
    }
}
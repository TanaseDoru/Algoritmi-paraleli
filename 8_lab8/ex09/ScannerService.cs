namespace ex09
{
    public class ScannerService
    {
        private readonly Mutex _mutex = new Mutex(false, "AV_0xAAEC");
        private readonly string _logFilename;

        public ScannerService(string logfilename)
        {
            _logFilename = logfilename;
        }

        public void Scan()
        {
            bool mutexOwned = false;

            try
            {
                mutexOwned = _mutex.WaitOne(1000);

                if (!mutexOwned)
                {
                    Console.WriteLine($"[{Environment.ProcessId}] A scanning session is already running");
                    return;
                }

                Console.WriteLine($"[{Environment.ProcessId}] Scanning your device for malware...");
                List<string> results = new List<string>();

                for (int ri = 0; ri < 50; ri++)
                {
                    string line =
                        $"{DateTime.Now} - PID=[{Environment.ProcessId}] - File['{ri}'] is {(Random.Shared.Next() % 3) switch { 0 => "CLEAN", 1 => "MALWARE", 2 => "VIRUS" }}\n";

                    results.Add(line);

                    Console.WriteLine($"[{Environment.ProcessId}] Scanned File['{ri}']");
                    Thread.Sleep(100);

                    File.AppendAllText(_logFilename, line);

                    Console.WriteLine($"[{Environment.ProcessId}] Written results for File['{ri}']");
                    Thread.Sleep(100);
                }

                Console.WriteLine($"[{Environment.ProcessId}] Finished!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Environment.ProcessId}] ERROR: {ex.Message}");
            }
            finally
            {
                if (mutexOwned) 
                {
                    Console.WriteLine($"[{Environment.ProcessId}] Releasing the mutex...");
                    _mutex.ReleaseMutex();
                }
                else
                {
                    Console.WriteLine($"[{Environment.ProcessId}] Mutex was not owned. Nothing to release.");
                }
            }
        }
    }
}

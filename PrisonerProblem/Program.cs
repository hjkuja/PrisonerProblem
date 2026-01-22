// This is a code example of the 100 prisoners problem

// Defaults
int amountOfPrisoners = 100; // should be divisible by 2
int timesToRun = 1500;
bool shufflePrisoners = true;
int coresToUse = 1; // default: single core
bool quiet = false; // default: keep per-run logging (can be expensive)

ParseArgs(args, ref amountOfPrisoners, ref timesToRun, ref shufflePrisoners, ref coresToUse, ref quiet);

if (amountOfPrisoners % 2 > 0)
{
    throw new ArgumentException("Amount of prisoners should be dividable by 2!");
}

if (coresToUse < 1) coresToUse = 1;
coresToUse = Math.Min(coresToUse, Environment.ProcessorCount);

int successfulRuns = 0;
int failedRuns = 0;

var stopwatch = System.Diagnostics.Stopwatch.StartNew();

var tasks = CreateWorkerTasks(
    coresToUse,
    timesToRun,
    workerRunCount =>
    {
        int localSuccess = 0;
        int localFail = 0;

        for (int runNum = 0; runNum < workerRunCount; runNum++)
        {
            bool runHasFailed = RunOnce(amountOfPrisoners, shufflePrisoners);

            if (!quiet)
            {
                Console.WriteLine($"Run has {(runHasFailed ? "failed" : "passed")}");
            }

            if (runHasFailed) localFail++;
            else localSuccess++;
        }

        Interlocked.Add(ref successfulRuns, localSuccess);
        Interlocked.Add(ref failedRuns, localFail);
    });

await Task.WhenAll(tasks);

stopwatch.Stop();

Console.WriteLine($"Successful runs {successfulRuns}");
Console.WriteLine($"Failed runs {failedRuns}");

// We can calculate the win % of all runs to see if it is close to the theoretical value
decimal per = ((decimal)successfulRuns / timesToRun) * 100;
Console.WriteLine($"Prisoner win % = {per:##.#####}%");
Console.WriteLine($"Total execution time: {stopwatch.Elapsed}");

// Wait for user input to exit
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static bool RunOnce(int amountOfPrisoners, bool shufflePrisoners)
{
    // Initialise the number variables
    int[] prisonerNumbers = new int[amountOfPrisoners];
    int[] boxes = new int[amountOfPrisoners];

    // Create prisoner numbers and boxes
    for (int i = 0; i < amountOfPrisoners; i++)
    {
        boxes[i] = i + 1;
        prisonerNumbers[i] = i + 1;
    }

    // Shuffle box numbers
    boxes.Shuffle();

    // Prisoner numbers can also be shuffled -> prisoners start in random order.
    if (shufflePrisoners) prisonerNumbers.Shuffle();

    // Every prisoner has (n / 2) tries to find their own number
    foreach (var prisoner in prisonerNumbers)
    {
        int tryNumber = 1;
        int maxTries = (amountOfPrisoners / 2);
        int prevPeek = prisoner;
        bool numberFound = false;

        while (!numberFound && tryNumber <= maxTries)
        {
            // The guess is always what was found from the previous box
            var guess = boxes[prevPeek - 1];

            if (guess == prisoner)
            {
                numberFound = true;
            }

            prevPeek = guess;
            tryNumber++;
        }

        if (!numberFound)
        {
            return true; // run has failed
        }
    }

    return false; // run has passed
}

static Task[] CreateWorkerTasks(int workerCount, int totalRuns, Action<int> workerBody)
{
    // Split totalRuns across workers as evenly as possible:
    // - first (totalRuns % workerCount) workers get (totalRuns/workerCount + 1) runs
    // - remaining workers get (totalRuns/workerCount) runs
    int baseRunsPerWorker = totalRuns / workerCount;
    int remainder = totalRuns % workerCount;

    var tasks = new Task[workerCount];
    for (int i = 0; i < workerCount; i++)
    {
        int workerRuns = baseRunsPerWorker + (i < remainder ? 1 : 0);
        tasks[i] = Task.Run(() => workerBody(workerRuns));
    }

    return tasks;
}

static void ParseArgs(
    string[] args,
    ref int amountOfPrisoners,
    ref int timesToRun,
    ref bool shufflePrisoners,
    ref int coresToUse,
    ref bool quiet)
{
    // Supported:
    // --prisoners <int>
    // --times <int>
    // --shufflePrisoners <true|false>
    // --cores <int>
    // --quiet
    // --help
    for (int i = 0; i < args.Length; i++)
    {
        string arg = args[i];

        if (arg is "--help" or "-h" or "/?")
        {
            PrintHelp();
            Environment.Exit(0);
        }

        if (arg is "--quiet")
        {
            quiet = true;
            continue;
        }

        if (arg is "--prisoners" && TryReadInt(args, ref i, out int p)) { amountOfPrisoners = p; continue; }
        if (arg is "--times" && TryReadInt(args, ref i, out int t)) { timesToRun = t; continue; }
        if (arg is "--cores" && TryReadInt(args, ref i, out int c)) { coresToUse = c; continue; }
        if (arg is "--shufflePrisoners" && TryReadBool(args, ref i, out bool s)) { shufflePrisoners = s; continue; }

        throw new ArgumentException($"Unknown argument '{arg}'. Use --help for usage.");
    }

    if (amountOfPrisoners <= 0) throw new ArgumentOutOfRangeException(nameof(amountOfPrisoners));
    if (timesToRun <= 0) throw new ArgumentOutOfRangeException(nameof(timesToRun));
}

static bool TryReadInt(string[] args, ref int i, out int value)
{
    value = 0;
    if (i + 1 >= args.Length) return false;
    if (!int.TryParse(args[i + 1], out value)) return false;
    i++;
    return true;
}

static bool TryReadBool(string[] args, ref int i, out bool value)
{
    value = false;
    if (i + 1 >= args.Length) return false;
    if (!bool.TryParse(args[i + 1], out value)) return false;
    i++;
    return true;
}

static void PrintHelp()
{
    Console.WriteLine(
        """
        PrisonerProblem options:
          --prisoners <int>           Amount of prisoners (must be divisible by 2). Default: 100
          --times <int>               How many runs to simulate. Default: 1500
          --shufflePrisoners <bool>   Shuffle prisoner order too. Default: true
          --cores <int>               Max CPU cores to use (capped to Environment.ProcessorCount). Default: 1
          --quiet                     Disable per-run logging (recommended for performance)
          --help                      Show this help

        Example:
          dotnet run --project PrisonerProblem/PrisonerProblem.csproj -c Release -- --times 1500 --cores 8 --quiet
        """);
}

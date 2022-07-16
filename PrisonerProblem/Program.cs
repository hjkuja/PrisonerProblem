// This is a code example of the 100 prisoners problem

// Amount of prisoners. Should be divisible by 2.
int amountOfPrisoners = 100;
int timesToRun = 1500;
bool shufflePrisoners = false;

int successfulRuns = 0;
int failedRuns = 0;

if (amountOfPrisoners % 2 > 0)
{
    throw new ArgumentException("Amount of prisoners should be dividable by 2!");
}

for (int runNum = 0; runNum < timesToRun; runNum++)
{
    // Initialise the number variables
    int[] prisonerNumbers = new int[amountOfPrisoners];
    int[] boxes = new int[amountOfPrisoners];
    bool runHasFailed = false;

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
        // Try number for logging / writing to console
        int tryNumber = 1;
        // Max tries is 50 when there is 100 prisoners => prisoner amt / 2
        int maxTries = (amountOfPrisoners / 2);
        // Previous peek = next box's number
        // Prisoner starts with their own number = "previous peek" was their own number
        int prevPeek = prisoner;
        // If prisoner found their number
        bool numberFound = false;

        while (!numberFound && tryNumber <= maxTries)
        {
            // The guess is always what was found from the previous box
            var guess = boxes[prevPeek - 1];

            // If prisoner finds their box
            if (guess == prisoner)
            {
                numberFound = true;
            }

            prevPeek = guess;
            tryNumber++;
        }

        if (!numberFound)
        {
            runHasFailed = true;
            break;
        }
    }

    Console.WriteLine($"Run No. #{runNum} has passed");
    if (runHasFailed)
    {
        failedRuns++;
    } else
    {
        successfulRuns++;
    }
}

Console.WriteLine($"Successful runs {successfulRuns}");
Console.WriteLine($"Failed runs {failedRuns}");

// We can calculate the win % of all runs to see if it is close to 30%
decimal per = (decimal.Parse(successfulRuns.ToString()) / decimal.Parse(timesToRun.ToString())) * 100;
Console.WriteLine($"Prisoner win % = {per.ToString("##.#####")}%");

Wait();

static void Wait()
{
    Console.WriteLine("Press any key to continue");
    Console.ReadLine();
}

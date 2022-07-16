// This is a code example of the 100 prisoners problem

var amountOfPrisoners = 100;

var prisonerNumbers = new int[amountOfPrisoners];
var boxes = new int[amountOfPrisoners];

// Create prisoner numbers and boxes
for (int i = 0; i < amountOfPrisoners; i++)
{
    boxes[i] = i + 1;
    prisonerNumbers[i] = i + 1;
}

// Shuffle box numbers
boxes.Shuffle();
prisonerNumbers.Shuffle(); // Prisoner numbers can also be shuffled -> prisoners start in random order.

for (int i = 0; i < amountOfPrisoners; i++)
{
    Console.WriteLine($"Box index={i} has {boxes[i]}");
}

// Every prisoner has (n / 2) tries to find their own number
foreach (var prisoner in prisonerNumbers)
{
    Console.WriteLine($"-------------- NEXT UP PRISONER #{prisoner} --------------");
    Console.WriteLine($"");

    var tryNumber = 1;
    var maxTries = (amountOfPrisoners / 2);
    var prevPeek = prisoner;
    var numberFound = false;
    while (!numberFound && tryNumber <= maxTries)
    {
        Console.WriteLine($"\nPrisoner #{prisoner} try No. #{tryNumber}:\n\t\tPrisoner peeks in the box #{prevPeek} (index:{prevPeek-1})");
        var guess = boxes[prevPeek - 1];

        // If prisoner finds their box
        if (guess == prisoner)
        {
            Console.WriteLine($"\t\t{guess}! It's CORRECT!");
            numberFound = true;
        } else
        {
            Console.WriteLine($"\t\t\"It's {guess}.\"");
            if (tryNumber < maxTries)
            {
                Console.WriteLine($"\t\t\"Must keep looking...\"");
            }
            else
            {
                Console.WriteLine("\t\t\"Oh no..\"");
            }
        }

        prevPeek = guess;
        tryNumber++;
    }

    Console.WriteLine();

    if (!numberFound)
    {
        Console.WriteLine($"Prisoner #{prisoner} did not find their number.\nEveryone loses! >:)");
        break;
    }
}

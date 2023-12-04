int total = 0;

string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day1.txt"));
foreach (string line in lines)
{
    // Very(!) naive approach I guess. Let's keep the first one simple.

    // Find the first digit.
    for (int i = 0; i < line.Length; ++i)
    {
        // The challenge specifically requests a digit, not the whole number, so nice and easy.
        if (char.IsDigit(line[i]))
        {
            int digit = line[i] - '0';
            total += digit * 10;
            break;
        }
    }

    // Now the last digit.
    for (int i = line.Length - 1; i >= 0; --i)
    {
        // The challenge specifically requests a digit, not the whole number, so nice and easy.
        if (char.IsDigit(line[i]))
        {
            int digit = line[i] - '0';
            total += digit;
            break;
        }
    }
}

Console.WriteLine($"Total: {total}");

// Part 2 - account for digits printed as words.

total = 0;
string[] digitWords = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

foreach (string line in lines)
{
    // Find the first digit.
    int digitValue = 0;
    int digitIndex = int.MaxValue;
    for (int i = 0; i < line.Length; ++i)
    {
        // The challenge specifically requests a digit, not the whole number, so nice and easy.
        if (char.IsDigit(line[i]))
        {
            digitIndex = i;
            digitValue = line[i] - '0';
            break;
        }
    }

    for (int i = 0; i < digitWords.Length; ++i)
    {
        int wordIndex = line.IndexOf(digitWords[i]);
        if (wordIndex >= 0 && wordIndex < digitIndex)
        {
            digitIndex = wordIndex;
            digitValue = i + 1;
        }
    }

    total += digitValue * 10;

    // Now the last digit.
    digitValue = 0;
    digitIndex = 0;
    for (int i = line.Length - 1; i >= 0; --i)
    {
        if (char.IsDigit(line[i]))
        {
            digitIndex = i;
            digitValue = line[i] - '0';
            break;
        }
    }

    for (int i = 0; i < digitWords.Length; ++i)
    {
        int wordIndex = line.LastIndexOf(digitWords[i]);
        if (wordIndex >= 0 && wordIndex >= digitIndex)
        {
            digitIndex = wordIndex;
            digitValue = i + 1;
        }
    }

    total += digitValue;
}

Console.WriteLine($"New total: {total}");
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
using System.Diagnostics;

static class Program
{
    private static bool IsSymbol(char c, out bool isAsterisk)
    {
        if (c == '*')
        {
            isAsterisk = true;
            return true;
        }

        isAsterisk = false;
        return c != '.' && !char.IsDigit(c);
    }

    private static bool AdjacentToSymbol(string[] lines, int lineIndex, int index, int length, out (int, int) symbolIndex, out bool isAsterisk)
    {
        if (index > 0 && IsSymbol(lines[lineIndex][index - 1], out isAsterisk))
        {
            symbolIndex = (lineIndex, index - 1);
            return true;
        }

        if (index + length < lines[lineIndex].Length - 1 && IsSymbol(lines[lineIndex][index + length], out isAsterisk))
        {
            symbolIndex = (lineIndex, index + length);
            return true;
        }

        // Above
        if (lineIndex > 0)
        {
            // Diagonal
            if (index > 0 && IsSymbol(lines[lineIndex - 1][index - 1], out isAsterisk))
            {
                symbolIndex = (lineIndex - 1, index - 1);
                return true;
            }

            if (index + length < lines[lineIndex - 1].Length - 1 && IsSymbol(lines[lineIndex - 1][index + length], out isAsterisk))
            {
                symbolIndex = (lineIndex - 1, index + length);
                return true;
            }

            for (int i = 0; i < length; ++i)
            {
                if (IsSymbol(lines[lineIndex - 1][index + i], out isAsterisk))
                {
                    symbolIndex = (lineIndex - 1, index + i);
                    return true;
                }
            }
        }

        // Below
        if (lineIndex < lines.Length - 1)
        {
            // Diagonal
            if (index > 0 && IsSymbol(lines[lineIndex + 1][index - 1], out isAsterisk))
            {
                symbolIndex = (lineIndex + 1, index - 1);
                return true;
            }
            if (index + length < lines[lineIndex + 1].Length - 1 && IsSymbol(lines[lineIndex + 1][index + length], out isAsterisk))
            {
                symbolIndex = (lineIndex + 1, index + length);
                return true;
            }
            for (int i = 0; i < length; ++i)
            {
                if (IsSymbol(lines[lineIndex + 1][index + i], out isAsterisk))
                {
                    symbolIndex = (lineIndex + 1, index + i);
                    return true;
                }
            }
        }

        isAsterisk = false;
        symbolIndex = (default, default);
        return false;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day3.txt"));

        int sum = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            int digitStart = 0;
            int digitLength = 0;

            for (int j = 0; j < line.Length; ++j)
            {
                if (char.IsDigit(line[j]))
                {
                    if (digitLength == 0)
                    {
                        digitStart = j;
                        digitLength = 1;
                    }
                    else
                    {
                        ++digitLength;
                    }
                }
                else
                {
                    if (digitLength > 0)
                    {
                        if (AdjacentToSymbol(lines, i, digitStart, digitLength, out _, out _))
                        {
                            int number = int.Parse(line.Substring(digitStart, digitLength));
                            sum += number;
                        }
                    }

                    digitLength = 0;
                }
            }

            if (digitLength > 0)
            {
                if (AdjacentToSymbol(lines, i, digitStart, digitLength, out _, out _))
                {
                    int number = int.Parse(line.Substring(digitStart, digitLength));
                    sum += number;
                }
            }
        }

        Console.WriteLine($"Sum: {sum}");

        // Part 2

        Dictionary<(int, int), List<int>> gearValues = new();

        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            int digitStart = 0;
            int digitLength = 0;

            for (int j = 0; j < line.Length; ++j)
            {
                if (char.IsDigit(line[j]))
                {
                    if (digitLength == 0)
                    {
                        digitStart = j;
                        digitLength = 1;
                    }
                    else
                    {
                        ++digitLength;
                    }
                }
                else
                {
                    if (digitLength > 0)
                    {
                        if (AdjacentToSymbol(lines, i, digitStart, digitLength, out (int, int) symbolIndex, out bool isAsterisk))
                        {
                            if (isAsterisk)
                            {
                                if (!gearValues.TryGetValue(symbolIndex, out List<int>? values))
                                {
                                    values = new();
                                    gearValues.Add(symbolIndex, values);
                                }

                                int number = int.Parse(line.Substring(digitStart, digitLength));
                                values.Add(number);
                            }
                        }
                    }

                    digitLength = 0;
                }
            }

            if (digitLength > 0)
            {
                if (AdjacentToSymbol(lines, i, digitStart, digitLength, out (int, int) symbolIndex, out bool isAsterisk))
                {
                    if (isAsterisk)
                    {
                        if (!gearValues.TryGetValue(symbolIndex, out List<int>? values))
                        {
                            values = new();
                            gearValues.Add(symbolIndex, values);
                        }

                        int number = int.Parse(line.Substring(digitStart, digitLength));
                        values.Add(number);
                    }
                }
            }
        }

        int gearValuesSum = 0;
        foreach (List<int> values in gearValues.Values)
        {
            if (values.Count == 2)
            {
                gearValuesSum += values[0] * values[1];
            }
        }

        Console.WriteLine($"Gear values sum: {gearValuesSum}");
    }
}
using System.Diagnostics;

static class Program
{
    private struct Data
    {
        public string String;
        public int[] Numbers;
    }

    private static long GetPossibleCombinations(char[] line, int startIndex, int[] numbers, Dictionary<string, long> cache)
    {
        if (numbers.Length == 0)
        {
            return !line.Contains('#') ? 1 : 0;            
        }

        if (startIndex >= line.Length)
        {
            return 0;
        }

        string cacheString = $"{new string(line, startIndex, line.Length - startIndex)}_{string.Join(',', numbers)}";
        if (cache.TryGetValue(cacheString, out long val))
        {
            return val;
        }

        if (line[startIndex] == '.')
        {
            for (int i = startIndex + 1; i < line.Length; ++i)
            {
                if (line[i] != '.')
                {
                    long sum = GetPossibleCombinations(line, i, numbers, cache);
                    cache[cacheString] = sum;
                    return sum;
                }
            }
        }
        else if (line[startIndex] == '?')
        {
            char[] newLine = line.ToArray();
            newLine[startIndex] = '#';

            long sum = GetPossibleCombinations(newLine, startIndex, numbers, cache);
            sum += GetPossibleCombinations(line, startIndex + 1, numbers, cache);
            cache[cacheString] = sum;
            return sum;
        }
        else
        {
            int number = numbers[0];
            for (int i = startIndex; i < line.Length; ++i)
            {
                if (line[i] == '.')
                    break;

                if (number - 1 == i - startIndex)
                {
                    if (startIndex + number < line.Length && line[startIndex + number] == '#')
                    {
                        break;
                    }

                    char[] newLine = line.ToArray();
                    for (int j = 0; j < number; ++j)
                        newLine[startIndex + j] = '1';

                    int[] newNumbers = numbers.Length == 1 ? Array.Empty<int>() : new int[numbers.Length - 1];
                    for (int j = 1; j < numbers.Length; ++j)
                        newNumbers[j - 1] = numbers[j];

                    long sum = GetPossibleCombinations(newLine, i + 2, newNumbers, cache);
                    cache[cacheString] = sum;
                    return sum;
                }
            }
        }

        cache[cacheString] = 0;
        return 0;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day12.txt"));

        // Parse data
        Data[] data = new Data[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            ref Data entry = ref data[i];

            string[] parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(parts.Length == 2);

            entry.String = parts[0];
            entry.Numbers = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }

        long sumOfValidCombinations = 0;

        Dictionary<string, long> cache = new();
        for (int i = 0; i < data.Length; ++i)
        {
            sumOfValidCombinations += GetPossibleCombinations(data[i].String.ToArray(), 0, data[i].Numbers, cache);
        }

        Console.WriteLine($"Answer: {sumOfValidCombinations}");

        // Part 2

        sumOfValidCombinations = 0;

        cache.Clear();
        for (int i = 0; i < data.Length; ++i)
        {
            string str = data[i].String;
            int[] numbers = new int[data[i].Numbers.Length * 5];
            for (int j = 0; j < 4; ++j)
            {
                str += $"?{data[i].String}";
            }

            for (int j = 0; j < 5; ++j)
            {
                int length = data[i].Numbers.Length;
                for (int k = 0; k < length; ++k)
                {
                    numbers[j * length + k] = data[i].Numbers[k];
                }
            }

            sumOfValidCombinations += GetPossibleCombinations(str.ToArray(), 0, numbers, cache);
        }

        Console.WriteLine($"Answer: {sumOfValidCombinations}");
        Console.ReadLine();
    }
}
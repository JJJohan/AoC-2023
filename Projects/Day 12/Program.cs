using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

static class Program
{
    private enum Condition
    {
        Unknown,
        Damaged,
        Operational
    }

    private class Line
    {
        public Condition[] Conditions = Array.Empty<Condition>();
        public int[] Numbers = Array.Empty<int>();

        public List<(int, int)> Ranges = new(); // index, count
        public List<HashSet<(int, int)>> FoundCombinations = new();
    }

    private static Condition GetCondition(char c) => c switch
    {
        '?' => Condition.Unknown,
        '#' => Condition.Damaged,
        '.' => Condition.Operational,
        _ => throw new InvalidOperationException(nameof(c)),
    };

    private static void RangeLookup(Line data, int j, int m, int n, int o, int p, int q, int k, int l, int r, int s, int t, int u)
    {
        int numberIndex = 0;
        int rangeIndex = 0;
        bool valid = true;
        HashSet<(int, int)> rangesFound = new();
        (int, int)[] pendingRanges = data.Ranges.ToArray();

        while (numberIndex != data.Numbers.Length)
        {
            int number = data.Numbers[numberIndex];

            if (numberIndex == j)
            {
                pendingRanges[rangeIndex].Item1 += k;
                pendingRanges[rangeIndex].Item2 -= k;
            }

            if (numberIndex == m)
            {
                pendingRanges[rangeIndex].Item1 += l;
                pendingRanges[rangeIndex].Item2 -= l;
            }

            if (numberIndex == n)
            {
                pendingRanges[rangeIndex].Item1 += r;
                pendingRanges[rangeIndex].Item2 -= r;
            }

            if (numberIndex == o)
            {
                pendingRanges[rangeIndex].Item1 += s;
                pendingRanges[rangeIndex].Item2 -= s;
            }

            if (numberIndex == p)
            {
                pendingRanges[rangeIndex].Item1 += t;
                pendingRanges[rangeIndex].Item2 -= t;
            }

            if (numberIndex == q)
            {
                pendingRanges[rangeIndex].Item1 += u;
                pendingRanges[rangeIndex].Item2 -= u;
            }

            while (valid)
            {
                ref (int offset, int count) range = ref pendingRanges[rangeIndex];

                if (range.count >= number)
                {
                    rangesFound.Add(new(range.offset, number));
                    range.offset += number + 1;
                    range.count -= number + 1;
                    ++numberIndex;
                    if (range.count <= 0)
                    {
                        ++rangeIndex;

                        if (rangeIndex == pendingRanges.Length && numberIndex != data.Numbers.Length)
                        {
                            valid = false;
                        }
                    }
                    break;
                }
                else
                {
                    ++rangeIndex;
                    if (rangeIndex == pendingRanges.Length)
                    {
                        valid = false;
                    }
                }
            }

            if (!valid)
                break;
        }

        if (valid)
        {
            data.FoundCombinations.Add(rangesFound);
        }
    }

    private static void GenerateCombinations(Line data, int[] numbers, (int, int)[] ranges, HashSet<(int, int)> rangesFound, int index, int biggestRange)
    {
        if (index == numbers.Length)
        {
            data.FoundCombinations.Add(new HashSet<(int, int)>(rangesFound));
            return;
        }

        for (int offset = 0; offset < biggestRange; ++offset)
        {
            ranges[index].Item1 += offset;
            ranges[index].Item2 -= offset;

            bool valid = true;

            for (int i = 0; i < ranges.Length; ++i)
            {
                if (ranges[i].Item2 < 0)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                GenerateCombinations(data, numbers, ranges, rangesFound, index + 1, biggestRange);
            }

            ranges[index].Item1 -= offset;
            ranges[index].Item2 += offset;
        }
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day12.txt"));

        Stopwatch timer = Stopwatch.StartNew();

        // Parse data
        Line[] lineData = new Line[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];

            Line data = lineData[i] = new Line();

            string[] parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(parts.Length == 2);

            data.Conditions = new Condition[parts[0].Length];
            for (int j = 0; j < parts[0].Length; ++j)
            {
                data.Conditions[j] = GetCondition(parts[0][j]);
            }

            data.Numbers = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }

        // Find ranges of unknown and broken cells.
        for (int i = 0; i < lineData.Length; ++i)
        {
            ref Line data = ref lineData[i];
            int startIndex = -1;
            for (int j = 0; j < data.Conditions.Length; ++j)
            {
                Condition c = data.Conditions[j];
                if (c != Condition.Operational)
                {
                    if (startIndex == -1)
                    {
                        startIndex = j;
                    }
                }
                else
                {
                    if (startIndex != -1)
                    {
                        data.Ranges.Add(new(startIndex, j - startIndex));
                        startIndex = -1;
                    }
                }
            }

            if (startIndex != -1)
            {
                data.Ranges.Add(new(startIndex, data.Conditions.Length - startIndex));
            }
        }

        // Find number of variations in each line.
        // Find all combinations, ignoring the requirement of '#'.

        int remaining = lineData.Length;
        for (int i = 0; i < lineData.Length; ++i)
        {
            ref Line data = ref lineData[i];

            // Loop over each number set once to test every possible combination.

            int biggestRange = 0;
            foreach ((int, int) range in data.Ranges)
            {
                biggestRange = Math.Max(range.Item2, biggestRange);
            }

            int[] numbers = data.Numbers.ToArray();
            (int, int)[] initialRanges = data.Ranges.ToArray();
            HashSet<(int, int)> initialRangesFound = new();

            biggestRange = initialRanges.Length;

            GenerateCombinations(data, numbers, initialRanges, initialRangesFound, 0, biggestRange);

            Console.WriteLine($"Remaining: {--remaining}");
        }

        // Count valid combinations.
        // Note the use of a string hash set as using a hash set of combined pairs appears to merge too aggressively.
        // Again a less than optimal solution, but not as awful as the nested hellscape above.
        long sumOfValidCombinations = 0;
        HashSet<string> comboStrings = new();
        StringBuilder sb = new();

        for (int i = 0; i < lineData.Length; ++i)
        {
            foreach (HashSet<(int, int)> combinations in lineData[i].FoundCombinations)
            {
                bool partOfRange = true;
                for (int j = 0; j < lineData[i].Conditions.Length; ++j)
                {
                    Condition c = lineData[i].Conditions[j];
                    if (c == Condition.Damaged)
                    {
                        partOfRange = false;
                        foreach ((int offset, int range) in combinations)
                        {
                            if (j >= offset && j < offset + range)
                            {
                                partOfRange = true;
                                break;
                            }
                        }
                    }

                    if (!partOfRange)
                        break;
                }

                for (int j = 0; j < lineData[i].Conditions.Length; ++j)
                {
                    bool found = false;
                    foreach ((int offset, int range) in combinations)
                    {
                        if (j >= offset && j < offset + range)
                        {
                            sb.Append('1');
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Condition c = lineData[i].Conditions[j];
                        sb.Append(c switch
                        {
                            Condition.Unknown => '?',
                            Condition.Damaged => '#',
                            Condition.Operational => '.',
                            _ => throw new InvalidOperationException(nameof(c)),
                        });
                    }

                }

                if (partOfRange)
                {
                    comboStrings.Add(sb.ToString());
                }
                sb.Clear();
            }

            sumOfValidCombinations += comboStrings.Count;
            comboStrings.Clear();
        }

        Console.WriteLine();
        Console.WriteLine($"Time elapsed: {timer.Elapsed.Hours} Hours, {timer.Elapsed.Minutes} Minutes");
        Console.WriteLine($"Answer: {sumOfValidCombinations}");
    }
}
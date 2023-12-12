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

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day12.txt"));

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
        Parallel.For(0, lineData.Length, i =>
        {
            ref Line data = ref lineData[i];

            // Loop over each number set once to test every possible combination.

            int biggestRange = 0;
            foreach ((int, int) range in data.Ranges)
            {
                biggestRange = Math.Max(range.Item2, biggestRange);
            }

            // I've never created a nested loop this deep before.
            // To be honest, I just couldn't nicely figure out how to test every combination,
            // so I've written this awful mess because there's only 6 numbers max per line.
            // Unsurprisingly it takes absolutely forever, but I've wasted enough hours on this as-is.
            if (data.Numbers.Length == 6)
            {
                for (int j = 0; j < data.Numbers.Length; ++j)
                {
                    for (int m = 0; m < data.Numbers.Length; ++m)
                    {
                        for (int n = 0; n < data.Numbers.Length; ++n)
                        {
                            for (int o = 0; o < data.Numbers.Length; ++o)
                            {
                                for (int p = 0; p < data.Numbers.Length; ++p)
                                {
                                    for (int q = 0; q < data.Numbers.Length; ++q)
                                    {
                                        for (int k = 0; k < biggestRange; ++k)
                                        {
                                            for (int l = 0; l < biggestRange; ++l)
                                            {
                                                for (int r = 0; r < biggestRange; ++r)
                                                {
                                                    for (int s = 0; s < biggestRange; ++s)
                                                    {
                                                        for (int t = 0; t < biggestRange; ++t)
                                                        {
                                                            for (int u = 0; u < biggestRange; ++u)
                                                            {
                                                                RangeLookup(data, j, m, n, o, p, q, k, l, r, s, t, u);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (data.Numbers.Length == 5)
            {
                for (int j = 0; j < data.Numbers.Length; ++j)
                {
                    for (int m = 0; m < data.Numbers.Length; ++m)
                    {
                        for (int n = 0; n < data.Numbers.Length; ++n)
                        {
                            for (int o = 0; o < data.Numbers.Length; ++o)
                            {
                                for (int p = 0; p < data.Numbers.Length; ++p)
                                {
                                    for (int k = 0; k < biggestRange; ++k)
                                    {
                                        for (int l = 0; l < biggestRange; ++l)
                                        {
                                            for (int r = 0; r < biggestRange; ++r)
                                            {
                                                for (int s = 0; s < biggestRange; ++s)
                                                {
                                                    for (int t = 0; t < biggestRange; ++t)
                                                    {
                                                        RangeLookup(data, j, m, n, o, p, 255, k, l, r, s, t, 255);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (data.Numbers.Length == 4)
            {
                for (int j = 0; j < data.Numbers.Length; ++j)
                {
                    for (int m = 0; m < data.Numbers.Length; ++m)
                    {
                        for (int n = 0; n < data.Numbers.Length; ++n)
                        {
                            for (int o = 0; o < data.Numbers.Length; ++o)
                            {
                                for (int k = 0; k < biggestRange; ++k)
                                {
                                    for (int l = 0; l < biggestRange; ++l)
                                    {
                                        for (int r = 0; r < biggestRange; ++r)
                                        {
                                            for (int s = 0; s < biggestRange; ++s)
                                            {
                                                RangeLookup(data, j, m, n, o, 255, 255, k, l, r, s, 255, 255);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (data.Numbers.Length == 3)
            {
                for (int j = 0; j < data.Numbers.Length; ++j)
                {
                    for (int m = 0; m < data.Numbers.Length; ++m)
                    {
                        for (int n = 0; n < data.Numbers.Length; ++n)
                        {
                            for (int k = 0; k < biggestRange; ++k)
                            {
                                for (int l = 0; l < biggestRange; ++l)
                                {
                                    for (int r = 0; r < biggestRange; ++r)
                                    {
                                        RangeLookup(data, j, m, n, 255, 255, 255, k, l, r, 255, 255, 255);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (data.Numbers.Length == 2)
            {
                for (int j = 0; j < data.Numbers.Length; ++j)
                {
                    for (int m = 0; m < data.Numbers.Length; ++m)
                    {
                        for (int k = 0; k < biggestRange; ++k)
                        {
                            for (int l = 0; l < biggestRange; ++l)
                            {
                                for (int r = 0; r < biggestRange; ++r)
                                {
                                    RangeLookup(data, j, m, 255, 255, 255, 255, k, l, 255, 255, 255, 255);
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Remaining: {--remaining}");
        });

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
        Console.WriteLine($"Answer: {sumOfValidCombinations}");
    }
}
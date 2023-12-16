using System.Diagnostics;

static class Program
{
    private static long Hash(string str)
    {
        long currentVal = 0;
        for (int j = 0; j < str.Length; ++j)
        {
            char c = str[j];
            currentVal += c;
            currentVal *= 17;
            currentVal %= 256;
        }

        return currentVal;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day15.txt"));

        Debug.Assert(lines.Length == 1);

        // Determine the ASCII code for the current character of the string.
        // Increase the current value by the ASCII code you just determined.
        // Set the current value to itself multiplied by 17.
        // Set the current value to the remainder of dividing itself by 256.

        long sum = 0;
        string[] parts = lines[0].Split(',');
        for (int i = 0; i < parts.Length; ++i)
        {
            string str = parts[i];
            long currentVal = Hash(str);

            sum += currentVal;
        }

        Console.WriteLine($"Answer: {sum}");

        // Part 2

        Dictionary<long, List<(string, int)>> map = new();
        for (int i = 0; i < parts.Length; ++i)
        {
            string str = parts[i];
            for (int j = 0; j < str.Length; ++j)
            {
                char c = str[j];
                if (c == '=')
                {
                    string key = str.Substring(0, j);
                    int newValue = int.Parse(str.Substring(j + 1));
                    long boxIndex = Hash(key);
                    if (map.TryGetValue(boxIndex, out List<(string, int)>? values))
                    {
                        bool replaced = false;
                        for (int k = 0; k < values.Count; ++k)
                        {
                            if (values[k].Item1 == key)
                            {
                                values[k] = (key, newValue);
                                replaced = true;
                                break;
                            }
                        }

                        if (!replaced)
                            values.Add((key, newValue));
                    }
                    else
                    {
                        map[boxIndex] = new List<(string, int)> { (key, newValue) };
                    }
                }
                else if (c == '-')
                {
                    string key = str.Substring(0, j);
                    long boxIndex = Hash(key);
                    if (map.TryGetValue(boxIndex, out List<(string, int)>? values))
                    {
                        for (int k = 0; k < values.Count; ++k)
                        {
                            if (values[k].Item1 == key)
                            {
                                values.RemoveAt(k);
                                break;
                            }
                        }
                    }
                }
            }
        }

        sum = 0;
        foreach (KeyValuePair<long, List<(string, int)>> pair in map)
        {
            if (pair.Value.Count == 0) continue;

            for (int i = 0; i < pair.Value.Count; ++i)
            {
                long val = pair.Key + 1;
                val *= (i + 1);
                val *= pair.Value[i].Item2;
                sum += val;
            }

        }

        Console.WriteLine($"Answer: {sum}");
    }
}
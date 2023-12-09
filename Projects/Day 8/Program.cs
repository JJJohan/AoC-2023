using System.Diagnostics;

static class Program
{
    private struct Node
    {
        public string Left;
        public string Right;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day8.txt"));

        string directions = lines[0];

        Dictionary<string, Node> nodes = new();
        for (int i = 2; i < lines.Length; ++i)
        {
            string[] split = lines[i].Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Debug.Assert(split.Length == 2);

            string[] pairSplit = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Debug.Assert(pairSplit.Length == 2);

            nodes.Add(split[0], new() { Left = pairSplit[0][1..], Right = pairSplit[1][..^1] });
        }

        Node node = nodes["AAA"];

        ulong steps = 0;
        bool found = false;
        while (!found)
        {
            for (int i = 0; i < directions.Length; ++i)
            {
                char dir = directions[i];
                string nextNode = dir == 'L' ? node.Left : node.Right;
                node = nodes[nextNode];
                ++steps;
                if (nextNode == "ZZZ")
                {
                    found = true;
                    break;
                }

            }
        }

        Console.WriteLine($"Answer: {steps}");

        // Part 2

        KeyValuePair<string, Node>[] nodesEndingWithAKvp = nodes.Where(x => x.Key[x.Key.Length - 1] == 'A').ToArray();
        (string, Node)[] nodesEndingWithA = new (string, Node)[nodesEndingWithAKvp.Length];
        for (int i = 0; i < nodesEndingWithAKvp.Length; ++i)
            nodesEndingWithA[i] = (nodesEndingWithAKvp[i].Key, nodesEndingWithAKvp[i].Value);

        long[] stepsForPath = new long[nodesEndingWithA.Length];
        bool[] nodesEnded = new bool[nodesEndingWithA.Length];
        int totalFound = 0;

        while (totalFound != nodesEndingWithA.Length)
        {
            for (int i = 0; i < directions.Length; ++i)
            {
                char dir = directions[i];

                for (int j = 0; j < nodesEndingWithA.Length; ++j)
                {
                    if (nodesEnded[j]) continue;

                    nodesEndingWithA[j].Item1 = dir == 'L' ? nodesEndingWithA[j].Item2.Left : nodesEndingWithA[j].Item2.Right;
                    nodesEndingWithA[j].Item2 = nodes[nodesEndingWithA[j].Item1];
                    ++stepsForPath[j];

                    if (nodesEndingWithA[j].Item1[2] == 'Z')
                    {
                        nodesEnded[j] = true;
                        ++totalFound;
                    }
                }
            }
        }

        // Find least common multiple of all steps.
        long lcm = stepsForPath[0];
        for (int i = 1; i < stepsForPath.Length; ++i)
        {
            lcm = Math.Abs(lcm * stepsForPath[i]) / GCD(lcm, stepsForPath[i]);
        }

        Console.WriteLine($"Answer: {lcm}");
    }

    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}
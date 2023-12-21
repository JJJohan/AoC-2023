using System.Diagnostics;

static class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day21.txt"));


        int cols = lines[0].Length;
        int rows = lines.Length;

        char[,] grid = new char[cols, rows];
        (int x, int y) start = (0, 0);
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            for (int j = 0; j < line.Length; ++j)
            {
                char c = line[j];
                grid[j, i] = c;

                if (c == 'S') start = (j, i);
            }
        }

        int steps = 64;
        Stack<HashSet<(int x, int y)>> points = new();
        points.Push(new() { start });

        while (steps > 0 && points.Count > 0)
        {
            if (steps == 0) break;
            --steps;

            HashSet<(int x, int y)> options = points.Pop();
            HashSet<(int x, int y)> newOptions = new();
            foreach ((int x, int y) in options)
            {
                if (x > 0 && grid[x - 1, y] == '.')
                {
                    newOptions.Add((x - 1, y));
                }

                if (x < cols - 1 && grid[x + 1, y] == '.')
                {
                    newOptions.Add((x + 1, y));
                }

                if (y > 0 && grid[x, y - 1] == '.')
                {
                    newOptions.Add((x, y - 1));
                }

                if (y < rows - 1 && grid[x, y + 1] == '.')
                {
                    newOptions.Add((x, y + 1));
                }
            }

            if (newOptions.Count > 0)
                points.Push(newOptions);
        }

        Debug.Assert(points.Count == 1);
        HashSet<(int x, int y)> availableTiles = points.Pop();

        Console.WriteLine($"Available tiles: {availableTiles.Count + 1}");

        // Part 2

        //steps = 26501365;
        steps = 1000;

        Dictionary<(int x, int y), int>[] optionSets = [new(), new()];
        Dictionary<(int x, int y), int> firstSet = optionSets[0];
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                firstSet[(x, y)] = 0;
            }
        }
        firstSet[start] = 1;

        int setIndex = 0;
        while (steps > 0)
        {
            --steps;

           // float percent = (float)(26501365 - steps) / 26501365 * 100.0f;
            //if (steps % 100 == 0) Console.WriteLine($"Steps to go: {steps} ({percent:0.00}%)");

            Dictionary<(int x, int y), int> set = optionSets[setIndex];
            Dictionary<(int x, int y), int> next = optionSets[(setIndex + 1) % optionSets.Length];
            foreach ((int x, int y) in set.Keys)
            {
                next[(x, y)] = 0;
            }

            foreach (KeyValuePair<(int, int), int> pair in set)
            {
                (int x, int y)  = pair.Key;
                int current = pair.Value;
                Debug.Assert(current >= 0);
                if (current == 0) continue;

                int xMin1Abs = x - 1;
                int yMin1Abs = y - 1;
                int xPlus1Abs = x + 1;
                int yPlus1Abs = y + 1;
                int xMin1 = x - 1;
                while (xMin1 < 0) xMin1 += cols;
                int xPlus1 = (x + 1) % cols;

                int yMin1 = y - 1;
                while (yMin1 < 0) yMin1 += rows;
                int yPlus1 = (y + 1) % rows;

                if (grid[xMin1, y] != '#')
                {
                    next[(xMin1, y)] = set[(xMin1, y)] + 1;
                }

                if (grid[xPlus1, y] != '#')
                {
                    next[(xPlus1, y)] = set[(xPlus1, y)] + 1;
                }

                if (grid[x, yMin1] != '#')
                {
                    next[(x, yMin1)] = set[(x, yMin1)] + 1;
                }

                if (grid[x, yPlus1] != '#')
                {
                    next[(x, yPlus1)] = set[(x, yPlus1)] + 1;
                }
            }

            foreach ((int x, int y) in next.Keys)
            {
                int current = set[(x, y)];
                if (current > 0)
                    set[(x, y)] = current - 1;
            }

            setIndex = (setIndex + 1) % optionSets.Length;
        }

        long availableTileCount = 0;
        foreach (int count in optionSets[setIndex].Values)
        {
            availableTileCount += count;
        }

        Console.WriteLine($"Available tiles: {availableTileCount}");
    }
}
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
                char letter = line[j];
                grid[j, i] = letter;

                if (letter == 'S') start = (j, i);
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

        Debug.Assert(rows == cols);
        long grids = 26501365 / rows;
        long rem = 26501365 % rows;

        // I stole this quadratic logic and solver from the AoC sub-Reddit so I don't really deserve the star,
        // but I spend quite a long time trying to figure out a way to map it and brute forcing it so eh.
        List<int> sequence = new();
        HashSet<(int x, int y)> work = new() { start };
        steps = 0;
        (int x, int y)[] dirs = new (int, int)[4];
        for (int n = 0; n < 3; n++)
        {
            long end = n * rows + rem;
            while (steps < end)
            {
                HashSet<(int x, int y)> next = new();
                foreach ((int x, int y) in work)
                {
                    dirs[0] = (x - 1, y);
                    dirs[1] = (x + 1, y);
                    dirs[2] = (x, y - 1);
                    dirs[3] = (x, y + 1);

                    for (int i = 0; i < 4; ++i)
                    {
                        (int x, int y) dir = dirs[i];

                        (int x, int y) coord = (dir.x % cols, dir.y % rows);
                        while (coord.x < 0) coord.x += cols;
                        while (coord.y < 0) coord.y += rows;
                        
                        if (grid[coord.x, coord.y] != '#')
                        {
                            next.Add((dir.x, dir.y));
                        }
                    }
                }

                work = next;
                ++steps;
            }

            sequence.Add(work.Count);
        }

        // Solve for the quadratic coefficients
        // ax^2 + bx + c
        int c = sequence[0];
        int aPlusB = sequence[1] - c;
        int fourAPlusTwoB = sequence[2] - c;
        int twoA = fourAPlusTwoB - (2 * aPlusB);
        int a = twoA / 2;
        int b = aPlusB - a;

        long availableTileCount = a * (grids * grids) + b * grids + c;

        Console.WriteLine($"Available tiles: {availableTileCount}");
    }
}
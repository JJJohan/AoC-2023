using System.Diagnostics;

static class Program
{
    private class Node
    {
        public bool Double;
        public int GalaxyId;
        public long X;
        public long Y;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day11.txt"));

        int rows = lines.Length;
        int cols = lines[0].Length;

        Node[,] nodes = new Node[cols, rows];

        // Flag galaxies and horizontal gaps
        for (int y = 0; y < rows; ++y)
        {
            string line = lines[y];
            bool hasGalaxy = false;
            for (int x = 0; x < cols; ++x)
            {
                char c = line[x];
                if (c == '#')
                    hasGalaxy = true;

                nodes[x, y] = new();

            }

            if (!hasGalaxy)
            {
                for (int x = 0; x < cols; ++x)
                {
                    nodes[x, y].Double = true;
                }
            }
        }

        // Find vertical gaps
        for (int x = 0; x < cols; ++x)
        {
            bool hasGalaxy = false;
            for (int y = 0; y < rows; ++y)
            {
                char c = lines[y][x];
                if (c == '#')
                {
                    hasGalaxy = true;
                    break;
                }
            }

            if (!hasGalaxy)
            {
                for (int y = 0; y < rows; ++y)
                {
                    nodes[x, y].Double = true;
                }
            }
        }

        // Mark galaxies
        List<Node> galaxies = new();
        int galaxyId = 0;
        for (int x = 0; x < cols; ++x)
        {
            int realY = 0;
            for (int y = 0; y < rows; ++y)
            {
                char c = lines[y][x];
                if (c == '#')
                {
                    nodes[x, y].GalaxyId = ++galaxyId;
                    nodes[x, y].Y = realY;
                    galaxies.Add(nodes[x, y]);
                }

                if (nodes[x, y].Double) realY += 2;
                else realY++;
            }
        }
        for (int y = 0; y < rows; ++y)
        {
            int realX = 0;
            for (int x = 0; x < cols; ++x)
            {
                nodes[x, y].X = realX;
                if (nodes[x, y].Double) realX += 2;
                else realX++;
            }
        }

        // Find distances between pairs
        long distanceSum = 0;
        HashSet<(int, int)> pairs = new();
        foreach (Node galaxy in galaxies)
        {
            Debug.Assert(galaxy.GalaxyId > 0);
            Debug.Assert(!galaxy.Double);
            foreach (Node other in galaxies)
            {
                if (galaxy == other) continue;
                if (pairs.Contains((other.GalaxyId, galaxy.GalaxyId))) continue;


                Debug.Assert(other.GalaxyId > 0);
                Debug.Assert(!other.Double);

                long dx = Math.Abs(galaxy.X - other.X);
                long dy = Math.Abs(galaxy.Y - other.Y);
                distanceSum += dx + dy;

                pairs.Add((galaxy.GalaxyId, other.GalaxyId));
                pairs.Add((other.GalaxyId, galaxy.GalaxyId));
            }
        }

        Console.WriteLine($"Answer: {distanceSum}");

        // Part 2

        // Mark galaxies
        for (int x = 0; x < cols; ++x)
        {
            long realY = 0;
            for (int y = 0; y < rows; ++y)
            {
                    nodes[x, y].Y = realY;
                if (nodes[x, y].Double) realY += 1_000_000;
                else realY++;
            }
        }
        for (int y = 0; y < rows; ++y)
        {
            long realX = 0;
            for (int x = 0; x < cols; ++x)
            {
                nodes[x, y].X = realX;
                if (nodes[x, y].Double) realX += 1_000_000;
                else realX++;
            }
        }

        // Find distances between pairs
        distanceSum = 0;
        pairs.Clear();
        foreach (Node galaxy in galaxies)
        {
            foreach (Node other in galaxies)
            {
                if (galaxy == other) continue;
                if (pairs.Contains((other.GalaxyId, galaxy.GalaxyId))) continue;

                long dx = Math.Abs(galaxy.X - other.X);
                long dy = Math.Abs(galaxy.Y - other.Y);
                distanceSum += dx + dy;

                pairs.Add((galaxy.GalaxyId, other.GalaxyId));
                pairs.Add((other.GalaxyId, galaxy.GalaxyId));
            }
        }

        Console.WriteLine($"Answer: {distanceSum}");
    }
}
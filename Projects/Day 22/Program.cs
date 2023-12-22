using System.Diagnostics;
using System.Numerics;

static class Program
{

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day22.txt"));

        Vector3[] starts = new Vector3[lines.Length];
        Vector3[] ends = new Vector3[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            string[] split = line.Split('~');
            Debug.Assert(split.Length == 2);
            int[] start = split[0].Split(',').Select(int.Parse).ToArray();
            int[] end = split[1].Split(',').Select(int.Parse).ToArray();
            Debug.Assert(start.Length == 3);
            Debug.Assert(end.Length == 3);

            starts[i] = new(start[0], start[1], start[2]);
            ends[i] = new(end[0], end[1], end[2]);
        }

        // find area
        Dictionary<int, HashSet<int>> supportHierarchy = new(); // upward
        Dictionary<int, HashSet<int>> supportedBy = new(); // downward
        Vector3 min = new(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3 max = new(int.MinValue, int.MinValue, int.MinValue);
        for (int i = 0; i < lines.Length; ++i)
        {
            Vector3 brickMin = Vector3.Min(starts[i], ends[i]);
            Vector3 brickMax = Vector3.Max(starts[i], ends[i]);
            min = Vector3.Min(min, brickMin);
            max = Vector3.Max(max, brickMax);
            starts[i] = brickMin;
            ends[i] = brickMax;

            supportHierarchy[i] = new();
            supportedBy[i] = new();
        }

        // Drop bricks.
        for (int i = 0; i < lines.Length; ++i)
        {
            ref Vector3 start = ref starts[i];
            ref Vector3 end = ref ends[i];
            if (start.Z == 1) continue;

            bool dropped = true;
            while (dropped)
            {
                for (int j = 0; j < lines.Length; ++j)
                {
                    if (i == j) continue;

                    ref Vector3 otherStart = ref starts[j];
                    ref Vector3 otherEnd = ref ends[j];
                    if (otherEnd.Z != start.Z - 1) continue; // not underneath.

                    if (start.X < otherStart.X || end.X > otherEnd.X || start.Y < otherStart.Y || end.Y > otherEnd.Y)
                    {
                        dropped = false;
                        break;
                    }
                }

                if (dropped)
                {
                    char last = (char)('A' + i);
                    Console.WriteLine($"Brick {last} dropped.");
                    --start.Z;
                    --end.Z;
                    if (start.Z == 1) break;
                }
            }
        }

        // Find supported bricks.
        for (int x = (int)min.X; x <= (int)max.X; ++x)
        {
            for (int y = (int)min.Y; y <= (int)max.Y; ++y)
            {
                int brickIdx = -1;
                int brickMin = 0;
                for (int z = (int)max.Z; z >= (int)min.Z; --z)
                {
                    if (z < brickMin) brickIdx = -1;

                    for (int i = 0; i < lines.Length; ++i)
                    {

                        ref Vector3 start = ref starts[i];
                        ref Vector3 end = ref ends[i];
                        if (x >= start.X && x <= end.X && y >= start.Y && y <= end.Y && z >= start.Z && z <= end.Z)
                        {
                            if (brickIdx != i) // different brick.
                            {
                                char last = (char)('A' + brickIdx);
                                char current = (char)('A' + i);

                                if (brickIdx != -1) // not the first.
                                {
                                    Console.WriteLine($"Brick {current} supports brick {last}.");
                                    supportHierarchy[i].Add(brickIdx);
                                    supportedBy[brickIdx].Add(i);
                                }

                                brickIdx = i;
                                brickMin = (int)start.Z - 1;
                            }
                        }
                    }
                }
            }
        }

        // Remove all but 1 support brick from each brick.
        for (int i = 0; i < lines.Length; ++i)
        {
            HashSet<int> supportBricks = supportedBy[i];
            while (supportBricks.Count > 1)
            {
                int brick = supportBricks.First();
                supportBricks.Remove(brick);
                supportHierarchy[brick].Remove(i);
            }
        }

        // Find how many bricks can be disintegrated.
        int distingrateCount = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            if (supportHierarchy[i].Count == 0)
            {
                char letter = (char)('A' + i);
                Console.WriteLine($"Can disintegrate brick {letter}.");
                ++distingrateCount;
            }
        }

        Console.WriteLine($"Bricks that can be disintegrated: {distingrateCount}");
    }
}
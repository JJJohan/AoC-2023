using System.Diagnostics;

static class Program
{
    private static bool IsPointOnSegment(in (double X, double Y) point, in (long X, long Y) start, in (long X, long Y) end)
    {
        return point.X >= Math.Min(start.X, end.X) && point.X <= Math.Max(start.X, end.X) &&
                point.Y >= Math.Min(start.Y, end.Y) && point.Y <= Math.Max(start.Y, end.Y);
    }

    // I've kind of adapted a finite segment intersection test as I got confused with only testing forwards (dot product I guess)
    private static bool RayLineIntersect2D(in (long X, long Y) p1, 
        in (long X, long Y) v1, 
        in (long X, long Y) p2, 
        in (long X, long Y) v2,
        out (double X, double Y) hit)
    {
        // Calculate slopes
        double m1 = (v1.Y - p1.Y) / (double)(v1.X - p1.X);
        double m2 = (v2.Y - p2.Y) / (double)(v2.X - p2.X);

        // Check if the lines are parallel
        if (Math.Abs(m1 - m2) < float.Epsilon)
        {
            hit = default;
            return false;
        }

        // Calculate intersection point
        hit.X = (m1 * p1.X - m2 * p2.X + p2.Y - p1.Y) / (m1 - m2);
        hit.Y = p1.Y + m1 * (hit.X - p1.X);

        return IsPointOnSegment(hit, p1, v1) && IsPointOnSegment(hit, p2, v2);
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day24.txt"));

        (long X, long Y, long Z)[] positions = new (long, long, long)[lines.Length];
        (long X, long Y, long Z)[] velocities = new (long, long, long)[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            string[] parts = line.Split('@', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(parts.Length == 2);

            long[] posInts = parts[0].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            Debug.Assert(posInts.Length == 3);

            long[] velocityInts = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            Debug.Assert(velocityInts.Length == 3);

            positions[i] = (posInts[0], posInts[1], posInts[2]);
            velocities[i] = (velocityInts[0], velocityInts[1], velocityInts[2]);
        }

        (double X, double Y) min = (200000000000000, 200000000000000);
        (double X, double Y) max = (400000000000000, 400000000000000);

        int intersections = 0;
        for (int i = 0; i < positions.Length; ++i)
        {
            (long X, long Y) pos1 = (positions[i].X, positions[i].Y);
            (long X, long Y) vel1 = (velocities[i].X, velocities[i].Y);
            vel1.X = pos1.X + vel1.X * 200000000000000;
            vel1.Y = pos1.Y + vel1.Y * 200000000000000;

            for (int j = i + 1; j < positions.Length; ++j)
            {
                (long X, long Y) pos2 = (positions[j].X, positions[j].Y);
                (long X, long Y) vel2 = (velocities[j].X, velocities[j].Y);
                vel2.X = pos2.X + vel2.X * 200000000000000;
                vel2.Y = pos2.Y + vel2.Y * 200000000000000;

                if (RayLineIntersect2D(pos1, vel1, pos2, vel2, out (double X, double Y) hit))
                {
                    if (hit.X >= min.X && hit.X < max.X && hit.Y >= min.Y && hit.Y < max.Y)
                    {
                        ++intersections;
                    }
                }
            }
        }

        Console.WriteLine($"Intersections inside test area: {intersections}");

        // Part 2

        // Nah no thanks.
    }
}
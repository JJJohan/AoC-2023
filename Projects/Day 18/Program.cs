using System.Diagnostics;
using System.Globalization;

static class Program
{
    private struct Data
    {
        public char Direction;
        public int Metres;
        public string ColorCode;
    }

    private static bool IsPointInPolygon(List<(int X, int Y)> holesDug, (int X, int Y) node)
    {
        bool result = false;
        int j = holesDug.Count - 1;
        for (int i = 0; i < holesDug.Count; i++)
        {
            if (holesDug[i].Y < node.Y && holesDug[j].Y >= node.Y ||
                holesDug[j].Y < node.Y && holesDug[i].Y >= node.Y)
            {
                if (holesDug[i].X + (node.Y - holesDug[i].Y) /
                   (holesDug[j].Y - holesDug[i].Y) *
                   (holesDug[j].X - holesDug[i].X) < node.X)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day18.txt"));

        Data[] data = new Data[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            string[] parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(parts.Length == 3);

            data[i] = new Data
            {
                Direction = parts[0][0],
                Metres = int.Parse(parts[1]),
                ColorCode = parts[2]
            };
        }

        List<(int X, int Y)> holesDug = new();
        (int X, int Y) current = (0, 0);
        holesDug.Add(current);

        int maxX = 0;
        int maxY = 0;
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        for (int i = 0; i < data.Length; ++i)
        {
            ref Data d = ref data[i];
            for (int j = 0; j < d.Metres; ++j)
            {
                if (d.Direction == 'D')
                    current.Y += 1;
                else if (d.Direction == 'U')
                    current.Y -= 1;
                else if (d.Direction == 'L')
                    current.X -= 1;
                else if (d.Direction == 'R')
                    current.X += 1;
                else
                    Debug.Fail("Unexpected direction.");

                minX = Math.Min(current.X, minX);
                minY = Math.Min(current.Y, minY);
                maxX = Math.Max(current.X, maxX);
                maxY = Math.Max(current.Y, maxY);
                holesDug.Add(current);
            }
        }

        long holeArea = 0;
        HashSet<(int, int)> boundary = new();
        for (int i = 0; i < holesDug.Count; ++i)
        {
            boundary.Add((holesDug[i].Item1, holesDug[i].Item2));
        }

        for (int y = minY; y <= maxY; ++y)
        {
            for (int x = minX; x <= maxX; ++x)
            {
                if (boundary.Contains((x, y)) || IsPointInPolygon(holesDug, (x, y)))
                {
                    ++holeArea;
                }
            }
        }

        Console.WriteLine($"Hole area: {holeArea}");

        // Part 2

        holesDug.Clear();
        current = (0, 0);
        holesDug.Add(current);

        // 'fix' the data.
        for (int i = 0; i < data.Length; ++i)
        {
            ref Data d = ref data[i];
            int metres = int.Parse(d.ColorCode.Substring(2, 5), NumberStyles.HexNumber);
            int dir = int.Parse(d.ColorCode.Substring(7, 1), NumberStyles.HexNumber);

            for (int j = 0; j < metres; ++j)
            {
                if (dir == 1) // Down
                    current.Y += 1;
                else if (dir == 3) // Up
                    current.Y -= 1;
                else if (dir == 2) // Left
                    current.X -= 1;
                else if (dir == 0) // Right
                    current.X += 1;
                else
                    Debug.Fail("Unexpected direction.");

                holesDug.Add(current);
            }
        }

        // Calculate the number of lattice points on the boundary
        long boundaryPoints = 0;
        for (int i = 0; i < holesDug.Count; ++i)
        {
            int nextIndex = (i + 1) % holesDug.Count;
            boundaryPoints += GCD(Math.Abs(holesDug[i].X - holesDug[nextIndex].X), Math.Abs(holesDug[i].Y - holesDug[nextIndex].Y));
        }

        // Calculate the number of lattice points strictly inside the polygon using Pick's Theorem
        long interiorPoints = CalculateInteriorPoints(holesDug);

        // Calculate the area using Pick's Theorem
        holeArea = interiorPoints + boundaryPoints / 2 - 1;  

        Console.WriteLine($"Hole area: {holeArea}");
    }

    // Helper method to calculate the greatest common divisor (GCD) using Euclidean algorithm
    private static long GCD(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    // Helper method to calculate the number of lattice points strictly inside the polygon
    private static long CalculateInteriorPoints(List<(int X, int Y)> polygon)
    {
        long interiorPoints = 0;

        // Calculate twice the signed area of the polygon
        for (int i = 0; i < polygon.Count; ++i)
        {
            int nextIndex = (i + 1) % polygon.Count;
            interiorPoints += (polygon[i].X + polygon[nextIndex].X) * (polygon[i].Y - polygon[nextIndex].Y);
        }

        // Take the absolute value and divide by 2 to get the signed area
        long signedArea = Math.Abs(interiorPoints) / 2;

        // Calculate the number of lattice points strictly inside the polygon
        return signedArea - polygon.Count + 1;
    }
}
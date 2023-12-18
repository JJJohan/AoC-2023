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

    private static char ValueToDirChar(int val) => val switch
    {
        0 => 'R',
        1 => 'D',
        2 => 'L',
        3 => 'U',
        _ => throw new InvalidOperationException()
    };

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

                holesDug.Add(current);
            }
        }

        long holeArea = GetPolygonArea(holesDug);

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
            char dir = ValueToDirChar(int.Parse(d.ColorCode.Substring(7, 1), NumberStyles.HexNumber));

            for (int j = 0; j < metres; ++j)
            {
                if (dir == 'D')
                    current.Y += 1;
                else if (dir == 'U')
                    current.Y -= 1;
                else if (dir == 'L')
                    current.X -= 1;
                else if (dir == 'R')
                    current.X += 1;
                else
                    Debug.Fail("Unexpected direction.");

                holesDug.Add(current);
            }
        }

        holeArea = GetPolygonArea(holesDug);        

        Console.WriteLine($"Hole area: {holeArea}");
    }

    private static long GetPolygonArea(List<(int X, int Y)> holesDug)
    {
        // I = S + 1 - B / 2
        long boundaryPoints = CalculateBoundaryPoints(holesDug);
        long areaPoints = CalculateInteriorPoints(holesDug);

        // Comment - wtf? The formula is 'I = S + 1 - B / 2'
        // I only get the correct answer if I make that 'I = |S - 1 - B / 2|'
        return Math.Abs(areaPoints - 1 - boundaryPoints / 2);
    }

    private static long GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    private static long CalculateBoundaryPoints(List<(int X, int Y)> polygon)
    {
        long boundaryPoints = polygon.Count;
        for (int i = 0; i < polygon.Count; ++i)
        {
            int dx = polygon[i].X - polygon[(i + 1) % polygon.Count].X;
            int dy = polygon[i].Y - polygon[(i + 1) % polygon.Count].Y;
            boundaryPoints += Math.Abs(GCD(dx, dy)) - 1;
        }

        return boundaryPoints;
    }

    private static long CalculateInteriorPoints(List<(int X, int Y)> polygon)
    {
        long interiorPoints = 0;

        for (int i = 2; i < polygon.Count; ++i)
        {
            int x1 = polygon[i].X - polygon[0].X;
            int y1 = polygon[i].Y - polygon[0].Y;

            int x2 = polygon[i - 1].X - polygon[0].X;
            int y2 = polygon[i - 1].Y - polygon[0].Y;

            interiorPoints += x1 * y2 - x2 * y1;
        }

        // In the formula the absolute value is returned.. but this works?
        return interiorPoints / 2;
    }
}
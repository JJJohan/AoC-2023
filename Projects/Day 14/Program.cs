using System.Diagnostics;

static class Program
{
    private enum Type
    {
        Empty,
        Fixed,
        Round
    }

    private struct Table
    {
        public Type[][] Grid;
        public int Rows;
        public int Cols;
    }

    private static Type GetType(char c) => c switch
    {
        '.' => Type.Empty,
        '#' => Type.Fixed,
        'O' => Type.Round,
        _ => throw new InvalidOperationException()
    };

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day14.txt"));

        // Parse data
        string firstLine = lines[0];
        Table table = new()
        {
            Grid = new Type[firstLine.Length][],
            Rows = lines.Length,
            Cols = firstLine.Length
        };

        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            Debug.Assert(line.Length == firstLine.Length);
            table.Grid[i] = new Type[line.Length];
            for (int j = 0; j < line.Length; ++j)
            {
                table.Grid[i][j] = GetType(line[j]);
            }
        }

        // Move all the rocks up.
        Type[][] gridCopy = new Type[table.Rows][];
        for (int i = 0; i < table.Rows; ++i)
        {
            gridCopy[i] = new Type[table.Cols];
            Array.Copy(table.Grid[i], gridCopy[i], table.Grid[i].Length);
        }

        for (int y = 1; y < table.Rows; ++y)
        {
            for (int x = 0; x < table.Cols; ++x)
            {
                if (table.Grid[y][x] == Type.Round)
                {
                    long currentRow = y;
                    while (currentRow > 0 && table.Grid[currentRow - 1][x] == Type.Empty)
                    {
                        table.Grid[currentRow - 1][x] = table.Grid[currentRow][x];
                        table.Grid[currentRow][x] = Type.Empty;
                        --currentRow;
                    }
                }
            }
        }

        // Compute the load
        long load = 0;
        for (int y = 0; y < table.Rows; ++y)
        {
            for (int x = 0; x < table.Cols; ++x)
            {
                if (table.Grid[y][x] == Type.Round)
                {
                    load += table.Rows - y;
                }
            }
        }

        Console.WriteLine($"Answer: {load}");

        // Part 2

        // Perform 1 billion cycles
        Stopwatch timer = Stopwatch.StartNew();
        int rowMin1 = table.Rows - 1;
        int colMin1 = table.Cols - 1;

        unchecked
        {
            for (int i = 0; i < 1_000_000_000; ++i)
            {
                // North
                for (int x = 0; x < table.Cols; ++x)
                {
                    for (int y = 1; y < gridCopy.Length; ++y)
                    {
                        Type[] row = gridCopy[y];
                        if (row[x] == Type.Round)
                        {
                            int currentRow = y;
                            while (currentRow > 0 && gridCopy[currentRow - 1][x] == Type.Empty)
                            {
                                --currentRow;
                            }

                            if (currentRow != y)
                            {
                                gridCopy[currentRow][x] = Type.Round;
                                row[x] = Type.Empty;
                            }
                        }
                    }
                }

                // West
                for (int y = 0; y < gridCopy.Length; ++y)
                {
                    Type[] row = gridCopy[y];
                    for (int x = 1; x < row.Length; ++x)
                    {
                        if (row[x] == Type.Round)
                        {
                            int currentCol = x;
                            while (currentCol > 0 && row[currentCol - 1] == Type.Empty)
                            {
                                --currentCol;
                            }

                            if (currentCol != x)
                            {
                                row[currentCol] = Type.Round;
                                row[x] = Type.Empty;
                            }
                        }
                    }
                }

                // South
                for (int x = 0; x < table.Cols; ++x)
                {
                    for (int y = table.Rows - 2; y >= 0; --y)
                    {
                        Type[] row = gridCopy[y];
                        if (row[x] == Type.Round)
                        {
                            int currentRow = y;
                            while (currentRow < rowMin1 && gridCopy[currentRow + 1][x] == Type.Empty)
                            {
                                ++currentRow;
                            }

                            if (currentRow != y)
                            {
                                gridCopy[currentRow][x] = Type.Round;
                                row[x] = Type.Empty;
                            }
                        }
                    }
                }

                // East
                for (int y = 0; y < gridCopy.Length; ++y)
                {
                    Type[] row = gridCopy[y];
                    for (int x = table.Cols - 2; x >= 0; --x)
                    {
                        if (row[x] == Type.Round)
                        {
                            int currentCol = x;
                            while (currentCol < colMin1 && row[currentCol + 1] == Type.Empty)
                            {
                                ++currentCol;
                            }

                            if (currentCol != x)
                            {
                                row[currentCol] = Type.Round;
                                row[x] = Type.Empty;
                            }
                        }
                    }
                }

                if (i % 1_000_000 == 0)
                {
                    int remaining = 1_000_000_000 - i;
                    double timePerIteration = (double)timer.ElapsedMilliseconds / i;
                    timePerIteration *= remaining;

                    double seconds = timePerIteration / 1000;
                    double minutes = seconds / 60;
                    int hours = (int)minutes / 60;
                    int secondsRemaining = (int)(seconds % 60);
                    int minutesRemaining = (int)(minutes % 60);
                    if (i != 0)
                    {
                        (int left, int top) = Console.GetCursorPosition();
                        Console.SetCursorPosition(left, top - 1);
                    }

                    Console.WriteLine($"ETA: {hours}:{minutesRemaining:00}:{secondsRemaining:00} ({remaining} remaining)");
                }
            }
        }

        // Compute the load
        load = 0;
        for (int y = 0; y < table.Rows; ++y)
        {
            for (int x = 0; x < table.Cols; ++x)
            {
                if (gridCopy[y][x] == Type.Round)
                {
                    load += table.Rows - y;
                }
            }
        }

        Console.WriteLine($"Answer: {load}");
        Console.ReadLine();
    }
}
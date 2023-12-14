using System.Diagnostics;

static class Program
{
    private struct Grid
    {
        public bool[,] Rocks;
        public int Rows;
        public int Cols;
        public string[] Lines;
        public int HorizontalReflectionIndex;
        public int VerticalReflectionIndex;
    }

    private static void AddGrid(List<Grid> grids, string[] lines, int i, int startIndex)
    {
        string firstLine = lines[startIndex];

        Grid grid = new()
        {
            Rocks = new bool[firstLine.Length, i - startIndex],
            Rows = i - startIndex,
            Cols = firstLine.Length,
            Lines = new string[i - startIndex],
            VerticalReflectionIndex = -1,
            HorizontalReflectionIndex = -1
        };

        for (int j = startIndex; j < i; ++j)
        {
            string gridLine = lines[j];
            Debug.Assert(gridLine.Length == firstLine.Length);
            grid.Lines[j - startIndex] = gridLine;
            for (int k = 0; k < gridLine.Length; ++k)
            {
                grid.Rocks[k, j - startIndex] = gridLine[k] == '#';
            }
        }
        grids.Add(grid);
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day13.txt"));

        // Parse data
        List<Grid> grids = new();
        int startIndex = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                AddGrid(grids, lines, i, startIndex);
                startIndex = i + 1;
            }
        }

        AddGrid(grids, lines, lines.Length, startIndex);

        Grid[] gridArray = grids.ToArray();

        long sum = 0;

        // Find horizontal reflections
        for (int i = 0; i < gridArray.Length; ++i)
        {
            ref Grid grid = ref gridArray[i];

            int reflectionIndex;
            List<(int, int)> reflections = new();
            for (int y = 0; y < grid.Rows - 1; ++y)
            {
                reflectionIndex = y;
                for (int x = 0; x < grid.Cols; ++x)
                {
                    if (grid.Rocks[x, y] != grid.Rocks[x, y + 1])
                    {
                        reflectionIndex = -1;
                        break;
                    }
                }

                // Validate its a proper reflection, otherwise keep going.
                int largestReflection = 0;
                if (reflectionIndex != -1)
                {
                    for (int j = 1; j < grid.Rows - 1; ++j)
                    {
                        if (reflectionIndex - j + 1 < 0 || reflectionIndex + j >= grid.Rows)
                        {
                            break;
                        }

                        for (int x = 0; x < grid.Cols; ++x)
                        {
                            if (grid.Rocks[x, reflectionIndex - j + 1] != grid.Rocks[x, reflectionIndex + j])
                            {
                                reflectionIndex = -1;
                                break;
                            }
                        }

                        if (reflectionIndex == -1)
                            break;

                        largestReflection = j;
                    }

                    // Valid reflection!
                    if (reflectionIndex != -1)
                    {
                        reflections.Add((largestReflection, reflectionIndex));
                    }
                }
            }

            reflections.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            if (reflections.Count > 0)
            {
                sum += (reflections[reflections.Count - 1].Item2 + 1) * 100;
                grid.HorizontalReflectionIndex = reflections[reflections.Count - 1].Item2;
            }
        }

        // Find vertical reflections
        for (int i = 0; i < gridArray.Length; ++i)
        {
            ref Grid grid = ref gridArray[i];

            int reflectionIndex;
            List<(int, int)> reflections = new();
            for (int x = 0; x < grid.Cols - 1; ++x)
            {
                reflectionIndex = x;
                for (int y = 0; y < grid.Rows; ++y)
                {
                    if (grid.Rocks[x, y] != grid.Rocks[x + 1, y])
                    {
                        reflectionIndex = -1;
                        break;
                    }
                }

                // Validate its a proper reflection, otherwise keep going.
                int largestReflection = 0;
                if (reflectionIndex != -1)
                {
                    for (int j = 1; j < grid.Cols - 1; ++j)
                    {
                        if (reflectionIndex - j + 1 < 0 || reflectionIndex + j >= grid.Cols)
                        {
                            break;
                        }

                        for (int y = 0; y < grid.Rows; ++y)
                        {
                            if (grid.Rocks[reflectionIndex - j + 1, y] != grid.Rocks[reflectionIndex + j, y])
                            {
                                reflectionIndex = -1;
                                break;
                            }
                        }

                        if (reflectionIndex == -1)
                            break;

                        largestReflection = j;
                    }

                    // Valid reflection!
                    if (reflectionIndex != -1)
                    {
                        reflections.Add((largestReflection, reflectionIndex));
                    }
                }
            }

            reflections.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            if (reflections.Count > 0)
            {
                sum += reflections[reflections.Count - 1].Item2 + 1;
                grid.VerticalReflectionIndex = reflections[reflections.Count - 1].Item2;
            }
        }

        Console.WriteLine($"Answer: {sum}");

        // Part 2

        sum = 0;

        // Find horizontal reflections
        for (int i = 0; i < gridArray.Length; ++i)
        {
            ref Grid grid = ref gridArray[i];

            int reflectionIndex;
            List<(int, int)> reflections = new();
            int incorrect = 0;
            for (int y = 0; y < grid.Rows - 1; ++y)
            {
                reflectionIndex = y;
                for (int x = 0; x < grid.Cols; ++x)
                {
                    if (grid.Rocks[x, y] != grid.Rocks[x, y + 1])
                    {
                        ++incorrect;
                        if (incorrect > 1)
                        {
                            reflectionIndex = -1;
                            break;
                        }
                    }
                }

                // Validate its a proper reflection, otherwise keep going.
                int largestReflection = 0;
                incorrect = 0;
                if (reflectionIndex != -1)
                {
                    for (int j = 1; j < grid.Rows - 1; ++j)
                    {
                        if (reflectionIndex - j + 1 < 0 || reflectionIndex + j >= grid.Rows)
                        {
                            break;
                        }

                        for (int x = 0; x < grid.Cols; ++x)
                        {
                            if (grid.Rocks[x, reflectionIndex - j + 1] != grid.Rocks[x, reflectionIndex + j])
                            {
                                ++incorrect;
                                if (incorrect > 1)
                                {
                                    reflectionIndex = -1;
                                    break;
                                }
                            }
                        }

                        if (reflectionIndex == -1)
                            break;

                        largestReflection = j;
                    }

                    // Valid reflection!
                    if (reflectionIndex != -1 && incorrect == 1)
                    {
                        reflections.Add((largestReflection, reflectionIndex));
                    }
                }
            }

            reflections.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            if (reflections.Count > 0)
            {
                sum += (reflections[reflections.Count - 1].Item2 + 1) * 100;
            }
        }

        // Find vertical reflections
        for (int i = 0; i < gridArray.Length; ++i)
        {
            ref Grid grid = ref gridArray[i];

            int reflectionIndex;
            List<(int, int)> reflections = new();
            int incorrect = 0;
            for (int x = 0; x < grid.Cols - 1; ++x)
            {
                reflectionIndex = x;
                for (int y = 0; y < grid.Rows; ++y)
                {
                    if (grid.Rocks[x, y] != grid.Rocks[x + 1, y])
                    {
                        ++incorrect;
                        if (incorrect > 1)
                        {
                            reflectionIndex = -1;
                            break;
                        }
                    }
                }

                // Validate its a proper reflection, otherwise keep going.
                int largestReflection = 0;
                incorrect = 0;
                if (reflectionIndex != -1)
                {
                    for (int j = 1; j < grid.Cols - 1; ++j)
                    {
                        if (reflectionIndex - j + 1 < 0 || reflectionIndex + j >= grid.Cols)
                        {
                            break;
                        }

                        for (int y = 0; y < grid.Rows; ++y)
                        {
                            if (grid.Rocks[reflectionIndex - j + 1, y] != grid.Rocks[reflectionIndex + j, y])
                            {
                                ++incorrect;
                                if (incorrect > 1)
                                {
                                    reflectionIndex = -1;
                                    break;
                                }
                            }
                        }

                        if (reflectionIndex == -1)
                            break;

                        largestReflection = j;
                    }

                    // Valid reflection!
                    if (reflectionIndex != -1 && incorrect == 1)
                    {
                        reflections.Add((largestReflection, reflectionIndex));
                    }
                }
            }

            reflections.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            if (reflections.Count > 0)
            {
                sum += reflections[reflections.Count - 1].Item2 + 1;
            }
        }

        Console.WriteLine($"Answer: {sum}");
    }
}
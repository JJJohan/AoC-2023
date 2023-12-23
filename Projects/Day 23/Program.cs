using System.Text;

static class Program
{
    private struct Tile
    {
        public byte X;
        public byte Y;
        public byte Type;
        public short Distance;
        public short Cost;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day23.txt"));

        // Parse data
        byte rows = (byte)lines.Length;
        byte cols = (byte)lines[0].Length;
        Tile[,] grid = new Tile[cols, rows];
        for (byte y = 0; y < lines.Length; ++y)
        {
            string line = lines[y];
            for (byte x = 0; x < line.Length; ++x)
            {
                grid[x, y] = new()
                {
                    X = x,
                    Y = y,
                    Type = (byte)line[x]
                };
            }
        }

        // Find start & end
        (byte x, byte y) start = (0, 0);
        (byte x, byte y) end = (0, (byte)(rows - 1));
        for (byte x = 0; x < cols; ++x)
        {
            if (grid[x, 0].Type == '.')
            {
                start.x = x;
            }

            if (grid[x, cols - 1].Type == '.')
            {
                end.x = x;
            }
        }

        // Compute distances
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                grid[x, y].Distance = (short)(Math.Abs(x - end.x) + Math.Abs(y - end.y));
            }
        }

        // A* search
        PriorityQueue<(byte, byte, byte, byte, short), short> activeTiles = new();
        activeTiles.Enqueue((start.x, start.y, start.x, start.y, 0), 0);
        Dictionary<(byte, byte), short> visitedTiles = new();
        (byte, byte, short)[] newTiles = new (byte, byte, short)[4];
        short steps = 0;
        while (activeTiles.Count > 0)
        {
            (byte x, byte y, byte prev_x, byte prev_y, short steps) index = activeTiles.Dequeue();
            ref Tile tile = ref grid[index.x, index.y];

            if (tile.X == end.x && tile.Y == end.y)
            {
                steps = Math.Max(steps, index.steps);
            }

            int newTileCount = 0;

            if (index.prev_y != tile.Y - 1 && tile.Y > 0 && (grid[tile.X, tile.Y - 1].Type == '.' || grid[tile.X, tile.Y - 1].Type == '^'))
            {
                newTiles[newTileCount++] = (tile.X, (byte)(tile.Y - 1), (short)(tile.Cost + 1));
            }
            if (index.prev_y != tile.Y + 1 && tile.Y < rows - 1 && (grid[tile.X, tile.Y + 1].Type == '.' || grid[tile.X, tile.Y + 1].Type == 'v'))
            {
                newTiles[newTileCount++] = (tile.X, (byte)(tile.Y + 1), (short)(tile.Cost + 1));
            }
            if (index.prev_x != tile.X - 1 && tile.X > 0 && (grid[tile.X - 1, tile.Y].Type == '.' || grid[tile.X - 1, tile.Y].Type == '<'))
            {
                newTiles[newTileCount++] = ((byte)(tile.X - 1), tile.Y, (short)(tile.Cost + 1));
            }
            if (index.prev_x != tile.X + 1 && tile.X < cols - 1 && (grid[tile.X + 1, tile.Y].Type == '.' || grid[tile.X + 1, tile.Y].Type == '>'))
            {
                newTiles[newTileCount++] = ((byte)(tile.X + 1), tile.Y, (short)(tile.Cost + 1));
            }

            for (int i = 0; i < newTileCount; ++i)
            {
                ref (byte X, byte Y, short Cost) next = ref newTiles[i];

                if (visitedTiles.TryGetValue((next.X, next.Y), out short prevCost))
                {
                    if (prevCost > next.Cost)
                        continue;
                }

                visitedTiles[(next.X, next.Y)] = next.Cost;
                activeTiles.Enqueue((next.X, next.Y, tile.X, tile.Y, (short)(index.steps + 1)), next.Cost);
            }
        }

        Console.WriteLine($"Most amount of steps: {steps}");

        // Part 2

        activeTiles.Enqueue((start.x, start.y, start.x, start.y, 0), 0);
        Dictionary<(byte, byte), byte> visitCounts = new();
        visitedTiles.Clear();
        steps = 0;

        while (activeTiles.Count > 0)
        {
            (byte x, byte y, byte prev_x, byte prev_y, short steps) index = activeTiles.Dequeue();
            ref Tile tile = ref grid[index.x, index.y];

            if (tile.X == end.x && tile.Y == end.y)
            {
                Console.WriteLine(index.steps);
                steps = Math.Max(steps, index.steps);
            }

            int newTileCount = 0;

            if (index.prev_y != tile.Y - 1 && tile.Y > 0 && (grid[tile.X, tile.Y - 1].Type != '#'))
            {
                newTiles[newTileCount++] = (tile.X, (byte)(tile.Y - 1), (short)(tile.Cost + 1));
            }
            if (index.prev_y != tile.Y + 1 && tile.Y < rows - 1 && (grid[tile.X, tile.Y + 1].Type != '#'))
            {
                newTiles[newTileCount++] = (tile.X, (byte)(tile.Y + 1), (short)(tile.Cost + 1));
            }
            if (index.prev_x != tile.X - 1 && tile.X > 0 && (grid[tile.X - 1, tile.Y].Type != '#'))
            {
                newTiles[newTileCount++] = ((byte)(tile.X - 1), tile.Y, (short)(tile.Cost + 1));
            }
            if (index.prev_x != tile.X + 1 && tile.X < cols - 1 && (grid[tile.X + 1, tile.Y].Type != '#'))
            {
                newTiles[newTileCount++] = ((byte)(tile.X + 1), tile.Y, (short)(tile.Cost + 1));
            }

            for (int i = 0; i < newTileCount; ++i)
            {
                ref (byte X, byte Y, short Cost) next = ref newTiles[i];

                if (visitCounts.TryGetValue((next.X, next.Y), out byte visits))
                {
                    if (visits > 100)
                        continue;
                }
                else
                {
                    visits = 0;
                }
                visitCounts[(next.X, next.Y)] = (byte)(visits + 1);

                visitedTiles[(next.X, next.Y)] = next.Cost;
                activeTiles.Enqueue((next.X, next.Y, tile.X, tile.Y, (short)(index.steps + 1)), next.Cost);
            }
        }

        StringBuilder sb = new();
        for (byte y = 0; y < rows; ++y)
        {
            for (byte x = 0; x < cols; ++x)
            {
                if (visitedTiles.ContainsKey((x, y)))
                    sb.Append("\x1b[91mO\x1b[39m");
                else
                    sb.Append((char)grid[x, y].Type);
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());

        Console.WriteLine($"Most amount of steps: {steps}");
    }
}
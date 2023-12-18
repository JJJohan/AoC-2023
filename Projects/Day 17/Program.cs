using System.Text;

static class Program
{
    private const bool PrintToConsole = false;

    private enum Direction : byte
    {
        Right,
        Left,
        Up,
        Down
    }
    private struct Tile
    {
        public byte X;
        public byte Y;
        public Direction Direction;
        public byte RepeatCount;
        public ushort HeatLoss;
        public Dictionary<(int, int), Direction> History;

        public Tile(Dictionary<(int, int), Direction> history, byte x, byte y, Direction direction, byte repeatCount, ushort heatLoss)
        {
            X = x;
            Y = y;
            Direction = direction;
            RepeatCount = repeatCount;
            HeatLoss = heatLoss;
            History = history;
        }
    }

    private static int rows;
    private static int cols;
    private static (byte X, byte Y) end;
    private static ushort[,] heatValues = new ushort[0, 0];

    private static void EnqueueTile(in Tile current, byte repeatCount, Direction direction, PriorityQueue<Tile, ushort> queue,
        Dictionary<(byte, byte, Direction, byte), ushort> cache)
    {
        byte x = (byte)(direction == Direction.Left ? current.X - 1 : direction == Direction.Right ? current.X + 1 : current.X);
        byte y = (byte)(direction == Direction.Down ? current.Y + 1 : direction == Direction.Up ? current.Y - 1 : current.Y);
        (byte, byte, Direction, byte) cacheKey = (x, y, direction, repeatCount);
        ushort newCost = (ushort)(current.HeatLoss + heatValues[x, y]);

        if (cache.TryGetValue(cacheKey, out ushort lowestHeat))
        {
            if (newCost >= lowestHeat) return;

            cache[cacheKey] = newCost;
        }
        else
        {
            cache.Add(cacheKey, newCost);
        }

        Dictionary<(int, int), Direction> newHistory = new(current.History);
        newHistory.TryAdd((x, y), direction);
        queue.Enqueue(new(newHistory, x, y, direction, repeatCount, newCost), (ushort)(Math.Abs(x - end.X) + Math.Abs(y - end.Y) + newCost));
    }

    private static void PrintShortestPath(in Tile path)
    {
        StringBuilder sb = new();
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                if (path.History.TryGetValue((x, y), out Direction dir))
                {
                    sb.Append("\x1b[91m");
                    if (dir == Direction.Left)
                        sb.Append('<');
                    else if (dir == Direction.Right)
                        sb.Append('>');
                    else if (dir == Direction.Up)
                        sb.Append('^');
                    else
                        sb.Append('V');
                    sb.Append("\x1b[39m");
                }
                else
                {
                    sb.Append(heatValues[x, y]);
                }
            }
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day17.txt"));

        rows = lines.Length;
        cols = lines[0].Length;

        heatValues = new ushort[cols, rows];
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            ushort[] numbers = line.Select(c => (ushort)(c - '0')).ToArray();
            for (int j = 0; j < cols; ++j)
                heatValues[j, i] = numbers[j];
        }

        end = ((byte)(cols - 1), (byte)(rows - 1));

        Tile start1 = new(new() { { (1, 0), Direction.Right } }, 1, 0, Direction.Right, 1, heatValues[1, 0]);
        Tile start2 = new(new() { { (0, 1), Direction.Down } }, 0, 1, Direction.Down, 1, heatValues[0, 1]);

        PriorityQueue<Tile, ushort> activeTiles = new();
        activeTiles.Enqueue(start1, 0);
        activeTiles.Enqueue(start2, 0);
        Dictionary<(byte, byte, Direction, byte), ushort> visitedTiles = new();

        Tile shortestPath = new();
        int lowestHeatLoss = int.MaxValue;
        while (activeTiles.Count > 0)
        {
            Tile current = activeTiles.Dequeue();

            if (current.X == end.X && current.Y == end.Y && current.HeatLoss < lowestHeatLoss)
            {
                shortestPath = current;
                lowestHeatLoss = current.HeatLoss;
                break;
            }

            if (current.Direction == Direction.Left)
            {
                if (current.RepeatCount < 2 && current.X > 0)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Left, activeTiles, visitedTiles);
                if (current.Y > 0)
                    EnqueueTile(current, 0, Direction.Up, activeTiles, visitedTiles);
                if (current.Y < rows - 1)
                    EnqueueTile(current, 0, Direction.Down, activeTiles, visitedTiles);
            }
            else if (current.Direction == Direction.Right)
            {
                if (current.RepeatCount < 2 && current.X < cols - 1)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Right, activeTiles, visitedTiles);
                if (current.Y > 0)
                    EnqueueTile(current, 0, Direction.Up, activeTiles, visitedTiles);
                if (current.Y < rows - 1)
                    EnqueueTile(current, 0, Direction.Down, activeTiles, visitedTiles);
            }
            else if (current.Direction == Direction.Up)
            {
                if (current.RepeatCount < 2 && current.Y > 0)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Up, activeTiles, visitedTiles);
                if (current.X > 0)
                    EnqueueTile(current, 0, Direction.Left, activeTiles, visitedTiles);
                if (current.X < cols - 1)
                    EnqueueTile(current, 0, Direction.Right, activeTiles, visitedTiles);
            }
            else // Down
            {
                if (current.RepeatCount < 2 && current.Y < rows - 1)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Down, activeTiles, visitedTiles);
                if (current.X > 0)
                    EnqueueTile(current, 0, Direction.Left, activeTiles, visitedTiles);
                if (current.X < cols - 1)
                    EnqueueTile(current, 0, Direction.Right, activeTiles, visitedTiles);
            }
        }

        PrintShortestPath(shortestPath);

        Console.WriteLine($"Lowest heat loss: {lowestHeatLoss}");

        // Part 2

        activeTiles.Clear();
        start1 = new(new() { { (1, 0), Direction.Right } }, 1, 0, Direction.Right, 0, heatValues[1, 0]);   
        start2 = new(new() { { (0, 1), Direction.Down } }, 0, 1, Direction.Down, 0, heatValues[0, 1]);
        activeTiles.Enqueue(start1, 0);
        activeTiles.Enqueue(start2, 0);
        visitedTiles.Clear();

        shortestPath = new();
        lowestHeatLoss = int.MaxValue;
        while (activeTiles.Count > 0)
        {
            Tile current = activeTiles.Dequeue();

            if (current.X == end.X && current.Y == end.Y && current.HeatLoss < lowestHeatLoss && current.RepeatCount >= 3)
            {
                shortestPath = current;
                lowestHeatLoss = current.HeatLoss;
                Console.WriteLine($"New entry: {lowestHeatLoss}");
                continue;
            }

            if (current.Direction == Direction.Left)
            {
                if (current.RepeatCount < 9 && current.X > 0)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Left, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.Y > 0)
                    EnqueueTile(current, 0, Direction.Up, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.Y < rows - 1)
                    EnqueueTile(current, 0, Direction.Down, activeTiles, visitedTiles);
            }
            else if (current.Direction == Direction.Right)
            {
                if (current.RepeatCount < 9 && current.X < cols - 1)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Right, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.Y > 0)
                    EnqueueTile(current, 0, Direction.Up, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.Y < rows - 1)
                    EnqueueTile(current, 0, Direction.Down, activeTiles, visitedTiles);
            }
            else if (current.Direction == Direction.Up)
            {
                if (current.RepeatCount < 9 && current.Y > 0)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Up, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.X > 0)
                    EnqueueTile(current, 0, Direction.Left, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.X < cols - 1)
                    EnqueueTile(current, 0, Direction.Right, activeTiles, visitedTiles);
            }
            else // Down
            {
                if (current.RepeatCount < 9 && current.Y < rows - 1)
                    EnqueueTile(current, (byte)(current.RepeatCount + 1), Direction.Down, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.X > 0)
                    EnqueueTile(current, 0, Direction.Left, activeTiles, visitedTiles);
                if (current.RepeatCount >= 3 && current.X < cols - 1)
                    EnqueueTile(current, 0, Direction.Right, activeTiles, visitedTiles);
            }
        }

        if (PrintToConsole)
        {
            PrintShortestPath(shortestPath);
        }

        Console.WriteLine($"Lowest heat loss: {lowestHeatLoss}");
    }
}
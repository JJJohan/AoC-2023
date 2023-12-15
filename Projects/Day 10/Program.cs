using System.Diagnostics;
using System.Drawing;
using System.Text;

static class Program
{
    [Flags]
    private enum Directions
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8
    }

    private class Node
    {
        public Directions Directions;
        public int X;
        public int Y;
        public int Value;
        public bool InsideLoop;
        public bool LoopNode;
        public Node? Previous;
        public Node? Next;
    }

    private static Directions Fail(char c)
    {
        Debug.Fail($"Unexpected char input: '{c}'.");
        return 0;
    }

    private static Directions GetDirections(char c)
    {
        return c switch
        {
            '|' => Directions.Up | Directions.Down,
            '-' => Directions.Left | Directions.Right,
            'L' => Directions.Up | Directions.Right,
            'J' => Directions.Up | Directions.Left,
            '7' => Directions.Down | Directions.Left,
            'F' => Directions.Down | Directions.Right,
            'S' => Directions.Up | Directions.Down | Directions.Left | Directions.Right,
            '.' => 0,
            _ => Fail(c)
        };
    }

    private static bool IsPointInPolygon(List<Node> loopNodes, Node node)
    {
        bool result = false;
        int j = loopNodes.Count - 1;
        for (int i = 0; i < loopNodes.Count; i++)
        {
            if (loopNodes[i].Y < node.Y && loopNodes[j].Y >= node.Y ||
                loopNodes[j].Y < node.Y && loopNodes[i].Y >= node.Y)
            {
                if (loopNodes[i].X + (node.Y - loopNodes[i].Y) /
                   (loopNodes[j].Y - loopNodes[i].Y) *
                   (loopNodes[j].X - loopNodes[i].X) < node.X)
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
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day10.txt"));

        // Build nodes

        Node[][] nodes = new Node[lines.Length][];
        Node? start = null;

        int lineLength = lines[0].Length;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            Debug.Assert(line.Length == lineLength);
            nodes[i] = new Node[line.Length];

            for (int j = 0; j < line.Length; ++j)
            {
                char c = line[j];

                nodes[i][j] = new Node
                {
                    X = j,
                    Y = i,
                    Directions = GetDirections(c)
                };

                if (c == 'S')
                {
                    start = nodes[i][j];
                }
            }
        }

        // Build path (simple, not really an algorithm like A*, etc.)
        Debug.Assert(start != null);

        Queue<Node> activeNodes = new();
        activeNodes.Enqueue(start);

        int maxSteps = 0;
        while (activeNodes.Count > 0)
        {
            Node node = activeNodes.Dequeue();
            maxSteps = Math.Max(node.Value, maxSteps);

            if (node.Directions.HasFlag(Directions.Left) && node.X > 0)
            {
                Node nextNode = nodes[node.Y][node.X - 1];
                if (nextNode.Directions.HasFlag(Directions.Right))
                {
                    if (nextNode.Value == 0)
                    {
                        nextNode.Value = node.Value + 1;
                        activeNodes.Enqueue(nextNode);
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Right) && node.X < lineLength)
            {
                Node nextNode = nodes[node.Y][node.X + 1];
                if (nextNode.Directions.HasFlag(Directions.Left))
                {
                    if (nextNode.Value == 0)
                    {
                        nextNode.Value = node.Value + 1;
                        activeNodes.Enqueue(nextNode);
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Up) && node.Y > 0)
            {
                Node nextNode = nodes[node.Y - 1][node.X];
                if (nextNode.Directions.HasFlag(Directions.Down))
                {
                    if (nextNode.Value == 0)
                    {
                        nextNode.Value = node.Value + 1;
                        activeNodes.Enqueue(nextNode);
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Down) && node.Y < lines.Length)
            {
                Node nextNode = nodes[node.Y + 1][node.X];
                if (nextNode.Directions.HasFlag(Directions.Up))
                {
                    if (nextNode.Value == 0)
                    {
                        nextNode.Value = node.Value + 1;
                        activeNodes.Enqueue(nextNode);
                    }
                }
            }
        }

        Console.WriteLine($"Answer: {maxSteps}");

        // Part 2

        Stack<Node> nodeStack = new();
        nodeStack.Push(start);
        while (nodeStack.Count > 0)
        {
            Node node = nodeStack.Pop();
            maxSteps = Math.Max(node.Value, maxSteps);

            if (node.Directions.HasFlag(Directions.Left) && node.X > 0)
            {
                Node nextNode = nodes[node.Y][node.X - 1];
                if (nextNode.Directions.HasFlag(Directions.Right))
                {
                    if (nextNode.Next != node)
                    {
                        nextNode.Previous = node;
                        node.Next = nextNode;
                        if (nextNode != start)
                        {
                            nodeStack.Push(nextNode);
                        }
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Right) && node.X < lineLength)
            {
                Node nextNode = nodes[node.Y][node.X + 1];
                if (nextNode.Directions.HasFlag(Directions.Left))
                {
                    if (nextNode.Next != node)
                    {
                        nextNode.Previous = node;
                        node.Next = nextNode;
                        if (nextNode != start)
                        {
                            nodeStack.Push(nextNode);
                        }
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Up) && node.Y > 0)
            {
                Node nextNode = nodes[node.Y - 1][node.X];
                if (nextNode.Directions.HasFlag(Directions.Down))
                {
                    if (nextNode.Next != node)
                    {
                        nextNode.Previous = node;
                        node.Next = nextNode;
                        if (nextNode != start)
                        {
                            nodeStack.Push(nextNode);
                        }
                    }
                }
            }
            if (node.Directions.HasFlag(Directions.Down) && node.Y < lines.Length)
            {
                Node nextNode = nodes[node.Y + 1][node.X];
                if (nextNode.Directions.HasFlag(Directions.Up))
                {
                    if (nextNode.Next != node)
                    {
                        nextNode.Previous = node;
                        node.Next = nextNode;

                        if (nextNode != start)
                        {
                            nodeStack.Push(nextNode);
                        }
                    }
                }
            }
        }

        int rows = lineLength;
        int cols = lines.Length;

        Node? prev = start.Previous;
        while (prev != null && prev != start)
        {
            prev.LoopNode = true;
            prev = prev.Previous;
        }

        List<Node> loopNodes = [start];
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                Node node = nodes[y][x];
                if (node.LoopNode) loopNodes.Add(node);
            }
        }

        int totalInside = 0;
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
        {
                Node node = nodes[y][x];
                if (!node.LoopNode && IsPointInPolygon(loopNodes, node))
                {
                    node.InsideLoop = true;
                    ++totalInside;
                }
            }
        }

        StringBuilder sb = new();
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                Node node = nodes[y][x];
                if (node.LoopNode)
                {
                    if (node.Directions.HasFlag(Directions.Up))
                    {
                        if (node.Directions.HasFlag(Directions.Down))
                        {
                            sb.Append('|');
                        }
                        if (node.Directions.HasFlag(Directions.Left))
                        {
                            sb.Append('┘');
                        }
                        if (node.Directions.HasFlag(Directions.Right))
                        {
                            sb.Append('└');
                        }
                    }
                    else if (node.Directions.HasFlag(Directions.Down))
                    {
                        if (node.Directions.HasFlag(Directions.Left))
                        {
                            sb.Append('┐');
                        }
                        if (node.Directions.HasFlag(Directions.Right))
                        {
                            sb.Append('┌');
                        }
                    }
                    else if (node.Directions.HasFlag(Directions.Left) && node.Directions.HasFlag(Directions.Right))
                    {
                        sb.Append('─');
                    }
                }
                else if (node.InsideLoop)
                {
                    sb.Append('I');
                }
                else
                {

                    sb.Append(' ');
                }
            }
            sb.AppendLine();
        }

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        File.WriteAllText(Path.Combine(desktop, "test.txt"), sb.ToString());

        Console.WriteLine($"Total cells inside: {totalInside}");
    }
}
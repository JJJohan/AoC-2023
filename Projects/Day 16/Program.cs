static class Program
{
    private enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    private static (int, int) GetNextCoord((int, int) coord, Direction dir) => dir switch
    {
        Direction.Left => (coord.Item1 - 1, coord.Item2),
        Direction.Up => (coord.Item1, coord.Item2 - 1),
        Direction.Right => (coord.Item1 + 1, coord.Item2),
        Direction.Down => (coord.Item1, coord.Item2 + 1),
        _ => throw new InvalidOperationException()
    };

    private class Beam
    {
        public (int X, int Y) Coord;
        public Direction Direction;
    }

    private static int ProcessBeams(Queue<Beam> beams, string[] lines)
    {
        int rows = lines.Length;
        int cols = lines[0].Length;

        (bool, HashSet<Direction>)[,] energised = new (bool, HashSet<Direction>)[cols, rows];
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {
                energised[x, y] = (false, new HashSet<Direction>());
            }
        }

        while (beams.Count > 0)
        {
            Beam beam = beams.Peek();

            char c = lines[beam.Coord.Y][beam.Coord.X];

            if (c == '.' || ((beam.Direction == Direction.Left || beam.Direction == Direction.Right) && c == '-')
                || ((beam.Direction == Direction.Up || beam.Direction == Direction.Down) && c == '|'))
            {
                // Do nothing.
            }
            else if (c == '/')
            {
                if (beam.Direction == Direction.Right)
                {
                    beam.Direction = Direction.Up;
                }
                else if (beam.Direction == Direction.Down)
                {
                    beam.Direction = Direction.Left;
                }
                else if (beam.Direction == Direction.Left)
                {
                    beam.Direction = Direction.Down;
                }
                else if (beam.Direction == Direction.Up)
                {
                    beam.Direction = Direction.Right;
                }
            }
            else if (c == '\\')
            {
                if (beam.Direction == Direction.Right)
                {
                    beam.Direction = Direction.Down;
                }
                else if (beam.Direction == Direction.Down)
                {
                    beam.Direction = Direction.Right;
                }
                else if (beam.Direction == Direction.Left)
                {
                    beam.Direction = Direction.Up;
                }
                else if (beam.Direction == Direction.Up)
                {
                    beam.Direction = Direction.Left;
                }
            }
            else if (c == '|' || c == '-')
            {
                Beam newBeam = new();
                newBeam.Coord = beam.Coord;
                if (beam.Direction == Direction.Left || beam.Direction == Direction.Right)
                {
                    newBeam.Direction = Direction.Up;
                    beam.Direction = Direction.Down;
                }
                else
                {
                    newBeam.Direction = Direction.Left;
                    beam.Direction = Direction.Right;
                }
                beams.Enqueue(newBeam);
            }

            (bool, HashSet<Direction>) entry = energised[beam.Coord.X, beam.Coord.Y];
            if (entry.Item2.Contains(beam.Direction))
            {
                beams.Dequeue();
                continue;
            }

            entry.Item2.Add(beam.Direction);
            energised[beam.Coord.X, beam.Coord.Y] = (true, entry.Item2);

            beam.Coord = GetNextCoord(beam.Coord, beam.Direction);
            if (beam.Coord.X < 0 || beam.Coord.X >= cols || beam.Coord.Y < 0 || beam.Coord.Y >= rows)
            {
                beams.Dequeue();
                continue;
            }
        }

        int energisedTiles = 0;
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                if (energised[x, y].Item1)
                {
                    ++energisedTiles;
                }
            }
        }

        return energisedTiles;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day16.txt"));

        int rows = lines.Length;
        int cols = lines[0].Length;

        // If the beam encounters empty space(.), it continues in the same direction.
        // If the beam encounters a mirror(/ or \), the beam is reflected 90 degrees depending on the angle of the mirror. For instance, a rightward - moving beam that encounters a / mirror would continue upward in the mirror's column, while a rightward-moving beam that encounters a \ mirror would continue downward from the mirror's column.
        // If the beam encounters the pointy end of a splitter(| or -), the beam passes through the splitter as if the splitter were empty space. For instance, a rightward - moving beam that encounters a -splitter would continue in the same direction.
        // If the beam encounters the flat side of a splitter(| or -), the beam is split into two beams going in each of the two directions the splitter's pointy ends are pointing. For instance, a rightward-moving beam that encounters a | splitter would split into two beams: one that continues upward from the splitter's column and one that continues downward from the splitter's column.

        Beam beam = new()
        {
            Coord = (0, 0),
            Direction = Direction.Right
        };

        Queue<Beam> beams = new();
        beams.Enqueue(beam);

        int energisedTiles = ProcessBeams(beams, lines);

        Console.WriteLine($"Answer: {energisedTiles}");

        // Part 2

        int mostTiles = 0;

        for (int x = 0; x < cols; ++x)
        {
            beams.Enqueue(new() { Coord = (x, 0), Direction = Direction.Down });
            energisedTiles = ProcessBeams(beams, lines);
            mostTiles = Math.Max(energisedTiles, mostTiles);

            beams.Enqueue(new() { Coord = (x, rows - 1), Direction = Direction.Up });
            energisedTiles = ProcessBeams(beams, lines);
            mostTiles = Math.Max(energisedTiles, mostTiles);
        }

        for (int y = 0; y < rows; ++y)
        {
            beams.Enqueue(new() { Coord = (0, y), Direction = Direction.Right });
            energisedTiles = ProcessBeams(beams, lines);
            mostTiles = Math.Max(energisedTiles, mostTiles);

            beams.Enqueue(new() { Coord = (cols - 1, y), Direction = Direction.Left });
            energisedTiles = ProcessBeams(beams, lines);
            mostTiles = Math.Max(energisedTiles, mostTiles);
        }

        Console.WriteLine($"Answer: {mostTiles}");
    }
}
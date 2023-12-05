using System.Diagnostics;

static class Program
{
    struct Map
    {
        public long DestinationStart;
        public long SourceStart;
        public int Count;

        public Map(long dstStart, long srcStart, int count)
        {
            DestinationStart = dstStart;
            SourceStart = srcStart;
            Count = count;
        }
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day5.txt"));


        string seedText = lines[0];
        seedText = seedText.Split(':')[1];
        string[] seedBits = seedText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        long[] seeds = new long[seedBits.Length];
        for (int i = 0; i < seeds.Length; ++i)
            seeds[i] = long.Parse(seedBits[i]);

        List<List<Map>> mapList = new();
        List<Map>? map = null;
        for (int i = 1; i < lines.Length; ++i)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                ++i;
                map = new();
                mapList.Add(map);
                continue;
            }

            Debug.Assert(map != null);
            string[] bits = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Debug.Assert(bits.Length == 3);
            long destinationStart = long.Parse(bits[0]);
            long sourceStart = long.Parse(bits[1]);
            int count = int.Parse(bits[2]);

            map.Add(new(destinationStart, sourceStart, count));
        }

        long[] mappedValues = seeds.ToArray();
        foreach (List<Map> list in mapList)
        {
            for (int i = 0; i < mappedValues.Length; ++i)
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    ref long value = ref mappedValues[i];
                    Map m = list[j];
                    if (value >= m.SourceStart && value < m.SourceStart + m.Count)
                    {
                        long offset = value - m.SourceStart;
                        value = m.DestinationStart + offset;
                        break;
                    }
                }
            }
        }

        long lowestLocationNumber = mappedValues.Min();

        Console.WriteLine($"Answer: {lowestLocationNumber}");

        // Part 2

        List<long> seedRanges = new List<long>();
        for (int i = 0; i < seeds.Length; i += 2)
        {
            long start = seeds[i];
            long count = seeds[i + 1];

            for (int j = 0; j < count; ++j)
            {
                seedRanges.Add(start + j);
            }
        }

        // Oh my god my memory usage.

        mappedValues = seedRanges.ToArray();
        foreach (List<Map> list in mapList)
        {
            // Yeah I'm just.. doubling down here with part 1 code.. It works though! :D
            Parallel.For(0, mappedValues.Length, i =>
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    ref long value = ref mappedValues[i];
                    Map m = list[j];
                    if (value >= m.SourceStart && value < m.SourceStart + m.Count)
                    {
                        long offset = value - m.SourceStart;
                        value = m.DestinationStart + offset;
                        break;
                    }
                }
            });
        }

        lowestLocationNumber = mappedValues.Min();

        Console.WriteLine($"Answer: {lowestLocationNumber}");
    }
}
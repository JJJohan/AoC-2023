using System.Diagnostics;

static class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day6.txt"));

        Debug.Assert(lines.Length == 2);
        int[] times = lines[0].Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        int[] distances = lines[1].Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

        Debug.Assert(times.Length == distances.Length);
        int sumOfWaysToWin = 0;
        for (int i = 0; i < times.Length; ++i)
        {
            int time = times[i];
            int distance = distances[i];

            int waysToWin = 0;
            for (int j = 1; j < time; ++j)
            {
                int remainingTime = time - j;
                int travelled = j * remainingTime;
                if (travelled >= distance)
                {
                    ++waysToWin;
                }
            }

            sumOfWaysToWin = sumOfWaysToWin == 0 ? waysToWin : sumOfWaysToWin * waysToWin;
        }

        Console.WriteLine($"Answer: {sumOfWaysToWin}");

        // Part 2

        long totalTime = long.Parse(lines[0].Split(':').Last().Replace(" ", ""));
        long totalDistance = long.Parse(lines[1].Split(':').Last().Replace(" ", ""));

        int totalWaysToWin = 0;
        for (long j = 1; j < totalTime; ++j)
        {
            long remainingTime = totalTime - j;
            long travelled = j * remainingTime;
            if (travelled >= totalDistance)
            {
                ++totalWaysToWin;
            }
        }

        Console.WriteLine($"Answer: {totalWaysToWin}");
    }
}
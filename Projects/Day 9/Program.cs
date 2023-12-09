static class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day9.txt"));

        long totalSum = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];

            int[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
            List<int[]> diffSet = [numbers];
            while (true)
            {
                int lineDifference = 0;
                int[] lastDiffSet = diffSet.Last();
                int[] diffs = new int[lastDiffSet.Length - 1];
                for (int j = 1; j < lastDiffSet.Length; ++j)
                {
                    int diff = lastDiffSet[j] - lastDiffSet[j - 1];
                    diffs[j - 1] = diff;
                    lineDifference += diff;
                }

                if (lineDifference == 0)
                {
                    break;
                }

                diffSet.Add(diffs);
            }

            int extrapolatedNumber = numbers.Last();
            for (int j = 1; j < diffSet.Count; ++j)
            {
                extrapolatedNumber += diffSet[j].Last();
            }

            totalSum += extrapolatedNumber;
        }

        Console.WriteLine($"Answer: {totalSum}");

        // Part 2

        totalSum = 0;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];

            int[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
            List<int[]> diffSet = [numbers];
            while (true)
            {
                int lineDifference = 0;
                int[] lastDiffSet = diffSet.Last();
                int[] diffs = new int[lastDiffSet.Length - 1];
                for (int j = 1; j < lastDiffSet.Length; ++j)
                {
                    int diff = lastDiffSet[j - 1] - lastDiffSet[j];
                    diffs[j - 1] = diff;
                    lineDifference += diff;
                }

                if (lineDifference == 0)
                {
                    break;
                }

                diffSet.Add(diffs);
            }

            int extrapolatedNumber = numbers[0];
            for (int j = 1; j < diffSet.Count; ++j)
            {
                extrapolatedNumber += diffSet[j][0];
            }

            totalSum += extrapolatedNumber;
        }

        Console.WriteLine($"Answer: {totalSum}");
    }
}
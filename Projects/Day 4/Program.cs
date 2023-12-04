static class Program
{
    struct Line
    {
        public HashSet<int> WinningNumbers;
        public HashSet<int> OtherNumbers;
        public int Matches;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day4.txt"));
        Line[] data = new Line[lines.Length];

        for (int i = 0; i < lines.Length; ++i)
        {
            ref Line d = ref data[i];
            d.WinningNumbers = new();
            d.OtherNumbers = new();
            d.Matches = 0;

            string l = lines[i];
            l = l.Split(':').Last();

            string[] parts = l.Split('|');
            string winningNumbers = parts[0];

            string[] winningNumberParts = winningNumbers.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string number in winningNumberParts)
            {
                d.WinningNumbers.Add(int.Parse(number));
            }

            string otherNumbers = parts[1];
            string[] otherNumberParts = otherNumbers.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string number in otherNumberParts)
            {
                d.OtherNumbers.Add(int.Parse(number));
            }
        }

        int total = 0;
        for (int i = 0; i < data.Length; ++i)
        {
            ref Line d = ref data[i];
            int lineValue = 0;
            foreach (int winningNumber in d.WinningNumbers)
            {
                if (d.OtherNumbers.Contains(winningNumber))
                {
                    lineValue = lineValue == 0 ? 1 : lineValue * 2;
                    ++d.Matches;
                }
            }

            total += lineValue;
        }

        Console.WriteLine($"Total: {total}");

        // Part 2

        int totalCards = data.Length;
        Stack<int> scratchCards = new();
        for (int i = 0; i < totalCards; ++i)
        {
            scratchCards.Push(i);
        }

        while (scratchCards.Count > 0)
        {
            int index = scratchCards.Pop();
            ref Line card = ref data[index];

            for (int i = 0; i < card.Matches; ++i)
            {
                int newIndex = index + i + 1;
                if (newIndex >= data.Length) break;
                scratchCards.Push(newIndex);
                ++totalCards;
            }
        }

        Console.WriteLine($"Total scratch cards: {totalCards}");
    }
}
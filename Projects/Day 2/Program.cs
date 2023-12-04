using System.Diagnostics;

static class Program
{
    private struct Game
    {
        public int Id;
        public int MinRed;
        public int MinGreen;
        public int MinBlue;
        public int MaxRed;
        public int MaxGreen;
        public int MaxBlue;
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day2.txt"));
        Game[] games = new Game[lines.Length];

        // Game number seems to match line number, but let's be safe by reading the ID from the line itself.
        // First parse the values into min & max values for each colour.
        for (int i = 0; i < lines.Length; ++i)
        {
            ref Game game = ref games[i];
            game.MinRed = int.MaxValue;
            game.MinGreen = int.MaxValue;
            game.MinBlue = int.MaxValue;

            string[] parts = lines[i].Split(':');
            Debug.Assert(parts.Length == 2);
            parts[0] = parts[0].Replace("Game ", "");
            game.Id = int.Parse(parts[0]);

            string[] rounds = parts[1].Split(';');
            foreach (string round in rounds)
            {
                string[] countAndColours = round.Split(',');
                foreach (string countAndColour in countAndColours)
                {
                    string[] countAndColourSplit = countAndColour.Trim().Split(' ');
                    Debug.Assert(countAndColourSplit.Length == 2);

                    int count = int.Parse(countAndColourSplit[0]);
                    string colour = countAndColourSplit[1];

                    switch (colour)
                    {
                        case "red":
                            game.MinRed = game.MinRed == int.MaxValue ? count : Math.Max(game.MinRed, count);
                            game.MaxRed = Math.Max(game.MaxRed, count);
                            break;
                        case "green":
                            game.MinGreen = game.MinGreen == int.MaxValue ? count : Math.Max(game.MinGreen, count);
                            game.MaxGreen = Math.Max(game.MaxGreen, count);
                            break;
                        case "blue":
                            game.MinBlue = game.MinBlue == int.MaxValue ? count : Math.Max(game.MinBlue, count);
                            game.MaxBlue = Math.Max(game.MaxBlue, count);
                            break;
                        default:
                            Debug.Fail("Unexpected value.");
                            break;
                    }
                }
            }
        }

        // Part 1 - get valid number of games.

        const int maxRed = 12;
        const int maxGreen = 13;
        const int maxBlue = 14;

        int sumOfMatchingGames = 0;

        for (int i = 0; i < games.Length; ++i)
        {
            ref Game game = ref games[i];
            if (game.MaxRed <= maxRed && game.MaxGreen <= maxGreen && game.MaxBlue <= maxBlue)
            {
                sumOfMatchingGames += game.Id;
            }
        }

        Console.WriteLine($"Sum of matching games: {sumOfMatchingGames}");

        // Part 2 - Minimum number required to get all games, then multiplied and summed.

        int sumPower = 0;

        for (int i = 0; i < games.Length; ++i)
        {
            ref Game game = ref games[i];
            sumPower += game.MinRed * game.MinGreen * game.MinBlue;
        }

        Console.WriteLine($"Sum of power of minimum cubes in all games: {sumPower}");
    }
}
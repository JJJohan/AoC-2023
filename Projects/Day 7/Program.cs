using System.Diagnostics;

static class Program
{
    private struct HandAndBid
    {
        public char[] Hand;
        public int Bid;
    }

    private static int GetHandRank(in HandAndBid handAndBid, out Dictionary<char, int> matches, bool withJoker)
    {
        matches = new();
        int jokers = 0;
        for (int i = 0; i < handAndBid.Hand.Length; ++i)
        {
            char c = handAndBid.Hand[i];

            if (withJoker && c == 'J')
            {
                ++jokers;
                continue;
            }

            if (!matches.TryGetValue(c, out int count))
                count = 0;

            matches[c] = count + 1;
        }

        int highest1 = 0;
        int highest2 = 0;

        foreach (KeyValuePair<char, int> kvp in matches)
        {
            if (kvp.Value > highest1)
            {
                highest2 = highest1;
                highest1 = kvp.Value;
            }
            else if (kvp.Value > highest2)
            {
                highest2 = kvp.Value;
            }
        }

        if (withJoker)
        {
            // five of a kind
            for (int i = jokers; i >= 0; --i)
                if (highest1 + i == 5) return 6;

            // four of a kind
            for (int i = jokers; i >= 0; --i)
                if (highest1 + i == 4) return 5;

            // full house
            if (highest1 == 3 && highest2 == 2) return 4;
            if (jokers >= 1 && highest1 == 2 && highest2 == 2) return 4;
            if (jokers >= 1 && highest1 == 3 && highest2 == 1) return 4;

            // three of a kind
            for (int i = jokers; i >= 0; --i)
                if (highest1 + i == 3) return 3;

            // two pair
            if (highest1 == 2 && highest2 == 2) return 2;
            if (jokers >= 1 && highest1 == 2 && highest2 == 1) return 2;
            if (jokers >= 2 && highest1 == 2 && highest2 == 0) return 2;

            // one pair
            for (int i = jokers; i >= 0; --i)
                if (highest1 + i == 2) return 1;

            return 0; // high card
        }
        
        return highest1 switch
        {
            5 => 6, // five of a kind
            4 => 5, // four of a kind
            3 when highest2 == 2 => 4, // full house
            3 => 3, // three of a kind
            2 when highest2 == 2 => 2, // two pair
            2 => 1, // one pair
            _ => 0 // high card
        };
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day7.txt"));

        Dictionary<char, int> pointLookup = new()
        {
            { '2', 1 },
            { '3', 2 },
            { '4', 3 },
            { '5', 4 },
            { '6', 5 },
            { '7', 6 },
            { '8', 7 },
            { '9', 8 },
            { 'T', 9 },
            { 'J', 10 },
            { 'Q', 11 },
            { 'K', 12 },
            { 'A', 13 },
        };

        HandAndBid[] handsAndBids = new HandAndBid[lines.Length];
        for (int i = 0; i <  lines.Length; ++i)
        {
            string line = lines[i];
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Debug.Assert(parts.Length == 2);
            Debug.Assert(parts[0].Length == 5);
            handsAndBids[i] = new() { Hand = parts[0].ToCharArray(), Bid = int.Parse(parts[1]) };
        }

        // Sort the hands based on score - ascending.
        Array.Sort(handsAndBids, (a, b) =>
        {
            int rank1 = GetHandRank(a, out Dictionary<char, int> matches1, false);
            int rank2 = GetHandRank(b, out Dictionary<char, int> matches2, false);

            if (rank1 == rank2)
            {
                for (int i = 0; i < a.Hand.Length; ++i)
                {
                    int value1 = pointLookup[a.Hand[i]];
                    int value2 = pointLookup[b.Hand[i]];
                    if (value1 != value2)
                        return value1.CompareTo(value2);
                }
            }

            return rank1.CompareTo(rank2);
        });

        long totalScore = 0;
        for (int i = 0; i < handsAndBids.Length; ++i)
        {
            totalScore += handsAndBids[i].Bid * (i + 1);
        }

        Console.WriteLine($"Answer: {totalScore}");

        // Part 2

        Dictionary<char, int> pointLookupWithJoker = new()
        {
            { 'J', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'T', 10 },
            { 'Q', 11 },
            { 'K', 12 },
            { 'A', 13 },
        };

        // Sort the hands based on score - ascending.
        Array.Sort(handsAndBids, (a, b) =>
        {
            int rank1 = GetHandRank(a, out Dictionary<char, int> matches1, true);
            int rank2 = GetHandRank(b, out Dictionary<char, int> matches2, true);

            if (rank1 == rank2)
            {
                for (int i = 0; i < a.Hand.Length; ++i)
                {
                    int value1 = pointLookupWithJoker[a.Hand[i]];
                    int value2 = pointLookupWithJoker[b.Hand[i]];
                    if (value1 != value2)
                        return value1.CompareTo(value2);
                }
            }

            return rank1.CompareTo(rank2);
        });

        totalScore = 0;
        for (int i = 0; i < handsAndBids.Length; ++i)
        {
            totalScore += handsAndBids[i].Bid * (i + 1);
        }

        Console.WriteLine($"Answer: {totalScore}");
    }
}
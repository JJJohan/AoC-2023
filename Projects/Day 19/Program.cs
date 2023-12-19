using System.Diagnostics;

static class Program
{
    private class Rule
    {
        public string DefaultNext;
        public List<(byte, int, char, string)> Rules;

        public Rule()
        {
            Rules = new();
            DefaultNext = string.Empty;
        }
    }

    private struct Input
    {
        public int[] Values;
    }

    private static byte CharToIndex(char c) => c switch
    {
        'x' => 0,
        'm' => 1,
        'a' => 2,
        's' => 3,
        _ => throw new InvalidOperationException()
    };

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day19.txt"));

        Dictionary<string, Rule> rules = new();

        // Parse rules
        int lineIndex = 0;
        while (lineIndex < lines.Length)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrEmpty(line))
            {
                ++lineIndex;
                break;
            }

            int bracketIndex = line.IndexOf('{');
            Debug.Assert(bracketIndex != -1);

            string ruleName = line[..bracketIndex];

            Rule rule = new() { Rules = new() };
            string rest = line.Substring(bracketIndex + 1, line.Length - bracketIndex - 2);
            string[] ruleParts = rest.Split(',');
            foreach (string rulePart in ruleParts)
            {
                string[] bits = rulePart.Split(':');
                if (bits.Length == 1)
                {
                    Debug.Assert(rule.DefaultNext == string.Empty);
                    rule.DefaultNext = bits[0];
                }
                else
                {
                    Debug.Assert(bits.Length == 2);
                    char letter = bits[0][0];
                    char condition = bits[0][1];
                    int value = int.Parse(bits[0].Substring(2));
                    rule.Rules.Add((CharToIndex(letter), value, condition, bits[1]));
                }
            }

            rules.Add(ruleName, rule);
            ++lineIndex;
        }

        // Parse inputs
        Input[] inputs = new Input[lines.Length - lineIndex];
        int inputIndex = 0;
        while (lineIndex < lines.Length)
        {
            string line = lines[lineIndex];
            Debug.Assert(line.StartsWith('{') && line.EndsWith('}'));
            line = line.Substring(1, line.Length - 2);
            string[] parts = line.Split(',');
            Debug.Assert(parts.Length == 4);
            inputs[inputIndex].Values = new int[4];
            for (int i = 0; i < parts.Length; ++i)
            {
                string[] varAndValue = parts[i].Split('=');
                Debug.Assert(varAndValue.Length == 2);
                inputs[inputIndex].Values[i] = int.Parse(varAndValue[1]);
            }
            ++inputIndex;
            ++lineIndex;
        }

        HashSet<Rule> visitedRules = new();
        List<Input> accepted = new();
        for (int i = 0; i < inputs.Length; ++i)
        {
            ref Input input = ref inputs[i];
            string ruleName = "in";
            while (ruleName != "R" && ruleName != "A")
            {
                Rule rule = rules[ruleName];
                ruleName = string.Empty;

                for (int j = 0; j < rule.Rules.Count; ++j)
                {
                    (byte index, int value, char condition, string next) = rule.Rules[j];

                    visitedRules.Add(rule);
                    if (condition == '<' && input.Values[index] < value)
                    {
                        ruleName = next;
                        break;
                    }
                    else if (condition == '>' && input.Values[index] > value)
                    {
                        ruleName = next;
                        break;
                    }
                }

                if (ruleName == string.Empty)
                    ruleName = rule.DefaultNext;
            }

            if (ruleName == "A")
                accepted.Add(input);
        }

        long sumOfValues = 0;
        for (int i = 0; i < accepted.Count; ++i)
        {
            foreach (int val in accepted[i].Values)
            {
                sumOfValues += val;
            }
        }
        Console.WriteLine($"Answer: {sumOfValues}");

        // Part 2

        (int min, int max)[] combinations = new[]
        {
            (1, 4000),
            (1, 4000),
            (1, 4000),
            (1, 4000)
        };

        long totalSum = GetCombinations(rules, combinations, "in");

        Console.WriteLine($"Max combinations: {totalSum}");
    }

    private static long GetCombinations(Dictionary<string, Rule> rules, (int min, int max)[] combinations, string currentRule)
    {
        for (int i = 0; i < combinations.Length; ++i)
            if (combinations[i].max < combinations[i].min) return 0;

        if (currentRule == "R")
        {
            return 0;
        }
        else if (currentRule == "A")
        {
            long sum = combinations[0].max - combinations[0].min + 1;
            for (int i = 1; i < combinations.Length; ++i)
            {
                sum *= (combinations[i].max - combinations[i].min + 1);
            }
            return sum;
        }
        else
        {
            Rule rule = rules[currentRule];
            long sum = 0;

            foreach ((byte index, int value, char op, string next) in rule.Rules)
            {
                if (op == '<')
                {
                    (int min, int max)[] newCombos = combinations.ToArray();
                    newCombos[index].max = value - 1;
                    sum += GetCombinations(rules, newCombos, next);
                    combinations[index].min = value;
                }
                else if (op == '>')
                {
                    (int min, int max)[] newCombos = combinations.ToArray();
                    newCombos[index].min = value + 1;
                    sum += GetCombinations(rules, newCombos, next);
                    combinations[index].max = value;
                }
            }

            return sum += GetCombinations(rules, combinations, rule.DefaultNext);
        }
    }
}
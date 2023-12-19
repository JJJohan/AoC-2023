using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

static class Program
{
    private class Rule
    {
        public string DefaultNext;
        public List<(char, int, char, string)> Rules;

        public Rule()
        {
            Rules = new();
            DefaultNext = string.Empty;
        }
    }

    private struct Input
    {
        public Dictionary<char, int> Values;
    }

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
                    rule.Rules.Add((letter, value, condition, bits[1]));
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
            inputs[inputIndex].Values = new();
            for (int i = 0; i < parts.Length; ++i)
            {
                string[] varAndValue = parts[i].Split('=');
                Debug.Assert(varAndValue.Length == 2);
                inputs[inputIndex].Values[varAndValue[0][0]] = int.Parse(varAndValue[1]);
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
                    (char letter, int value, char condition, string next) = rule.Rules[j];
                    if (input.Values.TryGetValue(letter, out int val))
                    {
                        visitedRules.Add(rule);
                        if (condition == '<' && val < value)
                        {
                            ruleName = next;
                            break;
                        }
                        else if (condition == '>' && val > value)
                        {
                            ruleName = next;
                            break;
                        }
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
            foreach (int val in accepted[i].Values.Values)
            {
                sumOfValues += val;
            }
        }
        Console.WriteLine($"Answer: {sumOfValues}");

        // Part 2

        long[] passes = new long[4];
        for (int i = 0; i < 4000; ++i)
        {
            passes[i] += GetCombinations(rules, ('x', i), "in");
            passes[i] += GetCombinations(rules, ('m', i), "in");
            passes[i] += GetCombinations(rules, ('a', i), "in");
            passes[i] += GetCombinations(rules, ('s', i), "in");
        }   

        long totalSum = passes[0] * passes[1] * passes[2] * passes[3];
        Console.WriteLine($"Max combinations: {totalSum}");
    }

    private static long GetCombinations(Dictionary<string, Rule> rules, (char c, long v) combination, string currentRule)
    {
        if (currentRule == "R")
        {
            return 0;
        }

        if (currentRule == "A")
        {
            return 1;
        }

        Rule rule = rules[currentRule];

        long sum = 0;
        foreach ((char c, int value, char op, string next) in rule.Rules)
        {
            if (combination.c == c)
            {
                if (op == '>' && combination.v > value)
                {
                    sum += GetCombinations(rules, combination, next);
                }
                else if (op == '<' && combination.v < value)
                {
                    sum += GetCombinations(rules, combination, next);
                }
            }
        }

        return sum + GetCombinations(rules, combination, rule.DefaultNext);
    }
}
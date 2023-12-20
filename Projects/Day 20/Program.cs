using System.Collections.Generic;
using System.Diagnostics;

static class Program
{
    private enum Type
    {
        Broadcaster,
        Conjunction,
        FlipFlop
    }

    private class Module
    {
        public string? Name;
        public Type Type;
        public bool OnOrHigh;
        public Dictionary<string, bool>? InputPulses;
        public (int, bool)[] InputPulsesArray;
        public int InputsReceived;
        public int TotalInputs;
        public int Index;
        public string[] Destinations = Array.Empty<string>();
        public int[] DestinationIndices = Array.Empty<int>();
    }

    public static void Main()
    {
        string[] lines = File.ReadAllLines(Path.Combine("..", "Input", "day20.txt"));

        Dictionary<string, Module> modules = new();
        Dictionary<string, Module> modulesClone = new();
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            string[] split = line.Split("->", StringSplitOptions.TrimEntries);
            Debug.Assert(split.Length == 2);
            string[] dests = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            string name = split[0];
            Type type;
            if (name[0] == '%')
            {
                type = Type.FlipFlop;
                name = name[1..];
            }
            else if (name[0] == '&')
            {
                type = Type.Conjunction;
                name = name[1..];
            }
            else
            {
                type = Type.Broadcaster;
            }

            modules.Add(name, new()
            {
                Name = name,
                Type = type,
                Destinations = dests,
                InputPulses = type == Type.Conjunction ? new() : null
            });

            // For part 2
            modulesClone.Add(name, new()
            {
                Name = name,
                Type = type,
                Destinations = dests,
                InputPulses = type == Type.Conjunction ? new() : null
            });
        }

        // Connect all conjunctions
        Module[] moduleArray1 = modules.Values.ToArray();
        for (int i = 0; i < moduleArray1.Length; i++)
            moduleArray1[i].Index = i;

        foreach (Module module in moduleArray1)
        {
            module.DestinationIndices = new int[module.Destinations.Length];
            int idx = 0;
            foreach (string dest in module.Destinations)
            {
                if (modules.TryGetValue(dest, out Module? m))
                {
                    module.DestinationIndices[idx++] = m.Index;
                    if (m.Type == Type.Conjunction)
                    {
                        Debug.Assert(m.InputPulses != null && module.Name != null);
                        m.InputPulses[module.Name] = false;
                        ++m.TotalInputs;
                    }
                }
                else
                {
                    module.DestinationIndices[idx++] = -1;
                }
            }
        }

        foreach (Module module in moduleArray1)
        {
            if (module.Type == Type.Conjunction)
            {
                module.InputPulsesArray = new (int, bool)[module.InputPulses.Count];
                int idx = 0;
                foreach (string name in module.InputPulses.Keys)
                {
                    module.InputPulsesArray[idx++] = (modules[name].Index, false);
                }
            }
        }

        int broadcasterIndex = modules["broadcaster"].Index;
        long lowPulseCount = 0;
        long highPulseCount = 0;

        Queue<(int, int, bool)> moduleQueue = new();
        for (int i = 0; i < 1000; ++i)
        {
            moduleQueue.Enqueue((-1, broadcasterIndex, false));

            while (moduleQueue.Count > 0)
            {
                (int last, int index, bool highPulse) = moduleQueue.Dequeue();

                if (highPulse)
                    ++highPulseCount;
                else
                    ++lowPulseCount;

                if (index == -1) 
                    continue;

                Module module = moduleArray1[index];

                switch (module.Type)
                {
                    case Type.FlipFlop:
                        {
                            bool prev = module.OnOrHigh;

                            if (!highPulse)
                                module.OnOrHigh = !module.OnOrHigh;

                            if (prev != module.OnOrHigh)
                            {
                                foreach (int destIdx in module.DestinationIndices)
                                {
                                    moduleQueue.Enqueue((index, destIdx, module.OnOrHigh));
                                }
                            }
                            break;
                        }
                    case Type.Conjunction:
                        {
                            for (int j = 0; j < module.InputPulsesArray.Length; ++j)
                            {
                                if (module.InputPulsesArray[j].Item1 == last)
                                {
                                    module.InputPulsesArray[j].Item2 = highPulse;
                                    break;
                                }
                            }

                            bool allHigh = true;
                            for (int j = 0; j < module.InputPulsesArray.Length; ++j)
                            {
                                if (!module.InputPulsesArray[j].Item2)
                                {
                                    allHigh = false;
                                    break;
                                }
                            }

                            foreach (int destIdx in module.DestinationIndices)
                            {
                                moduleQueue.Enqueue((index, destIdx, !allHigh));
                            }

                            break;
                        }
                    default: // Broadcaster
                        {
                            foreach (int destIdx in module.DestinationIndices)
                            {
                                moduleQueue.Enqueue((index, destIdx, highPulse));
                            }
                            break;
                        }
                }
            }
        }

        long multiplied = lowPulseCount * highPulseCount;
        Console.WriteLine($"Multiplied low and high pulse count: {multiplied}");

        // Part 2

        // Connect all conjunctions
        Module[] moduleArray = modulesClone.Values.ToArray();
        for (int i = 0; i < moduleArray.Length; i++)
            moduleArray[i].Index = i;

        foreach (Module module in moduleArray)
        {
            module.DestinationIndices = new int[module.Destinations.Length];
            int idx = 0;
            foreach (string dest in module.Destinations)
            {
                if (modulesClone.TryGetValue(dest, out Module? m))
                {
                    module.DestinationIndices[idx++] = m.Index;
                    if (m.Type == Type.Conjunction)
                    {
                        Debug.Assert(m.InputPulses != null && module.Name != null);
                        m.InputPulses[module.Name] = false;
                        ++m.TotalInputs;
                    }
                }
                else
                {
                    module.DestinationIndices[idx++] = -1;
                }
            }
        }

        foreach (Module module in moduleArray)
        {
            if (module.Type == Type.Conjunction)
            {
                module.InputPulsesArray = new (int, bool)[module.InputPulses.Count];
                int idx = 0;
                foreach (string name in module.InputPulses.Keys)
                {
                    module.InputPulsesArray[idx++] = (modulesClone[name].Index, false);
                }
            }
        }

        long buttonPresses = 0;
        bool rxHit = false;
        broadcasterIndex = modulesClone["broadcaster"].Index;
        Queue<(int last, int index, bool highPulse)> moduleCloneQueue = new();
        while (!rxHit)
        {
            moduleCloneQueue.Enqueue((-1, broadcasterIndex, false));
            ++buttonPresses;

            while (moduleCloneQueue.Count > 0)
            {
                (int last, int index, bool highPulse) = moduleCloneQueue.Dequeue();

                if (index == -1)
                {
                    if (!highPulse)
                    {
                        rxHit = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                Module module = moduleArray[index];

                switch (module.Type)
                {
                    case Type.FlipFlop:
                        {
                            bool prev = module.OnOrHigh;

                            if (!highPulse)
                                module.OnOrHigh = !module.OnOrHigh;

                            if (prev != module.OnOrHigh)
                            {
                                foreach (int destIdx in module.DestinationIndices)
                                {
                                    moduleCloneQueue.Enqueue((index, destIdx, module.OnOrHigh));
                                }
                            }
                            break;
                        }
                    case Type.Conjunction:
                        {
                            for (int i = 0; i < module.InputPulsesArray.Length; ++i)
                            {
                                if (module.InputPulsesArray[i].Item1 == last)
                                {
                                    module.InputPulsesArray[i].Item2 = highPulse;
                                    break;
                                }
                            }

                            bool allHigh = true;
                            for (int i = 0; i < module.InputPulsesArray.Length; ++i)
                            {
                                if (!module.InputPulsesArray[i].Item2)
                                {
                                    allHigh = false;
                                    break;
                                }
                            }

                            foreach (int destIdx in module.DestinationIndices)
                            {
                                moduleCloneQueue.Enqueue((index, destIdx, !allHigh));
                            }

                            break;
                        }
                    default: // Broadcaster
                        {
                            foreach (int destIdx in module.DestinationIndices)
                            {
                                moduleCloneQueue.Enqueue((index, destIdx, highPulse));
                            }
                            break;
                        }
                }
            }
        }

        Console.WriteLine($"Minimum button presses for low pulse to rx: {buttonPresses}");
    }
}
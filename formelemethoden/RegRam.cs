using System;
using System.Collections.Generic;
using System.Linq;
using formelemethoden;

namespace formelemethoden
{
    class RegGram
    {
        private readonly List<string> _lines = new List<string>();

        public RegGram(List<string> lines)
        {
            _lines = lines;
            CreateAutomata();
        }

        public RegGram(string[] lines)
        {
            _lines = lines.ToList();
            CreateAutomata();
        }

        public RegGram(SortedSet<Transition<string>> transitions, SortedSet<string> finalStates)
        {
            CreateGrammar(transitions, finalStates);
        }

        public Automata<string> CreateAutomata()
        {
            char finalState = _lines.Last().First();
            finalState++;
            string finalStateString = finalState.ToString();

            Automata<string> m = new Automata<string>();
            m.DefineAsStartState(_lines[0].First().ToString());

            List<char> symbols = new List<char>();

            foreach (string line in _lines)
            {
                string[] substrings = line.Split(' ');

                string fromState = substrings[0];

                for (int i = 2; i < substrings.Length; i += 2)
                {
                    string substring = substrings[i];
                    char symbol = substring.First();
                    if (symbol != '$' && !symbols.Contains(symbol)) symbols.Add(symbol);
                    if (substring.Length > 1)
                    {
                        string toState = substring[1].ToString();
                        if (substring.Last() == '*')
                        {
                            m.DefineAsFinalState(toState);
                        }
                        m.AddTransition(new Transition<string>(fromState, symbol, toState));
                    }
                    else
                    {
                        m.AddTransition(new Transition<string>(fromState, symbol, finalStateString));
                        if (!m.getEndStates.Contains(finalStateString)) m.DefineAsFinalState(finalStateString);
                    }
                }
            }

            m.SetAlphabet(symbols.ToArray());

            return m;
        }

        public void CreateGrammar(SortedSet<Transition<string>> transitions, SortedSet<string> finalStates)
        {
            foreach (Transition<string> transition in transitions)
            {
                bool inLines = false;
                int lineIndex = 0;
                for (var i = 0; i < _lines.Count; i++)
                {
                    string line = _lines[i];
                    if (line.StartsWith(transition.fromState))
                    {
                        inLines = true;
                        lineIndex = i;
                        break;
                    }
                }

                if (inLines)
                {
                    if (!finalStates.Contains(transition.toState) || transition.symbol == '$')
                    {
                        _lines[lineIndex] = string.Concat(new[]
                            {_lines[lineIndex], " | " + transition.symbol + transition.toState});
                    }
                    else
                    {
                        _lines[lineIndex] = string.Concat(new[]
                        {
                            _lines[lineIndex],
                            " | " + transition.symbol + transition.toState + " | " + transition.symbol
                        });
                    }
                }
                else
                {
                    if (!finalStates.Contains(transition.fromState))
                    {
                        if (!finalStates.Contains(transition.toState) || transition.symbol == '$')
                        {
                            _lines.Add(transition.fromState + " -> " + transition.symbol + transition.toState);
                        }
                        else
                        {
                            _lines.Add(transition.fromState + " -> " + transition.symbol + transition.toState + " | " +
                                       transition.symbol);
                        }
                    }
                    else
                    {
                        if (!finalStates.Contains(transition.toState) || transition.symbol == '$')
                        {
                            _lines.Add(transition.fromState + "* -> " + transition.symbol + transition.toState);
                        }
                        else
                        {
                            _lines.Add(transition.fromState + "* -> " + transition.symbol + transition.toState + " | " +
                                       transition.symbol);
                        }
                    }
                }
            }
        }

        public void PrintTransitions(SortedSet<Transition<string>> transitions)
        {
            foreach (Transition<string> transition in transitions)
            {
                Console.WriteLine(transition);
            }
        }

        public void Print()
        {
            foreach (string line in _lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
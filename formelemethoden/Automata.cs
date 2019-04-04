using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace formelemethoden
{
    public class Automata<T> where T : IComparable<T>
    {
        private SortedSet<Transition<T>> transitions;
        private SortedSet<T> states;
        private SortedSet<T> startStates;
        private SortedSet<T> endStates;
        private SortedSet<char> symbols;

        public SortedSet<Transition<T>> getTransitions
        {
            get { return transitions; }
        }

        public SortedSet<T> getEndStates
        {
            get { return endStates; }
        }

        public SortedSet<char> getAlphaBet
        {
            get { return symbols; }
        }

        public Automata() : this(new SortedSet<char>())
        {
        }

        public Automata(char[] s) : this(new SortedSet<char>(s))
        {
        }

        public Automata(SortedSet<char> symbols)
        {
            this.transitions = new SortedSet<Transition<T>>(new TransitionComparator<T>());
            this.states = new SortedSet<T>();
            this.endStates = new SortedSet<T>();
            this.startStates = new SortedSet<T>();
            this.symbols = symbols;
        }

        public void SetAlphabet(char[] s)
        {
            this.SetAlphabet(new SortedSet<char>(s));
        }

        public void SetAlphabet(SortedSet<char> symbols)
        {
            this.symbols = symbols;
        }

        public void AddTransition(Transition<T> t)
        {
            this.transitions.Add(t);
            states.Add(t.GetfromState());
            states.Add(t.GetToState());
        }

        public void DefineAsStartState(T t)
        {
            states.Add(t);
            startStates.Add(t);
        }

        public void DefineAsFinalState(T t)
        {
            states.Add(t);
            endStates.Add(t);
        }

        public void PrintTransitions()
        {
            foreach (Transition<T> t in transitions)
            {
                Console.WriteLine(t.ToString());
            }
        }

        public bool IsDFA()
        {
            bool isDFA = true;

            foreach (T from in states)
            {
                foreach (char symbol in symbols)
                {
                    isDFA = isDFA && GetToStates(from, symbol).Count == 1;
                }
            }

            return isDFA;
        }

        public List<T> GetToStates(T from, char symbol)
        {
            List<T> toStates = new List<T>();

            foreach (Transition<T> t in transitions)
            {
                if (t.fromState.Equals(from))
                {
                    if (t.symbol == symbol)
                    {
                        toStates.Add(t.toState);
                    }
                }
            }

            return toStates;
        }

        public bool Accept(string s)
        {
            List<bool> istrue = new List<bool>();

            foreach (T state in startStates)
            {
                istrue.Add(Accept(s, state));
            }

            return istrue.Contains(true);
        }

        public bool Accept(string s, T fromState, int index = 0)
        {
            List<bool> isTrue = new List<bool>();

            foreach (Transition<T> t in transitions)
            {
                if (t.fromState.Equals(fromState) && s[index] == t.symbol)
                {
                    if (index == s.Length - 1)
                    {
                        if (!endStates.Contains(t.toState))
                            return false;
                        return true;
                    }

                    isTrue.Add(Accept(s, t.toState, index + 1));
                }
            }

            return isTrue.Contains(true);
        }

        public SortedSet<string> ReturnLanguageTillLength(int length)
        {
            SortedSet<string> language = new SortedSet<string>();

            foreach (T state in startStates)
            {
                LanguageSearch(language, "", state, length);
            }

            return language;
        }

        public void LanguageSearch(SortedSet<string> language, string s, T fromState, int maxLength, int index = 0)
        {
            foreach (Transition<T> t in transitions)
            {
                if (fromState.Equals(t.fromState))
                {
                    if (index + 1 <= maxLength)
                    {
                        if (endStates.Contains(t.toState))
                        {
                            language.Add("" + s + t.symbol);
                        }
                        LanguageSearch(language, s + t.symbol, t.toState, maxLength, index + 1);
                    }
                }
            }
        }

        public void CreateGraph(string filename)
        {
            string filePath = $@"..\..\graphs\{filename}";
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filePath + ".dot"))
            {
                file.WriteLine("digraph {");
                file.WriteLine(@" """" [shape=none]");

                foreach (var endState in endStates)
                {
                    file.WriteLine($@" ""{endState}""  [shape=doublecircle]");
                }
                file.WriteLine();
                foreach (var startState in startStates)
                {
                    file.WriteLine($@""""" -> ""{startState}""");
                }

                foreach (Transition<T> t in transitions)
                {
                    file.WriteLine($@"""{t.fromState.ToString()}"" -> ""{t.toState.ToString()}""[label=""{t.symbol}"", weight=""{t.symbol}""];");
                }

                file.WriteLine("}");
            }

            FileDotEngine.Run(filePath);

        }

        public List<T> EClosure(T startState)
        {
            List<T> eclosure = new List<T>();

            foreach (var transition in transitions)
            {
                if (transition.fromState.Equals(startState))
                {
                    if (transition.symbol == Transition<string>.EPSILON)
                    {
                        if (IsEndState(transition.toState))
                        {
                            eclosure.Add(transition.toState);
                        }
                        else
                        {
                            eclosure.Add(transition.fromState);
                            eclosure = eclosure.Concat(EClosure(transition.toState)).ToList();
                        }
                    }

                    if (transition.symbol != Transition<string>.EPSILON)
                    {
                        eclosure.Add(transition.fromState);
                    }
                }
            }

            return eclosure.Distinct().ToList();
        }

        public bool IsEndState(T state)
        {
            foreach (var transition in transitions)
            {
                if (transition.fromState.Equals(state))
                {
                    return false;
                }
            }

            return true;
        }

        public List<T> Delta(List<T> searchStates, char symbol)
        {
            List<T> eclosureState = new List<T>();
            foreach (var transition in transitions)
            {
                if (searchStates.Contains(transition.fromState) && transition.symbol == symbol)
                {
                    eclosureState.Add(transition.toState);
                    eclosureState = eclosureState.Concat(EClosure(transition.toState)).Distinct().ToList();
                }
            }

            return eclosureState.Distinct().ToList();
        }

        public Automata<T> ToDFA()
        {
            if (this.IsDFA())
            {
                return (Automata<T>)this.MemberwiseClone();
            }

            Automata<T> automata = new Automata<T>(this.symbols);
            automata.states.Add(GetValue<T>(""));

            foreach (var startState in startStates)
            {
                List<T> eclosure = EClosure(startState);
                string eclosureString = string.Join(",", eclosure);
                if (IsEndState(eclosure))
                {
                    automata.endStates.Add(GetValue<T>(eclosureString));
                }

                automata.DefineAsStartState(GetValue<T>(eclosureString));
                foreach (var symbol in symbols)
                {
                    List<T> delta = Delta(eclosure, symbol);
                    string deltaString = string.Join(",", delta);

                    if (IsEndState(delta))
                    {
                        automata.endStates.Add(GetValue<T>(deltaString));
                    }

                    AddCheckedTransition(automata, eclosureString, deltaString, symbol);

                    automata = ToDfaHelper(delta, automata);
                }
            }
            return automata;
        }

        private void AddCheckedTransition(Automata<T> automata, string from, string to, char symbol)
        {
            if (from.Length > 0 && to.Length == 0)
            {
                automata.AddTransition(new Transition<T>(GetValue<T>(from), symbol, GetValue<T>("FUIK")));
            }
            else if(from.Length == 0 && to.Length == 0)
            {
                return;
            }
            else
            { 
                automata.AddTransition(new Transition<T>(GetValue<T>(from), symbol, GetValue<T>(to)));
            }

        }

        public T GetValue<T>(String value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public Automata<T> ToDfaHelper(List<T> delta, Automata<T> automata)
        {
            foreach (var symbol in symbols)
            {
                var delta2 = Delta(delta, symbol);
                string deltaString = string.Join(",", delta);
                string delta2String = string.Join(",", delta2);

                if (IsEndState(delta2))
                {
                    automata.endStates.Add(GetValue<T>(delta2String));
                }

                if (IsEndState(delta))
                {
                    automata.endStates.Add(GetValue<T>(deltaString));
                }

                if (CheckForDuplicateStates(automata, delta2String))
                {
                    AddCheckedTransition(automata, deltaString, delta2String, symbol);
                }
                else
                {
                    AddCheckedTransition(automata, deltaString, delta2String, symbol);
                    ToDfaHelper(delta2, automata);
                }
            }
            return automata;
        }

        public bool IsEndState(List<T> delta)
        {
            foreach (var endState in endStates)
            {
                if (delta.Contains(endState))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckForDuplicateStates(Automata<T> automata, string delta)
        {
            foreach (var state in automata.states)
            {
                if (state.Equals(delta))
                {
                    return true;
                }
            }

            return false;
        }

        public Automata<T> Reverse()
        {
            SortedSet<T> temp = startStates;
            Automata<T> m = (Automata<T>)this.MemberwiseClone();

            m.startStates = m.endStates;
            m.endStates = temp;


            var tempTransitions = m.transitions.Select(transition =>
            {
                return new Transition<T>(transition.toState, transition.symbol, transition.fromState);
            }).ToList();

            m.transitions = new SortedSet<Transition<T>>(from x in tempTransitions select x, new TransitionComparator<T>());

            return m;
        }

        public Automata<T> Minimalise()
        {
             return ((Automata<T>)this.MemberwiseClone()).Reverse().ToDFA().Reverse().ToDFA();
        }
    }
}
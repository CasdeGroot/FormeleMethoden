using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formelemethoden
{
    enum Operator { Plus, Star, Or, Dot, One }

    class RegExp
    {
        public Operator Operator { get; set; }
        public string Terminals { get; set; }
        public static char Counter ='A';

        private RegExp left;
        private RegExp right;

        public RegExp()
        {
            Operator = Operator.One;
            Terminals = "";
            left = null;
            right = null;
        }

        public RegExp(string p)
        {
            Operator = Operator.One;
            Terminals = p;
            left = null;
            right = null;
        }

        public RegExp Plus()
        {
            return new RegExp
            {
                Operator = Operator.Plus,
                left = this
            };
        }

        public RegExp Star()
        {
            return new RegExp
            {
                Operator = Operator.Star,
                left = this
            };
        }

        public RegExp Or(RegExp e2)
        {
            return new RegExp
            {
                Operator = Operator.Or,
                left = this,
                right = e2
            };
        }

        public RegExp Dot(RegExp e2)
        {
            return new RegExp
            {
                Operator = Operator.Dot,
                left = this,
                right = e2
            };
        }

        public SortedSet<string> GetLanguage(int maxSteps)
        {
            SortedSet<string> emptyLanguage = new SortedSet<string>(new RegExpComparator());
            SortedSet<string> languageResult = new SortedSet<string>(new RegExpComparator());

            SortedSet<string> languageLeft, languageRight;

            if (maxSteps < 1) return emptyLanguage;

            switch (Operator)
            {
                case Operator.One:
                    languageResult.Add(Terminals);
                    break;

                case Operator.Star:
                case Operator.Plus:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguage(maxSteps - 1);
                    languageResult.UnionWith(languageLeft);
                    for (int i = 1; i < maxSteps; i++)
                    {
                        HashSet<String> languageTemp = new HashSet<String>(languageResult);
                        foreach (String s1 in languageLeft)
                        {
                            foreach (String s2 in languageTemp)
                            {
                                languageResult.Add(s1 + s2);
                            }
                        }
                    }
                    if (Operator == Operator.Star)
                    { languageResult.Add(""); }
                    break;

                case Operator.Or:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguage(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.GetLanguage(maxSteps - 1);
                    languageResult.UnionWith(languageLeft);
                    languageResult.UnionWith(languageRight);
                    break;

                case Operator.Dot:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguage(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.GetLanguage(maxSteps - 1);
                    foreach (string s1 in languageLeft)
                    {
                        foreach (string s2 in languageRight)
                        {
                            languageResult.Add(s1 + s2);
                        }
                    }
                    break;

                default:
                    Console.WriteLine("getLanguage is nog niet gedefinieerd voor de operator: " + Operator);
                    break;
            }
            return languageResult;
        }

        public static void PrintLanguage(SortedSet<string> language)
        {
            foreach (string s in language)
            {
                Console.WriteLine(s);
            }
        }

        public Automata<string> ToAutomata()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<string> automata = new Automata<string>(alphabet);
            automata.DefineAsStartState("S");
            automata.DefineAsFinalState("F");

            var left = this.GetTransitions("S", "F");
            foreach (var transition in left)
            {
                automata.AddTransition(transition);
            }

            Counter = 'A';
            return automata;
        }

        private string getStateNumber()
        {
            if (Counter == 'S' || Counter == 'F')
            {
                Counter++;
            }

            return Counter++.ToString();
        }

        public List<Transition<string>> GetTransitions(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();

            switch (this.Operator)
            {
                case Operator.Dot:
                    transitions = this.Dot(beginState, endState);
                    break;

                case Operator.One:
                    transitions = this.One(beginState, endState);
                    break;

                case Operator.Or:
                    transitions = this.Or(beginState, endState);
                    break;

                case Operator.Plus:
                    transitions = this.Plus(beginState, endState);
                    break;

                case Operator.Star:
                    transitions = this.Star(beginState, endState);
                    break;
            }

            return transitions;
        }

        public List<Transition<string>> Star(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();
            Transition<string> beginBox = new Transition<string>(beginState, '$', getStateNumber());
            Transition<string> beginEnd = new Transition<string>(beginState, '$', endState);
            Transition<string> boxBack = new Transition<string>(getStateNumber(), '$', beginBox.toState);
            Transition<string> boxEnd = new Transition<string>(boxBack.fromState, '$', endState);
            if (this.left != null)
            {
                var left = new ThompsonBox(this.left, beginBox.toState, boxBack.fromState);
                transitions = transitions.Concat(left.GetTransitions()).ToList();
            }

            if (this.right != null)
            {
               var right = new ThompsonBox(this.right, beginBox.toState, boxBack.fromState);
                transitions = transitions.Concat(right.GetTransitions()).ToList();
            }

            transitions.Add(beginBox);
            transitions.Add(beginEnd);
            transitions.Add(boxBack);
            transitions.Add(boxEnd);

            return transitions;
        }

        public List<Transition<string>> Dot(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();
            Transition<string> link = new Transition<string>(getStateNumber(), '$', getStateNumber());
            if (this.left != null)
            {
                var left = new ThompsonBox(this.left, beginState, link.fromState);
                transitions = transitions.Concat(left.GetTransitions()).ToList();
            }

            if (this.right != null)
            {
                var right = new ThompsonBox(this.right, link.toState, endState);
                transitions = transitions.Concat(right.GetTransitions()).ToList();
            }

            transitions.Add(link);

            return transitions;
        }

        public List<Transition<string>> Plus(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();
            Transition<string> plusBeginBox = new Transition<string>(beginState, '$', getStateNumber());
            Transition<string> plusBoxEnd = new Transition<string>(getStateNumber(), '$', endState);
            Transition<string> plusBeginEnd = new Transition<string>(beginState, '$', endState);

            if (this.left != null)
            {
                var left = new ThompsonBox(this.left, plusBeginBox.toState, plusBoxEnd.fromState);
                transitions = transitions.Concat(left.GetTransitions()).ToList();
            }

            transitions.Add(plusBeginBox);
            transitions.Add(plusBeginEnd);
            transitions.Add(plusBoxEnd);

            return transitions;
        }

        public List<Transition<string>> One(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();
            var tempState = "";
            if (Terminals.Length == 1)
            {
                transitions.Add(new Transition<string>(beginState, Terminals[0], endState));
            }
            else
            {
                for (int i = 0; i < Terminals.Length; i++)
                {
                    var terminal = Terminals[i];

                    if (i == Terminals.Length - 1)
                    {
                        transitions.Add(new Transition<string>(tempState, terminal, endState));

                    }
                    else if (i == 0)
                    {
                        tempState = getStateNumber();
                        transitions.Add(new Transition<string>(beginState, terminal, tempState));
                    }
                    else
                    {
                        var temp = getStateNumber();
                        transitions.Add(new Transition<string>(tempState, terminal, temp));
                        tempState = temp;
                    }
                }
            }

            return transitions;
        }

        public List<Transition<string>> Or(string beginState, string endState)
        {
            List<Transition<string>> transitions = new List<Transition<string>>();
            Transition<string> leftTo = new Transition<string>(beginState, '$', getStateNumber());
            Transition<string> leftFrom = new Transition<string>(getStateNumber(), '$', endState);
            Transition<string> rightTo = new Transition<string>(beginState, '$', getStateNumber());
            Transition<string> rightFrom = new Transition<string>(getStateNumber(), '$', endState);

            if (this.left != null)
            {
                var left = new ThompsonBox(this.left, leftTo.toState, leftFrom.fromState);
                transitions = transitions.Concat(left.GetTransitions()).ToList();
            }

            if (this.right != null)
            {
                var right = new ThompsonBox(this.right, rightTo.toState, rightFrom.fromState);
                transitions = transitions.Concat(right.GetTransitions()).ToList();
            }

            transitions.Add(leftTo);
            transitions.Add(leftFrom);
            transitions.Add(rightTo);
            transitions.Add(rightFrom);

            return transitions;
        }
    }
}

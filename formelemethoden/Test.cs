using System;
using System.Collections.Generic;
using System.Linq;

namespace formelemethoden
{

    public class Test
    {
        static RegExp expr1, expr2, expr3, expr4, expr5, a, b, all;

        public static Automata<string> GetExampleSlide8Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<string>("q0", 'a', "q1"));
            m.AddTransition(new Transition<string>("q0", 'b', "q4"));

            m.AddTransition(new Transition<string>("q1", 'a', "q4"));
            m.AddTransition(new Transition<string>("q1", 'b', "q2"));

            m.AddTransition(new Transition<string>("q2", 'a', "q3"));
            m.AddTransition(new Transition<string>("q2", 'b', "q4"));

            m.AddTransition(new Transition<string>("q3", 'a', "q1"));
            m.AddTransition(new Transition<string>("q3", 'b', "q2"));

            m.AddTransition(new Transition<string>("q4", 'a'));
            m.AddTransition(new Transition<string>("q4", 'b'));

            m.DefineAsStartState("q0");

            m.DefineAsFinalState("q2");
            m.DefineAsFinalState("q3");

            return m;
        }

        public static Automata<String> GetExampleSlide14Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<String>("A", 'a', "C"));
            m.AddTransition(new Transition<String>("A", 'b', "B"));
            m.AddTransition(new Transition<String>("A", 'b', "C"));

            m.AddTransition(new Transition<String>("B", 'b', "C"));
            m.AddTransition(new Transition<String>("B", "C"));

            m.AddTransition(new Transition<String>("C", 'a', "D"));
            m.AddTransition(new Transition<String>("C", 'a', "E"));
            m.AddTransition(new Transition<String>("C", 'b', "D"));

            m.AddTransition(new Transition<String>("D", 'a', "B"));
            m.AddTransition(new Transition<String>("D", 'a', "C"));

            m.AddTransition(new Transition<String>("E", 'a'));
            m.AddTransition(new Transition<String>("E", "D"));

            // only on start state in a dfa:
            m.DefineAsStartState("A");

            // two final states:
            m.DefineAsFinalState("C");
            m.DefineAsFinalState("E");

            return m;
        }

        public static void TestRegRam(Automata<string> m)
        {
            RegGram regGram = new RegGram(m.getTransitions, m.getEndStates);
            regGram.Print();
            Console.WriteLine();

            string[] lines = { "S -> aA", "A -> aS | b" };
            RegGram newgram = new RegGram(lines);
            Automata<string> m2 = newgram.CreateAutomata();
            newgram.PrintTransitions(m2.getTransitions);
        }

        public static void TestRegExp()
        {
            a = new RegExp("a");
            b = new RegExp("b");

            // expr1: "baa"
            expr1 = new RegExp("baa");
            // expr2: "bb"
            expr2 = new RegExp("bb");
            // expr3: "baa | baa"
            expr3 = expr1.Or(expr2);

            // all: "(a|b)*"
            all = (a.Or(b)).Star();

            // expr4: "(baa | baa)+"
            expr4 = expr3.Plus();
            // expr5: "(baa | bb)+ (a|b)*"
            expr5 = expr4.Dot(all);

            var autm = expr5.ToAutomata();
            autm.ToDFA().CreateGraph("4_toDFA");
            autm.Minimalise().CreateGraph("4_Minimalised2");
            autm.CreateGraph("4_Thompson3");
         }

        public static void TestLanguage()
        {
            TestRegExp();
            Console.WriteLine("taal van (baa):\n");
            RegExp.PrintLanguage(expr1.GetLanguage(5));
            Console.WriteLine("taal van (bb):\n");
            RegExp.PrintLanguage(expr2.GetLanguage(5));
            Console.WriteLine("taal van (baa | bb):\n" );
            RegExp.PrintLanguage(expr3.GetLanguage(5));
            Console.WriteLine("taal van (a|b)*:\n");
            RegExp.PrintLanguage(all.GetLanguage(5));
            Console.WriteLine("taal van (baa | bb)+:\n");
            RegExp.PrintLanguage(expr4.GetLanguage(5));
            Console.WriteLine("taal van (baa | bb)+ (a|b)*:\n");
            RegExp.PrintLanguage(expr5.GetLanguage(5));
        }

        public static void PrintLanguageTillLength(Automata<string> m, int length)
        {
            SortedSet<string> language = m.ReturnLanguageTillLength(length);
            foreach (string s in language)
            {
                Console.Write(s + " ");
            }
            Console.WriteLine();
        }

        public static Automata<string> NdfaToDFa()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<string>("0", '$', "1"));
            m.AddTransition(new Transition<string>("0", '$', "7"));
            m.AddTransition(new Transition<string>("1", '$', "2"));
            m.AddTransition(new Transition<string>("1", '$', "4"));
            m.AddTransition(new Transition<string>("2", 'a', "3"));
            m.AddTransition(new Transition<string>("4", 'b', "5"));
            m.AddTransition(new Transition<string>("3", '$', "6"));
            m.AddTransition(new Transition<string>("5", '$', "6"));
            m.AddTransition(new Transition<string>("6", '$', "1"));
            m.AddTransition(new Transition<string>("6", '$', "7"));
            m.AddTransition(new Transition<string>("7", 'a', "8"));
            m.AddTransition(new Transition<string>("8", 'b', "9"));
            m.AddTransition(new Transition<string>("9", 'b', "10"));

            m.DefineAsStartState("0");
            m.DefineAsFinalState("10");

            return m;
        }

        public static Automata<string> MinimalizationAutomata()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<string>("0", 'a', "0"));
            m.AddTransition(new Transition<string>("0", 'b', "1"));
            m.AddTransition(new Transition<string>("1", 'a', "2"));
            m.AddTransition(new Transition<string>("1", 'b', "1"));
            m.AddTransition(new Transition<string>("2", 'a', "0"));
            m.AddTransition(new Transition<string>("2", 'b', "3"));
            m.AddTransition(new Transition<string>("3", 'a', "4"));
            m.AddTransition(new Transition<string>("3", 'b', "1"));
            m.AddTransition(new Transition<string>("4", 'b', "3"));
            m.AddTransition(new Transition<string>("4", 'a', "5"));
            m.AddTransition(new Transition<string>("5", 'a', "0"));
            m.AddTransition(new Transition<string>("5", 'b', "3"));

            m.DefineAsStartState("0");
            m.DefineAsFinalState("4");
            m.DefineAsFinalState("2");

            return m;
        }

        public static Automata<string> NDfaToDfa2()
        {
            char[] alphabet = { '0', '1' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<string>("a", '0', "a"));
            m.AddTransition(new Transition<string>("a", '0', "d"));
            m.AddTransition(new Transition<string>("a", '1', "d"));
            m.AddTransition(new Transition<string>("a", '0', "e"));
            m.AddTransition(new Transition<string>("a", '1', "e"));
            m.AddTransition(new Transition<string>("d", '0', "e"));
            m.AddTransition(new Transition<string>("a", '0', "b"));
            m.AddTransition(new Transition<string>("a", '0', "c"));
            m.AddTransition(new Transition<string>("c", '1', "b"));
            m.AddTransition(new Transition<string>("b", '0', "c"));
            m.AddTransition(new Transition<string>("b", '1', "e"));

            m.DefineAsFinalState("e");
            m.DefineAsStartState("a");

            return m;
        }

        public static Automata<string> TestAutomata2()
        {
            char[] alphabet = { '0', '1' };
            Automata<string> m = new Automata<string>(alphabet);

            m.AddTransition(new Transition<string>("q0", '0', "q0"));
            m.AddTransition(new Transition<string>("q0", '1', "q1"));
            m.AddTransition(new Transition<string>("q1", '0', "q0"));
            m.AddTransition(new Transition<string>("q1", '1', "q1"));

            m.DefineAsFinalState("q0");
            m.DefineAsFinalState("q1");
            m.DefineAsStartState("q0");

            return m;
        }
    }
}
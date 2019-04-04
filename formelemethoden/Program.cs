using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formelemethoden
{
    class Program
    {
        static void Main(string[] args)
        {
            Automata<string> m1 = Test.GetExampleSlide8Lesson2();
            m1.PrintTransitions();
            Console.WriteLine("isDFA: " + m1.IsDFA());
            Console.WriteLine("Accept: " + m1.Accept("abababab"));
            Test.PrintLanguageTillLength(m1, 5);
            m1.CreateGraph("1_graph2");

            Console.WriteLine();
            Console.WriteLine();

            Automata<string> m2 = Test.GetExampleSlide14Lesson2();
            m2.PrintTransitions();
            Test.PrintLanguageTillLength(m2, 5);
            Console.WriteLine("isDFA: " + m2.IsDFA());
            Console.WriteLine("Accept: " + m2.Accept("aab"));
            m2.CreateGraph("1_graph1");

            Test.TestRegExp();
            Test.TestRegRam(m2);
            var m = Test.NdfaToDFa();
            m.CreateGraph("2_NDFA");
            m.Reverse().CreateGraph("2_NdfaReverse");
            m.ToDFA().CreateGraph("2_DFA");
            m.ToDFA().Reverse().CreateGraph("2_Reverse");
            m.ToDFA().CreateGraph("2_Minimalised");

            var m3 = Test.MinimalizationAutomata();
            m3.CreateGraph("3_MinimalisationAutomata");
            m3.Reverse().CreateGraph("3_Reversed");
            m3.Minimalise().CreateGraph("3_DoneMinimalization");

            var m4 = Test.NDfaToDfa2();
            m4.CreateGraph("5_NDFA");
            m4.ToDFA().CreateGraph("5_DFA");
            m4.ToDFA().Reverse().CreateGraph("5_DFA_REV");
            m4.ToDFA().Minimalise().CreateGraph("5_DFA_MIN");

            var m5 = Test.TestAutomata2();
            m5.CreateGraph("6_DFA");
            m5.Reverse().CreateGraph("6_MIN");
            m5.Reverse().ToDFA().CreateGraph("6_REV_DFA");
            Console.ReadKey();
        }
    }
}

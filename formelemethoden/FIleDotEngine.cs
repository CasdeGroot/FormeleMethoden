using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formelemethoden
{
    class FileDotEngine
    {
        public static void Run(string dotFilePath)
        {
            string executable = @"..\..\external\dot.exe";
            string output =$@"{dotFilePath}.png";


            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = executable;
            process.StartInfo.Arguments = $"-Tpng {dotFilePath}.dot -o {output}";
            Console.WriteLine(process.StartInfo.Arguments);
            // Go
            process.Start();
            // and wait dot.exe to complete and exit
            process.WaitForExit();
        }
    }
}

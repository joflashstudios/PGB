using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO.Win32
{
    public static class IOTest
    {
        public static void RunTest()
        {
            StringBuilder numbers = new StringBuilder();
            Random r = new Random();
            for (int x = 1; x < 20; x++)
            {
                numbers.Append(r.Next(1, 12589) + "\n");
                System.Threading.Thread.Sleep(75);
                r = new Random(r.Next(900));                
            }
            Console.Write(numbers);
            System.IO.File.WriteAllText("C:\\users\\elizabeth\\numbers.txt", numbers.ToString());
            Console.ReadLine();
        }
    }
}

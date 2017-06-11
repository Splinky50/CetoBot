using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Replayer
{
	class Program
	{
		static void Main(string[] args)
		{
			string mainFolder = @"C:\Projects\Entelect 2017\Test\Version 1.0.5\Game Engine\Replays";
			string[] mainDirs = Directory.GetDirectories(mainFolder);
			Array.Sort(mainDirs);

			string[] dirs = Directory.GetDirectories(mainDirs[mainDirs.Length-1]);
			//Array.Sort(dirs);
			Array.Sort(dirs, (a, b) => int.Parse(a.Substring(a.IndexOf('d')+1)) - int.Parse(b.Substring(a.IndexOf('d')+1)));


			foreach (string s in dirs)
			{
				Console.Clear();
				string[] mapStrings = File.ReadAllLines(s + "\\A - map.txt");
				
				foreach(string line in mapStrings)
				{
					foreach(char c in line)
					{
						if(c == '*')
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else if(c == '!')
						{
							Console.ForegroundColor = ConsoleColor.Blue;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						Console.Write(c);
					}
					Console.WriteLine();
				}

				Console.ReadKey();
			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("******* END ****************");
			Console.ReadKey();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CetoBot.Domain;
using System.Drawing;

namespace CetoLearner
{
	class Program
	{
		static void Main(string[] args)
		{
			MapGenerator myGen = new MapGenerator(10, 10, 40);
			myGen.PrintMap();
			Console.WriteLine();

			DataPoint test = new DataPoint(myGen.CurrentMap, new Point(5, 5));
			Console.WriteLine(test.ToString());

			Console.ReadKey();
		}
	}
}

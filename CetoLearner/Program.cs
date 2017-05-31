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
			const int DATA_SET_SIZE = 100;

			// section generate data points
			List<DataPoint> dataList = new List<DataPoint>();


			for(int i = 0; i < DATA_SET_SIZE/2; i++)
			{
				MapGenerator myGen = new MapGenerator(10, 10, 40);
				DataPoint nextPoint = myGen.GetRandomOpenSpace(true);
				dataList.Add(nextPoint);

//				Console.WriteLine("ITERATION: " + i);
//				myGen.PrintMap();
//				Console.WriteLine(nextPoint.ToString());
//				Console.WriteLine("*******************************************");
//				Console.ReadKey();

			}

			for (int i = 0; i < DATA_SET_SIZE / 2; i++)
			{
				MapGenerator myGen = new MapGenerator(10, 10, 40);
				dataList.Add(myGen.GetRandomOpenSpace(false));
			}

			const double STEP_SIZE = 0.01;

			Dictionary<string, double> weights = new Dictionary<string, double>();
			Dictionary<string, double> features = new Dictionary<string, double>();
			string[] featureList =
			{
				"NorthHit",
				"NorthMiss",
				"NorthOpen",
				"SouthHit",
				"SouthMiss",
				"SouthOpen",
				"EastHit",
				"EastMiss",
				"EastOpen",
				"WestHit",
				"WestMiss",
				"WestOpen"
			};

			weights["intercept"] = 1;
			foreach (string s in featureList)
			{
				weights[s] = 0;
			}

			for (int i = 0; i < 100; i++)
			{
				foreach(KeyValuePair<string,double> w in weights)
				{
					double partial = 0.0;		// TODO: Finish this equation - need data set by this point
					weights[w.Key] = w.Value + STEP_SIZE * partial;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CetoBot.Domain;
using System.Drawing;
using CetoCommon;
using System.Windows.Input;
using System.IO;

namespace CetoLearner
{
	class Program
	{
		static void Main(string[] args)
		{
			string logPath = @"C:\Projects\Entelect 2017\CetoBot\CetoLearner\weightLog " + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".csv";
			const int LOGGING_INTERVAL = 10;
			int[] roundAve = new int[LOGGING_INTERVAL];
			
			// section generate data points
			List<DataPoint> dataList = new List<DataPoint>();
			Dictionary<int, WeightSet> WeightSets = new Dictionary<int, WeightSet>();
			//List<WeightSet> WeightSets = new List<WeightSet>();


			string[] featureList =
			{
				"HitSpacesInRow",
				//"HitSpacesInRow1",
				//"HitSpacesInRow2",
				//"HitSpacesInRow3",
				//"HitSpacesInRow4",
				//"HitSpacesInRow5",
				//"OpenSpacesInRow",
				"AdjMisses",
				"NonMissSpaces1",
				"NonMissSpaces2",
				"NonMissSpaces3",
				"NonMissSpaces4",
				"NonMissSpaces5",
				"NoDiagHitsWithAdjHit"
			};

			MapGenerator tempMap = new MapGenerator(10, 10, 0);
			foreach (int len in tempMap.GetRemainingShipLengths())
			{
				WeightSets[len] = (new WeightSet(featureList));
			}
			string[] csvHeadings = { "GAMES,ROUNDS" };
			foreach (KeyValuePair<int, WeightSet> w in WeightSets)
			{
				foreach (string s in featureList)
				{
					csvHeadings[0] += (","+s+w.Key);
				}
			}
			File.WriteAllLines(logPath, csvHeadings);
			

			Console.WriteLine("Starting Learning...");
			const double STEP_SIZE = 0.01;

			int GameCount = 0;
			double p = 0.0;
			double[] partial = new double[featureList.Length];

			bool manualB = false;
			bool printB = false;
			bool continueToLearnB = true;


			while (continueToLearnB == true)
			{
				bool gameOverB = false;
				int Rounds = 0;

				// Create Empty Map
				MapGenerator myMap = new MapGenerator(10, 10, 0);


				while (gameOverB == false)
				{
					Console.Clear();
					if(Console.KeyAvailable)
					{
						ConsoleKeyInfo keyPressed = Console.ReadKey();
						if(keyPressed.KeyChar == 'm')
						{
							manualB = true;
							printB = true;
						}
						else if(keyPressed.KeyChar == 'a')
						{
							manualB = false;
						}
						else if (keyPressed.KeyChar == 's')
						{
							printB = true;
						}
						else if (keyPressed.KeyChar == 'c')
						{
							printB = false;
							Console.Clear();
						}
					}


					int minShipLen = myMap.GetMinimumRemainingShipLength();

					Point shootAt = DecisionMaker.AimCannons(myMap, WeightSets[minShipLen], printB);
					bool isHitB = myMap.ShootAtPoint(shootAt);


					int j = 0;
					foreach (string s in featureList)
					{
						partial[j] = 0.0;
						DataPoint d = new DataPoint(myMap.CurrentMap, shootAt, isHitB ? 1 : -1);

						double val = 0.0;
						val = (d.Class == 1 ? 1 : 0);
						val -= MLHelper.Probability(WeightSets[minShipLen], d.Features);
						val *= d.Features[s];
						partial[j] += val;
						WeightSets[minShipLen].Weights[s] = WeightSets[minShipLen].Weights[s] + STEP_SIZE * partial[j];
						j++;
					}


					Rounds++;
					if (printB == true)
					{
						Console.WriteLine();
						Console.WriteLine();
						myMap.PrintMap();
						Console.WriteLine();
						Console.WriteLine();
						Console.WriteLine("******************************************");
						Console.WriteLine("GAME: " + (GameCount + 1));
						Console.WriteLine("ROUND: " + Rounds);
						Console.WriteLine("REM SHIPS: " + myMap.GetRemainingShips().Length);
						Console.WriteLine("MIN LEN SHIP: " + myMap.GetMinimumRemainingShipLength().ToString());
						Console.WriteLine("FEATURES: ");
						foreach (string s in featureList)
						{
							Console.WriteLine(s + ":\t\t" + WeightSets[minShipLen].Weights[s]);
						}
						Console.WriteLine("******************************************");
						Console.WriteLine("");
					}

					if (manualB == true)
					{
						ConsoleKeyInfo keyPressed = Console.ReadKey();
						if (keyPressed.KeyChar == 'a')
						{
							manualB = false;
						}
					}

					if (myMap.GetRemainingShips().Length == 0)
					{
						gameOverB = true;
					}
				}
				roundAve[GameCount % 10] = Rounds;
				GameCount++;
				if (printB == true)
				{
					Console.WriteLine("******************************************");
					Console.WriteLine("************* GAME END ***************");
					Console.WriteLine("******************************************");

					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine("GAMES: " + (GameCount));
					Console.WriteLine("ROUND COUNT: " + (Rounds));
					foreach (KeyValuePair<int, WeightSet> w in WeightSets)
					{
						Console.WriteLine("MIN LEN SHIP: " + w.Key);
						Console.WriteLine("FEATURES: ");
						foreach (string s in featureList)
						{
							Console.WriteLine(s + ":\t\t" + WeightSets[w.Key].Weights[s]);
						}
						Console.WriteLine();
					}
				}

				if(GameCount % 10 == 0)
				{
					string[] oldFile = File.ReadAllLines(logPath);
					List<string> fileStrings = new List<string>();
					foreach(string s in oldFile)
					{
						fileStrings.Add(s);
					}
					string strToAdd = GameCount.ToString();
					int ave = 0;
					foreach(int i in roundAve)
					{
						ave += i;
					}
					ave = ave / roundAve.Length;
					strToAdd += ("," + ave);
					foreach (KeyValuePair<int, WeightSet> w in WeightSets)
					{
						foreach (string s in featureList)
						{
							strToAdd += ("," + WeightSets[w.Key].Weights[s]);
						}
					}
					fileStrings.Add(strToAdd);
					File.WriteAllLines(logPath, fileStrings);
				}
				
				if(GameCount == int.MaxValue)
				{
					Console.WriteLine("REACHED MAXIMUM LOOPS");
					continueToLearnB = false;
				}

				if (manualB == true)
				{
					ConsoleKeyInfo keyPressed =  Console.ReadKey();
					if (keyPressed.KeyChar == 'a')
					{
						manualB = false;
					}
				}
			}

			Console.WriteLine("******************************************");
			Console.WriteLine("********LEARNING OVER*****************");
			Console.WriteLine("******************************************");
		}


		void Log()
		{

		}
	}
}

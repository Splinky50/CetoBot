using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CetoBot.Domain;
using System.Drawing;

namespace CetoCommon
{
	public static class DecisionMaker
	{
		public static Point AimCannons(dynamic state, WeightSet weights)
		{
			return AimCannons(new MapGenerator(state), weights);
		}

		public static Point AimCannons(MapGenerator myMap, WeightSet weights, bool printB = false)
		{

			// Look for open space with highest probability
			Point shootAt = new Point();
			bool firstOpenSpaceFoundB = false;
			double highestProb = 0.0f;
			List<DataPoint> possiblePoints = new List<DataPoint>();
			foreach (MapSpace m in myMap.CurrentMap.Cells)
			{
				if (m.State == SpaceState.Open)
				{
					if (firstOpenSpaceFoundB == false)
					{
						firstOpenSpaceFoundB = true;
						shootAt = m.Coords;
					}
					double prob = 0.0f;

					DataPoint d = new DataPoint(myMap.CurrentMap, m.Coords);
					prob = MLHelper.Probability(weights, d.Features);
					prob = Math.Round(prob, 10);
					//Log(String.Format("{0}",prob));
					if (prob > highestProb)
					{
						highestProb = prob;
						possiblePoints = new List<DataPoint>();
					}

					if (prob == highestProb)
					{
						possiblePoints.Add(d);
					}
				}
			}

			Random rnd = new Random();
			if (possiblePoints.Count > 0)
			{
				shootAt = possiblePoints[rnd.Next(0, possiblePoints.Count)].Coords;
			}

			if (printB == true)
			{
				Console.WriteLine("HIGHEST PROB: " + highestProb);
				Console.WriteLine("POINTS: " + possiblePoints.Count);
			}

			return shootAt;
		}
	}
}

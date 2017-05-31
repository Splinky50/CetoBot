using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CetoLearner
{
	public class DataPoint
	{
		public Point Coords;
		public Dictionary<string, double> Features;
		public int Class = 1;

		public DataPoint(Map map, Point coords, int outPutClass)
		{
			Coords = coords;
			Features = BuildFeatures(map, coords);
			Class = outPutClass;
		}

		public override string ToString()
		{
			string retS = "";
			retS += "X: " + Coords.X + Environment.NewLine;
			retS += "Y: " + Coords.Y + Environment.NewLine;

			foreach(KeyValuePair<string, double> feature in Features)
			{
				retS += feature.Key + ": " + feature.Value + Environment.NewLine;
			}
			return retS;
		}


		// THIS FUNCTION WILL DEFINE FEATURES OF LEARNER
		public static Dictionary<string, double> BuildFeatures(Map map, Point coords)
		{
			Dictionary<string, double> features = new Dictionary<string, double>();

			// North Space is a hit
			if (map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.Hit)
			{
				features["NorthHit"] = 1.0f;
			}
			else
			{
				features["NorthHit"] = 0.0f;
			}

			// North Space is a miss
			if (map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.Miss)
			{
				features["NorthMiss"] = 1.0f;
			}
			else
			{
				features["NorthMiss"] = 0.0f;
			}

			// North Space is open
			if (map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.Open)
			{
				features["NorthOpen"] = 1.0f;
			}
			else
			{
				features["NorthOpen"] = 0.0f;
			}


			// South Space is a hit
			if (map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.Hit)
			{
				features["SouthHit"] = 1.0f;
			}
			else
			{
				features["SouthHit"] = 0.0f;
			}

			// South Space is a miss
			if (map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.Miss)
			{
				features["SouthMiss"] = 1.0f;
			}
			else
			{
				features["SouthMiss"] = 0.0f;
			}

			// South Space is open
			if (map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.Open)
			{
				features["SouthOpen"] = 1.0f;
			}
			else
			{
				features["SouthOpen"] = 0.0f;
			}


			// West Space is a hit
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Hit)
			{
				features["WestHit"] = 1.0f;
			}
			else
			{
				features["WestHit"] = 0.0f;
			}

			// West Space is a miss
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Miss)
			{
				features["WestMiss"] = 1.0f;
			}
			else
			{
				features["WestMiss"] = 0.0f;
			}

			// West Space is open
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Open)
			{
				features["WestOpen"] = 1.0f;
			}
			else
			{
				features["WestOpen"] = 0.0f;
			}


			// East Space is a hit
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Hit)
			{
				features["EastHit"] = 1.0f;
			}
			else
			{
				features["EastHit"] = 0.0f;
			}

			// East Space is a miss
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Miss)
			{
				features["EastMiss"] = 1.0f;
			}
			else
			{
				features["EastMiss"] = 0.0f;
			}

			// East Space is open
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Open)
			{
				features["EastOpen"] = 1.0f;
			}
			else
			{
				features["EastOpen"] = 0.0f;
			}

			return features;
		}
	}
}

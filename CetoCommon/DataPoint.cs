using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CetoCommon
{
	public class DataPoint
	{
		public Point Coords;
		public Dictionary<string, double> Features;
		public int Class = 1;

		public DataPoint(Map map, Point coords, int outPutClass = 1)
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


		//"HitSpacesInRow"
		//"OpenSpacesInRow"
		//"AdjMisses"
		//"NonMissSpaces1" - 1 non-hit space in the row adjacent to space
		//"NonMissSpaces2" - 2 non-hit space in the row adjacent to space
		//"NonMissSpaces3" - 3 non-hit space in the row adjacent to space
		//"NonMissSpaces4" - 4 non-hit space in the row adjacent to space
		//"NonMissSpaces5" - 5 non-hit space in the row adjacent to space
		//"HitSpacesInRow1"
		//"HitSpacesInRow2"
		//"HitSpacesInRow3"
		//"HitSpacesInRow4"
		//"HitSpacesInRow5"
		//"NoDiagHitsWithAdjHit"

		// THIS FUNCTION BUILDS FEATURES FOR SINGLE MAP SPACE BASED ON MAP INFO
		public static Dictionary<string, double> BuildFeatures(Map map, Point coords)
		{
			Dictionary<string, double> features = new Dictionary<string, double>();

			//"HitSpacesInRow"
			int noVertHits = 0;
			noVertHits = GetNumberOfBlocksInRow(map, coords, DIRECTION.VERTICAL, SpaceState.Hit);
			int noHorHits = 0;
			noHorHits = GetNumberOfBlocksInRow(map, coords, DIRECTION.HORIZONTAL, SpaceState.Hit);
			if(noVertHits < noHorHits)
			{
				noVertHits = noHorHits;
			}
			features["HitSpacesInRow"] = noVertHits;
			for (int i = 1; i <= 5; i++)
			{
				if (noVertHits == i)
				{
					features[("HitSpacesInRow" + i)] = 1;
				}
				else
				{
					features[("HitSpacesInRow" + i)] = 0;
				}
			}


			//"NoOpen"
			int noVertOpen = 0;
			noVertOpen = GetNumberOfBlocksInRow(map, coords, DIRECTION.VERTICAL, SpaceState.Open);
			int noHorOpen = 0;
			noHorOpen = GetNumberOfBlocksInRow(map, coords, DIRECTION.HORIZONTAL, SpaceState.Open);
			if (noVertOpen < noHorOpen)
			{
				noVertOpen = noHorOpen;
			}
			features["OpenSpacesInRow"] = noVertOpen;

			//"NoMisses"
			int noMisses = 0;
			// South Space
			if ((map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.Miss) || (map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.None))
			{
				noMisses++;
			}
			// North Space
			if ((map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.Miss) || (map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.None))
			{
				noMisses++;
			}
			// East Space
			if ((map.GetSpace(coords.X+1, coords.Y).State == SpaceState.Miss) || (map.GetSpace(coords.X+1, coords.Y).State == SpaceState.None))
			{
				noMisses++;
			}
			// West Space
			if ((map.GetSpace(coords.X-1, coords.Y).State == SpaceState.Miss) || (map.GetSpace(coords.X-1, coords.Y).State == SpaceState.None))
			{
				noMisses++;
			}
			if(noMisses != 0)
			{
				noMisses = (int)Math.Pow(2, (noMisses - 1));
			}
			features["AdjMisses"] = noMisses;


			// "NonHitSpaces"
			int vertNonHit = GetNumberOfBlocksInRow(map, coords, DIRECTION.VERTICAL);
			int horNonHit = GetNumberOfBlocksInRow(map, coords, DIRECTION.HORIZONTAL);
			if (vertNonHit < horNonHit)
			{
				vertNonHit = horNonHit;
			}
			
			for(int i = 1; i <= 5; i++)
			{
				if(vertNonHit == i)
				{
					features[("NonMissSpaces" + i)] = 1;
				}
				else
				{
					features[("NonMissSpaces" + i)] = 0;
				}
			}

			//"NoDiagHitsWithAdjHit"
			int noDiagHits = 0;
			// north hit
			if(map.GetSpace(coords.X, coords.Y + 1).State == SpaceState.Hit)
			{
				if((map.GetSpace(coords.X-1, coords.Y + 1).State != SpaceState.Hit) && (map.GetSpace(coords.X+1, coords.Y + 1).State != SpaceState.Hit))
				{
					noDiagHits++;
				}
			}
			// south hit
			if (map.GetSpace(coords.X, coords.Y - 1).State == SpaceState.Hit)
			{
				if ((map.GetSpace(coords.X - 1, coords.Y - 1).State != SpaceState.Hit) && (map.GetSpace(coords.X + 1, coords.Y - 1).State != SpaceState.Hit))
				{
					noDiagHits++;
				}
			}
			// east hit
			if (map.GetSpace(coords.X+1, coords.Y).State == SpaceState.Hit)
			{
				if ((map.GetSpace(coords.X + 1, coords.Y + 1).State != SpaceState.Hit) && (map.GetSpace(coords.X + 1, coords.Y - 1).State != SpaceState.Hit))
				{
					noDiagHits++;
				}
			}
			// west hit
			if (map.GetSpace(coords.X - 1, coords.Y).State == SpaceState.Hit)
			{
				if ((map.GetSpace(coords.X - 1, coords.Y + 1).State != SpaceState.Hit) && (map.GetSpace(coords.X - 1, coords.Y - 1).State != SpaceState.Hit))
				{
					noDiagHits++;
				}
			}
			features["NoDiagHitsWithAdjHit"] = noDiagHits;


			return features;
		}

		private enum DIRECTION
		{
			VERTICAL,
			HORIZONTAL
		}
		private static int GetNumberOfBlocksInRow(Map map, Point coords, DIRECTION dir, SpaceState state)
		{
			int blocksInRow = 0;
			int loopEnd = (dir == DIRECTION.VERTICAL ? map.Height : map.Width);
			int loopBegin = (dir == DIRECTION.VERTICAL ? coords.Y : coords.X);

			for (int i = loopBegin + 1; i < loopEnd; i++)
			{
				MapSpace m;
				if (dir == DIRECTION.VERTICAL)
				{
					m = map.GetSpace(coords.X, i);
				}
				else
				{
					m = map.GetSpace(i, coords.Y);
				}
				if(m.State == state)
				{
					blocksInRow++;
				}
				else
				{
					break;
				}
			}

			for (int i = loopBegin - 1; i > 0; i--)
			{
				MapSpace m;
				if (dir == DIRECTION.VERTICAL)
				{
					m = map.GetSpace(coords.X, i);
				}
				else
				{
					m = map.GetSpace(i, coords.Y);
				}
				if(m.State == state)
				{
					blocksInRow++;
				}
				else
				{
					break;
				}
			}
			return blocksInRow;
		}
		private static int GetNumberOfBlocksInRow(Map map, Point coords, DIRECTION dir)
		{
			int blocksInRow = 0;
			int loopEnd = (dir == DIRECTION.VERTICAL ? map.Height : map.Width);
			int loopBegin = (dir == DIRECTION.VERTICAL ? coords.Y : coords.X);

			for (int i = loopBegin + 1; i < loopEnd; i++)
			{
				MapSpace m;
				if (dir == DIRECTION.VERTICAL)
				{
					m = map.GetSpace(coords.X, i);
				}
				else
				{
					m = map.GetSpace(i, coords.Y);
				}
				if ((m.State != SpaceState.Miss) && (m.State != SpaceState.None))
				{
					blocksInRow++;
				}
				else
				{
					break;
				}
			}

			for (int i = loopBegin - 1; i > 0; i--)
			{
				MapSpace m;
				if (dir == DIRECTION.VERTICAL)
				{
					m = map.GetSpace(coords.X, i);
				}
				else
				{
					m = map.GetSpace(i, coords.Y);
				}
				if (m.State != SpaceState.Hit)
				{
					blocksInRow++;
				}
				else
				{
					break;
				}
			}
			return blocksInRow;
		}

	}
}

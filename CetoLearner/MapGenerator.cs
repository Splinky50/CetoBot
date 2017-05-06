using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using CetoBot.Domain;

namespace CetoLearner
{
	class MapGenerator
	{
		

		MapGenerator(int mapWidth, int mapHeight)
		{


			// 1. Randomly Place ships
			var shipsToPlace = new List<Ship>
			{
				Ship.Battleship,
				Ship.Carrier,
				Ship.Cruiser,
				Ship.Destroyer,
				Ship.Submarine
			};

			List<ShipPlacement> MyShips = new List<ShipPlacement>();

			// for now randomly place ships
			foreach (Ship s in shipsToPlace)
			{
				bool gotPosB = false;
				Random rnd = new Random();

				while (gotPosB == false)
				{
					int x = rnd.Next(0, mapWidth);
					int y = rnd.Next(0, mapHeight);

					var v = Enum.GetValues(typeof(Direction));
					Direction newDir = (Direction)v.GetValue(new Random().Next(v.Length));

					ShipPlacement newShip = new ShipPlacement(new Point(x, y), s, newDir);
					if ((newShip.IsCollidedB(MyShips) == false) && (newShip.IsOffMapB(mapWidth, mapHeight) == false))
					{
						MyShips.Add(newShip);
						gotPosB = true;
					}
				}
			}


			
		}
	}


	public enum SpaceState
	{
		None,		// space is not on board
		Open,		// Open seas captain
		Miss,		// Miss
		Hit			// Hit
	}

	class MapSpace
	{
		SpaceState State = SpaceState.None;
		Point Coords = new Point();

	}
}

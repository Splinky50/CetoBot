using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using CetoBot.Domain;

namespace CetoCommon
{
	public class MapGenerator
	{
		public List<ShipPlacement> ShipList;
		public Map CurrentMap = new Map();
		public int Height;
		public int Width;
		public int Shots;
		public int Hits;
		public int Misses;

		public MapGenerator(int mapWidth, int mapHeight, int maxShots)
		{
			Height = mapHeight;
			Width = mapWidth;

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
					Direction newDir = (Direction)v.GetValue(rnd.Next(v.Length));

					ShipPlacement newShip = new ShipPlacement(new Point(x, y), s, newDir);
					if ((newShip.IsCollidedB(MyShips) == false) && (newShip.IsOffMapB(mapWidth, mapHeight) == false))
					{
						MyShips.Add(newShip);
						gotPosB = true;
					}
				}
			}
			// assign list of ships
			ShipList = MyShips;


			// 2. Randomly choose number of shots
			Random rand = new Random();
			int numShots = 0;
			if (maxShots != 0)
			{
				numShots = rand.Next(1, maxShots + 1);
				Shots = numShots;
			}

			// 3. Choose random co-ords to shoot at
			// create list
			CurrentMap.Cells = new List<MapSpace>();
			for(int i = 0; i < mapWidth*mapHeight; i++)
			{
				MapSpace nextSpace = new MapSpace(i % mapWidth, (int)(i / mapHeight));
				nextSpace.State = SpaceState.Open;
				CurrentMap.Cells.Add(nextSpace);
			}
			while (numShots > 0)
			{
				bool foundOpenB = false;

				while (foundOpenB == false)
				{
					int shotX = rand.Next(0, mapWidth);
					int shotY = rand.Next(0, mapHeight);

					if(CurrentMap.GetSpace(shotX, shotY).State == SpaceState.Open)
					{
						foundOpenB = true;

						bool isHitB = false;
						foreach(ShipPlacement s in ShipList)
						{
							if(s.IsCollidedB(CurrentMap.GetSpace(shotX, shotY).Coords))
							{
								isHitB = true;
								break;
							}
						}

						if(isHitB == true)
						{
							Hits++;
							CurrentMap.GetSpace(shotX, shotY).State = SpaceState.Hit;
						}
						else
						{
							Misses++;
							CurrentMap.GetSpace(shotX, shotY).State = SpaceState.Miss;
						}

					}

				}
				numShots--;
			}
		}


		public MapGenerator(dynamic state)
		{
			this.Width = state.PlayerMap.MapWidth;
			this.Height = state.PlayerMap.MapHeight;


			CurrentMap.Cells = new List<MapSpace>();
			for (int i = 0; i < Width * Height; i++)
			{
				MapSpace nextSpace = new MapSpace((int)(state.OpponentMap.Cells[i].X), (int)(state.OpponentMap.Cells[i].Y));
				if (state.OpponentMap.Cells[i].Damaged == true)
				{
					nextSpace.State = SpaceState.Hit;
				}
				else if (state.OpponentMap.Cells[i].Missed == true)
				{
					nextSpace.State = SpaceState.Miss;
				}
				else
				{
					nextSpace.State = SpaceState.Open;
				}
				CurrentMap.Cells.Add(nextSpace);
			}
		}

		public bool ShootAtPoint(Point shootAt)
		{
			bool isHitB = false;

			foreach (ShipPlacement s in ShipList)
			{
				if (s.IsCollidedB(CurrentMap.GetSpace(shootAt).Coords))
				{
					isHitB = true;
					break;
				}
			}

			if (isHitB == true)
			{
				CurrentMap.GetSpace(shootAt).State = SpaceState.Hit;
			}
			else
			{
				CurrentMap.GetSpace(shootAt).State = SpaceState.Miss;
			}

			return isHitB;
		}


		public DataPoint GetRandomOpenSpace(bool HitOrMiss, int maxIterations = 1000)
		{
			Random rnd = new Random();
			DataPoint retPoint = new DataPoint(this.CurrentMap, new Point(Height - 1, Width-1), -1);

			bool gotSpace = false;
			while((gotSpace == false) && (maxIterations > 0))
			{
				maxIterations--;
				int x = rnd.Next(0, Width);
				int y = rnd.Next(0, Height);

				if (this.CurrentMap.GetSpace(x, y).State == SpaceState.Open)
				{
					bool isHit = false;

					foreach (ShipPlacement s in ShipList)
					{
						if (s.IsCollidedB(new Point(x, y)))
						{
							isHit = true;
						}
					}

					retPoint = new DataPoint(this.CurrentMap, new Point(x, y), isHit ? 1 : -1);

					if (((isHit == true) && (HitOrMiss == true)) || ((isHit == false) && (HitOrMiss == false)))
					{
						gotSpace = true;
					}
				}
			}

			return retPoint;
		}

		public ShipPlacement[] GetRemainingShips()
		{
			List<ShipPlacement> remShips = new List<ShipPlacement>();

			foreach(ShipPlacement s in ShipList)
			{
				bool shipIsAliveB = false;
				foreach(MapSpace m in CurrentMap.Cells)
				{
					if((s.IsCollidedB(m.Coords)) && (m.State == SpaceState.Open))
					{
						shipIsAliveB = true;
						break;
					}
				}

				if(shipIsAliveB == true)
				{
					remShips.Add(s);
				}
			}
			return remShips.ToArray();
		}

		public int[] GetRemainingShipLengths()
		{
			List<int> remLens = new List<int>();
			foreach(ShipPlacement s in GetRemainingShips())
			{
				int len = s.ShipLength;
				if(remLens.Contains(len) == false)
				{
					remLens.Add(len);
				}
			}
			return remLens.ToArray();
		}

		public int GetMinimumRemainingShipLength()
		{
			int minLen = int.MaxValue;
			foreach(int len in GetRemainingShipLengths())
			{
				if(len < minLen)
				{
					minLen = len;
				}
			}
			return minLen;
		}

		public void PrintMap()
		{
			Shots = 0;
			Hits = 0;
			Misses = 0;

			for (int j = 0; j < Height + 2; j++)
			{
				Console.ForegroundColor = ConsoleColor.White;
				if ((j == 0) || (j == Height+1))
				{
					for (int i = 0; i < Width + 2; i++)
					{
						Console.Write("-");
					}
					Console.WriteLine();
				}
				else
				{
					Console.Write("|");
					for (int i = 0; i < Width; i++)
					{
						string strToWrite = "~";
						Console.ForegroundColor = ConsoleColor.White;

						foreach (ShipPlacement s in ShipList)
						{
							if(s.IsCollidedB(new Point(i, j-1)))
							{
								strToWrite = "S";
								Console.ForegroundColor = ConsoleColor.Green;
							}
						}


						if (CurrentMap.GetSpace(i , j-1).State == SpaceState.Hit)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							strToWrite = "*";
							Shots++;
							Hits++;
						}
						else if(CurrentMap.GetSpace(i, j - 1).State == SpaceState.Miss)
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							strToWrite = "!";
							Shots++;
							Misses++;
						}

						Console.Write(strToWrite);
						Console.ForegroundColor = ConsoleColor.White;
					}
					Console.WriteLine("|");
				}
			}

			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine();
			Console.WriteLine("Number of Shots: " + Shots);
			Console.WriteLine("Number of Hits: " + Hits);
			Console.WriteLine("Number of Misses: " + Misses);
			Console.WriteLine();
		}
	}


	public enum SpaceState
	{
		None,		// space is not on board
		Open,		// Open seas captain
		Miss,		// Miss
		Hit			// Hit
	}


	
	public class MapSpace
	{
		public SpaceState State = SpaceState.None;
		public Point Coords = new Point();

		public MapSpace(int x, int y)
		{
			Coords.X = x;
			Coords.Y = y;
		}

	}

	// Map Class
	public class Map
	{
		public List<MapSpace> Cells =  new List<MapSpace>();

		public MapSpace GetSpace(int x, int y)
		{
			foreach(MapSpace m in Cells)
			{
				if((m.Coords.X == x) && (m.Coords.Y == y))
				{
					return m;
				}
			}
			return new MapSpace(x, y);
		}

		public MapSpace GetSpace(Point point)
		{
			foreach (MapSpace m in Cells)
			{
				if ((m.Coords.X == point.X) && (m.Coords.Y == point.Y))
				{
					return m;
				}
			}
			return new MapSpace(point.X, point.Y);
		}

		public int Height
		{
			get
			{
				int h = 0;
				foreach (MapSpace m in Cells)
				{
					if(m.Coords.Y > h)
					{
						h = m.Coords.Y;
					}
				}
				return h;
			}
		}

		public int Width
		{
			get
			{
				int h = 0;
				foreach (MapSpace m in Cells)
				{
					if (m.Coords.X > h)
					{
						h = m.Coords.X;
					}
				}
				return h;
			}
		}
	}
}

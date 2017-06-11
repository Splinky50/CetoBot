using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CetoBot.Domain;

namespace CetoBot.Domain
{
	public class ShipPlacement
	{
		public Point ShipPoint { get; set; }
		public Ship ShipType { get; set; }
		public Direction ShipDirection { get; set; }

		public int ShipLength
		{
			get
			{
				int len = 0;
				switch (ShipType)
				{
					case Ship.Battleship:
						len = 4;
						break;

					case Ship.Carrier:
						len = 5;
						break;

					case Ship.Cruiser:
						len = 3;
						break;

					case Ship.Destroyer:
						len = 2;
						break;

					case Ship.Submarine:
						len = 3;
						break;

					default:
						len = 0;
						break;
				}
				return len;
			}
		}

		public List<Point> Points
		{
			get
			{
				List<Point> myPoints = new List<Point>();
				for (int i = 0; i < this.ShipLength; i++)
				{
					switch (this.ShipDirection)
					{
						case Direction.East:
							myPoints.Add(new Point(this.ShipPoint.X + i, this.ShipPoint.Y));
							break;

						case Direction.South:
							myPoints.Add(new Point(this.ShipPoint.X, this.ShipPoint.Y - i));
							break;

						case Direction.West:
							myPoints.Add(new Point(this.ShipPoint.X - i, this.ShipPoint.Y));
							break;

						case Direction.North:
							myPoints.Add(new Point(this.ShipPoint.X, this.ShipPoint.Y + i));
							break;

						default:
							break;
					}
				}
				return myPoints;
			}
		}

		public ShipPlacement(Point newPoint, Ship newType, Direction newDir)
		{
			ShipPoint = newPoint;
			ShipType = newType;
			ShipDirection = newDir;
		}


		public bool IsCollidedB(Point otherPoint)
		{
			bool collidedB = false;

			foreach (Point myPoint in this.Points)
			{
				if (myPoint == otherPoint)
				{
					collidedB = true;
					break;
				}

				if (collidedB)
					break;
			}

			return collidedB;
		}

		public bool IsCollidedB(ShipPlacement otherShip)
		{
			bool collidedB = false;

			foreach (Point myPoint in this.Points)
			{
				foreach (Point otherPoint in otherShip.Points)
				{
					if (myPoint == otherPoint)
					{
						collidedB = true;
						break;
					}
				}

				if (collidedB)
					break;
			}

			return collidedB;
		}

		public bool IsCollidedB(List<ShipPlacement> otherShips)
		{
			bool collidedB = false;

			foreach (ShipPlacement otherShip in otherShips)
			{
				if (this.IsCollidedB(otherShip) == true)
				{
					collidedB = true;
					break;
				}
			}

			return collidedB;
		}

		public bool IsOffMapB(int mapWidth, int mapHeight)
		{
			bool offMapB = false;

			foreach (Point p in this.Points)
			{
				if ((p.X < 0) || (p.Y < 0) || (p.X >= mapWidth) || (p.Y >= mapHeight))
				{
					offMapB = true;
					break;
				}
			}

			return offMapB;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Drawing;

namespace CetoBot.Domain
{
	public class PlaceShipCommand
	{

		public List<ShipPlacement> MyShips { get; set; }

        public override string ToString()
        {
            var output = "";
            for (var i = 0; i < MyShips.Count; i++)
            {
                output += $"{MyShips[i].ShipType} {MyShips[i].ShipPoint.X} {MyShips[i].ShipPoint.Y} {MyShips[i].ShipDirection}";
                if (i + 1 != MyShips.Count)
                {
                    output += Environment.NewLine;
                }
            }
            return output;
        }
    }
}
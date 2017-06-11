using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using CetoBot.Domain;
using CetoCommon;

namespace CetoBot
{
    public class Bot
    {
        protected string WorkingDirectory { get; set; }
        protected string Key { get; set; }

        private const string CommandFileName = "command.txt";

        private const string PlaceShipFileName = "place.txt";

        private const string StateFileName = "state.json";

        public Bot(string key, string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            Key = key;
        }

        public void Execute()
        {
            dynamic state = JsonConvert.DeserializeObject(LoadState());

            int phase = state.Phase;

            if (phase == 1)
            {
                var placeShips = PlaceShips((int)state.MapDimension, (int)state.MapDimension);
                WritePlaceShips(placeShips);
            }
            else
            {
                var move = MakeMove(state);
                WriteMove(move);
            }
        }

        private PlaceShipCommand PlaceShips(int mapWidth, int mapHeight)
        {
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
			foreach(Ship s in shipsToPlace)
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

			return new PlaceShipCommand
			{
				MyShips = MyShips
            };
        }

		Dictionary<string, double> weights = new Dictionary<string, double>();
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

		private Command MakeMove(dynamic state)
        {
			Dictionary<string, double> weights = new Dictionary<string, double>();
			/*weights["NorthHit"] = 184983;
			weights["NorthMiss"] = -12206;
			weights["NorthOpen"] = -6000;

			weights["SouthHit"] = 184983;
			weights["SouthMiss"] = -12206;
			weights["SouthOpen"] = -6000;

			weights["EastHit"] = 184983;
			weights["EastMiss"] = -12206;
			weights["EastOpen"] = -6000;

			weights["WestHit"] = 184983;
			weights["WestMiss"] = -12206;
			weights["WestOpen"] = -6000;*/

			weights["NorthHit"] = 0.996507500606418;
			weights["NorthMiss"] = -0.0671588524624563;
			weights["NorthOpen"] = -0.0534422400458033;

			weights["SouthHit"] = 0.996507500606418;
			weights["SouthMiss"] = -0.0671588524624563;
			weights["SouthOpen"] = -0.0534422400458033;

			weights["EastHit"] = 0.996507500606418;
			weights["EastMiss"] = -0.0671588524624563;
			weights["EastOpen"] = -0.0534422400458033;

			weights["WestHit"] = 0.996507500606418;
			weights["WestMiss"] = -0.0671588524624563;
			weights["WestOpen"] = -0.0534422400458033;


			// Generate map from Json
			MapGenerator myMap = new MapGenerator(state);

			// Look for open space with highest probability
			Point shootAt = new Point();
			bool firstOpenSpaceFoundB = false;
			double highestProb = 0.0f;
			List<DataPoint> possiblePoints = new List<DataPoint>();
			foreach(MapSpace m in myMap.CurrentMap.Cells)
			{
				if(m.State == SpaceState.Open)
				{
					if(firstOpenSpaceFoundB == false)
					{
						firstOpenSpaceFoundB = true;
						shootAt = m.Coords;
					}
					double prob = 0.0f;

					DataPoint d = new DataPoint(myMap.CurrentMap, m.Coords);
					prob = MLHelper.Probability(weights, d.Features);
					prob = Math.Round(prob, 6);
					//Log(String.Format("{0}",prob));
					if(prob >= highestProb)
					{
						highestProb = prob;
					}
				}
			}

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
					prob = Math.Round(prob, 6);
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


			return new Command(Code.FireShot, shootAt.X, shootAt.Y);
        }

        private string LoadState()
        {
            var filename = Path.Combine(WorkingDirectory, StateFileName);
            try
            {
                string jsonText;
                using (var file = new StreamReader(filename))
                {
                    jsonText = file.ReadToEnd();
                }

                return jsonText;
            }
            catch (IOException e)
            {
                Log($"Unable to read state file: {filename}");
                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
                return null;
            }
        }

        private void WriteMove(Command command)
        {
            var filename = Path.Combine(WorkingDirectory, CommandFileName);

            try
            {
                using (var file = new StreamWriter(filename))
                {
                    file.WriteLine(command);
                }

                Log("Command: " + command);
            }
            catch (IOException e)
            {
                Log($"Unable to write command file: {filename}");

                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
            }
        }

        private void WritePlaceShips(PlaceShipCommand placeShipCommand)
        {
            var filename = Path.Combine(WorkingDirectory, PlaceShipFileName);
            try
            {
                using (var file = new StreamWriter(filename))
                {
                    file.WriteLine(placeShipCommand);
                }

                Log("Placeship command: " + placeShipCommand);
            }
            catch (IOException e)
            {
                Log($"Unable to write place ship command file: {filename}");

                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
            }

        }

        private void Log(string message)
        {
            Console.WriteLine("[BOT]\t{0}", message);
        }
    }
}
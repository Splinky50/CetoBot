using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using CetoBot.Domain;

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
					Direction newDir = (Direction)v.GetValue(new Random().Next(v.Length));

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

        private Command MakeMove(dynamic state)
        {
            var random = new Random();
            var possibleShipCommands = Enum.GetValues(typeof(Code));
            var code = (Code) possibleShipCommands.GetValue(random.Next(0, possibleShipCommands.Length));
            return new Command(code, random.Next(0, 9), random.Next(0, 9));
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CetoLearner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CetoLearner.Tests
{
	[TestClass()]
	public class MapGeneratorTests
	{
		[TestMethod()]
		public void MapGeneratorTest()
		{
			MapGenerator myGen = new MapGenerator(10, 10, 40);

			myGen.PrintMap();

			Assert.Fail();
		}
	}
}
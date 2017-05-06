using Microsoft.VisualStudio.TestTools.UnitTesting;
using CetoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CetoBot.Tests
{
	[TestClass()]
	public class BotTests
	{
		[TestMethod()]
		public void ExecuteTest()
		{
			Bot bot = new Bot("A", @"C:\Projects\Entelect 2017\CetoBot\Ceto");

			bot.Execute();

			Assert.Fail();
		}
	}
}
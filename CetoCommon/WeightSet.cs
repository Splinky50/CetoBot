using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CetoCommon
{
	public class WeightSet
	{
		public string[] FeatureList;
		public Dictionary<string, double> Weights = new Dictionary<string, double>();

		public WeightSet(string[] wSet)
		{
			FeatureList = wSet;
			foreach (string s in FeatureList)
			{
				Weights[s] = 0.0f;
			}
		}
	}
}

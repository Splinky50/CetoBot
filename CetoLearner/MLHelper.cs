using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CetoLearner
{
	class MLHelper
	{
		public static double Score(Dictionary<string, double> weights, Dictionary<string, double> features)
		{
			double score = weights["intercept"];
			foreach(KeyValuePair<string, double> feature in features)
			{
				score += weights[feature.Key] * feature.Value;
			}
			return score;
		}


		public static double Probability(Dictionary<string, double> weights, Dictionary<string, double> features)
		{
			double score = MLHelper.Score(weights, features);
			return (1 / (1+Math.Pow(Math.E,(-1.0f*score))));
		}

	}
}

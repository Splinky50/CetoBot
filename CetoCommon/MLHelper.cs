using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CetoCommon
{
	public class MLHelper
	{
		public static double Score(WeightSet weights, Dictionary<string, double> features)
		{
			//double score = weights["intercept"];
			double score = 0.0;
			foreach (string feature in weights.FeatureList)
			{
				score += weights.Weights[feature] * features[feature];
			}
			return score;
		}


		public static double Probability(WeightSet weights, Dictionary<string, double> features)
		{
			double score = MLHelper.Score(weights, features);
			return (1.0f / (1.0f+Math.Pow(Math.E,(-1.0f*score))));
		}

	}
}

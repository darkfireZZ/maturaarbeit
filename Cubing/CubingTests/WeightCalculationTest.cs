using Cubing;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class WeightCalculationTest
    {
        [TestMethod]
        public void Test()
        {
            string path = @"C:\Users\LocalUser\Google Drive\Schule\Maturaarbeit\data.csv";
            List<CsTimerData> data = new List<CsTimerData>();
            CsTimerData.ReadCsTimerData(path, example => data.Add(example));
            double[] weights = MoveWeightsUtils.CalculateWeights(data);

            foreach (double weight in weights)
                Console.WriteLine(weight);
        }
    }
}
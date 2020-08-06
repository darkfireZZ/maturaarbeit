using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class WeightedTwoPhaseSolverTest
    {
        [DataRow("R")]
        [DataRow("R U R'")]
        [DataRow("L B2 D F' R' U L2")]
        [DataRow("R U R' U' R' F R2 U' R' U' R U R' F'")]
        [DataTestMethod]
        public void StaticTest(string algString)
        {
            Alg scramble = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(scramble);

            TimeSpan timeout = TimeSpan.FromSeconds(1);
            double returnValue = 4000;
            double requiredValue = 5000;
            double[] weights = {
                169.658122242946,
                253.796705882179,
                211.084512473536,
                206.193406852305,
                299.809258427086,
                155.496912851115,
                158.690899467791,
                281.70422762001,
                346.448146439892,
                274.885031555195,
                311.019164835258,
                258.486161100002,
                196.885474673572,
                275.657440421246,
                214.624405800259,
                322.649818802591,
                354.012863621357,
                321.02200029978 };

            double[] weightedPhase2PruningTable = WeightedPruningTables.CreateWeightedPhase2Table(weights);

            Alg solution = WeightedTwoPhaseSolver.FindSolution(cube, timeout, returnValue, requiredValue, weights, weightedPhase2PruningTable);
            Console.WriteLine("Scramble: " + scramble + "\nSolution: " + solution + "\nLength: " + solution.Length);

            Assert.IsTrue(solution.Select(move => weights[(int)move]).Sum() <= requiredValue);

            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.CreateSolved();

            result.ApplyAlg(scramble);
            result.ApplyAlg(solution);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CostTooHighTest() //There used to be an error when solving this scramble
        {
            string algString = "B2 D B F' D L U B U' F' R' F2 B' L' F' R B2 R' U R' L2 D R2 F2 L B2";
            Alg scramble = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(scramble);

            TimeSpan timeout = TimeSpan.FromSeconds(0);
            double requiredValue = 4700d;

            double[] weights = {
                169.658122242946, // R
                253.796705882179, // R2
                211.084512473536, // R'
                206.193406852305, // U
                299.809258427086, // U2
                155.496912851115, // U'
                158.690899467791, // F
                281.70422762001,  // F2
                346.448146439892, // F'
                274.885031555195, // L
                311.019164835258, // L2
                258.486161100002, // L'
                196.885474673572, // D
                275.657440421246, // D2
                214.624405800259, // D'
                322.649818802591, // B
                354.012863621357, // B2
                321.02200029978   // B'
            };

            double[] weightedPhase2PruningTable = WeightedPruningTables.CreateWeightedPhase2Table(weights);

            Alg solution = WeightedTwoPhaseSolver.FindSolution(cube, timeout, requiredValue, requiredValue, weights, weightedPhase2PruningTable);

            double cost = solution.Select(move => weights[(int)move])
                                  .Sum();

            Assert.IsTrue(cost <= requiredValue);

            Console.WriteLine("\nScramble: " + scramble + "\nSolution: " + solution + "\nCost: " + cost + " Length: " + solution.Length);
        }

        [DataTestMethod]
        [DataRow(true)]
        public void RandomTest(bool writeToFile)
        {
            TableController.InitializePhase1Tables();
            TableController.InitializePhase2Tables();

            Random random = new Random(7777777);

            TimeSpan timeout = TimeSpan.FromSeconds(0);
            double startMaximumCost = 10000d; //inclusive
            double endMaximumCost = 5000d; //inclusive
            int numDifferentValues = 101;
            double decrement = (startMaximumCost - endMaximumCost) / (numDifferentValues - 1);
            int repetitions = 100;

            double[] weights = {
                169.658122242946, // R
                253.796705882179, // R2
                211.084512473536, // R'
                206.193406852305, // U
                299.809258427086, // U2
                155.496912851115, // U'
                158.690899467791, // F
                281.70422762001,  // F2
                346.448146439892, // F'
                274.885031555195, // L
                311.019164835258, // L2
                258.486161100002, // L'
                196.885474673572, // D
                275.657440421246, // D2
                214.624405800259, // D'
                322.649818802591, // B
                354.012863621357, // B2
                321.02200029978   // B'
            };

            double[] weightedPhase2PruningTable = WeightedPruningTables.CreateWeightedPhase2Table(weights);

            (double maximumCost, double milliseconds)[] totalTimes = new (double maximumCost, double milliseconds)[numDifferentValues];
            Stopwatch timer = new Stopwatch();

            for (int iteration = 0; iteration < numDifferentValues; iteration++)
            {
                double requiredValue = startMaximumCost - iteration * decrement;
                totalTimes[iteration].maximumCost = requiredValue;

                for (int repetition = 0; repetition < repetitions; repetition++)
                {
                    Alg scramble = Alg.FromRandomMoves(random.Next(20, 30), random);
                    CubieCube cube = CubieCube.FromAlg(scramble);

                    timer.Restart();
                    Alg solution = WeightedTwoPhaseSolver.FindSolution(cube, timeout, requiredValue, requiredValue, weights, weightedPhase2PruningTable);
                    timer.Stop();

                    double cost = solution.Select(move => weights[(int)move]).Sum();

                    Assert.IsTrue(cost <= requiredValue);

                    CubieCube expected = CubieCube.CreateSolved();
                    CubieCube result = CubieCube.CreateSolved();

                    result.ApplyAlg(scramble);
                    result.ApplyAlg(solution);

                    Assert.AreEqual(expected, result);

                    totalTimes[iteration].milliseconds += timer.ElapsedMilliseconds;
                }
            }

            if (writeToFile)
            {
                using (StreamWriter writer = new StreamWriter(new FileStream("../../../weightedTwoPhaseSolverTestTimeResult.csv", FileMode.Open)))
                {
                    writer.WriteLine("maximum cost;total time (ms) for " + repetitions + " repetitions;");
                    for (int iteration = 0; iteration < numDifferentValues; iteration++)
                        writer.WriteLine(totalTimes[iteration].maximumCost + ";" + totalTimes[iteration].milliseconds + ";");
                }
            }
        }
    }
}
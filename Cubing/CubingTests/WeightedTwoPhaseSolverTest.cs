using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            float returnValue = 4000;
            float requiredValue = 5000;
            float[] weights = {
                169.658122242946f,
                253.796705882179f,
                211.084512473536f,
                206.193406852305f,
                299.809258427086f,
                155.496912851115f,
                158.690899467791f,
                281.70422762001f,
                346.448146439892f,
                274.885031555195f,
                311.019164835258f,
                258.486161100002f,
                196.885474673572f,
                275.657440421246f,
                214.624405800259f,
                322.649818802591f,
                354.012863621357f,
                321.02200029978f };

            float[] weightedPhase2PruningTable = WeightedPruningTables.CreateWeightedPhase2CornerEquatorTable(weights);

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
            float requiredValue = 4700f;

            float[] weights = {
                169.658122242946f, // R
                253.796705882179f, // R2
                211.084512473536f, // R'
                206.193406852305f, // U
                299.809258427086f, // U2
                155.496912851115f, // U'
                158.690899467791f, // F
                281.70422762001f,  // F2
                346.448146439892f, // F'
                274.885031555195f, // L
                311.019164835258f, // L2
                258.486161100002f, // L'
                196.885474673572f, // D
                275.657440421246f, // D2
                214.624405800259f, // D'
                322.649818802591f, // B
                354.012863621357f, // B2
                321.02200029978f   // B'
            };

            float[] weightedPhase2PruningTable = WeightedPruningTables.CreateWeightedPhase2CornerEquatorTable(weights);

            Alg solution = WeightedTwoPhaseSolver.FindSolution(cube, timeout, requiredValue, requiredValue, weights, weightedPhase2PruningTable);

            float cost = solution.Select(move => weights[(int)move])
                                  .Sum();

            Assert.IsTrue(cost <= requiredValue);

            Console.WriteLine("\nScramble: " + scramble + "\nSolution: " + solution + "\nCost: " + cost + " Length: " + solution.Length);
        }
    }
}
using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class TwoPhaseSolverTest
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
            int returnLength = 0;
            int requiredLength = 20;

            Alg solution = TwoPhaseSolver.FindSolution(cube, timeout, returnLength, requiredLength);
            Console.WriteLine("Scramble: " + scramble + "\nSolution: " + solution + "\nLength: " + solution.Length);

            Assert.IsTrue(solution.Length <= 20);

            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.CreateSolved();

            result.ApplyAlg(scramble);
            result.ApplyAlg(solution);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomTest()
        {
            Random random = new Random(7777777);
            int repetitions = 20;

            TimeSpan timeout = TimeSpan.FromSeconds(1);
            int returnLength = 18;
            int requiredLength = 20;

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg scramble = Alg.FromRandomMoves(random.Next(20, 30), random);
                CubieCube cube = CubieCube.FromAlg(scramble);

                Alg solution = TwoPhaseSolver.FindSolution(cube, timeout, returnLength, requiredLength, solveDifferentOrientations: true, solveInverse: true);
                Console.WriteLine("\nScramble: " + scramble + "\nSolution: " + solution + "\nLength: " + solution.Length);

                Assert.IsTrue(solution.Length <= 20);

                CubieCube expected = CubieCube.CreateSolved();
                CubieCube result = CubieCube.CreateSolved();

                result.ApplyAlg(scramble);
                result.ApplyAlg(solution);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
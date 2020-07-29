using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class SinglePhaseSolverTest
    {
        [TestMethod]
        public void Test()
        {
            Alg testAlg1 = Alg.FromString("R U R' U'");
            Alg solution1 = SinglePhaseSolver.FindPhase1Solution(CubieCube.FromAlg(testAlg1));
            Assert.IsTrue(testAlg1.Length >= solution1.Length);
            Console.WriteLine(solution1);

            Alg testAlg2 = Alg.FromString("R U L D B F2 R' D2 R D' B2 F' D'");
            Alg solution2 = SinglePhaseSolver.FindPhase1Solution(CubieCube.FromAlg(testAlg2));
            Assert.IsTrue(testAlg2.Length >= solution2.Length);
            Console.WriteLine(solution2);

            Alg testAlg3 = Alg.FromString("R2 U' L2 D2 F2");
            Alg solution3 = SinglePhaseSolver.FindPhase2Solution(CubieCube.FromAlg(testAlg3));
            Console.WriteLine(solution3);

            Alg testAlg4 = Alg.FromString("R U2 B2 L' B' D2 U B F2 L F R'");
            CubieCube cube = CubieCube.FromAlg(testAlg4);
            Alg solution4 = SinglePhaseSolver.FindSolution(cube);
            Console.WriteLine(solution4);
        }
    }
}
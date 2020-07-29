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
        public void Test1(string algString)
        {
            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            TimeSpan timeout = TimeSpan.FromSeconds(10);
            int returnLength = 20;
            int requiredLength = -1;

            TwoPhaseSolver.alg = algString;
            Alg solution = TwoPhaseSolver.FindSolution(cube, timeout, returnLength, requiredLength);

            Console.WriteLine(solution);
        }
    }
}
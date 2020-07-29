using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CubingTests
{
    //TODO better test method names
    //TEST distribution
    [TestClass]
    public class PruneTablesTest
    {
        [DataRow("F2")]
        [DataRow("R")]
        [DataRow("L D B")]
        [DataRow("R U R' U'")]
        [DataRow("F R U R' U' F'")]
        [DataRow("R U R' U R U2 R'")]
        [DataTestMethod]
        public void Table1Test(string algString)
        {
            TableController.InitializePhase1PruningTable();

            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            int co = Coordinates.GetCoCoord(cube);
            int eo = Coordinates.GetEoCoord(cube);
            int equator = Coordinates.GetEquatorDistributionCoord(cube);

            int pruningIndex = Coordinates.GetPhase1PruningIndex(co, eo, equator);

            int pruningValue = TableController.Phase1PruningTable[pruningIndex];

            Assert.IsTrue(pruningValue <= alg.Length);
            Console.WriteLine(pruningValue);
        }

        [TestMethod]
        public void Table1SizeTest()
        {
            TableController.InitializePhase1PruningTable();

            Assert.AreEqual(TableController.Phase1PruningTable.Max(), 12);
        }

        [DataRow("F2")]
        [DataRow("U'")]
        [DataRow("R2 F2 U'")]
        [DataRow("L2 B2 F2 D2")]
        [DataRow("R2 D2 B2 U F2 U2 L2 D' U'")]
        [DataTestMethod]
        public void Table2Test(string algString)
        {
            TableController.InitializePhase2PruningTable();

            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            int cornerPermutation = Coordinates.GetCpCoord(cube);
            int equatorPermutation = Coordinates.GetEquatorPermutationCoord(cube);

            int pruningIndex = TwoPhaseConstants.NumEquatorPermutations * cornerPermutation + equatorPermutation;

            int pruningValue = TableController.Phase2PruningTable[pruningIndex];

            Assert.IsTrue(pruningValue <= alg.Length);
            Console.WriteLine(pruningValue);
        }

        [TestMethod]
        public void Table2SizeTest()
        {
            TableController.InitializePhase2PruningTable();

            //The phase 2 pruning table doesn't store the exact number of
            //moves needed to solve the case, the number stored is a minimum
            //number of moves needed
            Assert.IsTrue(TableController.Phase2PruningTable.Max() <= 18);
        }
    }
}
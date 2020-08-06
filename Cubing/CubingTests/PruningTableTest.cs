using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CubingTests
{
    //TEST distribution
    [TestClass]
    public class PruningTableTest
    {
        [DataRow("F2")]
        [DataRow("R")]
        [DataRow("L D B")]
        [DataRow("R U R' U'")]
        [DataRow("F R U R' U' F'")]
        [DataRow("R U R' U R U2 R'")]
        [DataTestMethod]
        public void Table1DataTest(string algString)
        {
            TableController.InitializePhase1PruningTable();

            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            int co = Coordinates.GetCoCoord(cube);
            int eo = Coordinates.GetEoCoord(cube);
            int equator = Coordinates.GetEquatorDistributionCoord(cube);

            int pruningIndex = PruningTables.GetPhase1PruningIndex(co, eo, equator);

            int pruningValue = TableController.Phase1PruningTable[pruningIndex];

            Assert.IsTrue(pruningValue <= alg.Length);
            Console.WriteLine(pruningValue);
        }

        [TestMethod]
        public void Table1Test()
        {
            TableController.InitializePhase1PruningTable();

            Assert.AreEqual(TableController.Phase1PruningTable.Length, TwoPhaseConstants.PruningTableSizePhase1);

            Assert.AreEqual(TableController.Phase1PruningTable.Max(), 12);
        }

        [DataRow("F2")]
        [DataRow("U'")]
        [DataRow("R2 F2 U'")]
        [DataRow("L2 B2 F2 D2")]
        [DataRow("R2 D2 B2 U F2 U2 L2 D' U'")]
        [DataTestMethod]
        public void CornerEquatorTable2DataTest(string algString)
        {
            TableController.InitializePhase2CornerEquatorPruningTable();

            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            int cornerPermutation = Coordinates.GetCpCoord(cube);
            int equatorPermutation = Coordinates.GetEquatorPermutationCoord(cube);

            int pruningIndex = TwoPhaseConstants.NumEquatorPermutations * cornerPermutation + equatorPermutation;

            int pruningValue = TableController.Phase2CornerEquatorPruningTable[pruningIndex];

            Assert.IsTrue(pruningValue <= alg.Length);
            Console.WriteLine(pruningValue);
        }

        [TestMethod]
        public void CornerEquatorTable2Test()
        {

            TableController.InitializePhase2CornerEquatorPruningTable();

            Assert.AreEqual(TableController.Phase2CornerEquatorPruningTable.Length, TwoPhaseConstants.CornerEquatorPruningTableSizePhase2);

            //The phase 2 pruning tables don't store the exact number of
            //moves needed to solve the case, the number stored is a minimum
            //number of moves needed
            Assert.IsTrue(TableController.Phase2CornerEquatorPruningTable.Max() <= 18);
        }

        [DataRow("F2")]
        [DataRow("U'")]
        [DataRow("R2 F2 U'")]
        [DataRow("L2 B2 F2 D2")]
        [DataRow("R2 D2 B2 U F2 U2 L2 D' U'")]
        [DataTestMethod]
        public void CornerUdTable2DataTest(string algString)
        {
            TableController.InitializePhase2CornerUdPruningTable();

            Alg alg = Alg.FromString(algString);
            CubieCube cube = CubieCube.FromAlg(alg);

            int udEdgePermutation = Coordinates.GetUdEdgePermutationCoord(cube);
            int cornerPermutation = Coordinates.GetCpCoord(cube);

            int pruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(udEdgePermutation, cornerPermutation);

            int pruningValue = TableController.Phase2CornerUdPruningTable[pruningIndex];

            Assert.IsTrue(pruningValue <= alg.Length);
            Console.WriteLine(pruningValue);
        }

        [TestMethod]
        public void CornerUdTable2Test()
        {
            TableController.InitializePhase2CornerUdPruningTable();

            Assert.AreEqual(TableController.Phase2CornerUdPruningTable.Length, TwoPhaseConstants.CornerUdPruningTableSizePhase2);

            //The phase 2 pruning tables don't store the exact number of
            //moves needed to solve the case, the number stored is a minimum
            //number of moves needed
            Assert.IsTrue(TableController.Phase2CornerUdPruningTable.Max() <= 18);
        }
    }
}
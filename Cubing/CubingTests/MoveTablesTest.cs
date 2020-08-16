using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class MoveTablesTest
    {
        [TestMethod]
        public void CreateCoMoveTableTest()
        {
            short[,] coMoveTable = MoveTables.CreateCornerOrientationMoveTable();

            Assert.IsNotNull(coMoveTable);

            Assert.AreEqual(coMoveTable.GetLength(0), Coordinates.NumCornerOrientations);
            Assert.AreEqual(coMoveTable.GetLength(1), Constants.NumMoves);

            for (int co = 0; co < Coordinates.NumCornerOrientations; co++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)coMoveTable[co, move] < Coordinates.NumCornerOrientations);
        }

        [TestMethod]
        public void CreateEoMoveTableTest()
        {
            short[,] eoMoveTable = MoveTables.CreateEdgeOrientationMoveTable();

            Assert.IsNotNull(eoMoveTable);

            Assert.AreEqual(eoMoveTable.GetLength(0), Coordinates.NumEdgeOrientations);
            Assert.AreEqual(eoMoveTable.GetLength(1), Constants.NumMoves);

            for (int eo = 0; eo < Coordinates.NumEdgeOrientations; eo++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)eoMoveTable[eo, move] < Coordinates.NumEdgeOrientations);
        }

        [TestMethod]
        public void CreateEquatorDistributionMoveTableTest()
        {
            short[,] equatorDistributionMoveTable = MoveTables.CreateEquatorDistributionMoveTable();

            Assert.IsNotNull(equatorDistributionMoveTable);

            Assert.AreEqual(equatorDistributionMoveTable.GetLength(0), Coordinates.NumEquatorDistributions);
            Assert.AreEqual(equatorDistributionMoveTable.GetLength(1), Constants.NumMoves);

            for (int equatorDistribution = 0; equatorDistribution < Coordinates.NumEquatorDistributions; equatorDistribution++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)equatorDistributionMoveTable[equatorDistribution, move] < Coordinates.NumEquatorDistributions);
        }

        [TestMethod]
        public void CreateEquatorPermutationMoveTableTest()
        {
            sbyte[,] equatorPermutationMoveTable = MoveTables.CreateEquatorOrderMoveTable();

            Assert.IsNotNull(equatorPermutationMoveTable);

            Assert.AreEqual(equatorPermutationMoveTable.GetLength(0), Coordinates.NumEquatorOrders);
            Assert.AreEqual(equatorPermutationMoveTable.GetLength(1), Constants.NumMoves);
        }

        [TestMethod]
        public void CreateCpMoveTableTest()
        {
            ushort[,] cpMoveTable = MoveTables.CreateCornerPermutationMoveTable();

            Assert.IsNotNull(cpMoveTable);

            Assert.AreEqual(cpMoveTable.GetLength(0), Coordinates.NumCornerPermutations);
            Assert.AreEqual(cpMoveTable.GetLength(1), Constants.NumMoves);

            for (int cp = 0; cp < Coordinates.NumCornerPermutations; cp++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue(cpMoveTable[cp, move] < Coordinates.NumCornerPermutations);
        }

        [TestMethod]
        public void CreateUdEdgeOrderMoveTableTest()
        {
            ushort[,] udEdgeOrderMoveTable = MoveTables.CreateUdEdgeOrderMoveTable();

            Assert.IsNotNull(udEdgeOrderMoveTable);

            Assert.AreEqual(udEdgeOrderMoveTable.GetLength(0), Coordinates.NumUdEdgeOrders);
            Assert.AreEqual(udEdgeOrderMoveTable.GetLength(1), TwoPhaseConstants.NumMovesPhase2);

            for (int udEdgeOrder = 0; udEdgeOrder < Coordinates.NumUdEdgeOrders; udEdgeOrder++)
                foreach (Move move in TwoPhaseConstants.Phase2Moves)
                    Assert.IsTrue(udEdgeOrderMoveTable[udEdgeOrder, MoveTables.Phase1IndexToPhase2Index[(int)move]] < Coordinates.NumUdEdgeOrders);
        }

        [TestMethod]
        public void TestAllPhase1()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            TableController.InitializeCornerOrientationMoveTable();
            TableController.InitializeCornerPermutationMoveTable();
            TableController.InitializeEdgeOrientationMoveTable();
            TableController.InitializeEquatorDistributionMoveTable();

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg randomMoves = Alg.FromRandomMoves(length, random);

                int resultCo = 0;
                int resultCp = 0;
                int resultEo = 0;
                int resultEquatorDistribution = 0;

                CubieCube cube = CubieCube.CreateSolved();

                for (int moveIndex = 0; moveIndex < length; moveIndex++)
                {
                    resultCo = TableController.CornerOrientationMoveTable[resultCo, (int)randomMoves[moveIndex]];
                    resultCp = TableController.CornerPermutationMoveTable[resultCp, (int)randomMoves[moveIndex]];
                    resultEo = TableController.EdgeOrientationMoveTable[resultEo, (int)randomMoves[moveIndex]];
                    resultEquatorDistribution = TableController.EquatorDistributionMoveTable[resultEquatorDistribution, (int)randomMoves[moveIndex]];

                    cube.ApplyMove(randomMoves[moveIndex]);
                }

                int expectedCo = Coordinates.GetCornerOrientation(cube);
                Assert.AreEqual(expectedCo, resultCo);

                int expectedCp = Coordinates.GetCornerPermutation(cube);
                Assert.AreEqual(expectedCp, resultCp);

                int expectedEo = Coordinates.GetEdgeOrientation(cube);
                Assert.AreEqual(expectedEo, resultEo);

                int expectedEquatorDistribution = Coordinates.GetEquatorDistribution(cube);
                Assert.AreEqual(expectedEquatorDistribution, resultEquatorDistribution);
            }
        }

        [TestMethod]
        public void TestAllPhase2()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            TableController.InitializeUdEdgeOrderMoveTable();
            TableController.InitializeEquatorOrderMoveTable();

            double[] moveProbabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d
            };

            int expectedEquatorPermutation;
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg randomMoves = Alg.FromRandomMoves(length, random, moveProbabilities);

                int resultUdEdgeOrder = 0;
                int resultEquatorPermutation = 0;

                CubieCube cube = CubieCube.CreateSolved();

                for (int moveIndex = 0; moveIndex < length; moveIndex++)
                {
                    resultUdEdgeOrder = TableController.UdEdgeOrderMoveTable[resultUdEdgeOrder, MoveTables.Phase1IndexToPhase2Index[(int)randomMoves[moveIndex]]];
                    resultEquatorPermutation = TableController.EquatorOrderMoveTable[resultEquatorPermutation, (int)randomMoves[moveIndex]];

                    cube.ApplyMove(randomMoves[moveIndex]);

                    expectedEquatorPermutation = Coordinates.GetEquatorOrder(cube);
                    Assert.AreEqual(expectedEquatorPermutation, resultEquatorPermutation);
                }

                int expectedUdEdgeOrder = Coordinates.GetUdEdgeOrder(cube);
                Assert.AreEqual(expectedUdEdgeOrder, resultUdEdgeOrder);

                expectedEquatorPermutation = Coordinates.GetEquatorOrder(cube);
                Assert.AreEqual(expectedEquatorPermutation, resultEquatorPermutation);
            }
        }
    }
}
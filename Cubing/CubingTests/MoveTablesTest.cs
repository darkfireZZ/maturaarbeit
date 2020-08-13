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
            short[,] coMoveTable = MoveTables.CreateCoMoveTable();

            Assert.IsNotNull(coMoveTable);

            Assert.AreEqual(coMoveTable.GetLength(0), Coordinates.NumCoCoords);
            Assert.AreEqual(coMoveTable.GetLength(1), Constants.NumMoves);

            for (int co = 0; co < Coordinates.NumCoCoords; co++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)coMoveTable[co, move] < Coordinates.NumCoCoords);
        }

        [TestMethod]
        public void CreateEoMoveTableTest()
        {
            short[,] eoMoveTable = MoveTables.CreateEoMoveTable();

            Assert.IsNotNull(eoMoveTable);

            Assert.AreEqual(eoMoveTable.GetLength(0), Coordinates.NumEoCoords);
            Assert.AreEqual(eoMoveTable.GetLength(1), Constants.NumMoves);

            for (int eo = 0; eo < Coordinates.NumEoCoords; eo++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)eoMoveTable[eo, move] < Coordinates.NumEoCoords);
        }

        [TestMethod]
        public void CreateEquatorDistributionMoveTableTest()
        {
            short[,] equatorDistributionMoveTable = MoveTables.CreateEquatorDistributionMoveTable();

            Assert.IsNotNull(equatorDistributionMoveTable);

            Assert.AreEqual(equatorDistributionMoveTable.GetLength(0), Coordinates.NumEquatorDistributionCoords);
            Assert.AreEqual(equatorDistributionMoveTable.GetLength(1), Constants.NumMoves);

            for (int equatorDistribution = 0; equatorDistribution < Coordinates.NumEquatorDistributionCoords; equatorDistribution++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue((uint)equatorDistributionMoveTable[equatorDistribution, move] < Coordinates.NumEquatorDistributionCoords);
        }

        [TestMethod]
        public void CreateEquatorPermutationMoveTableTest()
        {
            sbyte[,] equatorPermutationMoveTable = MoveTables.CreateEquatorPermutationMoveTable();

            Assert.IsNotNull(equatorPermutationMoveTable);

            Assert.AreEqual(equatorPermutationMoveTable.GetLength(0), Coordinates.NumEquatorPermutationCoords);
            Assert.AreEqual(equatorPermutationMoveTable.GetLength(1), Constants.NumMoves);
        }

        [TestMethod]
        public void CreateCpMoveTableTest()
        {
            ushort[,] cpMoveTable = MoveTables.CreateCpMoveTable();

            Assert.IsNotNull(cpMoveTable);

            Assert.AreEqual(cpMoveTable.GetLength(0), Coordinates.NumCpCoords);
            Assert.AreEqual(cpMoveTable.GetLength(1), Constants.NumMoves);

            for (int cp = 0; cp < Coordinates.NumCpCoords; cp++)
                for (int move = 0; move < Constants.NumMoves; move++)
                    Assert.IsTrue(cpMoveTable[cp, move] < Coordinates.NumCpCoords);
        }

        [TestMethod]
        public void CreateUdEdgePermutationMoveTableTest()
        {
            ushort[,] udEdgePermutationMoveTable = MoveTables.CreateUdEdgePermutationMoveTable();

            Assert.IsNotNull(udEdgePermutationMoveTable);

            Assert.AreEqual(udEdgePermutationMoveTable.GetLength(0), Coordinates.NumUdEdgePermutationCoords);
            Assert.AreEqual(udEdgePermutationMoveTable.GetLength(1), TwoPhaseConstants.NumMovesPhase2);

            for (int udEdgePermutation = 0; udEdgePermutation < Coordinates.NumUdEdgePermutationCoords; udEdgePermutation++)
                foreach (Move move in TwoPhaseConstants.Phase2Moves)
                    Assert.IsTrue(udEdgePermutationMoveTable[udEdgePermutation, MoveTables.Phase1IndexToPhase2Index[(int)move]] < Coordinates.NumUdEdgePermutationCoords);
        }

        [TestMethod]
        public void TestAllPhase1()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            TableController.InitializeCoMoveTable();
            TableController.InitializeCpMoveTable();
            TableController.InitializeEoMoveTable();
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
                    resultCo = TableController.CoMoveTable[resultCo, (int)randomMoves[moveIndex]];
                    resultCp = TableController.CpMoveTable[resultCp, (int)randomMoves[moveIndex]];
                    resultEo = TableController.EoMoveTable[resultEo, (int)randomMoves[moveIndex]];
                    resultEquatorDistribution = TableController.EquatorDistributionMoveTable[resultEquatorDistribution, (int)randomMoves[moveIndex]];

                    cube.ApplyMove(randomMoves[moveIndex]);
                }

                int expectedCo = Coordinates.GetCoCoord(cube);
                Assert.AreEqual(expectedCo, resultCo);

                int expectedCp = Coordinates.GetCpCoord(cube);
                Assert.AreEqual(expectedCp, resultCp);

                int expectedEo = Coordinates.GetEoCoord(cube);
                Assert.AreEqual(expectedEo, resultEo);

                int expectedEquatorDistribution = Coordinates.GetEquatorDistributionCoord(cube);
                Assert.AreEqual(expectedEquatorDistribution, resultEquatorDistribution);
            }
        }

        [TestMethod]
        public void TestAllPhase2()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            TableController.InitializeUdEdgePermutationMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();

            double[] moveProbabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d };

            int expectedEquatorPermutation;
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg randomMoves = Alg.FromRandomMoves(length, random, moveProbabilities);

                int resultUdEdgePermutation = 0;
                int resultEquatorPermutation = 0;

                CubieCube cube = CubieCube.CreateSolved();

                for (int moveIndex = 0; moveIndex < length; moveIndex++)
                {
                    resultUdEdgePermutation = TableController.UdEdgePermutationMoveTable[resultUdEdgePermutation, MoveTables.Phase1IndexToPhase2Index[(int)randomMoves[moveIndex]]];
                    resultEquatorPermutation = TableController.EquatorPermutationMoveTable[resultEquatorPermutation, (int)randomMoves[moveIndex]];

                    cube.ApplyMove(randomMoves[moveIndex]);

                    expectedEquatorPermutation = Coordinates.GetEquatorPermutationCoord(cube);
                    Assert.AreEqual(expectedEquatorPermutation, resultEquatorPermutation);
                }

                int expectedUdEdgePermutation = Coordinates.GetUdEdgePermutationCoord(cube);
                Assert.AreEqual(expectedUdEdgePermutation, resultUdEdgePermutation);

                expectedEquatorPermutation = Coordinates.GetEquatorPermutationCoord(cube);
                Assert.AreEqual(expectedEquatorPermutation, resultEquatorPermutation);
            }
        }
    }
}
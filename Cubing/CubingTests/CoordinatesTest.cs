using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class CoordinatesTest
    {
        [TestMethod]
        public void CoCoordTest() //Tests GetCOCoord, SetCOCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetCOCoord and SetCOCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetCornerOrientation(expected);
            Coordinates.SetCornerOrientation(result, coord);

            CollectionAssert.AreEqual(expected.CornerOrientation, result.CornerOrientation);

            //if solved orientation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetCornerOrientation(result, 0);

            CollectionAssert.AreEqual(expected.CornerOrientation, result.CornerOrientation);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetCornerOrientation(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetCornerOrientation(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetCornerOrientation(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCornerOrientation(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCornerOrientation(CubieCube.CreateSolved(), Coordinates.NumCornerOrientations));
        }

        [TestMethod]
        public void EoCoordTest() //Tests GetEOCoord, SetEOCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetEOCoord and SetEOCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetEdgeOrientation(expected);
            Coordinates.SetEdgeOrientation(result, coord);

            CollectionAssert.AreEqual(expected.EdgeOrientation, result.EdgeOrientation);

            //if solved orientation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEdgeOrientation(result, 0);

            CollectionAssert.AreEqual(expected.EdgeOrientation, result.EdgeOrientation);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetEdgeOrientation(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEdgeOrientation(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEdgeOrientation(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEdgeOrientation(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEdgeOrientation(CubieCube.CreateSolved(), Coordinates.NumEdgeOrientations));
        }

        [TestMethod]
        public void EquatorDistributionCoordTest() //Tests GetEquatorDistributionCoord, SetEquatorDistributionCoord
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            CubieCube expected;
            CubieCube result;
            int expectedCoord;
            int resultCoord;

            Move[] phase2Moves = { Move.R2, Move.L2, Move.F2, Move.B2, Move.U1, Move.U2, Move.U3, Move.D1, Move.D2, Move.D3 };

            IEnumerable<Move> randomPhase2Moves()
            {
                yield return phase2Moves[random.Next(0, phase2Moves.Length)];
            }

            //solved tests
            Assert.AreEqual(0, Coordinates.GetEquatorDistribution(CubieCube.CreateSolved()));

            expected = CubieCube.CreateSolved();

            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.FromAlg(randomPhase2Moves().Take(length));
                expectedCoord = Coordinates.GetEquatorDistribution(expected);
                resultCoord = Coordinates.GetEquatorDistribution(result);

                Assert.AreEqual(expectedCoord, resultCoord);
            }

            //scrambled tests
            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.CreateSolved();
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                expectedCoord = Coordinates.GetEquatorDistribution(expected);
                Coordinates.SetEquatorDistribution(result, expectedCoord);
                resultCoord = Coordinates.GetEquatorDistribution(result);
                Assert.AreEqual(expectedCoord, resultCoord);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEquatorDistribution(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEquatorDistribution(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorDistribution(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorDistribution(CubieCube.CreateSolved(), Coordinates.NumEquatorDistributions));
        }

        [TestMethod]
        public void EquatorPermutationCoordTest() //Tests GetEquatorPermutationCoord, SetEquatorPermutationCoord
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            //if solved permutation corresponds to the coordinate 0
            //SetEquatorPermutationCoord()
            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEquatorOrder(result, 0);
            CubingAssert.HaveEqualEquatorEdgePermutation(expected, result);

            expected = CubieCube.FromAlg(Alg.FromString("R2 L2"));
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEquatorOrder(result, Coordinates.NumEquatorOrders - 1);

            //GetEquatorPermutationCoord()
            result = CubieCube.CreateSolved();
            Assert.AreEqual(0, Coordinates.GetEquatorOrder(result));

            result.ApplyAlg(Alg.FromString("F' R' B' D' L2"));
            Assert.AreEqual(0, Coordinates.GetEquatorOrder(result));

            //apply B1
            int expectedCoord = 17;
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.B1);
            int resultCoord = Coordinates.GetEquatorOrder(cube);
            Assert.AreEqual(expectedCoord, resultCoord);

            //apply B2
            expectedCoord = 6;
            cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.B2);
            resultCoord = Coordinates.GetEquatorOrder(cube);
            Assert.AreEqual(expectedCoord, resultCoord);

            //if applying GetEquatorPermutationCoord() and SetEquatorPermutationCoord() results in the same array as at the beginning
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                result = CubieCube.CreateSolved();

                int coord = Coordinates.GetEquatorOrder(expected);
                Coordinates.SetEquatorOrder(result, coord);

                CubingAssert.HaveEqualEquatorEdgePermutation(expected, result);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEquatorOrder(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEquatorOrder(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorOrder(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorOrder(CubieCube.CreateSolved(), Coordinates.NumEquatorOrders));
        }

        [TestMethod]
        public void UEdgeCoordTest()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            CubieCube expected;
            CubieCube result;
            int expectedCoord;
            int resultCoord;

            double[] phase2probabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d };

            //solved tests
            Assert.AreEqual(0, Coordinates.GetUEdgeDistribution(CubieCube.CreateSolved()));

            //scrambled tests
            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.CreateSolved();
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                expectedCoord = Coordinates.GetUEdgePermutation(expected);
                Coordinates.SetUEdgePermutation(result, expectedCoord);
                resultCoord = Coordinates.GetUEdgePermutation(result);

                int expectedDistribution = expectedCoord / Coordinates.NumUEdgePermutations;
                int expectedPermutation = expectedCoord % Coordinates.NumUEdgePermutations;
                int resultDistribution = resultCoord / Coordinates.NumUEdgePermutations;
                int resultPermutation = resultCoord % Coordinates.NumUEdgePermutations;

                Assert.AreEqual(expectedDistribution, resultDistribution);
                Assert.AreEqual(expectedPermutation, resultPermutation);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetUEdgePermutation(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetUEdgePermutation(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUEdgePermutation(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUEdgePermutation(CubieCube.CreateSolved(), Coordinates.NumUEdgePermutations));
        }

        [TestMethod]
        public void CpCoordTest() //Tests GetCpCoord, SetCpCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetCpCoord and SetCpCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetCornerPermutation(expected);
            Coordinates.SetCornerPermutation(result, coord);

            CollectionAssert.AreEqual(expected.CornerPermutation, result.CornerPermutation);

            //apply R2 to a solved cube
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.R2);

            int expectedCoord = 36177;
            int resultCoord = Coordinates.GetCornerPermutation(cube);

            Assert.AreEqual(expectedCoord, resultCoord);

            expected = CubieCube.CreateSolved();
            expected.ApplyMove(Move.R2);

            result = CubieCube.CreateSolved();
            Coordinates.SetCornerPermutation(result, expectedCoord);

            CollectionAssert.AreEqual(expected.CornerPermutation, result.CornerPermutation);

            //if solved permutation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetCornerPermutation(result, 0);

            CollectionAssert.AreEqual(expected.CornerPermutation, result.CornerPermutation);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetCornerPermutation(result));

            //example from http://kociemba.org/math/coordlevel
            Corner[] cp = new Corner[] { Corner.DFR, Corner.UFL, Corner.ULB, Corner.URF, Corner.DRB, Corner.DLF, Corner.DBL, Corner.UBR };
            cube = CubieCube.Create(cp, CubieCube.SolvedCO, CubieCube.SolvedEP, CubieCube.SolvedEO, CubieCube.SolvedCenters);
            resultCoord = Coordinates.GetCornerPermutation(cube);
            expectedCoord = 21021;
            Assert.AreEqual(expectedCoord, resultCoord);

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetCornerPermutation(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetCornerPermutation(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCornerPermutation(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCornerPermutation(CubieCube.CreateSolved(), Coordinates.NumCornerPermutations));
        }

        [TestMethod]
        public void EpCoordTest() //Tests GetCpCoord, SetCpCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetEpCoord and SetEpCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetEdgePermutation(expected);
            Coordinates.SetEdgePermutation(result, coord);

            CollectionAssert.AreEqual(expected.EdgePermutation, result.EdgePermutation);

            //apply R2 to a solved cube
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.R2);

            int expectedCoord = 123763104;
            int resultCoord = Coordinates.GetEdgePermutation(cube);

            Assert.AreEqual(expectedCoord, resultCoord);

            expected = CubieCube.CreateSolved();
            expected.ApplyMove(Move.R2);

            result = CubieCube.CreateSolved();
            Coordinates.SetEdgePermutation(result, expectedCoord);

            CollectionAssert.AreEqual(expected.EdgePermutation, result.EdgePermutation);

            //if solved permutation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEdgePermutation(result, 0);

            CollectionAssert.AreEqual(expected.EdgePermutation, result.EdgePermutation);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetEdgePermutation(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEdgePermutation(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEdgePermutation(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEdgePermutation(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEdgePermutation(CubieCube.CreateSolved(), Coordinates.NumEdgePermutations));
        }

        [TestMethod]
        public void UdEdgeOrderCoordTest() //Tests GetUdEdgePermutationCoord, SetUdEdgePermutationCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            double[] phase2probabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d };

            //if solved permutation corresponds to the coordinate 0
            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random, phase2probabilities));
            Coordinates.SetUdEdgeOrder(result, 0);

            CollectionAssert.AreEqual(expected.EdgePermutation.Take(8).ToArray(), result.EdgePermutation.Take(8).ToArray());

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetUdEdgeOrder(result));

            //if applying GetUdEdgePermutationCoord and
            //SetUdEdgePermutationCoord results in the same array as at the
            //beginning
            Alg randomAlg = Alg.FromRandomMoves(length, random, phase2probabilities);
            expected = CubieCube.FromAlg(randomAlg);
            result = CubieCube.CreateSolved();

            int coord = Coordinates.GetUdEdgeOrder(expected);
            Coordinates.SetUdEdgeOrder(result, coord);

            CollectionAssert.AreEqual(expected.EdgePermutation.Take(8).ToArray(), result.EdgePermutation.Take(8).ToArray());

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetUdEdgeOrder(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetUdEdgeOrder(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUdEdgeOrder(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUdEdgeOrder(CubieCube.CreateSolved(), Coordinates.NumUdEdgeOrders));
        }

        [TestMethod]
        public void CombineUAndDEdgeCoordsTest()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            double[] phase2probabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d };

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg alg = Alg.FromRandomMoves(length, random, phase2probabilities);
                CubieCube cube = CubieCube.FromAlg(alg);

                int uEdgeCoord = Coordinates.GetUEdgePermutation(cube);
                int dEdgeCoord = Coordinates.GetDEdgePermutation(cube);
                int result = Coordinates.CombineUEdgePermutationAndDEdgeOrder(uEdgeCoord, dEdgeCoord % Coordinates.NumDEdgeOrders);
                int expected = Coordinates.GetUdEdgeOrder(cube);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
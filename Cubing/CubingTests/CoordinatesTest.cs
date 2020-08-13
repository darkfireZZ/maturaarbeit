using Cubing.ThreeByThree;
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

            int coord = Coordinates.GetCoCoord(expected);
            Coordinates.SetCoCoord(result, coord);

            CollectionAssert.AreEqual(expected.CO, result.CO);

            //if solved orientation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetCoCoord(result, 0);

            CollectionAssert.AreEqual(expected.CO, result.CO);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetCoCoord(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetCoCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetCoCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCoCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCoCoord(CubieCube.CreateSolved(), Coordinates.NumCoCoords));
        }

        [TestMethod]
        public void EoCoordTest() //Tests GetEOCoord, SetEOCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetEOCoord and SetEOCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetEoCoord(expected);
            Coordinates.SetEoCoord(result, coord);

            CollectionAssert.AreEqual(expected.EO, result.EO);

            //if solved orientation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEoCoord(result, 0);

            CollectionAssert.AreEqual(expected.EO, result.EO);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetEoCoord(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEoCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEoCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEoCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEoCoord(CubieCube.CreateSolved(), Coordinates.NumEoCoords));
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
            Assert.AreEqual(0, Coordinates.GetEquatorDistributionCoord(CubieCube.CreateSolved()));

            expected = CubieCube.CreateSolved();

            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.FromAlg(randomPhase2Moves().Take(length));
                expectedCoord = Coordinates.GetEquatorDistributionCoord(expected);
                resultCoord = Coordinates.GetEquatorDistributionCoord(result);

                Assert.AreEqual(expectedCoord, resultCoord);
            }

            //scrambled tests
            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.CreateSolved();
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                expectedCoord = Coordinates.GetEquatorDistributionCoord(expected);
                Coordinates.SetEquatorDistributionCoord(result, expectedCoord);
                resultCoord = Coordinates.GetEquatorDistributionCoord(result);
                Assert.AreEqual(expectedCoord, resultCoord);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEquatorDistributionCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEquatorDistributionCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorDistributionCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorDistributionCoord(CubieCube.CreateSolved(), Coordinates.NumEquatorDistributionCoords));
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
            Coordinates.SetEquatorPermutationCoord(result, 0);
            CubingAssert.HaveEqualEquatorEdgePermutation(expected, result);

            expected = CubieCube.FromAlg(Alg.FromString("R2 L2"));
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEquatorPermutationCoord(result, Coordinates.NumEquatorPermutationCoords - 1);

            //GetEquatorPermutationCoord()
            result = CubieCube.CreateSolved();
            Assert.AreEqual(0, Coordinates.GetEquatorPermutationCoord(result));

            result.ApplyAlg(Alg.FromString("F' R' B' D' L2"));
            Assert.AreEqual(0, Coordinates.GetEquatorPermutationCoord(result));

            //apply B1
            int expectedCoord = 17;
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.B1);
            int resultCoord = Coordinates.GetEquatorPermutationCoord(cube);
            Assert.AreEqual(expectedCoord, resultCoord);

            //apply B2
            expectedCoord = 6;
            cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.B2);
            resultCoord = Coordinates.GetEquatorPermutationCoord(cube);
            Assert.AreEqual(expectedCoord, resultCoord);

            //if applying GetEquatorPermutationCoord() and SetEquatorPermutationCoord() results in the same array as at the beginning
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                result = CubieCube.CreateSolved();

                int coord = Coordinates.GetEquatorPermutationCoord(expected);
                Coordinates.SetEquatorPermutationCoord(result, coord);

                CubingAssert.HaveEqualEquatorEdgePermutation(expected, result);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEquatorPermutationCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEquatorPermutationCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorPermutationCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEquatorPermutationCoord(CubieCube.CreateSolved(), Coordinates.NumEquatorPermutationCoords));
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
            Assert.AreEqual(0, Coordinates.GetUEdgeDistributionCoord(CubieCube.CreateSolved()));

            //scrambled tests
            for (int i = 0; i < repetitions; i++)
            {
                result = CubieCube.CreateSolved();
                expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                expectedCoord = Coordinates.GetUEdgeCoord(expected);
                Coordinates.SetUEdgeCoord(result, expectedCoord);
                resultCoord = Coordinates.GetUEdgeCoord(result);

                int expectedDistribution = expectedCoord / Coordinates.NumUEdgePermutationCoords;
                int expectedPermutation = expectedCoord % Coordinates.NumUEdgePermutationCoords;
                int resultDistribution = resultCoord / Coordinates.NumUEdgePermutationCoords;
                int resultPermutation = resultCoord % Coordinates.NumUEdgePermutationCoords;

                Assert.AreEqual(expectedDistribution, resultDistribution);
                Assert.AreEqual(expectedPermutation, resultPermutation);
            }

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetUEdgeCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetUEdgeCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUEdgeCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUEdgeCoord(CubieCube.CreateSolved(), Coordinates.NumUEdgeCoordsPhase1));
        }

        [TestMethod]
        public void CpCoordTest() //Tests GetCpCoord, SetCpCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetCpCoord and SetCpCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetCpCoord(expected);
            Coordinates.SetCpCoord(result, coord);

            CollectionAssert.AreEqual(expected.CP, result.CP);

            //apply R2 to a solved cube
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.R2);

            int expectedCoord = 36177;
            int resultCoord = Coordinates.GetCpCoord(cube);

            Assert.AreEqual(expectedCoord, resultCoord);

            expected = CubieCube.CreateSolved();
            expected.ApplyMove(Move.R2);

            result = CubieCube.CreateSolved();
            Coordinates.SetCpCoord(result, expectedCoord);

            CollectionAssert.AreEqual(expected.CP, result.CP);

            //if solved permutation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetCpCoord(result, 0);

            CollectionAssert.AreEqual(expected.CP, result.CP);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetCpCoord(result));

            //example from http://kociemba.org/math/coordlevel
            Corner[] cp = new Corner[] { Corner.DFR, Corner.UFL, Corner.ULB, Corner.URF, Corner.DRB, Corner.DLF, Corner.DBL, Corner.UBR };
            cube = CubieCube.Create(cp, CubieCube.SolvedCO, CubieCube.SolvedEP, CubieCube.SolvedEO, CubieCube.SolvedCenters);
            resultCoord = Coordinates.GetCpCoord(cube);
            expectedCoord = 21021;
            Assert.AreEqual(expectedCoord, resultCoord);

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetCpCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetCpCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCpCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetCpCoord(CubieCube.CreateSolved(), Coordinates.NumCpCoords));
        }

        [TestMethod]
        public void EpCoordTest() //Tests GetCpCoord, SetCpCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            //if applying GetEpCoord and SetEpCoord results in the same array as at the beginning
            CubieCube expected = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            CubieCube result = CubieCube.CreateSolved();

            int coord = Coordinates.GetEpCoord(expected);
            Coordinates.SetEpCoord(result, coord);

            CollectionAssert.AreEqual(expected.EP, result.EP);

            //apply R2 to a solved cube
            CubieCube cube = CubieCube.CreateSolved();
            cube.ApplyMove(Move.R2);

            int expectedCoord = 123763104;
            int resultCoord = Coordinates.GetEpCoord(cube);

            Assert.AreEqual(expectedCoord, resultCoord);

            expected = CubieCube.CreateSolved();
            expected.ApplyMove(Move.R2);

            result = CubieCube.CreateSolved();
            Coordinates.SetEpCoord(result, expectedCoord);

            CollectionAssert.AreEqual(expected.EP, result.EP);

            //if solved permutation corresponds to the coordinate 0
            expected = CubieCube.CreateSolved();
            result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
            Coordinates.SetEpCoord(result, 0);

            CollectionAssert.AreEqual(expected.EP, result.EP);

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetEpCoord(result));

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetEpCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetEpCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEpCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetEpCoord(CubieCube.CreateSolved(), Coordinates.NumEpCoords));
        }

        [TestMethod]
        public void UdEdgePermutationCoordTest() //Tests GetUdEdgePermutationCoord, SetUdEdgePermutationCoord
        {
            Random random = new Random(7777777);
            int length = 50;

            double[] phase2probabilities = {
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d,
                0d, 1d, 0d, 1d, 1d, 1d, 0d, 1d, 0d };

            //if solved permutation corresponds to the coordinate 0
            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.FromAlg(Alg.FromRandomMoves(length, random, phase2probabilities));
            Coordinates.SetUdEdgePermutationCoord(result, 0);

            CollectionAssert.AreEqual(expected.EP.Take(8).ToArray(), result.EP.Take(8).ToArray());

            result = CubieCube.CreateSolved();

            Assert.AreEqual(0, Coordinates.GetUdEdgePermutationCoord(result));

            //if applying GetUdEdgePermutationCoord and
            //SetUdEdgePermutationCoord results in the same array as at the
            //beginning
            Alg randomAlg = Alg.FromRandomMoves(length, random, phase2probabilities);
            expected = CubieCube.FromAlg(randomAlg);
            result = CubieCube.CreateSolved();

            int coord = Coordinates.GetUdEdgePermutationCoord(expected);
            Coordinates.SetUdEdgePermutationCoord(result, coord);

            CollectionAssert.AreEqual(expected.EP.Take(8).ToArray(), result.EP.Take(8).ToArray());

            //exceptions
            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.GetUdEdgePermutationCoord(null));

            Assert.ThrowsException<ArgumentNullException>(() => Coordinates.SetUdEdgePermutationCoord(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUdEdgePermutationCoord(CubieCube.CreateSolved(), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Coordinates.SetUdEdgePermutationCoord(CubieCube.CreateSolved(), Coordinates.NumUdEdgePermutationCoords));
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

                int uEdgeCoord = Coordinates.GetUEdgeCoord(cube);
                int dEdgeCoord = Coordinates.GetDEdgeCoord(cube);
                int result = Coordinates.CombineUAndDEdgePermutation(uEdgeCoord, dEdgeCoord);
                int expected = Coordinates.GetUdEdgePermutationCoord(cube);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Cubing.ThreeByThree.Constants;

namespace CubingTests
{
    [TestClass]
    public class CubieCubeTest
    {
        public void MoveReplacementTest(Action<CubieCube, CubieCube> test)
        {
            Alg uReplacementAlg = Alg.FromString("R L' F2 B2 R L' D L R' B2 F2 L R'");

            foreach (Move move in Enum.GetValues(typeof(Move)))
            {
                foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
                    if (Face.U.Rotate(rotation) == move.Face())
                    {
                        CubieCube result = CubieCube.CreateSolved();
                        result.ApplyMove(move);
                        CubieCube expected = CubieCube.CreateSolved();
                        expected.ApplyAlg(uReplacementAlg.Rotate(rotation) * move.QuarterTurns());

                        test(expected, result);
                    }
            }
        }

        public void SexyMoveTest(Action<CubieCube, CubieCube> test)
        {
            Alg sexyMove = Alg.FromString("R U R' U'");
            foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
            {
                CubieCube expected = CubieCube.CreateSolved();
                CubieCube result = CubieCube.CreateSolved();
                result.ApplyAlg(sexyMove.Rotate(rotation) * 6);

                test(expected, result);
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            Assert.AreEqual(
                CubieCube.CreateSolved(), CubieCube.Create(
                    CubieCube.SolvedCP, CubieCube.SolvedCO,
                    CubieCube.SolvedEP, CubieCube.SolvedEO,
                    CubieCube.SolvedCenters));

            #region test exceptions
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.Create(null, CubieCube.SolvedCO,
                        CubieCube.SolvedEP, CubieCube.SolvedEO,
                        CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.Create(CubieCube.SolvedCP, null,
                        CubieCube.SolvedEP, CubieCube.SolvedEO,
                        CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, null, CubieCube.SolvedEO,
                        CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, CubieCube.SolvedEP, null,
                        CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, CubieCube.SolvedEP,
                        CubieCube.SolvedEO, null));

            Assert.ThrowsException<ArgumentException>(()
                => CubieCube.Create(new Corner[NumCorners + 1],
                        CubieCube.SolvedCO, CubieCube.SolvedEP,
                        CubieCube.SolvedEO, CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        new int[NumCorners + 1], CubieCube.SolvedEP,
                        CubieCube.SolvedEO, CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, new Edge[NumEdges + 1],
                        CubieCube.SolvedEO, CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, CubieCube.SolvedEP,
                        new int[NumEdges + 1], CubieCube.SolvedCenters));
            Assert.ThrowsException<ArgumentException>(()
                => CubieCube.Create(CubieCube.SolvedCP,
                        CubieCube.SolvedCO, CubieCube.SolvedEP,
                        CubieCube.SolvedEO, new Face[NumFaces + 1]));
            #endregion test exceptions
        }

        [TestMethod]
        public void CreateRandomTest()
        {
            Random random = new Random(7777777);
            int numIterations = 50;

            TimeSpan timeout = TimeSpan.FromSeconds(20);

            for (int iteration = 0; iteration < numIterations; iteration++)
            {
                CubieCube cube = CubieCube.CreateRandom(random);
                int cornerPermutation = Coordinates.GetCpCoord(cube);
                int edgePermutation   = Coordinates.GetEpCoord(cube);
                bool cornerParity = Coordinates.CpCoordinateParity(cornerPermutation);
                bool edgeParity   = Coordinates.EpCoordinateParity(edgePermutation);
                Assert.IsTrue(cornerParity == edgeParity);

                Assert.IsNotNull(TwoPhaseSolver.FindSolution(cube, timeout, 30, -1));
            }
        }

        [TestMethod]
        public void ApplyMovesTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Alg alg = Alg.FromRandomMoves(length, random);

            CubieCube expected = CubieCube.CreateSolved();
            expected.ApplyAlg(alg);

            CubieCube result = CubieCube.CreateSolved();
            foreach (Move move in alg)
                result.ApplyMove(move);

            Assert.AreEqual(expected, result);
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.CreateSolved().ApplyAlg(null));
        }

        [TestMethod]
        public void ApplyMoveTest()
        {
            //CP
            MoveReplacementTest((expected, result) =>
                CollectionAssert.AreEqual(expected.CP, result.CP));
            SexyMoveTest((expected, result) =>
                CollectionAssert.AreEqual(expected.CP, result.CP));
            //CO
            MoveReplacementTest((expected, result) =>
                CollectionAssert.AreEqual(expected.CO, result.CO));
            SexyMoveTest((expected, result) =>
                CollectionAssert.AreEqual(expected.CO, result.CO));
            //EP
            MoveReplacementTest((expected, result) =>
                CollectionAssert.AreEqual(expected.EP, result.EP));
            SexyMoveTest((expected, result) =>
                CollectionAssert.AreEqual(expected.EP, result.EP));
            //EO
            MoveReplacementTest((expected, result) =>
                CollectionAssert.AreEqual(expected.EO, result.EO));
            SexyMoveTest((expected, result) =>
                CollectionAssert.AreEqual(expected.EO, result.EO));
        }

        [TestMethod]
        public void RotateTest()
        {
            CubieCube expected, result;

            //repeating the same rotation will eventually lead to the
            //initial state again
            expected = CubieCube.CreateSolved();
            result = CubieCube.CreateSolved();
            foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
            {
                int reps = rotation.QuarterTurns() == 2 ? 2 : 4;
                for (int i = 0; i < reps; i++)
                    result.Rotate(rotation);
                Assert.AreEqual(expected, result);
            }

            //repeating the same two rotations will eventually lead to the
            //initial state again
            expected = CubieCube.CreateSolved();
            result = CubieCube.CreateSolved();
            foreach (Rotation rotation1 in Enum.GetValues(typeof(Rotation)))
                foreach (Rotation rotation2 in Enum.GetValues(typeof(Rotation)))
                {
                    if (rotation1.Axis() == rotation2.Axis()) //already tested
                        continue;

                    int reps = (rotation1.QuarterTurns() == 2
                                || rotation2.QuarterTurns() == 2)
                                ? 2 : 3;
                    for (int i = 0; i < reps; i++)
                    {
                        result.Rotate(rotation1);
                        result.Rotate(rotation2);
                    }

                    Assert.AreEqual(expected, result);
                }

            //a move and a rotation on the same face are commutative
            expected = CubieCube.CreateSolved();
            result = CubieCube.CreateSolved();
            foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
                for (int face = 0; face < 2; face++)
                    for (int qts = 1; qts <=3; qts++)
                    {
                        Move move = Moves.MoveFromFaceAndQuarterTurns(
                            (Face)((int)rotation.Axis() + face * 3), qts);

                        result.ApplyMove(move);
                        result.Rotate(rotation);
                        result.ApplyMove(move.Inverse());
                        result.Rotate(rotation.Inverse());

                        Assert.AreEqual(expected, result);
                    }

            //make sure that (x y x' == z)
            //here because this used to be a problem
            expected = CubieCube.RotationsArray[(int)Rotation.z1];
            result = CubieCube.CreateSolved();
            result.Rotate(Rotation.x1);
            result.Rotate(Rotation.y1);
            result.Rotate(Rotation.x3);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void MultiplyTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Alg alg = Alg.FromRandomMoves(length, random);

            CubieCube expected = CubieCube.CreateSolved();
            CubieCube result = CubieCube.CreateSolved();

            foreach (Move move in alg)
            {
                expected.ApplyMove(move);
                CubieCube moveCube = CubieCube.CreateSolved();
                moveCube.ApplyMove(move);
                result.Multiply(moveCube);
            }

            Assert.AreEqual(expected, result);
            Assert.ThrowsException<ArgumentNullException>(()
                => CubieCube.CreateSolved().Multiply(null));
        }

        [TestMethod]
        public void CloneTest()
        {
            CubieCube cube = CubieCube.CreateSolved();
            CubieCube clone = cube.Clone();

            Assert.IsTrue(cube == clone);

            CollectionAssert.AreEqual(cube.CP, clone.CP);
            CollectionAssert.AreEqual(cube.CO, clone.CO);
            CollectionAssert.AreEqual(cube.EO, clone.EO);
            CollectionAssert.AreEqual(cube.EP, clone.EP);

            Assert.IsFalse(ReferenceEquals(cube, clone));

            Assert.IsFalse(ReferenceEquals(cube.CP, clone.CP));
            Assert.IsFalse(ReferenceEquals(cube.CO, clone.CO));
            Assert.IsFalse(ReferenceEquals(cube.EO, clone.EO));
            Assert.IsFalse(ReferenceEquals(cube.EP, clone.EP));
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            Assert.ThrowsException<NotImplementedException>(()
                => CubieCube.CreateSolved().GetHashCode());
        }

        [TestMethod]
        public void Inverse()
        {
            CubieCube expected = CubieCube.CreateSolved();
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Alg alg = Alg.FromRandomMoves(length, random);
                CubieCube result = CubieCube.FromAlg(alg);
                result.Inverse();
                result.ApplyAlg(alg);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
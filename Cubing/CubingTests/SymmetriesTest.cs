using static Cubing.ThreeByThree.Constants;
using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubingTests
{
    [TestClass]
    public class SymmetriesTest
    {
        [TestMethod]
        public void PermutedCubesTest()
        {
            //make sure that every cube is unique and no cube is null
            for (int index = 0; index < Symmetries.NumSymmetries;
                index++)
            {
                Assert.IsNotNull(Symmetries.SymmetryCubes[index]);
                for (int prev = 0; prev < index; prev++)
                    Assert.AreNotEqual(Symmetries.SymmetryCubes[prev],
                        Symmetries.SymmetryCubes[index]);
            }

            CubieCube expected = CubieCube.CreateSolved();
            expected.Mirror(Axis.x);
            Assert.AreEqual(expected, Symmetries.SymmetryCubes[1]);

            expected = CubieCube.CreateSolved();
            expected.Rotate(Rotation.y1);
            Assert.AreEqual(expected, Symmetries.SymmetryCubes[2]);

            expected = CubieCube.CreateSolved();
            expected.Rotate(Rotation.z2);
            Assert.AreEqual(expected, Symmetries.SymmetryCubes[8]);

            expected = CubieCube.CreateSolved();
            expected.Rotate(Rotation.x1);
            expected.Rotate(Rotation.y1);
            Assert.AreEqual(expected, Symmetries.SymmetryCubes[16]);
        }

        [TestMethod]
        public void MultiplyTest()
        {
            for (int cp1 = 0; cp1 < Symmetries.NumSymmetries; cp1++)
                for (int cp2 = 0; cp2 < Symmetries.NumSymmetries; cp2++)
                {
                    CubieCube expected
                        = Symmetries.SymmetryCubes[cp1].Clone();
                    expected.Multiply(Symmetries.SymmetryCubes[cp2]);

                    var sexpected = Symmetries.SymmetryCubes[21];
                    var sresult = Symmetries.SymmetryCubes[1].Clone();
                    sresult.Multiply(Symmetries.SymmetryCubes[16]);

                    Assert.AreEqual(sexpected, sresult);

                    CubieCube result = Symmetries.SymmetryCubes[
                        Symmetries.MultiplyIndex[cp1, cp2]];

                    Assert.AreEqual(expected, result);
                }
        }

        [TestMethod]
        public void InverseIndexTest()
        {
            for (int index = 0; index < Symmetries.NumSymmetries; index++)
            {
                CubieCube permCube = Symmetries.SymmetryCubes[index];
                CubieCube invCube = Symmetries.SymmetryCubes[Symmetries.InverseIndex[index]];
                CubieCube solved = CubieCube.CreateSolved();

                CubieCube test1 = permCube.Clone();
                test1.Multiply(invCube);
                Assert.AreEqual(solved, test1);

                CubieCube test2 = invCube.Clone();
                test2.Multiply(permCube);
                Assert.AreEqual(solved, test2);
            }
        }

        [TestMethod]
        public void RotationIndexTest()
        {
            for (int rotation = 0; rotation < NumRotations; rotation++)
                Assert.AreEqual(CubieCube.RotationsArray[rotation],
                    Symmetries.SymmetryCubes[
                        Symmetries.RotationIndex[rotation]]);
        }

        [TestMethod]
        public void MirrorIndexTest()
        {
            for (int axis = 0; axis < NumAxes; axis++)
                Assert.AreEqual(CubieCube.MirrorsArray[axis],
                    Symmetries.SymmetryCubes[
                        Symmetries.MirrorIndex[axis]]);
        }
    }
}
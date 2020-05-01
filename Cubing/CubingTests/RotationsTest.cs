using static Cubing.ThreeByThree.Constants;
using Cubing.ThreeByThree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class RotationsTest
    {
        [TestMethod]
        public void Test()
        {
            Rotation[] rotations = {
                Rotation.x1, Rotation.x2, Rotation.x3,
                Rotation.y1, Rotation.y2, Rotation.y3,
                Rotation.z1, Rotation.z2, Rotation.z3 };

            for (int rotation = 0; rotation < NumRotations; rotation++)
                Assert.AreEqual((Rotation)rotation, rotations[rotation]);

            Assert.AreEqual(NumRotations, Enum.GetValues(typeof(Rotation)).Length);
        }

        [TestMethod]
        public void RotationToStringTest()
        {
            Assert.AreEqual("x",  Rotations.RotationToString(Rotation.x1));
            Assert.AreEqual("x2", Rotations.RotationToString(Rotation.x2));
            Assert.AreEqual("x'", Rotations.RotationToString(Rotation.x3));
            Assert.AreEqual("y",  Rotations.RotationToString(Rotation.y1));
            Assert.AreEqual("y2", Rotations.RotationToString(Rotation.y2));
            Assert.AreEqual("y'", Rotations.RotationToString(Rotation.y3));
            Assert.AreEqual("z",  Rotations.RotationToString(Rotation.z1));
            Assert.AreEqual("z2", Rotations.RotationToString(Rotation.z2));
            Assert.AreEqual("z'", Rotations.RotationToString(Rotation.z3));
        }

        [TestMethod]
        public void RotationFromStringTest()
        {
            Assert.AreEqual(Rotation.x1, Rotations.RotationFromString("x"));
            Assert.AreEqual(Rotation.x2, Rotations.RotationFromString("x2"));
            Assert.AreEqual(Rotation.x3, Rotations.RotationFromString("x'"));
            Assert.AreEqual(Rotation.y1, Rotations.RotationFromString("y"));
            Assert.AreEqual(Rotation.y2, Rotations.RotationFromString("y2"));
            Assert.AreEqual(Rotation.y3, Rotations.RotationFromString("y'"));
            Assert.AreEqual(Rotation.z1, Rotations.RotationFromString("z"));
            Assert.AreEqual(Rotation.z2, Rotations.RotationFromString("z2"));
            Assert.AreEqual(Rotation.z3, Rotations.RotationFromString("z'"));

            Assert.ThrowsException<ArgumentException>(()
                => Rotations.RotationFromString("Exc"));
            Assert.ThrowsException<ArgumentNullException>(()
                => Rotations.RotationFromString(null));
        }

        [TestMethod]
        public void AxisTest()
        {
            Assert.AreEqual(Axis.x, Rotation.x1.Axis());
            Assert.AreEqual(Axis.x, Rotation.x2.Axis());
            Assert.AreEqual(Axis.x, Rotation.x3.Axis());
            Assert.AreEqual(Axis.y, Rotation.y1.Axis());
            Assert.AreEqual(Axis.y, Rotation.y2.Axis());
            Assert.AreEqual(Axis.y, Rotation.y3.Axis());
            Assert.AreEqual(Axis.z, Rotation.z1.Axis());
            Assert.AreEqual(Axis.z, Rotation.z2.Axis());
            Assert.AreEqual(Axis.z, Rotation.z3.Axis());
        }

        [TestMethod]
        public void QuarterTurnsTest()
        {
            Assert.AreEqual(1, Rotation.x1.QuarterTurns());
            Assert.AreEqual(2, Rotation.x2.QuarterTurns());
            Assert.AreEqual(3, Rotation.x3.QuarterTurns());
            Assert.AreEqual(1, Rotation.y1.QuarterTurns());
            Assert.AreEqual(2, Rotation.y2.QuarterTurns());
            Assert.AreEqual(3, Rotation.y3.QuarterTurns());
            Assert.AreEqual(1, Rotation.z1.QuarterTurns());
            Assert.AreEqual(2, Rotation.z2.QuarterTurns());
            Assert.AreEqual(3, Rotation.z3.QuarterTurns());
        }
    }
}
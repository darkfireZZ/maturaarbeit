using static Cubing.ThreeByThree.Faces;
using Cubing.ThreeByThree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class FacesTest
    {
        [TestMethod]
        public void FaceToStringTest()
        {
            Assert.AreEqual("R", FaceToString(Face.R));
            Assert.AreEqual("U", FaceToString(Face.U));
            Assert.AreEqual("F", FaceToString(Face.F));
            Assert.AreEqual("L", FaceToString(Face.L));
            Assert.AreEqual("D", FaceToString(Face.D));
            Assert.AreEqual("B", FaceToString(Face.B));
        }

        [TestMethod]
        public void FaceFromStringTest()
        {
            Assert.AreEqual(Face.R, FaceFromString(FaceToString(Face.R)));
            Assert.AreEqual(Face.U, FaceFromString(FaceToString(Face.U)));
            Assert.AreEqual(Face.F, FaceFromString(FaceToString(Face.F)));
            Assert.AreEqual(Face.L, FaceFromString(FaceToString(Face.L)));
            Assert.AreEqual(Face.D, FaceFromString(FaceToString(Face.D)));
            Assert.AreEqual(Face.B, FaceFromString(FaceToString(Face.B)));

            Assert.ThrowsException<ArgumentNullException>(() => FaceFromString(null));
            Assert.ThrowsException<ArgumentException>(() => FaceFromString("Exc"));
        }

        [TestMethod]
        public void AxisTest()
        {
            Assert.AreEqual(Axis.x, Face.R.Axis());
            Assert.AreEqual(Axis.y, Face.U.Axis());
            Assert.AreEqual(Axis.z, Face.F.Axis());
            Assert.AreEqual(Axis.x, Face.L.Axis());
            Assert.AreEqual(Axis.y, Face.D.Axis());
            Assert.AreEqual(Axis.z, Face.B.Axis());
        }

        [TestMethod]
        public void OppositeFaceTest()
        {
            Assert.AreEqual(Face.L, Face.R.OppositeFace());
            Assert.AreEqual(Face.D, Face.U.OppositeFace());
            Assert.AreEqual(Face.B, Face.F.OppositeFace());
            Assert.AreEqual(Face.R, Face.L.OppositeFace());
            Assert.AreEqual(Face.U, Face.D.OppositeFace());
            Assert.AreEqual(Face.F, Face.B.OppositeFace());
        }

        [TestMethod]
        public void RotateTest()
        {
            Assert.AreEqual(Face.R, Face.R.Rotate(Rotation.x3));
            Assert.AreEqual(Face.D, Face.L.Rotate(Rotation.z3));
            Assert.AreEqual(Face.F, Face.D.Rotate(Rotation.x1));
            Assert.AreEqual(Face.L, Face.R.Rotate(Rotation.y2));

            foreach (Face face in Enum.GetValues(typeof(Face)))
            {
                Assert.AreEqual(face.Axis(), face.Rotate(Rotation.x2).Axis());
                Assert.AreEqual(face.Axis(), face.Rotate(Rotation.y2).Axis());
                Assert.AreEqual(face.Axis(), face.Rotate(Rotation.z2).Axis());
            }
        }
    }
}
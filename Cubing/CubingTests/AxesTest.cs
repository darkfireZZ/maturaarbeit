using static Cubing.ThreeByThree.Constants;
using Cubing.ThreeByThree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class AxesTest
    {
        [TestMethod]
        public void TestEnum()
        {
            Assert.AreEqual(0, (int)Axis.x);
            Assert.AreEqual(1, (int)Axis.y);
            Assert.AreEqual(2, (int)Axis.z);

            Assert.AreEqual(NumAxes, Enum.GetValues(typeof(Axis)).Length);
        }

        [TestMethod]
        public void AxisToStringTest()
        {
            Assert.AreEqual("x", Axes.AxisToString(Axis.x));
            Assert.AreEqual("y", Axes.AxisToString(Axis.y));
            Assert.AreEqual("z", Axes.AxisToString(Axis.z));
        }

        [TestMethod]
        public void AxisFromStringTest()
        {
            Assert.AreEqual(Axis.x, Axes.AxisFromString("x"));
            Assert.AreEqual(Axis.y, Axes.AxisFromString("y"));
            Assert.AreEqual(Axis.z, Axes.AxisFromString("z"));

            Assert.ThrowsException<ArgumentException>(()
                => Axes.AxisFromString("Exc"));
            Assert.ThrowsException<ArgumentNullException>(()
                => Axes.AxisFromString(null));
        }
    }
}
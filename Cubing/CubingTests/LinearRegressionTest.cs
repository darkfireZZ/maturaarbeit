using Cubing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class LinearRegressionTest
    {
        [TestMethod]
        public void PredictTest()
        {
            double[,] features = { { 1d, 0d }, { 1d, 1d }, { 1d, 2d }, { 1d, 3d }, { 1d, 4d } };
            double[] theta = { 1d, -1d };
            double[] labels = { 1d, 0d, -1d, -2d, -3d };
            double[] predictions = LinearRegression.Predict(features, theta);
            CollectionAssert.AreEqual(labels, predictions);
        }

        [TestMethod]
        public void PerfectFitTest()
        {
            double[,] features = { { 1d, 0d }, { 1d, 1d }, { 1d, 2d }, { 1d, 3d }, { 1d, 4d } };
            double[] theta = { 1d, -1d };
            double[] labels = { 1d, 0d, -1d, -2d, -3d };
            Assert.AreEqual(0d, LinearRegression.Cost(features, labels, theta), 0.001d);
            Assert.AreEqual(0d, LinearRegression.Gradient(features, labels, theta).Sum(), 0.001d);
        }

        [TestMethod]
        public void TestConvergence()
        {
            double[,] features = { { 1d, 0d }, { 1d, 1d }, { 1d, 2d }, { 1d, 3d }, { 1d, 4d } };
            double[] initialTheta = { 0d, 0d };
            double[] expectedTheta = { 1d, -1d };
            double[] labels = { 1d, 0d, -1d, -2d, -3d };
            double learningRate = .1d;
            int numberOfIterations = 200;
            double[] learnedTheta = LinearRegression.Learn(features, labels, initialTheta, learningRate, numberOfIterations);
            for (int index = 0; index < learnedTheta.Length; index++)
                Assert.AreEqual(expectedTheta[index], learnedTheta[index], 0.01d);
        }
    }
}
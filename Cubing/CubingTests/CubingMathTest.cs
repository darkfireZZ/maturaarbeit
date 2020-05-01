using Cubing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CubingTests
{
    [TestClass]
    public class CubingMathTest
    {
        [TestMethod]
        public void BinomialCoefficientTest()
        {
            int maxN = 13;
            int maxK = 13;

            static int factorial(int n)
            {
                int ret = 1;
                for (int i = 1; i <= n; i++)
                    ret *= i;
                return ret;
            }

            static int altBC(int n, int k)
            {
                if (k > n || k < 0)
                    return 0;
                else
                    return factorial(n) / factorial(k) / factorial(n - k);
            }

            for (int n = 0; n < maxN; n++)
                for (int k = 0; k < maxK; k++)
                    Assert.AreEqual(altBC(n, k), CubingMath.BinomialCoefficient(n, k));
        }
    }
}
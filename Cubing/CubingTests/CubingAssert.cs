using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CubingTests
{
    public static class CubingAssert
    {
        public static void HaveEqualEquatorEdgePermutation(CubieCube expected, CubieCube actual)
        {
            CubingAssert.HasFourEquatorEdges(expected);
            CubingAssert.HasFourEquatorEdges(actual);

            IEnumerator<(Edge edge, int index)> enumerator1 = expected.EP.Select((edge, index) => (edge, index)).GetEnumerator();
            IEnumerator<(Edge edge, int index)> enumerator2 = actual.EP.Select((edge, index) => (edge, index)).GetEnumerator();

            for (int equatorEdge = 0; equatorEdge < TwoPhaseConstants.NumEquatorEdges; equatorEdge++)
            {
                while (enumerator1.MoveNext() && enumerator1.Current.edge < Edge.FR);
                while (enumerator2.MoveNext() && enumerator2.Current.edge < Edge.FR);

                if (enumerator1.Current.edge != enumerator2.Current.edge)
                    throw new AssertFailedException("The two cubes do not have the same equator edge permutation: " +
                        enumerator1.Current.edge + " at index " + enumerator1.Current.index + " of " + nameof(expected) + " is not equal to " +
                        enumerator2.Current.edge + " at index " + enumerator2.Current.index + " of " + nameof(actual) + ".");
            }
        }

        public static void HasFourEquatorEdges(CubieCube cube)
        {
            int count = cube.EP.Count(edge => edge >= Edge.FR);
            if (count != TwoPhaseConstants.NumEquatorEdges)
                throw new AssertFailedException("Cube has " + count + " equator edges instead of 4.");
        }
    }
}
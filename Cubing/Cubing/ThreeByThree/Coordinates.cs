using static Cubing.ThreeByThree.Constants;
using Cubing.ThreeByThree.TwoPhase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Provides functions for mapping <see cref="CubieCube"/> state to
    /// integers and vice versa.
    /// </summary>
    public static class Coordinates
    {
        /// <summary>
        /// The number of different corner orientation coordinates.
        /// </summary>
        public const int NumCoCoords = 2187; // 3 ^ 7

        /// <summary>
        /// The number of different edge orientation coordinates.
        /// </summary>
        public const int NumEoCoords = 2048; // 2 ^ 11

        /// <summary>
        /// The number of different equator distribution coordinates.
        /// </summary>
        public const int NumEquatorDistributionCoords = 495; // (12 choose 4)

        /// <summary>
        /// The number of different equator permutation coordinates.
        /// </summary>
        public const int NumEquatorPermutationCoords = 24; // 4!

        /// <summary>
        /// The number of different equator coordinates.
        /// </summary>
        public const int NumEquatorCoords = NumEquatorDistributionCoords * NumEquatorPermutationCoords;

        /// <summary>
        /// The number of different eo-equtor coordinates.
        /// </summary>
        public const int NumEoEquatorCoords = 1013759; // NumEOCoords * NumEquatorCoords - 1

        /// <summary>
        /// The number of different corner permutation coordinates.
        /// </summary>
        public const int NumCpCoords = 40320; // 8!

        /// <summary>
        /// The number of different U and D edge permutation coordinates.
        /// </summary>
        public const int NumUdEdgePermutationCoords = 40320; // 8!

        //used for faster calculations of permutation coordinates
        private static readonly int[] factorial = { 1, 1, 2, 6, 24, 120, 720, 5040 }; //factorial[n] = n!

        /// <summary>
        /// Map the corner orientation state of a <see cref="CubieCube"/>
        /// to an integer.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to get the corner orientation
        /// coordinate from.
        /// </param>
        /// <returns>
        /// The corner orientation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetCoCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coord = 0;
            for (int corner = 0; corner < NumCorners - 1; corner++) //the orientation of the last corner is given by the orientation of the other corners
                coord = coord * 3 + cube.CO[corner];
            return coord;
        }

        /// <summary>
        /// Set the corner orientation of a <see cref="CubieCube"/> using
        /// a coordinate.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to set the corner orientation.
        /// </param>
        /// <param name="coordinate">The corner orientation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> &lt; 0 or
        /// <paramref name="coordinate"/> &gt; 2186.
        /// </exception>
        public static void SetCoCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumCoCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumCoCoords + ": " + coordinate);

            int sum = 0;
            for (int corner = NumCorners - 2; corner >= 0; corner--)
            {
                int o = coordinate % 3;
                sum += o;
                cube.CO[corner] = o;
                coordinate /= 3;
            }
            cube.CO[NumCorners - 1] = (123456789 - sum) % 3; //123456789 could be anything > 15 and divisible by 3
        }

        /// <summary>
        /// Map the edge orientation state of a <see cref="CubieCube"/> to
        /// an integer.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to get the edge orientation coordinate
        /// from.
        /// </param>
        /// <returns>
        /// The edge orientation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEoCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coord = 0;
            for (int edge = 0; edge < NumEdges - 1; edge++)
                coord = (coord << 1) + cube.EO[edge];
            return coord;
        }

        /// <summary>
        /// Set the edge orientation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to set the edge orientation.
        /// </param>
        /// <param name="coordinate">The edge orientation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> &lt; 0 or
        /// <paramref name="coordinate"/> &gt; 2047.
        /// </exception>
        public static void SetEoCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEoCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEoCoords + ": " + coordinate);

            int sum = 0;
            for (int edge = NumEdges - 2; edge >= 0; edge--)
            {
                int o = coordinate % 2;
                cube.EO[edge] = o;
                sum += o;
                coordinate >>= 1;
            }
            cube.EO[NumEdges - 1] = sum % 2;
        }

        //TODO understand
        /// <summary>
        /// Map the distribution of the equator edges of a
        /// <see cref="CubieCube"/> to an integer (ignoring the
        /// permutation).
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to get the equator coordinate from.
        /// </param>
        /// <returns>
        /// The equator coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEquatorDistributionCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            //The following works because int the CubieCube representation
            //the equator edges are the last 4. This is also why it is
            //iterating backwards.
            int coord = 0, numEquator = 0;
            for (int i = NumEdges - 1; i >= 0; i--)
                if ((int)cube.EP[i] >= 8)
                    coord += CubingMath.BinomialCoefficient(NumEdges - 1 - i, ++numEquator);
            return coord;
        }

        //TODO understand
        //IMPR
        /// <summary>
        /// Set the distribution of the equator edges of a
        /// <see cref="CubieCube"/> using a coordinate. Mixes up the
        /// edges.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to set the distribution of the equator edges.
        /// </param>
        /// <param name="coordinate">The equator coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> &lt; 0 or
        /// <paramref name="coordinate"/> &gt; 494.
        /// </exception>
        public static void SetEquatorDistributionCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorDistributionCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEquatorDistributionCoords + ": " + coordinate);

            /*
            //The following works because in the CubieCube representation
            //the equator edges are the last 4.
            int numEquator = 0, numNonEquator = 0;
            for (int edge = 0; edge < NumEdges; edge++)
            {
                int bc = CubingMath.BinomialCoefficient(11 - edge, 4 - numEquator);
                if (coord >= bc)
                {
                    coord -= bc;
                    cube.EP[edge] = (Edge)(11 - numEquator++);
                }
                else
                    cube.EP[edge] = (Edge)(numNonEquator++);
            }
            */

            Edge[] equatorEdges = { Edge.FR, Edge.FL, Edge.BL, Edge.BR };
            Edge[] nonEquatorEdges = { Edge.UR, Edge.UF, Edge.UL, Edge.UB, Edge.DR, Edge.DF, Edge.DL, Edge.DB };


            int a = coordinate;
            for (int edge = 0; edge < NumEdges; edge++)
                cube.EP[edge] = (Edge)(-1);

            int x = 4;
            for (int j = 0; j < NumEdges; j++)
                if (a - CubingMath.BinomialCoefficient(11 - j, x) >= 0)
                {
                    cube.EP[j] = equatorEdges[4 - x];
                    a -= CubingMath.BinomialCoefficient(11 - j, x);
                    x--;
                }

            x = 0;
            for (int j = 0; j < NumEdges; j++)
                if (cube.EP[j] == (Edge)(-1))
                {
                    cube.EP[j] = nonEquatorEdges[x];
                    x++;
                }
        }

        public static int GetEquatorPermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coord = 0;
            int equatorEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex >= 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                if (edge < (int)Edge.FR) //if edge is not an equator edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> ((edge - TwoPhaseConstants.NumUdEdges) * 4)) & 0xF;
                coord += factorial[equatorEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumEdges - 1 - edge) * 4);
                if (equatorEdgeIndex > 0)
                    equatorEdgeIndex--;
                else
                    break;
            }
            return coord;
        }

        public static void SetEquatorPermutationCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorPermutationCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int list = 0x89AB;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EP
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int equatorEdgeIndex = 3; equatorEdgeIndex > 0; equatorEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[equatorEdgeIndex];
                coordinate %= factorial[equatorEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge < Edge.FR) ; //find the next equator edge on the cube
                cube.EP[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((TwoPhaseConstants.NumEquatorEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge < Edge.FR) ; //find the last equator edge on the cube
            cube.EP[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        public static int GetEquatorCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetEquatorDistributionCoord(cube) * NumEquatorPermutationCoords + GetEquatorPermutationCoord(cube);
        }

        public static void SetEquatorCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetEquatorDistributionCoord(cube, coordinate / 24);
            SetEquatorPermutationCoord(cube, coordinate % 24);
        }

        /// <summary>
        /// Map the corner permutation state of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to get the corner permutation coordinate
        /// from.
        /// </param>
        /// <returns>
        /// The corner permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEoEquatorCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int eo = GetEoCoord(cube);
            int equator = GetEquatorDistributionCoord(cube);
            int eoEquator = NumEoCoords * equator + eo;

            return eoEquator;
        }

        /// <summary>
        /// Set the edge orientation and the distribution of the equator edges
        /// of a <see cref="CubieCube"/> using a coordinate. Mixes up the edges.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to set the eo-equator coordinate.
        /// </param>
        /// <param name="coordinate">The eo-equator coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> &lt; 0 or
        /// <paramref name="coordinate"/> &gt; 1013759.
        /// </exception>
        public static void SetEoEquatorCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((ulong)coordinate >= NumEoEquatorCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range.");

            int eo = coordinate % NumEoCoords;
            SetEoCoord(cube, eo);

            int equator = coordinate / NumEoCoords;
            SetEquatorDistributionCoord(cube, equator);
        }

        /// <summary>
        /// Map the corner permutation and the distribution of the equator edges
        /// of a <see cref="CubieCube"/> to an integer.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to get the eo-equator coordinate
        /// from.
        /// </param>
        /// <returns>
        /// The eo-equator coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetCpCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            //inspired by the 2x2x2 scrambler of the WCA (as of 20.04.2020):
            //https://github.com/thewca/tnoodle-lib/blob/master/scrambles/src/main/java/org/worldcubeassociation/tnoodle/puzzle/TwoByTwoSolver.java
            int list = 0x01234567;
            int coord = 0;
            for (int cornerIndex = NumCorners - 1; cornerIndex > 0; cornerIndex--) //last corner is skipped because the information is redundant
            {
                int corner = (int)cube.CP[cornerIndex];
                int numberOfLeftCornersWithHigherOrder = (list >> corner * 4) & 0xF;
                coord += factorial[cornerIndex] * numberOfLeftCornersWithHigherOrder;

                list -= 0x1111111 >> ((NumCorners - 1 - corner) * 4);
            }

            return coord;
        }

        /// <summary>
        /// Set the corner permutation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> to set the corner permutation.
        /// </param>
        /// <param name="coordinate">The corner permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> &lt; 0 or
        /// <paramref name="coordinate"/> &gt; 5039.
        /// </exception>
        public static void SetCpCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumCpCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //inspired by the 2x2x2 scrambler of the WCA (as of 20.04.2020):
            //https://github.com/thewca/tnoodle-lib/blob/master/scrambles/src/main/java/org/worldcubeassociation/tnoodle/puzzle/TwoByTwoSolver.java
            int list = 0x01234567; //used as a faster alternative to a list of corners
            for (int cornerIndex = NumCorners - 1; cornerIndex > 0; cornerIndex--)
            {
                int numberOfLeftCornersWithHigherOrder = coordinate / factorial[cornerIndex];
                int corner = (list >> numberOfLeftCornersWithHigherOrder * 4) & 0xF;

                list = (list & (0xFFFFFFF >> (7 - numberOfLeftCornersWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftCornersWithHigherOrder * 4)) >> 4); //remove corner form list
                coordinate %= factorial[cornerIndex]; //remove corner from coordinate

                cube.CP[cornerIndex] = (Corner)corner;
            }

            cube.CP[0] = (Corner)list; //last corner is the only one remaining in the list
        }
        
        public static int GetUdEdgePermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            //inspired by the 2x2x2 scrambler of the WCA (as of 20.04.2020):
            //https://github.com/thewca/tnoodle-lib/blob/master/scrambles/src/main/java/org/worldcubeassociation/tnoodle/puzzle/TwoByTwoSolver.java
            int b = 0x01234567;
            int coord = 0;
            for (int edgeIndex = TwoPhaseConstants.NumUdEdges - 1; edgeIndex > 0; edgeIndex--) //last edge is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                int numberOfLeftEdgesWithHigherOrder = (b >> edge * 4) & 0xF;
                coord += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                b -= 0x1111111 >> (TwoPhaseConstants.NumUdEdges - 1 - edge) * 4;
            }

            return coord;
        }
        
        public static void SetUdEdgePermutationCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUdEdgePermutationCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //inspired by the 2x2x2 scrambler of the WCA (as of 20.04.2020):
            //https://github.com/thewca/tnoodle-lib/blob/master/scrambles/src/main/java/org/worldcubeassociation/tnoodle/puzzle/TwoByTwoSolver.java
            int list = 0x01234567; //used as a faster alternative to a list of corners
            for (int edgeIndex = TwoPhaseConstants.NumUdEdges - 1; edgeIndex > 0; edgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[edgeIndex];
                int edge = (list >> numberOfLeftEdgesWithHigherOrder * 4) & 0xF;

                list = (list & (0xFFFFFFF >> (TwoPhaseConstants.NumUdEdges - 1 - numberOfLeftEdgesWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftEdgesWithHigherOrder * 4)) >> 4); //remove edge form list
                coordinate %= factorial[edgeIndex]; //remove corner from coordinate

                cube.EP[edgeIndex] = (Edge)edge;
            }

            cube.EP[0] = (Edge)list; //last corner is the only one remaining in the list
        }

        //TODO move
        /// <summary>
        /// Get the index into the pruning table given the corner orientation,
        /// edge orientation and equator coordinates.
        /// </summary>
        /// <param name="co">The corner orientation coordinate.</param>
        /// <param name="eo">The edge orientation coodinate.</param>
        /// <param name="equator">The equator coordinate.</param>
        /// <returns>
        /// The index into the pruning table for the given parameters.
        /// </returns>
        public static int GetPruningIndex(int co, int eo, int equator)
        {
            int eoEquator = equator * NumEoCoords + eo;
            int reducedEoEquator = Phase1Tables.ReduceEoEquatorCoordinate[eoEquator];
            int reductionSymmetry = Phase1Tables.ReductionSymmetry[eoEquator];
            int rotatedCo = Phase1Tables.ConjugateCoCoordinate[co, reductionSymmetry];
            int pruningCoordinate = reducedEoEquator * NumCoCoords + rotatedCo;

            return pruningCoordinate;
        }
    }
}
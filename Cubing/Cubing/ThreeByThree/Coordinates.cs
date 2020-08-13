using System;
using System.Collections.Generic;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

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
        /// The number of different U-edge distribution coordinates during phase 1.
        /// </summary>
        public const int NumUEdgeDistributionCoordsPhase1 = 495; // (12 choose 4)


        /// <summary>
        /// The numboer of different U-edge distribution coordinates during phase 2.
        /// </summary>
        public const int NumUEdgeDistributionCoordsPhase2 = 70; // (8 choose 4)

        /// <summary>
        /// The number of different U-edge permutation coordinates.
        /// </summary>
        public const int NumUEdgePermutationCoords = 24; // 4!

        /// <summary>
        /// The number of different U-edge coordinates during phase 1.
        /// </summary>
        public const int NumUEdgeCoordsPhase1 = NumUEdgeDistributionCoordsPhase1 * NumUEdgePermutationCoords;

        /// <summary>
        /// The number of different U-edge coordinates during phase 2.
        /// </summary>
        public const int NumUEdgeCoordsPhase2 = NumUEdgeDistributionCoordsPhase2 * NumUEdgePermutationCoords;

        /// <summary>
        /// The number of different D-edge distribution coordinates during phase 1.
        /// </summary>
        public const int NumDEdgeDistributionCoordsPhase1 = 495; // (12 choose 4)

        /// <summary>
        /// The number of different D-edge distribution coordinates during phase 2.
        /// </summary>
        public const int NumDEdgeDistributionCoordsPhase2 = 70; // (8 choose 4)

        /// <summary>
        /// The number of different D-edge permutation coordinates.
        /// </summary>
        public const int NumDEdgePermutationCoords = 24; // 4!

        /// <summary>
        /// The number of different D-edge coordinates during phase 1.
        /// </summary>
        public const int NumDEdgeCoordsPhase1 = NumDEdgeDistributionCoordsPhase1 * NumDEdgePermutationCoords;

        /// <summary>
        /// The number of different D-edge coordinates during phase 2.
        /// </summary>
        public const int NumDEdgeCoordsPhase2 = NumDEdgeDistributionCoordsPhase2 * NumDEdgePermutationCoords;

        /// <summary>
        /// The number of different eo-equtor coordinates.
        /// </summary>
        public const int NumEoEquatorCoords = 1013759; // NumEOCoords * NumEquatorCoords - 1

        /// <summary>
        /// The number of different corner permutation coordinates.
        /// </summary>
        public const int NumCpCoords = 40320; // 8!

        /// <summary>
        /// The number of different edge permutation coordinates.
        /// </summary>
        public const int NumEpCoords = 479001600; //12!

        /// <summary>
        /// The number of different U and D edge permutation coordinates.
        /// </summary>
        public const int NumUdEdgePermutationCoords = 40320; // 8!

        //used for faster calculations of permutation coordinates
        private static readonly int[] factorial =
            { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800 }; //factorial[n] = n!

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

            //The following works because in the CubieCube representation
            //the equator edges are the last 4.
            int coord = 0, numEquator = 0;
            for (int i = NumEdges - 1; i >= 0; i--)
                if ((int)cube.EP[i] >= 8)
                    coord += Utils.BinomialCoefficient(NumEdges - 1 - i, ++numEquator);
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

            int numEquator = 0, numNonEquator = 0;
            for (int edge = 0; edge < NumEdges; edge++)
            {
                int bc = Utils.BinomialCoefficient(11 - edge, NumEquatorEdges - numEquator);
                if (coordinate >= bc) //current edge is an equator edge
                {
                    coordinate -= bc;
                    cube.EP[edge] = (Edge)(11 - numEquator++);
                }
                else
                    cube.EP[edge] = (Edge)(numNonEquator++);
            }

            //TODO remove
            /*Edge[] equatorEdges = { Edge.FR, Edge.FL, Edge.BL, Edge.BR };
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
            */
        }

        public static int GetEquatorPermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coord = 0;
            int equatorEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                if (edge < (int)Edge.FR) //if edge is not an equator edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> ((edge - NumUdEdges) * 4)) & 0xF;
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

            //IMPR inefficient
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
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumEquatorEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
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

        public static int GetUEdgeDistributionCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coord = 0, numUEdges = 0;
            for (int i = 0; i < NumEdges; i++)
                if (cube.EP[i] <= Edge.UR)
                    coord += Utils.BinomialCoefficient(i, ++numUEdges);
            return coord;
        }

        public static void SetUEdgeDistributionCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgeDistributionCoordsPhase1)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEquatorDistributionCoords + ": " + coordinate);

            int numUEdges = 0, numNonUEdges = 0;
            for (int edge = NumEdges - 1; edge >= 0; edge--)
            {
                int bc = Utils.BinomialCoefficient(edge, NumUEdges - numUEdges);
                if (coordinate >= bc) //current edge is an equator edge
                {
                    coordinate -= bc;
                    cube.EP[edge] = (Edge)(NumUEdges - 1 - numUEdges++);
                }
                else
                    cube.EP[edge] = (Edge)(NumEdges - 1 - numNonUEdges++);
            }
        }

        public static int GetUEdgePermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coord = 0;
            int uEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                if (edge > (int)Edge.UR) //if edge is not a U-layer edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> (edge * 4)) & 0xF;
                coord += factorial[uEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumUEdges - 1 - edge) * 4);
                if (uEdgeIndex > 0)
                    uEdgeIndex--;
                else
                    break;
            }
            return coord;
        }

        public static void SetUEdgePermutationCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgePermutationCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //IMPR inefficient
            int list = 0x0123;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EP
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int uEdgeIndex = 3; uEdgeIndex > 0; uEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[uEdgeIndex];
                coordinate %= factorial[uEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge > Edge.UR) ; //find the next U-layer edge on the cube
                cube.EP[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumUEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge > Edge.UR) ; //find the last U-layer edge on the cube
            cube.EP[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        public static int GetUEdgeCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetUEdgeDistributionCoord(cube) * NumEquatorPermutationCoords + GetUEdgePermutationCoord(cube);
        }

        public static void SetUEdgeCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgeCoordsPhase1)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetUEdgeDistributionCoord(cube, coordinate / NumUEdgePermutationCoords);
            SetUEdgePermutationCoord(cube, coordinate % NumUEdgePermutationCoords);
        }

        public static int GetDEdgeDistributionCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coord = 0, numUEdges = 0;
            for (int i = 0; i < NumEdges; i++)
                if (((int)cube.EP[i] >= (int)Edge.DF) && ((int)cube.EP[i] <= (int)Edge.DR))
                    coord += Utils.BinomialCoefficient(i, ++numUEdges);
            return coord;
        }

        public static void SetDEdgeDistributionCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgeDistributionCoordsPhase1)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int numDEdges = 0, numNonDEdges = 0;
            for (int edge = NumEdges - 1; edge >= 0; edge--)
            {
                int bc = Utils.BinomialCoefficient(edge, NumDEdges - numDEdges);
                if (coordinate >= bc) //current edge is a D-layer edge
                {
                    coordinate -= bc;
                    cube.EP[edge] = (Edge)(NumUdEdges - 1 - numDEdges++);
                }
                else
                {
                    if (numNonDEdges < 4)
                        cube.EP[edge] = (Edge)(NumEdges - 1 - numNonDEdges++);
                    else
                        cube.EP[edge] = (Edge)(NumUEdges - 1 - numNonDEdges++);
                }
            }
        }

        public static int GetDEdgePermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coord = 0;
            int dEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                if (edge < (int)Edge.DF || edge > (int)Edge.DR) //if edge is not a D-layer edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> ((edge - NumUEdges) * 4)) & 0xF;
                coord += factorial[dEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumUdEdges - 1 - edge) * 4);
                if (dEdgeIndex > 0)
                    dEdgeIndex--;
                else
                    break;
            }
            return coord;
        }

        public static void SetDEdgePermutationCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgePermutationCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //IMPR inefficient
            int list = 0x4567;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EP
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int dEdgeIndex = 3; dEdgeIndex > 0; dEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[dEdgeIndex];
                coordinate %= factorial[dEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && (edgeEnumerator.Current.edge < Edge.DF || edgeEnumerator.Current.edge > Edge.DR)) ; //find the next D-layer edge on the cube
                cube.EP[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumDEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && (edgeEnumerator.Current.edge < Edge.DF || edgeEnumerator.Current.edge > Edge.DR)) ; //find the last D-layer edge on the cube
            cube.EP[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        public static int GetDEdgeCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetDEdgeDistributionCoord(cube) * NumDEdgePermutationCoords + GetDEdgePermutationCoord(cube);
        }

        public static void SetDEdgeCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgeCoordsPhase1)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetDEdgeDistributionCoord(cube, coordinate / 24);
            SetDEdgePermutationCoord(cube, coordinate % 24);
        }

        public static int CombineUAndDEdgePermutation(int uEdgeCoordinate, int dEdgeCoordinate)
        {
            if ((uint)uEdgeCoordinate >= NumUEdgeCoordsPhase2)
                throw new ArgumentOutOfRangeException(nameof(uEdgeCoordinate) + " is out of range: " + uEdgeCoordinate);
            if ((uint)dEdgeCoordinate >= NumDEdgeCoordsPhase2)
                throw new ArgumentOutOfRangeException(nameof(dEdgeCoordinate) + " is out of range: " + dEdgeCoordinate);

            int uEdgeDistribution = uEdgeCoordinate / NumUEdgePermutationCoords;
            int uEdgePermutation  = uEdgeCoordinate % NumUEdgePermutationCoords;
            int dEdgePermutation  = dEdgeCoordinate % NumDEdgePermutationCoords;

            int coord = 0;

            int uEdgeList = 0x0123;
            int dEdgeList = 0x4567;

            int b = 0x01234567; //TODO rename

            int numUEdges = 0, numDEdges = 0;
            for (int edgeIndex = NumUdEdges - 1; edgeIndex >= 0; edgeIndex--)
            {
                int edge;
                int bc = Utils.BinomialCoefficient(edgeIndex, 4 - numUEdges);
                if (uEdgeDistribution >= bc) //current edge is a U-layer edge
                {
                    uEdgeDistribution -= bc;

                    int numberOfLeftUEdgesWithHigherOrder = uEdgePermutation / factorial[NumUEdges - 1 - numUEdges];
                    uEdgePermutation %= factorial[NumUEdges - 1 - numUEdges];
                    edge = (uEdgeList >> (numberOfLeftUEdgesWithHigherOrder * 4)) & 0xF;
                    uEdgeList = ((uEdgeList & (0xFFF0 << (numberOfLeftUEdgesWithHigherOrder * 4))) >> 4) + (uEdgeList & (0xFFFF >> ((NumUEdges - numberOfLeftUEdgesWithHigherOrder) * 4)));
                    numUEdges++;
                }
                else
                {
                    int numberOfLeftDEdgesWithHigherOrder = dEdgePermutation / factorial[NumDEdges - 1 - numDEdges];
                    dEdgePermutation %= factorial[NumDEdges - 1 - numDEdges];
                    edge = (dEdgeList >> (numberOfLeftDEdgesWithHigherOrder * 4)) & 0xF;
                    dEdgeList = ((dEdgeList & (0xFFF0 << (numberOfLeftDEdgesWithHigherOrder * 4))) >> 4) + (dEdgeList & (0xFFFF >> ((NumDEdges - numberOfLeftDEdgesWithHigherOrder) * 4)));
                    numDEdges++;
                }

                int numberOfLeftEdgesWithHigherOrder = (b >> edge * 4) & 0xF;
                coord += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                b -= 0x1111111 >> (NumUdEdges - 1 - edge) * 4;
            }

            return coord;
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

                list = (list & (0xFFFFFFF >> (NumCorners - 1 - numberOfLeftCornersWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftCornersWithHigherOrder * 4)) >> 4); //remove corner form list
                coordinate %= factorial[cornerIndex]; //remove corner from coordinate

                cube.CP[cornerIndex] = (Corner)corner;
            }

            cube.CP[0] = (Corner)list; //last corner is the only one remaining in the list
        }

        public static int GetEpCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            long list = 0x0123456789AB;
            int coord = 0;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last edge is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                int numberOfLeftEdgesWithHigherOrder = (int)((list >> edge * 4) & 0xF);
                coord += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x11111111111 >> ((NumEdges - 1 - edge) * 4);
            }

            return coord;
        }

        public static void SetEpCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEpCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            long list = 0x0123456789AB; //used as a faster alternative to a list of edges
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[edgeIndex];
                int edge = (int)((list >> numberOfLeftEdgesWithHigherOrder * 4) & 0xF);

                list = (list & (0xFFFFFFFFFFF >> (NumEdges - 1 - numberOfLeftEdgesWithHigherOrder) * 4)) + ((list & (0xFFFFFFFFFF0 << numberOfLeftEdgesWithHigherOrder * 4)) >> 4); //remove edge form list
                coordinate %= factorial[edgeIndex]; //remove edge from coordinate

                cube.EP[edgeIndex] = (Edge)edge;
            }

            cube.EP[0] = (Edge)list; //last edge is the only one remaining in the list
        }

        public static int GetUdEdgePermutationCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            //inspired by the 2x2x2 scrambler of the WCA (as of 20.04.2020):
            //https://github.com/thewca/tnoodle-lib/blob/master/scrambles/src/main/java/org/worldcubeassociation/tnoodle/puzzle/TwoByTwoSolver.java
            int b = 0x01234567;
            int coord = 0;
            for (int edgeIndex = NumUdEdges - 1; edgeIndex > 0; edgeIndex--) //last edge is skipped because the information is redundant
            {
                int edge = (int)cube.EP[edgeIndex];
                int numberOfLeftEdgesWithHigherOrder = (b >> edge * 4) & 0xF;
                coord += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                b -= 0x1111111 >> (NumUdEdges - 1 - edge) * 4;
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
            for (int edgeIndex = NumUdEdges - 1; edgeIndex > 0; edgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[edgeIndex];
                int edge = (list >> numberOfLeftEdgesWithHigherOrder * 4) & 0xF;

                list = (list & (0xFFFFFFF >> (NumUdEdges - 1 - numberOfLeftEdgesWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftEdgesWithHigherOrder * 4)) >> 4); //remove edge form list
                coordinate %= factorial[edgeIndex]; //remove corner from coordinate

                cube.EP[edgeIndex] = (Edge)edge;
            }

            cube.EP[0] = (Edge)list; //last corner is the only one remaining in the list
        }

        /// <summary>
        /// Calculate the parity of the corners using a corner permutation
        /// coordinate. True means odd parity and false means even parity.
        /// </summary>
        /// <param name="coordinate">
        /// The corner permutation coordinate to use.
        /// </param>
        /// <returns>
        /// The parity of the corners for the given corner permutation
        /// coordinate.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static bool CpCoordinateParity(int coordinate)
        {
            if ((uint)coordinate >= NumCpCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int sum = 0;
            for (int corner = NumCorners - 1; corner > 0; corner--)
            {
                sum += coordinate / factorial[corner];
                coordinate %= factorial[corner];
            }
            return sum % 2 == 1; //return true if parity is odd
        }

        /// <summary>
        /// Calculate the parity of the edges using a edge permutation
        /// coordinate. True means odd parity and false means even parity.
        /// </summary>
        /// <param name="coordinate">
        /// The edge permutation coordinate to use.
        /// </param>
        /// <returns>
        /// The parity of the edges for the given edge permutation coordinate.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static bool EpCoordinateParity(int coordinate)
        {
            if ((uint)coordinate >= NumEpCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int sum = 0;
            for (int edge = NumEdges - 1; edge > 0; edge--)
            {
                sum += coordinate / factorial[edge];
                coordinate %= factorial[edge];
            }
            return sum % 2 == 1; //return true if parity is odd
        }
    }
}
 
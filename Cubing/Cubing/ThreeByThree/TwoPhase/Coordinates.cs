using System;
using System.Collections.Generic;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Provides functions for mapping <see cref="CubieCube"/> states to
    /// integers and vice versa.
    /// </summary>
    public static class Coordinates
    {
        /// <summary>
        /// The number of different corner orientations.
        /// </summary>
        public const int NumCornerOrientations = 2187; // 3 ^ 7

        /// <summary>
        /// The number of different edge orientations.
        /// </summary>
        public const int NumEdgeOrientations = 2048; // 2 ^ 11

        /// <summary>
        /// The number of different equator distributions.
        /// </summary>
        public const int NumEquatorDistributions = 495; // (12 choose 4)

        /// <summary>
        /// The number of different equator orders.
        /// </summary>
        public const int NumEquatorOrders = 24; // 4!

        /// <summary>
        /// The number of different equator permutations.
        /// </summary>
        public const int NumEquatorPermutations = NumEquatorDistributions * NumEquatorOrders;

        /// <summary>
        /// The number of different U-edge distributions.
        /// </summary>
        public const int NumUEdgeDistributions = 495; // (12 choose 4)

        /// <summary>
        /// The number of different U-edge distribution coordinates during phase 2.
        /// </summary>
        public const int NumUEdgeDistributionCoordsPhase2 = 70; // (8 choose 4)

        /// <summary>
        /// The number of different U-edge orders.
        /// </summary>
        public const int NumUEdgeOrders = 24; // 4!

        /// <summary>
        /// The number of different U-edge permutations.
        /// </summary>
        public const int NumUEdgePermutations = NumUEdgeDistributions * NumUEdgeOrders;

        /// <summary>
        /// The number of different U-edge permutations during phase 2.
        /// </summary>
        public const int NumUEdgePermutationsPhase2 = NumUEdgeDistributionCoordsPhase2 * NumUEdgeOrders;

        /// <summary>
        /// The number of different D-edge distribution coordinates.
        /// </summary>
        public const int NumDEdgeDistributions = 495; // (12 choose 4)

        /// <summary>
        /// The number of different D-edge distribution coordinates during phase 2.
        /// </summary>
        public const int NumDEdgeDistributionsPhase2 = 70; // (8 choose 4)

        /// <summary>
        /// The number of different D-edge orders.
        /// </summary>
        public const int NumDEdgeOrders = 24; // 4!

        /// <summary>
        /// The number of different D-edge permutations.
        /// </summary>
        public const int NumDEdgePermutations = NumDEdgeDistributions * NumDEdgeOrders;

        /// <summary>
        /// The number of different D-edge permutations during phase 2.
        /// </summary>
        public const int NumDEdgePermutationsPhase2 = NumDEdgeDistributionsPhase2 * NumDEdgeOrders;

        /// <summary>
        /// The number of different combined edge orientation and equator distribution coordinates.
        /// </summary>
        public const int NumEoEquatorCoords = NumEdgeOrientations * NumEquatorDistributions - 1;

        /// <summary>
        /// The number of different corner permutation coordinates.
        /// </summary>
        public const int NumCornerPermutations = 40_320; // 8!

        /// <summary>
        /// The number of different edge permutation coordinates.
        /// </summary>
        public const int NumEdgePermutations = 479_001_600; //12!

        /// <summary>
        /// The number of different U and D edge permutation coordinates.
        /// </summary>
        public const int NumUdEdgeOrders = 40_320; // 8!

        //used for faster calculations of permutation coordinates
        private static readonly int[] factorial =
            { 1, 1, 2, 6, 24, 120, 720, 5040, 40_320, 362_880, 3_628_800, 39_916_800 }; //factorial[n] = n!

        /// <summary>
        /// Map the corner orientation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the corner orientation coordinate.
        /// </param>
        /// <returns>
        /// The corner orientation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetCornerOrientation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coordinate = 0;
            for (int corner = 0; corner < NumCorners - 1; corner++) //the orientation of the last corner is redundant
                coordinate = coordinate * 3 + cube.CornerOrientation[corner];
            return coordinate;
        }

        /// <summary>
        /// Set the corner orientation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the corner orientation.
        /// </param>
        /// <param name="coordinate">The corner orientation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetCornerOrientation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumCornerOrientations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumCornerOrientations + ": " + coordinate);

            int sum = 0;
            for (int corner = NumCorners - 2; corner >= 0; corner--)
            {
                int orientation = coordinate % 3;
                sum += orientation;
                cube.CornerOrientation[corner] = orientation;
                coordinate /= 3;
            }
            cube.CornerOrientation[NumCorners - 1] = (777 - sum) % 3; //777 could be anything > 15 and divisible by 3
        }

        /// <summary>
        /// Map the edge orientation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the edge orientation coordinate.
        /// </param>
        /// <returns>
        /// The edge orientation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEdgeOrientation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coordinate = 0;
            for (int edge = 0; edge < NumEdges - 1; edge++)
                coordinate = (coordinate << 1) + cube.EdgeOrientation[edge];
            return coordinate;
        }

        /// <summary>
        /// Set the edge orientation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the edge orientation.
        /// </param>
        /// <param name="coordinate">The edge orientation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEdgeOrientation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEdgeOrientations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEdgeOrientations + ": " + coordinate);

            int sum = 0;
            for (int edge = NumEdges - 2; edge >= 0; edge--)
            {
                int orientation = coordinate & 0b1;
                cube.EdgeOrientation[edge] = orientation;
                sum += orientation;
                coordinate >>= 1;
            }
            cube.EdgeOrientation[NumEdges - 1] = sum & 0b1;
        }

        /// <summary>
        /// Map the equator edge distribution of a see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the equator edge distribution coordinate.
        /// </param>
        /// <returns>
        /// The equator edge distribution coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEquatorDistribution(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            //The following works because in the CubieCube representation
            //the equator edges are the last 4.
            int coordinate = 0, numEquatorEdgesDone = 0;
            for (int edge = NumEdges - 1; edge >= 0; edge--)
                if ((int)cube.EdgePermutation[edge] >= 8)
                    coordinate += Utils.BinomialCoefficient(NumEdges - 1 - edge, ++numEquatorEdgesDone);
            return coordinate;
        }

        /// <summary>
        /// Set the equator edge distribution of a <see cref="CubieCube"/>
        /// using a coordinate. Mixes up the permutation of the non-equator
        /// edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the equator edge distribution.
        /// </param>
        /// <param name="coordinate">
        /// The equator edge distribution coordinate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEquatorDistribution(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorDistributions)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEquatorDistributions + ": " + coordinate);

            int numEquatorEdgesDone = 0, numNonEquatorEdgesDone = 0;
            for (int edge = 0; edge < NumEdges; edge++)
            {
                int binomialCoefficient = Utils.BinomialCoefficient(11 - edge, NumEquatorEdges - numEquatorEdgesDone);
                if (coordinate >= binomialCoefficient) //current edge is an equator edge
                {
                    coordinate -= binomialCoefficient;
                    cube.EdgePermutation[edge] = (Edge)(11 - numEquatorEdgesDone++);
                }
                else
                    cube.EdgePermutation[edge] = (Edge)numNonEquatorEdgesDone++;
            }
        }
        
        /// <summary>
        /// Map the equator edge order of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the equator edge order coordinate.
        /// </param>
        /// <returns>
        /// The equator edge order coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEquatorOrder(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coordinate = 0;
            int equatorEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EdgePermutation[edgeIndex];
                if (edge < (int)Edge.FR) //if edge is not an equator edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> ((edge - NumUdEdges) * 4)) & 0xF;
                coordinate += factorial[equatorEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumEdges - 1 - edge) * 4);
                if (equatorEdgeIndex > 0)
                    equatorEdgeIndex--;
                else
                    break;
            }
            return coordinate;
        }

        /// <summary>
        /// Set the equator edge order of a <see cref="CubieCube"/> using a
        /// coordinate. Leaves the permutation of all non-equator edges intact.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the equator edge order.
        /// </param>
        /// <param name="coordinate">The equator edge order.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEquatorOrder(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorOrders)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //IMPR
            int list = 0x89AB;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EdgePermutation
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int equatorEdgeIndex = 3; equatorEdgeIndex > 0; equatorEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[equatorEdgeIndex];
                coordinate %= factorial[equatorEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge < Edge.FR) ; //find the next equator edge on the cube
                cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumEquatorEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge < Edge.FR) ; //find the last equator edge on the cube
            cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        /// <summary>
        /// Map the equator edge permutation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the equator edge permutation coordinate.
        /// </param>
        /// <returns>
        /// The equator edge permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEquatorPermutation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetEquatorDistribution(cube) * NumEquatorOrders + GetEquatorOrder(cube);
        }

        /// <summary>
        /// Set the equator edge permutation of a <see cref="CubieCube"/> using
        /// a coordinate. Mixes up the permutation of the non-equator edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the equator edge permutation.
        /// </param>
        /// <param name="coordinate">The equator edge permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEquatorPermutation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEquatorPermutations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetEquatorDistribution(cube, coordinate / 24);
            SetEquatorOrder(cube, coordinate % 24);
        }

        /// <summary>
        /// Map the U-edge distribution of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the U-edge distribution coordinate.
        /// </param>
        /// <returns>
        /// The U-edge distribution coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetUEdgeDistribution(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coord = 0, numUEdges = 0;
            for (int i = 0; i < NumEdges; i++)
                if (cube.EdgePermutation[i] <= Edge.UR)
                    coord += Utils.BinomialCoefficient(i, ++numUEdges);
            return coord;
        }

        /// <summary>
        /// Set the U-edge distribution of a <see cref="CubieCube"/> using a
        /// coordinate. Mixes up the permutation of the non-U-edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the U-edge distribution.
        /// </param>
        /// <param name="coordinate">
        /// The U-edge distribution coordinate of <paramref name="cube"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetUEdgeDistributionCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgeDistributions)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " must be between 0 and " + NumEquatorDistributions + ": " + coordinate);

            int numUEdgesDone = 0, numNonUEdgesDone = 0;
            for (int edge = NumEdges - 1; edge >= 0; edge--)
            {
                int binomialCoefficient = Utils.BinomialCoefficient(edge, NumUEdges - numUEdgesDone);
                if (coordinate >= binomialCoefficient) //current edge is an equator edge
                {
                    coordinate -= binomialCoefficient;
                    cube.EdgePermutation[edge] = (Edge)(NumUEdges - 1 - numUEdgesDone++);
                }
                else
                    cube.EdgePermutation[edge] = (Edge)(NumEdges - 1 - numNonUEdgesDone++);
            }
        }

        /// <summary>
        /// Map the U-edge order of a <see cref="CubieCube"/> to an integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the U-edge order coordinate.
        /// </param>
        /// <returns>
        /// The U-edge order coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetUEdgeOrder(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coordinate = 0;
            int uEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EdgePermutation[edgeIndex];
                if (edge > (int)Edge.UR) //if edge is not a U-layer edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> (edge * 4)) & 0xF;
                coordinate += factorial[uEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumUEdges - 1 - edge) * 4);
                if (uEdgeIndex > 0)
                    uEdgeIndex--;
                else
                    break;
            }
            return coordinate;
        }

        /// <summary>
        /// Set the U-edge order of a <see cref="CubieCube"/> using a
        /// coordinate. Leaves the order of all non-U-edges intact.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the U-edge order.
        /// </param>
        /// <param name="coordinate">The U-edge order coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetUEdgeOrder(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgeOrders)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //IMPR
            int list = 0x0123;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EdgePermutation
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int uEdgeIndex = 3; uEdgeIndex > 0; uEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[uEdgeIndex];
                coordinate %= factorial[uEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge > Edge.UR) ; //find the next U-layer edge on the cube
                cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumUEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && edgeEnumerator.Current.edge > Edge.UR) ; //find the last U-layer edge on the cube
            cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        /// <summary>
        /// Map the U-edge permutation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the U-edge permutation coordinate.
        /// </param>
        /// <returns>
        /// The U-edge permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is out of range.
        /// </exception>
        public static int GetUEdgePermutation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetUEdgeDistribution(cube) * NumEquatorOrders + GetUEdgeOrder(cube);
        }

        /// <summary>
        /// Set the U-edge permutation of a <see cref="CubieCube"/> using a
        /// coordinate. Mixes up the permutation of the non-U-edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the U-edge permutation.
        /// </param>
        /// <param name="coordinate">The U-edge permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetUEdgePermutation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUEdgePermutations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetUEdgeDistributionCoord(cube, coordinate / NumUEdgeOrders);
            SetUEdgeOrder(cube, coordinate % NumUEdgeOrders);
        }

        /// <summary>
        /// Map the D-edge distribution of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the D-edge distribution coordinate.
        /// </param>
        /// <returns>
        /// The D-edge distribution coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetDEdgeDistribution(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int coordinate = 0, numUEdgesDone = 0;
            for (int i = 0; i < NumEdges; i++)
                if (((int)cube.EdgePermutation[i] >= (int)Edge.DF) && ((int)cube.EdgePermutation[i] <= (int)Edge.DR))
                    coordinate += Utils.BinomialCoefficient(i, ++numUEdgesDone);
            return coordinate;
        }

        /// <summary>
        /// Set the D-edge distribution of a <see cref="CubieCube"/> using a
        /// coordinate. Mixes up the permutation of the non-D-edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the D-edge distribution coordinate.
        /// </param>
        /// <param name="coordinate">
        /// The D-edge distribution coordinate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetDEdgeDistribution(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgeDistributions)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int numDEdgesDone = 0, numNonDEdgesDone = 0;
            for (int edge = NumEdges - 1; edge >= 0; edge--)
            {
                int binomialCoefficient = Utils.BinomialCoefficient(edge, NumDEdges - numDEdgesDone);
                if (coordinate >= binomialCoefficient) //current edge is a D-layer edge
                {
                    coordinate -= binomialCoefficient;
                    cube.EdgePermutation[edge] = (Edge)(NumUdEdges - 1 - numDEdgesDone++);
                }
                else
                {
                    if (numNonDEdgesDone < 4)
                        cube.EdgePermutation[edge] = (Edge)(NumEdges - 1 - numNonDEdgesDone++);
                    else
                        cube.EdgePermutation[edge] = (Edge)(NumUEdges - 1 - numNonDEdgesDone++);
                }
            }
        }

        /// <summary>
        /// Map the D-edge order of a <see cref="CubieCube"/> to an integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the D-edge order coordinate.
        /// </param>
        /// <returns>
        /// The D-edge order coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetDEdgeOrder(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x0123;
            int coordinate = 0;
            int dEdgeIndex = 3;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last corner is skipped because the information is redundant
            {
                int edge = (int)cube.EdgePermutation[edgeIndex];
                if (edge < (int)Edge.DF || edge > (int)Edge.DR) //if edge is not a D-layer edge, skip it
                    continue;
                int numberOfLeftEdgesWithHigherOrder = (list >> ((edge - NumUEdges) * 4)) & 0xF;
                coordinate += factorial[dEdgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x111 >> ((NumUdEdges - 1 - edge) * 4);
                if (dEdgeIndex > 0)
                    dEdgeIndex--;
                else
                    break;
            }
            return coordinate;
        }

        /// <summary>
        /// Set the D-edge order of a <see cref="CubieCube"/> using a
        /// coordinate. Leaves the permutation of all non-D-edges intact.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the D-edge order.
        /// </param>
        /// <param name="coordinate">The D-edge order coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range
        /// </exception>
        public static void SetDEdgeOrder(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgeOrders)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            //IMPR
            int list = 0x4567;
            IEnumerator<(Edge edge, int index)> edgeEnumerator = cube.EdgePermutation
                .Select((edge, index) => (edge, index))
                .Reverse()
                .GetEnumerator();
            for (int dEdgeIndex = 3; dEdgeIndex > 0; dEdgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[dEdgeIndex];
                coordinate %= factorial[dEdgeIndex];
                int edge = (list >> (numberOfLeftEdgesWithHigherOrder * 4)) & 0xF;
                while (edgeEnumerator.MoveNext() && (edgeEnumerator.Current.edge < Edge.DF || edgeEnumerator.Current.edge > Edge.DR)) ; //find the next D-layer edge on the cube
                cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)edge;
                list = ((list & (0xFFF0 << (numberOfLeftEdgesWithHigherOrder * 4))) >> 4) + (list & (0xFFFF >> ((NumDEdges - numberOfLeftEdgesWithHigherOrder) * 4)));
            }
            while (edgeEnumerator.MoveNext() && (edgeEnumerator.Current.edge < Edge.DF || edgeEnumerator.Current.edge > Edge.DR)) ; //find the last D-layer edge on the cube
            cube.EdgePermutation[edgeEnumerator.Current.index] = (Edge)(list & 0xF);
        }

        /// <summary>
        /// Map the D-edge permutation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the D-edge permutation coordinate.
        /// </param>
        /// <returns>
        /// The D-edge permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetDEdgePermutation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            return GetDEdgeDistribution(cube) * NumDEdgeOrders + GetDEdgeOrder(cube);
        }

        /// <summary>
        /// Set the D-edge permutation of a <see cref="CubieCube"/> using a
        /// coordinate. Mixes up the permutation of the non-D-edges.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the D-edge permutation.
        /// </param>
        /// <param name="coordinate">The D-edge permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetDEdgePermutation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumDEdgePermutations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            SetDEdgeDistribution(cube, coordinate / 24);
            SetDEdgeOrder(cube, coordinate % 24);
        }

        //TODO add all required parameter checks
        /// <summary>
        /// Combine a U-edge permutation coordinate and a D-edge order
        /// coordinate to form the corresponding U- and D-edge permutation
        /// coordinate.
        /// </summary>
        /// <param name="uEdgePermutation">
        /// The U-edge permutation coordinate.
        /// </param>
        /// <param name="dEdgeOrder">
        /// The D-edge order coordinate.
        /// </param>
        /// <returns>
        /// The U- and D-edge order coordinate created from the given U-edge
        /// permutation and D-edge order coordinates.
        /// </returns>
        public static int CombineUEdgePermutationAndDEdgeOrder(int uEdgePermutation, int dEdgeOrder)
        {
            if ((uint)uEdgePermutation >= NumUEdgePermutationsPhase2)
                throw new ArgumentOutOfRangeException(nameof(uEdgePermutation) + " is out of range: " + uEdgePermutation);
            if ((uint)dEdgeOrder >= NumDEdgeOrders)
                throw new ArgumentOutOfRangeException(nameof(dEdgeOrder) + " is out of range: " + dEdgeOrder);

            int uEdgeDistribution = uEdgePermutation / NumUEdgeOrders;
            int uEdgeOrder  = uEdgePermutation % NumUEdgeOrders;

            int coordinate = 0;

            int uEdgeList = 0x0123;
            int dEdgeList = 0x4567;

            int list = 0x01234567;

            int numUEdgesDone = 0, numDEdgesDone = 0;
            for (int edgeIndex = NumUdEdges - 1; edgeIndex >= 0; edgeIndex--)
            {
                int edge;
                int bc = Utils.BinomialCoefficient(edgeIndex, 4 - numUEdgesDone);
                if (uEdgeDistribution >= bc) //current edge is a U-layer edge
                {
                    uEdgeDistribution -= bc;

                    int numberOfLeftUEdgesWithHigherOrder = uEdgeOrder / factorial[NumUEdges - 1 - numUEdgesDone];
                    uEdgeOrder %= factorial[NumUEdges - 1 - numUEdgesDone];
                    edge = (uEdgeList >> (numberOfLeftUEdgesWithHigherOrder * 4)) & 0xF;
                    uEdgeList = ((uEdgeList & (0xFFF0 << (numberOfLeftUEdgesWithHigherOrder * 4))) >> 4) + (uEdgeList & (0xFFFF >> ((NumUEdges - numberOfLeftUEdgesWithHigherOrder) * 4)));
                    numUEdgesDone++;
                }
                else
                {
                    int numberOfLeftDEdgesWithHigherOrder = dEdgeOrder / factorial[NumDEdges - 1 - numDEdgesDone];
                    dEdgeOrder %= factorial[NumDEdges - 1 - numDEdgesDone];
                    edge = (dEdgeList >> (numberOfLeftDEdgesWithHigherOrder * 4)) & 0xF;
                    dEdgeList = ((dEdgeList & (0xFFF0 << (numberOfLeftDEdgesWithHigherOrder * 4))) >> 4) + (dEdgeList & (0xFFFF >> ((NumDEdges - numberOfLeftDEdgesWithHigherOrder) * 4)));
                    numDEdgesDone++;
                }

                int numberOfLeftEdgesWithHigherOrder = (list >> edge * 4) & 0xF;
                coordinate += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x1111111 >> (NumUdEdges - 1 - edge) * 4;
            }

            return coordinate;
        }

        /// <summary>
        /// Map the edge orientation and equator edge distribution of a
        /// <see cref="CubieCube"/> to an integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the combined edge orientation and equator
        /// edge distribution coordinate.
        /// </param>
        /// <returns>
        /// The combined edge orientation and equator edge distribution
        /// coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEoEquatorCoord(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int edgeOrientation = GetEdgeOrientation(cube);
            int equatorDistribution = GetEquatorDistribution(cube);
            int eoEquator = NumEdgeOrientations * equatorDistribution + edgeOrientation;

            return eoEquator;
        }

        /// <summary>
        /// Set the edge orientation and the equator edge distribution of a
        /// <see cref="CubieCube"/> using a coordinate. Mixes up the order of
        /// the non-equator edges.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> of which to set the edge orientation
        /// and equator edge distribution.
        /// </param>
        /// <param name="coordinate">
        /// The combined edge orientation and equator edge permutation
        /// coordinate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEoEquatorCoord(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((ulong)coordinate >= NumEoEquatorCoords)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range.");

            int edgeOrientation = coordinate % NumEdgeOrientations;
            SetEdgeOrientation(cube, edgeOrientation);

            int equatorDistribution = coordinate / NumEdgeOrientations;
            SetEquatorDistribution(cube, equatorDistribution);
        }

        /// <summary>
        /// Map the corner permutation of a <see cref="CubieCube"/> to an
        /// integer.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> of which to get the corner permutation
        /// coordinate.
        /// </param>
        /// <returns>
        /// The corner permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetCornerPermutation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x01234567;
            int coordinate = 0;
            for (int cornerIndex = NumCorners - 1; cornerIndex > 0; cornerIndex--) //last corner is skipped because the information is redundant
            {
                int corner = (int)cube.CornerPermutation[cornerIndex];
                int numberOfLeftCornersWithHigherOrder = (list >> corner * 4) & 0xF;
                coordinate += factorial[cornerIndex] * numberOfLeftCornersWithHigherOrder;

                list -= 0x1111111 >> ((NumCorners - 1 - corner) * 4);
            }

            return coordinate;
        }

        /// <summary>
        /// Set the corner permutation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The <see cref="CubieCube"/> of which to set the corner permutation.
        /// </param>
        /// <param name="coordinate">The corner permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetCornerPermutation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumCornerPermutations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int list = 0x01234567; //used as a faster alternative to a list of corners
            for (int cornerIndex = NumCorners - 1; cornerIndex > 0; cornerIndex--)
            {
                int numberOfLeftCornersWithHigherOrder = coordinate / factorial[cornerIndex];
                int corner = (list >> numberOfLeftCornersWithHigherOrder * 4) & 0xF;

                list = (list & (0xFFFFFFF >> (NumCorners - 1 - numberOfLeftCornersWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftCornersWithHigherOrder * 4)) >> 4); //remove corner form list
                coordinate %= factorial[cornerIndex]; //remove corner from coordinate

                cube.CornerPermutation[cornerIndex] = (Corner)corner;
            }

            cube.CornerPermutation[0] = (Corner)list; //last corner is the only one remaining in the list
        }

        /// <summary>
        /// Map the edge permutation of a <see cref="CubieCube"/> to an integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the edge permutation coordinate.
        /// </param>
        /// <returns>
        /// The edge permutation coordinate of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetEdgePermutation(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            long list = 0x0123456789AB;
            int coordinate = 0;
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--) //last edge is skipped because the information is redundant
            {
                int edge = (int)cube.EdgePermutation[edgeIndex];
                int numberOfLeftEdgesWithHigherOrder = (int)((list >> edge * 4) & 0xF);
                coordinate += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x11111111111 >> ((NumEdges - 1 - edge) * 4);
            }

            return coordinate;
        }

        /// <summary>
        /// Set the edge permutation of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the edge permutation.
        /// </param>
        /// <param name="coordinate">The edge permutation coordinate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetEdgePermutation(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumEdgePermutations)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            long list = 0x0123456789AB; //used as a faster alternative to a list of edges
            for (int edgeIndex = NumEdges - 1; edgeIndex > 0; edgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[edgeIndex];
                int edge = (int)((list >> numberOfLeftEdgesWithHigherOrder * 4) & 0xF);

                list = (list & (0xFFFFFFFFFFF >> (NumEdges - 1 - numberOfLeftEdgesWithHigherOrder) * 4)) + ((list & (0xFFFFFFFFFF0 << numberOfLeftEdgesWithHigherOrder * 4)) >> 4); //remove edge form list
                coordinate %= factorial[edgeIndex]; //remove edge from coordinate

                cube.EdgePermutation[edgeIndex] = (Edge)edge;
            }

            cube.EdgePermutation[0] = (Edge)list; //last edge is the only one remaining in the list
        }

        /// <summary>
        /// Map the U- and D-edge order of a <see cref="CubieCube"/> to
        /// an integer.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to get the U- and D-edge order.
        /// </param>
        /// <returns>
        /// The U- and D-edge order of <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static int GetUdEdgeOrder(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int list = 0x01234567;
            int coordinate = 0;
            for (int edgeIndex = NumUdEdges - 1; edgeIndex > 0; edgeIndex--) //last edge is skipped because the information is redundant
            {
                int edge = (int)cube.EdgePermutation[edgeIndex];
                int numberOfLeftEdgesWithHigherOrder = (list >> edge * 4) & 0xF;
                coordinate += factorial[edgeIndex] * numberOfLeftEdgesWithHigherOrder;

                list -= 0x1111111 >> (NumUdEdges - 1 - edge) * 4;
            }

            return coordinate;
        }
        
        /// <summary>
        /// Set the U- and D-edge order of a <see cref="CubieCube"/> using a
        /// coordinate.
        /// </summary>
        /// <param name="cube">
        /// The cube of which to set the U- and D-edge order.
        /// </param>
        /// <param name="coordinate">
        /// The U- and D-edge order coordinate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static void SetUdEdgeOrder(CubieCube cube, int coordinate)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");
            if ((uint)coordinate >= NumUdEdgeOrders)
                throw new ArgumentOutOfRangeException(nameof(coordinate) + " is out of range: " + coordinate);

            int list = 0x01234567; //used as a faster alternative to a list of corners
            for (int edgeIndex = NumUdEdges - 1; edgeIndex > 0; edgeIndex--)
            {
                int numberOfLeftEdgesWithHigherOrder = coordinate / factorial[edgeIndex];
                int edge = (list >> numberOfLeftEdgesWithHigherOrder * 4) & 0xF;

                list = (list & (0xFFFFFFF >> (NumUdEdges - 1 - numberOfLeftEdgesWithHigherOrder) * 4)) + ((list & (0xFFFFFF0 << numberOfLeftEdgesWithHigherOrder * 4)) >> 4); //remove edge form list
                coordinate %= factorial[edgeIndex]; //remove corner from coordinate

                cube.EdgePermutation[edgeIndex] = (Edge)edge;
            }

            cube.EdgePermutation[0] = (Edge)list; //last corner is the only one remaining in the list
        }

        /// <summary>
        /// Calculate the parity of the corners from a corner permutation
        /// coordinate. True corresponds to odd parity and false to even parity.
        /// </summary>
        /// <param name="coordinate">
        /// The corner permutation coordinate.
        /// </param>
        /// <returns>
        /// The parity of the corners for the given corner permutation
        /// coordinate.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static bool CornerPermutationParity(int coordinate)
        {
            if ((uint)coordinate >= NumCornerPermutations)
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
        /// Calculate the parity of the edges from a edge permutation
        /// coordinate. True corresponds to odd parity and false to even parity.
        /// </summary>
        /// <param name="coordinate">
        /// The edge permutation coordinate.
        /// </param>
        /// <returns>
        /// The parity of the edges for the given edge permutation coordinate.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="coordinate"/> is out of range.
        /// </exception>
        public static bool EdgePermutationParity(int coordinate)
        {
            if ((uint)coordinate >= NumEdgePermutations)
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
using System.Linq;
using static Cubing.ThreeByThree.TwoPhase.Coordinates;
using static Cubing.ThreeByThree.TwoPhase.Symmetries;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the symmetry tables used to generate the pruning tables for
    /// the two-phase solver.
    /// </summary>
    public static class SymmetryReduction
    {
        /// <summary>
        /// The number of different symmetry reduced combined edge orientation
        /// and equator distribution coordinates.
        /// </summary>
        public const int NumEoEquatorSymmetryClasses = 64430;

        /// <summary>
        /// The number of different symmetry reduced corner permutation
        /// coordinates.
        /// </summary>
        public const int NumCornerPermutationSymmetryClasses = 2768;

        /// <summary>
        /// <c><see cref="ConjugateCornerOrientationCoordinate"/>[co, s]</c>
        /// conjugates the corner orientation <c>co</c> with the symmetry
        /// represented by the index <c>s</c>.
        /// (<c>s</c> * <c>co</c> * <c>s</c> ^ -1)
        /// </summary>
        public static int[,] ConjugateCornerOrientationCoordinate = null;

        /// <summary>
        /// Reduce a combined edge orientation and equator distribution
        /// coordinate to a coordinate shared by all of its symmetries.
        /// </summary>
        public static int[] ReduceEoEquatorCoordinate = null;

        /// <summary>
        /// Get an expanded representative of a reduced edge orientation and
        /// equator distribution coordinate.
        /// </summary>
        public static int[] ExpandEoEquatorCoordinate = null;

        /// <summary>
        /// Stores the index of the symmetry needed to get from the orientation
        /// of an expanded combined edge orientation and equator distribution
        /// coordinate to its representative.
        /// </summary>
        public static int[] EoEquatorReductionSymmetry = null;

        /// <summary>
        /// Stores flags for the symmetries of combined edge orientation and
        /// equator distribution coordinates.
        /// <c><see cref="EoEquatorSymmetries"/>[c] &gt;&gt; s</c> expresses
        /// whether the reduced combined equator orientation and edge
        /// distribution coordinate <c>c</c> is symmetrical along the symmetry
        /// represented by the index<c>s</c>.
        /// </summary>
        public static int[] EoEquatorSymmetries = null;

        /// <summary>
        /// <c><see cref="ConjugateUdEdgeOrderCoordinate"/>[udEdges, s]</c>
        /// conjugates the U- and D-edge order <c>udEdges</c> with the
        /// symmetry represented by the index <c>s</c>.
        /// (<c>s</c> * <c>udEdges</c> * <c>s</c> ^ -1)
        /// </summary>
        public static int[,] ConjugateUdEdgeOrderCoordinate = null;

        /// <summary>
        /// Reduce a corner permutation to a coordinate shared by all of its
        /// symmetries.
        /// </summary>
        public static int[] ReduceCornerPermutationCoordinate = null;

        /// <summary>
        /// Get an expanded representative of a reduced corner permutation
        /// coordinate.
        /// </summary>
        public static int[] ExpandCornerPermutationCoordinate = null;

        /// <summary>
        /// Stores the index of the symmetry needed to get from the orientation
        /// of an expanded corner permutation coordinate to its representative.
        /// </summary>
        public static int[] CornerPermutationReductionSymmetry = null;

        /// <summary>
        /// Stores flags for the symmetries of corner permutation coordinates.
        /// <c><see cref="CornerPermutationSymmetries"/>[i] &gt;&gt; s</c>
        /// expresses whether the reduced corner permutation coordinate <c>c</c>
        /// is symmetrical along the symmetry represented by the index <c>s</c>.
        /// </summary>
        public static int[] CornerPermutationSymmetries = null;

        static SymmetryReduction()
        {
            //avoid naming conflicts
            CubieCube cube;
            int reducedEoEquator, reducedCornerPermutation;

            #region initialize ConjugateCoCoordinate
            ConjugateCornerOrientationCoordinate = new int[NumCornerOrientations, NumSymmetriesDh4];

            //invalidate
            for (int cornerOrientation = 0; cornerOrientation < NumCornerOrientations; cornerOrientation++)
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                    ConjugateCornerOrientationCoordinate[cornerOrientation, symmetryIndex] = -1;

            //populate
            cube = CubieCube.CreateSolved();
            for (int cornerOrientation = 0; cornerOrientation < NumCornerOrientations; cornerOrientation++)
            {
                cube.IsMirrored = false; //TEST if necessary
                SetCornerOrientation(cube, cornerOrientation);
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                {
                    //conjugate cube
                    CubieCube symmetryCube = SymmetryCubes[symmetryIndex].Clone();
                    symmetryCube.MultiplyCorners(cube);
                    symmetryCube.MultiplyCorners(SymmetryCubes[InverseIndex[symmetryIndex]]);

                    //store result
                    ConjugateCornerOrientationCoordinate[cornerOrientation, symmetryIndex] = GetCornerOrientation(symmetryCube);
                }
            }
            #endregion initialize ConjugateCoCoordinate

            //TODO simplify
            #region initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and EoEquatorReductionSymmetry
            //initialize and invalidate
            ReduceEoEquatorCoordinate  = Enumerable.Repeat(-1, NumEquatorDistributions * NumEdgeOrientations)
                                                   .ToArray();
            ExpandEoEquatorCoordinate  = Enumerable.Repeat(-1, NumEoEquatorSymmetryClasses)
                                                   .ToArray();
            EoEquatorReductionSymmetry = Enumerable.Repeat(-1, NumEquatorDistributions * NumEdgeOrientations)
                                                   .ToArray();

            //populate
            cube = CubieCube.CreateSolved();
            reducedEoEquator = 0;
            for (int equatorDistribution = 0; equatorDistribution < NumEquatorDistributions; equatorDistribution++)
            {
                SetEquatorDistribution(cube, equatorDistribution);
                for (int edgeOrientation = 0; edgeOrientation < NumEdgeOrientations; edgeOrientation++)
                {
                    int eoEquator = NumEdgeOrientations * equatorDistribution + edgeOrientation;
                    if (ReduceEoEquatorCoordinate[eoEquator] == -1)
                    {
                        SetEdgeOrientation(cube, edgeOrientation);

                        //store representative
                        ReduceEoEquatorCoordinate[eoEquator] = reducedEoEquator;
                        EoEquatorReductionSymmetry[eoEquator] = 0;
                        ExpandEoEquatorCoordinate[reducedEoEquator] = eoEquator;

                        //go through all symmetries of the current cube and set
                        //their reduced index to the same full index
                        for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                        {
                            //symmetry ^ -1 * cube * symmetry
                            CubieCube symCube = SymmetryCubes[InverseIndex[symmetryIndex]].Clone();
                            symCube.MultiplyEdges(cube);
                            symCube.MultiplyEdges(SymmetryCubes[symmetryIndex]);

                            //store
                            int newEdgeOrientation = GetEdgeOrientation(symCube);
                            int newEquatorDistribution = GetEquatorDistribution(symCube);
                            int newEoEquator = NumEdgeOrientations * newEquatorDistribution + newEdgeOrientation;
                            //TODO test if necessary
                            if (ReduceEoEquatorCoordinate[newEoEquator] == -1)
                            {
                                ReduceEoEquatorCoordinate[newEoEquator] = reducedEoEquator;
                                EoEquatorReductionSymmetry[newEoEquator] = symmetryIndex;
                            }
                        }
                        reducedEoEquator++;
                    }
                }
            }
            #endregion initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and EoEquatorReductionSymmetry

            #region initialize EoEquatorSymmetries
            //initialize and invalidate
            EoEquatorSymmetries = Enumerable.Repeat(0, NumEoEquatorSymmetryClasses)
                                            .ToArray();

            //populate
            cube = CubieCube.CreateSolved();
            for (reducedEoEquator = 0; reducedEoEquator < NumEoEquatorSymmetryClasses; reducedEoEquator++)
            {
                //set cube to the representative of the reduced index
                int eoEquator = ExpandEoEquatorCoordinate[reducedEoEquator];
                SetEoEquatorCoord(cube, eoEquator);

                //find all symmetries of the cube
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                {
                    //symmetry * cube * symmetry ^ -1
                    CubieCube symmetryCube = SymmetryCubes[symmetryIndex].Clone();
                    symmetryCube.MultiplyEdges(cube);
                    symmetryCube.MultiplyEdges(SymmetryCubes[InverseIndex[symmetryIndex]]);
                    //set flag if symmetrical
                    int newEoEquator = GetEoEquatorCoord(symmetryCube);
                    if (eoEquator == newEoEquator)
                        EoEquatorSymmetries[reducedEoEquator] |= 1 << symmetryIndex;
                }
            }
            #endregion initialize EoEoquatorSymmetries

            #region initialize ConjugateUdEdgeOrderCoordinate
            ConjugateUdEdgeOrderCoordinate = new int[NumUdEdgeOrders, NumSymmetriesDh4];

            //invalidate
            for (int udEdgeOrders = 0; udEdgeOrders < NumUdEdgeOrders; udEdgeOrders++)
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                    ConjugateUdEdgeOrderCoordinate[udEdgeOrders, symmetryIndex] = -1;

            //populate
            cube = CubieCube.CreateSolved();
            for (int udEdgePermutation = 0; udEdgePermutation < NumUdEdgeOrders; udEdgePermutation++)
            {
                SetUdEdgeOrder(cube, udEdgePermutation);
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                {
                    //conjugate cube
                    CubieCube symmetryCube = SymmetryCubes[symmetryIndex].Clone();
                    symmetryCube.MultiplyEdges(cube);
                    symmetryCube.MultiplyEdges(SymmetryCubes[InverseIndex[symmetryIndex]]);

                    //store result
                    ConjugateUdEdgeOrderCoordinate[udEdgePermutation, symmetryIndex] = GetUdEdgeOrder(symmetryCube);
                }
            }
            #endregion initialize ConjugateUdEdgeOrderCoordinate

            #region initialize ReduceCpCoordinate, ExpandCpCoordinate and CpReductionSymmetry
            //initialize and invalidate
            ReduceCornerPermutationCoordinate  = Enumerable.Repeat(-1, NumCornerPermutations)
                                                           .ToArray();
            ExpandCornerPermutationCoordinate  = Enumerable.Repeat(-1, NumCornerPermutationSymmetryClasses)
                                                           .ToArray();
            CornerPermutationReductionSymmetry = Enumerable.Repeat(-1, NumCornerPermutations)
                                                           .ToArray();

            //populate
            cube = CubieCube.CreateSolved();
            reducedCornerPermutation = 0;
            for (int cornerPermutation = 0; cornerPermutation < NumCornerPermutations; cornerPermutation++)
            {
                if (ReduceCornerPermutationCoordinate[cornerPermutation] == -1)
                {
                    cube.IsMirrored = false; //TEST if necessary
                    SetCornerPermutation(cube, cornerPermutation);

                    //store representative
                    ReduceCornerPermutationCoordinate[cornerPermutation] = reducedCornerPermutation;
                    CornerPermutationReductionSymmetry[cornerPermutation] = 0;
                    ExpandCornerPermutationCoordinate[reducedCornerPermutation] = cornerPermutation;

                    //go through all symmetries of the current cube and set
                    //their reduced index to the same full index
                    for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                    {
                        //symmetry ^ -1 * cube * symmetry
                        CubieCube symmetryCube = SymmetryCubes[InverseIndex[symmetryIndex]].Clone();
                        symmetryCube.MultiplyCorners(cube);
                        symmetryCube.MultiplyCorners(SymmetryCubes[symmetryIndex]);

                        //store
                        int newCornerPermutation = GetCornerPermutation(symmetryCube);
                        //TODO test if necessary
                        if (ReduceCornerPermutationCoordinate[newCornerPermutation] == -1)
                        {
                            ReduceCornerPermutationCoordinate[newCornerPermutation] = reducedCornerPermutation;
                            CornerPermutationReductionSymmetry[newCornerPermutation] = symmetryIndex;
                        }
                    }
                    reducedCornerPermutation++;
                }
            }
            #endregion initialize ReduceCpCoordinate, ExpandCpCoordinate and CpReductionSymmetry

            #region initialize CpSymmetries
            CornerPermutationSymmetries = Enumerable.Repeat(0, NumCornerPermutationSymmetryClasses)
                                                    .ToArray();

            cube = CubieCube.CreateSolved();
            for (reducedCornerPermutation = 0; reducedCornerPermutation < NumCornerPermutationSymmetryClasses; reducedCornerPermutation++)
            {
                //set cube to the representative of the reduced index
                int cornerPermutation = ExpandCornerPermutationCoordinate[reducedCornerPermutation];
                SetCornerPermutation(cube, cornerPermutation);

                //find all symmetries of the cube
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetriesDh4; symmetryIndex++)
                {
                    //symmetry * cube * symmetry ^ -1
                    CubieCube symmetryCube = SymmetryCubes[symmetryIndex].Clone();
                    symmetryCube.MultiplyCorners(cube);
                    symmetryCube.MultiplyCorners(SymmetryCubes[InverseIndex[symmetryIndex]]);
                    //set flag if symmetrical
                    int newCornerPermutation = GetCornerPermutation(symmetryCube);
                    if (cornerPermutation == newCornerPermutation)
                        CornerPermutationSymmetries[reducedCornerPermutation] |= 1 << symmetryIndex;
                }
            }
            #endregion initialize CpSymmetries
        }
    }
}
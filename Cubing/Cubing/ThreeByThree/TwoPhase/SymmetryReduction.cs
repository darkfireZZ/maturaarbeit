using static Cubing.ThreeByThree.Coordinates;
using static Cubing.ThreeByThree.TwoPhase.TwoPhaseConstants;
using System;
using System.Linq;

//IMPR documentation
namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the tables used to generate the pruning tables.
    /// </summary>
    public static class SymmetryReduction
    {
        /// <summary>
        /// <c><see cref="ConjugateCoCoordinate"/>[co, s]</c> conjugates the
        /// corner orientation <c>co</c> with the symmetry <c>s</c> (<c>s</c>
        /// * <c>co</c> * <c>s</c> ^ -1).
        /// </summary>
        public static int[,] ConjugateCoCoordinate = null;

        /// <summary>
        /// Reduce the eo-equator coordinate to a coordinate shared by all its
        /// symmetries.
        /// </summary>
        public static int[] ReduceEoEquatorCoordinate = null;

        /// <summary>
        /// Get an expanded representative of a reduced eo-equator coordinate.
        /// </summary>
        public static int[] ExpandEoEquatorCoordinate = null;

        /// <summary>
        /// Stores the symmetry needed to get from the orientation of a full
        /// eo-equator coordinate to its representative.
        /// </summary>
        public static int[] EoEquatorReductionSymmetry = null;

        /// <summary>
        /// Stores flags for the symmetries of reduced eo-equator coordinates.
        /// <c><see cref="EoEquatorSymmetries"/>[i] &gt;&gt;
        /// s</c> is the flag for whether the reduced eo-equator coordinate
        /// <c>i</c> is symmetrical along <c>s</c>.
        /// </summary>
        public static int[] EoEquatorSymmetries = null;

        /// <summary>
        /// <c><see cref="ConjugateUdEdgePermutationCoordinate"/>[udEdges, s]</c>
        /// conjugates the corner orientation <c>udEdges</c> with the symmetry
        /// <c>s</c> (<c>s</c> * <c>udEdges</c> * <c>s</c> ^ -1).
        /// </summary>
        public static int[,] ConjugateUdEdgePermutationCoordinate = null;

        /// <summary>
        /// Reduce the corner permutation to a coordinate shared by all its
        /// symmetries.
        /// </summary>
        public static int[] ReduceCpCoordinate = null;

        /// <summary>
        /// Get an expanded representative of a reduced corner permutation
        /// coordinate.
        /// </summary>
        public static int[] ExpandCpCoordinate = null;

        /// <summary>
        /// Stores the symmetry needed to get from the orientation of a full
        /// corner permutation coordinate to its representative.
        /// </summary>
        public static int[] CpReductionSymmetry = null;

        /// <summary>
        /// Stores flags for the symmetries of reduced corner permutation
        /// coordinates. <c><see cref="CpSymmetries"/>[i] &gt;&gt; s</c> is the
        /// flag for whether the reduced corner permutation coordinate <c>i</c>
        /// is symmetrical along <c>s</c>.
        /// </summary>
        public static int[] CpSymmetries = null;

        static SymmetryReduction()
        {
            //avoid naming conflicts
            CubieCube cube;
            int reducedEoEquator, reducedCp;

            #region initialize ConjugateCoCoordinate
            ConjugateCoCoordinate = new int[NumCoCoords, NumSymmetriesDh4];

            //invalidate
            for (int co = 0; co < NumCoCoords; co++)
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                    ConjugateCoCoordinate[co, sym] = -1;

            //populate
            cube = CubieCube.CreateSolved();
            for (int co = 0; co < NumCoCoords; co++)
            {
                cube.IsMirrored = false;
                SetCoCoord(cube, co);
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                {
                    //conjugate cube
                    CubieCube symmetryCube = Symmetries.SymmetryCubes[sym].Clone();
                    symmetryCube.MultiplyCorners(cube);
                    symmetryCube.MultiplyCorners(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    //store result
                    ConjugateCoCoordinate[co, sym] = GetCoCoord(symmetryCube);
                }
            }
            #endregion initialize ConjugateCoCoordinate

            #region initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and EoEquatorReductionSymmetry
            //initialize and invalidate arrays
            ReduceEoEquatorCoordinate  = Enumerable.Repeat(-1, NumEquatorDistributionCoords * NumEoCoords)
                                                   .ToArray();
            ExpandEoEquatorCoordinate  = Enumerable.Repeat(-1, NumReducedEoEquatorCoordinates)
                                                   .ToArray();
            EoEquatorReductionSymmetry = Enumerable.Repeat(-1, NumEquatorDistributionCoords * NumEoCoords)
                                                   .ToArray();

            cube = CubieCube.CreateSolved();
            reducedEoEquator = 0;
            for (int equator = 0; equator < NumEquatorDistributionCoords; equator++)
            {
                SetEquatorDistributionCoord(cube, equator);
                for (int eo = 0; eo < NumEoCoords; eo++)
                {
                    int eoEquator = NumEoCoords * equator + eo;
                    if (ReduceEoEquatorCoordinate[eoEquator] == -1)
                    {
                        SetEoCoord(cube, eo);

                        //store representative
                        ReduceEoEquatorCoordinate[eoEquator] = reducedEoEquator;
                        EoEquatorReductionSymmetry[eoEquator] = 0;
                        ExpandEoEquatorCoordinate[reducedEoEquator] = eoEquator;

                        //go through all symmetries of the current cube and set
                        //their reduced index to the same full index
                        for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                        {
                            //sym ^ -1 * cube * sym
                            CubieCube symCube = Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]].Clone();
                            symCube.MultiplyEdges(cube);
                            symCube.MultiplyEdges(Symmetries.SymmetryCubes[sym]);

                            //store
                            int newEoCoord = GetEoCoord(symCube);
                            int newEquatorCoord = GetEquatorDistributionCoord(symCube);
                            int newEoEquator = NumEoCoords * newEquatorCoord + newEoCoord;
                            //TODO test if necessary
                            if (ReduceEoEquatorCoordinate[newEoEquator] == -1)
                            {
                                ReduceEoEquatorCoordinate[newEoEquator] = reducedEoEquator;
                                EoEquatorReductionSymmetry[newEoEquator] = sym;
                            }
                        }
                        reducedEoEquator++;
                    }
                }
            }
            #endregion initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and EoEquatorReductionSymmetry

            #region initialize EoEquatorSymmetries
            EoEquatorSymmetries = Enumerable.Repeat(0, NumReducedEoEquatorCoordinates)
                                            .ToArray();

            cube = CubieCube.CreateSolved();
            for (reducedEoEquator = 0; reducedEoEquator < NumReducedEoEquatorCoordinates; reducedEoEquator++)
            {
                //set cube to the representative of the reduced index
                int eoEquator = ExpandEoEquatorCoordinate[reducedEoEquator];
                SetEoEquatorCoord(cube, eoEquator);

                //find all symmetries of the cube
                for (int symIndex = 0; symIndex < NumSymmetriesDh4; symIndex++)
                {
                    //symCube = symIndex * cube * symIndex ^ -1
                    CubieCube symCube = Symmetries.SymmetryCubes[symIndex].Clone();
                    symCube.MultiplyEdges(cube);
                    symCube.MultiplyEdges(Symmetries.SymmetryCubes[Symmetries.InverseIndex[symIndex]]);
                    //set flag if symmetry
                    int newEoEquator = GetEoEquatorCoord(symCube);
                    if (eoEquator == newEoEquator)
                        EoEquatorSymmetries[reducedEoEquator] |= 1 << symIndex;
                }
            }
            #endregion initialize EoEoquatorSymmetries

            #region initialize ConjugateUdEdgePermutationCoordinate
            ConjugateUdEdgePermutationCoordinate = new int[NumUdEdgePermutationCoords, NumSymmetriesDh4];

            //invalidate
            for (int udEdges = 0; udEdges < NumUdEdgePermutationCoords; udEdges++)
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                    ConjugateUdEdgePermutationCoordinate[udEdges, sym] = -1;

            //populate
            cube = CubieCube.CreateSolved();
            for (int udEdges = 0; udEdges < NumUdEdgePermutationCoords; udEdges++)
            {
                SetUdEdgePermutationCoord(cube, udEdges);
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                {
                    //conjugate cube
                    CubieCube symmetryCube = Symmetries.SymmetryCubes[sym].Clone();
                    symmetryCube.MultiplyEdges(cube);
                    symmetryCube.MultiplyEdges(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    //store result
                    ConjugateUdEdgePermutationCoordinate[udEdges, sym] = GetUdEdgePermutationCoord(symmetryCube);
                }
            }
            #endregion initialize ConjugateUdEdgePermutationCoordinate

            #region initialize ReduceCpCoordinate, ExpandCpCoordinate and CpReductionSymmetry
            //initialize and invalidate arrays
            ReduceCpCoordinate  = Enumerable.Repeat(-1, NumCpCoords)
                                            .ToArray();
            ExpandCpCoordinate  = Enumerable.Repeat(-1, NumReducedCornerPermutationCoordinates)
                                            .ToArray();
            CpReductionSymmetry = Enumerable.Repeat(-1, NumCpCoords)
                                            .ToArray();

            cube = CubieCube.CreateSolved();
            reducedCp = 0;
            for (int cp = 0; cp < NumCpCoords; cp++)
            {
                if (ReduceCpCoordinate[cp] == -1)
                {
                    cube.IsMirrored = false; //TEST if necessary
                    SetCpCoord(cube, cp);

                    //store representative
                    ReduceCpCoordinate[cp] = reducedCp;
                    CpReductionSymmetry[cp] = 0;
                    ExpandCpCoordinate[reducedCp] = cp;

                    //go through all symmetries of the current cube and set
                    //their reduced index to the same full index
                    for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                    {
                        //sym ^ -1 * cube * sym
                        CubieCube symCube = Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]].Clone();
                        symCube.MultiplyCorners(cube);
                        symCube.MultiplyCorners(Symmetries.SymmetryCubes[sym]);

                        //store
                        int newCp = GetCpCoord(symCube);
                        //TODO test if necessary
                        if (ReduceCpCoordinate[newCp] == -1)
                        {
                            ReduceCpCoordinate[newCp] = reducedCp;
                            CpReductionSymmetry[newCp] = sym;
                        }
                    }
                    reducedCp++;
                }
            }
            #endregion initialize ReduceCpCoordinate, ExpandCpCoordinate and CpReductionSymmetry

            #region initialize CpSymmetries
            CpSymmetries = Enumerable.Repeat(0, NumReducedCornerPermutationCoordinates)
                                            .ToArray();

            cube = CubieCube.CreateSolved();
            for (reducedCp = 0; reducedCp < NumReducedCornerPermutationCoordinates; reducedCp++)
            {
                //set cube to the representative of the reduced index
                int cp = ExpandCpCoordinate[reducedCp];
                SetCpCoord(cube, cp);

                //find all symmetries of the cube
                for (int symIndex = 0; symIndex < NumSymmetriesDh4; symIndex++)
                {
                    //symCube = symIndex * cube * symIndex ^ -1
                    CubieCube symCube = Symmetries.SymmetryCubes[symIndex].Clone();
                    symCube.MultiplyCorners(cube);
                    symCube.MultiplyCorners(Symmetries.SymmetryCubes[Symmetries.InverseIndex[symIndex]]);
                    //set flag if symmetry
                    int newCp = GetCpCoord(symCube);
                    if (cp == newCp)
                        CpSymmetries[reducedCp] |= 1 << symIndex;
                }
            }
            #endregion initialize CpSymmetries
        }
    }
}
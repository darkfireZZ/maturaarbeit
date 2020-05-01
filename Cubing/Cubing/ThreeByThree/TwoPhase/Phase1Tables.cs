using static Cubing.ThreeByThree.Coordinates;
using static Cubing.ThreeByThree.TwoPhase.TwoPhaseConstants;
using System.Linq;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the tables needed for phase 1 of the two-phase algorithm
    /// except move tables which are located in <see cref="MoveTables"/> and
    /// the pruning table which is located in <see cref="PruningTables"/>.
    /// </summary>
    public static class Phase1Tables
    {
        /// <summary>
        /// <c>ConjugateCoCoordinate[co, s]</c> conjugates the corner orientation <c>co</c> with the symmetry <c>s</c>.
        /// </summary>
        public static int[,] ConjugateCoCoordinate = null;

        /// <summary>
        /// Reduce the eo-equator coordinate to a coordinate that is shared by
        /// all its symmetries.
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
        public static int[] ReductionSymmetry = null;

        //TODO test if short would be enough
        /// <summary>
        /// Stores flags for the symmetries of reduced eo-equtor coordinates.
        /// <c><see cref="ReducedEoEquatorCoordinateSymmetries"/>[i] &gt;&gt;
        /// s</c> is the flag for whether the reduced eo-equator coordinate
        /// <c>i</c> has the symmetry <c>s</c>. Where <c>s</c> is a symmetry
        /// for phase 1.
        /// </summary>
        public static int[] ReducedEoEquatorCoordinateSymmetries = null;

        static Phase1Tables()
        {
            #region initialize ConjugateCoCoordinate
            ConjugateCoCoordinate = new int[NumCoCoords, NumSymmetriesPhase1];

            //invalidate all entries
            for (int co = 0; co < NumCoCoords; co++)
                for (int sym = 0; sym < NumSymmetriesPhase1; sym++)
                    ConjugateCoCoordinate[co, sym] = -1;

            CubieCube coordinateCube = CubieCube.CreateSolved();
            for (int co = 0; co < NumCoCoords; co++)
            {
                coordinateCube.IsMirrored = false; //TODO test if necessary
                SetCoCoord(coordinateCube, co);
                for (int sym = 0; sym < NumSymmetriesPhase1; sym++)
                {
                    //conjugate coordinate cube
                    CubieCube symmetryCube = Symmetries.SymmetryCubes[sym].Clone();
                    symmetryCube.MultiplyCorners(coordinateCube);
                    symmetryCube.MultiplyCorners(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    //store result
                    ConjugateCoCoordinate[co, sym] = GetCoCoord(symmetryCube);
                }
            }
            #endregion initialize ConjugateCoCoordinate

            #region initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and ReductionSymmetry
            //initialize and invalidate arrays
            ReduceEoEquatorCoordinate = Enumerable
                .Repeat(-1, NumEquatorDistributionCoords * NumEoCoords)
                .ToArray();
            ExpandEoEquatorCoordinate = Enumerable
                .Repeat(-1, NumReducedEoEquatorCoordinates)
                .ToArray();
            ReductionSymmetry = Enumerable
                .Repeat(-1, NumEquatorDistributionCoords * NumEoCoords)
                .ToArray();

            CubieCube cube = CubieCube.CreateSolved();
            int reducedIndex = 0;
            for (int equator = 0; equator < NumEquatorDistributionCoords; equator++)
            {
                SetEquatorDistributionCoord(cube, equator);
                for (int eo = 0; eo < NumEoCoords; eo++)
                {
                    int fullIndex = NumEoCoords * equator + eo;
                    if (ReduceEoEquatorCoordinate[fullIndex] == -1)
                    {
                        //store representative
                        ReduceEoEquatorCoordinate[fullIndex] = reducedIndex;
                        ReductionSymmetry[fullIndex] = 0;
                        ExpandEoEquatorCoordinate[reducedIndex] = fullIndex;

                        //go through all symmetries of the current cube and set
                        //their reduced index to the same full index
                        SetEoCoord(cube, eo);
                        for (int sym = 0; sym < NumSymmetriesPhase1; sym++)
                        {
                            //sym ^ -1 * cube * sym
                            CubieCube symCube = Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]].Clone();
                            symCube.MultiplyEdges(cube);
                            symCube.MultiplyEdges(Symmetries.SymmetryCubes[sym]);

                            //store
                            int newEoCoord = GetEoCoord(symCube);
                            int newEquatorCoord = GetEquatorDistributionCoord(symCube);
                            int newFullIndex = NumEoCoords * newEquatorCoord + newEoCoord;
                            //TODO test if necessary
                            if (ReduceEoEquatorCoordinate[newFullIndex] == -1)
                            {
                                ReduceEoEquatorCoordinate[newFullIndex] = reducedIndex;
                                ReductionSymmetry[newFullIndex] = sym;
                            }
                        }
                        reducedIndex++;
                    }
                }
            }
            #endregion initialize ReduceEoEquatorCoordinate, ExpandEoEquatorCoordinate and ReductionSymmetry

            #region initialize ReducedEoEquatorCoordinateSymmetries
            //initialize and invalidate array
            ReducedEoEquatorCoordinateSymmetries = Enumerable
                .Repeat(0, NumReducedEoEquatorCoordinates)
                .ToArray();

            cube = CubieCube.CreateSolved();
            for (reducedIndex = 0; reducedIndex < NumReducedEoEquatorCoordinates; reducedIndex++)
            {
                //set cube to the representative of the reduced index
                int expandedIndex = ExpandEoEquatorCoordinate[reducedIndex];
                int eo = expandedIndex % 2048;
                int equator = expandedIndex / 2048;
                SetEoCoord(cube, eo);
                SetEquatorDistributionCoord(cube, equator);

                //find all symmetries of the cube
                for (int symIndex = 0; symIndex < NumSymmetriesPhase1; symIndex++)
                {
                    //symCube = symIndex * cube * symIndex ^ -1
                    CubieCube symCube = Symmetries.SymmetryCubes[symIndex].Clone();
                    symCube.MultiplyEdges(cube);
                    symCube.MultiplyEdges(Symmetries.SymmetryCubes[Symmetries.InverseIndex[symIndex]]);
                    //set flag if symmetry
                    int newEo = GetEoCoord(symCube);
                    int newEquator = GetEquatorDistributionCoord(symCube);
                    if (eo == newEo && equator == newEquator)
                        ReducedEoEquatorCoordinateSymmetries[reducedIndex] |= 1 << symIndex;
                }
            }
            #endregion initialize ReducedEoEoquatorCoordinateSymmetries
        }
    }
}
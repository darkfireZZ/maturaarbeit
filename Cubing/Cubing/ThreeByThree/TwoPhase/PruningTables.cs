using System;
using System.Linq;
using static Cubing.ThreeByThree.Constants;
using static Cubing.ThreeByThree.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the pruning tables for the two-phase solver.
    /// </summary>
    public static class PruningTables
    {
        //TEST how much worse are param checks
        /// <summary>
        /// Get the index into the phase 1 pruning table given the corner
        /// orientation, edge orientation and equator coordinates.
        /// </summary>
        /// <remarks>
        /// There are no parameter checks in order to speed up the computation.
        /// Therefore make sure that all the parameters are in range.
        /// </remarks>
        /// <param name="co">The corner orientation coordinate.</param>
        /// <param name="eo">The edge orientation coodinate.</param>
        /// <param name="equatorDistribution">
        /// The equator distribution coordinate.
        /// </param>
        /// <returns>
        /// The index into the pruning table for the given parameters.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// May be thrown if any of the parameters is out of range, but does
        /// not need to be.
        /// </exception>
        public static int GetPhase1PruningIndex(int co, int eo, int equatorDistribution)
        {
            int eoEquator = equatorDistribution * NumEoCoords + eo;
            int reducedEoEquator = SymmetryReduction.ReduceEoEquatorCoordinate[eoEquator];
            int reductionSymmetry = SymmetryReduction.EoEquatorReductionSymmetry[eoEquator];
            int rotatedCo = SymmetryReduction.ConjugateCoCoordinate[co, reductionSymmetry];
            int pruningCoordinate = reducedEoEquator * NumCoCoords + rotatedCo;

            return pruningCoordinate;
        }

        //TEST how much worse are param checks
        /// <summary>
        /// Get the index into the phase 2 corner permutation and U- and D-edge
        /// permutation pruning table given the U- and D-edge permutation and
        /// the corner permutation.
        /// </summary>
        /// <remarks>
        /// There are no parameter checks in order to speed up the computation.
        /// Therefore make sure that all the parameters are in range.
        /// </remarks>
        /// <param name="udEdgePermutation">
        /// The U- and D-edge permutation coordinate.
        /// </param>
        /// <param name="cp">The corner permutation coordinate.</param>
        /// <returns>
        /// The index into the pruning table for the given parameters.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// May be thrown if any of the parameters is out of range, but does
        /// not need to be.
        /// </exception>
        public static int GetPhase2CornerUdPruningIndex(int udEdgePermutation, int cp)
        {
            int reducedCp = SymmetryReduction.ReduceCpCoordinate[cp];
            int reductionSymmetry = SymmetryReduction.CpReductionSymmetry[cp];
            int rotatedUdEdgePermutation = SymmetryReduction.ConjugateUdEdgePermutationCoordinate[udEdgePermutation, reductionSymmetry];
            int pruningCoordinate = NumUdEdgePermutationCoords * reducedCp + rotatedUdEdgePermutation;

            return pruningCoordinate;
        }

        //IMPR perf
        /// <summary>
        /// Create the pruning table for phase 1.
        /// </summary>
        /// <returns>The pruning table for phase 1.</returns>
        public static byte[] CreatePhase1Table()
        {
            byte invalid = 0xFF;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.PruningTableSizePhase1)
                .ToArray();
            pruningTable[0] = 0; //solved

            TableController.InitializeCoMoveTable();
            TableController.InitializeEoMoveTable();
            TableController.InitializeEquatorDistributionMoveTable();

            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + TwoPhaseConstants.PruningTableSizePhase1 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.PruningTableSizePhase1));

            while (done < TwoPhaseConstants.PruningTableSizePhase1)
            {
                for (int reducedEoEquator = 0; reducedEoEquator < TwoPhaseConstants.NumReducedEoEquatorCoordinates; reducedEoEquator++)
                    for (int co = 0; co < NumCoCoords; co++)
                    {
                        int index = NumCoCoords * reducedEoEquator + co;
                        if (pruningTable[index] == depth)
                        {
                            int expandedEoEquator = SymmetryReduction.ExpandEoEquatorCoordinate[reducedEoEquator];
                            int eo = expandedEoEquator % NumEoCoords;
                            int equator = expandedEoEquator / NumEoCoords;
                            for (int move = 0; move < NumMoves; move++)
                            {
                                //apply move
                                int newCo = TableController.CoMoveTable[co, move];
                                int newEo = TableController.EoMoveTable[eo, move];
                                int newEquator = TableController.EquatorDistributionMoveTable[equator, move];
                                //same as calling 
                                //GetPruningCoord(newCo, newEo, newEquator),
                                //but the reduced eo-equator coordinate is
                                //required.
                                int newExpandedEoEquator = newEquator * NumEoCoords + newEo;
                                int newReducedEoEquator = SymmetryReduction.ReduceEoEquatorCoordinate[newExpandedEoEquator];
                                int reductionSymmetry = SymmetryReduction.EoEquatorReductionSymmetry[newExpandedEoEquator];
                                newCo = SymmetryReduction.ConjugateCoCoordinate[newCo, reductionSymmetry];
                                int newIndex = newReducedEoEquator * NumCoCoords + newCo;

                                //store depth
                                if (pruningTable[newIndex] == invalid)
                                {
                                    pruningTable[newIndex] = (byte)(depth + 1);
                                    done++;

                                    int flags = SymmetryReduction.EoEquatorSymmetries[newReducedEoEquator];
                                    for (int symmetry = 1; symmetry < TwoPhaseConstants.NumSymmetriesDh4; symmetry++)
                                    {
                                        flags >>= 1;
                                        if ((flags & 0b1) == 1)
                                        {
                                            int rotatedCo = SymmetryReduction.ConjugateCoCoordinate[newCo, symmetry];
                                            int rotatedIndex = NumCoCoords * newReducedEoEquator + rotatedCo;
                                            if (pruningTable[rotatedIndex] == invalid)
                                            {
                                                pruningTable[rotatedIndex] = (byte)(depth + 1);
                                                done++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                depth++;
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.PruningTableSizePhase1));
            }
            return pruningTable;
        }


        //IMRP perf
        /// <summary>
        /// Create the corner permutation and U- and D-edge permutation pruning
        /// table for phase 2.
        /// </summary>
        /// <returns>
        /// The corner permutation and U- and D-edge permutation pruning table
        /// for phase 2.
        /// </returns>
        public static byte[] CreatePhase2CornerUdTable()
        {
            byte invalid = 0xFF;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.CornerUdPruningTableSizePhase2)
                .ToArray();
            pruningTable[0] = 0; //solved

            TableController.InitializeCpMoveTable();
            TableController.InitializeUdEdgePermutationMoveTable();

            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + TwoPhaseConstants.CornerUdPruningTableSizePhase2 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.CornerUdPruningTableSizePhase2));

            while (done < TwoPhaseConstants.CornerUdPruningTableSizePhase2)
            {
                for (int reducedCp = 0; reducedCp < TwoPhaseConstants.NumReducedCornerPermutationCoordinates; reducedCp++)
                    for (int udEdgePermutation = 0; udEdgePermutation < NumUdEdgePermutationCoords; udEdgePermutation++)
                    {
                        int index = NumUdEdgePermutationCoords * reducedCp + udEdgePermutation;
                        if (pruningTable[index] == depth)
                        {
                            int expandedCp = SymmetryReduction.ExpandCpCoordinate[reducedCp];
                            for (int phase2Move = 0; phase2Move < TwoPhaseConstants.NumMovesPhase2; phase2Move++)
                            {
                                //apply move
                                int newUdEdgePermutation = TableController.UdEdgePermutationMoveTable[udEdgePermutation, phase2Move];
                                int newCp = TableController.CpMoveTable[expandedCp, MoveTables.Phase2IndexToPhase1Index[phase2Move]];
                                int newReducedCp = SymmetryReduction.ReduceCpCoordinate[newCp];
                                int reductionSymmetry = SymmetryReduction.CpReductionSymmetry[newCp];
                                newUdEdgePermutation = SymmetryReduction.ConjugateUdEdgePermutationCoordinate[newUdEdgePermutation, reductionSymmetry];
                                int newIndex = NumUdEdgePermutationCoords * newReducedCp + newUdEdgePermutation;

                                //store depth
                                if (pruningTable[newIndex] == invalid)
                                {
                                    pruningTable[newIndex] = (byte)(depth + 1);
                                    done++;

                                    int flags = SymmetryReduction.CpSymmetries[newReducedCp];
                                    for (int symmetry = 1; symmetry < TwoPhaseConstants.NumSymmetriesDh4; symmetry++)
                                    {
                                        flags >>= 1;
                                        if ((flags & 0b1) == 1)
                                        {
                                            int rotatedUdEdgePermutation = SymmetryReduction.ConjugateUdEdgePermutationCoordinate[newUdEdgePermutation, symmetry];
                                            int rotatedIndex = NumUdEdgePermutationCoords * newReducedCp + rotatedUdEdgePermutation;
                                            if (pruningTable[rotatedIndex] == invalid)
                                            {
                                                pruningTable[rotatedIndex] = (byte)(depth + 1);
                                                done++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                depth++;
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.CornerUdPruningTableSizePhase2));
            }
            return pruningTable;
        }

        /// <summary>
        /// Create the corner permutation and equator permutation pruning table
        /// for phase 2.
        /// </summary>
        /// <returns>
        /// The corner permutation and equator permutation pruning table for
        /// phase 2.
        /// </returns>
        public static byte[] CreatePhase2CornerEquatorTable()
        {
            byte invalid = 255;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.CornerEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCpMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();

            pruningTable[0] = 0;
            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + TwoPhaseConstants.CornerEquatorPruningTableSizePhase2 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.CornerEquatorPruningTableSizePhase2));

            while (done < TwoPhaseConstants.CornerEquatorPruningTableSizePhase2)
            {
                for (int cp = 0; cp < NumCpCoords; cp++)
                {
                    for (int equatorPermutation = 0; equatorPermutation < TwoPhaseConstants.NumEquatorPermutations; equatorPermutation++)
                    {
                        int index = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                        if (pruningTable[index] == depth)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newCp = TableController.CpMoveTable[cp, move];
                                int newEquatorPerm = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                                int newIndex = TwoPhaseConstants.NumEquatorPermutations * newCp + newEquatorPerm;

                                if (pruningTable[newIndex] == invalid)
                                {
                                    pruningTable[newIndex] = (byte)(depth + 1);
                                    done++;
                                }
                            }
                        }
                    }
                }
                depth++;
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)TwoPhaseConstants.CornerEquatorPruningTableSizePhase2));
            }

            return pruningTable;
        }
    }
}
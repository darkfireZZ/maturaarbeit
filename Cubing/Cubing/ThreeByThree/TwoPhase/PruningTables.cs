using System;
using System.Linq;
using static Cubing.ThreeByThree.Constants;
using static Cubing.ThreeByThree.TwoPhase.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the pruning tables for the two-phase solver.
    /// </summary>
    public static class PruningTables
    {
        /// <summary>
        /// Get the index into the phase 1 pruning table given corner
        /// orientation, edge orientation and equator distribution coordinates.
        /// </summary>
        /// <remarks>
        /// There are no parameter checks in order to speed up the computation.
        /// </remarks>
        /// <param name="cornerOrientation">The corner orientation coordinate.</param>
        /// <param name="edgeOrientation">The edge orientation coodinate.</param>
        /// <param name="equatorDistribution">
        /// The equator distribution coordinate.
        /// </param>
        /// <returns>
        /// The index into the phase 1 pruning table for the given coordinates.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// May get thrown if any of the parameters is out of range.
        /// </exception>
        public static int GetPhase1PruningIndex(int cornerOrientation, int edgeOrientation, int equatorDistribution)
        {
            int eoEquator = equatorDistribution * NumEdgeOrientations + edgeOrientation;
            int reducedEoEquator = SymmetryReduction.ReduceEoEquatorCoordinate[eoEquator];
            int reductionSymmetry = SymmetryReduction.EoEquatorReductionSymmetry[eoEquator];
            int rotatedCornerOrientation = SymmetryReduction.ConjugateCornerOrientationCoordinate[cornerOrientation, reductionSymmetry];
            int pruningCoordinate = reducedEoEquator * NumCornerOrientations + rotatedCornerOrientation;

            return pruningCoordinate;
        }

        /// <summary>
        /// Get the index into the phase 2 corner permutation and U- and D-edge
        /// permutation pruning table given U- and D-edge order and corner
        /// permutation.
        /// </summary>
        /// <remarks>
        /// There are no parameter checks in order to speed up the computation.
        /// </remarks>
        /// <param name="udEdgeOrder">
        /// The U- and D-edge order coordinate.
        /// </param>
        /// <param name="cornerPermutation">The corner permutation coordinate.</param>
        /// <returns>
        /// The index into the phase 2 corner permutation and U- and D-edge
        /// order pruning table for the given parameters.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// May get thrown if any of the parameters is out of range.
        /// </exception>
        public static int GetPhase2CornerUdPruningIndex(int udEdgeOrder, int cornerPermutation)
        {
            int reducedCornerPermutation = SymmetryReduction.ReduceCornerPermutationCoordinate[cornerPermutation];
            int reductionSymmetry = SymmetryReduction.CornerPermutationReductionSymmetry[cornerPermutation];
            int rotatedUdEdgeOrder = SymmetryReduction.ConjugateUdEdgeOrderCoordinate[udEdgeOrder, reductionSymmetry];
            int pruningCoordinate = NumUdEdgeOrders * reducedCornerPermutation + rotatedUdEdgeOrder;

            return pruningCoordinate;
        }

        /// <summary>
        /// Create a pruning table using corner orientation, equator
        /// distribution and edge orientation. Every table entry contains the
        /// exact number of moves required to solve the position represented by
        /// said coordinates.
        /// </summary>
        /// <returns>
        /// Create a pruning table using corner orientation, equator
        /// distribution and edge orientation.
        /// </returns>
        public static byte[] CreatePhase1Table()
        {
            byte invalid = 0xFF;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, PruningTableConstants.PruningTableSizePhase1)
                .ToArray();

            TableController.InitializeCornerOrientationMoveTable();
            TableController.InitializeEdgeOrientationMoveTable();
            TableController.InitializeEquatorDistributionMoveTable();

            pruningTable[0] = 0; //solved
            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + PruningTableConstants.PruningTableSizePhase1 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.PruningTableSizePhase1));

            while (done < PruningTableConstants.PruningTableSizePhase1)
            {
                for (int reducedEoEquator = 0; reducedEoEquator < SymmetryReduction.NumEoEquatorSymmetryClasses; reducedEoEquator++)
                    for (int cornerOrientation = 0; cornerOrientation < NumCornerOrientations; cornerOrientation++)
                    {
                        int index = NumCornerOrientations * reducedEoEquator + cornerOrientation;
                        if (pruningTable[index] == depth)
                        {
                            int expandedEoEquator = SymmetryReduction.ExpandEoEquatorCoordinate[reducedEoEquator];
                            int edgeOrientation = expandedEoEquator % NumEdgeOrientations;
                            int equatorDistribution = expandedEoEquator / NumEdgeOrientations;
                            for (int move = 0; move < NumMoves; move++)
                            {
                                //apply move
                                int newCornerOrientation = TableController.CornerOrientationMoveTable[cornerOrientation, move];
                                int newEdgeOrientation = TableController.EdgeOrientationMoveTable[edgeOrientation, move];
                                int newEquatorDistribution = TableController.EquatorDistributionMoveTable[equatorDistribution, move];
                                
                                //same as calling GetPhase1PruningIndex,
                                //but the reduced edge orientation and equator
                                //distribution coordinate is required.
                                int newExpandedEoEquator = newEquatorDistribution * NumEdgeOrientations + newEdgeOrientation;
                                int newReducedEoEquator = SymmetryReduction.ReduceEoEquatorCoordinate[newExpandedEoEquator];
                                int reductionSymmetry = SymmetryReduction.EoEquatorReductionSymmetry[newExpandedEoEquator];
                                newCornerOrientation = SymmetryReduction.ConjugateCornerOrientationCoordinate[newCornerOrientation, reductionSymmetry];
                                int newIndex = newReducedEoEquator * NumCornerOrientations + newCornerOrientation;

                                //store depth
                                if (pruningTable[newIndex] == invalid)
                                {
                                    pruningTable[newIndex] = (byte)(depth + 1);
                                    done++;

                                    int flags = SymmetryReduction.EoEquatorSymmetries[newReducedEoEquator];
                                    for (int symmetry = 1; symmetry < Symmetries.NumSymmetriesDh4; symmetry++)
                                    {
                                        flags >>= 1;
                                        if ((flags & 0b1) == 1)
                                        {
                                            int rotatedCornerOrientation = SymmetryReduction.ConjugateCornerOrientationCoordinate[newCornerOrientation, symmetry];
                                            int rotatedIndex = NumCornerOrientations * newReducedEoEquator + rotatedCornerOrientation;
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
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.PruningTableSizePhase1));
            }
            return pruningTable;
        }

        /// <summary>
        /// Create a pruning table using corner permutation and U- and D-edge
        /// order. Every table entry contains the exact number of moves
        /// required to solve the position represented by said coordinates.
        /// </summary>
        /// <returns>
        /// Create a pruning table using corner permutation and U- and D-edge
        /// order.
        /// </returns>
        public static byte[] CreatePhase2CornerUdTable()
        {
            byte invalid = 0xFF;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, PruningTableConstants.CornerUdPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCornerPermutationMoveTable();
            TableController.InitializeUdEdgeOrderMoveTable();

            pruningTable[0] = 0; //solved
            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + PruningTableConstants.CornerUdPruningTableSizePhase2 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.CornerUdPruningTableSizePhase2));

            while (done < PruningTableConstants.CornerUdPruningTableSizePhase2)
            {
                for (int reducedCornerPermutation = 0; reducedCornerPermutation < SymmetryReduction.NumCornerPermutationSymmetryClasses; reducedCornerPermutation++)
                    for (int udEdgeOrder = 0; udEdgeOrder < NumUdEdgeOrders; udEdgeOrder++)
                    {
                        int index = NumUdEdgeOrders * reducedCornerPermutation + udEdgeOrder;
                        if (pruningTable[index] == depth)
                        {
                            int expandedCornerPermutation = SymmetryReduction.ExpandCornerPermutationCoordinate[reducedCornerPermutation];
                            for (int phase2Move = 0; phase2Move < TwoPhaseConstants.NumMovesPhase2; phase2Move++)
                            {
                                //apply move
                                int newUdEdgeOrder = TableController.UdEdgeOrderMoveTable[udEdgeOrder, phase2Move];
                                int newCornerPermutation = TableController.CornerPermutationMoveTable[expandedCornerPermutation, MoveTables.Phase2IndexToPhase1Index[phase2Move]];

                                //same as calling GetPhase2CornerUdPruningIndex,
                                //but the reduced edge orientation and equator
                                //distribution coordinate is required.
                                int newReducedCornerPermutation = SymmetryReduction.ReduceCornerPermutationCoordinate[newCornerPermutation];
                                int reductionSymmetry = SymmetryReduction.CornerPermutationReductionSymmetry[newCornerPermutation];
                                newUdEdgeOrder = SymmetryReduction.ConjugateUdEdgeOrderCoordinate[newUdEdgeOrder, reductionSymmetry];
                                int newIndex = NumUdEdgeOrders * newReducedCornerPermutation + newUdEdgeOrder;

                                //store depth
                                if (pruningTable[newIndex] == invalid)
                                {
                                    pruningTable[newIndex] = (byte)(depth + 1);
                                    done++;

                                    int flags = SymmetryReduction.CornerPermutationSymmetries[newReducedCornerPermutation];
                                    for (int symmetry = 1; symmetry < Symmetries.NumSymmetriesDh4; symmetry++)
                                    {
                                        flags >>= 1;
                                        if ((flags & 0b1) == 1)
                                        {
                                            int rotatedUdEdgePermutation = SymmetryReduction.ConjugateUdEdgeOrderCoordinate[newUdEdgeOrder, symmetry];
                                            int rotatedIndex = NumUdEdgeOrders * newReducedCornerPermutation + rotatedUdEdgePermutation;
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
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.CornerUdPruningTableSizePhase2));
            }
            return pruningTable;
        }

        /// <summary>
        /// Create a pruning table using corner permutation and equator order.
        /// Every table entry contains the exact number of moves required to
        /// solve the position represented by said coordinates.
        /// </summary>
        /// <returns>
        /// Create a pruning table using corner permutation equator order.
        /// </returns>
        public static byte[] CreatePhase2CornerEquatorTable()
        {
            byte invalid = 255;

            byte[] pruningTable = Enumerable
                .Repeat(invalid, PruningTableConstants.CornerEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCornerPermutationMoveTable();
            TableController.InitializeEquatorOrderMoveTable();

            pruningTable[0] = 0; //solved
            int done = 1;
            int depth = 0;

            string outputFormat = "depth: {0}, done: {1}/" + PruningTableConstants.CornerEquatorPruningTableSizePhase2 + " ({2:P})";
            Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.CornerEquatorPruningTableSizePhase2));

            while (done < PruningTableConstants.CornerEquatorPruningTableSizePhase2)
            {
                for (int cornerPermutation = 0; cornerPermutation < NumCornerPermutations; cornerPermutation++)
                {
                    for (int equatorPermutation = 0; equatorPermutation < NumEquatorOrders; equatorPermutation++)
                    {
                        int index = NumEquatorOrders * cornerPermutation + equatorPermutation;
                        if (pruningTable[index] == depth)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newCornerPermutation = TableController.CornerPermutationMoveTable[cornerPermutation, move];
                                int newEquatorOrder = TableController.EquatorOrderMoveTable[equatorPermutation, move];
                                int newIndex = NumEquatorOrders * newCornerPermutation + newEquatorOrder;

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
                Console.WriteLine(string.Format(outputFormat, depth, done, done / (double)PruningTableConstants.CornerEquatorPruningTableSizePhase2));
            }

            return pruningTable;
        }
    }
}
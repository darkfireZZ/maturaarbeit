using System.Linq;
using static Cubing.ThreeByThree.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the pruning tables for the weighted two-phase solver.
    /// </summary>
    public static class WeightedPruningTables
    {
        /// <summary>
        /// Create a weighted pruning table using the corner permutation and
        /// equator edge permutation.
        /// </summary>
        /// <param name="weights">The weights of the moves.</param>
        /// <returns>
        /// A weighted pruning table using the corner permutation and equator
        /// edge permutation.
        /// </returns>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static double[] CreateWeightedPhase2CornerEquatorTable(double[] weights)
        {
            MoveWeightsUtils.ValidateWeights(weights);

            double invalid = double.NaN;

            double[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.CornerEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCpMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();

            pruningTable[0] = 0d;
            int numChanged = -1;

            while (numChanged != 0)
            {
                numChanged = 0;
                for (int cp = 0; cp < NumCpCoords; cp++)
                {
                    for (int equatorPermutation = 0; equatorPermutation < TwoPhaseConstants.NumEquatorPermutations; equatorPermutation++)
                    {
                        int index = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                        if (pruningTable[index] != invalid)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newCp = TableController.CpMoveTable[cp, move];
                                int newEquatorPermutation = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                                int newIndex = TwoPhaseConstants.NumEquatorPermutations * newCp + newEquatorPermutation;

                                double newPruningValue = pruningTable[index] + weights[move];

                                if (pruningTable[newIndex] == invalid || pruningTable[newIndex] > newPruningValue)
                                {
                                    pruningTable[newIndex] = newPruningValue;
                                    numChanged++;
                                }
                            }
                        }
                    }
                }
            }

            return pruningTable;
        }

        /// <summary>
        /// Create a weighted pruning table using the U- and D-edge permutation
        /// and equator edge permutation.
        /// </summary>
        /// <param name="weights">The weights of the moves.</param>
        /// <returns>
        /// A weighted pruning table using the U- and D-edge permutation and
        /// equator edge permutation.
        /// </returns>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static double[] CreateWeightedPhase2UdEquatorTable(double[] weights)
        {
            MoveWeightsUtils.ValidateWeights(weights);

            double invalid = double.NaN;

            double[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.UdEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeUdEdgePermutationMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();

            pruningTable[0] = 0d;
            int numChanged = -1;

            while (numChanged != 0)
            {
                numChanged = 0;
                for (int udEdgePermutation = 0; udEdgePermutation < NumUdEdgePermutationCoords; udEdgePermutation++)
                {
                    for (int equatorPermutation = 0; equatorPermutation < TwoPhaseConstants.NumEquatorPermutations; equatorPermutation++)
                    {
                        int index = TwoPhaseConstants.NumEquatorPermutations * udEdgePermutation + equatorPermutation;
                        if (pruningTable[index] != invalid)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newUdEdgePermutation = TableController.UdEdgePermutationMoveTable[udEdgePermutation, MoveTables.Phase1IndexToPhase2Index[move]];
                                int newEquatorPermutation = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                                int newIndex = TwoPhaseConstants.NumEquatorPermutations * newUdEdgePermutation + newEquatorPermutation;

                                double newPruningValue = pruningTable[index] + weights[move];

                                if (pruningTable[newIndex] == invalid || pruningTable[newIndex] > newPruningValue)
                                {
                                    pruningTable[newIndex] = newPruningValue;
                                    numChanged++;
                                }
                            }
                        }
                    }
                }
            }

            return pruningTable;
        }
    }
}
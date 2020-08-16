using System.Linq;
using static Cubing.ThreeByThree.TwoPhase.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains the pruning tables for the weighted two-phase solver.
    /// </summary>
    public static class WeightedPruningTables
    {
        /// <summary>
        /// Create a weighted pruning table using corner permutation and
        /// equator edge order. Every table entry contains the minimum cost
        /// required to solve the position represented by said coordinates.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        /// <returns>
        /// A weighted pruning table using the corner permutation and equator
        /// edge order.
        /// </returns>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static float[] CreateWeightedPhase2CornerEquatorTable(float[] weights)
        {
            MoveWeightsUtils.ValidateWeights(weights);

            float invalid = float.NaN;

            float[] pruningTable = Enumerable
                .Repeat(invalid, PruningTableConstants.CornerEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCornerPermutationMoveTable();
            TableController.InitializeEquatorOrderMoveTable();

            pruningTable[0] = 0f;
            int numChanged = -1;

            while (numChanged != 0)
            {
                numChanged = 0;
                for (int cornerPermutation = 0; cornerPermutation < NumCornerPermutations; cornerPermutation++)
                {
                    for (int equatorOrder = 0; equatorOrder < NumEquatorOrders; equatorOrder++)
                    {
                        int index = NumEquatorOrders * cornerPermutation + equatorOrder;
                        if (pruningTable[index] != invalid)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newCornerPermutation = TableController.CornerPermutationMoveTable[cornerPermutation, move];
                                int newEquatorOrder = TableController.EquatorOrderMoveTable[equatorOrder, move];
                                int newIndex = NumEquatorOrders * newCornerPermutation + newEquatorOrder;

                                float newPruningValue = pruningTable[index] + weights[move];

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

        //Not used
        /// <summary>
        /// Create a weighted pruning table using U- and D-edge order and
        /// equator edge order. Every table entry contains the minimum cost
        /// required to solve the position represented by said coordinates.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        /// <returns>
        /// A weighted pruning table using U- and D-edge order and equator edge
        /// order.
        /// </returns>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static float[] CreateWeightedPhase2UdEquatorTable(float[] weights)
        {
            MoveWeightsUtils.ValidateWeights(weights);

            float invalid = float.NaN;

            float[] pruningTable = Enumerable
                .Repeat(invalid, PruningTableConstants.UdEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeUdEdgeOrderMoveTable();
            TableController.InitializeEquatorOrderMoveTable();

            pruningTable[0] = 0f;
            int numChanged = -1;

            while (numChanged != 0)
            {
                numChanged = 0;
                for (int udEdgeOrder = 0; udEdgeOrder < NumUdEdgeOrders; udEdgeOrder++)
                {
                    for (int equatorOrder = 0; equatorOrder < NumEquatorOrders; equatorOrder++)
                    {
                        int index = NumEquatorOrders * udEdgeOrder + equatorOrder;
                        if (pruningTable[index] != invalid)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newUdEdgeOrder = TableController.UdEdgeOrderMoveTable[udEdgeOrder, MoveTables.Phase1IndexToPhase2Index[move]];
                                int newEquatorOrder = TableController.EquatorOrderMoveTable[equatorOrder, move];
                                int newIndex = NumEquatorOrders * newUdEdgeOrder + newEquatorOrder;

                                float newPruningValue = pruningTable[index] + weights[move];

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
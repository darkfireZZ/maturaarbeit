using static Cubing.ThreeByThree.TwoPhase.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Constants related to pruning tables.
    /// </summary>
    public static class PruningTableConstants
    {
        /// <summary>
        /// The number of entries in the pruning table for phase 1.
        /// </summary>
        public const int PruningTableSizePhase1 = SymmetryReduction.NumEoEquatorSymmetryClasses * NumCornerOrientations;

        /// <summary>
        /// The number of entries in the corner permutation and equator
        /// permutation pruning table for phase 2.
        /// </summary>
        public const int CornerEquatorPruningTableSizePhase2 = NumCornerPermutations * NumEquatorOrders;

        /// <summary>
        /// The number of entries in the corner permutation and U- and D-Edge
        /// order pruning table for phase 2.
        /// </summary>
        public const int CornerUdPruningTableSizePhase2 = SymmetryReduction.NumCornerPermutationSymmetryClasses * NumUdEdgeOrders;

        /// <summary>
        /// The number of entries in the U- and D-edge order and equator edge
        /// permutation pruning table for phase 2.
        /// </summary>
        public const int UdEquatorPruningTableSizePhase2 = NumUdEdgeOrders * NumEquatorOrders;

    }
}
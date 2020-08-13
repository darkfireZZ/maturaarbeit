namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains constants for the two-phase algorithm.
    /// </summary>
    public class TwoPhaseConstants
    {
        /// <summary>
        /// The maximum depth of phase 1.
        /// </summary>
        public const int MaxDepthPhase1 = 12;
        /// <summary>
        /// The maximum depth of phase 2.
        /// </summary>
        public const int MaxDepthPhase2 = 18;

        /// <summary>
        /// The number of different symmetry reduced EoEquator coordinates.
        /// </summary>
        public const int NumReducedEoEquatorCoordinates = 64430;

        /// <summary>
        /// The number of different symmetry reduced corner permutation
        /// coordinates.
        /// </summary>
        public const int NumReducedCornerPermutationCoordinates = 2768;

        /// <summary>
        /// The number of symmetries in the subgroup Dh4.
        /// </summary>
        public const int NumSymmetriesDh4 = Symmetries.NumSymmetries / 3; //16

        /// <summary>
        /// The number of entries in the pruning table for phase 1.
        /// </summary>
        public const int PruningTableSizePhase1 = NumReducedEoEquatorCoordinates * Coordinates.NumCoCoords;

        /// <summary>
        /// The number of entries in the corner permutation and equator
        /// permutation pruning table for phase 2.
        /// </summary>
        public const int CornerEquatorPruningTableSizePhase2 = Coordinates.NumCpCoords * Coordinates.NumEquatorPermutationCoords;

        /// <summary>
        /// The number of entries in the corner permutation and U- and D-Edge
        /// permutation pruning table for phase 2.
        /// </summary>
        public const int CornerUdPruningTableSizePhase2 = NumReducedCornerPermutationCoordinates * Coordinates.NumUdEdgePermutationCoords;

        /// <summary>
        /// The number of entries in the U- and D-edge permutation and equator
        /// edge permutation pruning table for phase 2.
        /// </summary>
        public const int UdEquatorPruningTableSizePhase2 = Coordinates.NumUdEdgePermutationCoords * Coordinates.NumEquatorPermutationCoords;

        /// <summary>
        /// The number of different possible permutations of the equator edges.
        /// </summary>
        public const int NumEquatorPermutations = 24; // 4!

        /// <summary>
        /// The number of moves used in phase 2.
        /// </summary>
        /// <remarks>
        /// See also <seealso cref="Phase2Moves"/>.
        /// </remarks>
        public const int NumMovesPhase2 = 10;

        /// <summary>
        /// The moves used in phase 2 in their integer representation.
        /// </summary>
        public static readonly Move[] Phase2Moves = new Move[] {
            Move.R2, Move.U1, Move.U2, Move.U3, Move.F2,
            Move.L2, Move.D1, Move.D2, Move.D3, Move.B2
        };
    }
}
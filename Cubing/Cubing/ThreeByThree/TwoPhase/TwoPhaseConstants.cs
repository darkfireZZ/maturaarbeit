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
        //TODO test value
        /// <summary>
        /// The maximum depth of phase 2.
        /// </summary>
        public const int MaxDepthPhase2 = 18;

        /// <summary>
        /// The number of symmetry reduced EoEquator coordinates.
        /// </summary>
        public const int NumReducedEoEquatorCoordinates = 64430;

        /// <summary>
        /// The number of symmetries used in phase 1 of the two-phase-algorithm.
        /// </summary>
        public const int NumSymmetriesPhase1 = Symmetries.NumSymmetries / 3; //16

        /// <summary>
        /// The number of entries in the pruning table for phase 1.
        /// </summary>
        public const int PruningTableSizePhase1 = NumReducedEoEquatorCoordinates * Coordinates.NumCoCoords;

        /// <summary>
        /// The number of entries in the pruning table for phase 2.
        /// </summary>
        public const int PruningTableSizePhase2 = Coordinates.NumCpCoords * Coordinates.NumEquatorPermutationCoords;

        /// <summary>
        /// The number of symmetries used in phase2 of the two-phase-algorithm.
        /// </summary>
        public const int NumSymmetriesPhase2 = Symmetries.NumSymmetries; //48

        /// <summary>
        /// The number of different possible permutations of the equator edges.
        /// </summary>
        public const int NumEquatorPermutations = 24; // 4!

        //TODO make readonlyarray
        /// <summary>
        /// The moves used in phase 2 in their integer representation.
        /// </summary>
        public static readonly Move[] Phase2Moves = new Move[] {
            Move.U1, Move.U2, Move.U3,
            Move.D1, Move.D2, Move.D3,
            Move.R2, Move.F2, Move.L2, Move.B2 };
    }
}
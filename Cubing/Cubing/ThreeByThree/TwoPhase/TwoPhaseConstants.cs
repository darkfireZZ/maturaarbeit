namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Constants related to the two-phase algorithm.
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
        /// The number of moves used in phase 2.
        /// </summary>
        /// <remarks>
        /// See also <seealso cref="Phase2Moves"/>.
        /// </remarks>
        public const int NumMovesPhase2 = 10;

        /// <summary>
        /// The moves used in phase 2. Do not change.
        /// </summary>
        public static readonly Move[] Phase2Moves = new Move[] {
            Move.R2, Move.U1, Move.U2, Move.U3, Move.F2,
            Move.L2, Move.D1, Move.D2, Move.D3, Move.B2
        };
    }
}
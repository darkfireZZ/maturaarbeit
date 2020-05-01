using System;
using System.Collections.Generic;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Provides extension methods for <see cref="Move"/>.
    /// </summary>
    public static class Moves
    {
        private static readonly string[] _moveToStringArray = new[] {
            "R", "R2", "R'",
            "U", "U2", "U'",
            "F", "F2", "F'",
            "L", "L2", "L'",
            "D", "D2", "D'",
            "B", "B2", "B'"
        };

        private static readonly Dictionary<string, Move> _stringToMoveDict = new Dictionary<string, Move>() {
            ["R"] = Move.R1, ["R2"] = Move.R2, ["R'"] = Move.R3,
            ["U"] = Move.U1, ["U2"] = Move.U2, ["U'"] = Move.U3,
            ["F"] = Move.F1, ["F2"] = Move.F2, ["F'"] = Move.F3,
            ["L"] = Move.L1, ["L2"] = Move.L2, ["L'"] = Move.L3,
            ["D"] = Move.D1, ["D2"] = Move.D2, ["D'"] = Move.D3,
            ["B"] = Move.B1, ["B2"] = Move.B2, ["B'"] = Move.B3,
        };

        /// <summary>
        /// Get the string representation of a move.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="MoveFromString(string)"/> to get a
        /// <see cref="Move"/> from a string.
        /// </remarks>
        /// <param name="move">
        /// The move to get the string representation of.
        /// </param>
        /// <returns>The string representation of a move.</returns>
        public static string MoveToString(Move move)
            => _moveToStringArray[(int)move];

        /// <summary>
        /// Maps the string representations of a move to a
        /// <see cref="Move"/> enum.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="MoveToString(Move)"/> to get a string from
        /// a <see cref="Move"/>.
        /// </remarks>
        /// <param name="move">
        /// The string to get the enum representation of.
        /// </param>
        /// <returns>The enum representation of the move.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="move"/> does not correspond to a
        /// move.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="move"/> is null.
        /// </exception>
        public static Move MoveFromString(string move)
        {
            if (move is null)
                throw new ArgumentNullException(nameof(move) + " is null.");

            try { return _stringToMoveDict[move]; }
            catch (KeyNotFoundException exception)
            {
                throw new ArgumentException(nameof(move) + " is invalid: " + move, exception);
            }
        }

        /// <summary>
        /// Get the move on the specified face with the specified number
        /// of quarter turns in clockwise direction.
        /// </summary>
        /// <param name="face">
        /// The face of the move.
        /// </param>
        /// <param name="quarterTurns">
        /// The number of quarter turns in clockwise direction of the move.
        /// </param>
        /// <returns>
        /// The move on the specified face with the specified number
        /// of quarter turns in clockwise direction.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="quarterTurns"/> has a value other
        /// than 1, 2 or 3.
        /// </exception>
        public static Move MoveFromFaceAndQuarterTurns(Face face, int quarterTurns)
        {
            if (quarterTurns < 1 || quarterTurns > 3)
                throw new ArgumentOutOfRangeException(nameof(quarterTurns) + " must either be 1, 2 or 3, but is " + quarterTurns + ".");
            return (Move)((int)face * 3 + quarterTurns - 1);
        }

        /// <summary>
        /// Get the axis of a move.
        /// </summary>
        /// <param name="move">The move to get the axis of.</param>
        /// <returns>The axis of the specified move.</returns>
        public static Axis Axis(this Move move)
            => (Axis)((int)move / 3 % 3);

        /// <summary>
        /// Get the face of a move.
        /// </summary>
        /// <param name="move">The move to get the face of.</param>
        /// <returns>The face of the specified move.</returns>
        public static Face Face(this Move move)
            => (Face)((int)move / 3);

        /// <summary>
        /// Get the number of quarter turns in clockwise direction of the
        /// move.
        /// </summary>
        /// <param name="move">
        /// The move to get the number of quarter turns of.
        /// </param>
        /// <returns>The number of quarter turns of the move.</returns>
        public static int QuarterTurns(this Move move)
            => (int)move % 3 + 1;

        /// <summary>
        /// Get the inverse of a move.
        /// </summary>
        /// <param name="move">The move to get the inverse of.</param>
        /// <returns>The inverse of a move.</returns>
        public static Move Inverse(this Move move)
            => MoveFromFaceAndQuarterTurns(move.Face(), 4 - move.QuarterTurns());

        /// <summary>
        /// Get a move the is a specific rotation away.
        /// </summary>
        /// <param name="move">The move to rotate.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>
        /// The <paramref name="move"/> rotated by
        /// <paramref name="rotation"/>.
        /// </returns>
        public static Move Rotate(this Move move, Rotation rotation)
            => MoveFromFaceAndQuarterTurns(move.Face().Rotate(rotation), move.QuarterTurns());
    }

    /// <summary>
    /// All different 3x3x3 moves in half turn metric (HTM).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>move / 3</c> is the face of a move. Where 0 is the R face, 1 is
    /// the U face, 2 is the F face, 3 is the L face, 4 is the D face and
    /// 5 is the B face.
    /// </para>
    /// <para>
    /// <c>move % 3 + 1</c> is the number of quarter turns in clockwise
    /// direction of a move.
    /// </para>
    /// </remarks>
    public enum Move : int
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        R1 = 0, R2 = 1, R3 = 2,
        U1 = 3, U2 = 4, U3 = 5,
        F1 = 6, F2 = 7, F3 = 8,
        L1 = 9, L2 = 10, L3 = 11,
        D1 = 12, D2 = 13, D3 = 14,
        B1 = 15, B2 = 16, B3 = 17
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
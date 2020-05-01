using System;
using System.Collections.Generic;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Provides extension methods for <see cref="Rotation"/>.
    /// </summary>
    public static class Rotations
    {
        private static readonly string[] _rotationToStringArray = {
            "x", "x2", "x'",
            "y", "y2", "y'",
            "z", "z2", "z'"
        };
        private static readonly Dictionary<string, Rotation> _stringToRotationDict = new Dictionary<string, Rotation>()
        {
            ["x"] = Rotation.x1, ["x2"] = Rotation.x2, ["x'"] = Rotation.x3,
            ["y"] = Rotation.y1, ["y2"] = Rotation.y2, ["y'"] = Rotation.y3,
            ["z"] = Rotation.z1, ["z2"] = Rotation.z2, ["z'"] = Rotation.z3
        };

        /// <summary>
        /// Get the string representation of a rotation.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="RotationFromString(string)"/> to get a
        /// <see cref="Rotation"/> from a string.
        /// </remarks>
        /// <param name="rotation">
        /// The rotation to get the string representation of.
        /// </param>
        /// <returns>The string representation of a rotation.</returns>
        public static string RotationToString(Rotation rotation)
            => _rotationToStringArray[(int)rotation];

        /// <summary>
        /// Maps the string representations of a rotation to a
        /// <see cref="Rotation"/> enum.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="RotationToString(Rotation)"/> to get a
        /// string from a <see cref="Rotation"/>.
        /// </remarks>
        /// <param name="rotation">
        /// The string to get the enum representation of.
        /// </param>
        /// <returns>The enum representation of the rotation.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="rotation"/> does not correspond to a
        /// rotation.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="rotation"/> is null.
        /// </exception>
        public static Rotation RotationFromString(string rotation)
        {
            if (rotation is null)
                throw new ArgumentNullException(nameof(rotation) + " is null.");
            try { return _stringToRotationDict[rotation]; }
            catch (KeyNotFoundException exception)
            {
                throw new ArgumentException(nameof(rotation) + " is invalid: " + rotation, exception);
            }
        }

        /// <summary>
        /// Get the axis of a rotation.
        /// </summary>
        /// <param name="rotation">
        /// The rotation to get the axis of.
        /// </param>
        /// <returns>The axis of the specified rotation.</returns>
        public static Axis Axis(this Rotation rotation)
            => (Axis)((int)rotation / 3);

        /// <summary>
        /// Get the number of quarter turns in clockwise direction of the
        /// rotation.
        /// </summary>
        /// <param name="rotation">
        /// The rotation to get the number of quarter turns of.
        /// </param>
        /// <returns>The number of quarter turns of the rotation.</returns>
        public static int QuarterTurns(this Rotation rotation)
            => (int)rotation % 3 + 1;

        /// <summary>
        /// Get the rotation on the specified axis with the specified
        /// number of quarter turns.
        /// </summary>
        /// <param name="axis">
        /// The axis of the rotation.
        /// </param>
        /// <param name="quarterTurns">
        /// The number of quarter turns of the rotation.
        /// </param>
        /// <returns>
        /// The rotation on the specified face with the specified number
        /// of quarter turns.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="quarterTurns"/> has a value other
        /// than 1, 2 or 3.
        /// </exception>
        public static Rotation RotationFromAxisAndQuarterTurns(Axis axis, int quarterTurns)
        {
            if (quarterTurns < 1 || quarterTurns > 3)
                throw new ArgumentOutOfRangeException(nameof(quarterTurns) + " must either be 1, 2 or 3, but is " + quarterTurns + ".");
            return (Rotation)((int)axis * 3 + quarterTurns - 1);
        }

        /// <summary>
        /// Get the inverse of a rotation.
        /// </summary>
        /// <param name="rotation">The rotation to get the inverse of.</param>
        /// <returns>The inverse of a rotation.</returns>
        public static Rotation Inverse(this Rotation rotation)
            => RotationFromAxisAndQuarterTurns(rotation.Axis(), 4 - rotation.QuarterTurns());
    }

    /// <summary>
    /// All different rotations in half turn metric (HTM).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>rotation / 3</c> is the axis of a rotation. Where 0 means it is
    /// around the x axis, 1 means it is around the y axis and 2 means it
    /// is around the z axis.
    /// </para>
    /// <para>
    /// <c>rotation % 3 + 1</c> is the number of quarter turns of a
    /// rotation.
    /// </para>
    /// </remarks>
    public enum Rotation : int
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        x1 = 0, x2 = 1, x3 = 2,
        y1 = 3, y2 = 4, y3 = 5,
        z1 = 6, z2 = 7, z3 = 8
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
using System;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Provides extension methods for <see cref="Axis"/>.
    /// </summary>
    public static class Axes
    {
        /// <summary>
        /// Get the string representation of an axis.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="AxisFromString(string)"/> to get an
        /// <see cref="Axis"/> from a string.
        /// </remarks>
        /// <param name="axis">
        /// The axis to get the string representation of.
        /// </param>
        /// <returns>The string representation of an axis.</returns>
        public static string AxisToString(Axis axis)
            => axis.ToString();

        /// <summary>
        /// Maps the string representations of an axis to an <see cref="Axis"/>
        /// enum.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="AxisToString(Axis)"/> to get a string from an
        /// <see cref="Axis"/>.
        /// </remarks>
        /// <param name="axis">
        /// The string to get the enum representation of.
        /// </param>
        /// <returns>The enum representation of the axis.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="axis"/> does not correspond to an axis.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="axis"/> is null.
        /// </exception>
        public static Axis AxisFromString(string axis)
        {
            switch (axis)
            {
                case "x": return Axis.x;
                case "y": return Axis.y;
                case "z": return Axis.z;
                case null:
                    throw new ArgumentNullException(nameof(axis) + " is null.");
                default:
                    throw new ArgumentException(nameof(axis) + " is invalid: " + axis);
            }
        }
    }

    /// <summary>
    /// All different axes of a 3x3x3 cube. x = 0, y = 1, z = 2
    /// </summary>
    #pragma warning disable CS1591
    public enum Axis : int { x = 0, y = 1, z = 2 }
    #pragma warning restore CS1591
}
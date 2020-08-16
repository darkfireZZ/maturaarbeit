using System;
using System.Collections.Generic;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree
{
    /// <summary>
    /// Provides extension methods for <see cref="Face"/>.
    /// </summary>
    public static class Faces
    {
        private static readonly Dictionary<string, Face> _stringToFaceDict =
            new Dictionary<string, Face>() {
            ["R"] = Face.R,
            ["U"] = Face.U,
            ["F"] = Face.F,
            ["L"] = Face.L,
            ["D"] = Face.D,
            ["B"] = Face.B };

        private static readonly Face[] _oppositeFaceArray =
            new Face[] { Face.L, Face.D, Face.B, Face.R, Face.U, Face.F };

        private static readonly Face[,] _rotateArray = null;

        static Faces()
        {
            //initialize _rotateArray
            Face[,] rotateByOne = new[,] {
                { Face.R, Face.F, Face.D},
                { Face.B, Face.U, Face.R},
                { Face.U, Face.L, Face.F},
                { Face.L, Face.B, Face.U},
                { Face.F, Face.D, Face.L},
                { Face.D, Face.R, Face.B}
            };
            _rotateArray = new Face[NumFaces, NumRotations];
            for (int face = 0; face < NumFaces; face++)
            {
                for (int axis = 0; axis < NumAxes; axis++)
                {
                    _rotateArray[face, axis * 3] = rotateByOne[face, axis];
                    _rotateArray[face, axis * 3 + 1] = rotateByOne[(int)_rotateArray[face, axis * 3], axis];
                    _rotateArray[face, axis * 3 + 2] = rotateByOne[(int)_rotateArray[face, axis * 3 + 1], axis];
                }
            }
        }

        /// <summary>
        /// Get the string representation of a face.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="FaceFromString(string)"/> to get a
        /// <see cref="Face"/> from a string.
        /// </remarks>
        /// <param name="face">
        /// The face to get the string representation of.
        /// </param>
        /// <returns>The string representation of a face.</returns>
        public static string FaceToString(Face face)
            => face.ToString();
        
        /// <summary>
        /// Maps the string representations of a face to a
        /// <see cref="Face"/> enum.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="FaceToString(Face)"/> to get a string from
        /// a <see cref="Face"/>.
        /// </remarks>
        /// <param name="face">
        /// The string to get the enum representation of.
        /// </param>
        /// <returns>The enum representation of the face.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="face"/> does not correspond to a
        /// face.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="face"/> is null.
        /// </exception>
        public static Face FaceFromString(string face)
        {
            if (face is null)
                throw new ArgumentNullException(nameof(face) + " is null.");

            try { return _stringToFaceDict[face]; }
            catch (KeyNotFoundException exception)
            {
                throw new ArgumentException(nameof(face) + " is invalid: " + face, exception);
            }
        }

        /// <summary>
        /// Get the axis of a face.
        /// </summary>
        /// <param name="face">The face to get the axis of.</param>
        /// <returns>The axis of the specified face.</returns>
        public static Axis Axis(this Face face)
            => (Axis)((int)face % 3);

        /// <summary>
        /// Get a face the is a specific rotation away.
        /// </summary>
        /// <param name="face">The face to rotate.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>
        /// The <paramref name="face"/> rotated by
        /// <paramref name="rotation"/>.
        /// </returns>
        public static Face Rotate(this Face face, Rotation rotation)
            => _rotateArray[(int)face, (int)rotation];

        /// <summary>
        /// Get the opposite face of a face.
        /// </summary>
        /// <param name="face">
        /// The face to get the opposite face of.
        /// </param>
        /// <returns>
        /// The opposite face of <paramref name="face"/>.
        /// </returns>
        public static Face OppositeFace(this Face face)
            => _oppositeFaceArray[(int)face];
    }

    /// <summary>
    /// All different faces of a 3x3x3 cube. R = 0, U = 1, F = 2, L = 3,
    /// D = 4, B = 4
    /// </summary>
    #pragma warning disable CS1591
    public enum Face : int { R = 0, U = 1, F = 2, L = 3, D = 4, B = 5 }
    #pragma warning restore CS1591
}
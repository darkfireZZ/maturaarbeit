using System;
using System.Collections.Generic;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Provides utility functions for validating and manipulating arrays of
    /// weights for the 18 HTM moves.
    /// </summary>
    public static class MoveWeightsUtils
    {
        /// <summary>
        /// To be considered valid, the weights array cannot be null, must have
        /// a length of exactly 18 and cannot contain any value that is either
        /// <see cref="double.NaN"/> of negative. If the weights array is
        /// invalid, a <see cref="InvalidWeightsException"/> is thrown.
        /// </summary>
        /// <param name="weights">The weights array to validate.</param>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if the weights array is invalid.
        /// </exception>
        public static void ValidateWeights(double[] weights)
        {
            if (weights is null)
                throw new InvalidWeightsException("Invalid weights", new ArgumentNullException(nameof(weights) + " is null."));
            if (weights.Length != NumMoves)
                throw new InvalidWeightsException("Invalid weights", new ArgumentException(nameof(weights) + " must have length " + NumMoves + ": " + weights.Length));
            for (int index = 0; index < NumMoves; index++)
            {
                if (double.IsNaN(weights[index]))
                    throw new InvalidWeightsException("Invalid weights", new ArgumentException(nameof(weights) + " cannot contain " + double.NaN + " values: index " + index));
                if (weights[index] < 0d)
                    throw new InvalidWeightsException("Invalid weights", new ArgumentException(nameof(weights) + " cannot contain negative values: index " + index + " = " + weights[index]));
            }
        }

        //IMPR doc
        /// <summary>
        /// Create a set of moves ordered by their weights.
        /// </summary>
        /// <param name="moves">A collection of moves to sort.</param>
        /// <param name="weights">The weights to sort by.</param>
        /// <returns>A set of moves ordered by their weights.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="moves"/> is null.
        /// </exception>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static IEnumerable<Move> OrderMoves(IEnumerable<Move> moves, double[] weights)
        {
            if (moves is null)
                throw new ArgumentNullException(nameof(moves) + " is null.");
            ValidateWeights(weights);

            return moves.Distinct().OrderBy(move => weights[(int)move]);
        }
    }

    /// <summary>
    /// An exception only thrown by
    /// <see cref="MoveWeightsUtils.ValidateWeights(double[])"/>. It indicates
    /// that the weights were invalid. The inner exception explains the reason.
    /// </summary>
    public class InvalidWeightsException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InvalidWeightsException"/> class.
        /// </summary>
        public InvalidWeightsException() : base() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InvalidWeightsException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        public InvalidWeightsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InvalidWeightsException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the
        /// <paramref name="inner"/> parameter is not a null reference, the
        /// current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public InvalidWeightsException(string message, Exception inner) : base(message, inner) { }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree
{
    //TODO test if array needs to be readonly
    //TODO implement GetHashCode
    /// <summary>
    /// An immutable class for manipulating and retrieving information about
    /// algs.
    /// </summary>
    public class Alg : IEnumerable<Move>, IEquatable<IEnumerable<Move>>
    {
        /// <summary>
        /// Compare if two algs are equal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Two algs are considered equal if they are not null and
        /// contain the same moves in the same order.
        /// </para>
        /// </remarks>
        /// <param name="alg1">The first alg to compare.</param>
        /// <param name="alg2">The second alg to compare.</param>
        /// <returns>Whether the two algs are equal.</returns>
        public static bool AreEqual(IEnumerable<Move> alg1, IEnumerable<Move> alg2)
        {
            if (alg1 is null || alg2 is null)
                return false;

            if (alg1.Count() != alg2.Count()) return false;
            foreach (bool b in alg1.Zip(alg2, (e1, e2) => e1 == e2))
                if (!b) return false;
            return true;
        }

        /// <summary>
        /// Compare if an alg is equal to another alg.
        /// </summary>
        /// <param name="alg1">The first alg to compare.</param>
        /// <param name="alg2">The second alg to compare.</param>
        /// <returns>Whether the two algs are equal.</returns>
        public static bool operator== (Alg alg1, IEnumerable<Move> alg2)
            => AreEqual(alg1, alg2);

        /// <summary>
        /// Compare if an alg is not equal to another alg.
        /// </summary>
        /// <param name="alg1">First alg to compare.</param>
        /// <param name="alg2">Second alg to compare.</param>
        /// <returns>Whether the two algs not are equal.</returns>
        public static bool operator!= (Alg alg1, IEnumerable<Move> alg2)
            => !AreEqual(alg1, alg2);

        /// <summary>
        /// Concatenates two algs.
        /// </summary>
        /// <param name="alg1">The first alg.</param>
        /// <param name="alg2">The second alg.</param>
        /// <returns>The two specified algs concatenated.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg1"/> or <paramref name="alg2"/> is
        /// null.
        /// </exception>
        public static Alg operator+ (Alg alg1, Alg alg2)
        {
            if (alg1 is null)
                throw new ArgumentNullException(nameof(alg1) + " is null.");
            if (alg2 is null)
                throw new ArgumentNullException(nameof(alg2) + " is null.");

            return new Alg(alg1.Concat(alg2).ToArray());
        }

        /// <summary>
        /// Repeat an alg.
        /// </summary>
        /// <param name="alg">The alg to repeat.</param>
        /// <param name="numTimes">The number of repetitions.</param>
        /// <returns>
        /// <paramref name="alg"/> repeated for the specified number of times.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="numTimes"/> is negative.
        /// </exception>
        public static Alg operator* (Alg alg, int numTimes)
        {
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null.");
            if (numTimes < 0)
                throw new ArgumentOutOfRangeException(nameof(numTimes) + " is smaller than 0: " + numTimes);

            Alg repeatedAlg = Alg.Empty;
            for (int repetition = 0; repetition < numTimes; repetition++)
                repeatedAlg += alg;
            return repeatedAlg;
        }

        /// <summary>
        /// Get the string representation of an alg.
        /// </summary>
        /// <param name="alg">
        /// The alg to get the string representation of.
        /// </param>
        /// <returns>
        /// The string representation of <paramref name="alg"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        public static string ToString(IEnumerable<Move> alg)
        {
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null.");

            var enumerator = alg.GetEnumerator();
            if (!enumerator.MoveNext()) return "";

            string returnValue = Moves.MoveToString(enumerator.Current);
            while (enumerator.MoveNext())
                returnValue += " " + Moves.MoveToString(enumerator.Current);
            return returnValue;
        }

        /// <summary>
        /// Get the inverse of an alg (reverse the sequence and change and
        /// change the direction of all moves).
        /// </summary>
        /// <param name="alg">The alg to get the inverse of.</param>
        /// <returns>
        /// The inverse of <paramref name="alg"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        public static Alg Inverse(IEnumerable<Move> alg)
        {
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null");

            Move[] moves = alg.ToArray();
            Array.Reverse(moves);
            for (int index = 0; index < moves.Length; index++)
                moves[index] = moves[index].Inverse();
            return new Alg(moves);
        }

        /// <summary>
        /// Rotate an alg.
        /// </summary>
        /// <param name="alg">The alg to rotate.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>
        /// <paramref name="alg"/> rotated by <paramref name="rotation"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        public static Alg Rotate(IEnumerable<Move> alg, Rotation rotation)
        {
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null.");

            Move[] moves = alg.ToArray();
            for (int index = 0; index < moves.Length; index++)
                moves[index] = moves[index].Rotate(rotation);
            return new Alg(moves);
        }

        /// <summary>
        /// Create an <see cref="Alg"/> from enumerable.
        /// </summary>
        /// <param name="alg">The alg to copy.</param>
        /// <returns>The copied alg.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        public static Alg FromEnumerable(IEnumerable<Move> alg)
            => FromEnumerable(alg, 0, alg.Count());

        /// <summary>
        /// Create an <see cref="Alg"/> from enumerable.
        /// </summary>
        /// <param name="alg">The alg to copy.</param>
        /// <param name="startIndex">
        /// The index to start copying at.
        /// </param>
        /// <param name="endIndex"> The index to stop copying at </param>
        /// <returns>The copied alg.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="alg"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <c><paramref name="startIndex"/> &lt; 0</c> or if
        /// <c>stopIndex &gt; <paramref name="alg"/>.Count()</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <c><paramref name="startIndex"/> &gt;
        /// <paramref name="endIndex"/></c>.
        /// </exception>
        public static Alg FromEnumerable(IEnumerable<Move> alg, int startIndex, int endIndex)
        {
            #region parameter checks
            if (alg is null)
                throw new ArgumentNullException(nameof(alg) + " is null");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex) + " is smaller than 0: " + startIndex);
            if (endIndex > alg.Count())
                throw new ArgumentOutOfRangeException(nameof(endIndex) + " is larger than the size of " + nameof(alg) + ": " + endIndex);
            if (startIndex > endIndex)
                throw new ArgumentException(nameof(startIndex) + " (" + startIndex + ") is larger than " + nameof(endIndex) + " (" + endIndex + ").");
            #endregion parameter checks

            return new Alg(alg
                   .Skip(startIndex)
                   .Take(endIndex - startIndex)
                   .ToArray());
        }

        /// <summary>
        /// Create an alg from a string representation.
        /// </summary>
        /// <param name="algString">A string representation of an alg.</param>
        /// <returns>The alg representated by the given string.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="algString"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="algString"/> is not a representation of
        /// an alg.
        /// </exception>
        public static Alg FromString(string algString)
        {
            if (algString is null)
                throw new ArgumentNullException(nameof(algString) + " is null.");

            return new Alg
            (
                algString.Split(' ')
                    .Select(moveString => Moves.MoveFromString(moveString))
                    .ToArray()
            );
        }

        /// <summary>
        /// Create a random alg. Does not allow two consecutive moves on the
        /// same face or three consecutive moves on the same axis.
        /// </summary>
        /// <param name="length">The length of the alg.</param>
        /// <param name="random">The random number generator used.</param>
        /// <returns>A random alg.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="random"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if length is smaller than zero.
        /// </exception>
        public static Alg FromRandomMoves(int length, Random random)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length) + " is smaller than 0: " + length);
            if (random is null)
                throw new ArgumentNullException(nameof(random) + " is null.");

            Face[] faces = new Face[length];
            if (length > 0)
            {
                faces[0] = (Face)random.Next(minValue: 0, maxValue: 6);
                if (length > 1)
                {
                    Face face = (Face)random.Next(minValue: 0, maxValue: 5); //value can be all faces but faces[0]
                    if (face == faces[0]) face = (Face)5;
                    faces[1] = face;
                }
                for (int index = 2; index < length; index++)
                {
                    Face face;
                    if (faces[index - 2].Axis() == faces[index - 1].Axis()) //if the two previous moves were on opposite faces
                    {
                        face = (Face)random.Next(minValue: 0, maxValue: 4); //value can be all moves but moves[index - 2] and moves[index - 1]
                        if (face.Axis() == faces[index - 1].Axis()) face = (Face)((int)face % 2 + 4); //move = 4 or move = 5
                    }
                    else
                    {
                        face = (Face)random.Next(minValue: 0, maxValue: 5);
                        if (face == faces[index - 1]) face = (Face)5;
                    }
                    faces[index] = face;
                }
            }

            Move[] moves = new Move[length];
            for (int index = 0; index < length; index++)
                moves[index] = Moves.MoveFromFaceAndQuarterTurns(faces[index], random.Next(1, 4));

            return new Alg(moves);
        }

        /// <summary>
        /// Create a random alg. Does not allow two consecutive moves on the
        /// same face or three consecutive moves on the same axis. At every
        /// point in the alg the probability of each move is equal to the
        /// probability of the move divided by the sum of all possible moves'
        /// probabilities.
        /// </summary>
        /// <param name="length">The length of the alg.</param>
        /// <param name="random">The random number generator used.</param>
        /// <param name="probabilities">
        /// The probability of each move relative to the other moves.
        /// </param>
        /// <returns>A random alg.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="random"/> or
        /// <paramref name="probabilities"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="length"/> is smaller than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="probabilities"/> does not have length 18,
        /// contains at least one negative value or if too many moves have a
        /// probability of 0.
        /// </exception>
        public static Alg FromRandomMoves(int length, Random random, double[] probabilities)
        {
            #region parameter checks
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length) + " is negative: " + length);
            if (random is null)
                throw new ArgumentNullException(nameof(random) + " is null.");
            if (probabilities is null)
                throw new ArgumentNullException(nameof(probabilities) + " is null.");
            if (probabilities.Length != NumMoves)
                throw new ArgumentException("The length of " + nameof(probabilities) + " must be " + NumMoves + ": " + probabilities.Length);

            for (int index = 0; index < NumMoves; index++)
                if (probabilities[index] < 0d)
                    throw new ArgumentException(nameof(probabilities) + " cannot contain negative values (index " + index + "): " + probabilities[index]);

            //IMPR
            if (length == 1)
            {
                if (probabilities.Sum() == 0d)
                    throw new ArgumentException("An alg of length 1 must have at least one move with non-zero probability.");
            }
            else if (length == 2)
            {
                int faceCount = probabilities
                    .Select((probability, index) => (probability, face: ((Move)index).Face()))
                    .Where(element => element.probability != 0d)
                    .Select(element => element.face)
                    .Distinct()
                    .Count();

                if (faceCount < 2)
                    throw new ArgumentException("An alg of length 2 must have at least two different faces with non-zero probabilities: " + faceCount);
            }
            else if (length > 2)
            {
                int axisCount = 0;
                for (int axis = 0; axis < NumAxes; axis++)
                    if (probabilities.Skip(axis * 3).Take(3).Sum() > 0d)
                        axisCount++;
                if (axisCount < 2)
                    throw new ArgumentException("An alg of length " + length + " must have at least two different faces with non-zero probabilities: " + axisCount);
            }
            #endregion parameter checks

            Move[] moves = Enumerable
                .Repeat((Move)(-1), length)
                .ToArray();

            for (int index = 0; index < length; index++)
            {
                double maxMoveValue = probabilities.Sum();
                Axis forbiddenAxis = (Axis)(-1);
                Face forbiddenFace = (Face)(-1);
                
                if (index > 0)
                {
                    if (index > 1)
                    {
                        if (moves[index - 1].Axis() == moves[index - 2].Axis())
                        {
                            forbiddenAxis = moves[index - 1].Axis();
                            for (int move = 0; move < 3; move++)
                            {
                                maxMoveValue -= probabilities[(int)forbiddenAxis * 3 + move];
                                maxMoveValue -= probabilities[(int)forbiddenAxis * 3 + move + 9];
                            }
                            goto next;
                        }
                    }

                    forbiddenFace = moves[index - 1].Face();
                    for (int qt = 1; qt <= 3; qt++)
                    {
                        Move forbiddenMove = Moves.MoveFromFaceAndQuarterTurns(forbiddenFace, qt);
                        maxMoveValue -= probabilities[(int)forbiddenMove];
                    }
                }

                next:
                double moveValue = random.NextDouble() * maxMoveValue;
                double progress = 0d;
                for (int move = 0; move < NumMoves; move++)
                {
                    if (((Move)move).Axis() == forbiddenAxis || ((Move)move).Face() == forbiddenFace)
                        continue;

                    progress += probabilities[move];
                    if (moveValue < progress)
                    {
                        moves[index] = (Move)move;
                        break;
                    }
                }
            }

            return new Alg(moves);
        }

        /// <summary>
        /// An empty alg.
        /// </summary>
        public static Alg Empty = new Alg(new Move[0]);

        private readonly Move[] _moves = null;

        private Alg(Move[] moves)
            => _moves = (Move[])moves.Clone();

        /// <summary>
        /// Get the move at a specific index.
        /// </summary>
        /// <param name="index">The index to be edited.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if <paramref name="index"/> is out of range.
        /// </exception>
        public Move this[int index]
        {
            get
            {
                if ((uint)index >= this.Length)
                    throw new IndexOutOfRangeException("Index is out of range: " + index);

                return _moves[index];
            }
        }

        /// <summary>
        /// The length of the alg.
        /// </summary>
        public int Length => _moves.Length;

        /// <summary>
        /// Get how many times each type of move is contained in the alg.
        /// <c><see cref="GetCountOfEachMove()"/>[(int)m]</c> (where m is a
        /// <see cref="Move"/>) returns how many times m is contained in the
        /// alg.
        /// </summary>
        /// <returns>
        /// An array containing the count of each type of move in the alg.
        /// </returns>
        public int[] GetCountOfEachMove()
        {
            int[] numberOfEachMove = Enumerable.Repeat(0, NumMoves)
                                               .ToArray();
            foreach (int move in _moves) numberOfEachMove[move]++;
            return numberOfEachMove;
        }

        /// <summary>
        /// Determines whether this object is equal to another object. They are
        /// considered equal, if the other object is enumerable and contains
        /// the same moves in the same order.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// Whether this object is equal to the other object.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IEnumerable<Move>)
                return Alg.AreEqual(this, obj as IEnumerable<Move>);
            else
                return false;
        }

        /// <summary>
        /// Determines whether this alg is equal to another alg. They are
        /// considered equal, if both contain the same moves in the same
        /// order.
        /// </summary>
        /// <param name="other">The alg to compare to.</param>
        /// <returns>
        /// Whether this alg is equal to the other alg.
        /// </returns>
        public bool Equals(IEnumerable<Move> other)
            => Alg.AreEqual(this, other);

        /// <summary>
        /// Get the inverse of this alg (reverse the sequence and change and
        /// change the direction of all moves).
        /// </summary>
        /// <returns>The inverse of this object.</returns>
        public Alg Inverse()
            => Alg.Inverse(this);

        /// <summary>
        /// Rotate this alg.
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>
        /// This object rotated by <paramref name="rotation"/>.
        /// </returns>
        public Alg Rotate(Rotation rotation)
            => Alg.Rotate(this, rotation);

        /// <summary>
        /// <inheritdoc/>
        /// Same as <see cref="ToString(IEnumerable{Move})"/>.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override string ToString()
            => Alg.ToString(_moves);

        //IMPR
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int returnValue = 0;
            int hash = 0;
            int iteration = 0;
            foreach (Move move in this)
            {
                hash = hash * 18 + (int)move;
                if (++iteration >= 7)
                {
                    returnValue ^= hash;
                    hash = 0;
                    iteration = 0;
                }
            }
            if (iteration != 0)
                returnValue ^= hash;
            return returnValue;
        }

        /// <inheritdoc/>
        public IEnumerator<Move> GetEnumerator()
            => ((IEnumerable<Move>)_moves).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
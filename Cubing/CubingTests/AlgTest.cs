using Cubing.ThreeByThree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CubingTests
{
    [TestClass]
    public class AlgTest
    {
        [TestMethod]
        public void EqualityTest()
        {
            Alg alg1 = Alg.FromEnumerable(new[] { Move.L2, Move.F1, Move.R3 });
            Alg alg2 = Alg.FromEnumerable(new[] { Move.L2, Move.F1, Move.R3 });
            Alg nullAlg = null;

            Assert.IsTrue(alg1.Equals(alg1));
            Assert.IsTrue(alg1.Equals(alg2));
            Assert.IsFalse(alg1.Equals(nullAlg));

            Assert.IsTrue(Alg.AreEqual(alg1, alg1));
            Assert.IsTrue(Alg.AreEqual(alg1, alg2));
            Assert.IsFalse(Alg.AreEqual(null, alg1));

            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(alg1 == alg1);
            Assert.IsTrue(alg1 == alg2);
            Assert.IsFalse(alg1 == nullAlg);
            Assert.IsFalse(nullAlg == alg1);

            Assert.IsFalse(alg1 != alg1);
            Assert.IsFalse(alg1 != alg2);
            Assert.IsTrue(alg1 != nullAlg);
            Assert.IsTrue(nullAlg != alg1);
            #pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void ToStringTest()
        {
            Alg alg = Alg.FromEnumerable(new[] { Move.B1, Move.R3, Move.F2 });
            string expected = "B R' F2";
            string result1 = Alg.ToString(alg);
            string result2 = alg.ToString();
            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
            Assert.ThrowsException<ArgumentNullException>(() => Alg.ToString(null));
        }

        [TestMethod]
        public void FromStringTest()
        {
            string alg = "B R' F2 D'";
            Assert.AreEqual(alg, Alg.ToString(Alg.FromString(alg)));
            Assert.ThrowsException<ArgumentException>(() => Alg.FromString("Exp"));
            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromString(null));
        }

        [TestMethod]
        public void FromRandomMovesTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Alg.FromRandomMoves(-1, random));
            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromRandomMoves(0, null));

            Assert.AreEqual(Alg.Empty, Alg.FromRandomMoves(0, random));

            Alg randomAlg = Alg.FromRandomMoves(length, random);

            Assert.AreEqual(length, randomAlg.Length);

            var enumerator = randomAlg.GetEnumerator();
            if (enumerator.MoveNext())
            {
                Move previous2 = enumerator.Current;
                if (enumerator.MoveNext())
                {
                    Move previous1 = enumerator.Current;
                    Assert.AreNotEqual(previous1.Face(), previous2.Face());
                    while (enumerator.MoveNext())
                    {
                        Assert.AreNotEqual(enumerator.Current.Face(), previous1.Face());
                        if (enumerator.Current.Axis() == previous1.Axis())
                            Assert.AreNotEqual(enumerator.Current.Axis(), previous2.Axis());
                        previous2 = previous1;
                        previous1 = enumerator.Current;
                    }
                }
            }
        }

        [TestMethod]
        public void FromRandomMoves2ExceptionTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            double[] probabilities = Enumerable
                .Repeat(1d, 18)
                .ToArray();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Alg.FromRandomMoves(-1, random, probabilities));
            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromRandomMoves(length, null, probabilities));
            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromRandomMoves(length, random, null));

            //negative probability value
            probabilities[7] = -1;
            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(length, random, probabilities));

            //all probability values zero
            probabilities = Enumerable
                .Repeat(0d, 18)
                .ToArray();

            Alg lengthZero = Alg.FromRandomMoves(0, random, probabilities);
            Assert.AreEqual(0, lengthZero.Length);

            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(1, random, probabilities));

            //one probability value non-zero
            probabilities[7] = 1d;

            Alg lengthOne = Alg.FromRandomMoves(1, random, probabilities);
            Assert.AreEqual(Alg.FromString("F2"), lengthOne);

            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(2, random, probabilities));

            //two probability values non-zero
            probabilities[8] = 1d;
            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(2, random, probabilities));

            probabilities[8] = 0d;
            probabilities[15] = 1d;

            Alg lengthTwo = Alg.FromRandomMoves(2, random, probabilities);
            CollectionAssert.AreEquivalent(Alg.FromString("F2 B").ToArray(), lengthTwo.ToArray());

            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(3, random, probabilities));

            //three probability values non-zero
            probabilities[8] = 1d;
            Assert.ThrowsException<ArgumentException>(() => Alg.FromRandomMoves(3, random, probabilities));

            probabilities[8] = 0d;
            probabilities[0] = 1d;

            Alg lengthThree = Alg.FromRandomMoves(3, random, probabilities);
            Assert.IsTrue(lengthThree.Contains(Move.R1));
        }

        [TestMethod]
        public void FromRandomMoves2Test()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 1000;

            double[] probabilities = Enumerable
                .Repeat(0d, Constants.NumMoves)
                .ToArray();

            probabilities[9] = 100000;
            probabilities[3] = 1;
            probabilities[4] = 1;
            probabilities[5] = 1;
            probabilities[6] = 3;

            Move[] possibleMoves = { Move.L1, Move.U1, Move.U2, Move.U3, Move.F1 };

            int[] sumOfMoves = Enumerable.
                Repeat(0, Constants.NumMoves)
                .ToArray();

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                int lastCount = sumOfMoves.Sum();

                Alg randomAlg = Alg.FromRandomMoves(length, random, probabilities);
                Assert.AreEqual(length, randomAlg.Length);

                for (int move = 0; move < Constants.NumMoves; move++)
                    sumOfMoves[move] += randomAlg.Count(randomMove => randomMove == (Move)move);

                Assert.AreEqual(length, sumOfMoves.Sum() - lastCount);
            }
            Assert.AreEqual(length * repetitions, sumOfMoves.Sum());

            double[] effectiveProbabilities = sumOfMoves
                .Select(count => count / (double)(length * repetitions))
                .ToArray();

            //make sure only move with probability > 0 occur
            double impossibleMoveProbability = effectiveProbabilities
                .Where((probability, index) => !possibleMoves.Contains((Move)index))
                .Sum();
            Assert.AreEqual(0d, impossibleMoveProbability);

            //compare the effective probabilities
            double delta = 0.02d;

            Assert.AreEqual(1 /  2d, effectiveProbabilities[9], delta);
            Assert.AreEqual(1 /  4d, effectiveProbabilities[6], delta);
            Assert.AreEqual(1 / 12d, effectiveProbabilities[3], delta);
            Assert.AreEqual(1 / 12d, effectiveProbabilities[4], delta);
            Assert.AreEqual(1 / 12d, effectiveProbabilities[5], delta);
        }

        [TestMethod]
        public void FromEnumerableTest()
        {
            Move[] moves = { Move.R1, Move.D2, Move.B3 };

            Alg alg1 = Alg.FromEnumerable(moves);
            Assert.AreEqual("R D2 B'", alg1.ToString());
            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromEnumerable(null));

            Alg alg2 = Alg.FromEnumerable(moves, 0, moves.Length);
            Assert.AreEqual(alg2, alg1);

            for (int i = 0; i < moves.Length; i++)
            {
                Alg alg3 = Alg.FromEnumerable(moves, i, i);
                Assert.AreEqual(0, alg3.Length);
            }

            Alg alg4a = Alg.FromEnumerable(moves, 0, 2);
            Alg alg4b = Alg.FromEnumerable(moves.Take(2));
            Assert.AreEqual(alg4b, alg4a);

            Alg alg5a = Alg.FromEnumerable(moves, 1, 3);
            Alg alg5b = Alg.FromEnumerable(moves.Skip(1));
            Assert.AreEqual(alg5b, alg5a);

            Assert.ThrowsException<ArgumentNullException>(() => Alg.FromEnumerable(null, 0, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Alg.FromEnumerable(moves, -1, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Alg.FromEnumerable(moves, 0, moves.Length + 1));
            Assert.ThrowsException<ArgumentException>(() => Alg.FromEnumerable(moves, 1, 0));
        }

        [TestMethod]
        public void CreateEmptyTest()
        {
            Assert.AreEqual("", Alg.Empty.ToString());
            Assert.AreEqual(0, Alg.Empty.Length);
            #pragma warning disable IDE0059 // Unnecessary assignment of a value
            foreach (Move move in Alg.Empty)
                Assert.Inconclusive();
            #pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        [TestMethod]
        public void IndexerTest()
        {
            Alg alg = Alg.FromString("R U2 F'");

            Assert.AreEqual(Move.R1, alg[0]);
            Assert.AreEqual(Move.U2, alg[1]);
            Assert.AreEqual(Move.F3, alg[2]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => alg[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => alg[4]);
        }

        [TestMethod]
        public void LengthTest()
        {
            Assert.AreEqual(3, Alg.FromString("F2 D R'").Length);
            Assert.AreEqual(5, Alg.FromString("L B2 L2 B' F2").Length);
        }

        [TestMethod]
        public void GetNumberOfEachMoveTest()
        {
            Alg alg = Alg.FromString("L2 B F' R L2 F2 L2 R");
            int[] expected = {
                2, 0, 0, 0, 0, 0, 0, 1, 1,
                0, 3, 0, 0, 0, 0, 1, 0, 0 };
            int[] result = alg.GetCountOfEachMove();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void InverseTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Alg original = Alg.FromString("R U R' F' R U R' U' R' F R2 U' R' U'");
            Alg expected = Alg.FromString("U R U R2 F' R U R U' R' F R U' R'");
            Alg result = Alg.Inverse(original);
            Assert.AreEqual(expected, result);

            Alg randomMoves = Alg.FromRandomMoves(length, random);
            Assert.AreEqual(randomMoves, Alg.Inverse(Alg.Inverse(randomMoves)));
            Assert.AreEqual(randomMoves, randomMoves.Inverse().Inverse());
            Assert.AreEqual(randomMoves.Inverse(), Alg.Inverse(randomMoves));
            Assert.ThrowsException<ArgumentNullException>(() => Alg.Inverse(null));
        }

        [TestMethod]
        public void RotateTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Rotation rotation = Rotation.x1;

            Alg original = Alg.FromString("B2 D U'");
            Alg expected = Alg.FromString("D2 F B'");
            Assert.AreEqual(expected, original.Rotate(rotation));

            Alg randomMoves = Alg.FromRandomMoves(length, random);
            Assert.AreEqual(randomMoves.Inverse(), Alg.Inverse(randomMoves));
            Assert.ThrowsException<ArgumentNullException>(() =>
                Alg.Rotate(null, rotation));
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            Random random = new Random(7777777);
            int length = 50;

            Alg alg = Alg.FromRandomMoves(length, random);

            int index = 0;
            foreach (Move move in alg)
                Assert.AreEqual(alg[index++], move);
            Assert.AreEqual(alg.Length, index);
        }

        [TestMethod]
        public void OperatorTest()
        {
            Random random = new Random(7777777);
            int length1 = 50, length2 = 50;
            int times = 4;

            Alg nullAlg = null;

            // +
            Alg alg1 = Alg.FromRandomMoves(length1, random);
            Alg alg2 = Alg.FromRandomMoves(length2, random);
            Alg addTest = alg1 + alg2;
            Assert.AreEqual(alg1, Alg.FromEnumerable(addTest, 0,
                alg1.Length));
            Assert.AreEqual(alg2, Alg.FromEnumerable(addTest, alg1.Length,
                alg1.Length + alg2.Length));

            Assert.ThrowsException<ArgumentNullException>(() => nullAlg + alg1);
            Assert.ThrowsException<ArgumentNullException>(() => alg1 + nullAlg);

            // *
            Alg multiplyTest = alg1 * times;
            for (int i = 0; i < times; i++)
                Assert.AreEqual(alg1, Alg.FromEnumerable(multiplyTest,
                    i * alg1.Length, (i + 1) * alg1.Length));

            Assert.ThrowsException<ArgumentNullException>(() => nullAlg * times);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => alg1 * -1);
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            Random random = new Random(7777777);
            int length = 50;
            int repetitions = 50;

            for (int rep = 0; rep < repetitions; rep++)
            {
                Alg alg = Alg.FromRandomMoves(length, random);
                Alg clone = Alg.FromEnumerable(alg);
                Assert.AreEqual(alg.GetHashCode(), clone.GetHashCode());
            }
        }
    }
}
using Cubing.ThreeByThree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CubingTests
{
    [TestClass]
    public class MovesTest
    {
        [TestMethod]
        public void EnumTest()
        {
            Assert.AreEqual(0 , (int)Move.R1);
            Assert.AreEqual(1 , (int)Move.R2);
            Assert.AreEqual(2 , (int)Move.R3);
            Assert.AreEqual(3 , (int)Move.U1);
            Assert.AreEqual(4 , (int)Move.U2);
            Assert.AreEqual(5 , (int)Move.U3);
            Assert.AreEqual(6 , (int)Move.F1);
            Assert.AreEqual(7 , (int)Move.F2);                ;
            Assert.AreEqual(8 , (int)Move.F3);
            Assert.AreEqual(9 , (int)Move.L1);
            Assert.AreEqual(10, (int)Move.L2);
            Assert.AreEqual(11, (int)Move.L3);
            Assert.AreEqual(12, (int)Move.D1);
            Assert.AreEqual(13, (int)Move.D2);
            Assert.AreEqual(14, (int)Move.D3);
            Assert.AreEqual(15, (int)Move.B1);
            Assert.AreEqual(16, (int)Move.B2);
            Assert.AreEqual(17, (int)Move.B3);
        }

        [TestMethod]
        public void MoveToStringTest()
        {
            Assert.AreEqual("R" , Moves.MoveToString(Move.R1));
            Assert.AreEqual("R2", Moves.MoveToString(Move.R2));
            Assert.AreEqual("R'", Moves.MoveToString(Move.R3));
            Assert.AreEqual("U" , Moves.MoveToString(Move.U1));
            Assert.AreEqual("U2", Moves.MoveToString(Move.U2));
            Assert.AreEqual("U'", Moves.MoveToString(Move.U3));
            Assert.AreEqual("F" , Moves.MoveToString(Move.F1));
            Assert.AreEqual("F2", Moves.MoveToString(Move.F2));
            Assert.AreEqual("F'", Moves.MoveToString(Move.F3));
            Assert.AreEqual("L" , Moves.MoveToString(Move.L1));
            Assert.AreEqual("L2", Moves.MoveToString(Move.L2));
            Assert.AreEqual("L'", Moves.MoveToString(Move.L3));
            Assert.AreEqual("D" , Moves.MoveToString(Move.D1));
            Assert.AreEqual("D2", Moves.MoveToString(Move.D2));
            Assert.AreEqual("D'", Moves.MoveToString(Move.D3));
            Assert.AreEqual("B" , Moves.MoveToString(Move.B1));
            Assert.AreEqual("B2", Moves.MoveToString(Move.B2));
            Assert.AreEqual("B'", Moves.MoveToString(Move.B3));
        }

        [TestMethod]
        public void MoveFromStringTest()
        {
            Assert.AreEqual(Move.R1, Moves.MoveFromString("R" ));
            Assert.AreEqual(Move.R2, Moves.MoveFromString("R2"));
            Assert.AreEqual(Move.R3, Moves.MoveFromString("R'"));
            Assert.AreEqual(Move.U1, Moves.MoveFromString("U" ));
            Assert.AreEqual(Move.U2, Moves.MoveFromString("U2"));
            Assert.AreEqual(Move.U3, Moves.MoveFromString("U'"));
            Assert.AreEqual(Move.F1, Moves.MoveFromString("F" ));
            Assert.AreEqual(Move.F2, Moves.MoveFromString("F2"));
            Assert.AreEqual(Move.F3, Moves.MoveFromString("F'"));
            Assert.AreEqual(Move.L1, Moves.MoveFromString("L" ));
            Assert.AreEqual(Move.L2, Moves.MoveFromString("L2"));
            Assert.AreEqual(Move.L3, Moves.MoveFromString("L'"));
            Assert.AreEqual(Move.D1, Moves.MoveFromString("D" ));
            Assert.AreEqual(Move.D2, Moves.MoveFromString("D2"));
            Assert.AreEqual(Move.D3, Moves.MoveFromString("D'"));
            Assert.AreEqual(Move.B1, Moves.MoveFromString("B" ));
            Assert.AreEqual(Move.B2, Moves.MoveFromString("B2"));
            Assert.AreEqual(Move.B3, Moves.MoveFromString("B'"));

            Assert.ThrowsException<ArgumentNullException>(() => Moves.MoveFromString(null));
            Assert.ThrowsException<ArgumentException>(() => Moves.MoveFromString("Exp"));
        }

        public void AxisTest()
        {
            Assert.AreEqual(Axis.x, Move.R1.Axis());
            Assert.AreEqual(Axis.x, Move.R2.Axis());
            Assert.AreEqual(Axis.x, Move.R3.Axis());
            Assert.AreEqual(Axis.x, Move.L1.Axis());
            Assert.AreEqual(Axis.x, Move.L2.Axis());
            Assert.AreEqual(Axis.x, Move.L3.Axis());
            Assert.AreEqual(Axis.y, Move.U1.Axis());
            Assert.AreEqual(Axis.y, Move.U2.Axis());
            Assert.AreEqual(Axis.y, Move.U3.Axis());
            Assert.AreEqual(Axis.y, Move.D1.Axis());
            Assert.AreEqual(Axis.y, Move.D2.Axis());
            Assert.AreEqual(Axis.y, Move.D3.Axis());
            Assert.AreEqual(Axis.z, Move.F1.Axis());
            Assert.AreEqual(Axis.z, Move.F2.Axis());
            Assert.AreEqual(Axis.z, Move.F3.Axis());
            Assert.AreEqual(Axis.z, Move.B1.Axis());
            Assert.AreEqual(Axis.z, Move.B2.Axis());
            Assert.AreEqual(Axis.z, Move.B3.Axis());
        }
    
        public void FaceTest()
        {
            Assert.AreEqual(Face.R, Move.R1.Face());
            Assert.AreEqual(Face.R, Move.R2.Face());
            Assert.AreEqual(Face.R, Move.R3.Face());
            Assert.AreEqual(Face.U, Move.U1.Face());
            Assert.AreEqual(Face.U, Move.U2.Face());
            Assert.AreEqual(Face.U, Move.U3.Face());
            Assert.AreEqual(Face.F, Move.F1.Face());
            Assert.AreEqual(Face.F, Move.F2.Face());
            Assert.AreEqual(Face.F, Move.F3.Face());
            Assert.AreEqual(Face.L, Move.L1.Face());
            Assert.AreEqual(Face.L, Move.L2.Face());
            Assert.AreEqual(Face.L, Move.L3.Face());
            Assert.AreEqual(Face.D, Move.D1.Face());
            Assert.AreEqual(Face.D, Move.D2.Face());
            Assert.AreEqual(Face.D, Move.D3.Face());
            Assert.AreEqual(Face.B, Move.B1.Face());
            Assert.AreEqual(Face.B, Move.B2.Face());
            Assert.AreEqual(Face.B, Move.B3.Face());
        }

        [TestMethod]
        public void MoveFromFaceAndQuarterTurnsTest()
        {
            Assert.AreEqual(Move.R1, Moves.MoveFromFaceAndQuarterTurns(Face.R, 1));
            Assert.AreEqual(Move.U3, Moves.MoveFromFaceAndQuarterTurns(Face.U, 3));
            Assert.AreEqual(Move.B2, Moves.MoveFromFaceAndQuarterTurns(Face.B, 2));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Moves.MoveFromFaceAndQuarterTurns(Face.D, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Moves.MoveFromFaceAndQuarterTurns(Face.D, 0));
        }

        [TestMethod]
        public void QuarterTurnsTest()
        {
            Assert.AreEqual(1, Move.L1.QuarterTurns());
            Assert.AreEqual(2, Move.R2.QuarterTurns());
            Assert.AreEqual(3, Move.U3.QuarterTurns());
        }

        [TestMethod]
        public void InverseTest()
        {
            Assert.AreEqual(Move.B3, Move.B1.Inverse());
            Assert.AreEqual(Move.L2, Move.L2.Inverse());
            Assert.AreEqual(Move.F1, Move.F3.Inverse());
        }

        [TestMethod]
        public void RotateTest()
        {
            foreach (Move move in Enum.GetValues(typeof(Move)))
                foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
                {
                    Assert.AreEqual(move.QuarterTurns(), move.Rotate(rotation).QuarterTurns());
                    Assert.AreEqual(move.Face().Rotate(rotation), move.Rotate(rotation).Face());
                }

            Assert.AreEqual(Move.F1, Move.D1.Rotate(Rotation.x1));
            Assert.AreEqual(Move.L3, Move.F3.Rotate(Rotation.y1));
            Assert.AreEqual(Move.B2, Move.F2.Rotate(Rotation.y2));
            Assert.AreEqual(Move.D1, Move.L1.Rotate(Rotation.z3));
        }
    }
}
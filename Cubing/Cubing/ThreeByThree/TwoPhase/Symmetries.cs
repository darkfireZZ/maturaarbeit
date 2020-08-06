using System.Linq;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains utilities for manipultion of cubes with symmetries.
    /// </summary>
    public static class Symmetries
    {
        /// <summary>
        /// The number of posiible symmetries of a 3x3x3 cube.
        /// </summary>
        public const int NumSymmetries = 48;

        /// <summary>
        /// The 48 possible symmetries represented as
        /// <see cref="CubieCube"/>s. Do not change.
        /// </summary>
        public static readonly CubieCube[] SymmetryCubes = null;
        /// <summary>
        /// <c><see cref="MultiplyIndex"/>[i, k]</c> is
        /// the product of the two symmetries represented by the indexes
        /// <c>i</c> and <c>k</c>. Do not change.
        /// </summary>
        public static readonly int[,] MultiplyIndex = null;
        /// <summary>
        /// <c><see cref="MultiplyIndex"/>[i]</c> is the inverse of the cube
        /// represented by <c>i</c>. Do not change.
        /// </summary>
        public static readonly int[] InverseIndex = null;
        /// <summary>
        /// <c><see cref="RotationIndex"/>[r]</c> is the index of the cube
        /// that represents the rotation <c>r</c>. Do not change.
        /// </summary>
        public static readonly int[] RotationIndex = null;
        /// <summary>
        /// <c><see cref="MirrorIndex"/>[a]</c> is the index of the cube
        /// that represents the mirror <c>a</c>. Do not change.
        /// </summary>
        public static readonly int[] MirrorIndex = null;

        static Symmetries()
        {
            CubieCube cube; //avoid name clashes

            #region initialize SymmetryCubes
            SymmetryCubes = new CubieCube[NumSymmetries];
            cube = CubieCube.CreateSolved();

            for (int x1y1 = 0; x1y1 < 3; x1y1++)
            {
                for (int z2 = 0; z2 < 2; z2++)
                {
                    for (int y1 = 0; y1 < 4; y1++)
                    {
                        for (int mirror = 0; mirror < 2; mirror++)
                        {
                            int index = (x1y1 << 4) + (z2 << 3) + (y1 << 1) + mirror;
                            SymmetryCubes[index] = cube.Clone();
                            cube.Mirror(Axis.x);
                        }
                        cube.Rotate(Rotation.y1);
                    }
                    cube.Rotate(Rotation.z2);
                }
                cube.Rotate(Rotation.x1);
                cube.Rotate(Rotation.y1);
            }
            #endregion initialize SymmetryCubes

            #region initialize MultiplyIndex
            MultiplyIndex = new int[NumSymmetries, NumSymmetries];
            for (int t1 = 0; t1 < NumSymmetries; t1++)
                for (int t2 = 0; t2 < NumSymmetries; t2++)
                    MultiplyIndex[t1, t2] = -1;

            for (int t1 = 0; t1 < NumSymmetries; t1++)
                for (int t2 = 0; t2 < NumSymmetries; t2++)
                {
                    cube = SymmetryCubes[t1].Clone();
                    cube.Multiply(SymmetryCubes[t2]);
                    for (int prodIndex = 0; prodIndex < NumSymmetries;
                        prodIndex++)
                    {
                        if (cube == SymmetryCubes[prodIndex])
                        {
                            MultiplyIndex[t1, t2] = prodIndex;
                            break;
                        }
                    }
                }
            #endregion MultiplyIndex

            #region initialize InverseIndex
            InverseIndex = Enumerable.Repeat(-1, NumSymmetries).ToArray();
            
            for (int inverseIndex = 0; inverseIndex < NumSymmetries;
                inverseIndex++)
            {
                CubieCube inverseCube = Symmetries.SymmetryCubes[inverseIndex];
                for (int index = 0; index < NumSymmetries; index++)
                {
                    cube = SymmetryCubes[index].Clone();
                    cube.Multiply(inverseCube);
                    if (cube == CubieCube.CreateSolved())
                    {
                        InverseIndex[index] = inverseIndex;
                        break;
                    }
                }
            }
            #endregion initialize InverseIndex

            #region initialize RotationIndex
            RotationIndex = Enumerable.Repeat(-1, NumRotations).ToArray();

            for (int rotation = 0; rotation < NumRotations; rotation++)
            {
                cube = CubieCube.RotationsArray[rotation];
                for (int t = 0; t < NumSymmetries; t++)
                    if (cube == SymmetryCubes[t])
                    {
                        RotationIndex[rotation] = t;
                        break;
                    }
            }
            #endregion initialize RotationIndex

            #region initialize MirrorIndex
            MirrorIndex = Enumerable.Repeat(-1, NumAxes).ToArray();

            for (int axis = 0; axis < NumAxes; axis++)
            {
                cube = CubieCube.CreateSolved();
                cube.Mirror((Axis)axis);
                for (int t = 0; t < NumSymmetries; t++)
                    if (cube == SymmetryCubes[t])
                    {
                        MirrorIndex[axis] = t;
                        break;
                    }
            }
            #endregion initialize MirrorIndex
        }
    }
}
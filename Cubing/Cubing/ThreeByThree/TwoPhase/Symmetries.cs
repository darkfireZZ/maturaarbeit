using System;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains utilities related to symmetries.
    /// </summary>
    public static class Symmetries
    {
        /// <summary>
        /// The number of possible symmetries of a 3x3x3 cube.
        /// </summary>
        public const int NumSymmetries = 48;

        /// <summary>
        /// The number of symmetries in the subgroup Dh4.
        /// </summary>
        public const int NumSymmetriesDh4 = NumSymmetries / 3; //16

        /// <summary>
        /// The 48 possible symmetries represented as <see cref="CubieCube"/>s.
        /// Do not change.
        /// </summary>
        public static readonly CubieCube[] SymmetryCubes = null;
        /// <summary>
        /// <c><see cref="MultiplyIndex"/>[i, k]</c> is the product of the two
        /// symmetries represented by the indexes <c>i</c> and <c>k</c>. Do not
        /// change.
        /// </summary>
        public static readonly int[,] MultiplyIndex = null;
        /// <summary>
        /// <c><see cref="InverseIndex"/>[i]</c> is the symmetry index inverse
        /// of the symmetry cube represented by the index <c>i</c>. Do not
        /// change.
        /// </summary>
        public static readonly int[] InverseIndex = null;
        /// <summary>
        /// <c><see cref="RotationIndex"/>[r]</c> is the symmetry index of the cube
        /// that represents the <see cref="Rotation"/> <c>r</c>. Do not change.
        /// </summary>
        public static readonly int[] RotationIndex = null;
        /// <summary>
        /// <c><see cref="MirrorIndex"/>[a]</c> is the symmetry index of the
        /// cube that represents the mirror through the <see cref="Axis"/>
        /// <c>a</c>. Do not change.
        /// </summary>
        public static readonly int[] MirrorIndex = null;

        static Symmetries()
        {
            CubieCube cube; //avoid name clashes

            #region initialize SymmetryCubes
            SymmetryCubes = Enumerable.Repeat<CubieCube>(null, NumSymmetries)
                                      .ToArray();
            cube = CubieCube.CreateSolved();

            for (int x1y1 = 0; x1y1 < 3; x1y1++) //rotate by 120°
            {
                for (int z2 = 0; z2 < 2; z2++) //rotate by 180°
                {
                    for (int y1 = 0; y1 < 4; y1++) //rotate by 90°
                    {
                        for (int mirror = 0; mirror < 2; mirror++) //mirror
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

            //invalidate
            for (int symmetryIndex1 = 0; symmetryIndex1 < NumSymmetries; symmetryIndex1++)
                for (int symmetryIndex2 = 0; symmetryIndex2 < NumSymmetries; symmetryIndex2++)
                    MultiplyIndex[symmetryIndex1, symmetryIndex2] = -1;

            //populate
            for (int symmetryIndex1 = 0; symmetryIndex1 < NumSymmetries; symmetryIndex1++)
                for (int symmetryIndex2 = 0; symmetryIndex2 < NumSymmetries; symmetryIndex2++)
                {
                    cube = SymmetryCubes[symmetryIndex1].Clone();
                    cube.Multiply(SymmetryCubes[symmetryIndex2]);
                    for (int productIndex = 0; productIndex < NumSymmetries; productIndex++)
                    {
                        if (cube == SymmetryCubes[productIndex])
                        {
                            MultiplyIndex[symmetryIndex1, symmetryIndex2] = productIndex;
                            break;
                        }
                    }
                }
            #endregion MultiplyIndex

            #region initialize InverseIndex
            InverseIndex = Enumerable.Repeat(-1, NumSymmetries)
                                     .ToArray();
            
            for (int inverseIndex = 0; inverseIndex < NumSymmetries; inverseIndex++)
            {
                CubieCube inverseCube = SymmetryCubes[inverseIndex];
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetries; symmetryIndex++)
                {
                    cube = SymmetryCubes[symmetryIndex].Clone();
                    cube.Multiply(inverseCube);
                    if (cube == CubieCube.CreateSolved())
                    {
                        InverseIndex[symmetryIndex] = inverseIndex;
                        break;
                    }
                }
            }
            #endregion initialize InverseIndex

            #region initialize RotationIndex
            RotationIndex = Enumerable.Repeat(-1, NumRotations)
                                      .ToArray();

            foreach (Rotation rotation in Enum.GetValues(typeof(Rotation)))
            {
                cube = CubieCube.RotationsArray[(int)rotation];
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetries; symmetryIndex++)
                    if (cube == SymmetryCubes[symmetryIndex])
                    {
                        RotationIndex[(int)rotation] = symmetryIndex;
                        break;
                    }
            }
            #endregion initialize RotationIndex

            #region initialize MirrorIndex
            MirrorIndex = Enumerable.Repeat(-1, NumAxes)
                                    .ToArray();

            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                cube = CubieCube.CreateSolved();
                cube.Mirror(axis);
                for (int symmetryIndex = 0; symmetryIndex < NumSymmetries; symmetryIndex++)
                    if (cube == SymmetryCubes[symmetryIndex])
                    {
                        MirrorIndex[(int)axis] = symmetryIndex;
                        break;
                    }
            }
            #endregion initialize MirrorIndex
        }
    }
}
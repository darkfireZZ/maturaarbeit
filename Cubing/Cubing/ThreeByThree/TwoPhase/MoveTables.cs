using System;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Contains move tables used to accelerate applying moves to a cube.
    /// </summary>
    public static class MoveTables
    {
        /// <summary>
        /// <c><see cref="Phase1IndexToPhase2Index"/>[m]</c>, where <c>m</c> is
        /// the index of a phase 1 move, contains the index of the same move
        /// used in phase 2. If the move is not used in phase 2, the value
        /// stored is <c>-1</c>. Do not change.
        /// </summary>
        public static int[] Phase1IndexToPhase2Index = new int[] {
            -1, 0, -1, // R, R2, R'
             1, 2,  3, // U, U2, U'
            -1, 4, -1, // F, F2, F'
            -1, 5, -1, // L, L2, L'
             6, 7,  8, // D, D2, D'
            -1, 9, -1  // B, B2, B'
        };

        /// <summary>
        /// <c><see cref="Phase2IndexToPhase1Index"/>[m]</c>, where <c>m</c> is
        /// the index of a phase 2 move, contains the index of the same move
        /// used in phase 1. Do not change.
        /// </summary>
        public static int[] Phase2IndexToPhase1Index = new int[]
        {
             1,  3,  4,  5,  7, // R2, U, U2, U', F2
            10, 12, 13, 14, 16  // L2, D, D2, D', B2
        };

        /// <summary>
        /// Create a move table for corner orientation.
        /// </summary>
        /// <returns>
        /// A move table for corner orientation.
        /// </returns>
        public static short[,] CreateCornerOrientationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] cornerOrientationMoveTable = new short[Coordinates.NumCornerOrientations, NumMoves];

            //invalidate table
            for (int cornerOrientation = 0; cornerOrientation < Coordinates.NumCornerOrientations; cornerOrientation++)
                for (int move = 0; move < NumMoves; move++)
                    cornerOrientationMoveTable[cornerOrientation, move] = -1;

            //populate table
            for (int cornerOrientation = 0; cornerOrientation < Coordinates.NumCornerOrientations; cornerOrientation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetCornerOrientation(cube, cornerOrientation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyCorners(CubieCube.MovesArray[face * 3]);
                        cornerOrientationMoveTable[cornerOrientation, face * 3 + move] = (short)Coordinates.GetCornerOrientation(cube);
                    }
                }

            return cornerOrientationMoveTable;
        }

        /// <summary>
        /// Create a move table for edge orientation.
        /// </summary>
        /// <returns>
        /// A move table for edge orientation.
        /// </returns>
        public static short[,] CreateEdgeOrientationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] edgeOrientationMoveTable = new short[Coordinates.NumEdgeOrientations, NumMoves];

            //invalidate table
            for (int edgeOrientation = 0; edgeOrientation < Coordinates.NumEdgeOrientations; edgeOrientation++)
                for (int move = 0; move < NumMoves; move++)
                    edgeOrientationMoveTable[edgeOrientation, move] = -1;

            //populate table
            for (int edgeOrientation = 0; edgeOrientation < Coordinates.NumEdgeOrientations; edgeOrientation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetEdgeOrientation(cube, edgeOrientation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        edgeOrientationMoveTable[edgeOrientation, face * 3 + move] = (short)Coordinates.GetEdgeOrientation(cube);
                    }
                }

            return edgeOrientationMoveTable;
        }

        /// <summary>
        /// Create a move table for equator distribution.
        /// </summary>
        /// <returns>
        /// A move table for equator distribution.
        /// </returns>
        public static short[,] CreateEquatorDistributionMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] equatorDistributionMoveTable = new short[Coordinates.NumEquatorDistributions, NumMoves];

            //invalidate table
            for (int equatorDistribution = 0; equatorDistribution < Coordinates.NumEquatorDistributions; equatorDistribution++)
                for (int move = 0; move < NumMoves; move++)
                    equatorDistributionMoveTable[equatorDistribution, move] = -1;

            //populate table
            for (int equatorDistribution = 0; equatorDistribution < Coordinates.NumEquatorDistributions; equatorDistribution++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetEquatorDistribution(cube, equatorDistribution);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        equatorDistributionMoveTable[equatorDistribution, face * 3 + move] = (short)Coordinates.GetEquatorDistribution(cube);
                    }
                }

            return equatorDistributionMoveTable;
        }

        //TODO remove redundant indexes
        /// <summary>
        /// Create a move table for equator order. Only valid for cubes in the
        /// subgroup G1.
        /// </summary>
        /// <returns>
        /// A move table for equator order.
        /// </returns>
        public static sbyte[,] CreateEquatorOrderMoveTable()
        {
            sbyte[,] equatorOrderMoveTable = new sbyte[Coordinates.NumEquatorOrders, NumMoves];

            //invalidate table
            for (int equatorOrder = 0; equatorOrder < Coordinates.NumEquatorOrders; equatorOrder++)
                for (int move = 0; move < NumMoves; move++)
                    equatorOrderMoveTable[equatorOrder, move] = -1;

            //populate table
            for (int equatorOrder = 0; equatorOrder < Coordinates.NumEquatorOrders; equatorOrder++)
                for (int face = 0; face < NumFaces; face++)
                {
                    CubieCube cube = CubieCube.CreateSolved();
                    Coordinates.SetEquatorOrder(cube, equatorOrder);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        if (TwoPhaseConstants.Phase2Moves.Contains((Move)(face * 3 + move)))
                            equatorOrderMoveTable[equatorOrder, face * 3 + move] = (sbyte)Coordinates.GetEquatorOrder(cube);
                    }
                }

            return equatorOrderMoveTable;
        }

        /// <summary>
        /// Create a move table for equator permutation
        /// </summary>
        /// <returns>
        /// A move table for equator permutation.
        /// </returns>
        public static short[,] CreateEquatorPermutationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] equatorPermutationMoveTable = new short[Coordinates.NumEquatorPermutations, NumMoves];

            //invalidate table
            for (int equatorPermutation = 0; equatorPermutation < Coordinates.NumEquatorPermutations; equatorPermutation++)
                for (int move = 0; move < NumMoves; move++)
                    equatorPermutationMoveTable[equatorPermutation, move] = -1;

            //populate table
            for (int equatorPermutation = 0; equatorPermutation < Coordinates.NumEquatorPermutations; equatorPermutation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetEquatorPermutation(cube, equatorPermutation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        equatorPermutationMoveTable[equatorPermutation, face * 3 + move] = (short)Coordinates.GetEquatorPermutation(cube);
                    }
                }

            return equatorPermutationMoveTable;
        }

        /// <summary>
        /// Create a move table for corner permutation.
        /// </summary>
        /// <returns>
        /// A move table for corner permutation.
        /// </returns>
        public static ushort[,] CreateCornerPermutationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            ushort[,] cornerPermutationMoveTable = new ushort[Coordinates.NumCornerPermutations, NumMoves];

            //invalidate table
            for (int cornerPermutation = 0; cornerPermutation < Coordinates.NumCornerPermutations; cornerPermutation++)
                for (int move = 0; move < NumMoves; move++)
                    cornerPermutationMoveTable[cornerPermutation, move] = ushort.MaxValue;

            //populate table
            for (int cornerPermutation = 0; cornerPermutation < Coordinates.NumCornerPermutations; cornerPermutation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetCornerPermutation(cube, cornerPermutation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyCorners(CubieCube.MovesArray[face * 3]);
                        cornerPermutationMoveTable[cornerPermutation, face * 3 + move] = (ushort)Coordinates.GetCornerPermutation(cube);
                    }
                }

            return cornerPermutationMoveTable;
        }

        /// <summary>
        /// Create a move table for U-edge permutation.
        /// </summary>
        /// <returns>
        /// A move table for U-edge permutation.
        /// </returns>
        public static short[,] CreateUEdgePermutationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] uEdgePermutationMoveTable = new short[Coordinates.NumUEdgePermutations, NumMoves];

            //invalidate table
            for (int uEdgePermutation = 0; uEdgePermutation < Coordinates.NumUEdgePermutations; uEdgePermutation++)
                for (int move = 0; move < NumMoves; move++)
                    uEdgePermutationMoveTable[uEdgePermutation, move] = -1;

            //populate table
            for (int uEdgePermutation = 0; uEdgePermutation < Coordinates.NumUEdgePermutations; uEdgePermutation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetUEdgePermutation(cube, uEdgePermutation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        uEdgePermutationMoveTable[uEdgePermutation, face * 3 + move] = (short)Coordinates.GetUEdgePermutation(cube);
                    }
                }

            return uEdgePermutationMoveTable;
        }
        
        /// <summary>
        /// Create a move table for D-edge permutation.
        /// </summary>
        /// <returns>
        /// A move table for D-edge permutation.
        /// </returns>
        public static short[,] CreateDEdgePermutationMoveTable()
        {
            CubieCube cube = CubieCube.CreateSolved();
            short[,] dEdgePermutationMoveTable = new short[Coordinates.NumDEdgePermutations, NumMoves];

            //invalidate table
            for (int dEdgePermutation = 0; dEdgePermutation < Coordinates.NumDEdgePermutations; dEdgePermutation++)
                for (int move = 0; move < NumMoves; move++)
                    dEdgePermutationMoveTable[dEdgePermutation, move] = -1;

            //populate table
            for (int dEdgePermutation = 0; dEdgePermutation < Coordinates.NumDEdgePermutations; dEdgePermutation++)
                for (int face = 0; face < NumFaces; face++)
                {
                    Coordinates.SetDEdgePermutation(cube, dEdgePermutation);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        dEdgePermutationMoveTable[dEdgePermutation, face * 3 + move] = (short)Coordinates.GetDEdgePermutation(cube);
                    }
                }

            return dEdgePermutationMoveTable;
        }

        /// <summary>
        /// Create a move table for U- and D-edge order. Only valid for cubes
        /// in the subgroup G1.
        /// </summary>
        /// <returns>
        /// A move table for U- and D-edge order.
        /// </returns>
        public static ushort[,] CreateUdEdgeOrderMoveTable()
        {
            ushort[,] udEdgeOrderMoveTable = new ushort[Coordinates.NumUdEdgeOrders, TwoPhaseConstants.NumMovesPhase2];

            //invalidate table
            for (int udEdgeOrder = 0; udEdgeOrder < Coordinates.NumUdEdgeOrders; udEdgeOrder++)
                for (int move = 0; move < TwoPhaseConstants.NumMovesPhase2; move++)
                    udEdgeOrderMoveTable[udEdgeOrder, move] = ushort.MaxValue;

            //populate table
            for (int udEdgeOrder = 0; udEdgeOrder < Coordinates.NumCornerPermutations; udEdgeOrder++)
                for (int face = 0; face < NumFaces; face++)
                {
                    CubieCube cube = CubieCube.CreateSolved();
                    Coordinates.SetUdEdgeOrder(cube, udEdgeOrder);
                    for (int move = 0; move < 3; move++)
                    {
                        cube.MultiplyEdges(CubieCube.MovesArray[face * 3]);
                        if (TwoPhaseConstants.Phase2Moves.Contains((Move)(face * 3 + move)))
                            udEdgeOrderMoveTable[udEdgeOrder, Phase1IndexToPhase2Index[face * 3 + move]] = (ushort)Coordinates.GetUdEdgeOrder(cube);
                    }
                }

            return udEdgeOrderMoveTable;
        }
    }
}
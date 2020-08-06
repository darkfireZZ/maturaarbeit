using static Cubing.ThreeByThree.Constants;
using System.Linq;
using System;

namespace Cubing.ThreeByThree.TwoPhase
{
    [System.Obsolete("Not optimized")]
    /// <summary>
    /// <see cref="SinglePhaseSolver"/> contains methods for solving each phase
    /// of the two-phase algorithm optimally. It uses the first solution found
    /// and is not optimized any further.
    /// </summary>
    public static class SinglePhaseSolver
    {
        /// <summary>
        /// Find a near optimal phase 1 solution for a <see cref="CubieCube"/>.
        /// </summary>
        /// <param name="cube">The cube to solve.</param>
        /// <param name="prune">Whether to use pruning.</param>
        /// <returns>
        /// A near optimal phase 1 solution for <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static Alg FindPhase1Solution(CubieCube cube, bool prune = true)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int eo = Coordinates.GetEoCoord(cube);
            int co = Coordinates.GetCoCoord(cube);
            int equatorDistribution = Coordinates.GetEquatorDistributionCoord(cube);

            return FindPhase1Solution(eo, co, equatorDistribution, prune);
        }

        /// <summary>
        /// Find a near optimal phase 1 solution for a cube using eo, co and
        /// equator coordinates.
        /// </summary>
        /// <param name="co">The edge orientation coordinate of the cube to solve.</param>
        /// <param name="eo">The corner orientation coordinate of the cube to solve.</param>
        /// <param name="equatorDistribution">The equator coordinate of the cube to solve.</param>
        /// <param name="prune">Whether to use pruning.</param>
        /// <returns>
        /// A near optimal phase 1 solution for the given coordinates.
        /// </returns>
        public static Alg FindPhase1Solution(int eo, int co, int equatorDistribution, bool prune = true)
        {
            TableController.InitializePhase1MoveTables();
            if (prune)
                TableController.InitializePhase1PruningTable();

            if (prune)
            {
                //find depth
                int pruningCoord = PruningTables.GetPhase1PruningIndex(co, eo, equatorDistribution);
                int requiredMoves = TableController.Phase1PruningTable[pruningCoord];

                int[] solutionArray = new int[requiredMoves];
                if (SearchPhase1(eo, co, equatorDistribution, 0, solutionArray, prune)) //if a solution is found
                    return Alg.FromEnumerable(solutionArray.Cast<Move>());
                else
                    return null;
            }
            else
            {
                for (int depth = 0; depth <= TwoPhaseConstants.MaxDepthPhase1; depth++)
                {
                    int[] solutionArray = new int[depth];
                    if (SearchPhase1(eo, co, equatorDistribution, 0, solutionArray, prune))
                        return Alg.FromEnumerable(solutionArray.Cast<Move>());
                }
                return null;
            }
        }

        private static bool SearchPhase1(int eo, int co, int equator, int depth, int[] solution, bool prune)
        {
            int length = solution.Length;
            int remainingMoves = length - depth;

            //return solution if solved
            if (remainingMoves == 0)
            {
                if (eo == 0 && co == 0 && equator == 0)
                    return true;
                else
                    return false;
            }

            //prune
            if (prune)
            {
                int pruningCoord = PruningTables.GetPhase1PruningIndex(co, eo, equator);
                if (TableController.Phase1PruningTable[pruningCoord] > remainingMoves)
                    return false;
            }

            //increase depth
            for (int move = 0; move < NumMoves; move++)
            {
                //TEST
                //TEST improvement
                //improve performance by preventing two consecutive moves on the same face
                if (depth >= 1)
                {
                    int faceOfMove = move / 3;
                    int faceOfLastMove = solution[depth - 1] / 3;

                    bool moveIsOnTheSameFaceAsLastMove = faceOfMove == faceOfLastMove;

                    if (moveIsOnTheSameFaceAsLastMove)
                        continue;

                    //improve performance by preventing three consecutive moves on the same axis
                    if (depth >= 2)
                    {
                        int axisOfMove = faceOfMove % 3;
                        int axisOfLastMove = faceOfLastMove % 3;
                        int axisOfMoveBeforeLastMove = (solution[depth - 2] / 3) % 3;

                        bool moveIsOnTheSameAxisAsLastTwoMoves = (axisOfMove == axisOfLastMove) && (axisOfMove == axisOfMoveBeforeLastMove);

                        if (moveIsOnTheSameAxisAsLastTwoMoves)
                            continue;
                    }
                }

                solution[depth] = move;

                int newEo = TableController.EoMoveTable[eo, move];
                int newCo = TableController.CoMoveTable[co, move];
                int newEquator = TableController.EquatorDistributionMoveTable[equator, move];
                if (SearchPhase1(newEo, newCo, newEquator, depth + 1, solution, prune))
                    return true;
            }

            //if no solution is found
            return false;
        }

        //TODO finish
        /// <summary>
        /// Find a near optimal phase 2 solution for a <see cref="CubieCube"/>.
        /// </summary>
        /// <param name="cube">The cube to solve.</param>
        /// <param name="prune">Whether to use pruning.</param>
        /// <returns>
        /// A near optimal phase 2 solution for <paramref name="cube"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cube"/> is null.
        /// </exception>
        public static Alg FindPhase2Solution(CubieCube cube, bool prune = true)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            int cp = Coordinates.GetCpCoord(cube);
            int equatorPermutation = Coordinates.GetEquatorPermutationCoord(cube);
            int udEdgePermutation = Coordinates.GetUdEdgePermutationCoord(cube);

            return FindPhase2Solution(cp, equatorPermutation, udEdgePermutation, prune);
        }

        //TODO improve documentation
        /// <summary>
        /// Find a near optimal phase 2 solution for a cube using cp, ... and ... coordinates
        /// </summary>
        /// <param name="cp">The corner permutation coordinate of the cube to solve.</param>
        /// <param name="prune">Whether to use pruning.</param>
        /// <returns>
        /// A near optimal phase 2 solution for the given coordinates.
        /// </returns>
        public static Alg FindPhase2Solution(int cp, int equatorPermutation, int udEdgePermutation, bool prune = true)
        {
            TableController.InitializePhase2MoveTables();
            if (prune)
                TableController.InitializePhase2CornerEquatorPruningTable();

            int depth = 0;
            if (prune)
            {
                int index = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                depth = TableController.Phase2CornerEquatorPruningTable[index];
            }

            for (; depth <= TwoPhaseConstants.MaxDepthPhase2; depth++)
            {
                int[] solutionArray = new int[depth];
                if (SearchPhase2(cp, equatorPermutation, udEdgePermutation, 0, solutionArray, prune))
                    return Alg.FromEnumerable(solutionArray.Cast<Move>());
            }
            return null;
        }

        private static bool SearchPhase2(int cp, int equatorPermutation, int udEdgePermutation, int depth, int[] solution, bool prune = true)
        {
            int length = solution.Length;
            int remainingMoves = length - depth;

            //return solution if solved
            if (remainingMoves == 0)
            {
                if (cp == 0 && udEdgePermutation == 0)
                    return true;
                else
                    return false;
            }

            if (prune)
            {
                int pruningIndex = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                if (TableController.Phase2CornerEquatorPruningTable[pruningIndex] > remainingMoves)
                    return false;
            }

            //increase depth
            foreach (int move in TwoPhaseConstants.Phase2Moves)
            {
                //TODO understand
                /*if (remainingMoves < NoPhase2MovesThreshold && TwoPhaseConstants.Phase2Moves.Contains(move))
                    continue;*/

                //TEST
                //TEST improvement
                //improve performance by preventing two consecutive moves on the same face
                if (depth >= 1)
                {
                    int faceOfMove = move / 3;
                    int faceOfLastMove = solution[depth - 1] / 3;

                    bool moveIsOnTheSameFaceAsLastMove = faceOfMove == faceOfLastMove;

                    if (moveIsOnTheSameFaceAsLastMove)
                        continue;

                    //improve performance by preventing three consecutive moves on the same axis
                    if (depth >= 2)
                    {
                        int axisOfMove = faceOfMove % 3;
                        int axisOfLastMove = faceOfLastMove % 3;
                        int axisOfMoveBeforeLastMove = (solution[depth - 2] / 3) % 3;

                        bool moveIsOnTheSameAxisAsLastTwoMoves = (axisOfMove == axisOfLastMove) && (axisOfMove == axisOfMoveBeforeLastMove);

                        if (moveIsOnTheSameAxisAsLastTwoMoves)
                            continue;
                    }
                }

                solution[depth] = move;

                int newCp = TableController.CpMoveTable[cp, move];
                int newEquatorPermutation = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                int newUdEdgePermutation = TableController.UdEdgePermutationMoveTable[udEdgePermutation, MoveTables.Phase1IndexToPhase2Index[move]];
                if (SearchPhase2(newCp, newEquatorPermutation, newUdEdgePermutation, depth + 1, solution))
                    return true;
            }

            //if no solution is found
            return false;
        }

        /// <summary>
        /// Find a solution for a cube.
        /// </summary>
        /// <param name="cube">The cube to solve.</param>
        /// <returns>A solution to the given cube.</returns>
        public static Alg FindSolution(CubieCube cube)
        {
            if (cube is null)
                throw new ArgumentNullException(nameof(cube) + " is null.");

            Alg phase1Solution = FindPhase1Solution(cube);

            CubieCube phase1SolvedCube = cube.Clone();
            phase1SolvedCube.ApplyAlg(phase1Solution);

            Alg phase2Solution = FindPhase2Solution(phase1SolvedCube);

            return phase1Solution + phase2Solution;
        }
    }
}
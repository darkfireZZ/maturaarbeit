using static Cubing.ThreeByThree.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

//TODO implement inverse solving
namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// An implementation of Herbert Kociemba's two-phase algorithm.
    /// </summary>
    public class TwoPhaseSolver
    {
        private static object _lockObject = new object();

        /// <summary>
        /// The maximum allowed length for a phase 2 solution.
        /// </summary>
        public static int MaxAllowedPhase2Length = 10;

        private class SyncedIntWrapper
        {
            private int _value = default;

            public int Value
            {
                get
                {
                    lock (_lockObject)
                        return _value;
                }
                set
                {
                    lock (_lockObject)
                        _value = value;
                }
            }
        }

        private class SyncedBoolWrapper
        {
            private bool _value = default;

            public bool Value
            {
                get
                {
                    lock (_lockObject)
                        return _value;
                }
                set
                {
                    lock (_lockObject)
                        _value = value;
                }
            }
        }

        private const int SameFace = 0, SameAxisInWrongOrder = 3;

        private SyncedBoolWrapper _isTerminated = null;
        private SyncedIntWrapper _shortestSolutionLength = null;
        private SyncedIntWrapper _shortestSolutionIndex = null;
        private List<Alg> _solutions = null;
        private Stopwatch _timePassed = null;
        private TimeSpan _timeout = default;
        private int _returnLength = 20;
        private int _requiredLength = 30;

        private CubieCube _notRotatedCube = null;

        private int _cp = 0;
        private int _uEdges = 0;
        private int _dEdges = 0;

        private bool _solveInverse = false;
        private int _rotation = 0;

        private int[] _currentPhase1Solution = null;
        private int[] _currentPhase2Solution = null;

        //TEST with impossible length
        /// <summary>
        /// Find a solution for a <see cref="CubieCube"/> using six concurrent
        /// threads.
        /// </summary>
        /// <param name="cubeToSolve">The cube to solve.</param>
        /// <param name="timeout">
        /// Interrupt the search after this number of milliseconds.
        /// </param>
        /// <param name="returnLength">
        /// The search stops as soon as a solution of the specified length is
        /// found. Be careful with setting <paramref name="returnLength"/> to a
        /// value smaller than 20. You are allowed to do this, though there
        /// might be no solution with your length. In that case the program
        /// will try to find a solution which matches the given length. This
        /// may take a VERY long time.
        /// </param>
        /// <param name="requiredLength">
        /// Ignore the timeout until a solution of a length smaller than or equal to
        /// <paramref name="requiredLength"/> is found. If the value of
        /// <paramref name="requiredLength"/> is negative, this parameter is ignored
        /// (default behaviour). If <paramref name="requiredLength"/> is not
        /// negative it must be larger than or equal to
        /// <paramref name="returnLength"/>. Setting the value of
        /// <paramref name="requiredLength"/> avoids returning null. Setting the
        /// value to 30 or higher will return the first solution found after
        /// timeout.
        /// </param>
        /// <param name="solveDifferentOrientations">
        /// Start 3 thread each solving a 120° offset of the cube.
        /// </param>
        /// <param name="solveInverse">
        /// Start another thread for every orientation which solves the inverse.
        /// </param>
        /// <returns>
        /// A solution for the specified cube or null if no solution is found
        /// for the given parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cubeToSolve"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="returnLength"/> is negative or if
        /// <paramref name="requiredLength"/> is positive but smaller than
        /// <paramref name="returnLength"/>.
        /// </exception>
        public static Alg FindSolution(CubieCube cubeToSolve, TimeSpan timeout, int returnLength, int requiredLength = 30, bool solveDifferentOrientations = true, bool solveInverse = true)
        {
            if (cubeToSolve is null)
                throw new ArgumentNullException(nameof(cubeToSolve) + " is null.");
            if (returnLength < 0)
                throw new ArgumentOutOfRangeException(nameof(returnLength) + " cannot be negative: " + returnLength);
            if (requiredLength > 0 && requiredLength < returnLength)
                throw new ArgumentOutOfRangeException(nameof(requiredLength) + " must either be negative or >= " + nameof(returnLength) + ": " + requiredLength + " (" + nameof(requiredLength) + ") < " + returnLength + " (" + nameof(returnLength) + ")");

            TableController.InitializePhase1Tables();
            TableController.InitializePhase2Tables();

            Stopwatch timePassed = new Stopwatch();
            timePassed.Start();

            SyncedBoolWrapper isTerminated = new SyncedBoolWrapper() { Value = false };
            SyncedIntWrapper shortestSolutionLength = new SyncedIntWrapper() { Value = 30 };
            SyncedIntWrapper shortestSolutionIndex = new SyncedIntWrapper() { Value = -1 };
            List<Alg> solutions = new List<Alg>();

            int numThreads = (solveDifferentOrientations ? 3 : 1) * (solveInverse ? 2 : 1);
            int increment = solveDifferentOrientations ? 1 : 2;
            
            Thread[] solvers = new Thread[numThreads];

            for (int orientation = 0; orientation < numThreads; orientation += increment)
            {
                TwoPhaseSolver solver = new TwoPhaseSolver()
                {
                    _notRotatedCube = cubeToSolve,
                    _timePassed = timePassed,
                    _timeout = timeout,
                    _returnLength = returnLength,
                    _requiredLength = requiredLength,
                    _isTerminated = isTerminated,
                    _solutions = solutions,
                    _shortestSolutionLength = shortestSolutionLength,
                    _shortestSolutionIndex = shortestSolutionIndex,
                    _rotation = orientation % 3,
                    _solveInverse = orientation / 3 == 1
                };

                Thread solverThread = new Thread(new ThreadStart(solver.StartSearch));
                solvers[orientation] = solverThread;
                solverThread.Start();
            }

            foreach (Thread solverThread in solvers)
                solverThread.Join();

            timePassed.Stop();

            if (solutions.Count > 0)
                return solutions[shortestSolutionIndex.Value];
            else
                return null;
        }

        private TwoPhaseSolver() { }

        private void StartSearch()
        {
            CubieCube rotatedCube = CubieCube.CreateSolved();

            for (int i = 0; i < _rotation; i++)
            {
                rotatedCube.Rotate(Rotation.y3);
                rotatedCube.Rotate(Rotation.x3);
            }

            rotatedCube.Multiply(_notRotatedCube);

            for (int i = 0; i < _rotation; i++)
            {
                rotatedCube.Rotate(Rotation.x1);
                rotatedCube.Rotate(Rotation.y1);
            }

            if (_solveInverse)
                rotatedCube.Inverse();

            //calculate coordinates
            int co = Coordinates.GetCoCoord(rotatedCube);
            int cp = Coordinates.GetCpCoord(rotatedCube);
            int eo = Coordinates.GetEoCoord(rotatedCube);
            int equator = Coordinates.GetEquatorCoord(rotatedCube);
            int uEdges = Coordinates.GetUEdgeCoord(rotatedCube);
            int dEdges = Coordinates.GetDEdgeCoord(rotatedCube);

            //store coordinates used in phase 2
            _cp = cp;
            _uEdges = uEdges;
            _dEdges = dEdges;

            int pruningIndex = Coordinates.GetPhase1PruningIndex(co, eo, equator / Coordinates.NumEquatorPermutationCoords);
            int minPhase1Length = TableController.Phase1PruningTable[pruningIndex];
            int maxPhase1Length = (_requiredLength > 0 && _requiredLength < GodsNumber) ? _requiredLength : GodsNumber;

            _currentPhase1Solution = new int[maxPhase1Length];
            _currentPhase2Solution = new int[MaxAllowedPhase2Length];

            for (int phase1Length = minPhase1Length; phase1Length < maxPhase1Length; phase1Length++)
                SearchPhase1(eo, co, equator, 0, phase1Length, minPhase1Length);
        }

        private void SearchPhase1(int eo, int co, int equator, int depth, int remainingMoves, int previousPruningValue)
        {
            if (_isTerminated.Value)
                return;

            if (remainingMoves == 0) //check if solved
            {
                lock (_lockObject) //manage timeout
                    if (_timePassed.Elapsed > _timeout && (_requiredLength < 0 || (_solutions.Count > 0 && _shortestSolutionLength.Value <= _requiredLength)))
                        _isTerminated.Value = true;

                int cp = _cp;
                int uEdges = _uEdges;
                int dEdges = _dEdges;
                int equatorPermutation = equator;

                for (int moveIndex = 0; moveIndex < depth; moveIndex++)
                {
                    int move = _currentPhase1Solution[moveIndex];

                    cp = TableController.CpMoveTable[cp, move];
                    uEdges = TableController.UEdgesMoveTable[uEdges, move];
                    dEdges = TableController.DEdgesMoveTable[dEdges, move];
                }

                int udEdgePermutation = Coordinates.CombineUAndDEdgePermutation(uEdges, dEdges);

                int pruningIndex = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                int minMoves = TableController.Phase2PruningTable[pruningIndex];
                int maxMoves = Math.Min(_shortestSolutionLength.Value - depth - 1, MaxAllowedPhase2Length);

                for (int length = minMoves; length <= maxMoves; length++)
                    SearchPhase2(cp, equatorPermutation, udEdgePermutation, 0, length, phase1Length: depth);

                return;
            }

            //increase depth
            for (int move = 0; move < NumMoves; move++)
            {
                //If the cube is already in the subgroup H and there are less
                //than 5 moves left it is possible to stay in the subgroup only
                //if exclusivly phase 2 moves are used, which means that this
                //solution can also be generated in phase 2 and is therefore
                //skipped.
                if (previousPruningValue == 0 && remainingMoves < 5 && !TwoPhaseConstants.Phase2Moves.Contains((Move)move))
                    continue;

                //prevent two consecutive moves on the same face or two
                //consecutive moves on the same axis in the wrong order
                if (depth > 0)
                {
                    int relation = move / 3 - _currentPhase1Solution[depth - 1] / 3;
                    if (relation == SameFace || relation == SameAxisInWrongOrder)
                        continue;
                }

                int newEo = TableController.EoMoveTable[eo, move];
                int newCo = TableController.CoMoveTable[co, move];
                int newEquator = TableController.EquatorMoveTable[equator, move];

                //prune
                int pruningCoord = Coordinates.GetPhase1PruningIndex(newCo, newEo, newEquator / Coordinates.NumEquatorPermutationCoords);
                int pruningValue = TableController.Phase1PruningTable[pruningCoord];
                if (pruningValue > remainingMoves - 1)
                    continue;

                _currentPhase1Solution[depth] = move;
                SearchPhase1(newEo, newCo, newEquator, depth + 1, remainingMoves - 1, pruningValue);
            }
        }

        private void SearchPhase2(int cp, int equatorPermutation, int udEdgePermutation, int depth, int remainingMoves, int phase1Length)
        {
            if (_isTerminated.Value)
                return;

            if (remainingMoves == 0 && udEdgePermutation == 0) //check if solved
            {
                Alg solution = Alg.FromEnumerable(_currentPhase1Solution.Take(phase1Length).Concat(_currentPhase2Solution.Take(depth)).Cast<Move>());

                for (int i = 0; i < _rotation; i++)
                    solution = solution.Rotate(Rotation.y3).Rotate(Rotation.x3);

                if (_solveInverse)
                {
                    solution = solution.Inverse();
                }

                lock (_lockObject)
                {
                    if (solution.Length < _shortestSolutionLength.Value)
                    {
                        _shortestSolutionIndex.Value = _solutions.Count;
                        _shortestSolutionLength.Value = solution.Length;
                    }

                    _solutions.Add(solution);
                }

                if (solution.Length <= _returnLength)
                    _isTerminated.Value = true;

                return;
            }

            //increase depth
            foreach (int move in TwoPhaseConstants.Phase2Moves)
            {
                //prevent two consecutive moves on the same face or two
                //consecutive moves on the same axis in the wrong order
                if (depth == 0)
                {
                    if (phase1Length > 0)
                    {
                        int relation = move / 3 - _currentPhase1Solution[phase1Length - 1] / 3;
                        if (relation == SameFace || relation == SameAxisInWrongOrder)
                            continue;
                    }
                }
                else
                {
                    int relation = move / 3 - _currentPhase2Solution[depth - 1] / 3;
                    if (relation == SameFace || relation == SameAxisInWrongOrder)
                        continue;
                }

                int newCp = TableController.CpMoveTable[cp, move];
                int newEquatorPermutation = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                int newUdEdgePermutation = TableController.UdEdgePermutationMoveTable[udEdgePermutation, move];

                //prune
                int pruningIndex = TwoPhaseConstants.NumEquatorPermutations * newCp + newEquatorPermutation;
                if (TableController.Phase2PruningTable[pruningIndex] > remainingMoves - 1)
                    continue;

                _currentPhase2Solution[depth] = move;
                SearchPhase2(newCp, newEquatorPermutation, newUdEdgePermutation, depth + 1, remainingMoves - 1, phase1Length);
            }
        }
    }
}
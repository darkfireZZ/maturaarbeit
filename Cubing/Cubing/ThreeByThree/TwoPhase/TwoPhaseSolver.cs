using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// An implementation of Herbert Kociemba's two-phase algorithm.
    /// </summary>
    public class TwoPhaseSolver
    {
        private static readonly object _lockObject = new object();

        /// <summary>
        /// The maximum allowed length for a phase 2 solution.
        /// </summary>
        public static int MaxPhase2Length { get; set; } = 10;

        private const int SameFace = 0, SameAxisInWrongOrder = 3;

        /// <summary>
        /// Interrupts the search if set to true.
        /// </summary>
        public SyncedBoolWrapper IsTerminated { get; set; }
        private SyncedIntWrapper _shortestSolutionLength;
        private SyncedIntWrapper _shortestSolutionIndex;
        private List<Alg> _solutions;
        private Stopwatch _timePassed;
        private TimeSpan _timeout;
        private int _returnLength;
        private int _requiredLength;

        private CubieCube _notRotatedCube;

        private int _cp;
        private int _uEdges;
        private int _dEdges;

        private bool _inversed;
        private int _rotation;

        private int[] _currentPhase1Solution;
        private int[] _currentPhase2Solution;

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
        /// <param name="searchDifferentOrientations">
        /// Start 3 thread each solving a 120° offset of the cube.
        /// </param>
        /// <param name="searchInverse">
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
        public static Alg FindSolution(CubieCube cubeToSolve, TimeSpan timeout, int returnLength, int requiredLength, bool searchDifferentOrientations = true, bool searchInverse = true)
        {
            if (cubeToSolve is null)
                throw new ArgumentNullException(nameof(cubeToSolve) + " is null.");
            if (returnLength < 0)
                throw new ArgumentOutOfRangeException(nameof(returnLength) + " cannot be negative: " + returnLength);
            if ((uint)requiredLength < returnLength)
                throw new ArgumentOutOfRangeException(nameof(requiredLength) + " must either be negative or >= " + nameof(returnLength) + ": " + requiredLength + " (" + nameof(requiredLength) + ") < " + returnLength + " (" + nameof(returnLength) + ")");

            TableController.InitializePhase1Tables();
            TableController.InitializePhase2Tables();

            Stopwatch timePassed = new Stopwatch();
            timePassed.Start();

            SyncedBoolWrapper isTerminated = new SyncedBoolWrapper() { Value = false };
            SyncedIntWrapper shortestSolutionLength = new SyncedIntWrapper() { Value = 30 };
            SyncedIntWrapper shortestSolutionIndex = new SyncedIntWrapper() { Value = -1 };
            List<Alg> solutions = new List<Alg>();

            int numThreads = (searchDifferentOrientations ? 3 : 1) * (searchInverse ? 2 : 1);
            int increment = searchDifferentOrientations ? 1 : 2;
            
            Thread[] solvers = new Thread[numThreads];

            for (int orientation = 0; orientation < numThreads; orientation += increment)
            {
                TwoPhaseSolver solver = new TwoPhaseSolver()
                {
                    _notRotatedCube = cubeToSolve.Clone(), //create a copy to avoid issues caused by multithreading
                    _timePassed = timePassed,
                    _timeout = timeout,
                    _returnLength = returnLength,
                    _requiredLength = requiredLength,
                    IsTerminated = isTerminated,
                    _solutions = solutions,
                    _shortestSolutionLength = shortestSolutionLength,
                    _shortestSolutionIndex = shortestSolutionIndex,
                    _rotation = orientation % 3,
                    _inversed = orientation / 3 == 1
                };

                Thread solverThread = new Thread(new ThreadStart(solver.StartSearch));
                solvers[orientation] = solverThread;
                solverThread.Start();
            }

            foreach (Thread solverThread in solvers)
                solverThread.Join();

            timePassed.Stop(); //not required, only for the sake of completeness

            if (solutions.Count > 0)
                return solutions[shortestSolutionIndex.Value];
            else
                return null;
        }

        private TwoPhaseSolver() { }

        private void StartSearch()
        {
            #region rotate cube
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

            if (_inversed)
                rotatedCube.Inverse();
            #endregion rotate cube

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

            int pruningIndex = PruningTables.GetPhase1PruningIndex(co, eo, equator / Coordinates.NumEquatorPermutationCoords);
            int minPhase1Length = TableController.Phase1PruningTable[pruningIndex];
            int maxPhase1Length = (_requiredLength > 0 && _requiredLength < GodsNumber) ? _requiredLength : GodsNumber;

            _currentPhase1Solution = new int[maxPhase1Length];
            _currentPhase2Solution = new int[MaxPhase2Length];

            for (int phase1Length = minPhase1Length; phase1Length < maxPhase1Length; phase1Length++)
                SearchPhase1(eo, co, equator, depth: 0, remainingMoves: phase1Length, minPhase1Length);
        }

        private void SearchPhase1(int eo, int co, int equator, int depth, int remainingMoves, int previousPruningValue)
        {
            if (IsTerminated.Value)
                return;

            if (remainingMoves == 0) //check if solved
            {
                lock (_lockObject) //manage timeout
                    if (_timePassed.Elapsed > _timeout && (_requiredLength < 0 || (_solutions.Count > 0 && _shortestSolutionLength.Value <= _requiredLength)))
                        IsTerminated.Value = true;

                int cp = _cp;
                int uEdges = _uEdges;
                int dEdges = _dEdges;
                int equatorPermutation = equator;

                //TEST improvement
                for (int moveIndex = 0; moveIndex < depth; moveIndex++)
                {
                    int move = _currentPhase1Solution[moveIndex];

                    cp = TableController.CpMoveTable[cp, move];
                }

                int cornerEquatorPruningIndex = TwoPhaseConstants.NumEquatorPermutations * cp + equator;
                int cornerEquatorPruningValue = TableController.Phase2CornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (cornerEquatorPruningValue > MaxPhase2Length)
                    return;

                for (int moveIndex = 0; moveIndex < depth; moveIndex++)
                {
                    int move = _currentPhase1Solution[moveIndex];

                    uEdges = TableController.UEdgesMoveTable[uEdges, move];
                    dEdges = TableController.DEdgesMoveTable[dEdges, move];
                }

                int udEdgePermutation = Coordinates.CombineUAndDEdgePermutation(uEdges, dEdges);

                //prune
                int cornerUdPruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(udEdgePermutation, cp);
                int minMoves = TableController.Phase2CornerUdPruningTable[cornerUdPruningIndex];
                int maxMoves = Math.Min(_shortestSolutionLength.Value - depth - 1, MaxPhase2Length);

                for (int length = minMoves; length <= maxMoves; length++)
                    SearchPhase2(cp, equatorPermutation, udEdgePermutation, depth: 0, remainingMoves: length, phase1Length: depth);

                return;
            }

            //increase depth
            for (int move = 0; move < NumMoves; move++)
            {
                //If the cube is already in the subgroup H and there are less
                //than 5 moves left it is only possible to stay in the subgroup
                //if exclusivly phase 2 moves are used, which means that this
                //solution can also be generated in phase 2.
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
                int pruningCoord = PruningTables.GetPhase1PruningIndex(newCo, newEo, newEquator / Coordinates.NumEquatorPermutationCoords);
                int pruningValue = TableController.Phase1PruningTable[pruningCoord];
                if (pruningValue > remainingMoves - 1)
                    continue;

                _currentPhase1Solution[depth] = move;
                SearchPhase1(newEo, newCo, newEquator, depth + 1, remainingMoves - 1, pruningValue);
            }
        }

        private void SearchPhase2(int cp, int equatorPermutation, int udEdgePermutation, int depth, int remainingMoves, int phase1Length)
        {
            if (IsTerminated.Value)
                return;

            if (remainingMoves == 0 && equatorPermutation == 0) //check if solved
            {
                Alg solution = Alg.FromEnumerable(_currentPhase1Solution.Take(phase1Length).Concat(_currentPhase2Solution.Take(depth)).Cast<Move>());

                for (int i = 0; i < _rotation; i++)
                    solution = solution.Rotate(Rotation.y3).Rotate(Rotation.x3);

                if (_inversed)
                    solution = solution.Inverse();

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
                    IsTerminated.Value = true;

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
                int newUdEdgePermutation = TableController.UdEdgePermutationMoveTable[udEdgePermutation, MoveTables.Phase1IndexToPhase2Index[move]];

                //prune
                int cornerUdPruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(newUdEdgePermutation, newCp);
                int cornerUdPruningValue = TableController.Phase2CornerUdPruningTable[cornerUdPruningIndex];
                int cornerEquatorPruningIndex = TwoPhaseConstants.NumEquatorPermutations * newCp + newEquatorPermutation;
                int cornerEquatorPruningValue = TableController.Phase2CornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (Math.Max(cornerUdPruningValue, cornerEquatorPruningValue) > remainingMoves - 1)
                    continue;

                _currentPhase2Solution[depth] = move;
                SearchPhase2(newCp, newEquatorPermutation, newUdEdgePermutation, depth + 1, remainingMoves - 1, phase1Length);
            }
        }
    }
}
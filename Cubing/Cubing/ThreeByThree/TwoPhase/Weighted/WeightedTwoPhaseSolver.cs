using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static Cubing.ThreeByThree.Constants;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// A weighted adaptation of Herbert Kociemba's two-phase algorithm.
    /// </summary>
    public class WeightedTwoPhaseSolver
    {
        private static readonly object _lockObject = new object();

        /// <summary>
        /// The maximum allowed length for a phase 2 solution.
        /// </summary>
        public static int MaxPhase2Length { get; set; } = 10;
        /// <summary>
        /// The maximum allowed length for a phase 1 solution.
        /// </summary>
        public static int MaxPhase1Length { get; set; } = 25;

        private const int SameFace = 0, SameAxisInWrongOrder = 3;

        private float[] _nonRotatedWeights;
        private float[] _rotatedWeights;
        private IEnumerable<Move> _phase1MoveOrder;
        private IEnumerable<Move> _phase2MoveOrder;
        private float[] _weightedCornerEquatorPruningTable;

        /// <summary>
        /// Interrupts the search when set to true.
        /// </summary>
        public SyncedBoolWrapper IsTerminated;

        private SyncedFloatWrapper _bestSolutionCost;
        private SyncedIntWrapper _bestSolutionIndex;
        private List<Alg> _solutions;
        private Stopwatch _timePassed;
        private TimeSpan _timeout;
        private float _returnCost;
        private float _requiredCost;
        private float _deltaMin;

        private CubieCube _notRotatedCube;

        private int _cp;
        private int _uEdges;
        private int _dEdges;

        private bool _inversed;
        private int _rotation;

        private int[] _currentPhase1Solution;
        private int[] _currentPhase2Solution;

        /// <summary>
        /// Find a near-optimal solution for a cube in respect to the cost of
        /// the moves. If no matching solution is found, the return value is
        /// null.
        /// </summary>
        /// <param name="cubeToSolve">The cube to solve.</param>
        /// <param name="timeout">
        /// Interrupt the search after that much time has passed.
        /// </param>
        /// <param name="returnCost">
        /// The search stops as soon as a solution of the specified cost is
        /// found. Be careful with setting <paramref name="returnCost"/> to a
        /// value too small, because there might not be a solution cheaper than
        /// or matching that cost. In which case the algorithm will run for a
        /// long time.
        /// </param>
        /// <param name="requiredCost">
        /// Ignore the timeout until a solution of a cost less than or equal to
        /// <paramref name="requiredCost"/> is found. If the value of
        /// <paramref name="requiredCost"/> is negative, this parameter is
        /// ignored. If <paramref name="requiredCost"/> is positive, it must be
        /// larger than or equal to <paramref name="returnCost"/>. Setting the
        /// value to a large value will cause the first solution found after
        /// timeout to be returned.
        /// </param>
        /// <param name="weights">The weights to use.</param>
        /// <param name="weightedCornerEquatorPruningTable">
        /// The weighted corner permutation and equator order pruning table
        /// created by
        /// <see cref="WeightedPruningTables.CreateWeightedPhase2CornerEquatorTable(float[])"/>.
        /// The weights used to create this table must be equatl to
        /// <paramref name="weights"/>.
        /// </param>
        /// <param name="deltaMin">
        /// The minimum difference a newly found solution has to exceed the
        /// cheapest solution found. Only used to experiment. Set to 0 for
        /// optimal solutions.
        /// </param>
        /// <param name="searchDifferentOrientations">
        /// Start three threads each solving a 120° offset of the cube.
        /// </param>
        /// <param name="searchInverse">
        /// For every orientation solved, start another thread that solves its
        /// inverse.
        /// </param>
        /// <returns>
        /// A near-optimal solution for the specified cube in respect to its
        /// cost. Null, if no matching solution is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cubeToSolve"/> or
        /// <paramref name="weightedCornerEquatorPruningTable"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="timeout"/>,
        /// <paramref name="returnCost"/> or <paramref name="deltaMin"/> is
        /// negative. Also thrown if <paramref name="requiredCost"/> is
        /// positive and less than <paramref name="returnCost"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="weightedCornerEquatorPruningTable"/>.Length
        /// is not of the expected size.
        /// </exception>
        /// <exception cref="InvalidWeightsException">
        /// Thrown if <paramref name="weights"/> is invalid.
        /// </exception>
        public static Alg FindSolution(CubieCube cubeToSolve, TimeSpan timeout, float returnCost, float requiredCost, float[] weights, float[] weightedCornerEquatorPruningTable, float deltaMin = 0f, bool searchDifferentOrientations = true, bool searchInverse = true)
        {
            #region parameter checks
            if (cubeToSolve is null)
                throw new ArgumentNullException(nameof(cubeToSolve) + " is null.");
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout) + " cannot be negative: " + timeout);
            if (returnCost < 0d)
                throw new ArgumentOutOfRangeException(nameof(returnCost) + " cannot be negative: " + returnCost);
            if (requiredCost > 0d && requiredCost < returnCost)
                throw new ArgumentOutOfRangeException(nameof(requiredCost) + " must either be negative or >= " + nameof(returnCost) + ": " + requiredCost + " (" + nameof(requiredCost) + ") < " + returnCost + " (" + nameof(returnCost) + ")");
            MoveWeightsUtils.ValidateWeights(weights);
            if (weightedCornerEquatorPruningTable is null)
                throw new ArgumentNullException(nameof(weightedCornerEquatorPruningTable) + " is null.");
            if (weightedCornerEquatorPruningTable.Length != PruningTableConstants.CornerEquatorPruningTableSizePhase2)
                throw new ArgumentException(nameof(weightedCornerEquatorPruningTable) + " must be of size " + PruningTableConstants.CornerEquatorPruningTableSizePhase2);
            if (deltaMin < 0d)
                throw new ArgumentOutOfRangeException(nameof(deltaMin) + " cannot be negative: " + deltaMin);
            #endregion paramter checks

            //initialize move tables
            TableController.InitializeCornerOrientationMoveTable();
            TableController.InitializeCornerPermutationMoveTable();
            TableController.InitializeDEdgePermutationMoveTable();
            TableController.InitializeEdgeOrientationMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();
            TableController.InitializeUdEdgeOrderMoveTable();
            TableController.InitializeUEdgePermutationMoveTable();

            //initialize pruning tables
            TableController.InitializePhase1PruningTable();
            TableController.InitializePhase2CornerEquatorPruningTable();
            TableController.InitializePhase2CornerUdPruningTable();

            Stopwatch timePassed = new Stopwatch();
            timePassed.Start();

            SyncedBoolWrapper isTerminated = new SyncedBoolWrapper() { Value = false };
            SyncedFloatWrapper bestSolutionCost = new SyncedFloatWrapper() { Value = float.MaxValue };
            SyncedIntWrapper bestSolutionIndex = new SyncedIntWrapper() { Value = -1 };
            List<Alg> solutions = new List<Alg>();

            int numThreads = (searchDifferentOrientations ? 3 : 1) * (searchInverse ? 2 : 1);
            int increment = searchDifferentOrientations ? 1 : 2;

            Thread[] solvers = new Thread[numThreads];

            for (int orientation = 0; orientation < numThreads; orientation += increment)
            {
                WeightedTwoPhaseSolver solver = new WeightedTwoPhaseSolver()
                {
                    _notRotatedCube = cubeToSolve.Clone(), //create a copy to avoid issues caused by multithreading
                    _timePassed = timePassed,
                    _timeout = timeout,
                    _returnCost = returnCost,
                    _requiredCost = requiredCost,
                    IsTerminated = isTerminated,
                    _solutions = solutions,
                    _bestSolutionCost = bestSolutionCost,
                    _bestSolutionIndex = bestSolutionIndex,
                    _rotation = orientation % 3,
                    _inversed = orientation / 3 == 1,
                    _nonRotatedWeights = (float[])weights.Clone(), //create a copy to avoid issues caused by multithreading
                    _weightedCornerEquatorPruningTable = weightedCornerEquatorPruningTable,
                    _deltaMin = deltaMin
                };

                Thread solverThread = new Thread(new ThreadStart(solver.StartSearch));
                solvers[orientation] = solverThread;
                solverThread.Start();
            }

            foreach (Thread solverThread in solvers)
                solverThread.Join();

            timePassed.Stop(); //not required, only for the sake of completeness

            if (solutions.Count > 0)
                return solutions[bestSolutionIndex.Value];
            else
                return null;
        }

        private WeightedTwoPhaseSolver() { }

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

            #region rotate weights
            _rotatedWeights = new float[NumMoves];

            for (int oldIndex = 0; oldIndex < NumMoves; oldIndex++)
            {
                int newIndex = oldIndex;
                for (int i = 0; i < _rotation; i++)
                    newIndex = (int)((Move)newIndex).Rotate(Rotation.x1).Rotate(Rotation.y1);
                _rotatedWeights[newIndex] = _nonRotatedWeights[oldIndex];
            }

            if (_inversed)
            {
                for (int face = 0; face < NumFaces; face++)
                {
                    //face * 3 = 90° cw, face * 3 + 2 = 90° ccw
                    float temp = _rotatedWeights[face * 3];
                    _rotatedWeights[face * 3] = _rotatedWeights[face * 3 + 2];
                    _rotatedWeights[face * 3 + 2] = temp;
                }
            }
            #endregion rotate weights

            _phase1MoveOrder = MoveWeightsUtils.OrderedMoves((Move[])Enum.GetValues(typeof(Move)), _rotatedWeights);
            _phase2MoveOrder = MoveWeightsUtils.OrderedMoves(TwoPhaseConstants.Phase2Moves, _rotatedWeights);

            //calculate coordinates
            int co = Coordinates.GetCornerOrientation(rotatedCube);
            int cp = Coordinates.GetCornerPermutation(rotatedCube);
            int eo = Coordinates.GetEdgeOrientation(rotatedCube);
            int equator = Coordinates.GetEquatorPermutation(rotatedCube);
            int uEdges = Coordinates.GetUEdgePermutation(rotatedCube);
            int dEdges = Coordinates.GetDEdgePermutation(rotatedCube);

            //store coordinates used in phase 2
            _cp = cp;
            _uEdges = uEdges;
            _dEdges = dEdges;

            int pruningIndex = PruningTables.GetPhase1PruningIndex(co, eo, equator / Coordinates.NumEquatorOrders);
            int minPhase1Length = TableController.Phase1PruningTable[pruningIndex];

            _currentPhase1Solution = new int[MaxPhase1Length];
            _currentPhase2Solution = new int[MaxPhase2Length];

            for (int phase1Length = minPhase1Length; phase1Length < MaxPhase1Length; phase1Length++)
                SearchPhase1(eo, co, equator, depth: 0, remainingMoves: phase1Length, minPhase1Length);
        }

        private void SearchPhase1(int eo, int co, int equator, int depth, int remainingMoves, int previousPruningValue)
        {
            if (IsTerminated.Value)
                return;

            if (remainingMoves == 0) //check if solved
            {
                lock (_lockObject) //manage timeout
                    if (_timePassed.Elapsed > _timeout && (_requiredCost < 0d || (_solutions.Count > 0 && _bestSolutionCost.Value <= _requiredCost)))
                        IsTerminated.Value = true;

                int cp = _cp;
                int uEdges = _uEdges;
                int dEdges = _dEdges;
                int equatorPermutation = equator;

                float phase1Cost = 0f;

                for (int moveIndex = 0; moveIndex < depth; moveIndex++)
                {
                    int move = _currentPhase1Solution[moveIndex];

                    phase1Cost += _rotatedWeights[moveIndex];
                    cp = TableController.CornerPermutationMoveTable[cp, move];
                    uEdges = TableController.UEdgePermutationMoveTable[uEdges, move];
                    dEdges = TableController.DEdgePermutationMoveTable[dEdges, move];
                }

                int cornerEquatorPruningIndex = Coordinates.NumEquatorOrders * cp + equator;
                double weightedCornerEquatorPruningValue = _weightedCornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (weightedCornerEquatorPruningValue + phase1Cost + _deltaMin > _bestSolutionCost.Value)
                    return;
                int cornerEquatorPruningValue = TableController.Phase2CornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (cornerEquatorPruningValue > MaxPhase2Length)
                    return;

                int udEdgeOrder = Coordinates.CombineUEdgePermutationAndDEdgeOrder(uEdges, dEdges % Coordinates.NumDEdgeOrders);

                int cornerUdPruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(udEdgeOrder, cp);
                int minMoves = TableController.Phase2CornerUdPruningTable[cornerUdPruningIndex];

                for (int length = minMoves; length <= MaxPhase2Length; length++)
                    SearchPhase2(cp, equatorPermutation, udEdgeOrder, depth: 0, remainingMoves: length, phase1Cost, phase1Length: depth);

                return;
            }

            foreach (int move in _phase1MoveOrder)
            {
                //If the cube is already in the subgroup G1 and there are less
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

                int newEo = TableController.EdgeOrientationMoveTable[eo, move];
                int newCo = TableController.CornerOrientationMoveTable[co, move];
                int newEquator = TableController.EquatorPermutationMoveTable[equator, move];

                //prune
                int pruningCoord = PruningTables.GetPhase1PruningIndex(newCo, newEo, newEquator / Coordinates.NumEquatorOrders);
                int pruningValue = TableController.Phase1PruningTable[pruningCoord];
                if (pruningValue > remainingMoves - 1)
                    continue;

                _currentPhase1Solution[depth] = move;
                SearchPhase1(newEo, newCo, newEquator, depth + 1, remainingMoves - 1, pruningValue);
            }
        }

        private void SearchPhase2(int cp, int equatorPermutation, int udEdgeOrder, int depth, int remainingMoves, float costSoFar, int phase1Length)
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

                float solutionCost = solution.Select(move => _nonRotatedWeights[(int)move])
                                              .Sum();

                lock (_lockObject)
                {
                    if (solutionCost < _bestSolutionCost.Value)
                    {
                        _bestSolutionIndex.Value = _solutions.Count;
                        _bestSolutionCost.Value = solutionCost;
                    }

                    _solutions.Add(solution);
                }

                if (solutionCost <= _returnCost)
                    IsTerminated.Value = true;

                return;
            }

            //increase depth
            foreach (int move in _phase2MoveOrder)
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

                int newCp = TableController.CornerPermutationMoveTable[cp, move];
                int newEquatorPermutation = TableController.EquatorOrderMoveTable[equatorPermutation, move];
                int newUdEdgePermutation = TableController.UdEdgeOrderMoveTable[udEdgeOrder, MoveTables.Phase1IndexToPhase2Index[move]];

                //prune
                int cornerUdPruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(newUdEdgePermutation, newCp);
                int cornerUdPruningValue = TableController.Phase2CornerUdPruningTable[cornerUdPruningIndex];
                int cornerEquatorPruningIndex = Coordinates.NumEquatorOrders * newCp + newEquatorPermutation;
                int cornerEquatorPruningValue = TableController.Phase2CornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (Math.Max(cornerUdPruningValue, cornerEquatorPruningValue) > remainingMoves - 1)
                    continue;

                float newCostSoFar = costSoFar + _rotatedWeights[move];
                float weightedCornerEquatorPruningValue = _weightedCornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (weightedCornerEquatorPruningValue + newCostSoFar + _deltaMin > _bestSolutionCost.Value)
                    continue;

                _currentPhase2Solution[depth] = move;
                SearchPhase2(newCp, newEquatorPermutation, newUdEdgePermutation, depth + 1, remainingMoves - 1, costSoFar, phase1Length);
            }
        }
    }
}
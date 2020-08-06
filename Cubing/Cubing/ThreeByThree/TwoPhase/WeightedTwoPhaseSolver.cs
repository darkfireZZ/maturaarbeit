using static Cubing.ThreeByThree.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// An implementation of Herbert Kociemba's two-phase algorithm.
    /// </summary>
    public class WeightedTwoPhaseSolver
    {
        private static readonly object _lockObject = new object();

        /// <summary>
        /// The maximum allowed length for a phase 2 solution.
        /// </summary>
        public static int MaxPhase2Length { get; set; } = 10;
        //TODO calculate
        /// <summary>
        /// The maximum allowed length for a phase 1 solution.
        /// </summary>
        public static int MaxPhase1Length { get; set; } = 25;

        private const int SameFace = 0, SameAxisInWrongOrder = 3;

        private double[] _nonRotatedWeights;
        private double[] _rotatedWeights;
        private IEnumerable<Move> _phase1MoveOrder;
        private IEnumerable<Move> _phase2MoveOrder;
        private double[] _weightedPhase2PruningTable;

        /// <summary>
        /// Interrupts the search if set to true.
        /// </summary>
        public SyncedBoolWrapper IsTerminated;
        private SyncedDoubleWrapper _bestSolutionCost;
        private SyncedIntWrapper _bestSolutionIndex;
        private List<Alg> _solutions;
        private Stopwatch _timePassed;
        private TimeSpan _timeout;
        private double _returnValue;
        private double _requiredValue;

        private CubieCube _notRotatedCube;

        private int _cp;
        private int _uEdges;
        private int _dEdges;

        private bool _inversed;
        private int _rotation;

        private int[] _currentPhase1Solution;
        private int[] _currentPhase2Solution;

        public static Alg FindSolution(CubieCube cubeToSolve, TimeSpan timeout, double returnValue, double requiredValue, double[] weights, double[] weightedPhase2PruningTable, bool searchDifferentOrientations = true, bool searchInverse = true)
        {
            if (cubeToSolve is null)
                throw new ArgumentNullException(nameof(cubeToSolve) + " is null.");
            if (returnValue < 0d)
                throw new ArgumentOutOfRangeException(nameof(returnValue) + " cannot be negative: " + returnValue);
            if (requiredValue > 0d && requiredValue < returnValue)
                throw new ArgumentOutOfRangeException(nameof(requiredValue) + " must either be negative or >= " + nameof(returnValue) + ": " + requiredValue + " (" + nameof(requiredValue) + ") < " + returnValue + " (" + nameof(returnValue) + ")");
            MoveWeightsUtils.ValidateWeights(weights);

            TableController.InitializePhase1Tables();
            TableController.InitializePhase2Tables();

            Stopwatch timePassed = new Stopwatch();
            timePassed.Start();

            SyncedBoolWrapper isTerminated = new SyncedBoolWrapper() { Value = false };
            SyncedDoubleWrapper bestSolutionCost = new SyncedDoubleWrapper() { Value = double.MaxValue };
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
                    _returnValue = returnValue,
                    _requiredValue = requiredValue,
                    IsTerminated = isTerminated,
                    _solutions = solutions,
                    _bestSolutionCost = bestSolutionCost,
                    _bestSolutionIndex = bestSolutionIndex,
                    _rotation = orientation % 3,
                    _inversed = orientation / 3 == 1,
                    _nonRotatedWeights = (double[])weights.Clone(), //create a copy to avoid issues caused by multithreading
                    _weightedPhase2PruningTable = weightedPhase2PruningTable
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
            _rotatedWeights = new double[NumMoves];

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
                    double temp = _rotatedWeights[face * 3];
                    _rotatedWeights[face * 3] = _rotatedWeights[face * 3 + 2];
                    _rotatedWeights[face * 3 + 2] = temp;
                }
            }
            #endregion rotate weights

            _phase1MoveOrder = MoveWeightsUtils.OrderMoves(Moves.AllMoves, _rotatedWeights);
            _phase2MoveOrder = MoveWeightsUtils.OrderMoves(TwoPhaseConstants.Phase2Moves, _rotatedWeights);

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
                    if (_timePassed.Elapsed > _timeout && (_requiredValue < 0d || (_solutions.Count > 0 && _bestSolutionCost.Value <= _requiredValue)))
                        IsTerminated.Value = true;

                int cp = _cp;
                int uEdges = _uEdges;
                int dEdges = _dEdges;
                int equatorPermutation = equator;

                double phase1Cost = 0d;

                for (int moveIndex = 0; moveIndex < depth; moveIndex++)
                {
                    int move = _currentPhase1Solution[moveIndex];

                    phase1Cost += _rotatedWeights[moveIndex];
                    cp = TableController.CpMoveTable[cp, move];
                    uEdges = TableController.UEdgesMoveTable[uEdges, move];
                    dEdges = TableController.DEdgesMoveTable[dEdges, move];
                }

                int cornerEquatorPruningIndex = TwoPhaseConstants.NumEquatorPermutations * cp + equator;
                double weightedCornerEquatorPruningValue = _weightedPhase2PruningTable[cornerEquatorPruningIndex];
                if (weightedCornerEquatorPruningValue + phase1Cost > _bestSolutionCost.Value) //TODO better comment //double inaccuracies don't matter
                    return;
                int cornerEquatorPruningValue = TableController.Phase2CornerEquatorPruningTable[cornerEquatorPruningIndex];
                if (cornerEquatorPruningValue > MaxPhase2Length)
                    return;

                int udEdgePermutation = Coordinates.CombineUAndDEdgePermutation(uEdges, dEdges);

                int cornerUdPruningIndex = PruningTables.GetPhase2CornerUdPruningIndex(udEdgePermutation, cp);
                int minMoves = TableController.Phase2CornerUdPruningTable[cornerUdPruningIndex];

                for (int length = minMoves; length <= MaxPhase2Length; length++)
                    SearchPhase2(cp, equatorPermutation, udEdgePermutation, depth: 0, remainingMoves: length, phase1Cost, phase1Length: depth);

                return;
            }

            foreach (int move in _phase1MoveOrder)
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

        private void SearchPhase2(int cp, int equatorPermutation, int udEdgePermutation, int depth, int remainingMoves, double costSoFar, int phase1Length)
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

                double solutionCost = solution.Select(move => _nonRotatedWeights[(int)move])
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

                if (solutionCost <= _returnValue)
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

                double newCostSoFar = costSoFar + _rotatedWeights[move];
                double weightedCornerEquatorPruningValue = _weightedPhase2PruningTable[cornerEquatorPruningIndex];
                if (weightedCornerEquatorPruningValue + newCostSoFar > _bestSolutionCost.Value) //TODO better comment //double inaccuracies don't matter
                    continue;

                _currentPhase2Solution[depth] = move;
                SearchPhase2(newCp, newEquatorPermutation, newUdEdgePermutation, depth + 1, remainingMoves - 1, costSoFar, phase1Length);
            }
        }
    }
}
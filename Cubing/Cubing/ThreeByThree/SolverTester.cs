﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubing.ThreeByThree
{
    //TEST if TimeSpan can be negative
    /// <summary>
    /// An immutable class to test and compare different solvers.
    /// </summary>
    public class SolverTester
    {
        private TimeSpan[] _timeouts = null;
        /// <summary>
        /// All timeouts to test.
        /// </summary>
        public TimeSpan[] Timeouts { get => _timeouts.ToArray(); }
        /// <summary>
        /// The number of different timeouts tested.
        /// </summary>
        public int NumIterations { get; }
        private CubieCube[] _cubesToSolve = null;
        /// <summary>
        /// The cubes the solvers have to solve.
        /// </summary>
        public CubieCube[] CubesToSolve { get => _cubesToSolve.ToArray(); }
        /// <summary>
        /// The number of different cubes tested.
        /// </summary>
        public int NumCubesToSolve { get; }

        /// <summary>
        /// Initialize the tester.
        /// </summary>
        /// <param name="cubesToSolve">
        /// The cubes the solvers have to solve.
        /// </param>
        /// <param name="timeouts">
        /// All timeouts to test.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cubesToSolve"/> or
        /// <paramref name="timeouts"/> is null.
        /// </exception>
        public SolverTester(IEnumerable<CubieCube> cubesToSolve, IEnumerable<TimeSpan> timeouts)
        {
            if (cubesToSolve is null)
                throw new ArgumentNullException(nameof(cubesToSolve) + " is null.");
            if (timeouts is null)
                throw new ArgumentNullException(nameof(timeouts) + " is null.");

            _timeouts = timeouts.ToArray();
            this.NumIterations = _timeouts.Length;
            _cubesToSolve = cubesToSolve.ToArray();
            this.NumCubesToSolve = _cubesToSolve.Length;
        }

        /// <summary>
        /// Run the test for a specific solver.
        /// <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/><c>[i][c]</c>
        /// refers to the solution for cube <c>c</c> in iteration <c>i</c>.
        /// </summary>
        /// <param name="solver">The solver to test.</param>
        /// <returns>
        /// The solutions for <see cref="CubesToSolve"/> found for a specific
        /// timeout.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="solver"/> is null.
        /// </exception>
        public Alg[,] RunTest(Func<CubieCube, TimeSpan, Alg> solver)
        {
            if (solver is null)
                throw new ArgumentNullException(nameof(solver) + " is null.");

            Alg[,] testResult = new Alg[NumIterations, NumCubesToSolve];

            for (int timeoutIndex = 0; timeoutIndex < NumIterations; timeoutIndex++)
                for (int cubeIndex = 0; cubeIndex < NumCubesToSolve; cubeIndex++)
                    testResult[timeoutIndex, cubeIndex] = solver(_cubesToSolve[cubeIndex], Timeouts[timeoutIndex]);

            return testResult;
        }

        /// <summary>
        /// Calculate the cost for every iteration of a test result generated
        /// by <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/>. If for
        /// one cube no solution was found, the cost of the entire iteration
        /// the cube belongs to becomes <see cref="float.PositiveInfinity"/>.
        /// </summary>
        /// <param name="testResult">
        /// Solutions found by
        /// <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/>.
        /// </param>
        /// <param name="costCalculator">
        /// The function used to calculate the cost of each individual solution.
        /// Does not need to handle null values.
        /// </param>
        /// <returns>
        /// The cost of every iteration of a test result generated by
        /// <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/>.
        /// </returns>
        public static float[] TotalCostOfEachIteration(Alg[,] testResult, Func<IEnumerable<Move>, float> costCalculator)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult) + " is null.");
            if (costCalculator is null)
                throw new ArgumentNullException(nameof(costCalculator) + " is null.");

            int numIterations = testResult.GetLength(0);
            int numCubesToSolve = testResult.GetLength(1);

            float[] totalCostOfEachIteration = Enumerable.Repeat(0f, numIterations)
                                                     .ToArray();

            for (int iteration = 0; iteration < numIterations; iteration++)
                for (int cubeIndex = 0; cubeIndex < numCubesToSolve; cubeIndex++)
                {
                    if (testResult[iteration, cubeIndex] is null)
                        continue;
                    totalCostOfEachIteration[iteration] += costCalculator(testResult[iteration, cubeIndex]);
                }

            return totalCostOfEachIteration;
        }

        public static int[] CountSolvedOfEachIteration(Alg[,] testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult) + " is null.");

            int numIterations = testResult.GetLength(0);
            int numCubesToSolve = testResult.GetLength(1);

            int[] countSolvedOfEachIteration = Enumerable.Repeat(0, numIterations)
                                                         .ToArray();

            for (int iteration = 0; iteration < numIterations; iteration++)
                for (int cubeIndex = 0; cubeIndex < numCubesToSolve; cubeIndex++)
                    if (!(testResult[iteration, cubeIndex] is null))
                        countSolvedOfEachIteration[iteration]++;

            return countSolvedOfEachIteration;
        }
    }
}
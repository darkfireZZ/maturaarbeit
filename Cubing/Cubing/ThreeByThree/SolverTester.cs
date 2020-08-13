﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubing.ThreeByThree
{
    //TEST if TimeSpan can be negative
    /// <summary>
    /// A class to test and compare different solvers.
    /// </summary>
    public class SolverTester
    {
        /// <summary>
        /// The initial timeout that is tested.
        /// </summary>
        public TimeSpan InitialTimeout { get; }
        /// <summary>
        /// The final timeout that is tested.
        /// </summary>
        public TimeSpan EndTimeout { get; }
        /// <summary>
        /// How much the timeout is increased after each iteration.
        /// </summary>
        public TimeSpan TimeoutIncrement { get; }
        /// <summary>
        /// The total number of different timeouts that are tested.
        /// </summary>
        public int NumIterations { get; }
        /// <summary>
        /// The cubes the solvers have to solve.
        /// </summary>
        public IEnumerable<CubieCube> CubesToSolve { get; }

        /// <summary>
        /// Initialize the tester.
        /// </summary>
        /// <param name="cubesToSolve">
        /// The cubes the solvers have to solve.
        /// </param>
        /// <param name="initialTimeout">
        /// The initial timeout that is tested.
        /// </param>
        /// <param name="endTimeout">
        /// The final timeout that is tested.
        /// </param>
        /// <param name="numIterations">
        /// The total number of different timeouts that are tested.
        /// </param>
        public SolverTester(IEnumerable<CubieCube> cubesToSolve, TimeSpan initialTimeout, TimeSpan endTimeout, int numIterations)
        {
            if (cubesToSolve is null)
                throw new ArgumentNullException(nameof(cubesToSolve) + " is null.");
            if (numIterations < 0)
                throw new ArgumentOutOfRangeException(nameof(numIterations) + " cannot be negative: " + numIterations);

            this.InitialTimeout = initialTimeout;
            this.EndTimeout = endTimeout;
            this.NumIterations = numIterations;
            this.TimeoutIncrement = TimeSpan.FromMilliseconds((EndTimeout.TotalMilliseconds - InitialTimeout.TotalMilliseconds) / (NumIterations - 1));
            this.CubesToSolve = cubesToSolve;
        }

        /// <summary>
        /// Run the test for a specific solver.
        /// <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/><c>[i][c]</c>
        /// refers to the solution for cube <c>c</c> in iteration <c>i</c>. See
        /// <see cref="GetTimeouts()"/> for the timeouts used in each iteration.
        /// </summary>
        /// <param name="solver">The solver to test.</param>
        /// <returns>
        /// The solutions for <see cref="CubesToSolve"/> found for a specific
        /// timeout.
        /// </returns>
        public Alg[,] RunTest(Func<CubieCube, TimeSpan, Alg> solver)
        {
            if (solver is null)
                throw new ArgumentNullException(nameof(solver) + " is null.");

            int numCubesToSolve = CubesToSolve.Count();
            Alg[,] testResult = new Alg[NumIterations, numCubesToSolve];
            TimeSpan timeout = InitialTimeout;

            for (int iteration = 0; iteration < NumIterations; iteration++)
            {
                int cubeIndex = 0;
                foreach (CubieCube cubeToSolve in CubesToSolve)
                    testResult[iteration, cubeIndex++] = solver(cubeToSolve, timeout);

                timeout += TimeoutIncrement;
            }

            return testResult;
        }

        /// <summary>
        /// Calculate the cost for every iteration of a test result generated
        /// by <see cref="RunTest(Func{CubieCube, TimeSpan, Alg})"/>. If for
        /// one cube no solution was found, the cost of the entire iteration
        /// the cube belongs to becomes <see cref="double.PositiveInfinity"/>.
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
        public static double[] TotalCostOfEachIteration(Alg[,] testResult, Func<IEnumerable<Move>, double> costCalculator)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult) + " is null.");
            if (costCalculator is null)
                throw new ArgumentNullException(nameof(costCalculator) + " is null.");

            int numIterations = testResult.GetLength(0);
            int numCubesToSolve = testResult.GetLength(1);

            double[] totalCostOfEachIteration = Enumerable.Repeat(0d, numIterations)
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

        /// <summary>
        /// Get all the timeouts tested.
        /// </summary>
        /// <returns>All the timeouts tested.</returns>
        public TimeSpan[] GetTimeouts()
        {
            TimeSpan[] timeouts = new TimeSpan[NumIterations];

            TimeSpan timeout = InitialTimeout;
            for (int iteration = 0; iteration < NumIterations; iteration++)
            {
                timeouts[iteration] = timeout;
                timeout += TimeoutIncrement;
            }

            return timeouts;
        }
    }
}
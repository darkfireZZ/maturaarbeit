#r "C:\Users\LocalUser\Projects\maturaarbeit\Cubing\Cubing\bin\Debug\netstandard2.0\Cubing.dll"

using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using System;
using System.Collections.Generic;
using System.Linq;

//variables
int numCubesToSolve = 1000;
double initialTimeoutMillis = 1d;
double endTimeoutMillis = 10d;
int numIterations = 10;
int seed = 7777777;

double[] weights = //calculated using CalculateWeights.csx
{
    177.527119373527,
    256.239210260758,
    185.304489806619,
    193.680509666202,
    253.667627641438,
    135.866588707329,
    163.998402236243,
    257.066110436587,
    336.524947207386,
    285.310717153464,
    325.593253468817,
    240.214190137905,
    177.452605718219,
    275.449097777110,
    213.972822847726,
    277.684255277544,
    316.044385167830,
    333.445050902919
};

//calculate required time
double requiredMillis = (initialTimeoutMillis + endTimeoutMillis) * numCubesToSolve * numIterations / 2;
TimeSpan requiredTime = TimeSpan.FromMilliseconds(requiredMillis);
Console.WriteLine("It will take " + requiredTime + " for every solver tested.");

//initialize
Random random = new Random(seed);
CubieCube[] cubesToSolve = new CubieCube[numCubesToSolve];
for (int cubeIndex = 0; cubeIndex < numCubesToSolve; cubeIndex++)
    cubesToSolve[cubeIndex] = CubieCube.CreateRandom(random);

TimeSpan initialTimeout = TimeSpan.FromMilliseconds(initialTimeoutMillis);
TimeSpan endTimeout = TimeSpan.FromMilliseconds(endTimeoutMillis);

SolverTester solverTester = new SolverTester(cubesToSolve, initialTimeout, endTimeout, numIterations);
TimeSpan[] timeouts = solverTester.GetTimeouts();

double[] weightedPruningTable = WeightedPruningTables.CreateWeightedPhase2Table(weights);

Func<CubieCube, TimeSpan, Alg> regularTwoPhaseSolver = (cubeToSolve, timeout) => TwoPhaseSolver.FindSolution(cubeToSolve, timeout, 0, -1);
Func<CubieCube, TimeSpan, Alg> weightedTwoPhaseSolver = (cubeToSolve, timeout) => WeightedTwoPhaseSolver.FindSolution(cubeToSolve, timeout, 0d, -1d, weights, weightedPruningTable);

//evaluate and print
Func<IEnumerable<Move>, double> lengthCostCalculator = alg => (double)alg.Count();
Func<IEnumerable<Move>, double> weightsCostCalculator = alg => MoveWeightsUtils.CalculateCost(alg, weights);

Alg[,] regularTwoPhaseResults = solverTester.RunTest(regularTwoPhaseSolver);
int[] regularTwoPhaseNumSolved = SolverTester.CountSolvedOfEachIteration(regularTwoPhaseResults);
double[] regularTwoPhaseLengths = SolverTester.TotalCostOfEachIteration(regularTwoPhaseResults, lengthCostCalculator);
double[] regularTwoPhaseCosts = SolverTester.TotalCostOfEachIteration(regularTwoPhaseResults, weightsCostCalculator);
Console.WriteLine("regular two-phase results:");
for (int iteration = 0; iteration < numIterations; iteration++)
{
    double percentageSolved = regularTwoPhaseNumSolved[iteration] * 100d / numCubesToSolve;
    double averageLength = regularTwoPhaseLengths[iteration] / regularTwoPhaseNumSolved[iteration];
    double averageCost = regularTwoPhaseCosts[iteration] / regularTwoPhaseNumSolved[iteration];
    Console.WriteLine("timeout: " + timeouts[iteration] + ", percentage solved: " + percentageSolved + ", average length: " + averageLength + ", average cost: " + averageCost);
}

Alg[,] weightedTwoPhaseResults = solverTester.RunTest(weightedTwoPhaseSolver);
int[] weightedTwoPhaseNumSolved = SolverTester.CountSolvedOfEachIteration(weightedTwoPhaseResults);
double[] weightedTwoPhaseLengths = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, lengthCostCalculator);
double[] weightedTwoPhaseCosts = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, weightsCostCalculator);
Console.WriteLine("weighted two-phase results:");
for (int iteration = 0; iteration < numIterations; iteration++)
{
    double percentageSolved = weightedTwoPhaseNumSolved[iteration] * 100d / numCubesToSolve;
    double averageLength = weightedTwoPhaseLengths[iteration] / weightedTwoPhaseNumSolved[iteration];
    double averageCost = weightedTwoPhaseCosts[iteration] / weightedTwoPhaseNumSolved[iteration];
    Console.WriteLine("timeout: " + timeouts[iteration] + ", percentage solved: " + percentageSolved + ", average length: " + averageLength + ", average cost: " + averageCost);
}
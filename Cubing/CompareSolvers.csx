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

float[] weights = //calculated using CalculateWeights.csx
{
    177.527119373527f,
    256.239210260758f,
    185.304489806619f,
    193.680509666202f,
    253.667627641438f,
    135.866588707329f,
    163.998402236243f,
    257.066110436587f,
    336.524947207386f,
    285.310717153464f,
    325.593253468817f,
    240.214190137905f,
    177.452605718219f,
    275.449097777110f,
    213.972822847726f,
    277.684255277544f,
    316.044385167830f,
    333.445050902919f
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

float[] weightedPruningTable = WeightedPruningTables.CreateWeightedPhase2CornerEquatorTable(weights);

Func<CubieCube, TimeSpan, Alg> regularTwoPhaseSolver = (cubeToSolve, timeout) => TwoPhaseSolver.FindSolution(cubeToSolve, timeout, 0, -1);
Func<CubieCube, TimeSpan, Alg> weightedTwoPhaseSolver = (cubeToSolve, timeout) => WeightedTwoPhaseSolver.FindSolution(cubeToSolve, timeout, 0f, -1f, weights, weightedPruningTable);

//evaluate and print
Func<IEnumerable<Move>, float> lengthCostCalculator = alg => (float)alg.Count();
Func<IEnumerable<Move>, float> weightsCostCalculator = alg => MoveWeightsUtils.CalculateCost(alg, weights);

Alg[,] regularTwoPhaseResults = solverTester.RunTest(regularTwoPhaseSolver);
int[] regularTwoPhaseNumSolved = SolverTester.CountSolvedOfEachIteration(regularTwoPhaseResults);
float[] regularTwoPhaseLengths = SolverTester.TotalCostOfEachIteration(regularTwoPhaseResults, lengthCostCalculator);
float[] regularTwoPhaseCosts = SolverTester.TotalCostOfEachIteration(regularTwoPhaseResults, weightsCostCalculator);
Console.WriteLine("regular two-phase results:");
for (int iteration = 0; iteration < numIterations; iteration++)
{
    float percentageSolved = regularTwoPhaseNumSolved[iteration] * 100f / numCubesToSolve;
    float averageLength = regularTwoPhaseLengths[iteration] / regularTwoPhaseNumSolved[iteration];
    float averageCost = regularTwoPhaseCosts[iteration] / regularTwoPhaseNumSolved[iteration];
    Console.WriteLine("timeout: " + timeouts[iteration] + ", percentage solved: " + percentageSolved + ", average length: " + averageLength + ", average cost: " + averageCost);
}

Alg[,] weightedTwoPhaseResults = solverTester.RunTest(weightedTwoPhaseSolver);
int[] weightedTwoPhaseNumSolved = SolverTester.CountSolvedOfEachIteration(weightedTwoPhaseResults);
float[] weightedTwoPhaseLengths = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, lengthCostCalculator);
float[] weightedTwoPhaseCosts = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, weightsCostCalculator);
Console.WriteLine("weighted two-phase results:");
for (int iteration = 0; iteration < numIterations; iteration++)
{
    float percentageSolved = weightedTwoPhaseNumSolved[iteration] * 100f / numCubesToSolve;
    float averageLength = weightedTwoPhaseLengths[iteration] / weightedTwoPhaseNumSolved[iteration];
    float averageCost = weightedTwoPhaseCosts[iteration] / weightedTwoPhaseNumSolved[iteration];
    Console.WriteLine("timeout: " + timeouts[iteration] + ", percentage solved: " + percentageSolved + ", average length: " + averageLength + ", average cost: " + averageCost);
}
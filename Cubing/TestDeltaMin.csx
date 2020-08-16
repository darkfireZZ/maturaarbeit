#r "C:\Users\LocalUser\Projects\maturaarbeit\Cubing\Cubing\bin\Debug\netstandard2.0\Cubing.dll"

using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using System;
using System.Collections.Generic;
using System.Linq;

//variables
int numCubesToSolve = 100;
int seed = 7777777;
float[] deltaMin = { 0f, 50f, 100f, 150f, 200f, 250f };

/*double initialTimeoutMillis = 1d; //inclusive
double endTimeoutMillis = 10d; //inclusive
int numIterations = 10;
double timeoutIncrement = (endTimeoutMillis - initialTimeoutMillis) / (numIterations - 1);
TimeSpan[] timeouts = Enumerable.Range(1, 10)
                              .Select(iteration => iteration * timeoutIncrement)
                              .Select(timeoutMillis => TimeSpan.FromMilliseconds(timeoutMillis))
                              .ToArray();*/

double[] timeoutsMillis = { 1d, 2d, 3d, 4d, 5d, 6d, 7d, 8d, 9d, 10d, 20d, 30d, 40d, 50d, 60d, 70d, 80d, 90d, 100d, 200d, 300d, 400d, 500d, 600d, 700d, 800d, 900d, 1000d };
TimeSpan[] timeouts = timeoutsMillis.Select(timeoutMillis => TimeSpan.FromMilliseconds(timeoutMillis))
                                   .ToArray();
int numIterations = timeouts.Length;

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
TimeSpan requiredTime = TimeSpan.FromMilliseconds(timeouts.Aggregate(TimeSpan.Zero, (acc, val) => acc + val).TotalMilliseconds * numCubesToSolve);
Console.WriteLine("It will take " + requiredTime + " for every solver tested.");

//initialize
Random random = new Random(seed);
CubieCube[] cubesToSolve = new CubieCube[numCubesToSolve];
for (int cubeIndex = 0; cubeIndex < numCubesToSolve; cubeIndex++)
    cubesToSolve[cubeIndex] = CubieCube.CreateRandom(random);

SolverTester solverTester = new SolverTester(cubesToSolve, timeouts);

float[] weightedPruningTable = WeightedPruningTables.CreateWeightedPhase2CornerEquatorTable(weights);

foreach (float minImprovement in deltaMin)
{
    Func<CubieCube, TimeSpan, Alg> weightedTwoPhaseSolver = (cubeToSolve, timeout) => WeightedTwoPhaseSolver.FindSolution(cubeToSolve, timeout, 0f, -1f, weights, weightedPruningTable);

    //evaluate and print
    Func<IEnumerable<Move>, float> lengthCostCalculator = alg => (float)alg.Count();
    Func<IEnumerable<Move>, float> weightsCostCalculator = alg => MoveWeightsUtils.CalculateCost(alg, weights);

    Alg[,] weightedTwoPhaseResults = solverTester.RunTest(weightedTwoPhaseSolver);
    int[] weightedTwoPhaseNumSolved = SolverTester.CountSolvedOfEachIteration(weightedTwoPhaseResults);
    float[] weightedTwoPhaseLengths = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, lengthCostCalculator);
    float[] weightedTwoPhaseCosts = SolverTester.TotalCostOfEachIteration(weightedTwoPhaseResults, weightsCostCalculator);
    Console.WriteLine("minImprovement = " + minImprovement);
    Console.WriteLine("timeout;percentage solved;average length;average cost");
    for (int iteration = 0; iteration < numIterations; iteration++)
    {
        float percentageSolved = weightedTwoPhaseNumSolved[iteration] * 100f / numCubesToSolve;
        float averageLength = weightedTwoPhaseLengths[iteration] / weightedTwoPhaseNumSolved[iteration];
        float averageCost = weightedTwoPhaseCosts[iteration] / weightedTwoPhaseNumSolved[iteration];
        Console.WriteLine(timeouts[iteration] + ";" + percentageSolved + ";" + averageLength + ";" + averageCost);
    }
}
#r "C:\Users\LocalUser\Projects\maturaarbeit\Cubing\Cubing\bin\Debug\netstandard2.0\Cubing.dll"

using Cubing;
using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using System;
using System.Collections.Generic;
using System.Linq;
using static Cubing.ThreeByThree.Constants;

//takes a while to run
string path = @"C:\Users\LocalUser\Google Drive\Schule\Maturaarbeit\data.csv";
List<CsTimerData> data = new List<CsTimerData>();
CsTimerData.ReadCsTimerData(path, example => data.Add(example));
data.RemoveAll(example => example.Penalty != CubingPenalty.Okay);

int numExamples = data.Count();
double[,] features = new double[numExamples, NumMoves + 1]; //18 moves + cube pickup time
double[] labels = new double[numExamples];
Console.WriteLine("Number of valid examples: " + numExamples);

int exampleIndex = 0;
foreach (CsTimerData example in data)
{
    features[exampleIndex, 0] = 1;

    int[] countOfEachMove = Alg.FromString(example.Scramble).GetCountOfEachMove();
    for (int moveIndex = 0; moveIndex < NumMoves; moveIndex++)
        features[exampleIndex, moveIndex + 1] = countOfEachMove[moveIndex];

    labels[exampleIndex] = example.Milliseconds;
    exampleIndex++;
}

double[] weights = LinearRegression.Learn(features, labels, learningRate: 0.05, numberOfIterations: 10000);

int totalMillis = data.Select(example => example.Milliseconds)
                           .Sum();
double averageMillis = (double)totalMillis / numExamples;
int totalMoves = data.Select(example => Alg.FromString(example.Scramble).Length)
                     .Sum();
double averageMoves = (double)totalMoves / numExamples;
double cost = LinearRegression.Cost(features, labels, weights);
double absCost = LinearRegression.AbsoluteCost(features, labels, weights);

Console.WriteLine("Squared cost: " + cost);
Console.WriteLine("Absolute cost: " + absCost);
Console.WriteLine("average time (in ms): " + averageMillis);
Console.WriteLine("average number of moves:" + averageMoves);

foreach (double weight in weights)
    Console.WriteLine(weight);
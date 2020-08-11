#r "C:\Users\LocalUser\Projects\maturaarbeit\Cubing\Cubing\bin\Debug\netstandard2.0\Cubing.dll"

using Cubing;
using Cubing.ThreeByThree.TwoPhase;
using System;
using System.Collections.Generic;

//takes a while to run
string path = @"C:\Users\LocalUser\Google Drive\Schule\Maturaarbeit\data.csv";
List<CsTimerData> data = new List<CsTimerData>();
CsTimerData.ReadCsTimerData(path, example => data.Add(example));
double[] weights = MoveWeightsUtils.CalculateWeights(data);

foreach (double weight in weights)
    Console.WriteLine(weight);
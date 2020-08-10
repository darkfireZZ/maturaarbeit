using System.Linq;
using static Cubing.ThreeByThree.Coordinates;

namespace Cubing.ThreeByThree.TwoPhase
{
    public static class WeightedPruningTables
    {
        //TODO think what happens when R2 > 2 * R
        public static double[] CreateWeightedPhase2Table(double[] weights)
        {
            MoveWeightsUtils.ValidateWeights(weights);

            double invalid = double.NaN;

            double[] pruningTable = Enumerable
                .Repeat(invalid, TwoPhaseConstants.CornerEquatorPruningTableSizePhase2)
                .ToArray();

            TableController.InitializeCpMoveTable();
            TableController.InitializeEquatorPermutationMoveTable();

            pruningTable[0] = 0d;
            int numChanged = -1;

            while (numChanged != 0)
            {
                numChanged = 0;
                for (int cp = 0; cp < NumCpCoords; cp++)
                {
                    for (int equatorPermutation = 0; equatorPermutation < TwoPhaseConstants.NumEquatorPermutations; equatorPermutation++)
                    {
                        int index = TwoPhaseConstants.NumEquatorPermutations * cp + equatorPermutation;
                        if (pruningTable[index] != invalid)
                        {
                            foreach (int move in TwoPhaseConstants.Phase2Moves)
                            {
                                int newCp = TableController.CpMoveTable[cp, move];
                                int newEquatorPerm = TableController.EquatorPermutationMoveTable[equatorPermutation, move];
                                int newIndex = TwoPhaseConstants.NumEquatorPermutations * newCp + newEquatorPerm;

                                double newPruningValue = pruningTable[index] + weights[move];

                                if (pruningTable[newIndex] == invalid || pruningTable[newIndex] > newPruningValue)
                                {
                                    pruningTable[newIndex] = newPruningValue;
                                    numChanged++;
                                }
                            }
                        }
                    }
                }
            }

            return pruningTable;
        }
    }
}
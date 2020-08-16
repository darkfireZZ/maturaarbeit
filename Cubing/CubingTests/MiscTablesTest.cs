using Cubing.ThreeByThree;
using Cubing.ThreeByThree.TwoPhase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Cubing.ThreeByThree.TwoPhase.Coordinates;
using static Cubing.ThreeByThree.TwoPhase.Symmetries;

namespace CubingTests
{
    [TestClass]
    public class MiscTablesTest
    {
        //IMPR performance
        [TestMethod]
        public void ConjugateCoCoordinateTest()
        {
            Assert.AreEqual(2187, SymmetryReduction.ConjugateCornerOrientationCoordinate.GetLength(0));
            Assert.AreEqual(16, SymmetryReduction.ConjugateCornerOrientationCoordinate.GetLength(1));

            CubieCube coordCube = CubieCube.CreateSolved();
            for (int co = 0; co < NumCornerOrientations; co++)
            {
                SetCornerOrientation(coordCube, co);
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                {
                    CubieCube symCube = Symmetries.SymmetryCubes[sym].Clone();
                    symCube.Multiply(coordCube);
                    symCube.Multiply(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    CubieCube result = CubieCube.CreateSolved();
                    SetCornerOrientation(result, SymmetryReduction.ConjugateCornerOrientationCoordinate[co, sym]);

                    CollectionAssert.AreEqual(symCube.CornerOrientation, result.CornerOrientation);
                }
            }
        }

        //IMPR performance
        [TestMethod]
        public void ReductionSymmetryTest()
        {
            CubieCube cube = CubieCube.CreateSolved();

            for (int equator = 0; equator < NumEquatorDistributions; equator++)
            {
                SetEquatorDistribution(cube, equator);
                for (int eo = 0; eo < NumEdgeOrientations; eo++)
                {
                    SetEdgeOrientation(cube, eo);

                    int reducedCoord = SymmetryReduction.ReduceEoEquatorCoordinate[equator * NumEdgeOrientations + eo];
                    int expandedCoord = SymmetryReduction.ExpandEoEquatorCoordinate[reducedCoord];

                    int symmetryIndex = SymmetryReduction.EoEquatorReductionSymmetry[equator * NumEdgeOrientations + eo];

                    CubieCube symCube = Symmetries.SymmetryCubes[symmetryIndex].Clone();
                    symCube.Multiply(cube);
                    symCube.Multiply(Symmetries.SymmetryCubes[Symmetries.InverseIndex[symmetryIndex]]);

                    Assert.AreEqual(expandedCoord % NumEdgeOrientations, GetEdgeOrientation(symCube));
                    Assert.AreEqual(expandedCoord / NumEdgeOrientations, GetEquatorDistribution(symCube));
                }
            }
        }

        [TestMethod]
        public void ReduceEoEquatorCoordinateTest()
        {
            Random random = new Random(7777777);
            int length = 30;
            int count = 100;

            for (int i = 0; i < count; i++)
            {
                CubieCube cube = CubieCube.FromAlg(Alg.FromRandomMoves(length, random));
                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                {
                    CubieCube symCube = Symmetries.SymmetryCubes[sym].Clone();
                    symCube.Multiply(cube);
                    symCube.Multiply(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    int cubeEoEquator = GetEoEquatorCoord(cube);
                    int cubeReducedEoEquator = SymmetryReduction.ReduceEoEquatorCoordinate[cubeEoEquator];

                    int symCubeEoEquator = GetEoEquatorCoord(symCube);
                    int symCubeReducedEoEqutor = SymmetryReduction.ReduceEoEquatorCoordinate[symCubeEoEquator];

                    Assert.AreEqual(cubeReducedEoEquator, symCubeReducedEoEqutor);
                }
            }
        }

        //IMPR performance
        [TestMethod]
        public void EoEquatorCoordinateTest()
        {
            bool found;

            for (int expanded = 0; expanded < NumEquatorDistributions * NumEdgeOrientations; expanded++)
            {
                found = false;

                int reduced = SymmetryReduction.ReduceEoEquatorCoordinate[expanded];
                int expandedSym = SymmetryReduction.ExpandEoEquatorCoordinate[reduced];

                CubieCube expandedCube = CubieCube.CreateSolved();
                SetEdgeOrientation(expandedCube, expanded % 2048);
                SetEquatorDistribution(expandedCube, expanded / 2048);

                for (int sym = 0; sym < NumSymmetriesDh4; sym++)
                {
                    CubieCube symCube = Symmetries.SymmetryCubes[sym].Clone();
                    symCube.MultiplyEdges(expandedCube);
                    symCube.MultiplyEdges(Symmetries.SymmetryCubes[Symmetries.InverseIndex[sym]]);

                    if (expandedSym % 2048 == GetEdgeOrientation(symCube) &&
                        expandedSym / 2048 == GetEquatorDistribution(symCube))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    Assert.Fail();
            }
        }
    }
}
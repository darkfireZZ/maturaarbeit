using System;
using System.IO;

namespace Cubing.ThreeByThree.TwoPhase
{
    /// <summary>
    /// Stores all the frequently used tables for the two-phase solver.
    /// </summary>
    public static class TableController
    {
        /// <summary>
        /// The directory where the tables are saved.
        /// </summary>
        public static string Directory { get; set; } = @"C:\ProgramData\nbCubeData";

        /// <summary>
        /// The edge orientation move table.
        /// </summary>
        public static short[,] EoMoveTable { get; set; } = null;
        /// <summary>
        /// The corner orientation move table.
        /// </summary>
        public static short[,] CoMoveTable { get; set; } = null;
        /// <summary>
        /// The equator distribution move table.
        /// </summary>
        public static short[,] EquatorDistributionMoveTable { get; set; } = null;
        /// <summary>
        /// The equator permutation move table.
        /// </summary>
        public static sbyte[,] EquatorPermutationMoveTable { get; set; } = null;
        /// <summary>
        /// The equator move table.
        /// </summary>
        public static short[,] EquatorMoveTable { get; set; } = null;
        /// <summary>
        /// The U-edges move table.
        /// </summary>
        public static short[,] UEdgesMoveTable { get; set; } = null;
        /// <summary>
        /// The D-edges move table.
        /// </summary>
        public static short[,] DEdgesMoveTable { get; set; } = null;
        /// <summary>
        /// The corner permutation move table.
        /// </summary>
        public static ushort[,] CpMoveTable { get; set; } = null;
        /// <summary>
        /// The U and D edge permutation move table.
        /// </summary>
        public static ushort[,] UdEdgePermutationMoveTable { get; set; } = null;

        /// <summary>
        /// The pruning table for phase 1.
        /// </summary>
        public static byte[] Phase1PruningTable { get; set; } = null;

        /// <summary>
        /// The corner permutation and equator permutation pruning table for phase 2.
        /// </summary>
        public static byte[] Phase2CornerEquatorPruningTable { get; set; } = null;

        /// <summary>
        /// The corner permutation and U- and D-edge permutation pruning table for phase 2.
        /// </summary>
        public static byte[] Phase2CornerUdPruningTable { get; set; } = null;

        /// <summary>
        /// Initialize the corner orientation move table for phase 1.
        /// </summary>
        public static void InitializeCoMoveTable()
        {
            if (CoMoveTable is null)
            {
                Console.WriteLine("Initializing corner orientation move table");
                CoMoveTable = MoveTables.CreateCoMoveTable();
            }
        }

        /// <summary>
        /// Initialize the edge orientation move table for phase 1.
        /// </summary>
        public static void InitializeEoMoveTable()
        {
            if (EoMoveTable is null)
            {
                Console.WriteLine("Initializing edge orientation move table");
                EoMoveTable = MoveTables.CreateEoMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator distribution move table.
        /// </summary>
        public static void InitializeEquatorDistributionMoveTable()
        {
            if (EquatorDistributionMoveTable is null)
            {
                Console.WriteLine("Initializing equator distribution move table");
                EquatorDistributionMoveTable = MoveTables.CreateEquatorDistributionMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator permutation move table.
        /// </summary>
        public static void InitializeEquatorPermutationMoveTable()
        {
            if (EquatorPermutationMoveTable is null)
            {
                Console.WriteLine("Initializing equator permutation move table");
                EquatorPermutationMoveTable = MoveTables.CreateEquatorPermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the equator move table.
        /// </summary>
        public static void InitializeEquatorMoveTable()
        {
            if (EquatorMoveTable is null)
            {
                Console.WriteLine("Initializing equator move table");
                EquatorMoveTable = MoveTables.CreateEquatorMoveTable();
            }
        }

        /// <summary>
        /// Initialize the U-edges move table.
        /// </summary>
        public static void InitializeUEdgesMoveTable()
        {
            if (UEdgesMoveTable is null)
            {
                Console.WriteLine("Initializing U-edges move table");
                UEdgesMoveTable = MoveTables.CreateUEdgesMoveTable();
            }
        }

        /// <summary>
        /// Initialize the D-edges move table.
        /// </summary>
        public static void InitializeDEdgesMoveTable()
        {
            if (DEdgesMoveTable is null)
            {
                Console.WriteLine("Initializing D-edges move table");
                DEdgesMoveTable = MoveTables.CreateDEdgesMoveTable();
            }
        }

        /// <summary>
        /// Initialize the corner permutation move table.
        /// </summary>
        public static void InitializeCpMoveTable()
        {
            if (CpMoveTable is null)
            {
                Console.WriteLine("Initializing corner permutation move table");
                CpMoveTable = MoveTables.CreateCpMoveTable();
            }
        }

        /// <summary>
        /// Initialize the U and D edge permutation table.
        /// </summary>
        public static void InitializeUdEdgePermutationMoveTable()
        {
            if (UdEdgePermutationMoveTable is null)
            {
                Console.WriteLine("Initializing U and D edge permutation move table");
                UdEdgePermutationMoveTable = MoveTables.CreateUdEdgePermutationMoveTable();
            }
        }

        /// <summary>
        /// Initialize the move tables for phase 1.
        /// </summary>
        public static void InitializePhase1MoveTables()
        {
            InitializeCoMoveTable();
            InitializeEoMoveTable();
            InitializeEquatorDistributionMoveTable();
            InitializeEquatorMoveTable();
        }

        /// <summary>
        /// Initialize the move tables for phase 2.
        /// </summary>
        public static void InitializePhase2MoveTables()
        {
            InitializeCpMoveTable();
            InitializeUdEdgePermutationMoveTable();
            InitializeEquatorMoveTable();
            InitializeEquatorPermutationMoveTable();
            InitializeUEdgesMoveTable();
            InitializeDEdgesMoveTable();
        }

        //TODO handle exceptions
        /// <summary>
        /// Initialize the pruning table for phase 1.
        /// </summary>
        public static void InitializePhase1PruningTable()
        {
            if (Phase1PruningTable is null)
            {
                string file = Directory + @"\phase1pruning.table";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 1 pruning table");
                    Phase1PruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 1 pruning table");
                    Phase1PruningTable = PruningTables.CreatePhase1Table();
                    File.WriteAllBytes(file, Phase1PruningTable);
                }
            }
        }

        //TODO handle exceptions
        /// <summary>
        /// Initialize the pruning table for phase 2.
        /// </summary>
        public static void InitializePhase2CornerEquatorPruningTable()
        {
            if (Phase2CornerEquatorPruningTable is null)
            {
                string file = Directory + @"\phase2CornerEquatorPruning.table";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 2 corner permutation and equator permutation pruning table");
                    Phase2CornerEquatorPruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 2 corner permutation and equator permutation pruning table");
                    Phase2CornerEquatorPruningTable = PruningTables.CreatePhase2CornerEquatorTable();
                    File.WriteAllBytes(file, Phase2CornerEquatorPruningTable);
                }
            }
        }

        //TODO handle exceptions
        /// <summary>
        /// Initialize the corner permutation and equator permutation pruning table for phase 2.
        /// </summary>
        public static void InitializePhase2CornerUdPruningTable()
        {
            if (Phase2CornerUdPruningTable is null)
            {
                string file = Directory + @"\phase2CornerUDPruning.table";
                if (File.Exists(file))
                {
                    Console.WriteLine("Loading phase 2 corner permutation and U- and D-edge permutation pruning table");
                    Phase2CornerUdPruningTable = File.ReadAllBytes(file);
                }
                else
                {
                    Console.WriteLine("Initializing phase 2 corner permutation and U- and D-edge permutation pruning table");
                    Phase2CornerUdPruningTable = PruningTables.CreatePhase2CornerUDTable();
                    File.WriteAllBytes(file, Phase2CornerUdPruningTable);
                }
            }
        }

        /// <summary>
        /// Initialize the pruning tables for phase 2.
        /// </summary>
        public static void InitializePhase2PruningTables()
        {
            InitializePhase2CornerEquatorPruningTable();
            InitializePhase2CornerUdPruningTable();
        }

        /// <summary>
        /// Initialize the tables for phase 1.
        /// </summary>
        public static void InitializePhase1Tables()
        {
            InitializePhase1MoveTables();
            InitializePhase1PruningTable();
        }

        /// <summary>
        /// Initialize the tables for phase 2.
        /// </summary>
        public static void InitializePhase2Tables()
        {
            InitializePhase2MoveTables();
            InitializePhase2PruningTables();
        }
    }
}
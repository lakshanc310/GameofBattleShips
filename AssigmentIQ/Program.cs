using AssigmentIQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssigmentIQ
{
    internal class Program
    {
        private const int SPACELENGTH = 19;
        private const int BOXCOUNT = 101;

        static void Main(string[] args)
        {
            bool isShowShips = false;
            Fleet userFleet = new Fleet();
            Fleet enemyFleet = new Fleet();

            Dictionary<char, int> Coordinates = PopulateDictionary();
            ///print First Line of my Map
            printFirstLine();
            for (int h = 0; h < SPACELENGTH; h++)
            {
                Console.Write(" ");
            }

            PrintMap(userFleet.FirePositions, userFleet, enemyFleet, isShowShips);

            int Game;
            for (Game = 1; Game < BOXCOUNT; Game++)
            {
                userFleet.StepsTaken++;

                Position position = new Position();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("***************************************************************************************************************");
                Console.WriteLine("Enter firing coodinates please.");
                Console.WriteLine("Firing coodinates can be given @ character from A-J with a number from 1-10. EG A1 ");
                string input = Console.ReadLine();
                position = MaptoNumericCoordnates(input, Coordinates);

                if (position.x == -1 || position.y == -1)
                {
                    Console.WriteLine("Invalid coordinates!");
                    Game--;
                    continue;
                }

                if (userFleet.FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    Console.WriteLine("This coordinate already being shot.");
                    Game--;
                    continue;
                }


                enemyFleet.Fire();

                var index = userFleet.FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);

                if (index == -1)
                    userFleet.FirePositions.Add(position);

                Console.Clear();



                userFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                userFleet.CheckShipStatus(enemyFleet.FirePositions);

                enemyFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                enemyFleet.CheckShipStatus(userFleet.FirePositions);

                printFirstLine();
                for (int h = 0; h < SPACELENGTH; h++)
                {
                    Console.Write(" ");
                }



                PrintMap(userFleet.FirePositions, userFleet, enemyFleet, isShowShips);

                Commentator(userFleet, true);
                Commentator(enemyFleet, false);
                if (enemyFleet.IsDestroyedAll || userFleet.IsDestroyedAll) { break; }


            }

            Console.ForegroundColor = ConsoleColor.White;

            if (enemyFleet.IsDestroyedAll && !userFleet.IsDestroyedAll)
            {
                Console.WriteLine("Game Ended, you win.");
            }
            else if (!enemyFleet.IsDestroyedAll && userFleet.IsDestroyedAll)
            {
                Console.WriteLine("Game Ended, you lose.");
            }
            else
            {
                Console.WriteLine("Game Ended, draw.");
            }

            Console.WriteLine("Total steps taken:{0} ", Game);
            Console.ReadLine();
        }
        static Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };

            return Coordinate;
        }
        static void PrintMap(List<Position> positions, Fleet userFleet, Fleet enemyFleet, bool showEnemyShips)
        {
            printFirstLine();
            Console.WriteLine();
            if (!showEnemyShips)
                showEnemyShips = userFleet.IsDestroyedAll;

            List<Position> SortedLFirePositions = positions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedShipsPositions = enemyFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedShipsPositions = SortedShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            int hitCounter = 0;
            int EnemyshipCounter = 0;
            int myShipCounter = 0;
            int enemyHitCounter = 0;

            char row = 'A';
            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool keepGoing = true;

                        #region row indicator
                        if (y == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("[" + row + "]");
                            row++;
                        }
                        #endregion


                        if (SortedLFirePositions.Count != 0 && SortedLFirePositions[hitCounter].x == x && SortedLFirePositions[hitCounter].y == y)
                        {

                            if (SortedLFirePositions.Count - 1 > hitCounter)
                                hitCounter++;

                            if (enemyFleet.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[*]");
                                keepGoing = false;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("[X]");

                                keepGoing = false;
                            }

                        }

                        if (keepGoing && showEnemyShips && SortedShipsPositions.Count != 0 && SortedShipsPositions[EnemyshipCounter].x == x && SortedShipsPositions[EnemyshipCounter].y == y)

                        {

                            if (SortedShipsPositions.Count - 1 > EnemyshipCounter)
                                EnemyshipCounter++;

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("[O]");
                            keepGoing = false;
                        }

                        if (keepGoing)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("[-]");
                        }


                        PrintStatistic(x, y, userFleet);


                        if (y == 10)
                        {
                            Console.Write("      ");
                            PrintMapOfEnemy(x, row, userFleet, enemyFleet, ref myShipCounter, ref enemyHitCounter);
                        }
                    }

                    Console.WriteLine();
                }

            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }
        static Position MaptoNumericCoordnates(string input, Dictionary<char, int> Coordinates)
        {
            Position pos = new Position();

            char[] inputSplit = input.ToUpper().ToCharArray();


            if (inputSplit.Length < 2 || inputSplit.Length > 4)
            {
                return pos;
            }




            if (Coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }


            if (inputSplit.Length == 3)
            {

                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }

            }


            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }

            return pos;
        }
        static void PrintMapOfEnemy(int x, char row, Fleet userFleet, Fleet enemyFleet, ref int MyshipCounter, ref int EnemyHitCounter)
        {
            List<Position> EnemyFirePositions = new List<Position>();
            row--;
            Random random = new Random();
            List<Position> SortedLFirePositions = enemyFleet.FirePositions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedLShipsPositions = userFleet.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            try
            {

                for (int y = 1; y < 11; y++)
                {
                    bool keepGoing = true;

                    #region row indicator
                    if (y == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[" + row + "]");
                        row++;
                    }
                    #endregion


                    if (SortedLFirePositions.Count != 0 && SortedLFirePositions[EnemyHitCounter].x == x && SortedLFirePositions[EnemyHitCounter].y == y)
                    {

                        if (SortedLFirePositions.Count - 1 > EnemyHitCounter)
                            EnemyHitCounter++;

                        if (userFleet.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                        {

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[*]");
                            keepGoing = false;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("[X]");
                            keepGoing = false;
                        }
                    }

                    if (keepGoing && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[MyshipCounter].x == x && SortedLShipsPositions[MyshipCounter].y == y)
                    {
                        if (SortedLShipsPositions.Count - 1 > MyshipCounter)
                            MyshipCounter++;

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");
                        keepGoing = false;
                    }

                    if (keepGoing)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[-]");
                    }
                    PrintStatistic(x, y, enemyFleet);

                }
            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }
        static void printFirstLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[ ]");
            for (int i = 1; i < 11; i++)
                Console.Write("[" + i + "]");
        }
        static void Commentator(Fleet battleShip, bool isMyShip)
        {
            string title = isMyShip ? "Your" : "Enemy";

            if (battleShip.CheckPBattleship && battleShip.IsBattleshipSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(battleShip.Battleship));
                battleShip.CheckPBattleship = false;
            }

            if (battleShip.CheckDestroyerA && battleShip.IsDestroyerASunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(battleShip.DestroyerA));
                battleShip.CheckDestroyerA = false;
            }

            if (battleShip.CheckDestroyerB && battleShip.IsDestroyerBSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(battleShip.CheckDestroyerB));
                battleShip.CheckDestroyerB = false;
            }
        }
        static void PrintStatistic(int x, int y, Fleet fleet)
        {
            if (x == 1 && y == 10)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Fleet Details:");
            }


            if (x == 2 && y == 10)
            {
                if (fleet.IsBattleshipSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("BattleShip [5]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("BattleShip [5]");
                }
            }

            if (x == 3 && y == 10)
            {
                if (fleet.IsDestroyerASunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("DestroyerA [4]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("DestroyerA [4]");
                }
            }

            if (x == 4 && y == 10)
            {

                if (fleet.IsDestroyerBSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("DestroyerB [4]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("DestroyerB [4]");
                }
            }

            if ((x == 5 || x == 6 || x > 6) && y == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Console.Write(" ");
                }
            }

        }

    }
}


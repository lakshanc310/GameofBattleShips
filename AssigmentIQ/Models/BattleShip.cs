using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssigmentIQ.Models
{
    public class Fleet
    {
        Random random = new Random();
        private const int BATTLESHIP = 5;
        private const int DESTROYERA = 4;
        private const int DESTROYERB = 4;

        public Fleet()
        {
            Battleship = GeneratePosistion(BATTLESHIP);
            DestroyerA = GeneratePosistion(DESTROYERA);
            DestroyerB = GeneratePosistion(DESTROYERB);
        }

        public int StepsTaken { get; set; } = 0;
        public List<Position> Battleship { get; set; }
        public List<Position> DestroyerA { get; set; }
        public List<Position> DestroyerB { get; set; }
        public List<Position> AllShipsPosition { get; set; } = new List<Position>();
        public List<Position> FirePositions { get; set; } = new List<Position>();

        public bool IsBattleshipSunk { get; set; } = false;
        public bool IsDestroyerASunk { get; set; } = false;
        public bool IsDestroyerBSunk { get; set; } = false;
        public bool IsDestroyedAll { get; set; } = false;

        public bool CheckPBattleship { get; set; } = true;
        public bool CheckDestroyerA { get; set; } = true;
        public bool CheckDestroyerB { get; set; } = true;
        public List<Position> GeneratePositionRandomly(int size)
        {
            List<Position> positions = new List<Position>();

            int direction = random.Next(1, size);
            int row = random.Next(1, 11);
            int col = random.Next(1, 11);

            if (direction % 2 != 0)//travel horizontically
            {
                if (row - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row - i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row + i;
                        pos.y = col;
                        positions.Add(pos);
                    }
                }
            }
            else//travel Vertically
            {
                if (col - size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row;
                        pos.y = col - i;
                        positions.Add(pos);
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        Position pos = new Position();
                        pos.x = row;
                        pos.y = col + i;
                        positions.Add(pos);
                    }
                }
            }
            return positions;
        }
        public List<Position> GeneratePosistion(int size)
        {
            List<Position> positions = new List<Position>();

            bool IsExist = false;

            do
            {
                positions = GeneratePositionRandomly(size);
                IsExist = positions.Where(AP => AllShipsPosition.Exists(ShipPos => ShipPos.x == AP.x && ShipPos.y == AP.y)).Any();
            }
            while (IsExist);

            AllShipsPosition.AddRange(positions);


            return positions;
        }
        public Fleet CheckShipStatus(List<Position> HitPositions)
        {
            IsBattleshipSunk = Battleship.Where(B => !HitPositions.Any(H => B.x == H.x && B.y == H.y)).ToList().Count == 0;
            IsDestroyerASunk = DestroyerA.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;
            IsDestroyerBSunk = DestroyerB.Where(D => !HitPositions.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;

            IsDestroyedAll = IsBattleshipSunk && IsDestroyerASunk && IsDestroyerBSunk;
            return this;
        }
        public Fleet Fire()
        {
            Position EnemyShotPos = new Position();
            bool alreadyShot = false;
            do
            {
                EnemyShotPos.x = random.Next(1, 11);
                EnemyShotPos.y = random.Next(1, 11);
                alreadyShot = FirePositions.Any(EFP => EFP.x == EnemyShotPos.x && EFP.y == EnemyShotPos.y);
            }
            while (alreadyShot);

            FirePositions.Add(EnemyShotPos);
            return this;
        }
    }
}

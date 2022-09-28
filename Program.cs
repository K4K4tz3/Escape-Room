using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Escape_Room
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Init Room + randomNumber
            Random randomNumber = new Random();
            Vector2 oldPlayerPosition = new Vector2(0, 0);
            Vector2 newPlayerPosition = new Vector2(0, 0);
            CreateRoom(out int[,] room, randomNumber, out oldPlayerPosition);
            bool carryKey = false;
            #endregion
            int[,,] maze = new int[7, 7, 3] { { {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {3, 0, 0}, {4, 0, 0} },
                                              { {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0} },
                                              { {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {3, 0, 0}, {4, 0, 0}, {3, 0, 0}, {4, 0, 0} },
                                              { {4, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 1}, {3, 0, 0}, {3, 0, 0}, {4, 0, 0} },
                                              { {4, 0, 0}, {3, 0, 0}, {4, 0, 0}, {3, 0, 0}, {4, 0, 0}, {3, 0, 0}, {4, 0, 0} },
                                              { {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, {3, 0, 0}, },
                                              { {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, {4, 0, 0}, } };

            string inputstr = Console.ReadLine();
            int input = Int16.Parse(inputstr);

            #region GameLoop
            if (input == 1)
            {
                while (true)
                {
                    Console.Clear();
                    PrintRoom(room);
                    Console.ForegroundColor = ConsoleColor.Black;
                    ConsoleKeyInfo pressedKey = Console.ReadKey();
                    newPlayerPosition = CheckKeyPress(pressedKey, room, oldPlayerPosition, newPlayerPosition, ref carryKey);
                    UpdatePlayerPosition(ref room, ref oldPlayerPosition, newPlayerPosition);
                }
            }
            else if (input == 2)
            {
                SetPlayerPosition(ref oldPlayerPosition, 3, 3);
                SetPlayerPosition(ref newPlayerPosition, 3, 3);
                while (true)
                {
                    Console.Clear();
                    FogOfWarUpdater(ref maze, newPlayerPosition);
                    UpdatePlayerPosition(ref maze, ref oldPlayerPosition, newPlayerPosition);
                    PrintRoom(maze);
                    Console.ForegroundColor = ConsoleColor.Black;
                    ConsoleKeyInfo pressedKey = Console.ReadKey();
                    newPlayerPosition = CheckKeyPress(pressedKey, maze, oldPlayerPosition, newPlayerPosition, ref carryKey);
                }
            }
            #endregion
        }
        #region Build v1
        static void CreateRoom(out int[,] room, Random randomNumber, out Vector2 PlayerPosition)
        {
            room = new int[randomNumber.Next(5, 11), randomNumber.Next(11, 21)];
            // Room detail's:
            // 0 -> roomGround; 1 -> roomWall; 2 -> hallwayGround; 3 -> hallwayWall; 4 -> LockedDoor; 5 -> Key; 6 -> Player
            int xLength = room.GetLength(1);
            int yLength = room.GetLength(0);

            //Wall placement
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (x == 0 || x == xLength - 1 || y == 0 || y == yLength - 1) room[y, x] = 1;
                }
            }
            //Key placement
            room[randomNumber.Next(1, yLength - 1), randomNumber.Next(1, xLength - 1)] = 5;
            //Door placement
            int xSave;
            int ySave;
            switch (randomNumber.Next(1, 4))
            {
                case 1:
                    xSave = 0;
                    ySave = randomNumber.Next(1, yLength - 1);
                    room[ySave, xSave] = 4;
                    break;
                case 2:
                    xSave = xLength - 1;
                    ySave = randomNumber.Next(1, yLength - 1);
                    room[ySave, xSave] = 4;
                    break;
                case 3:
                    xSave = randomNumber.Next(1, xLength - 1);
                    ySave = 0;
                    room[ySave, xSave] = 4;
                    break;
                case 4:
                    xSave = randomNumber.Next(1, xLength - 1);
                    ySave = yLength - 1;
                    room[ySave, xSave] = 4;
                    break;
            }
            //Player placement
            do
            {
                xSave = randomNumber.Next(1, --xLength);
                ySave = randomNumber.Next(1, --yLength);
                if (room[ySave, xSave] == 0)
                {
                    room[ySave, xSave] = 6;
                    PlayerPosition.X = xSave;
                    PlayerPosition.Y = ySave;
                    break;
                }
            } while (true);
        }
        static void UpdatePlayerPosition(ref int[,] room, ref Vector2 oldPlayerPosition, Vector2 newPlayerPosition)
        {
            room[(int)oldPlayerPosition.Y, (int)oldPlayerPosition.X] = 0;
            room[(int)newPlayerPosition.Y, (int)newPlayerPosition.X] = 6;
            oldPlayerPosition = newPlayerPosition;
        }
        static void PrintRoom(int[,] room)
        {
            Console.WriteLine();
            for (int y = 0; y < room.GetLength(0); y++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
                for (int x = 0; x < room.GetLength(1); x++)
                {
                    switch (room[y, x])
                    {
                        case 0:
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.Write(" ");
                            break;
                        case 1:
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.Write(" ");
                            break;
                        case 4:
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.Write(" ");
                            break;
                        case 5:
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.Write(" ");
                            break;
                        case 6:
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("@");
                            break;
                    }
                    if (x == 0 || x == room.GetLength(1)) Console.BackgroundColor = ConsoleColor.Gray;
                    else Console.BackgroundColor = ConsoleColor.Black;

                    //Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        static Vector2 CheckKeyPress(ConsoleKeyInfo keyInfo, int[,] room, Vector2 oldPlayerPosition, Vector2 newPlayerPosition, ref bool ownKey)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.W:
                    newPlayerPosition.X = oldPlayerPosition.X;
                    newPlayerPosition.Y = oldPlayerPosition.Y - 1;
                    break;
                case ConsoleKey.A:
                    newPlayerPosition.X = oldPlayerPosition.X - 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.D:
                    newPlayerPosition.X = oldPlayerPosition.X + 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.S:
                    newPlayerPosition.X = oldPlayerPosition.X;
                    newPlayerPosition.Y = oldPlayerPosition.Y + 1;
                    break;
            }
            Vector2 position = new Vector2((int)newPlayerPosition.Y, (int)newPlayerPosition.X);
            try
            {
                switch (room[(int)newPlayerPosition.Y, (int)newPlayerPosition.X])
                {
                    case 0:
                        return newPlayerPosition;
                    case 1:
                        return oldPlayerPosition;
                    case 4:
                        if (ownKey) return newPlayerPosition;
                        return oldPlayerPosition;
                    case 5:
                        ownKey = true;
                        return newPlayerPosition;
                }
            }
            catch
            {
                return oldPlayerPosition;
            }
            return oldPlayerPosition;
        }
        #endregion
        #region Build v2
        static void CreateRoom(out int[,,] room, Random randomNumber, out Vector2 PlayerPosition)
        {
            room = new int[randomNumber.Next(5, 11), randomNumber.Next(11, 21), 3];
            // Room detail's:
            // room[y, x, 0]: TileInfo:       0 -> empty; 1 -> roomGround; 2 -> roomWall; 3 -> hallwayGround, 4 -> hallwayWall; 5 -> lockedDoor; 6 -> unlockedDoor; 7 -> Key
            // room[y, x, 1]: Visibility:     0 -> notVisible; 1 -> Visible
            // room[y, x, 2]: isPlayerOnTile: 0 -> no; 1 -> yes
            int xLength = room.GetLength(1);
            int yLength = room.GetLength(0);

            //Wall placement
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (x == 0 || x == xLength - 1 || y == 0 || y == yLength - 1) room[y, x, 0] = 1;
                    room[y, x, 1] = 0;
                    room[y, x, 2] = 0;
                }
            }
            //Key placement
            room[randomNumber.Next(1, yLength - 1), randomNumber.Next(1, xLength - 1), 0] = 5;
            //Door placement
            int xSave;
            int ySave;
            switch (randomNumber.Next(1, 4))
            {
                case 1:
                    xSave = 0;
                    ySave = randomNumber.Next(1, yLength - 1);
                    room[ySave, xSave, 0] = 4;
                    break;
                case 2:
                    xSave = xLength - 1;
                    ySave = randomNumber.Next(1, yLength - 1);
                    room[ySave, xSave, 0] = 4;
                    break;
                case 3:
                    xSave = randomNumber.Next(1, xLength - 1);
                    ySave = 0;
                    room[ySave, xSave, 0] = 4;
                    break;
                case 4:
                    xSave = randomNumber.Next(1, xLength - 1);
                    ySave = yLength - 1;
                    room[ySave, xSave, 0] = 4;
                    break;
            }
            //Player placement
            do
            {
                xSave = randomNumber.Next(1, --xLength);
                ySave = randomNumber.Next(1, --yLength);
                if (room[ySave, xSave, 0] == 1)
                {
                    room[ySave, xSave, 2] = 1;
                    PlayerPosition.X = xSave;
                    PlayerPosition.Y = ySave;
                    break;
                }
            } while (true);
        }
        static void PrintRoom(int[,,] room)
        {
            Console.WriteLine();
            for (int y = 0; y < room.GetLength(0); y++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
                for (int x = 0; x < room.GetLength(1); x++)
                {
                    if (room[y, x, 1] == 1)
                    {
                        if (room[y, x, 2] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("@");
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            switch (room[y, x, 0])
                            {
                                case 0:
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    break;
                                case 1:
                                case 3:
                                    Console.BackgroundColor = ConsoleColor.Gray;
                                    break;
                                case 2:
                                case 4:
                                    Console.BackgroundColor = ConsoleColor.DarkGray;
                                    break;
                                case 5:
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    break;
                                case 6:
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    break;
                                case 7:
                                    Console.BackgroundColor = ConsoleColor.Yellow;
                                    break;
                            }
                            Console.Write(" ");
                            //if (x == 0 || x == room.GetLength(1)) Console.BackgroundColor = ConsoleColor.Gray;
                            //else Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }
        static void SetPlayerPosition(ref Vector2 playerPosition, int x, int y)
        {
            playerPosition.X = x;
            playerPosition.Y = y;
        }
        static void FogOfWarUpdater(ref int[,,] room, Vector2 playerPosition)
        {
            List<Vector2> tileList = new List<Vector2>() { playerPosition };
            List<Vector2> tileCheck = new List<Vector2>() { new Vector2(0, -1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(1, 0) };
            List<Vector2> bufferList = new List<Vector2>();
            Vector2 tileSave;
            do
            {
                bufferList.Clear();
                foreach (Vector2 tile in tileList)
                {
                    switch (room[(int)tile.Y, (int)tile.X, 0])
                    {
                        case 1:
                        case 3:
                            room[(int)tile.Y, (int)tile.X, 1] = 1;
                            tileSave = tile;
                            foreach (Vector2 tileCoords in tileCheck)
                            {
                                tile = tileSave;
                                if (room[(int)tile.Y + (int)tileCoords.Y, (int)tile.X + (int)tileCoords.X, 0] == 1 || room[(int)tile.Y + (int)tileCoords.Y, (int)tile.X + (int)tileCoords.X, 0] == 3) bufferList.Add(tile);
                            }
                            break;
                        case 2:
                        case 4:
                            room[(int)tile.Y, (int)tile.X, 1] = 1;
                            break;
                    }

                }
                tileList.Clear();
                tileList = bufferList;
            } while (tileList.Count() > 0);
        }
        static void CheckTiles(ref List<Vector2> bufferList, Vector2 currentTile, int[,,] room)
        {
            List<Vector2> tileCheck = new List<Vector2>() { new Vector2(0, -1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(1, 0) };

            foreach (Vector2 tileCoords in tileCheck)
            {
                if (room[(int)currentTile.Y + (int)tileCoords.Y, (int)currentTile.X + (int)tileCoords.X, 0] == 1 || room[(int)currentTile.Y + (int)tileCoords.Y, (int)currentTile.X + (int)tileCoords.X, 0] == 3) bufferList.Add(currentTile);
            }
        }
        static Vector2 CheckKeyPress(ConsoleKeyInfo keyInfo, int[,,] room, Vector2 oldPlayerPosition, Vector2 newPlayerPosition, ref bool ownKey)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.W:
                    newPlayerPosition.X = oldPlayerPosition.X;
                    newPlayerPosition.Y = oldPlayerPosition.Y - 1;
                    break;
                case ConsoleKey.A:
                    newPlayerPosition.X = oldPlayerPosition.X - 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.D:
                    newPlayerPosition.X = oldPlayerPosition.X + 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.S:
                    newPlayerPosition.X = oldPlayerPosition.X;
                    newPlayerPosition.Y = oldPlayerPosition.Y + 1;
                    break;
            }
            Vector2 position = new Vector2((int)newPlayerPosition.Y, (int)newPlayerPosition.X);
            try
            {
                switch (room[(int)newPlayerPosition.Y, (int)newPlayerPosition.X, 0])
                {
                    case 1:
                    case 3:
                        return newPlayerPosition;
                    case 0:
                    case 2:
                    case 4:
                        return oldPlayerPosition;
                    case 5:
                        if (ownKey) return newPlayerPosition;
                        return oldPlayerPosition;
                    case 7:
                        ownKey = true;
                        return newPlayerPosition;
                }
            }
            catch
            {
                return oldPlayerPosition;
            }
            return oldPlayerPosition;
        }
        static void UpdatePlayerPosition(ref int[,,] room, ref Vector2 oldPlayerPosition, Vector2 newPlayerPosition)
        {
            room[(int)oldPlayerPosition.Y, (int)oldPlayerPosition.X, 2] = 0;
            room[(int)newPlayerPosition.Y, (int)newPlayerPosition.X, 2] = 1;
            oldPlayerPosition = newPlayerPosition;
        }
        #endregion
    }
}

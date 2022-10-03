using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Escape_Room
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Init Room + randomNumber
            Random randomNumber = new Random();
            Vector2 oldPlayerPosition;
            Vector2 newPlayerPosition = new Vector2(0, 0);
            Vector2 doorPosition;
            Console.CursorVisible = false;
            #endregion
            #region GameLoop
            PrintText(new List<string>() { "Dear Traveler.", "You arrived.", 
                                           "Your mission is to escape this room.", 
                                           "Do that by using wasd to move around.", 
                                           "The question is how do you escape?", 
                                           "But that is up to you..." }            );
            for(int i = 0; i < randomNumber.Next(3,9); i++)
            {
                CreateRoom(out int[,] room, randomNumber, out oldPlayerPosition, out doorPosition);
                bool carryKey = false;
                bool loop = true;
                if(i > 0)
                {
                    Thread.Sleep(600);
                    Console.Clear();
                    PrintText("HAHAHAHahaha...", new List<string> { "You really thought that would be so easy?", 
                                                                    "Good luck next time!", 
                                                                    "You will never escape!" }, randomNumber);
                }
                while (loop)
                {
                    Console.Clear();
                    PrintRoom(room);
                    Console.ForegroundColor = ConsoleColor.Black;
                    ConsoleKeyInfo pressedKey = Console.ReadKey();
                    newPlayerPosition = CheckKeyPress(pressedKey, room, oldPlayerPosition, newPlayerPosition, ref carryKey);
                    UpdatePlayerPosition(ref room, ref oldPlayerPosition, newPlayerPosition);
                    if (newPlayerPosition == doorPosition) loop = false;
                }
            }
            Thread.Sleep(1200);
            Console.Clear();
            PrintText(new List<string>() { "You've escaped.", 
                                           "You did well for such a primitive being...",
                                           "Farewell and good luck for you so you won't get captured again." });
            #endregion
        }
        static void CreateRoom(out int[,] room, Random randomNumber, out Vector2 PlayerPosition, out Vector2 doorPosition)
        {
            room = new int[randomNumber.Next(11, 21), randomNumber.Next(11, 21)];
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
            int xSave = 0;
            int ySave = 0;
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
            doorPosition.X = xSave;
            doorPosition.Y = ySave;
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
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
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
                case ConsoleKey.UpArrow:
                    newPlayerPosition.X = oldPlayerPosition.X;
                    newPlayerPosition.Y = oldPlayerPosition.Y - 1;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    newPlayerPosition.X = oldPlayerPosition.X - 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    newPlayerPosition.X = oldPlayerPosition.X + 1;
                    newPlayerPosition.Y = oldPlayerPosition.Y;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
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
        static void PrintText(List<string> text)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            foreach(string line in text)
            {
                foreach(char letter in line)
                {
                    Console.Write(letter);
                    Thread.Sleep(100);
                }
                Thread.Sleep(600);
                Console.WriteLine();
            }
        }
        static void PrintText(string laughter, List<string> text, Random randomNumber)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (char letter in laughter)
            {
                Console.Write(letter);
                Thread.Sleep(75);
            }
            Thread.Sleep(600);
            Console.WriteLine();
            foreach (char letter in text[randomNumber.Next(text.Count() - 1)])
            {
                Console.Write(letter);
                Thread.Sleep(100);
            }
            Thread.Sleep(600);
            Console.WriteLine();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chase_The_Star
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.Title = "Chase The Star";
            Console.Clear();
            Console.CursorVisible = false;
            Game chaseTheStar = new Game();
        }

    }
    class Game
    {
        
        public Tuple<int, int> StarCords { get; protected set; }

        public Game()
        {
        Start:
            Random rng = new Random();         
            Snake snk = new Snake();           

            getStarCords(rng,snk);
            //setStarPosition();
            string s = "PRESS ANY KEY TO START";
            Console.SetCursorPosition(40 - s.Length / 2, 12);
            Console.WriteLine(s);
            Console.ReadKey(true);
            Tick(snk, rng);

            Console.Clear();
            Console.SetCursorPosition(36, 12);
            Console.WriteLine("GAME OVER");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Again? Y/N");
            
            if (Console.ReadKey(true).Key == ConsoleKey.Y) { goto Start; }
        }

        void getStarCords(Random rng, Snake snk)
        {
            bool isValid = false;
            
            int starX=0;
            int starY=0;
            while (!isValid)
            {
                bool tempchecker = true;
                starX = rng.Next(0, Console.WindowWidth - 1);
                starY = rng.Next(0, Console.WindowHeight - 1);

                foreach (Segment seg in snk.Body)
                {
                    if (seg.Cords[0] == starX && seg.Cords[1] == starY) { tempchecker = false; }
                }
                isValid = tempchecker;
            }
            

            StarCords = Tuple.Create(starX, starY);
        }

        void setStarPosition()
        {
            Console.SetCursorPosition(StarCords.Item1, StarCords.Item2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("*");
            Console.ResetColor();
        }

        void Tick(Snake snk, Random rng) 
        {
            ConsoleKey lastKey=ConsoleKey.UpArrow;
            ConsoleKey move;
            Tuple<bool, ConsoleKey> moveResult;
            bool quit = false;
            while (!quit)
            {
                Console.Clear();

                setStarPosition();
                snk.Render();

                System.Threading.Thread.Sleep(100);

                move = Console.KeyAvailable ? Console.ReadKey(true).Key : lastKey;

                if ((lastKey == ConsoleKey.UpArrow && move == ConsoleKey.DownArrow) || (lastKey == ConsoleKey.DownArrow && move == ConsoleKey.UpArrow))
                {
                    move = lastKey;
                }
                if ((lastKey == ConsoleKey.LeftArrow && move == ConsoleKey.RightArrow) || (lastKey == ConsoleKey.RightArrow && move == ConsoleKey.LeftArrow))
                {
                    move = lastKey;
                }
                
                moveResult = snk.Move(move, lastKey);
                quit = moveResult.Item1;
                lastKey = moveResult.Item2 == ConsoleKey.NoName ? lastKey : moveResult.Item2;
                
                for (int j = 1; j < snk.Body.Count; j++) //Hit yourself
                {
                    if (snk.Body[j].Cords[0] == snk.Body[0].Cords[0] && snk.Body[j].Cords[1] == snk.Body[0].Cords[1])
                    {
                        quit = true;
                    }
                }
                if (snk.Body[0].Cords[0] == StarCords.Item1 && snk.Body[0].Cords[1] == StarCords.Item2) //Hit a star
                {
                    getStarCords(rng, snk);
                    snk.AddSegment(move);
                }

            }
        }
    }
    class Snake
    {
        public int Length { get; set; }
        public List<Segment> Body = new List<Segment>();

        public Snake()
        {
            Length = 0;
            Segment Head = new Segment(40, 12,'^');
            Segment Neck = new Segment(40, 13, '║');
            Body.Add(Head);
            Body.Add(Neck);
        }

        public void Render()
        {
            foreach (Segment s in Body)
            {
                Console.SetCursorPosition(s.Cords[0],s.Cords[1]);
                Console.Write(s.Image);
            }
        }

        public Tuple<bool, ConsoleKey> Move(ConsoleKey Key,ConsoleKey lastKey) //This calculates the new movement coordinates
        {
            if (Key.IN(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow))
            {
                for (int i = Body.Count - 1; i > 0; i--)
                {
                    Body[i].Cords[0] = Body[i - 1].Cords[0]; if (i > 1) { Body[i].Image = Body[i - 1].Image; } //Sets each segment to the previous one's cords
                    Body[i].Cords[1] = Body[i - 1].Cords[1];
                }
            }
            
            switch (Key) //Key input evaluated 
            {
                case ConsoleKey.Escape:
                    return Tuple.Create(true,Key);
                case ConsoleKey.UpArrow:
                    if (Body[0].Cords[1] > 0) { Body[0].Cords[1]--; Body[0].Image = '^'; Body[1].chooseImage(Key,lastKey); }
                    return Tuple.Create(false, Key);
                case ConsoleKey.DownArrow:
                    if (Body[0].Cords[1] < Console.WindowHeight - 1) { Body[0].Cords[1]++; Body[0].Image = 'V'; Body[1].chooseImage(Key,lastKey); }
                    return Tuple.Create(false, Key);
                case ConsoleKey.LeftArrow:
                    if (Body[0].Cords[0] > 0) { Body[0].Cords[0]--; Body[0].Image = '<'; Body[1].chooseImage(Key,lastKey); }
                    return Tuple.Create(false, Key);
                case ConsoleKey.RightArrow:
                    if (Body[0].Cords[0] < Console.WindowWidth - 1) { Body[0].Cords[0]++; Body[0].Image = '>'; Body[1].chooseImage(Key, lastKey); }
                    return Tuple.Create(false, Key);
                default:
                    return Tuple.Create(false, ConsoleKey.NoName);
            }

        }
        public void AddSegment(ConsoleKey move)
        {
            for (int i = 0; i < 3; i++)
            {
                Char newimg;
                if (Body[Body.Count - 1].Image.IN('╗', '╔')) { newimg = '║'; }
                else if (Body[Body.Count - 1].Image.IN('╝', '╚')) { newimg = '═'; }
                else { newimg = Body[Body.Count - 1].Image; }

                Body.Add(new Segment(Body[Length].Cords[0], Body[Length].Cords[1], newimg));
                Length++;
            }
        }
    }
    class Segment
    {
        public int[] Cords { get; protected set; }
        public char Image { get; set; }
        public Segment(int x,int y,Char img)
        {
            Cords = new int[2] { x, y };
            Image = img;
        }

        public void chooseImage(ConsoleKey key, ConsoleKey lastKey) //Handles Corner pieces
        {
            switch (lastKey)
            {
                case ConsoleKey.UpArrow:
                    if (key == ConsoleKey.LeftArrow) { Image = '╗'; } 
                    else if (key == ConsoleKey.RightArrow) { Image = '╔'; }
                    else { Image = '║'; }
                    break;
                case ConsoleKey.DownArrow:
                    if (key == ConsoleKey.LeftArrow) { Image = '╝'; }
                    else if (key == ConsoleKey.RightArrow) { Image = '╚'; }
                    else { Image = '║'; }
                    break;
                case ConsoleKey.LeftArrow:
                    if (key == ConsoleKey.UpArrow) { Image = '╚'; }
                    else if (key == ConsoleKey.DownArrow) { Image = '╔'; }
                    else { Image = '═'; }
                    break;
                case ConsoleKey.RightArrow:
                    if (key == ConsoleKey.UpArrow) { Image = '╝'; }
                    else if (key == ConsoleKey.DownArrow) { Image = '╗'; }
                    else { Image = '═'; }
                    break;
            }
        }
    }

    public static class Extensions
    {
        public static bool IN<T> (this T item, params T[] args)
        {
            return args.Contains(item);
        }
    }
}

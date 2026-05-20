using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int width = 30;
        static int height = 20;

        static List<(int x, int y)> snake = new();
        static (int x, int y) food;
        static (int dx, int dy) dir = (1, 0);
        static (int dx, int dy) nextDir = (1, 0);
        static int score = 0;
        static bool running = true;
        static Random rng = new();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "Snake";

            while (true)
            {
                InitGame();
                GameLoop();
                Console.Clear();
                Console.SetCursorPosition(width / 2 - 4, height / 2);
                Console.WriteLine($"Game Over! Score: {score}");
                Console.SetCursorPosition(width / 2 - 8, height / 2 + 1);
                Console.WriteLine("Press R to restart or Q to quit");

                while (true)
                {
                    var k = Console.ReadKey(true).Key;
                    if (k == ConsoleKey.R) break;
                    if (k == ConsoleKey.Q) return;
                }
            }
        }

        static void InitGame()
        {
            snake = new List<(int, int)>
            {
                (width / 2,     height / 2),
                (width / 2 - 1, height / 2),
                (width / 2 - 2, height / 2),
            };
            dir = (1, 0);
            nextDir = (1, 0);
            score = 0;
            running = true;
            SpawnFood();
            Console.Clear();
            DrawBorder();
        }

        static void SpawnFood()
        {
            do
            {
                food = (rng.Next(1, width - 1), rng.Next(1, height - 1));
            } while (snake.Contains(food));
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int x = 0; x < width; x++)
            {
                Console.SetCursorPosition(x, 0);      Console.Write('#');
                Console.SetCursorPosition(x, height);  Console.Write('#');
            }
            for (int y = 0; y <= height; y++)
            {
                Console.SetCursorPosition(0, y);          Console.Write('#');
                Console.SetCursorPosition(width - 1, y);  Console.Write('#');
            }
            Console.ResetColor();
        }

        static void GameLoop()
        {
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    nextDir = key switch
                    {
                        ConsoleKey.UpArrow    or ConsoleKey.W when dir.dy != 1  => (0, -1),
                        ConsoleKey.DownArrow  or ConsoleKey.S when dir.dy != -1 => (0, 1),
                        ConsoleKey.LeftArrow  or ConsoleKey.A when dir.dx != 1  => (-1, 0),
                        ConsoleKey.RightArrow or ConsoleKey.D when dir.dx != -1 => (1, 0),
                        _ => nextDir
                    };
                }

                dir = nextDir;

                var head = (snake[0].x + dir.dx, snake[0].y + dir.dy);

                if (head.Item1 <= 0 || head.Item1 >= width - 1 ||
                    head.Item2 <= 0 || head.Item2 >= height)
                {
                    running = false;
                    break;
                }

                if (snake.Contains(head))
                {
                    running = false;
                    break;
                }

                snake.Insert(0, head);

                bool ate = head == food;
                if (ate)
                {
                    score++;
                    SpawnFood();
                    DrawFood();
                }
                else
                {
                    var tail = snake[^1];
                    snake.RemoveAt(snake.Count - 1);
                    Console.SetCursorPosition(tail.x, tail.y);
                    Console.Write(' ');
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(snake[1].x, snake[1].y);
                Console.Write('O');
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.SetCursorPosition(snake[0].x, snake[0].y);
                Console.Write('@');
                Console.ResetColor();

                Console.SetCursorPosition(0, height + 1);
                Console.Write($"Score: {score}   ");

                Thread.Sleep(120);
            }
        }

        static void DrawFood()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(food.x, food.y);
            Console.Write('*');
            Console.ResetColor();
        }
    }
}

using System;
using System.Diagnostics;

namespace RaycasterApp;

class Program
{
    static readonly int mapWidth = 8;
    static readonly int mapHeight = 8;
    static char[,] map = new char[mapHeight, mapWidth];

    static void Main(string[] args)
    {
        GenerateMap();

        double posX = 3.5, posY = 3.5;
        double dirX = -1, dirY = 0;
        double planeX = 0, planeY = 0.66;

        const double baseMoveSpeed = 5.0;
        const double baseRotSpeed = 3.0;

        var stopwatch = Stopwatch.StartNew();
        double previousTime = stopwatch.Elapsed.TotalSeconds;

        Console.Clear();

        while (true)
        {
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;

            screenWidth = Math.Max(screenWidth, 30);
            screenHeight = Math.Max(screenHeight, 15);

            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - previousTime;
            previousTime = currentTime;

            double moveSpeed = baseMoveSpeed * deltaTime;
            double rotSpeed = baseRotSpeed * deltaTime;

            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.W)
                {
                    double newX = posX + dirX * moveSpeed;
                    double newY = posY + dirY * moveSpeed;

                    if (GetMap((int)posY, (int)newX) != '#') posX = newX;
                    if (GetMap((int)newY, (int)posX) != '#') posY = newY;
                }
                else if (key == ConsoleKey.S)
                {
                    double newX = posX - dirX * moveSpeed;
                    double newY = posY - dirY * moveSpeed;

                    if (GetMap((int)posY, (int)newX) != '#') posX = newX;
                    if (GetMap((int)newY, (int)posX) != '#') posY = newY;
                }
                else if (key == ConsoleKey.A)
                {
                    double oldDirX = dirX;
                    dirX = dirX * Math.Cos(rotSpeed) - dirY * Math.Sin(rotSpeed);
                    dirY = oldDirX * Math.Sin(rotSpeed) + dirY * Math.Cos(rotSpeed);

                    double oldPlaneX = planeX;
                    planeX = planeX * Math.Cos(rotSpeed) - planeY * Math.Sin(rotSpeed);
                    planeY = oldPlaneX * Math.Sin(rotSpeed) + planeY * Math.Cos(rotSpeed);
                }
                else if (key == ConsoleKey.D)
                {
                    double oldDirX = dirX;
                    dirX = dirX * Math.Cos(-rotSpeed) - dirY * Math.Sin(-rotSpeed);
                    dirY = oldDirX * Math.Sin(-rotSpeed) + dirY * Math.Cos(-rotSpeed);

                    double oldPlaneX = planeX;
                    planeX = planeX * Math.Cos(-rotSpeed) - planeY * Math.Sin(-rotSpeed);
                    planeY = oldPlaneX * Math.Sin(-rotSpeed) + planeY * Math.Cos(-rotSpeed);
                }
                else if (key == ConsoleKey.Escape)
                {
                    Console.SetCursorPosition(0, screenHeight + 1);
                    return;
                }
            }

            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < screenHeight; y++)
            {
                string row = "";
                for (int x = 0; x < screenWidth; x++)
                {
                    double cameraX = 2 * x / (double)screenWidth - 1;
                    double rayDirX = dirX + planeX * cameraX;
                    double rayDirY = dirY + planeY * cameraX;

                    int mapX = (int)posX;
                    int mapY = (int)posY;

                    double deltaDistX = Math.Abs(1 / (rayDirX == 0 ? 1e-6 : rayDirX));
                    double deltaDistY = Math.Abs(1 / (rayDirY == 0 ? 1e-6 : rayDirY));

                    double sideDistX;
                    double sideDistY;

                    int stepX, stepY;
                    bool hit = false;
                    bool side = false;

                    if (rayDirX < 0)
                    {
                        stepX = -1;
                        sideDistX = (posX - mapX) * deltaDistX;
                    }
                    else
                    {
                        stepX = 1;
                        sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                    }

                    if (rayDirY < 0)
                    {
                        stepY = -1;
                        sideDistY = (posY - mapY) * deltaDistY;
                    }
                    else
                    {
                        stepY = 1;
                        sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                    }

                    while (!hit)
                    {
                        if (sideDistX < sideDistY)
                        {
                            sideDistX += deltaDistX;
                            mapX += stepX;
                            side = false;
                        }
                        else
                        {
                            sideDistY += deltaDistY;
                            mapY += stepY;
                            side = true;
                        }

                        if (mapX < 0 || mapX >= mapWidth || mapY < 0 || mapY >= mapHeight)
                            break;

                        if (GetMap(mapY, mapX) == '#')
                            hit = true;
                    }

                    double perpWallDist = side
                        ? (mapY - posY + (1 - stepY) / 2.0) / (rayDirY == 0 ? 1e-6 : rayDirY)
                        : (mapX - posX + (1 - stepX) / 2.0) / (rayDirX == 0 ? 1e-6 : rayDirX);

                    int lineHeight = (int)(screenHeight / perpWallDist);

                    if (y >= screenHeight / 2 - lineHeight / 2 && y <= screenHeight / 2 + lineHeight / 2)
                        row += side ? "|" : "#";
                    else
                        row += " ";
                }
                Console.WriteLine(row);
            }

            System.Threading.Thread.Sleep(10);
        }
    }

    static void GenerateMap()
    {
        Random rand = new Random();
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1)
                {
                    map[y, x] = '#';
                }
                else
                {
                    map[y, x] = rand.NextDouble() < 0.2 ? '#' : '.';
                }
            }
        }

        map[3, 3] = '.';
        map[3, 4] = '.';
        map[4, 3] = '.';
        map[4, 4] = '.';
    }

    static char GetMap(int y, int x) => map[y, x];
}

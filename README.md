# LightweightRaycasterEngine
Simple Raycasting engine that generates levels upon booting into the app. 

        -------------------
          Work In Progress
        -------------------
This is a simple and lightweight C# application written using .NET 8.0 


![Screenshot 2025-06-20 203223](https://github.com/user-attachments/assets/8d82003b-e7cf-4de8-89dd-2b2ea851a12f)

This Raycaster Engine doesn't use any 3D libraries so it can practically run on any potato you look at.

Everytime you open the app it will create a randomly generated map using the GenerateMap() function I will demonstrate here derived from the source:

    static void GenerateMap()
    {
        Random rand = new Random();
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1)
                {
                    map[y, x] = '#';               // # = wall | . = Empty Space
                }
                else
                {
                    map[y, x] = rand.NextDouble() < 0.2 ? '#' : '.';
                }
            }
        }

        map[3, 3] = '.';       
        map[3, 4] = '.';
        map[4, 3] = '.';          //Ensures there's still space for the player to move around in
        map[4, 4] = '.';
    }
        _______________________
        |                     |
        | How to use on Linux |
        |_____________________|

* In the same Directory the RaycasterEngine is in run this command

          chmod +x RaycasterEngine
  
Then run the executable

        ./RaycasterEngine

Then you have awesome game in your terminal!

It's a bit buggy and has a bit of delay in the movement, but i do plan on fixing that soon. 

I also plan on adding a map into the game to show where the randomly generated tiles are. 

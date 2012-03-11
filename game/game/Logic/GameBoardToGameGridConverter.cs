using System.Collections.Generic;
using Game.City_Generator;
using Game.Logic.Entities;

namespace Game.Logic
{
    static class GameBoardToGameGridConverter
    {
        /******************
        class consts
        ****************/

        const int TILE_SIZE_CONVERSION = 32;


        /******************
        Methods
        ****************/

        /*
         * This function converts a gameBaord to a grid. It creates a new grid, and populates it according to the 
         * building list in the original board. 
         */
        internal static Grid convert(GameBoard board)
        {
            int x = board.Length * TILE_SIZE_CONVERSION;
            int y = board.Width * TILE_SIZE_CONVERSION;
            Entity[,] gameGrid = intiateGrid(x,y);

            Grid grid = new Grid(gameGrid);
            foreach(Game.City_Generator.Building origin in board.Buildings)
            {
                Game.Logic.Entities.Building result = convertBuilding(origin);
                Area area = convertToArea(origin);
                grid.addEntity(result, area);
            }
            //TODO - insert police/other buildings
            return grid;
        }

        /*
         * converts a generation-building to a game-building
         */
        public static Game.Logic.Entities.Building convertBuilding(Game.City_Generator.Building build)
        {
            Vector realSize = new Vector(build.Length * TILE_SIZE_CONVERSION, build.Width * TILE_SIZE_CONVERSION);
            int sizeModifier = (build.Width * build.Length);
            //TODO - why do I need to flip x & y?!
            return new Game.Logic.Entities.CivilianBuilding(realSize,sizeModifier, getExitVector(build));
        }

        private static Vector getExitVector(City_Generator.Building build)
        {
            int x = 0 , y = 0;
            switch (build.ExitDirection)
            {
                case(0):
                    x = -1;
                    break;
                case (1):
                    x = 1;
                    break;
                case (2):
                    y = -1;
                    break;
                case (3):
                    y = 1;
                    break;
            }

            return new Vector(x * build.Dimensions.Length, y * build.Dimensions.Width);
        }

        /*
         * Finds the area of a generation bulding.
         */
        private static Area convertToArea(Game.City_Generator.Building build)
        {
            return new Area(new Point (build.StartX * TILE_SIZE_CONVERSION, build.StartY * TILE_SIZE_CONVERSION), new Vector(build.Length * TILE_SIZE_CONVERSION, build.Width * TILE_SIZE_CONVERSION));
        }


        private static Entity[,] intiateGrid(int x, int y)
        {
            Entity[,] gameGrid = new Entity[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    gameGrid[i, j] = null;
                }
            }
            return gameGrid;
        }
    }
}

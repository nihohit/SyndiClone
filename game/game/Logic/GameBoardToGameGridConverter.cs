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
        const int BASE_BUILD_REACTION_TIME = 200;
        const int BASE_BUILD_HEALTH = 10;


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
            return grid;
        }

        /*
         * This function is just the basic reaction function for the basic civic buildings.
         */
        public static Reaction civBuildReact(List<Entity> ent)
        {
            //TODO - missing function
            return new Reaction(null, Action.IGNORE);
        }

        /*
         * converts a generation-building to a game-building
         */
        public static Game.Logic.Entities.Building convertBuilding(Game.City_Generator.Building build)
        {
            Vector size = new Vector (build.Length, build.Width);
            Vector realSize = new Vector(size.X * TILE_SIZE_CONVERSION, size.Y * TILE_SIZE_CONVERSION);
            int sizeModifier = (size.X * size.Y);
            SFML.Graphics.Sprite sprite = new SFML.Graphics.Sprite(build.Img);
            //TODO - why do I need to flip x & y?!
            sprite.Position = new SFML.Graphics.Vector2(build.StartY * TILE_SIZE_CONVERSION, build.StartX * TILE_SIZE_CONVERSION);
            return new Game.Logic.Entities.Building(BASE_BUILD_REACTION_TIME / sizeModifier, civBuildReact, BASE_BUILD_HEALTH * sizeModifier, realSize, Affiliation.INDEPENDENT, Sight.instance(SightType.CIV_SIGHT), sprite);
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

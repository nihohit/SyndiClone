using System.Collections.Generic;
using Game.City_Generator;
using Game.Logic.Entities;

namespace Game.Logic
{
    static class GameBoardToGameGridConverter
    {
        const int TILE_SIZE_CONVERSION = 32;
        const int BASE_BUILD_REACTION_TIME = 200;
        const int BASE_BUILD_HEALTH = 10;

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

        public static Reaction civBuildReact(List<Entity> ent)
        {
            //TODO - missing function
            return new Reaction(null, Action.IGNORE);
        }

        private static Game.Logic.Entities.Building convertBuilding(Game.City_Generator.Building build)
        {
            Vector size = new Vector (build.Length, build.Width);
            Vector realSize = new Vector(size.X * TILE_SIZE_CONVERSION, size.Y * TILE_SIZE_CONVERSION);
            int sizeModifier = (size.X * size.Y);
            return new Game.Logic.Entities.Building(BASE_BUILD_REACTION_TIME / sizeModifier, civBuildReact, BASE_BUILD_HEALTH * sizeModifier, realSize, Affiliation.INDEPENDENT, Sight.instance(SightType.CIV_SIGHT));
        }

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

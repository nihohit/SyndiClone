using System.Collections.Generic;
using Game.City_Generator;
using Game.Logic.Entities;
using System;

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
            int y = board.Length * TILE_SIZE_CONVERSION;
            int x = board.Depth * TILE_SIZE_CONVERSION;

            Grid grid = new Grid(y,x);
            int amountOfPoliceBuildings = Convert.ToInt32( System.Math.Log(board.Buildings.Count, 2));
            int ratio = board.Buildings.Count / amountOfPoliceBuildings;
            int i = 0;

            foreach(Game.City_Generator.Building origin in board.Buildings)
            {
                Game.Logic.Entities.Building result = null;
                if (i != ratio)
                {
                    result = convertToCivilianBuilding(origin);
                    i++;
                }
                else
                {
                    i = 0;
                    result = convertToPoliceStation(origin);
                }
                Area area = convertToArea(origin);
                grid.addEntity(result, area);
            }

            grid.initialiseTerrainGrid();
            grid.initialiseExitPoints();
            //TODO - insert police/other buildings

            return grid;
        }

        private static Entities.Building convertToPoliceStation(City_Generator.Building build)
        {
            Vector realSize = new Vector((short) (build.Depth * TILE_SIZE_CONVERSION), (short) (build.Length * TILE_SIZE_CONVERSION));
            int sizeModifier = (build.Depth * build.Length);
            //TODO - why do I need to flip x & y?!
            return new Game.Logic.Entities.PoliceStation(realSize, sizeModifier, getExitVector(build));
        }

        /*
         * converts a generation-building to a game-building
         */
        public static Game.Logic.Entities.Building convertToCivilianBuilding(Game.City_Generator.Building build)
        {
            Vector realSize = new Vector((short) (build.Depth * TILE_SIZE_CONVERSION), (short) (build.Length * TILE_SIZE_CONVERSION));
            int sizeModifier = (build.Depth * build.Length);
            //TODO - why do I need to flip x & y?!
            return new Game.Logic.Entities.CivilianBuilding(realSize, sizeModifier, getExitVector(build));
        }

        private static Vector getExitVector(City_Generator.Building build)
        {
            int x = 0 , y = 0;
            switch (build.ExitDirection)
            {
                case(0):
                    y = -1;
                    break;
                case (1):
                    y = 1;
                    break;
                case (2):
                    x = -1;
                    break;
                case (3):
                    x = 1;
                    break;
            }


            //TODO - something here is wrong, and the default exit locations seem rather random.
            return new Vector(
                (short) (x * build.Dimensions.Length * TILE_SIZE_CONVERSION/2), 
                (short)(y * build.Dimensions.Depth * TILE_SIZE_CONVERSION/2));
        }

        /*
         * Finds the area of a generation bulding.
         */
        private static Area convertToArea(Game.City_Generator.Building build)
        {
            return new Area(
                new Point((short) (build.StartY * TILE_SIZE_CONVERSION), (short) (build.StartX * TILE_SIZE_CONVERSION)), 
                new Vector((short) (build.Length * TILE_SIZE_CONVERSION), (short) (build.Depth * TILE_SIZE_CONVERSION)));
        }

    }
}

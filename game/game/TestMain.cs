using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game {
  class main {
    static int Main(string[] args) {
      City_Generator.GameBoard city = City_Generator.CityFactory.createCity(30, 20);
      //City_Generator.GameBoard city = City_Generator.CityFactory.debugCity();

      //Image img = City_Generator.CityImageGenerator.convert_to_image(city);

      Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
      Buffers.InputBuffer input = new Buffers.InputBuffer();
      Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
      Logic.GameLogic logic = new Logic.GameLogic(disp, input, sound, city);
      city.Img.SaveToFile("result.jpg");
      Graphic_Manager.DisplayManager display = new Graphic_Manager.DisplayManager(30 * 32, 20 * 32, 32, disp, city.Img);
      List<Logic.Entities.ExternalEntity> buildings = new List<Logic.Entities.ExternalEntity>();
      foreach (City_Generator.Building build in city.Buildings) {
        buildings.Add(new Game.Logic.Entities.ExternalEntity(Logic.GameBoardToGameGridConverter.convertBuilding(build), new Vector(build.StartX * 32, build.StartY * 32)));
      }
      Logic.Entities.ExternalEntity civ = new Logic.Entities.ExternalEntity(new Logic.Entities.Civilian(), new Vector(0, 0));
      buildings.Add(civ);
      disp.receiveVisibleEntities(buildings);
      display.loop();
      List<Logic.BufferEvent> list = new List<Logic.BufferEvent>();
      Random generator = new Random();
      Vector nextLocation = new Vector(generator.Next(-2, 2), generator.Next(-2, 2));
      int check = 0;

      while (true) {
        if (check == 100) {
          check = 0;
          int x, y;
          if (civ.Position.X > 0) {
            x = generator.Next(-2, 2);
          } else {
            x = generator.Next(0, 2);
          }
          if (civ.Position.Y > 0) {
            y = generator.Next(-2, 2);
          } else {
            y = generator.Next(0, 2);
          }
          nextLocation = new Vector(x, y);
        }
        Console.WriteLine("move now " + nextLocation.ToString());
        Logic.Area area = new Logic.Area(new Point(civ.Position.X, civ.Position.Y), nextLocation);
        Logic.BufferEvent eventMove = new Logic.MoveEvent(area, civ, 1);
        list.Add(eventMove);
        disp.receiveActions(list);
        display.loop();
        list.Clear();
        civ.Position = new Vector(civ.Position.X + nextLocation.X, civ.Position.Y + nextLocation.Y);
        check++;
        for (int i = 0; i < 10000000; i++) {
          i++;
          i--;
        }
      }

      Console.ReadKey();
      //System.Console.ReadKey();
      return 1;
    }

  }
}
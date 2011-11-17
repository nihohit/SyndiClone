using System.Collections.Generic;
using SFML.Graphics;

namespace Game.Graphic_Manager
{
    class DisplayBuffer : Game.Buffers.Buffer
    {
        Dictionary<Game.Logic.Entities.Entity, SpriteLoop> objectFinder;
        List<Sprite> newSprites;
        List<Sprite> removedSprites;
        
        void Game.Buffers.Buffer.receiveInput(Game.Buffers.Input input)
        {
            
            //TODO - missing function
        }

        List<Sprite> getNewSprites()
        {
            List<Sprite> ans = new List<Sprite>(newSprites);
            newSprites.Clear();
            return ans;
        }

        List<Sprite> spritesToRemove()
        {
            List<Sprite> ans = new List<Sprite>(removedSprites);
            newSprites.Clear();
            return ans;
        }

        bool Game.Buffers.Buffer.readyForInput()
        {
            //TODO - missing function
            return false;
        }

        bool Game.Buffers.Buffer.readyToOutput()
        {
            //TODO - missing function
            return false;
        }

        LinkedList<Game.Buffers.Output> Game.Buffers.Buffer.returnOutput()
        {
            //TODO - missing function
            return null;
        }
    }
}

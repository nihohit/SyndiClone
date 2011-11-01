using System.Collections.Generic;
using SFML.Graphics;

namespace Game.Buffers
{
    class DisplayBuffer : Buffer
    {
        Dictionary<Game.Logic.Entities.Entity, Sprite> objectFinder;
        List<Sprite> newSprites;
        List<Sprite> removedSprites;
        
        void Buffer.receiveInput(Input input)
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

        bool Buffer.readyForInput()
        {
            //TODO - missing function
            return false;
        }

        bool Buffer.readyToOutput()
        {
            //TODO - missing function
            return false;
        }

        LinkedList<Output> Buffer.returnOutput()
        {
            //TODO - missing function
            return null;
        }
    }
}

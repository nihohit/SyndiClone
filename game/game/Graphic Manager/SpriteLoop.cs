
using System.Collections.Generic;

namespace Game.Graphic_Manager
{
    internal class SpriteLoop
    {
        LoopedList<SFML.Graphics.Sprite> _list;

        SpriteLoop(LinkedList<SFML.Graphics.Sprite> list)
        {
            this._list = new LoopedList<SFML.Graphics.Sprite>(list);
        }

        SFML.Graphics.Sprite getNextSprite()
        {
            return this._list.getNextValue();

        }

    }

    internal class LoopedList<T> //TODO - needs testing
    {
        private LinkedListNode<T> current;
        private LinkedList<SFML.Graphics.Sprite> list;
        
        internal LoopedList(LinkedList<T> list)
        {
            current = list.First;
            list.AddLast(list.First);
        }

        internal T getNextValue()
        {
            current = current.Next;
            return current.Value;
        }


    }
}

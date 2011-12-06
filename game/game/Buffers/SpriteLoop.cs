using System.Collections.Generic;

namespace Game.Buffers
{
    internal class SpriteLoop
    {
        private readonly LoopedList<SFML.Graphics.Sprite> _list;

        internal SpriteLoop(LinkedList<SFML.Graphics.Sprite> list)
        {
            this._list = new LoopedList<SFML.Graphics.Sprite>(list);
        }

        internal SFML.Graphics.Sprite Next()
        {
            this._list.next();
            return this.getSprite();

        }

        internal SFML.Graphics.Sprite getSprite()
        {
            return this._list.getValue();

        }

    }

    internal class LoopedList<T> //TODO - needs testing
    {
        private LinkedListNode<T> current;
        private readonly LinkedList<T> list;
        
        internal LoopedList(LinkedList<T> _list)
        {
            current = _list.First;
            this.list = _list;
            this.list.AddLast(list.First);
        }

        internal T getValue()
        {
            return current.Value;
        }

        internal void next()
        {
            current = current.Next;
        }

    }
}

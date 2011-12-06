using System.Collections.Generic;
using SFML.Graphics;

namespace Game.Graphic_Manager
{
    enum SpriteImage
    {
        AGENT, CIVILIAN, 
    }

    enum agent
    {

    }

    internal enum DisplayCommand { MOVE_VIEW, ZOOM_VIEW, ADD_ENTITY, MOVE_ENTITY, ADD_SHOT, DESTROY_ENTITY }
    internal enum DecalType { EXPLOSION, BLOOD, RUBBLE }

    internal class Animation
    {
        LinkedList<Sprite> order;

        internal Animation(LinkedList<Sprite> _list)
        {
            this.order = new LinkedList<Sprite>(_list);
        }

        internal Animation(Logic.Area area, Logic.entityType type)
        {
            // TODO - missing function. The basic idea is to generate an explosion sprite based on size, and whether it's a person, building or vehicle
        }

        internal Sprite getNext()
        {
            order.RemoveFirst();
            Sprite temp = order.First.Value;
            return temp;
        }

        internal Sprite current()
        {
            Sprite temp = order.First.Value;
            return temp;
        }

        internal bool isDone()
        {
            return order.Count == 0;
        }
    }

}
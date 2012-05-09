using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System;

namespace Game.Graphic_Manager
{


    internal enum DisplayCommand { MOVE_VIEW, ZOOM_VIEW, ADD_ENTITY, MOVE_ENTITY, ADD_SHOT, DESTROY_ENTITY }
    internal enum DecalType { WRECKAGE, BLOOD, RUBBLE } //TODO - different vehicles wreckage?

    internal struct Decal
    {
        const int DECAL_STAY_TIME = 3000;
        static Dictionary<DecalType, Texture> decals = new Dictionary<DecalType,Texture>
            {
                //{DecalType.EXPLOSION, 
                {DecalType.BLOOD, new Texture("images/Decals/bloodsplatter.png")}
                //{DecalType.RUBBLE, 
            };

        private readonly Sprite _sprite;
        private int _stayTime;

        internal Decal(DecalType type)
        {
            this._sprite = new Sprite(decals[type]);
            this._stayTime = DECAL_STAY_TIME;
        }

        internal bool isDone()
        {
            return this._stayTime == 0;
        }

        internal Sprite getDecal()
        {
            this._stayTime--;
            return this._sprite;
        }


        internal void setLocation(SFML.Window.Vector2f vector)
        {
            this._sprite.Position= vector;
        }
    }

    internal class Animation
    {
        List<Sprite> order;

        internal Animation(List<Sprite> _list)
        {
            this.order = new List<Sprite>(_list);
        }

        internal Animation(Area area, Logic.entityType type)
        {
            // TODO - missing function. The basic idea is to generate an explosion sprite based on size, and whether it's a person, building or vehicle
        }

        internal Sprite getNext()
        {
            order.RemoveAt(0);
            Sprite temp = order[0];
            return temp;
        }

        internal Sprite current()
        {
            Sprite temp = order[0];
            return temp;
        }

        internal bool isDone()
        {
            return order.Count <= 1;
        }
    }

       internal class SpriteLoop
    {
        private readonly LoopedList<SFML.Graphics.Sprite> _list;

        internal SpriteLoop(List<SFML.Graphics.Sprite> list)
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
        private int currentIndex;
        private readonly List<T> list;
        
        internal LoopedList(List<T> _list)
        {
            currentIndex = 0;
            this.list = _list;
        }

        internal T getValue()
        {
            return list[currentIndex];
        }

        internal void next()
        {
            
            currentIndex++;
            if (currentIndex == list.Count)
                currentIndex = 0;
        }

    }

}
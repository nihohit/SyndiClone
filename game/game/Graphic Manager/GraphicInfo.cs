using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System;

namespace Game.Graphic_Manager
{


    internal enum DisplayCommand { MOVE_VIEW, ZOOM_VIEW, ADD_ENTITY, MOVE_ENTITY, ADD_SHOT, DESTROY_ENTITY }
    internal enum DecalType { WRECKAGE, BLOOD, RUBBLE } //TODO - different vehicles wreckages for different vehicles?
    
    //this struct represents decals - temporary static pictures.
    internal struct Decal
    {
        static readonly uint DECAL_STAY_TIME = FileHandler.getUintProperty("decal stay time", FileAccessor.DISPLAY);
        static Dictionary<DecalType, Texture> decals = new Dictionary<DecalType,Texture>
            {
                //{DecalType.EXPLOSION, 
                {DecalType.BLOOD, new Texture("images/Decals/bloodsplatter.png")}
                //{DecalType.RUBBLE, 
            };

        private readonly Sprite _sprite;
        private uint _stayTime;

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

    //this struct is in charge of changing Sprites with a limited amount of appearances. Also, supposed not to be connected to an ExternalEntity, but held in a different list. 
    //TODO - replace with a spriteloop and a timer? 
    internal struct Animation
    {
        List<Sprite> order;

        internal Animation(List<Sprite> _list)
        {
            this.order = new List<Sprite>(_list);
        }

        internal Animation(Area area, Logic.entityType type)
        {
            // TODO - missing function. The basic idea is to generate an explosion sprite based on size, and whether it's a person, building or vehicle
            order = null;
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

    //this represents a series of Sprites that are repeated one after the other. 
    internal struct SpriteLoop
    {
        private readonly LoopedList<SFML.Graphics.Sprite> _list;

        internal SpriteLoop(List<SFML.Graphics.Sprite> list)
        {
            this._list = new LoopedList<SFML.Graphics.Sprite>(list);
        }

        internal SFML.Graphics.Sprite nextSprite()
        {
            this._list.next();
            return this.CurrentSprite();
        }

        internal SFML.Graphics.Sprite CurrentSprite()
        {
            return this._list.getValue();
        }

    }

    internal class LoopedList<T> 
    {
        private int currentIndex;
        private readonly List<T> list;
        
        internal LoopedList(List<T> _list)
        {
            currentIndex = 0;
            this.list = new List<T>(_list);
        }

        internal T getValue()
        {
            return list[currentIndex];
        }

        internal void next()
        {
            this.currentIndex = currentIndex+1;
            if (this.currentIndex == list.Count)
                this.currentIndex = 0;
        }

    }

}
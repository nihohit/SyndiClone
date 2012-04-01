﻿using System.Collections.Generic;
using SFML.Graphics;
using System;

namespace Game.Graphic_Manager
{

    enum agent
    {

    }

    internal enum DisplayCommand { MOVE_VIEW, ZOOM_VIEW, ADD_ENTITY, MOVE_ENTITY, ADD_SHOT, DESTROY_ENTITY }
    internal enum DecalType { WRECKAGE, BLOOD, RUBBLE } //TODO - different vehicles wreckage?

    internal struct Decal
    {
        const int DECAL_STAY_TIME = 3000;
        static Dictionary<DecalType, Image> decals = new Dictionary<DecalType,Image>
            {
                //{DecalType.EXPLOSION, 
                {DecalType.BLOOD, new Image("images/bloodsplatter.png")}
                //{DecalType.RUBBLE, 
                
            };

        private readonly DecalType _type;
        private int _stayTime;

        internal Decal(DecalType type)
        {
            this._type = type;
            this._stayTime = DECAL_STAY_TIME;
        }

        bool isDone()
        {
            return this._stayTime == 0;
        }

        Image getDecal()
        {
            this._stayTime--;
            return decals[this._type];
        }

    }

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
            return order.Count <= 1;
        }
    }

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
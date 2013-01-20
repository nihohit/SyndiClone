using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;

namespace Game.Logic.Pathfinding
{
    enum VisibleColors { YELLOW, WHITE, RED }

    #region VisibleAStar

    public class VisibleAStar : AStar
    {
        #region fields

        Texture m_texture = new Texture("images/debug/yellow_pixel.png");

        static Game.Buffers.DisplayBuffer s_buffer;

        #endregion

        #region constructor

        public VisibleAStar(TerrainGrid gridHolder) : base(gridHolder)
        { }

        #endregion

        #region public methods

        public override List<Direction> FindPath(Point entry, Point goal, Direction originalDirection, AStarConfiguration configuration)
        {
            var mid = base.FindPath(entry, goal, originalDirection, configuration);
            List<Buffers.IBufferEvent> list = new List<Buffers.IBufferEvent>();
            var midPoint = entry;
            foreach (Direction dir in mid)
            {
                midPoint = new Point(midPoint, Vector.DirectionToVector(dir));
                var sprite = new Sprite(m_texture);
                sprite.Position = midPoint.ToVector2f();
                list.Add(new Buffers.DisplayImageBufferEvent(sprite)); 
            }
            s_buffer.ReceiveActions(list);
            return mid;
        }

        public static void Setup(Game.Buffers.DisplayBuffer buffer)
        {
            s_buffer = buffer;
        }

        #endregion
    }

    #endregion

    #region AdvancedVisibleAStar

    public class AdvancedVisibleAStar : AdvancedAStar
    {
        #region fields

        Texture m_texture = new Texture("images/debug/red_blot.png");
        static Game.Buffers.DisplayBuffer s_buffer;

        #endregion

        public AdvancedVisibleAStar(TerrainGrid gridHolder)
            : base(gridHolder, new VisibleAStar(gridHolder), new VisibleAStar(AdvancedAStar.MinimiseGrid(gridHolder)))
        { }

        public static void Setup(Game.Buffers.DisplayBuffer buffer)
        {
            s_buffer = buffer;
            VisibleAStar.Setup(buffer);
        }

        protected override List<Direction> AnalyseRudimentaryResults(AstarNode node, Point entry, Point goal, Direction originaldirection, AStarConfiguration configuration)
        {
            var midPoint = node;
            List<Buffers.IBufferEvent> list = new List<Buffers.IBufferEvent>();
            while (midPoint != null)
            {
                var sprite = new Sprite(m_texture);
                sprite.Position = ConvertToCentralPoint(midPoint.Point).ToVector2f();
                list.Add(new Buffers.DisplayImageBufferEvent(sprite));
                midPoint = midPoint.Parent;
            }
            lock (s_buffer)
            {
                s_buffer.ReceiveActions(list);
                s_buffer.Updated = true;
            }
            return base.AnalyseRudimentaryResults(node, entry, goal, originaldirection, configuration);
        }
    }

    #endregion
}

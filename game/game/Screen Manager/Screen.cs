
namespace Game.Screen_Manager
{
    public interface IScreen
    {
        void GainControl(SFML.Graphics.RenderWindow window, Gwen.Control.Canvas canvas);
        void Loop();
    }
}

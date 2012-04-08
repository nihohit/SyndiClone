using System.Collections.Generic;

namespace Game.Logic.Entities
{
    interface Shooter
    {

        Weapon weapon();

        HitFunction hitFunc();

        bool readyToShoot();

        Entity target();

        targetChooser targeter();

    }
}

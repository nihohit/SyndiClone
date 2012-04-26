using System.Collections.Generic;

namespace Game.Logic.Entities
{
    interface Shooter
    {

        Weapon weapon();

        bool readyToShoot();

        Entity target();

        targetChooser targeter();

    }
}

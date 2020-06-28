using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    class DayNightCycle : Module
    {
        float rotationSpeed = 20.0F;
        protected override void Update()
        {
            DirectionalLight.Main.WObject.LocalRotation *= new Quaternion(0, rotationSpeed * (float)Time.DeltaTime, 0);
        }
    }
}

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
        double sunAngle = 10;

        double dayTime = 600;

        protected override void Update()
        {
            sunAngle += (360D / dayTime) * Time.DeltaTime;
            //Engine.Debug.Log(sunAngle);
            DirectionalLight.Main.WObject.LocalRotation = new Quaternion(sunAngle, 270, 0);
        }
    }
}

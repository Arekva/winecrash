using System;
using Newtonsoft.Json;

namespace WEngine
{
    [Serializable]
    public struct PhysicMaterial
    {
        private double _friction; // binary serialisation
        private double _bounciness; // binary serialisation
        
        public double Friction
        {
            get => _friction;
            set => _friction = value;
        }
        public double Bounciness
        {
            get => _bounciness;
            set => _bounciness = value;
        }
        
        public PhysicMaterial(double friction, double bounciness) => (_friction, _bounciness) = (friction, bounciness);
    }
}
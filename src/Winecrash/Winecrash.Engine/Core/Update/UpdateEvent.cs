using System;

namespace WEngine
{
    public delegate void UpdateEventHandler(UpdateEventArgs e);

    public class UpdateEventArgs : EventArgs
    {
        public double DeltaTime { get; }

        public UpdateEventArgs(double deltaTime)
        {
            this.DeltaTime = deltaTime;
        }
    }
}

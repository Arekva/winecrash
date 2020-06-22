using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class Module : WObject
    {
        private int _ExecutionLayer;
        public int ExecutionLayer
        {
            get
            {
                return this.ExecutionLayer;
            }
        }
        private int _Group;
        public int Group 
        { 
            get
            {
                return _Group;
            }
            set
            {
                SetGroup(value);
            }
        }

        private int _ExecutionOrder = 0;
        public int ExecutionOrder
        {
            get
            {
                return this._ExecutionOrder;
            }

            set
            {

            }
        }

        private void SetGroup(int group)
        {
            if (group == _Group) return;
        }

        private Module()
        {

        }

        internal protected virtual void Update() { }
    }
}

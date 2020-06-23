using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class Module : BaseObject
    {
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
                return _ExecutionOrder;
            }

            set
            {
                SetOrder(value);
            }
        }

        private void SetOrder(int newOrder)
        {
            this._ExecutionOrder = newOrder;

            Engine.Group.GetGroup(this.Group).SortModules();
        }
        private void SetGroup(int newGroup)
        {
            if (newGroup == _Group) return;

            Group previousGroup = Engine.Group.GetGroup(this._Group);
            
            previousGroup.RemoveModule(this);

            Engine.Group.CreateOrGetGroup(newGroup, null, new[] { this });

            this._Group = newGroup;
        }

        public Module() : base()
        {
            Engine.Group.CreateOrGetGroup(0, null, new[] { this });
        }

        public WObject WObject { get; internal set; }

        internal bool StartDone { get; set; } = false;

        internal protected virtual void Creation() { }
        internal protected virtual void Start() { }
        internal protected virtual void Update() { }
        internal protected virtual void OnDelete() { }

        public sealed override void Delete()
        {
            this.OnDelete();

            this.WObject._Modules.Remove(this);
            this.WObject = null;
            Engine.Group.GetGroup(this.Group).RemoveModule(this);

            base.Delete();
        }
    }
}

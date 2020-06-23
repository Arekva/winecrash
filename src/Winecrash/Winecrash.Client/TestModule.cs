using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Client
{
    public class TestModule : Module
    {
        //public Int64[] dummy = new Int64[25_000_000];
        protected override void Creation()
        {
            //Debug.Log("TestModule creation ! Parent : " + this.WObject.Name);
        }

        protected override void Start()
        {
            //Debug.Log("TestModule first frame");
        }

        protected override void Update()
        {
            //Debug.Log("TestModule update");
        }

        protected override void OnDelete()
        {
            Debug.Log("TestModule delete. goodbye :(");
        }
    }
}

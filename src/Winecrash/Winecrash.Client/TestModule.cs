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
            if (Input.IsPressing(Keys.One))
            {
                Debug.Log("One is pressed");
            }
        }

        protected override void OnDelete()
        {
            Debug.Log("TestModule delete. goodbye :(");
        }
    }
}

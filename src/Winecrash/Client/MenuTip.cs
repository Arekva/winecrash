﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash.Client
{
    public class MenuTip : Module
    {
        private WEngine.GUI.Label _ReferenceLabel;
        public WEngine.GUI.Label ReferenceLabel
        {
            get
            {
                return _ReferenceLabel;
            }

            set
            {
                _ReferenceLabel = value;

                LbBaseMin = _ReferenceLabel.MinAnchor;
                LbBaseMax = _ReferenceLabel.MaxAnchor;
            }
        }

        private Vector2D LbBaseMin;
        private Vector2D LbBaseMax;

        public double AnimationScale { get; set; } = 0.025D;
        public double AnimationTime { get; set; } = 0.6D;

        public static string[] Tips { get; set; }

        protected override void Creation()
        {
            try
            {
                Tips = File.ReadAllLines("assets/other/tips.txt");
                Parallel.For(0, Tips.Length, i => 
                    Tips[i] = Tips[i].Replace(@"\n", "\n"));
            }
            catch(Exception e)
            {
                Debug.LogError("No tips available (searching for assets/other/tips.txt) :(");
            }
        }

        protected override void Update()
        {
            if (!ReferenceLabel) return;

            double pctAnimation = (Time.TimeSinceStart / AnimationTime) % 1.0D;
            pctAnimation = WMath.Remap(Math.Cos(WMath.Remap(pctAnimation, 0.0, 1.0, -1.0, 1.0)), 0.5, 1.0, 0.0, AnimationScale);

            ReferenceLabel.MinAnchor = LbBaseMin - new Vector2D(0.0, pctAnimation);
            ReferenceLabel.MaxAnchor = LbBaseMax + new Vector2D(0.0, pctAnimation);
        }

        public string SelectRandom()
        {
            if(Tips != null && Tips.Length != 0)
            {
                return Tips[new Random().Next(Tips.Length)];
            }
            else
            {
                return "Minecraft!";
            }
        }
    }
}

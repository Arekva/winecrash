﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    class World : Module
    {
        public static Random WorldRandom = new Random();
        public static int Seed { get; private set; }
        public static uint StartRadius = 37 - 22;
        public const uint StartLevel = 22;

        public static World Instance;

        public static Mesh DebugChunkMesh = Mesh.LoadFile("assets/models/Cube.obj", MeshFormats.Wavefront);

        protected override void Creation()
        {
            if(Instance)
            {
                this.Delete();
                return;
            }

            else
            {
                Instance = this;
            }
        }

        protected override void Start()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Ticket.CreateTicket(0, 0, StartLevel, TicketTypes.Start, TicketPreviousDirection.None, true);
            sw.Stop();
            Engine.Debug.Log("World created in " + sw.Elapsed.TotalMilliseconds.ToString("N2") + " ms");
        }


        internal static void Tick()
        {
            Ticket[] tickets = Ticket._Tickets.ToArray();

            for (int i = 0; i < tickets.Length; i++)
            {
                tickets[i].Tick();
            }
        }

        protected override void OnDelete()
        {
            Ticket[] tickets = Ticket._Tickets.ToArray();

            for (int i = 0; i < tickets.Length; i++)
            {
                tickets[i].Delete();
            }
        }
    }
}
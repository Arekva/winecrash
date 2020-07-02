using System;
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

        public static List<Chunk> SpawnNextUpdate = new List<Chunk>();

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
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
           
            Task.Run(() => Ticket.CreateTicket(0, 0, StartLevel, TicketTypes.Start, TicketPreviousDirection.None, true));
            //sw.Stop();
            //Engine.Debug.Log("World created in " + sw.Elapsed.TotalMilliseconds.ToString("N2") + " ms");

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

        public static void GlobalToLocal(Vector3I global, out Vector3I ChunkPosition, out Vector3I LocalPosition)
        {
            ChunkPosition = new Vector3I(
                (global.X / Chunk.Width) + (global.X < 0 ? -1 : 0), //X position
                (global.Z / Chunk.Depth) + (global.Z < 0 ? -1 : 0), //Y position
                0);                                                 //Z dimension

            int localX = global.X % Chunk.Width;
            if (localX < 0) localX += Chunk.Width;

            int localZ = global.Z % Chunk.Depth;
            if (localZ < 0) localZ += Chunk.Depth;

            LocalPosition = new Vector3I(localX, WMath.Clamp(global.Y, 0, 255), localZ);
        }
    }
}

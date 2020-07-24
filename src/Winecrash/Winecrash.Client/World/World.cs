using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static List<Chunk> SpawnNextUpdate = new List<Chunk>();



        public static double TickTime = 1D / 20D;
        private static double TimeSinceLastTick = 0.0D;
        public static int RandomTickSpeed = 3;

        internal static List<TickWaitItem> TickOnNextTick = new List<TickWaitItem>();

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

                Player.OnChangeChunk += UpdateChunks;
            }
        }

        private void UpdateChunks(Chunk c)
        {

            Task.Run(() => Ticket.CreateTicket(c.Position.X, c.Position.Y, 31, TicketTypes.Player, TicketPreviousDirection.None, true));

        }

        protected override void Start()
        {
            Thread worldGenerationThread = new Thread(() => Ticket.CreateTicket(0, 0, StartLevel, TicketTypes.Start, TicketPreviousDirection.None, true))
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            worldGenerationThread.Start();
        }

        protected override void Update()
        {
            if(TimeSinceLastTick >= TickTime)
            {
                object obj = new object();

                Ticket[] tickets;
                TickWaitItem[] twi;
                lock (obj)
                {
                    tickets = Ticket._Tickets.ToArray();
                    twi = TickOnNextTick.ToArray();
                    TickOnNextTick.Clear();
                }

                for (int i = 0; i < tickets.Length; i++)
                {
                    tickets[i].Chunk.Tick();
                }

                for (int i = 0; i < twi.Length; i++)
                {
                    twi[i].Block.Tick(twi[i].Type, twi[i].Chunk, twi[i].Position);
                }

                TimeSinceLastTick = 0.0D;
            }

            TimeSinceLastTick += Time.DeltaTime;
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

        public static void GlobalToLocal(Vector3F global, out Vector3I ChunkPosition, out Vector3I LocalPosition)
        {
            ChunkPosition = new Vector3I(
                ((int)global.X / Chunk.Width) + (global.X < 0.0F ? -1 : 0), //X position
                ((int)global.Z / Chunk.Depth) + (global.Z < 0.0F ? -1 : 0), //Y position
                0);                                                         //Z dimension

            float localX = global.X % Chunk.Width;
            if (localX < 0) localX += Chunk.Width;

            float localZ = global.Z % Chunk.Depth;
            if (localZ < 0) localZ += Chunk.Depth;

            LocalPosition = new Vector3I((int)localX, WMath.Clamp((int)global.Y, 0, 255), (int)localZ);
        }
    }
}

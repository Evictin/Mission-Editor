using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Space_Cats_V1._2

{
    public class MissionScriptNode
    {
        #region Node ID Constants
        public const int CMD_NULL = 0;
        public const int CMD_START = 1;
        public const int CMD_END = 2;
        public const int CMD_SET_ASTEROID_DENSITY = 3;
        public const int CMD_SPAWN_ENEMY1 = 4;
        public const int CMD_SPAWN_ENEMY1_WAVE = 5;
        #endregion

        public enum CommandID
        {
            Null, Start, End, SetAsteroidDensity, SpawnEnemy1, SpawnEnemy1Wave
        }

        protected CommandID z_command;
        private int z_timeStamp;
        private bool z_isDone;
        private List<MissionScriptNode> z_subNodes;

        [Description("The time at which this command starts execution (in milliseconds).")]
        virtual public int TimeStamp
        {
            get { return z_timeStamp; }
            set { z_timeStamp = value; }
        }
        [Browsable(false)]
        public CommandID Command
        {
            get { return z_command; }
            set { z_command = value; }
        }
        [Browsable(false)]
        public bool IsDone
        {
            get { return z_isDone; }
            set { z_isDone = value; }
        }

        public MissionScriptNode(CommandID command, int timeStamp)
        {
            z_command = command;
            z_timeStamp = timeStamp;
            z_isDone = false;
            z_subNodes = new List<MissionScriptNode>();
        }

        public MissionScriptNode()
            : this(CommandID.Null, 0)
        {
        }

        public MissionScriptNode(CommandID command, BinaryReader br)
            : this(command, br.ReadInt32())
        {
        }

        public static MissionScriptNode readNodeFromFile(BinaryReader br)
        {
            CommandID command = (CommandID)br.ReadInt32();
            switch (command)
            {
                case CommandID.Start:
                    return new MS_Start(br);
                case CommandID.End:
                    return new MS_End(br);
                case CommandID.SetAsteroidDensity:
                    return new MS_SetAsteroidDensity(br);
                case CommandID.SpawnEnemy1:
                    return new MS_SpawnEnemy1(br);
                case CommandID.SpawnEnemy1Wave:
                    return new MS_SpawnEnemy1Wave(br);
            }
            return null;
        }

        virtual public bool CanExecute(int missionTime)
        {
            return (missionTime>=TimeStamp);
        }

        virtual public void Execute(GameTime gameTime)
        {
            IsDone = true;
        }

        virtual public void reset()
        {
            IsDone = false;
            foreach (MissionScriptNode node in z_subNodes)
                node.reset();
        }

        virtual public void WriteToFile(BinaryWriter bw)
        {
            bw.Write((int)Command);
            bw.Write(z_timeStamp);
        }

        public override string ToString()
        {
            return "NULL";
        }
    }

    class MS_End : MissionScriptNode
    {
        public MS_End(int timeStamp)
            : base(CommandID.End, timeStamp)
        {
        }

        public MS_End(BinaryReader br)
            : base(CommandID.End, br)
        {
        }

        public override void Execute(GameTime gameTime)
        {
            IsDone = true;
            MissionManager.Instance.EndMission();
        }

        public override string ToString()
        {
            return string.Format("{0}: End", TimeStamp);
        }
    }

    class MS_Start : MissionScriptNode
    {
        [Browsable(false)]
        public override int TimeStamp
        {
            get
            {
                return base.TimeStamp;
            }
            set
            {
                base.TimeStamp = value;
            }
        }

        public MS_Start()
            : base(CommandID.Start, 0)
        {
        }

        public MS_Start(BinaryReader br)
            : base(CommandID.Start, br)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}: Start", TimeStamp);
        }
    }

    class MS_SetAsteroidDensity : MissionScriptNode
    {
        private Asteroid.AsteroidDensity z_density;
        [Description("The density of the asteroids on screen.")]
        public Asteroid.AsteroidDensity Density
        {
            get { return z_density; }
            set { z_density = value; }
        }

        public MS_SetAsteroidDensity(int timeStamp, Asteroid.AsteroidDensity density)
            : base(CommandID.SetAsteroidDensity, timeStamp)
        {
            z_density = density;
        }

        public MS_SetAsteroidDensity(BinaryReader br)
            : base(CommandID.SetAsteroidDensity, br)
        {
            z_density = (Asteroid.AsteroidDensity) br.ReadInt32();
        }

        public override void Execute(GameTime gameTime)
        {
            Asteroid.Density = z_density;
            IsDone = true;
        }

        public override void WriteToFile(BinaryWriter bw)
        {
            base.WriteToFile(bw);
            bw.Write((int)z_density);
        }

        public override string ToString()
        {
            return string.Format("{0}: SetAsteroidDensity:{1}", TimeStamp, z_density);
        }
    }

    class MS_SpawnEnemy1 : MissionScriptNode
    {
        private int z_AI_ID;
        public int AI_ID
        {
            get { return z_AI_ID; }
            set { z_AI_ID = value; }
        }

        public MS_SpawnEnemy1(int timeStamp, int AI_ID)
            : base(CommandID.SpawnEnemy1, timeStamp)
        {
            z_AI_ID = AI_ID;
        }

        public MS_SpawnEnemy1(BinaryReader br)
            : base(CommandID.SpawnEnemy1, br)
        {
            z_AI_ID = br.ReadInt32();
        }

        public override void Execute(GameTime gameTime)
        {
            EnemyManager.AddEnemy(Enemy1.getNewEnemy(MissionManager.GetAI(z_AI_ID)));
            IsDone = true;
        }

        public override void WriteToFile(BinaryWriter bw)
        {
            base.WriteToFile(bw);
            bw.Write(z_AI_ID);
        }

        public override string ToString()
        {
            return string.Format("{0}: SpawnEnemy1 AI:{1}", TimeStamp, z_AI_ID);
        }
    }

    class MS_SpawnEnemy1Wave : MissionScriptNode
    {
        private int z_timer;
        private int z_enemiesToSpawn, z_enemiesSpawned;
        private int z_spawnDelay;
        private int z_AI_ID1, z_AI_ID2;
        [Description("The number of enemies to spawn in this wave.")]
        public int EnemiesToSpawn
        {
            get { return z_enemiesToSpawn; }
            set { z_enemiesToSpawn = value; }
        }
        [Description("The delay between enemies spawned in this wave (in milliseconds).")]
        public int SpawnDelay
        {
            get { return z_spawnDelay; }
            set { z_spawnDelay = value; }
        }
        [Description("The low end of the range of AIs to spawn.\nSet both IDs to the same value to use only one.")]
        public int AIRangeLow
        {
            get { return z_AI_ID1; }
            set { z_AI_ID1 = value; }
        }
        [Description("The high end of the range of AIs to spawn.\nSet both IDs to the same value to use only one.")]
        public int AIRangeHigh
        {
            get { return z_AI_ID2; }
            set { z_AI_ID2 = value; }
        }

        public MS_SpawnEnemy1Wave(int timeStamp, int numEnemies, int spawnDelay, int AI_ID1, int AI_ID2)
            : base(CommandID.SpawnEnemy1Wave, timeStamp)
        {
            z_timer = 0;
            z_AI_ID1 = AI_ID1;
            z_AI_ID2 = AI_ID2;
            z_spawnDelay = spawnDelay;
            z_enemiesToSpawn = numEnemies;
            z_enemiesSpawned = 0;
        }

        public MS_SpawnEnemy1Wave(BinaryReader br)
            : base(CommandID.SpawnEnemy1Wave, br)
        {
            z_timer = 0;
            z_enemiesToSpawn = br.ReadInt32();
            z_spawnDelay = br.ReadInt32();
            z_AI_ID1 = br.ReadInt32();
            z_AI_ID2 = br.ReadInt32();
            z_enemiesSpawned = 0;
        }

        public override void Execute(GameTime gameTime)
        {
            z_timer += gameTime.ElapsedGameTime.Milliseconds;
            if (z_timer >= z_spawnDelay)
            {
                EnemyManager.AddEnemy(Enemy1.getNewEnemy(MissionManager.GetAI(
                    GameObject.RandomGen.Next(z_AI_ID1, z_AI_ID2 + 1))));
                ++z_enemiesSpawned;
                z_timer = 0;
            }
            IsDone = (z_enemiesSpawned==z_enemiesToSpawn);
        }

        override public void reset()
        {
            z_timer = 0;
            z_enemiesSpawned = 0;
        }

        public override void WriteToFile(BinaryWriter bw)
        {
            base.WriteToFile(bw);
            bw.Write(z_enemiesToSpawn);
            bw.Write(z_spawnDelay);
            bw.Write(z_AI_ID1);
            bw.Write(z_AI_ID2);
        }

        public override string ToString()
        {
            return string.Format("{0}: SpawnEnemy1Wave Qu:{1} Delay:{2} FromAI:{3} ToAI:{4}",
                TimeStamp, z_enemiesToSpawn, z_spawnDelay, z_AI_ID1, z_AI_ID2);
        }
    }
}

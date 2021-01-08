using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XBOXGameBarPoC_Win32
{
    class Game
    {
        private Process gProcess;
        public Module Client { get; set; }

        public Game()
        {
            Process[] pList = Process.GetProcessesByName("csgo");
            if (pList.Length > 0)
            {
                gProcess = pList[0];
                foreach (ProcessModule module in gProcess.Modules)
                {
                    if (module.ModuleName == "client.dll")
                    {
                        Module client;
                        client.module = module;
                        Client = client;
                    }
                }
                Memory.SetHandle(gProcess.Handle);
            }
        }

        public static bool operator !(Game game)
        {
            return (game.gProcess == null || game.Client.module == null);
        }
    }

    class Offsets
    {
        public static readonly int dwViewMatrix = 0x4D914D4;
        public static readonly int dwLocalPlayer = 0xD882BC;
        public static readonly int dwEntityList = 0x4D9FBD4;
    }

    // Structs
    struct Module
    {
        public ProcessModule module;

        public Matrix ViewMatrix()
        {
            return Memory.Read<Matrix>(module.BaseAddress + Offsets.dwViewMatrix);
        }

        public Entity LocalPlayer()
        {
            IntPtr _localPlayer = Memory.Read<IntPtr>(module.BaseAddress + Offsets.dwLocalPlayer);
            return Memory.Read<Entity>(_localPlayer);
        }

        public Entity[] EntityList()
        {
            List<Entity> entityList = new List<Entity>();
            EntityListItem[] entityPtrs = Memory.ReadArray<EntityListItem>(module.BaseAddress + Offsets.dwEntityList, 64);

            for (int i = 0; i < entityPtrs.Length; i++)
            {
                if (entityPtrs[i].Pointer != IntPtr.Zero)
                {
                    entityList.Add(Memory.Read<Entity>(entityPtrs[i].Pointer));
                }
            }

            return entityList.ToArray();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    struct EntityListItem
    {
        [FieldOffset(0)]
        public IntPtr Pointer;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct Entity
    {
        [FieldOffset(0xED)]
        public bool m_bDormant;

        [FieldOffset(0xF4)]
        public int m_iTeamNum;

        [FieldOffset(0x100)]
        public int m_iHealth;

        [FieldOffset(0x138)]
        public Vector3 m_vecOrigin;

        [FieldOffset(0x26A8)]
        private IntPtr m_dwBoneMatrix;

        public Matrix3x4 BoneMatrix(Bone index)
        {
            return Memory.Read<Matrix3x4>(m_dwBoneMatrix + (int)index * Marshal.SizeOf(typeof(Matrix3x4)));
        }
    }

    enum Bone
    {
        Head = 6
    }
}

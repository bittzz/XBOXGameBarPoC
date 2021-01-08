using System;
using System.IO.MemoryMappedFiles;
using Windows.Storage;
using System.Threading;
using System.Collections.Generic;

namespace XBOXGameBarPoC_Win32
{
    static class Program
    {
        static void Main()
        {
            MemoryMappedFile memoryMappedFile;
            Mutex mutex;

            while (true)
            {
                try
                {
                    memoryMappedFile = MemoryMappedFile.OpenExisting($"AppContainerNamedObjects\\{ (string)ApplicationData.Current.LocalSettings.Values["AppSID"] }\\XboxGameBarPoc_SharedMemory");
                    if (Mutex.TryOpenExisting($"AppContainerNamedObjects\\{ (string)ApplicationData.Current.LocalSettings.Values["AppSID"] }\\XboxGameBarPoc_Mutex", out mutex))
                    {
                        break;
                    }

                }
                catch (Exception)
                {
                    // MemoryMappedFile or Mutex has not yet been created
                }
                Thread.Sleep(1000);
            }

            using (memoryMappedFile)
            {
                using (MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor())
                {
                    using (mutex)
                    {
                        List<Box> boxList = new List<Box>();
                        Vector2 screenSize = new Vector2(1920, 1080);
                        Game CSGO = new Game();

                        while (true)
                        {
                            try
                            {
                                mutex.WaitOne();

                                if (!CSGO)
                                {
                                    Thread.Sleep(1000);
                                    CSGO = new Game();
                                    continue;
                                }

                                Matrix viewMatrix = CSGO.Client.ViewMatrix();
                                Entity localPlayer = CSGO.Client.LocalPlayer();
                                Entity[] entityList = CSGO.Client.EntityList();

                                foreach (Entity entity in entityList)
                                {
                                    if (entity.m_iTeamNum != localPlayer.m_iTeamNum && !entity.m_bDormant && entity.m_iHealth > 0 && viewMatrix.WorldToScreen(entity.m_vecOrigin, screenSize, out Vector2 feetScreenLocation))
                                    {
                                        Vector2 headScreenLocation = viewMatrix.WorldToScreen(entity.BoneMatrix(Bone.Head).ToVector3(), screenSize);
                                        boxList.Add(new Box(headScreenLocation, feetScreenLocation));
                                    }
                                }

                                viewAccessor.Write(0, boxList.Count);
                                viewAccessor.WriteArray<Box>(4, boxList.ToArray(), 0, boxList.Count);

                                boxList.Clear();
                            }
                            finally
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }
            }
        }
    }
}

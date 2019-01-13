using ColossalFramework;
using CSM.Injections;
using CSM.Networking;
using System;
using UnityEngine;
using System.Reflection;
using static TerrainTool;
using Harmony;

namespace CSM.Commands.Handler
{
    class TerrainModificationHandler : CommandHandler<TerrainModificationCommand>
    {
        public override byte ID => CommandIds.TerrainModificationCommand;

        public override void HandleOnClient(TerrainModificationCommand command) => Handle(command);

        public override void HandleOnServer(TerrainModificationCommand command, Player player) => Handle(command);

        private void Handle(TerrainModificationCommand command)
        {
            ushort[] m_tempBuffer = (ushort[])typeof(TerrainTool).GetField("m_tempBuffer", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>());


            TerrainManager instance = Singleton<TerrainManager>.instance;
            GameAreaManager manager2 = Singleton<GameAreaManager>.instance;
            SimulationManager manager3 = Singleton<SimulationManager>.instance;
            float[] brushData = command.BrushData;
            float num = command.BrushSize * 0.5f;
            float num2 = 16f;
            int b = 0x438;
            ushort[] rawHeights = instance.RawHeights;
            ushort[] finalHeights = instance.FinalHeights;
            ushort[] backupHeights = instance.BackupHeights;
            float strength = command.Strength;
            int num5 = 3;
            float num6 = 0.015625f;
            float num7 = 64f;
            Vector3 mousePosition = command.mousePosition;
            Vector3 vector2 = command.EndPosition - command.StartPosition;
            vector2.y = 0f;
            float sqrMagnitude = vector2.sqrMagnitude;
            if (sqrMagnitude != 0f)
            {
                sqrMagnitude = 1f / sqrMagnitude;
            }
            float num9 = 20f;
            bool flag = true; //(base.m_toolController.m_mode & ItemClass.Availability.Game) != ItemClass.Availability.None;
            int a = 0;
            int num11 = 0;
            //int currentCost = this.m_currentCost;
            int dirtBuffer = instance.DirtBuffer;
            int num14 = 0x80000;
            int dirtPrice = 0;
            
             if (flag)
            {
                if (manager2.PointOutOfArea(mousePosition))
                {
                    //this.m_toolErrors = ToolBase.ToolErrors.None | ToolBase.ToolErrors.OutOfArea;
                    return;
                }
                TerrainProperties properties = instance.m_properties;
                if (properties != null)
                {
                    dirtPrice = properties.m_dirtPrice;
                }
            }
            
            int num16 = Mathf.Max((int)(((mousePosition.x - num) / num2) + (b * 0.5f)), 0);
            int num17 = Mathf.Max((int)(((mousePosition.z - num) / num2) + (b * 0.5f)), 0);
            int num18 = Mathf.Min(((int)(((mousePosition.x + num) / num2) + (b * 0.5f))) + 1, b);
            int num19 = Mathf.Min(((int)(((mousePosition.z + num) / num2) + (b * 0.5f))) + 1, b);
            if (command.mode == Mode.Shift)
            {
                if (command.MouseRightDown)
                {
                    num9 = -num9;
                }
            }
            else if ((command.mode == Mode.Soften) && command.MouseRightDown)
            {
                num5 = 10;
            }
            if ((m_tempBuffer == null) || (m_tempBuffer.Length < (((num19 - num17) + 1) * ((num18 - num16) + 1))))
            {
                m_tempBuffer = new ushort[((num19 - num17) + 1) * ((num18 - num16) + 1)];
            }
            for (int i = num17; i <= num19; i++)
            {
                float z = (i - (b * 0.5f)) * num2;
                float f = ((((z - mousePosition.z) + num) / command.BrushSize) * 64f) - 0.5f;
                int num23 = Mathf.Clamp(Mathf.FloorToInt(f), 0, 0x3f);
                int num24 = Mathf.Clamp(Mathf.CeilToInt(f), 0, 0x3f);
                for (int k = num16; k <= num18; k++)
                {
                    float x = (k - (b * 0.5f)) * num2;
                    float num27 = ((((x - mousePosition.x) + num) / command.BrushSize) * 64f) - 0.5f;
                    int num28 = Mathf.Clamp(Mathf.FloorToInt(num27), 0, 0x3f);
                    int num29 = Mathf.Clamp(Mathf.CeilToInt(num27), 0, 0x3f);
                    int num30 = rawHeights[(i * (b + 1)) + k];
                    float num31 = num30 * num6;
                    float y = 0f;
                    if (flag && manager2.PointOutOfArea(new Vector3(x, mousePosition.y, z), num2 * 0.5f))
                    {
                        m_tempBuffer[(((i - num17) * ((num18 - num16) + 1)) + k) - num16] = (ushort)num30;
                    }
                    else
                    {
                        float num33 = brushData[(num23 * 0x40) + num28];
                        float num34 = brushData[(num23 * 0x40) + num29];
                        float num35 = brushData[(num24 * 0x40) + num28];
                        float num36 = brushData[(num24 * 0x40) + num29];
                        float num37 = num33 + ((num34 - num33) * (num27 - num28));
                        float num38 = num35 + ((num36 - num35) * (num27 - num28));
                        float t = num37 + ((num38 - num37) * (f - num23));
                        t *= strength;
                        if (t <= 0f)
                        {
                            m_tempBuffer[(((i - num17) * ((num18 - num16) + 1)) + k) - num16] = (ushort)num30;
                        }
                        else
                        {
                            if (command.mode == Mode.Shift)
                            {
                                y = (finalHeights[(i * (b + 1)) + k] * num6) + num9;
                            }
                            else if (command.mode == Mode.Level)
                            {
                                y = command.StartPosition.y;
                            }
                            else if (command.mode == Mode.Soften)
                            {
                                int num40 = Mathf.Max(k - num5, 0);
                                int num41 = Mathf.Max(i - num5, 0);
                                int num42 = Mathf.Min(k + num5, b);
                                int num43 = Mathf.Min(i + num5, b);
                                float num44 = 0f;
                                for (int m = num41; m <= num43; m++)
                                {
                                    for (int n = num40; n <= num42; n++)
                                    {
                                        float num47 = 1f - (((float)(((n - k) * (n - k)) + ((m - i) * (m - i)))) / ((float)(num5 * num5)));
                                        if (num47 > 0f)
                                        {
                                            y += finalHeights[(m * (b + 1)) + n] * (num6 * num47);
                                            num44 += num47;
                                        }
                                    }
                                }
                                if (num44 > 0.001f)
                                {
                                    y /= num44;
                                }
                                else
                                {
                                    y = finalHeights[(i * (b + 1)) + k];
                                }
                            }
                            else if (command.mode == Mode.Slope)
                            {
                                float num48 = (k - (b * 0.5f)) * num2;
                                float num49 = (i - (b * 0.5f)) * num2;
                                float num50 = (((num48 - command.StartPosition.x) * vector2.x) + ((num49 -command.StartPosition.z) * vector2.z)) * sqrMagnitude;
                                y = Mathf.Lerp(command.StartPosition.y, command.EndPosition.y, num50);
                            }
                            float num51 = y;
                            y = Mathf.Lerp(num31, y, t);
                            int num52 = Mathf.Clamp(Mathf.RoundToInt(y * num7), 0, 0xffff);
                            if (num52 == num30)
                            {
                                int num53 = Mathf.Clamp(Mathf.RoundToInt(num51 * num7), 0, 0xffff);
                                if (num53 > num30)
                                {
                                    if (((y - num31) * num7) > (manager3.m_randomizer.Int32(0, 0x2710) * 0.0001f))
                                    {
                                        num52++;
                                    }
                                }
                                else if ((num53 < num30) && (((num31 - y) * num7) > (manager3.m_randomizer.Int32(0, 0x2710) * 0.0001f)))
                                {
                                    num52--;
                                }
                            }
                            m_tempBuffer[(((i - num17) * ((num18 - num16) + 1)) + k) - num16] = (ushort)num52;
                            if (flag)
                            {
                                if (num52 > num30)
                                {
                                    a += num52 - num30;
                                }
                                else if (num52 < num30)
                                {
                                    num11 += num30 - num52;
                                }
                                int num54 = backupHeights[(i * (b + 1)) + k];
                                int num55 = Mathf.Abs((int)(num52 - num54)) - Mathf.Abs((int)(num30 - num54));
                                //currentCost += num55 * dirtPrice;
                            }
                        }
                    }
                }
            }
            int num56 = a;
            int num57 = num11;
            ToolBase.ToolErrors none = ToolBase.ToolErrors.None;
            if (flag)
            {
                if (a > num11)
                {
                    num56 = Mathf.Min(a, dirtBuffer + num11);
                    if (num56 < a)
                    {
                        none |= ToolBase.ToolErrors.None | ToolBase.ToolErrors.NotEnoughDirt;
                        GuideController properties = Singleton<GuideManager>.instance.m_properties;
                        if (properties != null)
                        {
                            Singleton<TerrainManager>.instance.m_notEnoughDirt.Activate(properties.m_notEnoughDirt);
                        }
                    }
                    GenericGuide tooMuchDirt = Singleton<TerrainManager>.instance.m_tooMuchDirt;
                    if (tooMuchDirt != null)
                    {
                        tooMuchDirt.Deactivate();
                    }
                }
                else if (num11 > a)
                {
                    num57 = Mathf.Min(num11, (num14 - dirtBuffer) + a);
                    if (num57 < num11)
                    {
                        none |= ToolBase.ToolErrors.None | ToolBase.ToolErrors.TooMuchDirt;
                        GuideController properties = Singleton<GuideManager>.instance.m_properties;
                        if (properties != null)
                        {
                            Singleton<TerrainManager>.instance.m_tooMuchDirt.Activate(properties.m_tooMuchDirt);
                        }
                    }
                    GenericGuide notEnoughDirt = Singleton<TerrainManager>.instance.m_notEnoughDirt;
                    if (notEnoughDirt != null)
                    {
                        notEnoughDirt.Deactivate();
                    }
                }
                /*if (currentCost != Singleton<EconomyManager>.instance.PeekResource(EconomyManager.Resource.Landscaping, currentCost))
                {
                    this.m_toolErrors = none | (ToolBase.ToolErrors.None | ToolBase.ToolErrors.NotEnoughMoney);
                    return;
                }
                currentCost = this.m_currentCost;
                */
            }
            //Singleton<TerrainTool>.instance.m_toolErrors = none;
            if ((num56 != 0) || (num57 != 0))
            {
                GenericGuide terrainToolNotUsed = Singleton<TerrainManager>.instance.m_terrainToolNotUsed;
                if ((terrainToolNotUsed != null) && !terrainToolNotUsed.m_disabled)
                {
                    terrainToolNotUsed.Disable();
                }
            }
            for (int j = num17; j <= num19; j++)
            {
                for (int k = num16; k <= num18; k++)
                {
                    int num60 = rawHeights[(j * (b + 1)) + k];
                    int num61 = m_tempBuffer[(((j - num17) * ((num18 - num16) + 1)) + k) - num16];
                    if (flag)
                    {
                        int num62 = num61 - num60;
                        if (num62 > 0)
                        {
                            if (a > num56)
                            {
                                num62 = ((a - 1) + (num62 * num56)) / a;
                            }
                            a -= num61 - num60;
                            num56 -= num62;
                            num61 = num60 + num62;
                            dirtBuffer -= num62;
                        }
                        else if (num62 < 0)
                        {
                            if (num11 > num57)
                            {
                                num62 = -(((num11 - 1) - (num62 * num57)) / num11);
                            }
                            num11 -= num60 - num61;
                            num57 += num62;
                            num61 = num60 + num62;
                            dirtBuffer -= num62;
                        }
                        int num63 = backupHeights[(j * (b + 1)) + k];
                        int num64 = Mathf.Abs((int)(num61 - num63)) - Mathf.Abs((int)(num60 - num63));
                        //currentCost += num64 * dirtPrice;
                    }
                    if (num61 != num60)
                    {
                        rawHeights[(j * (b + 1)) + k] = (ushort)num61;


                        int _strokeXmin = Math.Min(command.StrokeXmin, k);
                        int _strokeXmax = Math.Max(command.StrokeXmax, k);
                        int _strokeZmin = Math.Min(command.StrokeZmin, j);
                        int _strokeZmax = Math.Max(command.StrokeZmax, j);

                        typeof(TerrainTool).GetField("m_strokeXmin", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(),_strokeXmin);
                        typeof(TerrainTool).GetField("m_strokeXmax", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(),_strokeXmax);
                        typeof(TerrainTool).GetField("m_strokeZmin", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), _strokeZmin);
                        typeof(TerrainTool).GetField("m_strokeZmax", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), _strokeZmax);
                       
                    }
                }
            }
            if (flag)
            {
                instance.DirtBuffer = dirtBuffer;
                //this.m_currentCost = currentCost;
            }
            TerrainModify.UpdateArea((int)(num16 - 2), (int)(num17 - 2), (int)(num18 + 2), (int)(num19 + 2), true, false, false);
        }
    }
    

 }


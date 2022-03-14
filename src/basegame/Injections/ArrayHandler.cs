using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.Math;
using CSM.API;
using CSM.BaseGame.Helpers;
using HarmonyLib;

namespace CSM.BaseGame.Injections
{
    public static class ArrayHandler
    {
        // All the types for Array16<T> and Array32<T> we currently need to track
        private static readonly Type[] _array16Types = {
            typeof(Building), typeof(NetNode), typeof(NetSegment),
            typeof(PropInstance), typeof(TransportLine), typeof(ZoneBlock)
        };

        private static readonly Type[] _array32Types = { typeof(TreeInstance) };

        private static readonly List<ushort> _array16Collected = new List<ushort>();
        private static readonly List<uint> _array32Collected = new List<uint>();

        private static int _list16Pointer, _list32Pointer;

        private static bool _collecting = false;
        private static bool _applying = false;

        public static ushort[] Collected16
        {
            get { return _array16Collected.ToArray(); }
        }

        public static uint[] Collected32
        {
            get { return _array32Collected.ToArray(); }
        }

        /// <summary>
        /// Start collecting ids from id reservations.
        /// </summary>
        public static void StartCollecting()
        {
            if (_collecting)
            {
                Log.Warn("ArrayHandler: Collecting already in progress! Ignoring.");
                return;
            }

            if (_applying)
            {
                Log.Warn("ArrayHandler: Started collecting while applying! Ignoring.");
                return;
            }

            _collecting = true;
            _array16Collected.Clear();
            _array32Collected.Clear();
        }

        /// <summary>
        /// Stop collecting ids. The result can be found in Collected16 and Collected32 properties.
        /// </summary>
        public static void StopCollecting()
        {
            _collecting = false;
        }

        /// <summary>
        /// Start to intercept id reservations and apply the given ids.
        /// </summary>
        /// <param name="array16Ids">Ids to apply to Array16 calls (can be null)</param>
        /// <param name="array32Ids">Ids to apply to Array32 calls (can be null)</param>
        public static void StartApplying(ushort[] array16Ids, uint[] array32Ids)
        {
            if (_collecting)
            {
                Log.Warn("ArrayHandler: Started applying while collecting! Ignoring.");
                return;
            }

            if (_applying)
            {
                Log.Warn("ArrayHandler: Applying already in progress! Ignoring.");
                return;
            }

            _applying = true;

            _array16Collected.Clear();
            if (array16Ids != null)
            {
                _array16Collected.AddRange(array16Ids);
            }

            _array32Collected.Clear();
            if (array32Ids != null)
            {
                _array32Collected.AddRange(array32Ids);
            }

            _list16Pointer = 0;
            _list32Pointer = 0;
        }

        /// <summary>
        /// Stop intercepting id reservations, will print a warning if not all ids were applied.
        /// </summary>
        public static void StopApplying()
        {
            _applying = false;
            if (_list16Pointer != _array16Collected.Count || _list32Pointer != _array32Collected.Count)
            {
                Log.Warn("ArrayHandler: Not all collected array elements have been applied!");
            }
        }

        [HarmonyPatch]
        public class CreateItemRand16
        {
            public static bool Prefix(ref ushort item, ref Randomizer r, object __instance, ref bool __result)
            {
                if (_applying)
                {
                    CreateItem16.Prefix(ref item, __instance, ref __result);

                    // Skip this randomizer value
                    r.Int32(1);

                    // Don't execute original method
                    return false;
                }
                return true;
            }

            public static void Postfix(ref ushort item)
            {
                if (_collecting)
                {
                    _array16Collected.Add(item);
                }
            }

            public static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (Type t in _array16Types)
                {
                    Type arr = typeof(Array16<>).MakeGenericType(t);
                    yield return arr.GetMethod("CreateItem", new Type[] { typeof(ushort).MakeByRefType(), typeof(Randomizer).MakeByRefType() });
                }
            }
        }

        [HarmonyPatch]
        public class CreateItem16
        {
            public static bool Prefix(ref ushort item, object __instance, ref bool __result)
            {
                if (_applying)
                {
                    if (_list16Pointer == _array16Collected.Count)
                    {
                        Log.Warn("ArrayHandler: Applying more items than collected!");
                        return true;
                    }

                    item = _array16Collected[_list16Pointer];
                    _list16Pointer++;

                    ArrayXExtension.RemoveUnused(__instance, item);

                    // Return true, id reservation was successful
                    __result = true;

                    // Don't execute original method
                    return false;
                }

                return true;
            }

            public static void Postfix(ref ushort item)
            {
                if (_collecting)
                {
                    _array16Collected.Add(item);
                }
            }

            public static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (Type t in _array16Types)
                {
                    Type arr = typeof(Array16<>).MakeGenericType(t);
                    yield return arr.GetMethod("CreateItem", new Type[] { typeof(ushort).MakeByRefType() });
                }
            }
        }

        [HarmonyPatch]
        public class CreateItemRand32
        {
            public static bool Prefix(ref uint item, ref Randomizer r, object __instance, ref bool __result)
            {
                if (_applying)
                {
                    CreateItem32.Prefix(ref item, __instance, ref __result);

                    // Skip this randomizer value
                    r.Int32(1);

                    // Don't execute original method
                    return false;
                }
                return true;
            }

            public static void Postfix(ref uint item)
            {
                if (_collecting)
                {
                    _array32Collected.Add(item);
                }
            }

            public static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (Type t in _array32Types)
                {
                    Type arr = typeof(Array32<>).MakeGenericType(t);
                    yield return arr.GetMethod("CreateItem", new Type[] { typeof(uint).MakeByRefType(), typeof(Randomizer).MakeByRefType() });
                }
            }
        }

        [HarmonyPatch]
        public class CreateItem32
        {
            public static bool Prefix(ref uint item, object __instance, ref bool __result)
            {
                if (_applying)
                {
                    if (_list32Pointer == _array32Collected.Count)
                    {
                        Log.Warn("ArrayHandler: Applying more items than collected!");
                        return true;
                    }

                    item = _array32Collected[_list32Pointer];
                    _list32Pointer++;

                    ArrayXExtension.RemoveUnused(__instance, item);

                    // Return true, id reservation was successful
                    __result = true;

                    // Don't execute original method
                    return false;
                }

                return true;
            }

            public static void Postfix(ref uint item)
            {
                if (_collecting)
                {
                    _array32Collected.Add(item);
                }
            }

            public static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (Type t in _array32Types)
                {
                    Type arr = typeof(Array32<>).MakeGenericType(t);
                    yield return arr.GetMethod("CreateItem", new Type[] { typeof(uint).MakeByRefType() });
                }
            }
        }
    }
}

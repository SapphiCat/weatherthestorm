using Vintagestory.API.Common;
using Vintagestory.GameContent;
using HarmonyLib;
using System.IO;
using System.Collections.Generic;
using Vintagestory.API.MathTools;
using System.Reflection.Emit;
using System;

namespace weatherthestorm;

[HarmonyPatch]
public class WeatherTheStorm : ModSystem
{

    private static Harmony harmony;
    private static ICoreAPI api;

    public override void Start(ICoreAPI api)
    {   
        base.Start(api);
        WeatherTheStorm.api = api;
        ConfigStep();
        HarmonyStep();
    }

    private void HarmonyStep() {
        // time bless wikis <3
        if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
            harmony = new Harmony(Mod.Info.ModID);
            harmony.PatchAll(); // Applies all harmony patches
        }
    }

    private void ConfigStep() {
        string configloc = Mod.Info.ModID + ".json";
        
        try {WeatherTheStormConfig.Loaded = api.LoadModConfig<WeatherTheStormConfig>(configloc) ?? throw new IOException();}
        catch {api.StoreModConfig<WeatherTheStormConfig>(WeatherTheStormConfig.Loaded, configloc);}
    }

    public override void Dispose()
    {
        harmony.UnpatchAll(Mod.Info.ModID);
        api.StoreModConfig<WeatherTheStormConfig>(WeatherTheStormConfig.Loaded, Mod.Info.ModID + ".json");
        base.Dispose();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(SystemTemporalStability), "trySpawnForPlayer")]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator genny) {
        CodeMatcher cody = new CodeMatcher(instructions, genny)
        .Start().MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S));
        return cody.SetInstruction(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte) Math.Max(WeatherTheStormConfig.Loaded.MaxStormDrifterSpawnDistance, (sbyte) cody.Instruction.operand)))
        .Start()
        .MatchStartForward(new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(CollisionTester), nameof(CollisionTester.IsColliding))))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchHelpers), nameof(PatchHelpers.IsColliding_ButCool))))
        .InstructionEnumeration();
    }


    public static class PatchHelpers {
        // still don't have a working IL viewer set up, so we'll just hijack a method call!
        // return true if the spawn should be blocked.
        public static bool IsColliding_ButCool(CollisionTester testy, IBlockAccessor blockAccessor, Cuboidf entityBoxRel, Vec3d pos, bool alsoCheckTouch = false)
        {
            IPlayer player = api.World.NearestPlayer(pos.X, pos.Y, pos.Z);
            if (player == null) return true;


            return  (WeatherTheStormConfig.Loaded.AddLightCheck && blockAccessor.GetLightLevel(pos.AsBlockPos, EnumLightLevelType.OnlyBlockLight) > WeatherTheStormConfig.Loaded.MaxStormDrifterSpawnLight) ||
                    (WeatherTheStormConfig.Loaded.AddDistanceCheck && (double)player.Entity.Pos.DistanceTo(pos) < (double)WeatherTheStormConfig.Loaded.MinStormDrifterSpawnDistance) ||
                    testy.IsColliding(blockAccessor, entityBoxRel, pos, alsoCheckTouch);
        }
    }


    public class WeatherTheStormConfig {
        [Newtonsoft.Json.JsonIgnore] public static WeatherTheStormConfig Loaded = new();
        public bool AddLightCheck = true;
        public int MaxStormDrifterSpawnLight = 7;
        public bool AddDistanceCheck = true;
        public double MinStormDrifterSpawnDistance = 12;

        [Newtonsoft.Json.JsonIgnore] public sbyte MaxStormDrifterSpawnDistance;

        public WeatherTheStormConfig() {
            // cubed root of 2; should guarantee at least a half of the area is spawnable.
            // accounting for ground and the min range being spherical though... anyone's guess.
            this.MaxStormDrifterSpawnDistance = (sbyte) Math.Ceiling(MinStormDrifterSpawnDistance * 1.129);
        }
    }
}


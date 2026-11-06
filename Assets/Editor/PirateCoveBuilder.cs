#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

// Menu: Pirate Cove > Build Entire Game
// Builds tags, materials, prefabs, the MainMenu scene and the Game (endless runner) scene, and sets Build Settings.
public static class PirateCoveBuilder
{
    // ---- palette ----
    static readonly Color Gold = new Color(1f, 0.79f, 0.24f), Wood = new Color(0.55f, 0.35f, 0.17f),
        Dark = new Color(0.17f, 0.1f, 0.05f), Parchment = new Color(0.96f, 0.9f, 0.72f),
        Red = new Color(0.9f, 0.1f, 0.15f), Green = new Color(0.25f, 0.7f, 0.25f), White = Color.white;

    // ---- materials ----
    static Material mWood, mWoodAlt, mWoodDark, mRope, mTrunk, mLeaves, mGold, mChest, mStar, mBarrel, mBand, mGrey, mCrate, mSea, mHull, mDeck, mSail, mBlack, mWhite, mLantern, mSkin, mShirt, mVest, mPants, mBoot, mBandana, mCoat, mNavy, mRock, mSand, mEye, mSkull, mMoon, mStarDot, mCloud, mRain;
    // ---- prefabs ----
    static GameObject pSegment, pBarrel, pSpikes, pCrate, pStack, pCannon, pCoin, pChest, pStar, pIsland, pBall, pSplash;

    [MenuItem("Pirate Cove/Build Entire Game")]
    public static void Build()
    {
        if (!EditorUtility.DisplayDialog("Pirate Cove",
            "This builds the MainMenu and Game scenes, prefabs and materials.\n\nImport TMP Essentials first if you have not (Window > TextMeshPro > Import TMP Essential Resources).\n\nContinue?",
            "Build", "Cancel")) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EnsureTag("Hazard"); EnsureTag("Collectible");
        EnsureFolder("Prefabs"); EnsureFolder("Materials"); EnsureFolder("Scenes");

        var tmp = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        MakeMaterials();
        MakePrefabs();

        BuildGameScene();
        BuildMenuScene();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true)
        };
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Pirate Cove", "Done!\n\nThe MainMenu scene is open. Press Play.\nArrows / A-D = switch lanes, Space = jump, Esc = pause.", "OK");
    }

    // ================= materials & prefabs =================
    static Material Glow(string name, string hex, float k)
    {
        var m = Mat(name, hex);
        m.EnableKeyword("_EMISSION");
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        m.SetColor("_EmissionColor", c * k);
        EditorUtility.SetDirty(m);
        return m;
    }

    static Material Shiny(Material m, float gloss)
    {
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", gloss);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", gloss);
        EditorUtility.SetDirty(m);
        return m;
    }

    static void MakeMaterials()
    {
        mWood = Mat("Plank", "A9743F"); mWoodAlt = Mat("PlankAlt", "94632F"); mWoodDark = Mat("WoodDark", "4E2F17"); mRope = Mat("Rope", "CDB27A");
        mTrunk = Mat("Trunk", "6B4423"); mLeaves = Mat("Leaves", "2F9E44"); mGold = Shiny(Glow("Gold", "FFC93C", 1.2f), 0.8f);
        mChest = Mat("Chest", "8B5A2B"); mStar = Glow("StarYellow", "FFE14D", 1.5f); mBarrel = Mat("Barrel", "7B4A1E");
        mBand = Shiny(Mat("Band", "55585C"), 0.6f); mGrey = Shiny(Mat("Spike", "9A9EA3"), 0.7f); mCrate = Mat("Crate", "B8863B");
        mSea = Shiny(Mat("Sea", "1E88B8"), 0.92f); mHull = Mat("Hull", "5A3519"); mDeck = Mat("Deck", "A5743C");
        mSail = Mat("Sail", "EFE6CC"); mBlack = Mat("Black", "161616"); mWhite = Mat("White", "F2F2F2"); mLantern = Glow("Lantern", "FFB547", 2f);
        mSkin = Mat("Skin", "E0AC82"); mShirt = Mat("Shirt", "F0EAD6"); mVest = Mat("Vest", "6B3A1E"); mPants = Mat("Pants", "2B2F3A");
        mBoot = Mat("Boot", "1C1410"); mBandana = Mat("Bandana", "B3202A"); mCoat = Mat("Coat", "8E1B1B"); mNavy = Mat("Navy", "1F3A5F");
        mRock = Mat("RockGrey", "6E6A66"); mSand = Mat("SandIsle", "E3D29C"); mEye = Glow("SkullEye", "FF8A1F", 3f); mSkull = Mat("SkullRock", "8C877F");
        mMoon = Glow("Moon", "EFF3FF", 2f); mStarDot = Glow("StarDot", "FFFFFF", 3f); mCloud = Mat("Cloud", "FFFFFF"); mRain = Glow("Rain", "BFD4FF", 1.5f);
    }

    static void MakePrefabs()
    {
        // ---- pier segment (origin at start, runs along +Z, length 20) ----
        var seg = new GameObject("TrackSegment");
        for (int i = 0; i < 10; i++)
            Add(seg, Prim(PrimitiveType.Cube, "Plank" + i, new Vector3(0, -0.25f, i * 2f + 1f), Vector3.zero, new Vector3(7.4f, 0.5f, 1.94f), i % 2 == 0 ? mWood : mWoodAlt));
        Add(seg, Prim(PrimitiveType.Cube, "BeamL", new Vector3(-3.2f, -0.6f, 10), Vector3.zero, new Vector3(0.35f, 0.3f, 20), mWoodDark));
        Add(seg, Prim(PrimitiveType.Cube, "BeamR", new Vector3(3.2f, -0.6f, 10), Vector3.zero, new Vector3(0.35f, 0.3f, 20), mWoodDark));
        foreach (float z in new[] { 2.5f, 7.5f, 12.5f, 17.5f })
        {
            Add(seg, Prim(PrimitiveType.Cylinder, "PostL", new Vector3(-3.7f, 0.6f, z), Vector3.zero, new Vector3(0.3f, 0.6f, 0.3f), mWoodDark));
            Add(seg, Prim(PrimitiveType.Cylinder, "PostR", new Vector3(3.7f, 0.6f, z), Vector3.zero, new Vector3(0.3f, 0.6f, 0.3f), mWoodDark));
            Add(seg, Prim(PrimitiveType.Cylinder, "PileL", new Vector3(-3.2f, -1.2f, z), Vector3.zero, new Vector3(0.3f, 0.8f, 0.3f), mWoodDark));
            Add(seg, Prim(PrimitiveType.Cylinder, "PileR", new Vector3(3.2f, -1.2f, z), Vector3.zero, new Vector3(0.3f, 0.8f, 0.3f), mWoodDark));
        }
        foreach (float z in new[] { 0f, 5f, 10f, 15f })
        {
            Add(seg, Prim(PrimitiveType.Cylinder, "RopeL", new Vector3(-3.7f, 0.95f, z), new Vector3(90, 0, 0), new Vector3(0.07f, 2.5f, 0.07f), mRope));
            Add(seg, Prim(PrimitiveType.Cylinder, "RopeR", new Vector3(3.7f, 0.95f, z), new Vector3(90, 0, 0), new Vector3(0.07f, 2.5f, 0.07f), mRope));
        }
        Add(seg, Prim(PrimitiveType.Sphere, "LampL1", new Vector3(-3.7f, 1.4f, 2.5f), Vector3.zero, Vector3.one * 0.28f, mLantern));
        Add(seg, Prim(PrimitiveType.Sphere, "LampL2", new Vector3(-3.7f, 1.4f, 12.5f), Vector3.zero, Vector3.one * 0.28f, mLantern));
        Add(seg, Prim(PrimitiveType.Sphere, "LampR1", new Vector3(3.7f, 1.4f, 7.5f), Vector3.zero, Vector3.one * 0.28f, mLantern));
        Add(seg, Prim(PrimitiveType.Sphere, "LampR2", new Vector3(3.7f, 1.4f, 17.5f), Vector3.zero, Vector3.one * 0.28f, mLantern));
        // Jolly Roger flag
        Add(seg, Prim(PrimitiveType.Cylinder, "FlagPole", new Vector3(-3.95f, 1.9f, 12.5f), Vector3.zero, new Vector3(0.08f, 1.9f, 0.08f), mWoodDark));
        Add(seg, Prim(PrimitiveType.Cube, "Flag", new Vector3(-3.3f, 3.2f, 12.5f), Vector3.zero, new Vector3(1.2f, 0.75f, 0.04f), mBlack));
        Add(seg, Prim(PrimitiveType.Sphere, "SkullF", new Vector3(-3.3f, 3.25f, 12.46f), Vector3.zero, new Vector3(0.3f, 0.3f, 0.05f), mWhite));
        Add(seg, Prim(PrimitiveType.Sphere, "SkullB", new Vector3(-3.3f, 3.25f, 12.54f), Vector3.zero, new Vector3(0.3f, 0.3f, 0.05f), mWhite));
        StripColliders(seg);
        pSegment = SavePrefab(seg, "TrackSegment");

        // ---- obstacles (tag Hazard, trigger) ----
        var barrel = new GameObject("Barrel"); barrel.tag = "Hazard";
        Add(barrel, Prim(PrimitiveType.Cylinder, "Body", new Vector3(0, 0.55f, 0), Vector3.zero, new Vector3(0.9f, 0.55f, 0.9f), mBarrel));
        Add(barrel, Prim(PrimitiveType.Cylinder, "BandA", new Vector3(0, 0.3f, 0), Vector3.zero, new Vector3(0.95f, 0.04f, 0.95f), mBand));
        Add(barrel, Prim(PrimitiveType.Cylinder, "BandB", new Vector3(0, 0.8f, 0), Vector3.zero, new Vector3(0.95f, 0.04f, 0.95f), mBand));
        Add(barrel, Prim(PrimitiveType.Cylinder, "Lid", new Vector3(0, 1.1f, 0), Vector3.zero, new Vector3(0.8f, 0.02f, 0.8f), mWoodDark));
        StripColliders(barrel);
        var cc = barrel.AddComponent<CapsuleCollider>(); cc.isTrigger = true; cc.center = new Vector3(0, 0.55f, 0); cc.radius = 0.45f; cc.height = 1.1f;
        pBarrel = SavePrefab(barrel, "Barrel");

        var spikes = new GameObject("Spikes"); spikes.tag = "Hazard";
        Add(spikes, Prim(PrimitiveType.Cube, "Base", new Vector3(0, 0.05f, 0), Vector3.zero, new Vector3(1.6f, 0.1f, 0.7f), mWoodDark));
        foreach (float x in new[] { -0.55f, -0.18f, 0.18f, 0.55f })
            Add(spikes, Prim(PrimitiveType.Cube, "Spike", new Vector3(x, 0.4f, 0), new Vector3(0, 45, 0), new Vector3(0.24f, 0.7f, 0.24f), mGrey));
        StripColliders(spikes);
        var bc = spikes.AddComponent<BoxCollider>(); bc.isTrigger = true; bc.center = new Vector3(0, 0.35f, 0); bc.size = new Vector3(1.7f, 0.8f, 0.6f);
        pSpikes = SavePrefab(spikes, "Spikes");

        var crate = new GameObject("Crate"); crate.tag = "Hazard";
        Add(crate, Prim(PrimitiveType.Cube, "Body", new Vector3(0, 0.4f, 0), Vector3.zero, new Vector3(1.2f, 0.8f, 1.2f), mCrate));
        Add(crate, Prim(PrimitiveType.Cube, "FrameB", new Vector3(0, 0.05f, 0), Vector3.zero, new Vector3(1.25f, 0.1f, 1.25f), mWoodDark));
        Add(crate, Prim(PrimitiveType.Cube, "FrameT", new Vector3(0, 0.75f, 0), Vector3.zero, new Vector3(1.25f, 0.1f, 1.25f), mWoodDark));
        foreach (float zz in new[] { -0.61f, 0.61f })
            Add(crate, Prim(PrimitiveType.Cube, "Diag", new Vector3(0, 0.4f, zz), new Vector3(0, 0, 38), new Vector3(1.45f, 0.1f, 0.04f), mWoodDark));
        StripColliders(crate);
        var cb = crate.AddComponent<BoxCollider>(); cb.isTrigger = true; cb.center = new Vector3(0, 0.4f, 0); cb.size = new Vector3(1.25f, 0.8f, 1.25f);
        pCrate = SavePrefab(crate, "Crate");

        var stack = new GameObject("CrateStack"); stack.tag = "Hazard";
        Add(stack, Prim(PrimitiveType.Cube, "Low", new Vector3(0, 0.55f, 0), Vector3.zero, new Vector3(1.4f, 1.1f, 1f), mCrate));
        Add(stack, Prim(PrimitiveType.Cube, "High", new Vector3(0.05f, 1.65f, 0), new Vector3(0, 10, 0), new Vector3(1.2f, 1.0f, 0.9f), mCrate));
        foreach (float zz in new[] { -0.51f, 0.51f })
            Add(stack, Prim(PrimitiveType.Cube, "Diag", new Vector3(0, 0.55f, zz), new Vector3(0, 0, 35), new Vector3(1.6f, 0.1f, 0.04f), mWoodDark));
        StripColliders(stack);
        var sb = stack.AddComponent<BoxCollider>(); sb.isTrigger = true; sb.center = new Vector3(0, 1.1f, 0); sb.size = new Vector3(1.4f, 2.2f, 1f);
        pStack = SavePrefab(stack, "CrateStack");

        var cannon = new GameObject("Cannon"); cannon.tag = "Hazard";
        Add(cannon, Prim(PrimitiveType.Cube, "Cart", new Vector3(0, 0.45f, 0), Vector3.zero, new Vector3(0.9f, 0.25f, 1.3f), mWoodDark));
        Add(cannon, Prim(PrimitiveType.Cylinder, "Barrel", new Vector3(0, 0.8f, 0), new Vector3(90, 0, 0), new Vector3(0.55f, 0.9f, 0.55f), mBlack));
        Add(cannon, Prim(PrimitiveType.Cylinder, "Ring", new Vector3(0, 0.8f, 0.85f), new Vector3(90, 0, 0), new Vector3(0.64f, 0.06f, 0.64f), mBand));
        Add(cannon, Prim(PrimitiveType.Cylinder, "WheelL", new Vector3(-0.55f, 0.4f, 0), new Vector3(0, 0, 90), new Vector3(0.8f, 0.08f, 0.8f), mWoodDark));
        Add(cannon, Prim(PrimitiveType.Cylinder, "WheelR", new Vector3(0.55f, 0.4f, 0), new Vector3(0, 0, 90), new Vector3(0.8f, 0.08f, 0.8f), mWoodDark));
        StripColliders(cannon);
        var kb = cannon.AddComponent<BoxCollider>(); kb.isTrigger = true; kb.center = new Vector3(0, 0.55f, 0); kb.size = new Vector3(1.4f, 1.1f, 1.9f);
        pCannon = SavePrefab(cannon, "Cannon");

        // ---- pickups (tag Collectible, trigger) ----
        var coin = Prim(PrimitiveType.Cylinder, "Coin", new Vector3(0, 1f, 0), new Vector3(90, 0, 0), new Vector3(0.6f, 0.05f, 0.6f), mGold);
        coin.tag = "Collectible"; Object.DestroyImmediate(coin.GetComponent<Collider>());
        var sc = coin.AddComponent<SphereCollider>(); sc.isTrigger = true; sc.radius = 1f;
        var pk = coin.AddComponent<Pickup>(); pk.kind = Pickup.Kind.Coin; pk.value = 10;
        pCoin = SavePrefab(coin, "Coin");

        var chest = new GameObject("Chest"); chest.tag = "Collectible";
        Add(chest, Prim(PrimitiveType.Cube, "Body", new Vector3(0, 0.25f, 0), Vector3.zero, new Vector3(0.9f, 0.5f, 0.6f), mChest));
        Add(chest, Prim(PrimitiveType.Cylinder, "Lid", new Vector3(0, 0.5f, 0), new Vector3(0, 0, 90), new Vector3(0.6f, 0.45f, 0.6f), mChest));
        Add(chest, Prim(PrimitiveType.Cube, "BandL", new Vector3(-0.3f, 0.4f, 0), Vector3.zero, new Vector3(0.08f, 0.62f, 0.64f), mGold));
        Add(chest, Prim(PrimitiveType.Cube, "BandR", new Vector3(0.3f, 0.4f, 0), Vector3.zero, new Vector3(0.08f, 0.62f, 0.64f), mGold));
        Add(chest, Prim(PrimitiveType.Cube, "Lock", new Vector3(0, 0.45f, 0.31f), Vector3.zero, new Vector3(0.14f, 0.16f, 0.04f), mGold));
        foreach (float x in new[] { -0.2f, 0f, 0.2f })
            Add(chest, Prim(PrimitiveType.Sphere, "Gold", new Vector3(x, 0.86f, 0), Vector3.zero, new Vector3(0.22f, 0.14f, 0.22f), mGold));
        StripColliders(chest);
        var chb = chest.AddComponent<BoxCollider>(); chb.isTrigger = true; chb.center = new Vector3(0, 0.45f, 0); chb.size = new Vector3(1f, 0.9f, 0.7f);
        pk = chest.AddComponent<Pickup>(); pk.kind = Pickup.Kind.Chest; pk.value = 50; pk.spin = false;
        pChest = SavePrefab(chest, "Chest");

        var star = Prim(PrimitiveType.Cube, "Star", new Vector3(0, 1.3f, 0), new Vector3(45, 0, 45), Vector3.one * 0.6f, mStar);
        star.tag = "Collectible"; star.GetComponent<Collider>().isTrigger = true;
        pk = star.AddComponent<Pickup>(); pk.kind = Pickup.Kind.Star; pk.value = 1;
        pStar = SavePrefab(star, "Star");

        // ---- cannon fire + splash ----
        var splash = Prim(PrimitiveType.Sphere, "Splash", Vector3.zero, Vector3.zero, new Vector3(0.4f, 0.6f, 0.4f), mWhite);
        Object.DestroyImmediate(splash.GetComponent<Collider>()); splash.AddComponent<SplashFx>();
        pSplash = SavePrefab(splash, "Splash");
        var ball = Prim(PrimitiveType.Sphere, "CannonBall", Vector3.zero, Vector3.zero, Vector3.one * 0.5f, mBlack);
        Object.DestroyImmediate(ball.GetComponent<Collider>());
        ball.AddComponent<CannonBall>().splashPrefab = pSplash;
        pBall = SavePrefab(ball, "CannonBall");

        // ---- treasure island ----
        var isl = new GameObject("Island");
        Add(isl, Prim(PrimitiveType.Sphere, "Mound", Vector3.zero, Vector3.zero, new Vector3(7f, 2.2f, 6f), mSand));
        Add(isl, Prim(PrimitiveType.Sphere, "MoundTop", new Vector3(1f, 0.5f, 0.5f), Vector3.zero, new Vector3(3.5f, 1.2f, 3f), mSand));
        BuildPalm(isl, new Vector3(-2f, 0.9f, 0f), 0);
        BuildPalm(isl, new Vector3(1.8f, 1.0f, 1.2f), 1);
        Add(isl, Prim(PrimitiveType.Sphere, "Rock1", new Vector3(2.8f, 0.3f, -1.8f), Vector3.zero, new Vector3(1.3f, 1f, 1.2f), mRock));
        Add(isl, Prim(PrimitiveType.Sphere, "Rock2", new Vector3(-3f, 0.1f, 2.2f), Vector3.zero, new Vector3(1f, 0.8f, 0.9f), mRock));
        BuildGoldPile(isl, new Vector3(0.2f, 1.0f, -0.9f));
        Add(isl, Prim(PrimitiveType.Cube, "ChestBody", new Vector3(-0.9f, 1.15f, -1.2f), new Vector3(0, 25, 0), new Vector3(0.9f, 0.5f, 0.6f), mChest));
        Add(isl, Prim(PrimitiveType.Cylinder, "ChestLid", new Vector3(-0.9f, 1.4f, -1.2f), new Vector3(0, 25, 90), new Vector3(0.6f, 0.45f, 0.6f), mChest));
        Add(isl, Prim(PrimitiveType.Cylinder, "FlagPole", new Vector3(0.6f, 2.4f, 0.8f), Vector3.zero, new Vector3(0.07f, 1.5f, 0.07f), mWoodDark));
        Add(isl, Prim(PrimitiveType.Cube, "Flag", new Vector3(1.1f, 3.5f, 0.8f), Vector3.zero, new Vector3(1f, 0.6f, 0.04f), mBlack));
        StripColliders(isl);
        pIsland = SavePrefab(isl, "Island");
    }

    static void BuildPalm(GameObject parent, Vector3 basePos, int variant)
    {
        float h = 3.2f + variant * 0.6f;
        var tilt = Quaternion.Euler(0, 0, variant == 0 ? 8f : -8f);
        Vector3 top = basePos + tilt * (Vector3.up * h);
        var trunk = Prim(PrimitiveType.Cylinder, "Trunk", basePos + tilt * (Vector3.up * h * 0.5f), tilt.eulerAngles, new Vector3(0.28f, h * 0.5f, 0.28f), mTrunk);
        Add(parent, trunk);
        for (int k = 0; k < 7; k++)
        {
            Vector3 dir = Quaternion.Euler(0, k * 51.4f, 0) * Vector3.forward;
            var f = Prim(PrimitiveType.Cube, "Frond", top + dir * 0.9f + Vector3.down * 0.2f, Vector3.zero, new Vector3(0.7f, 0.05f, 2f), mLeaves);
            f.transform.rotation = Quaternion.LookRotation(dir + Vector3.down * 0.45f);
            Add(parent, f);
        }
        Add(parent, Prim(PrimitiveType.Sphere, "Coconut1", top + new Vector3(0.12f, -0.1f, 0), Vector3.zero, Vector3.one * 0.25f, mTrunk));
        Add(parent, Prim(PrimitiveType.Sphere, "Coconut2", top + new Vector3(-0.1f, -0.15f, 0.1f), Vector3.zero, Vector3.one * 0.25f, mTrunk));
    }

    static void BuildGoldPile(GameObject parent, Vector3 pos)
    {
        for (int i = 0; i < 12; i++)
        {
            float a = i * 0.63f, r = 0.15f + (i % 4) * 0.16f;
            Add(parent, Prim(PrimitiveType.Sphere, "Gold" + i, pos + new Vector3(Mathf.Cos(a) * r, 0.08f + (3 - i % 4) * 0.1f, Mathf.Sin(a) * r), Vector3.zero, new Vector3(0.38f, 0.26f, 0.38f), mGold));
        }
        for (int i = 0; i < 5; i++)
            Add(parent, Prim(PrimitiveType.Cylinder, "CoinDisc" + i, pos + new Vector3(Mathf.Cos(i * 1.3f) * 0.7f, 0.06f, Mathf.Sin(i * 1.3f) * 0.7f), new Vector3(15 * i, 20 * i, 10), new Vector3(0.28f, 0.015f, 0.28f), mGold));
    }

    // ---- a full pirate figure with swinging limbs (hero or chaser) ----
    static Transform Limb(GameObject root, string name, Vector3 hip, float len, float width, Material m)
    {
        var pivot = new GameObject(name);
        pivot.transform.SetParent(root.transform, false);
        pivot.transform.localPosition = hip;
        var mesh = Prim(PrimitiveType.Capsule, name + "Mesh", Vector3.zero, Vector3.zero, Vector3.one, m);
        mesh.transform.SetParent(pivot.transform, false);
        mesh.transform.localPosition = new Vector3(0, -len / 2f, 0);
        mesh.transform.localScale = new Vector3(width, len / 2f, width);
        return pivot.transform;
    }

    static GameObject BuildPirate(string name, Material shirt, Material vest, Material hat, bool villain)
    {
        var r = new GameObject(name);
        Add(r, Prim(PrimitiveType.Capsule, "Torso", new Vector3(0, 1.2f, 0), Vector3.zero, new Vector3(0.52f, 0.34f, 0.34f), vest));
        Add(r, Prim(PrimitiveType.Cylinder, "Belt", new Vector3(0, 0.9f, 0), Vector3.zero, new Vector3(0.5f, 0.035f, 0.36f), mBoot));
        Add(r, Prim(PrimitiveType.Cube, "Buckle", new Vector3(0, 0.9f, 0.19f), Vector3.zero, new Vector3(0.1f, 0.1f, 0.03f), mGold));
        Add(r, Prim(PrimitiveType.Sphere, "Head", new Vector3(0, 1.82f, 0), Vector3.zero, Vector3.one * 0.42f, mSkin));
        Add(r, Prim(PrimitiveType.Cylinder, "Bandana", new Vector3(0, 1.97f, 0), Vector3.zero, new Vector3(0.44f, 0.03f, 0.44f), mBandana));
        Add(r, Prim(PrimitiveType.Cylinder, "HatBrim", new Vector3(0, 2.04f, 0), Vector3.zero, new Vector3(0.8f, 0.025f, 0.8f), hat));
        Add(r, Prim(PrimitiveType.Cylinder, "HatCrown", new Vector3(0, 2.14f, 0), Vector3.zero, new Vector3(0.42f, 0.1f, 0.42f), hat));
        Add(r, Prim(PrimitiveType.Sphere, "HatSkull", new Vector3(0, 2.1f, 0.4f), Vector3.zero, new Vector3(0.14f, 0.14f, 0.03f), mWhite));
        if (villain)
        {
            Add(r, Prim(PrimitiveType.Sphere, "Beard", new Vector3(0, 1.66f, 0.12f), Vector3.zero, new Vector3(0.3f, 0.22f, 0.25f), mBoot));
            Add(r, Prim(PrimitiveType.Sphere, "Patch", new Vector3(-0.09f, 1.86f, 0.19f), Vector3.zero, new Vector3(0.11f, 0.11f, 0.03f), mBlack));
        }
        var legL = Limb(r, "LegL", new Vector3(-0.15f, 0.8f, 0), 0.8f, 0.2f, mPants);
        var legR = Limb(r, "LegR", new Vector3(0.15f, 0.8f, 0), 0.8f, 0.2f, mPants);
        foreach (var leg in new[] { legL, legR })
        {
            var boot = Prim(PrimitiveType.Cube, "Boot", Vector3.zero, Vector3.zero, new Vector3(0.24f, 0.14f, 0.34f), mBoot);
            boot.transform.SetParent(leg, false); boot.transform.localPosition = new Vector3(0, -0.76f, 0.05f);
        }
        var armL = Limb(r, "ArmL", new Vector3(-0.38f, 1.5f, 0), 0.6f, 0.16f, shirt);
        var armR = Limb(r, "ArmR", new Vector3(0.38f, 1.5f, 0), 0.6f, 0.16f, shirt);
        foreach (var arm in new[] { armL, armR })
        {
            var hand = Prim(PrimitiveType.Sphere, "Hand", Vector3.zero, Vector3.zero, Vector3.one * 0.17f, mSkin);
            hand.transform.SetParent(arm, false); hand.transform.localPosition = new Vector3(0, -0.62f, 0);
        }
        if (villain)
        {
            var blade = Prim(PrimitiveType.Cube, "Cutlass", Vector3.zero, Vector3.zero, new Vector3(0.05f, 0.75f, 0.1f), mGrey);
            blade.transform.SetParent(armR, false); blade.transform.localPosition = new Vector3(0, -0.95f, 0.1f);
            var hilt = Prim(PrimitiveType.Cube, "Hilt", Vector3.zero, Vector3.zero, new Vector3(0.2f, 0.05f, 0.08f), mGold);
            hilt.transform.SetParent(armR, false); hilt.transform.localPosition = new Vector3(0, -0.62f, 0.1f);
        }
        StripColliders(r);
        var anim = r.AddComponent<RunAnimator>();
        anim.legL = legL; anim.legR = legR; anim.armL = armL; anim.armR = armR;
        return r;
    }

    // ---- pirate galleon ----
    static GameObject BuildShip(string name, Vector3 pos, float yaw)
    {
        var s = new GameObject(name);
        Add(s, Prim(PrimitiveType.Cube, "Hull", new Vector3(0, 0.9f, 0), Vector3.zero, new Vector3(4.2f, 1.8f, 11f), mHull));
        Add(s, Prim(PrimitiveType.Cube, "BowL", new Vector3(-0.95f, 0.9f, 6.3f), new Vector3(0, 35, 0), new Vector3(2.6f, 1.8f, 2.2f), mHull));
        Add(s, Prim(PrimitiveType.Cube, "BowR", new Vector3(0.95f, 0.9f, 6.3f), new Vector3(0, -35, 0), new Vector3(2.6f, 1.8f, 2.2f), mHull));
        Add(s, Prim(PrimitiveType.Cube, "Bowsprit", new Vector3(0, 1.9f, 8f), new Vector3(-15, 0, 0), new Vector3(0.25f, 0.25f, 3.2f), mWoodDark));
        Add(s, Prim(PrimitiveType.Cube, "Stern", new Vector3(0, 1.9f, -4.2f), Vector3.zero, new Vector3(4.2f, 2f, 2.8f), mHull));
        Add(s, Prim(PrimitiveType.Cube, "Deck", new Vector3(0, 1.82f, 0.4f), Vector3.zero, new Vector3(3.8f, 0.1f, 8f), mDeck));
        Add(s, Prim(PrimitiveType.Cube, "TrimL", new Vector3(-2.12f, 1.4f, 0), Vector3.zero, new Vector3(0.08f, 0.14f, 11.2f), mGold));
        Add(s, Prim(PrimitiveType.Cube, "TrimR", new Vector3(2.12f, 1.4f, 0), Vector3.zero, new Vector3(0.08f, 0.14f, 11.2f), mGold));
        Add(s, Prim(PrimitiveType.Cube, "WindowA", new Vector3(-0.9f, 2.1f, -5.62f), Vector3.zero, new Vector3(0.7f, 0.5f, 0.05f), mLantern));
        Add(s, Prim(PrimitiveType.Cube, "WindowB", new Vector3(0.9f, 2.1f, -5.62f), Vector3.zero, new Vector3(0.7f, 0.5f, 0.05f), mLantern));
        Add(s, Prim(PrimitiveType.Cylinder, "Mast1", new Vector3(0, 5.2f, 1.8f), Vector3.zero, new Vector3(0.25f, 3.5f, 0.25f), mWoodDark));
        Add(s, Prim(PrimitiveType.Cylinder, "Mast2", new Vector3(0, 5f, -1.5f), Vector3.zero, new Vector3(0.22f, 3f, 0.22f), mWoodDark));
        var sail1 = Prim(PrimitiveType.Cube, "Sail1", new Vector3(0, 5.8f, 1.8f), new Vector3(0, 30, 0), new Vector3(3.6f, 3.2f, 0.12f), mSail);
        var sail2 = Prim(PrimitiveType.Cube, "Sail2", new Vector3(0, 5.4f, -1.5f), new Vector3(0, 30, 0), new Vector3(3.2f, 2.6f, 0.12f), mSail);
        Add(s, sail1); Add(s, sail2);
        Vector3 n = Quaternion.Euler(0, 30, 0) * Vector3.forward;
        var skull = Prim(PrimitiveType.Sphere, "Skull", new Vector3(0, 6.2f, 1.8f) + n * 0.1f, new Vector3(0, 30, 0), new Vector3(0.9f, 0.9f, 0.08f), mBlack);
        var b1 = Prim(PrimitiveType.Cube, "Bone1", new Vector3(0, 5.2f, 1.8f) + n * 0.1f, new Vector3(0, 30, 30), new Vector3(1.8f, 0.14f, 0.06f), mBlack);
        var b2 = Prim(PrimitiveType.Cube, "Bone2", new Vector3(0, 5.2f, 1.8f) + n * 0.1f, new Vector3(0, 30, -30), new Vector3(1.8f, 0.14f, 0.06f), mBlack);
        Add(s, skull); Add(s, b1); Add(s, b2);
        Add(s, Prim(PrimitiveType.Cube, "Flag", new Vector3(0.5f, 9f, 1.8f), new Vector3(0, 30, 0), new Vector3(1.1f, 0.65f, 0.04f), mBlack));
        Add(s, Prim(PrimitiveType.Sphere, "LampL", new Vector3(-1.7f, 3.1f, -5.4f), Vector3.zero, Vector3.one * 0.3f, mLantern));
        Add(s, Prim(PrimitiveType.Sphere, "LampR", new Vector3(1.7f, 3.1f, -5.4f), Vector3.zero, Vector3.one * 0.3f, mLantern));
        var muzzles = new System.Collections.Generic.List<Transform>();
        foreach (float side in new[] { -1f, 1f })
            foreach (float z in new[] { -2f, 0.8f, 3.2f })
            {
                Add(s, Prim(PrimitiveType.Cylinder, "Cannon", new Vector3(side * 2.15f, 1.2f, z), new Vector3(0, 0, 90), new Vector3(0.28f, 0.45f, 0.28f), mBlack));
                var m = new GameObject("Muzzle"); m.transform.position = new Vector3(side * 2.7f, 1.2f, z); m.transform.SetParent(s.transform, true);
                muzzles.Add(m.transform);
            }
        StripColliders(s);
        var sc = s.AddComponent<ShipCannons>(); sc.muzzles = muzzles.ToArray(); sc.ballPrefab = pBall;
        s.transform.localScale = Vector3.one * 1.4f;
        s.transform.position = pos; s.transform.eulerAngles = new Vector3(0, yaw, 0);
        return s;
    }

    static GameObject BuildSkullRock()
    {
        var r = new GameObject("SkullRock");
        Add(r, Prim(PrimitiveType.Sphere, "Cranium", new Vector3(0, 6f, 0), Vector3.zero, new Vector3(14f, 12f, 12f), mSkull));
        Add(r, Prim(PrimitiveType.Sphere, "Jaw", new Vector3(0, 1.8f, -1.5f), Vector3.zero, new Vector3(9f, 4f, 8f), mSkull));
        foreach (float x in new[] { -3.2f, 3.2f })
        {
            Add(r, Prim(PrimitiveType.Sphere, "Socket", new Vector3(x, 7f, -5.4f), Vector3.zero, new Vector3(3f, 3.4f, 2f), mBlack));
            Add(r, Prim(PrimitiveType.Sphere, "Eye", new Vector3(x, 7f, -6.2f), Vector3.zero, new Vector3(1.9f, 2.2f, 1f), mEye));
        }
        Add(r, Prim(PrimitiveType.Sphere, "Nose", new Vector3(0, 4.6f, -5.8f), Vector3.zero, new Vector3(1.4f, 2f, 1f), mBlack));
        foreach (float x in new[] { -2.4f, -1.2f, 0f, 1.2f, 2.4f })
            Add(r, Prim(PrimitiveType.Cube, "Tooth", new Vector3(x, 1.7f, -5.4f), Vector3.zero, new Vector3(1.1f, 1.6f, 0.9f), mWhite));
        StripColliders(r);
        return r;
    }

    static GameObject BuildSea()
    {
        var sea = Prim(PrimitiveType.Plane, "Sea", new Vector3(0, -1.5f, 0), Vector3.zero, new Vector3(30, 1, 30), mSea);
        Object.DestroyImmediate(sea.GetComponent<Collider>());
        return sea;
    }

    static void Atmosphere(Camera cam)
    {
        cam.clearFlags = CameraClearFlags.Skybox;
        var shader = Shader.Find("Skybox/Procedural");
        if (shader != null)
        {
            var sky = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PirateSky.mat");
            if (sky == null) { sky = new Material(shader); AssetDatabase.CreateAsset(sky, "Assets/Materials/PirateSky.mat"); }
            sky.SetFloat("_SunSize", 0.05f); sky.SetFloat("_SunSizeConvergence", 5f);
            sky.SetFloat("_AtmosphereThickness", 1.3f); sky.SetFloat("_Exposure", 1.25f);
            sky.SetColor("_SkyTint", new Color(0.55f, 0.62f, 0.8f)); sky.SetColor("_GroundColor", new Color(0.45f, 0.5f, 0.55f));
            EditorUtility.SetDirty(sky);
            RenderSettings.skybox = sky;
        }
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.005f; RenderSettings.fogColor = new Color(0.72f, 0.82f, 0.92f);
        var light = Object.FindFirstObjectByType<Light>();
        if (light != null)
        {
            light.color = new Color(1f, 0.92f, 0.78f); light.intensity = 1.5f; light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(38, -35, 0); RenderSettings.sun = light;
        }
        DynamicGI.UpdateEnvironment();
    }

    static void AddShip(string name, Vector3 offset, float yaw)
    {
        var s = BuildShip(name, new Vector3(offset.x, -1.9f, offset.z), yaw);
        var f = s.AddComponent<FollowPlayer>();
        f.offset = offset; f.driftAmplitude = 8f; f.driftSpeed = 0.12f; f.bobAmplitude = 0.25f; f.bobSpeed = 1f; f.rollDegrees = 3f;
    }

    // ================= Game scene =================
    static void BuildGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var cam = Camera.main;
        cam.transform.position = new Vector3(0, 4.5f, -7);
        cam.transform.rotation = Quaternion.Euler(18, 0, 0);
        Atmosphere(cam);
        var sea = BuildSea();

        var player = BuildPirate("Player", mShirt, mVest, mBlack, false);
        player.tag = "Player";
        var pcol = player.AddComponent<CapsuleCollider>(); pcol.center = new Vector3(0, 0.95f, 0); pcol.height = 1.9f; pcol.radius = 0.4f;
        player.AddComponent<Rigidbody>();
        player.AddComponent<RunnerPlayer>();
        cam.gameObject.AddComponent<CameraFollow>().target = player.transform;
        sea.AddComponent<FollowZ>().target = player.transform;

        // pirates chasing the hero
        var chaseGo = new GameObject("PirateChasers");
        var chasers = chaseGo.AddComponent<PirateChasers>();
        var c1 = BuildPirate("Pirate1", mShirt, mCoat, mBlack, true);
        var c2 = BuildPirate("Pirate2", mShirt, mNavy, mBlack, true);
        var c3 = BuildPirate("Pirate3", mShirt, mVest, mBlack, true);
        foreach (var c in new[] { c1, c2, c3 }) c.transform.localScale = Vector3.one * 1.05f;
        chasers.chasers = new[] { c1.transform, c2.transform, c3.transform };

        // pirate fleet sailing alongside + skull island ahead
        AddShip("PirateShip1", new Vector3(-26, 0, 60), 18f);
        AddShip("PirateShip2", new Vector3(30, 0, 110), -22f);
        AddShip("PirateShip3", new Vector3(-38, 0, 170), 12f);
        var rock = BuildSkullRock(); rock.transform.position = new Vector3(40, -2, 160);
        rock.AddComponent<FollowPlayer>().offset = new Vector3(40, 0, 160);

        var spGo = new GameObject("TrackSpawner");
        var sp = spGo.AddComponent<TrackSpawner>();
        sp.segmentPrefab = pSegment;
        sp.obstaclePrefabs = new[] { pBarrel, pSpikes, pCrate, pCannon, pStack };
        sp.islandPrefab = pIsland;

        // level themes: night / day / sunset / storm
        var th = new GameObject("LevelTheme").AddComponent<LevelTheme>();
        th.skyBase = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PirateSky.mat");
        th.moonMat = mMoon; th.starMat = mStarDot; th.cloudMat = mCloud; th.rainMat = mRain;
        th.seaRenderer = sea.GetComponent<Renderer>();
        th.sun = Object.FindFirstObjectByType<Light>();
        sp.coinPrefab = pCoin; sp.chestPrefab = pChest; sp.starPrefab = pStar;

        // ---------- HUD ----------
        var canvas = MakeCanvas();
        var hearts = new Image[3];
        for (int i = 0; i < 3; i++) hearts[i] = Dot(canvas, "Heart" + i, new Vector2(0, 1), new Vector2(30 + i * 70, -30), 56, Red);
        Dot(canvas, "CoinIcon", new Vector2(1, 1), new Vector2(-520, -40), 60, Gold);
        var coinText = Txt(canvas, "CoinText", "0", 46, new Vector2(1, 1), new Vector2(-345, -40), new Vector2(170, 70), TextAlignmentOptions.Left, White);
        Dot(canvas, "StarIcon", new Vector2(1, 1), new Vector2(-270, -40), 60, new Color(1f, 0.9f, 0.3f));
        var starText = Txt(canvas, "StarText", "0/5", 46, new Vector2(1, 1), new Vector2(-135, -40), new Vector2(130, 70), TextAlignmentOptions.Left, White);
        var distText = Txt(canvas, "DistText", "0 m", 46, new Vector2(0.5f, 1), new Vector2(0, -25), new Vector2(800, 70), TextAlignmentOptions.Center, White);
        var levelText = Txt(canvas, "LevelText", "Level 1", 34, new Vector2(0.5f, 1), new Vector2(0, -95), new Vector2(400, 50), TextAlignmentOptions.Center, Gold);
        var pauseBtn = Btn(canvas, "PauseButton", "II", new Vector2(1, 1), new Vector2(-30, -30), new Vector2(90, 90), Wood, 46);

        var pause = Overlay(canvas, "PausePanel", 0.65f);
        Txt(pause.transform, "Title", "PAUSED", 100, new Vector2(0.5f, 0.5f), new Vector2(0, 230), new Vector2(1000, 140), TextAlignmentOptions.Center, Gold);
        var bResume = Btn(pause.transform, "Resume", "RESUME", new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(480, 100), Wood);
        var bRestartP = Btn(pause.transform, "Restart", "RESTART", new Vector2(0.5f, 0.5f), new Vector2(0, -80), new Vector2(480, 100), Wood);
        var bMenuP = Btn(pause.transform, "Menu", "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(0, -210), new Vector2(480, 100), Wood);

        var over = Overlay(canvas, "GameOverPanel", 0.7f);
        Txt(over.transform, "Title", "GAME OVER", 110, new Vector2(0.5f, 0.5f), new Vector2(0, 250), new Vector2(1200, 150), TextAlignmentOptions.Center, Red);
        var overInfo = Txt(over.transform, "Info", "", 50, new Vector2(0.5f, 0.5f), new Vector2(0, 70), new Vector2(1500, 200), TextAlignmentOptions.Center, White);
        var bRestartO = Btn(over.transform, "Restart", "RESTART (R)", new Vector2(0.5f, 0.5f), new Vector2(-260, -150), new Vector2(440, 100), Wood);
        var bMenuO = Btn(over.transform, "Menu", "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(260, -150), new Vector2(440, 100), Wood);

        var done = Overlay(canvas, "LevelCompletePanel", 0.7f);
        Txt(done.transform, "Title", "LEVEL COMPLETE", 100, new Vector2(0.5f, 0.5f), new Vector2(0, 260), new Vector2(1400, 140), TextAlignmentOptions.Center, Gold);
        var doneInfo = Txt(done.transform, "Info", "", 50, new Vector2(0.5f, 0.5f), new Vector2(0, 60), new Vector2(1500, 260), TextAlignmentOptions.Center, White);
        var bNext = Btn(done.transform, "Next", "NEXT LEVEL", new Vector2(0.5f, 0.5f), new Vector2(-480, -190), new Vector2(420, 100), Wood);
        var bRestartD = Btn(done.transform, "Restart", "RESTART", new Vector2(0.5f, 0.5f), new Vector2(0, -190), new Vector2(420, 100), Wood);
        var bMenuD = Btn(done.transform, "Menu", "MAIN MENU", new Vector2(0.5f, 0.5f), new Vector2(480, -190), new Vector2(420, 100), Wood);

        var gmGo = new GameObject("RunnerGame");
        var gm = gmGo.AddComponent<RunnerGame>();
        gm.coinText = coinText; gm.starText = starText; gm.distText = distText; gm.levelText = levelText;
        gm.gameOverInfo = overInfo; gm.completeInfo = doneInfo; gm.hearts = hearts;
        gm.pausePanel = pause; gm.gameOverPanel = over; gm.completePanel = done;

        UnityEventTools.AddPersistentListener(pauseBtn.onClick, gm.TogglePause);
        UnityEventTools.AddPersistentListener(bResume.onClick, gm.Resume);
        UnityEventTools.AddPersistentListener(bRestartP.onClick, gm.Restart);
        UnityEventTools.AddPersistentListener(bMenuP.onClick, gm.BackToMenu);
        UnityEventTools.AddPersistentListener(bRestartO.onClick, gm.Restart);
        UnityEventTools.AddPersistentListener(bMenuO.onClick, gm.BackToMenu);
        UnityEventTools.AddPersistentListener(bNext.onClick, gm.NextLevel);
        UnityEventTools.AddPersistentListener(bRestartD.onClick, gm.Restart);
        UnityEventTools.AddPersistentListener(bMenuD.onClick, gm.BackToMenu);
        pause.SetActive(false); over.SetActive(false); done.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
    }

    // ================= Menu scene =================
    static void BuildMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var cam = Camera.main;
        cam.transform.position = new Vector3(0, 2.2f, -9);
        cam.transform.rotation = Quaternion.Euler(6, 12, 0);
        MakeMaterials();
        Atmosphere(cam);
        BuildSea();
        var ship = BuildShip("MenuShip", new Vector3(-9, -1.9f, 18), -28f);
        var sf = ship.AddComponent<FollowPlayer>(); sf.bobAmplitude = 0.25f; sf.rollDegrees = 3f;
        var isle = (GameObject)PrefabUtility.InstantiatePrefab(pIsland);
        isle.transform.position = new Vector3(8, -1f, 9); isle.transform.localScale = Vector3.one * 1.6f;
        var rockM = BuildSkullRock(); rockM.transform.position = new Vector3(38, -2, 95);

        var canvas = MakeCanvas();
        var mgGo = new GameObject("MenuUI");
        var menu = mgGo.AddComponent<MenuUI>();

        // main panel
        var main = Overlay(canvas, "MainPanel", 0f);
        Txt(main.transform, "Title1", "TREASURE HUNT", 120, new Vector2(0, 1), new Vector2(80, -50), new Vector2(1300, 140), TextAlignmentOptions.Left, Gold);
        Txt(main.transform, "Title2", "PIRATE COVE", 70, new Vector2(0, 1), new Vector2(80, -190), new Vector2(1000, 90), TextAlignmentOptions.Left, White);
        var left = new Vector2(0, 0.5f);
        var bPlay = Btn(main.transform, "Play", "PLAY", left, new Vector2(80, 180), new Vector2(520, 110), Gold, 56);
        var bLevels = Btn(main.transform, "Levels", "LEVELS", left, new Vector2(80, 50), new Vector2(520, 100), Wood);
        var bMissions = Btn(main.transform, "Missions", "MISSIONS", left, new Vector2(80, -70), new Vector2(520, 100), Wood);
        var bAch = Btn(main.transform, "Achievements", "ACHIEVEMENTS", left, new Vector2(80, -190), new Vector2(520, 100), Wood);
        var bSet = Btn(main.transform, "Settings", "SETTINGS", left, new Vector2(80, -310), new Vector2(520, 100), Wood);
        var bQuit = Btn(main.transform, "Quit", "QUIT", left, new Vector2(80, -430), new Vector2(520, 100), Wood);
        UnityEventTools.AddPersistentListener(bPlay.onClick, menu.Play);
        UnityEventTools.AddPersistentListener(bLevels.onClick, menu.ShowLevels);
        UnityEventTools.AddPersistentListener(bMissions.onClick, menu.ShowMissions);
        UnityEventTools.AddPersistentListener(bAch.onClick, menu.ShowAchievements);
        UnityEventTools.AddPersistentListener(bSet.onClick, menu.ShowSettings);
        UnityEventTools.AddPersistentListener(bQuit.onClick, menu.Quit);

        // levels
        Transform box; var levels = SubPanel(canvas, menu, "LevelsPanel", "SELECT LEVEL", out box);
        menu.levelButtons = new Button[4]; menu.levelLabels = new TMP_Text[4];
        for (int i = 0; i < 4; i++)
        {
            var b = Btn(box, "Level" + (i + 1), "Level " + (i + 1), new Vector2(0.5f, 0.5f), new Vector2(-450 + i * 300, 0), new Vector2(270, 330), Wood, 32);
            menu.levelButtons[i] = b; menu.levelLabels[i] = b.GetComponentInChildren<TMP_Text>();
            UnityEventTools.AddIntPersistentListener(b.onClick, menu.PlayLevel, i + 1);
        }

        // missions
        var missions = SubPanel(canvas, menu, "MissionsPanel", "MISSIONS", out box);
        menu.missionTexts = new TMP_Text[3]; menu.missionBars = new RectTransform[3];
        for (int i = 0; i < 3; i++)
        {
            float y = 170 - i * 150;
            menu.missionTexts[i] = Txt(box, "Mission" + i, "", 42, new Vector2(0.5f, 0.5f), new Vector2(0, y + 35), new Vector2(1000, 60), TextAlignmentOptions.Left, Dark);
            var bg = Box(box, "BarBg" + i, new Vector2(0.5f, 0.5f), new Vector2(0, y - 25), new Vector2(1000, 28), Dark, true);
            var fill = Stretch(bg.transform, "Fill", Green);
            var fr = (RectTransform)fill.transform; fr.anchorMax = new Vector2(0, 1);
            menu.missionBars[i] = fr;
        }

        // achievements
        var ach = SubPanel(canvas, menu, "AchievementsPanel", "ACHIEVEMENTS", out box);
        menu.achievementTexts = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
            menu.achievementTexts[i] = Txt(box, "Ach" + i, "", 42, new Vector2(0.5f, 0.5f), new Vector2(0, 140 - i * 140), new Vector2(1200, 70), TextAlignmentOptions.Left, Dark);

        // settings
        var set = SubPanel(canvas, menu, "SettingsPanel", "SETTINGS", out box);
        Txt(box, "MusicLabel", "Music", 46, new Vector2(0.5f, 0.5f), new Vector2(-450, 150), new Vector2(300, 60), TextAlignmentOptions.Left, Dark);
        Txt(box, "SfxLabel", "SFX", 46, new Vector2(0.5f, 0.5f), new Vector2(-450, 30), new Vector2(300, 60), TextAlignmentOptions.Left, Dark);
        menu.musicSlider = MakeSlider(box, new Vector2(120, 150));
        menu.sfxSlider = MakeSlider(box, new Vector2(120, 30));
        var bQ = Btn(box, "Quality", "Quality: High", new Vector2(0.5f, 0.5f), new Vector2(0, -100), new Vector2(700, 90), Wood, 40);
        menu.qualityText = bQ.GetComponentInChildren<TMP_Text>();
        UnityEventTools.AddPersistentListener(bQ.onClick, menu.CycleQuality);
        Txt(box, "Controls", "Controls: A / D or Left / Right arrows switch lanes,\nSpace or Up arrow jumps, Esc pauses.", 34, new Vector2(0.5f, 0.5f), new Vector2(0, -230), new Vector2(1150, 120), TextAlignmentOptions.Center, Dark);

        menu.mainPanel = main; menu.levelsPanel = levels; menu.missionsPanel = missions; menu.achievementsPanel = ach; menu.settingsPanel = set;
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    static GameObject SubPanel(Transform canvas, MenuUI menu, string name, string title, out Transform box)
    {
        var p = Overlay(canvas, name, 0.6f);
        var b = Box(p.transform, "Box", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1300, 800), Parchment, true);
        box = b.transform;
        Txt(box, "Title", title, 72, new Vector2(0.5f, 0.5f), new Vector2(0, 320), new Vector2(1100, 100), TextAlignmentOptions.Center, Dark);
        var back = Btn(box, "Back", "BACK", new Vector2(0.5f, 0.5f), new Vector2(0, -330), new Vector2(360, 90), Wood);
        UnityEventTools.AddPersistentListener(back.onClick, menu.ShowMain);
        return p;
    }

    static Slider MakeSlider(Transform parent, Vector2 pos)
    {
        var res = new DefaultControls.Resources();
        res.standard = UI("UISprite.psd"); res.background = UI("Background.psd"); res.knob = UI("Knob.psd");
        var go = DefaultControls.CreateSlider(res);
        go.transform.SetParent(parent, false);
        var r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos; r.sizeDelta = new Vector2(700, 40);
        return go.GetComponent<Slider>();
    }

    // ================= UI helpers =================
    static Transform MakeCanvas()
    {
        var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        cgo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = cgo.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080); cs.matchWidthOrHeight = 0.5f;
        var es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
        return cgo.transform;
    }

    static Sprite UI(string n) { return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/" + n); }

    static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = r.pivot = anchor; r.anchoredPosition = pos; r.sizeDelta = size;
        return r;
    }

    static Image Box(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, Color c, bool round)
    {
        var img = Rect(parent, name, anchor, pos, size).gameObject.AddComponent<Image>();
        img.color = c;
        if (round) { img.sprite = UI("UISprite.psd"); img.type = Image.Type.Sliced; }
        return img;
    }

    static Image Dot(Transform parent, string name, Vector2 anchor, Vector2 pos, float size, Color c)
    {
        var img = Box(parent, name, anchor, pos, new Vector2(size, size), c, false);
        img.sprite = UI("Knob.psd");
        return img;
    }

    static GameObject Stretch(Transform parent, string name, Color c)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var r = (RectTransform)go.transform;
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = c;
        return go;
    }

    static GameObject Overlay(Transform canvas, string name, float alpha) { return Stretch(canvas, name, new Color(0, 0, 0, alpha)); }

    static TextMeshProUGUI Txt(Transform parent, string name, string text, int size, Vector2 anchor, Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align, Color color)
    {
        var r = Rect(parent, name, anchor, pos, sizeDelta);
        var t = r.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.alignment = align; t.color = color; t.raycastTarget = false;
        return t;
    }

    static Button Btn(Transform parent, string name, string label, Vector2 anchor, Vector2 pos, Vector2 size, Color c, int fontSize = 44)
    {
        var img = Box(parent, name, anchor, pos, size, c, true);
        var b = img.gameObject.AddComponent<Button>(); b.targetGraphic = img;
        var t = Txt(img.transform, "Label", label, fontSize, new Vector2(0.5f, 0.5f), Vector2.zero, size, TextAlignmentOptions.Center, c == Gold ? Dark : White);
        return b;
    }

    // ================= asset helpers =================
    static void EnsureFolder(string n) { if (!AssetDatabase.IsValidFolder("Assets/" + n)) AssetDatabase.CreateFolder("Assets", n); }

    static void EnsureTag(string tag)
    {
        var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++) if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedProperties();
    }

    static Material Mat(string name, string hex)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader sh = GraphicsSettings.currentRenderPipeline != null ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
            if (sh == null) sh = Shader.Find("Standard");
            m = new Material(sh);
            AssetDatabase.CreateAsset(m, path);
        }
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        m.color = c; EditorUtility.SetDirty(m);
        return m;
    }

    static GameObject Prim(PrimitiveType t, string name, Vector3 pos, Vector3 rot, Vector3 scale, Material m)
    {
        var g = GameObject.CreatePrimitive(t);
        g.name = name; g.transform.position = pos; g.transform.eulerAngles = rot; g.transform.localScale = scale;
        g.GetComponent<Renderer>().sharedMaterial = m;
        return g;
    }

    static void Add(GameObject parent, GameObject child) { child.transform.SetParent(parent.transform, true); }

    static void StripColliders(GameObject root)
    {
        foreach (var c in root.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);
    }

    static GameObject SavePrefab(GameObject g, string name)
    {
        var p = PrefabUtility.SaveAsPrefabAsset(g, "Assets/Prefabs/" + name + ".prefab");
        Object.DestroyImmediate(g);
        return p;
    }
}
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MRU.InfiniteRunner
{
    public sealed class RuntimeUI
    {
        private readonly GameManager game;
        private readonly Canvas canvas;
        private readonly GameObject menu, hud, pause, results, shop, settings, worlds, missions;
        private readonly Text hudScore, hudCoins, hudWorld, hudPower, resultScore, resultHigh, resultCoins;
        private readonly Text skinInfo, worldInfo, settingsInfo, missionInfo, toast;
        private int shopSkin;

        // UI Palette matching the exact reference image design
        private static readonly Color DeepNavyBg  = new Color(0.04f, 0.08f, 0.18f, 0.96f);
        private static readonly Color RoyalBlue   = new Color(0.08f, 0.48f, 0.92f);
        private static readonly Color GoldenYellow= new Color(1.00f, 0.72f, 0.05f);
        private static readonly Color EmeraldGreen= new Color(0.15f, 0.78f, 0.25f);
        private static readonly Color CrimsonRed  = new Color(0.88f, 0.12f, 0.15f);
        private static readonly Color CyanAccent  = new Color(0.05f, 0.92f, 0.98f);
        private static readonly Color CardBgColor = new Color(0.08f, 0.14f, 0.28f, 0.92f);

        public RuntimeUI(GameManager owner)
        {
            game = owner;

            // Root Canvas
            GameObject canvasObject = new GameObject("MRU Cyber Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventObject = new GameObject("Event System", typeof(EventSystem), typeof(StandaloneInputModule));
                Object.DontDestroyOnLoad(eventObject);
            }

            // -------------------------------------------------------------
            // 1. MAIN MENU PANEL (Matching Screen 1 in reference image)
            // -------------------------------------------------------------
            menu = Panel("Main Menu Panel");
            AddText(menu.transform, "👑", 70, new Vector2(0f, 740f), GoldenYellow);
            AddText(menu.transform, "MRU", 95, new Vector2(0f, 660f), GoldenYellow);
            AddText(menu.transform, "INFINITE RUNNER", 42, new Vector2(0f, 575f), Color.white);
            AddText(menu.transform, "RUN  •  JUMP  •  COLLECT FOREVER!", 26, new Vector2(0f, 510f), CyanAccent);

            // Large Golden Yellow PLAY Button
            AddButton(menu.transform, "PLAY", 330f, 130f, GoldenYellow, game.StartRun, 640f);

            // Sleek Blue Action Buttons
            AddButton(menu.transform, "Select World", 175f, 110f, RoyalBlue, ShowWorlds, 640f);
            AddButton(menu.transform, "Shop", 45f, 110f, RoyalBlue, ShowShop, 640f);
            AddButton(menu.transform, "Missions & Rewards", -85f, 110f, EmeraldGreen, ShowMissions, 640f);
            AddButton(menu.transform, "Settings", -215f, 110f, RoyalBlue, ShowSettings, 640f);
            AddButton(menu.transform, "Privacy Policy", -345f, 110f, RoyalBlue,
                () => ShowMessage("Privacy Policy: www.mruinfiniterunner.com/privacy-policy.html"), 640f);

            AddText(menu.transform, "Kids & Family Safe  •  High Score: " + game.HighScore, 26, new Vector2(0f, -720f), new Color(0.75f, 0.82f, 0.92f));

            // -------------------------------------------------------------
            // 2. HUD PANEL (Matching Screen 2 in reference image)
            // -------------------------------------------------------------
            hud = Panel("HUD Panel");
            hudScore = AddText(hud.transform, "SCORE  0     BEST  0", 38, new Vector2(0f, 820f), Color.white);
            hudCoins = AddText(hud.transform, "🪙 0", 38, new Vector2(-360f, 820f), GoldenYellow);
            hudWorld = AddText(hud.transform, "ANCIENT TEMPLE", 28, new Vector2(0f, 740f), CyanAccent);
            hudPower = AddText(hud.transform, "", 28, new Vector2(0f, -720f), CyanAccent);

            // Pause Button top right
            AddButton(hud.transform, "||", 820f, 100f, RoyalBlue, game.TogglePause, 110f, 420f);

            // -------------------------------------------------------------
            // 3. PAUSE PANEL
            // -------------------------------------------------------------
            pause = Panel("Pause Panel");
            AddText(pause.transform, "PAUSED", 72, new Vector2(0f, 420f), Color.white);
            AddButton(pause.transform, "RESUME", 190f, 120f, GoldenYellow, game.TogglePause);
            AddButton(pause.transform, "RESTART RUN", 40f, 120f, EmeraldGreen, game.StartRun);
            AddButton(pause.transform, "HOME", -110f, 120f, RoyalBlue, game.ReturnHome);

            // -------------------------------------------------------------
            // 4. GAME OVER PANEL (Matching Screen 4 in reference image)
            // -------------------------------------------------------------
            results = Panel("Results Panel");

            // Top Crimson Red Ribbon Header
            GameObject redBanner = new GameObject("GameOverBanner", typeof(RectTransform), typeof(Image));
            redBanner.transform.SetParent(results.transform, false);
            RectTransform bannerRect = redBanner.GetComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.5f, 0.5f); bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
            bannerRect.sizeDelta = new Vector2(720f, 130f); bannerRect.anchoredPosition = new Vector2(0f, 540f);
            redBanner.GetComponent<Image>().color = CrimsonRed;
            AddText(redBanner.transform, "GAME OVER", 58, Vector2.zero, Color.white);

            // Score Display Card
            AddText(results.transform, "Your Score", 32, new Vector2(0f, 380f), new Color(0.85f, 0.90f, 0.98f));
            resultScore = AddText(results.transform, "2450", 68, new Vector2(0f, 305f), GoldenYellow);

            AddText(results.transform, "High Score", 28, new Vector2(0f, 215f), new Color(0.75f, 0.82f, 0.92f));
            resultHigh = AddText(results.transform, "6890", 48, new Vector2(0f, 155f), GoldenYellow);

            resultCoins = AddText(results.transform, "COINS EARNED  +0", 30, new Vector2(0f, 75f), CyanAccent);

            // Action Buttons matching reference image
            AddButton(results.transform, "Restart", -60f, 115f, EmeraldGreen, game.StartRun, 640f);
            AddButton(results.transform, "Home", -200f, 115f, RoyalBlue, game.ReturnHome, 640f);
            AddButton(results.transform, "Share Score", -340f, 115f, RoyalBlue,
                () => ShowMessage("Share your High Score with friends!"), 640f);

            // -------------------------------------------------------------
            // 5. SELECT WORLD PANEL (Matching Screen 3 in reference image)
            // -------------------------------------------------------------
            worlds = Panel("World Select Panel");

            // Header Banner
            GameObject worldBanner = new GameObject("WorldBanner", typeof(RectTransform), typeof(Image));
            worldBanner.transform.SetParent(worlds.transform, false);
            RectTransform wBannerRect = worldBanner.GetComponent<RectTransform>();
            wBannerRect.anchorMin = new Vector2(0.5f, 0.5f); wBannerRect.anchorMax = new Vector2(0.5f, 0.5f);
            wBannerRect.sizeDelta = new Vector2(720f, 110f); wBannerRect.anchoredPosition = new Vector2(0f, 620f);
            worldBanner.GetComponent<Image>().color = GoldenYellow;
            AddText(worldBanner.transform, "Select World", 52, Vector2.zero, Color.black);

            worldInfo = AddText(worlds.transform, "", 34, new Vector2(0f, 320f), Color.white);

            AddButton(worlds.transform, "◄ PREVIOUS", 110f, 115f, RoyalBlue, () => ChangeWorld(-1), 320f, -180f);
            AddButton(worlds.transform, "NEXT ►", 110f, 115f, RoyalBlue, () => ChangeWorld(1), 320f, 180f);

            AddButton(worlds.transform, "START RUN IN THIS WORLD", -60f, 125f, EmeraldGreen, game.StartRun, 680f);
            AddButton(worlds.transform, "Back", -220f, 115f, GoldenYellow, ShowMenu, 680f);

            // -------------------------------------------------------------
            // 6. CHARACTER SHOP PANEL
            // -------------------------------------------------------------
            shop = Panel("Shop Panel");
            AddText(shop.transform, "CHARACTER SHOP", 62, new Vector2(0f, 560f), GoldenYellow);
            skinInfo = AddText(shop.transform, "", 32, new Vector2(0f, 360f), Color.white);
            AddButton(shop.transform, "◄ PREVIOUS", 230f, 115f, RoyalBlue, () => ChangeSkin(-1), 320f, -180f);
            AddButton(shop.transform, "NEXT ►", 230f, 115f, RoyalBlue, () => ChangeSkin(1), 320f, 180f);
            AddButton(shop.transform, "EQUIP / UNLOCK", 85f, 115f, GoldenYellow, () => game.BuyOrSelectSkin(shopSkin), 680f);
            AddButton(shop.transform, "Back", -260f, 115f, RoyalBlue, ShowMenu, 680f);

            // -------------------------------------------------------------
            // 7. SETTINGS PANEL
            // -------------------------------------------------------------
            settings = Panel("Settings Panel");
            AddText(settings.transform, "SETTINGS", 64, new Vector2(0f, 480f), GoldenYellow);
            settingsInfo = AddText(settings.transform, "", 32, new Vector2(0f, 310f), Color.white);
            AddButton(settings.transform, "TOGGLE SOUND FX", 120f, 115f, RoyalBlue, () => game.SetSound(!game.Save.soundEnabled));
            AddButton(settings.transform, "TOGGLE VIBRATION", -20f, 115f, RoyalBlue, () => game.SetVibration(!game.Save.vibrationEnabled));
            AddButton(settings.transform, "PRIVACY POLICY", -160f, 115f, RoyalBlue,
                () => ShowMessage("Privacy Policy: www.mruinfiniterunner.com/privacy-policy.html"));
            AddButton(settings.transform, "Back", -310f, 115f, RoyalBlue, ShowMenu);

            // -------------------------------------------------------------
            // 8. MISSIONS PANEL
            // -------------------------------------------------------------
            missions = Panel("Missions Panel");
            AddText(missions.transform, "MISSIONS & REWARDS", 60, new Vector2(0f, 500f), GoldenYellow);
            missionInfo = AddText(missions.transform, "", 32, new Vector2(0f, 270f), Color.white);
            AddButton(missions.transform, "CLAIM DAILY REWARD  (+150 COINS)", 35f, 115f, GoldenYellow, game.ClaimDailyReward, 700f);
            AddButton(missions.transform, "CLAIM MISSION REWARD  (+250 COINS)", -105f, 115f, EmeraldGreen, game.ClaimMissionReward, 700f);
            AddButton(missions.transform, "Back", -245f, 115f, RoyalBlue, ShowMenu, 700f);

            // -------------------------------------------------------------
            // 9. TOAST NOTIFICATION
            // -------------------------------------------------------------
            toast = AddText(canvas.transform, "", 30, new Vector2(0f, -860f), GoldenYellow);

            ShowOnly(menu);
            ApplySkin(game.Save.selectedSkin);
            Refresh();
        }

        public void ShowMenu() { ShowOnly(menu); Refresh(); }
        public void ShowHud() { ShowOnly(hud); Refresh(); }
        public void ShowPause() { ShowOnly(pause); }

        public void ShowGameOver()
        {
            resultScore.text = game.Score.ToString();
            resultHigh.text = game.HighScore.ToString();
            resultCoins.text = "COINS EARNED  +" + game.RunCoins;
            ShowOnly(results);
        }

        public void ShowShop() { UpdateSkin(); ShowOnly(shop); }
        public void ShowSettings() { UpdateSettings(); ShowOnly(settings); }
        public void ShowWorlds() { UpdateWorld(); ShowOnly(worlds); }
        public void ShowMissions() { UpdateMission(); ShowOnly(missions); }

        public void ShowMessage(string message)
        {
            if (toast != null) toast.text = message;
        }

        public void ApplySkin(int index)
        {
            game.Runner.ApplySkin(index);
            UpdateSkin();
        }

        public void Refresh()
        {
            if (hudScore == null) return;
            hudScore.text = "SCORE  " + game.Score + "     BEST  " + game.HighScore;
            hudCoins.text = "🪙 " + game.Save.coins;
            hudWorld.text = game.CurrentWorldName.ToUpperInvariant() + "  (" + game.DistanceMeters + "m)";
            hudPower.text = game.ActivePowerText;

            if (shop.activeSelf) UpdateSkin();
            if (settings.activeSelf) UpdateSettings();
            if (worlds.activeSelf) UpdateWorld();
            if (missions.activeSelf) UpdateMission();
        }

        private void UpdateSkin()
        {
            if (skinInfo == null) return;
            string[] names = { "MRU CLASSIC (RED HOODIE)", "BLUE COMET", "JUNGLE SCOUT", "NIGHT SPARK", "DESERT DASH", "NEON DIVER" };
            int[] cost = { 0, 100, 250, 500, 900, 1500 };
            bool owned = (game.Save.ownedSkins & (1 << shopSkin)) != 0;

            skinInfo.text = names[shopSkin] + "\n\nSTATUS: " + (owned ? "UNLOCKED" : cost[shopSkin] + " COINS") +
                "\nYOUR COINS: " + game.Save.coins + "\nOUTFIT " + (shopSkin + 1) + " / " + names.Length;
        }

        private void ChangeSkin(int amount)
        {
            shopSkin = (shopSkin + amount + 6) % 6;
            UpdateSkin();
        }

        private void UpdateSettings()
        {
            settingsInfo.text = "Sound Effects: " + (game.Save.soundEnabled ? "ENABLED" : "MUTED") +
                "\nVibration Haptics: " + (game.Save.vibrationEnabled ? "ENABLED" : "DISABLED") +
                "\n\nOffline Mode Ready. Web & AdMob Enabled.";
        }

        private void UpdateWorld()
        {
            string[] names =
            {
                "Ancient Temple", "Jungle Adventure", "Sky Kingdom", "Desert Adventure", "Snow Mountain",
                "Volcano", "Futuristic Neon City", "Alien Planet", "Underwater Kingdom", "Space Station"
            };
            worldInfo.text = "WORLD " + (game.SelectedWorld + 1) + " / 10\n\n" + names[game.SelectedWorld] +
                "\n\n" + game.CurrentWorldDesc + "\n(Worlds transition dynamically every 300 meters!)";
        }

        private void ChangeWorld(int amount)
        {
            game.SelectWorld(game.SelectedWorld + amount);
            UpdateWorld();
        }

        private void UpdateMission()
        {
            int progress = Mathf.Min(game.Save.missionCoins, 50);
            missionInfo.text = "Coin Collector Mission\nProgress: " + progress + " / 50 Coins\n\nTotal Coins: " + game.Save.coins +
                "\nMission Reward: " + (game.Save.missionRewardClaimed ? "CLAIMED" : "250 COINS") +
                "\nBest Distance: " + game.Save.bestDistance + " m | Total Runs: " + game.Save.totalRuns;
        }

        private GameObject Panel(string name)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);

            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;

            Image image = panel.GetComponent<Image>();
            image.color = DeepNavyBg;
            return panel;
        }

        private static Text AddText(Transform parent, string value, int size, Vector2 position, Color color)
        {
            GameObject obj = new GameObject("Label", typeof(RectTransform), typeof(Text));
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f); rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(1000f, 150f); rect.anchoredPosition = position;

            Text text = obj.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void AddButton(Transform parent, string label, float y, float height, Color color,
            UnityAction action, float width = 640f, float x = 0f)
        {
            GameObject buttonObject = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f); rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height); rect.anchoredPosition = new Vector2(x, y);

            Image image = buttonObject.GetComponent<Image>();
            image.color = color;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            Text text = AddText(buttonObject.transform, label, 30, Vector2.zero, color == GoldenYellow ? Color.black : Color.white);
            text.rectTransform.sizeDelta = new Vector2(width - 30f, height - 8f);
        }

        private static void ShowOnly(GameObject visible)
        {
            visible.SetActive(true);
            Transform root = visible.transform.parent;
            for (int i = 0; i < root.childCount; i++)
            {
                GameObject child = root.GetChild(i).gameObject;
                if (child != visible && child.GetComponent<Image>() != null && child.GetComponent<RectTransform>().anchorMin == Vector2.zero)
                    child.SetActive(false);
            }
        }
    }
}

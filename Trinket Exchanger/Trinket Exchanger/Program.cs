using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Trinket_Exchanger
{
    class Program
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Menu _menu;
        public static bool hasYellow = false;
        public static bool hasRed = false;
        public static bool hasBlue = false;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            _menu = new Menu("Trinket Exchanger", "TrinketExchanger", true);
            var genMenu = new Menu("General", "TE.G");
            genMenu.AddItem(new MenuItem("yellow", "Buy Yellow at start").SetValue(true));
            genMenu.AddItem(new MenuItem("red", "Buy Red").SetValue(true));
            genMenu.AddItem(new MenuItem("redtimer", "Buy Red at min:").SetValue(new Slider(15, 1, 60)));
            genMenu.AddItem(new MenuItem("blue", "Buy Blue").SetValue(false));
            genMenu.AddItem(new MenuItem("bluetimer", "Buy Blue at min:").SetValue(new Slider(30, 1, 60)));
            _menu.AddSubMenu(genMenu);

            var miscMenu = new Menu("Misc", "TE.Misc");
            miscMenu.AddItem(new MenuItem("redSightstone", "Buy Red on Sightstone").SetValue(true));
            miscMenu.AddItem(new MenuItem("adc", "Buy Blue when IE + Zeal is finished").SetValue(true));
            miscMenu.AddItem(new MenuItem("rlevel", "Buy Red at Level").SetValue(true));
            miscMenu.AddItem(new MenuItem("rleveltimer", "Buy Red at Level:").SetValue(new Slider(9, 1, 18)));
            miscMenu.AddItem(new MenuItem("blevel", "Buy Blue at Level").SetValue(false));
            miscMenu.AddItem(new MenuItem("bleveltimer", "Buy Blue at Level:").SetValue(new Slider(9, 1, 18)));
            _menu.AddSubMenu(miscMenu);

            var upgradeMenu = new Menu("Upgrade", "TE.Upgrade");
            upgradeMenu.AddItem(new MenuItem("uylevel", "Upgrade Yellow at Level").SetValue(false));
            upgradeMenu.AddItem(new MenuItem("uylevel1", "To Greater Stealth Totem").SetValue(false));
            upgradeMenu.AddItem(new MenuItem("uylevel2", "To Greater Vision Totem").SetValue(false));
            upgradeMenu.AddItem(new MenuItem("uyleveltimer", "Upgrade Red at Level:").SetValue(new Slider(9, 9, 18)));
            upgradeMenu.AddItem(new MenuItem("urlevel", "Upgrade Red at Level").SetValue(false));
            upgradeMenu.AddItem(new MenuItem("urleveltimer", "Upgrade Red at Level:").SetValue(new Slider(9, 9, 18)));
            upgradeMenu.AddItem(new MenuItem("ublevel", "Upgrade Blue at Level").SetValue(false));
            upgradeMenu.AddItem(new MenuItem("ubleveltimer", "Upgrade Blue at Level:").SetValue(new Slider(9, 9, 18)));
            _menu.AddSubMenu(upgradeMenu);
           
            _menu.AddToMainMenu();
            Game.PrintChat("<font color=\"#FF9900\"><b>Trinket Exchanger</b></font> - Loaded");
            Game.OnGameUpdate += OnTick;
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.IsDead || Player.InShop())
            {
                //Yellow
                if (_menu.Item("yellow").GetValue<bool>() && !GetTrinket())
                {
                    Player.BuyItem(ItemId.Warding_Totem_Trinket);
                    hasYellow = true;
                }

                if (!hasRed && !hasBlue && (_menu.Item("red").GetValue<bool>() &&
                    (GetTimer() >= _menu.Item("redtimer").GetValue<Slider>().Value) ||
                    (_menu.Item("rlevel").GetValue<bool>()) &&
                     (Player.Level >= _menu.Item("rleveltimer").GetValue<Slider>().Value)))
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                    hasRed = true;
                }
                if (!hasBlue && (_menu.Item("blue").GetValue<bool>() &&
                    (GetTimer() >= _menu.Item("bluetimer").GetValue<Slider>().Value) || 
                    (_menu.Item("blevel").GetValue<bool>()) && (Player.Level >= _menu.Item("bleveltimer").GetValue<Slider>().Value)))
                {
                    Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                    hasBlue = true;
                }
                if (_menu.Item("redSightstone").GetValue<bool>() && !hasRed && !hasBlue &&
                    (Player.InventoryItems.Find(s => s.Id == ItemId.Sightstone).IsValidSlot() ||
                    Player.InventoryItems.Find(s => s.Id == ItemId.Ruby_Sightstone).IsValidSlot()))
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                    hasRed = true;
                }
                if (_menu.Item("adc").GetValue<bool>() && !hasBlue &&
                    Player.InventoryItems.Find(s => s.Id == ItemId.Infinity_Edge).IsValidSlot() && 
                    Player.InventoryItems.Find(s => s.Id == ItemId.Zeal).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                    hasBlue = true;
                }
                if (_menu.Item("uylevel").GetValue<bool>() && !hasBlue && !hasRed && hasYellow &&
                    Player.Level >= _menu.Item("uyleveltimer").GetValue<Slider>().Value)
                {
                    if (_menu.Item("uylevel1").GetValue<bool>())
                    {
                        Player.BuyItem(ItemId.Greater_Stealth_Totem_Trinket);
                    }
                    else if (_menu.Item("uylevel2").GetValue<bool>())
                    {
                        Player.BuyItem(ItemId.Greater_Vision_Totem_Trinket);
                    }
                    hasYellow = true;
                }
                if (_menu.Item("urlevel").GetValue<bool>() && !hasYellow && !hasBlue && hasRed &&
                    Player.Level >= _menu.Item("urleveltimer").GetValue<Slider>().Value)
                {
                    Player.BuyItem(ItemId.Oracles_Lens_Trinket);
                    hasRed = true;
                }
                if (_menu.Item("ublevel").GetValue<bool>() && !hasYellow && !hasRed && hasBlue &&
                    Player.Level >= _menu.Item("ubleveltimer").GetValue<Slider>().Value)
                {
                    Player.BuyItem(ItemId.Farsight_Orb_Trinket);
                    hasBlue = true;
                }

            }
        }

        public static bool GetTrinket()
        {
            //Yellow
            if ((Player.InventoryItems.Find(s => s.Id == ItemId.Warding_Totem_Trinket).IsValidSlot()) ||
                (Player.InventoryItems.Find(s => s.Id == ItemId.Greater_Stealth_Totem_Trinket).IsValidSlot()) ||
                (Player.InventoryItems.Find(s => s.Id == ItemId.Greater_Vision_Totem_Trinket).IsValidSlot()))
            {
                hasYellow = true;
            }
            else hasYellow = false;
            //Red
            if ((Player.InventoryItems.Find(s => s.Id == ItemId.Sweeping_Lens_Trinket).IsValidSlot()) ||
                (Player.InventoryItems.Find(s => s.Id == ItemId.Oracles_Lens_Trinket).IsValidSlot()))
            {
                hasRed = true;
            }
            else hasRed = false;
            //Blue
            if ((Player.InventoryItems.Find(s => s.Id == ItemId.Scrying_Orb_Trinket).IsValidSlot()) ||
                (Player.InventoryItems.Find(s => s.Id == ItemId.Farsight_Orb_Trinket).IsValidSlot()))
            {
                hasBlue = true;
            }
            else hasBlue = false;
            if (!hasYellow && !hasRed && !hasBlue) return false;
            return true;
        }

        public static float GetTimer()
        {
            return Game.Time / 60;
        }
    }
}

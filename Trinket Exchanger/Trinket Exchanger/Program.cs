﻿using System;
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

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            _menu = new Menu("Trinket Exchanger", "TrinketExchanger", true);
            _menu.AddItem(new MenuItem("yellow", "Buy Yellow at start").SetValue(true));
            _menu.AddItem(new MenuItem("red", "Buy Red").SetValue(true));
            _menu.AddItem(new MenuItem("redtimer", "Buy Red at min:").SetValue(new Slider(15, 1, 60)));
            _menu.AddItem(new MenuItem("blue", "Buy Blue").SetValue(true));
            _menu.AddItem(new MenuItem("bluetimer", "Buy Blue at min:").SetValue(new Slider(30, 1, 60)));
            _menu.AddItem(new MenuItem("redSightstone", "Buy Red on Sightstone").SetValue(true));
            _menu.AddItem(new MenuItem("adc", "Buy Blue when IE + Zeal is finished").SetValue(true));
            _menu.AddToMainMenu();
            Game.PrintChat("Trinket Exchanger loaded.");
            Game.OnGameUpdate += OnTick;
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.IsDead || Player.InShop())
            {
                if (_menu.Item("yellow").GetValue<bool>() &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Warding_Totem_Trinket).IsValidSlot() &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Scrying_Orb_Trinket).IsValidSlot() && 
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Sweeping_Lens_Trinket).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Warding_Totem_Trinket);
                }
                if (GetTimer() <= 1 && _menu.Item("yellow").GetValue<bool>() &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Warding_Totem_Trinket).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Warding_Totem_Trinket);
                }
                if (_menu.Item("red").GetValue<bool>() &&
                    (GetTimer() >= _menu.Item("redtimer").GetValue<Slider>().Value) &&
                    (GetTimer() < _menu.Item("bluetimer").GetValue<Slider>().Value) &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Sweeping_Lens_Trinket).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
                if (_menu.Item("blue").GetValue<bool>() &&
                    (GetTimer() >= _menu.Item("bluetimer").GetValue<Slider>().Value) &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Scrying_Orb_Trinket).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                }
                if (_menu.Item("red").GetValue<bool>() && _menu.Item("redSightstone").GetValue<bool>() &&
                    Player.InventoryItems.Find(s => s.Id == ItemId.Sightstone).IsValidSlot() ||
                    Player.InventoryItems.Find(s => s.Id == ItemId.Ruby_Sightstone).IsValidSlot() &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Sweeping_Lens_Trinket).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
                if (_menu.Item("blue").GetValue<bool>() && _menu.Item("adc").GetValue<bool>() &&
                    Player.InventoryItems.Find(s => s.Id == ItemId.Infinity_Edge).IsValidSlot() && 
                    Player.InventoryItems.Find(s => s.Id == ItemId.Zeal).IsValidSlot() &&
                    !Player.InventoryItems.Find(s => s.Id == ItemId.Scrying_Orb_Trinket).IsValidSlot())
                {
                    _menu.Item("red").SetValue(false);
                    Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                }
            }
        }

        public static float GetTimer()
        {
            return Game.Time / 60;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

namespace Trinket_Exchanger
{
    class Program
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Menu menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            menu = new Menu("Trinket Exchanger", "TrinketExchanger", true);
            menu.AddItem(new MenuItem("yellow", "Buy Yellow at start").SetValue(true));
            menu.AddItem(new MenuItem("red", "Buy Red").SetValue(true));
            menu.AddItem(new MenuItem("redtimer", "Buy Red at min:").SetValue(new Slider(15, 1, 60)));
            menu.AddItem(new MenuItem("blue", "Buy Blue").SetValue(true));
            menu.AddItem(new MenuItem("bluetimer", "Buy Blue at min:").SetValue(new Slider(30, 1, 60)));
            menu.AddItem(new MenuItem("redSightstone", "Buy Red on Sightstone").SetValue(true));
            menu.AddItem(new MenuItem("redWriggle", "Buy Red on Wriggle").SetValue(true));
            menu.AddToMainMenu();
            Game.PrintChat("Trinket Exchanger loaded.");
            Game.OnGameUpdate += OnTick;

        }

        private static void OnTick(EventArgs args)
        {

            
            if (Player.IsDead || Player.InShop())
            {
                if (GetTimer() < 1 && menu.Item("yellow").GetValue<bool>())
                {
                    Player.BuyItem(ItemId.Warding_Totem_Trinket);
                }
                if (menu.Item("red").GetValue<bool>() && (GetTimer() >= menu.Item("redtimer").GetValue<Slider>().Value) &&
                    (GetTimer() < menu.Item("bluetimer").GetValue<Slider>().Value))
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
                if (menu.Item("blue").GetValue<bool>() && (GetTimer() >= menu.Item("bluetimer").GetValue<Slider>().Value))
                {
                    Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                }
                if (menu.Item("red").GetValue<bool>() && menu.Item("redSightstone").GetValue<bool>() && Player.InventoryItems.Find(s => s.Id == ItemId.Sightstone).IsValidSlot() || Player.InventoryItems.Find(s => s.Id == ItemId.Ruby_Sightstone).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
                if (menu.Item("red").GetValue<bool>() && menu.Item("redWriggle").GetValue<bool>() && Player.InventoryItems.Find(s => s.Id == ItemId.Wriggles_Lantern).IsValidSlot())
                {
                    Player.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
            }
        }

        public static float GetTimer()
        {
            return Game.Time / 60;
        }
    }
}

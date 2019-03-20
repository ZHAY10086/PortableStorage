﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public static int BagID;
		public static int timer;

		public static Texture2D QE_Glow { get; set; }

		public static Texture2D QE_Gems { get; set; }

		//public GUI<PanelUI> PanelUI;

		public static ModHotKey HotkeyBag;

		public static Dictionary<string, List<int>> ammoTypes;
		public static Dictionary<string, int> tooltipIndexes;

		public override void Load()
		{
			Instance = this;

			TagSerializer.AddSerializer(new FrequencySerializer());

			Hooking.Hooking.Initialize();

			//HotkeyBag = this.Register("Open Bag", Keys.B);

			if (!Main.dedServ)
			{
				//this.LoadTextures();

				//PanelUI = Utility.SetupGUI<PanelUI>();
				//PanelUI.Visible = true;
			}
		}

		public override void Unload()
		{
			BaseLibrary.Utility.UnloadNullableTypes();
		}

		public override object Call(params object[] args)
		{
			if (args.Length < 1 || !(args[0] is string command)) return base.Call(args);

			switch (command)
			{
				case "RegisterIngredient" when args.Length == 2 && args[1] is short ID && !Utility.AlchemistBagWhitelist.Contains(ID):
				{
					Utility.AlchemistBagWhitelist.Add(ID);
					Logger.Info($"Ingredient '{ID}' added to Alchemist's Bag whitelist!");
					break;
				}
			}

			return base.Call(args);
		}

		public override void PostSetupContent()
		{
			Utility.Load();
		}

		// bag for throwing items

		public override void PostAddRecipes()
		{
			foreach (ModItem item in BaseLibrary.Utility.GetValue<Dictionary<string, ModItem>>(this, "items").Values)
			{
				Recipe recipe = Main.recipe.FirstOrDefault(x => x.createItem.type == item.item.type);
				if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value);
			}
		}

		//	public override void PreSaveAndQuit()
		//	{
		//		PanelUI.UI.Elements.Clear();
		//	}

		//	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		//	{
		//		int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

		//		if (HotbarIndex != -1 && PanelUI != null) layers.Insert(HotbarIndex + 1, PanelUI.InterfaceLayer);
		//	}

		//	public override void UpdateUI(GameTime gameTime)
		//	{
		//		foreach (var entity in TileEntity.ByPosition.Where(x => x.Value is BaseQETE))
		//		{
		//			if (Vector2.Distance(Main.LocalPlayer.Center, entity.Key.ToWorldCoordinates(16, 16)) > 240) PanelUI.UI.CloseUI((BaseQETE)entity.Value);
		//		}

		//		PanelUI.Update(gameTime);

		//		if (TileEntity.ByID.Values.OfType<BaseQETE>().Any(x => x.inScreen && !x.hovered))
		//		{
		//			Main.LocalPlayer.mouseInterface = true;
		//			Main.LocalPlayer.showItemIcon = false;
		//			Main.ItemIconCacheUpdate(0);
		//		}

		//		if (++timer > 60)
		//		{
		//			timer = 0;
		//			for (int i = 0; i < tooltipIndexes.Count; i++)
		//			{
		//				string key = tooltipIndexes.Keys.ElementAt(i);
		//				tooltipIndexes[key]++;
		//				if (tooltipIndexes[key] > ammoTypes[key].Count - 1) tooltipIndexes[key] = 0;
		//			}
		//		}
		//	}
	}
}
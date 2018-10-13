﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class AmmoPounch : BaseAmmoBag
	{
		public override Type UIType => typeof(TheBlackHolePanel);

		public static readonly List<int> ammoTypes = new List<int> { AmmoID.FallenStar, AmmoID.Sand, AmmoID.Snowball, AmmoID.CandyCorn, AmmoID.Stake };

		public AmmoPounch()
		{
			handler = new ItemHandler(27);
			handler.OnContentsChanged += slot =>
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					Player player = Main.player[item.owner];

					List<Item> joined = player.inventory.Concat(player.armor).Concat(player.dye).Concat(player.miscEquips).Concat(player.miscDyes).Concat(player.bank.item).Concat(player.bank2.item).Concat(new[] { player.trashItem }).Concat(player.bank3.item).ToList();
					int index = joined.FindIndex(x => x == item);
					if (index < 0) return;

					NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
				}
			};
			handler.IsItemValid += (slot, item) => ammoTypes.Contains(item.ammo);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ammo Pounch");
			Tooltip.SetDefault($"Stores {handler.Slots} stacks of misc. ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}
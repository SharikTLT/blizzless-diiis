﻿using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._player_shared_stash /* Player_Shared_Stash.acr */)]
	public sealed class Stash : Gizmo
	{
		public Stash(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.MinimapActive] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 202226;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			player.Inventory.StashRevealed = true;
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenSharedStashMessage) 
			{ 
				ActorID = DynamicID(player) 
			});
			if (!player.Inventory.StashLoaded)
				player.Inventory.LoadStashFromDB();
			else
				player.Inventory.RefreshInventoryToClient();
		}

        public override bool Reveal(Player player)
        {
			player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
			{
				HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("Player_Shared_Stash"),
				Place = new MessageSystem.Message.Fields.WorldPlace { Position = Position, WorldID = World.GlobalID },
				ImageInfo = 202226,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = -1,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = true,
				PlayerUseFlags = 0
			});

			return base.Reveal(player);
        }

        public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			player.Inventory.StashRevealed = false;
			player.Inventory.UnrevealStash();
			return true;
		}
	}
}

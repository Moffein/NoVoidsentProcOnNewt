using BepInEx;
using System;
using RoR2;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;

namespace R2API.Utils
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ManualNetworkRegistrationAttribute : Attribute
	{
	}
}

namespace NoVoidsentProcOnNewt
{
	[BepInPlugin("com.Moffein.NoVoidsentProcOnNewt", "NoVoidsentProcOnNewt", "1.0.0")]
    public class NoVoidsentProcOnNewtPlugin : BaseUnityPlugin
    {
		private static BodyIndex newtIndex;
		public void Awake()
        {
			RoR2.RoR2Application.onLoad += GetNewtBodyIndex;

			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC1Content.Items), "ExplodeOnDeathVoid")))
                {
					c.Emit(OpCodes.Ldarg_0);//healthcomponent
					c.EmitDelegate<Func<ItemDef, HealthComponent, ItemDef>>((item, self) =>
					{
						if (self.body.bodyIndex == newtIndex)
                        {
							return null;
                        }
						return item;
					});
                }
				else
                {
					Debug.LogError("NoVoidsentProcOnNewt: IL Hook Failed, the mod will not function.");
                }
			};
        }

		private void GetNewtBodyIndex()
        {
			newtIndex = BodyCatalog.FindBodyIndex("ShopkeeperBody");
        }
    }
}

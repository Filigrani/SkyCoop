using Il2Cpp;
using Il2CppTLD.PDID;
using MelonLoader;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public class DeathPacksManager
    {
        public static Container HandleDeathPack(string BackpackPrefab, Vector3 Position, Quaternion Rotation, string GUID, string OwnerName, bool IsBogus = false)
        {
            if (PdidTable.GetGameObject(GUID))
            {
                return null;
            }
            
            GameObject Obj = null;
            if (!IsBogus)
            {
                GameObject Reference = AssetManager.GetAssetFromGame<GameObject>(BackpackPrefab);
                Obj = UnityEngine.Object.Instantiate(Reference);
            }
            else
            {
                Obj = AssetManager.CreateBogusGear(BackpackPrefab);
            }
            if (Obj)
            {
                Obj.transform.position = Position;
                Obj.transform.rotation = Rotation;
                ObjectGuid GUIDOJB = Obj.GetComponent<ObjectGuid>();
                if (GUIDOJB == null)
                {
                    GUIDOJB = Obj.AddComponent<ObjectGuid>();
                }
                PdidTable.RuntimeRegister(GUIDOJB, GUID);

                Container Box = Obj.GetComponent<Container>();
                if (Box == null)
                {
                    Box = Obj.AddComponent<Container>();
                    Box.m_CloseAudio = "Play_SndGenCanvasBagZipperClose1";
                    Box.m_SearchAudio = "Play_SearchCloth";
                    Box.m_OpenAudio = "Play_SndGenCanvasBagZipperOpen1";
                    Box.m_LocalizedDisplayName = new LocalizedString();
                    Box.m_LocalizedDisplayName.m_LocalizationID = "GAMEPLAY_Backpack";
                }
                Box.m_StartHasBeenCalled = true;
                Box.m_Capacity = new Il2CppTLD.IntBackedUnit.ItemWeight(-1);
                Box.m_Inspected = true;
                Box.m_StartInspected = true;
                Box.m_DisableSerialization = true;
                Box.m_Restored = true;
                Box.m_MarkedInactiveInSaveData = true;
                Box.MarkAsInspected();
                DeathPackComp Comp = Box.gameObject.AddComponent<DeathPackComp>();
                Comp.m_OwnerName = OwnerName;
                return Box;
            }
            return null;
        }

        public static void HandleDeathPackRemoved(string GUID)
        {
            GameObject Box = PdidTable.GetGameObject(GUID);
            if (Box)
            {
                //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleDeathPackRemoved {GUID} found and deleted");
                UnityEngine.Object.Destroy(Box);
            }
            else
            {
                //SkyCoop.Logger.Log(ConsoleColor.Red, $"HandleDeathPackRemoved {GUID} not found!");
            }
        }

        public static void CreateMyDeathPack()
        {
            string Prefab = "CONTAINER_Backpack";
            string GUID = Guid.NewGuid().ToString();
            string OwnerName = ModMain.GetNickName();
            Vector3 Position = GameManager.GetPlayerTransform().position;
            Quaternion Rotation = GameManager.GetPlayerTransform().rotation;
            string JSON = "";

            Container Box = HandleDeathPack(Prefab, Position, Rotation, GUID, OwnerName);
            if (Box)
            {
                Inventory Inv = GameManager.GetInventoryComponent();
                List<GearItem> Gears = new List<GearItem>();
                for (int index = 0; index < Inv.m_Items.Count; ++index)
                {
                    GearItem gearItem = (GearItem)Inv.m_Items[index];
                    if (gearItem)
                    {
                        Gears.Add(gearItem);

                        if (GameManager.GetPlayerManagerComponent().m_ItemInHands && GameManager.GetPlayerManagerComponent().m_ItemInHands == gearItem)
                        {
                            GameManager.GetPlayerManagerComponent().UnequipImmediate(false);
                        }
                    }
                }

                foreach (GearItem Gear in Gears)
                {
                    if (!Gear.m_HandheldShortwaveItem)
                    {
                        if (Gear.m_WaterSupply)
                        {
                            Panel_PickWater Panel = InterfaceManager.GetPanel<Panel_PickWater>();
                            if (Panel)
                            {
                                InterfaceManager.GetPanel<Panel_PickWater>().TransferAllWaterInventoryToContainer(Box, Gear.m_WaterSupply);
                            }
                        }
                        else if (Gear.m_StackableItem && Box.AddToExistingStackable(Gear, Gear.GetNormalizedCondition(), Gear.m_StackableItem.m_Units))
                        {
                            Inv.DestroyGear(Gear.gameObject);
                        }
                        else
                        {
                            Box.AddGear(Gear);
                            Inv.RemoveGear(Gear.gameObject, true);
                        }
                    }
                }

                JSON = Box.Serialize();

                Box.DestroyAllGear();
            }
            ClientSend.SendDeathPack(Prefab, GUID, DataStr.CompressString(JSON), OwnerName, Position, Rotation);
        }
    }
}

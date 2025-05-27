using Il2Cpp;
using Il2CppNewtonsoft.Json;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopClient
{
    public class ClothingCombininizer
    {
        public static int GetCombination(string Hat1, string Hat2, string[] Combinable)
        {
            foreach (string Combo in Combinable)
            {
                if (Hat1 == Combo)
                {
                    if (string.IsNullOrEmpty(Hat2))
                    {
                        return 1;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else if (Hat2 == Combo)
                {
                    if (string.IsNullOrEmpty(Hat1))
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
            return 0;
        } 

        public static int GetHeadGearToUse(GearItem Gear1, GearItem Gear2)
        {
            string Hat1 = GetClothingNameFromGear(Gear1);
            string Hat2 = GetClothingNameFromGear(Gear2);

            if (string.IsNullOrEmpty(Hat1) && string.IsNullOrEmpty(Hat2))
            {
                return 0;
            }

            if (Hat1 == Hat2)
            {
                return 2;
            }

            if (!string.IsNullOrEmpty(Hat1) && string.IsNullOrEmpty(Hat2))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(Hat1) && !string.IsNullOrEmpty(Hat2))
            {
                return 2;
            }

            return GetCombination(Hat1, Hat2, new string[] 
            {
                "GEAR_Balaclava",
                "GEAR_BasicWoolScarf",
                "GEAR_CottonScarf",
                "GEAR_WoolWrap",
                "GEAR_WoolWrapCap",
                "GEAR_CookingPot",
            }
            );
        }
        //                                            1                       2                      3                      4
        public static int GetMostTopFromFour(GearItem GearBaseLayer, GearItem GearMidLayer, GearItem GearTopLayer, GearItem GearTopLayer2)
        {
            string BaseLayer = GetClothingNameFromGear(GearBaseLayer);
            string MidLayer = GetClothingNameFromGear(GearMidLayer);
            string TopLayer = GetClothingNameFromGear(GearTopLayer);
            string TopLayer2 = GetClothingNameFromGear(GearTopLayer2);

            //SkyCoop.Logger.Log($"BaseLayer {BaseLayer}");
            //SkyCoop.Logger.Log($"MidLayer {MidLayer}");
            //SkyCoop.Logger.Log($"TopLayer {TopLayer}");
            //SkyCoop.Logger.Log($"TopLayer2 {TopLayer2}");

            if (string.IsNullOrEmpty(TopLayer) && string.IsNullOrEmpty(TopLayer2)) // No pants
            {
                //SkyCoop.Logger.Log($"Returning TopLayer ({TopLayer}) and TopLayer2 ({TopLayer2}) is empty, checking bottom layers");
                if (string.IsNullOrEmpty(BaseLayer) && string.IsNullOrEmpty(MidLayer)) // No underware
                {
                    //SkyCoop.Logger.Log($"All 4 layers are empty, return nothing");
                    return 0;
                }
                
                if(BaseLayer == MidLayer) // Both underware are the same, return most top one.
                {
                    //SkyCoop.Logger.Log($"BaseLayer ({BaseLayer}) and MidLayer ({MidLayer}) are the same, returning MidLayer ({MidLayer})");
                    return 2;
                }

                if (!string.IsNullOrEmpty(BaseLayer) && string.IsNullOrEmpty(MidLayer))
                {
                    //SkyCoop.Logger.Log($"Returning BaseLayer ({BaseLayer}), cause MidLayer ({MidLayer}) is empty");
                    return 1;
                }

                if (string.IsNullOrEmpty(BaseLayer) && !string.IsNullOrEmpty(MidLayer))
                {
                    //SkyCoop.Logger.Log($"Returning MidLayer ({MidLayer}), cause BaseLayer ({BaseLayer}) is empty");
                    return 2;
                }

                //SkyCoop.Logger.Log($"Returning MidLayer ({MidLayer}), cause it's toppest layer, over BaseLayer ({BaseLayer})");
                return 2;
            }


            if (TopLayer == TopLayer2)
            {
                //SkyCoop.Logger.Log($"TopLayer ({TopLayer}) and TopLayer2 ({TopLayer2}) are the same, returning TopLayer2 ({TopLayer2})");
                return 4;
            }

            if (!string.IsNullOrEmpty(TopLayer) && string.IsNullOrEmpty(TopLayer2))
            {
                //SkyCoop.Logger.Log($"Returning TopLayer ({TopLayer}), cause TopLayer2 ({TopLayer2}) is empty");
                return 3;
            }

            if (string.IsNullOrEmpty(TopLayer) && !string.IsNullOrEmpty(TopLayer2))
            {
                //SkyCoop.Logger.Log($"Returning TopLayer2 ({TopLayer2}), cause TopLayer ({TopLayer}) is empty");
                return 4;
            }
            //SkyCoop.Logger.Log($"Returning TopLayer2 ({TopLayer2}), cause it's toppest layer, over TopLayer ({TopLayer})");
            return 4;
        }

        public static GearItem GetClothingInSlot(ClothingRegion s, ClothingLayer l)
        {
            if (GameManager.m_PlayerManager == null)
            {
                return null;
            }

            GearItem gi = GameManager.GetPlayerManagerComponent().GetClothingInSlot(s, l);
            if (gi)
            {
                return gi;
            }
            return null;
        }

        public static string GetClothingNameFromGear(GearItem gi)
        {
            if (gi)
            {
                return gi.name;
            }
            return "";
        }

        public static float GetClothingDamageFromGear(GearItem gi)
        {
            if (gi)
            {
                return gi.GetDamageBlendValue();
            }
            return 0;
        }

        public static DataStr.ClothingData GetClothing()
        {
            DataStr.ClothingData Data = new DataStr.ClothingData();

            GearItem Hat1 = GetClothingInSlot(ClothingRegion.Head, ClothingLayer.Base);
            GearItem Hat2 = GetClothingInSlot(ClothingRegion.Head, ClothingLayer.Mid);

            int HatResult = GetHeadGearToUse(Hat1, Hat2);
            switch (HatResult)
            {
                case 0:
                    Data.m_Hat1 = "";
                    Data.m_Hat1Damage = 0;
                    break;
                case 1:
                    Data.m_Hat1 = GetClothingNameFromGear(Hat1);
                    Data.m_Hat1Damage = GetClothingDamageFromGear(Hat1);
                    break;
                case 2:
                    Data.m_Hat1 = GetClothingNameFromGear(Hat2);
                    Data.m_Hat1Damage = GetClothingDamageFromGear(Hat2);
                    break;
                case 3:
                    Data.m_Hat1 = GetClothingNameFromGear(Hat1);
                    Data.m_Hat1Damage = GetClothingDamageFromGear(Hat1);

                    Data.m_Hat2 = GetClothingNameFromGear(Hat2);
                    Data.m_Hat2Damage = GetClothingDamageFromGear(Hat2);
                    break;
            }

            GearItem Shirt1 = GetClothingInSlot(ClothingRegion.Chest, ClothingLayer.Base);
            GearItem Shirt2 = GetClothingInSlot(ClothingRegion.Chest, ClothingLayer.Mid);
            GearItem Jacket1 = GetClothingInSlot(ClothingRegion.Chest, ClothingLayer.Top);
            GearItem Jacket2 = GetClothingInSlot(ClothingRegion.Chest, ClothingLayer.Top2);


            GearItem Pants1 = GetClothingInSlot(ClothingRegion.Legs, ClothingLayer.Top);
            GearItem Pants2 = GetClothingInSlot(ClothingRegion.Legs, ClothingLayer.Top2);
            GearItem LongUnderware1 = GetClothingInSlot(ClothingRegion.Legs, ClothingLayer.Base);
            GearItem LongUnderware2 = GetClothingInSlot(ClothingRegion.Legs, ClothingLayer.Mid);

            int BodyResult = GetMostTopFromFour(Shirt1, Shirt2, Jacket1, Jacket2);
            switch (BodyResult)
            {
                case 0:
                    Data.m_Body = "";
                    Data.m_BodyDamage = 0;
                    break;
                case 1:
                    Data.m_Body = GetClothingNameFromGear(Shirt1);
                    Data.m_BodyDamage = GetClothingDamageFromGear(Shirt1);
                    break;
                case 2:
                    Data.m_Body = GetClothingNameFromGear(Shirt2);
                    Data.m_BodyDamage = GetClothingDamageFromGear(Shirt2);
                    break;
                case 3:
                    Data.m_Body = GetClothingNameFromGear(Jacket1);
                    Data.m_BodyDamage = GetClothingDamageFromGear(Jacket1);
                    break;
                case 4:
                    Data.m_Body = GetClothingNameFromGear(Jacket2);
                    Data.m_BodyDamage = GetClothingDamageFromGear(Jacket2);
                    break;
            }
            int PantsResult = GetMostTopFromFour(LongUnderware1, LongUnderware2, Pants1, Pants2);
            switch (PantsResult)
            {
                case 0:
                    Data.m_Pants = "";
                    Data.m_PantsDamage = 0;
                    break;
                case 1:
                    Data.m_Pants = GetClothingNameFromGear(LongUnderware1);
                    Data.m_PantsDamage = GetClothingDamageFromGear(LongUnderware1);
                    break;
                case 2:
                    Data.m_Pants = GetClothingNameFromGear(LongUnderware2);
                    Data.m_PantsDamage = GetClothingDamageFromGear(LongUnderware2);
                    break;
                case 3:
                    Data.m_Pants = GetClothingNameFromGear(Pants1);
                    Data.m_PantsDamage = GetClothingDamageFromGear(Pants1);
                    break;
                case 4:
                    Data.m_Pants = GetClothingNameFromGear(Pants2);
                    Data.m_PantsDamage = GetClothingDamageFromGear(Pants2);
                    break;
            }

            GearItem Socks1 = GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Base);
            GearItem Socks2 = GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Mid);
            GearItem Boots = GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Top);

            int BootsResult = GetMostTopFromFour(Socks1, Socks2, Boots, null);
            switch (BootsResult)
            {
                case 0:
                case 4:
                    Data.m_Boots = "";
                    Data.m_BootsDamage = 0;
                    break;
                case 1:
                    Data.m_Boots = GetClothingNameFromGear(Socks1);
                    Data.m_BootsDamage = GetClothingDamageFromGear(Socks1);
                    break;
                case 2:
                    Data.m_Boots = GetClothingNameFromGear(Socks2);
                    Data.m_BootsDamage = GetClothingDamageFromGear(Socks2);
                    break;
                case 3:
                    Data.m_Boots = GetClothingNameFromGear(Boots);
                    Data.m_BootsDamage = GetClothingDamageFromGear(Boots);
                    break;
            }

            GearItem Gloves = GetClothingInSlot(ClothingRegion.Hands, ClothingLayer.Base);


            Data.m_Gloves = GetClothingNameFromGear(Gloves);
            Data.m_GlovesDamage = GetClothingDamageFromGear(Gloves);



            Data.m_Accs1 = GetClothingNameFromGear(GetClothingInSlot(ClothingRegion.Accessory, ClothingLayer.Base));
            Data.m_Accs2 = GetClothingNameFromGear(GetClothingInSlot(ClothingRegion.Accessory, ClothingLayer.Mid));

            Data.m_TechPack = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_TechnicalBackpack");

            return Data;
        }
    }
}

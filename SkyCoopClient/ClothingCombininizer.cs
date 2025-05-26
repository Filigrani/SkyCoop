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
            }
            );
        }

        public static int GetMostTopFromFour(GearItem GearBaseLayer, GearItem GearMidLayer, GearItem GearTopLayer, GearItem GearTopLayer2)
        {
            string BaseLayer = GetClothingNameFromGear(GearBaseLayer);
            string MidLayer = GetClothingNameFromGear(GearMidLayer);
            string TopLayer = GetClothingNameFromGear(GearTopLayer);
            string TopLayer2 = GetClothingNameFromGear(GearTopLayer2);

            if (string.IsNullOrEmpty(TopLayer) && string.IsNullOrEmpty(TopLayer2))
            {
                if(string.IsNullOrEmpty(BaseLayer) && string.IsNullOrEmpty(MidLayer))
                {
                    return 0;
                }
                
                if(BaseLayer == MidLayer)
                {
                    return 2;
                }

                if (!string.IsNullOrEmpty(BaseLayer) && string.IsNullOrEmpty(MidLayer))
                {
                    return 1;
                }

                if (string.IsNullOrEmpty(BaseLayer) && !string.IsNullOrEmpty(MidLayer))
                {
                    return 2;
                }

                return 2;
            }

            if (TopLayer == TopLayer2)
            {
                return 4;
            }

            if (!string.IsNullOrEmpty(TopLayer) && string.IsNullOrEmpty(TopLayer2))
            {
                return 3;
            }

            if (string.IsNullOrEmpty(TopLayer) && !string.IsNullOrEmpty(TopLayer2))
            {
                return 4;
            }
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
            int PantsResult = GetMostTopFromFour(Pants1, Pants2, LongUnderware1, LongUnderware2);
            switch (PantsResult)
            {
                case 0:
                    Data.m_Pants = "";
                    Data.m_PantsDamage = 0;
                    break;
                case 1:
                    Data.m_Pants = GetClothingNameFromGear(Pants1);
                    Data.m_PantsDamage = GetClothingDamageFromGear(Pants1);
                    break;
                case 2:
                    Data.m_Pants = GetClothingNameFromGear(Pants2);
                    Data.m_PantsDamage = GetClothingDamageFromGear(Pants2);
                    break;
                case 3:
                    Data.m_Pants = GetClothingNameFromGear(LongUnderware1);
                    Data.m_PantsDamage = GetClothingDamageFromGear(LongUnderware1);
                    break;
                case 4:
                    Data.m_Pants = GetClothingNameFromGear(LongUnderware2);
                    Data.m_PantsDamage = GetClothingDamageFromGear(LongUnderware2);
                    break;
            }

            GearItem Gloves = GetClothingInSlot(ClothingRegion.Hands, ClothingLayer.Base);
            GearItem Boots = GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Base);

            Data.m_Gloves = GetClothingNameFromGear(Gloves);
            Data.m_GlovesDamage = GetClothingDamageFromGear(Gloves);

            Data.m_Boots = GetClothingNameFromGear(Boots);
            Data.m_BootsDamage = GetClothingDamageFromGear(Boots);

            Data.m_Accs1 = GetClothingNameFromGear(GetClothingInSlot(ClothingRegion.Accessory, ClothingLayer.Base));
            Data.m_Accs2 = GetClothingNameFromGear(GetClothingInSlot(ClothingRegion.Accessory, ClothingLayer.Mid));

            Data.m_TechPack = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_TechnicalBackpack");

            return Data;
        }
    }
}

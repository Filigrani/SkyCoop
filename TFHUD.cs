using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoop
{
    public class TFHUD
    {
        public static GameObject TF2HUDLeft = null;
        public static GameObject TF2HUDRight = null;
        public static bool Enabled = true;
        
        public static void Init(AssetBundle Bundle, Transform CanvasTransform)
        {
            GameObject LoadedAssets15 = Bundle.LoadAsset<GameObject>("TF_HUDLeft");
            TF2HUDLeft = GameObject.Instantiate(LoadedAssets15, CanvasTransform);
            TF2HUDLeft.SetActive(false);
            GameObject LoadedAssets16 = Bundle.LoadAsset<GameObject>("TF_HUDRight");
            TF2HUDRight = GameObject.Instantiate(LoadedAssets16, CanvasTransform);
            TF2HUDRight.SetActive(false);
        }

        public static void DisableOriginalHUD()
        {
            if (InterfaceManager.m_Panel_HUD)
            {
                InterfaceManager.m_Panel_HUD.m_RegularSizeGroup.gameObject.SetActive(false);
                InterfaceManager.m_Panel_HUD.m_SmallSizeGroup.gameObject.SetActive(false);
                InterfaceManager.m_Panel_HUD.m_LargeSizeGroup.gameObject.SetActive(false);
                InterfaceManager.m_Panel_HUD.m_AimingStaminaBar.gameObject.transform.parent.gameObject.SetActive(false);
                InterfaceManager.m_Panel_HUD.m_EquipItemPopup.gameObject.SetActive(false);
            }
        }

        public static void Update() 
        {
            if (!Enabled)
            {
                if (TF2HUDRight != null)
                {
                    TF2HUDRight.SetActive(false);
                }    
                if(TF2HUDRight != null)
                {
                    TF2HUDLeft.SetActive(false);
                }
                return;
            }

            DisableOriginalHUD();


            if (TF2HUDLeft != null && TF2HUDRight != null)
            {
                bool Render = false;
                //InterfaceManager.m_Panel_HUD.m_NonEssentialHud.transform.GetChild(0).gameObject.activeSelf
                if (InterfaceManager.m_Panel_HUD != null && InterfaceManager.m_Panel_HUD.IsEnabled() && !InterfaceManager.m_Panel_HUD.GetHideHudElements())
                {
                    if ((InterfaceManager.m_Panel_Rest && InterfaceManager.m_Panel_Rest.IsEnabled()) ||
                        GameManager.IsMainMenuActive() ||
                        GameManager.GetPlayerManagerComponent().IsInspectModeActive() ||
                        HUDManager.DoNotRenderHUD() ||
                        InterfaceManager.IsOverlayActiveCached() ||
                        InterfaceManager.m_Panel_HUD.m_HideHudElements)
                    {
                        Render = false;
                    } else
                    {
                        Render = true;
                    }
                }

                TF2HUDLeft.SetActive(Render);
                TF2HUDRight.SetActive(Render);

                if (Render)
                {
                    Condition Con = GameManager.GetConditionComponent();
                    Freezing Frz = GameManager.GetFreezingComponent();
                    Hunger Hun = GameManager.GetHungerComponent();
                    Thirst Thr = GameManager.GetThirstComponent();
                    PlayerMovement Mov = GameManager.GetPlayerMovementComponent();
                    Fatigue Fa = GameManager.GetFatigueComponent();
                    Transform HP = TF2HUDLeft.transform.GetChild(0);
                    if (Con && Frz && Hun && Thr && Mov && Fa)
                    {
                        UnityEngine.UI.Text HpNumber = HP.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Text>();
                        UnityEngine.UI.Text HpMax = HP.GetChild(7).gameObject.GetComponent<UnityEngine.UI.Text>();
                        UnityEngine.UI.Image HpBar = HP.GetChild(5).gameObject.GetComponent<UnityEngine.UI.Image>();

                        HpNumber.text = Convert.ToInt32(Con.m_CurrentHP).ToString();
                        if (Con.m_CurrentHP < Con.m_MaxHP)
                        {
                            HpMax.text = Convert.ToInt32(Con.m_MaxHP).ToString();
                        } else
                        {
                            HpMax.text = "";
                        }
                        float healthPercent = (100f / Con.m_MaxHP) * Con.m_CurrentHP;
                        bool LowHp = Con.m_CurrentHP < 45;
                        bool OverHeal = Con.m_CurrentHP > 100;
                        HP.GetChild(3).gameObject.SetActive(LowHp);
                        HP.GetChild(2).gameObject.SetActive(OverHeal);
                        if (LowHp)
                        {
                            //UnityEngine.UI.Image imgComp = TF2HUD.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Image>();
                            //imgComp.color = Color.Lerp(new Color(159, 18, 7, 255), new Color(159, 18, 7, 0), Mathf.PingPong(Time.time * 2.2f, 1));
                        }
                        if (OverHeal)
                        {
                            //UnityEngine.UI.Image imgComp = TF2HUD.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Image>();
                            //imgComp.color = Color.Lerp(new Color(255, 255, 255, 255), new Color(255, 255, 255, 0), Mathf.PingPong(Time.time * 0.5f, 1));
                        }
                        HpBar.fillAmount = healthPercent / 100;

                        UnityEngine.UI.Image ColdBar = TF2HUDLeft.transform.GetChild(1).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Image>();
                        UnityEngine.UI.Image ThirstBar = TF2HUDLeft.transform.GetChild(2).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Image>();
                        UnityEngine.UI.Image HungerBar = TF2HUDLeft.transform.GetChild(3).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Image>();
                        UnityEngine.UI.Image FatigueBar = TF2HUDLeft.transform.GetChild(4).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Image>();
                        float coldPercent = (100f / Frz.m_MaxFreezing) * (Frz.m_MaxFreezing - Frz.m_CurrentFreezing);
                        ColdBar.fillAmount = coldPercent / 100;

                        float ThirstPercent = (100f / Thr.m_MaxThirst) * (Thr.m_MaxThirst - Thr.m_CurrentThirst);
                        ThirstBar.fillAmount = ThirstPercent / 100;

                        float HungerPercent = (100f / Hun.m_MaxReserveCalories) * Hun.m_CurrentReserveCalories;
                        HungerBar.fillAmount = HungerPercent / 100;

                        float FatiguePercent = (100f / Fa.m_MaxFatigue) * (Fa.m_MaxFatigue - Fa.m_CurrentFatigue);
                        FatigueBar.fillAmount = FatiguePercent / 100;

                        UnityEngine.UI.Image SprintBar = TF2HUDRight.transform.GetChild(0).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Image>();
                        UnityEngine.UI.Image SprintBarPenality = TF2HUDRight.transform.GetChild(0).GetChild(4).gameObject.GetComponent<UnityEngine.UI.Image>();
                        float SprintPercent = (100f / Mov.m_MaxSprintStamina) * Mov.m_SprintStamina;
                        float SprintPenalityPercent = GameManager.GetPlayerManagerComponent().GetNormalizedSprintReduction();
                        SprintBar.fillAmount = SprintPercent / 100;
                        SprintBarPenality.fillAmount = SprintPenalityPercent / 100;
                        TF2HUDRight.transform.GetChild(0).gameObject.SetActive(Mov.m_SprintStamina != Mov.GetModifiedMaxSprintStamina());


                        vp_FPSCamera vpFPS = GameManager.GetVpFPSCamera();
                        if (vpFPS != null && vpFPS.m_CurrentWeapon != null)
                        {
                            GearItem Gi = vpFPS.m_CurrentWeapon.m_GearItem;
                            int Ammo = vpFPS.m_CurrentWeapon.GetAmmoCount();
                            int Reserve = 0;
                            if (GameManager.GetInventoryComponent())
                            {
                                Reserve = GameManager.GetInventoryComponent().GetAmmoAvailableForWeapon(vpFPS.m_CurrentWeapon.m_GearItem);
                            }

                            if (Gi.m_BowItem)
                            {
                                Ammo = Gi.m_BowItem.GetNumArrowsInInventory();
                            }
                            int Total = Ammo + Reserve;
                            if (Total > 0)
                            {
                                TF2HUDRight.transform.GetChild(1).gameObject.SetActive(true);
                                TF2HUDRight.transform.GetChild(1).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = Ammo.ToString();
                                TF2HUDRight.transform.GetChild(1).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = Reserve.ToString();
                            } else
                            {
                                TF2HUDRight.transform.GetChild(1).gameObject.SetActive(false);
                                if (Gi.m_FlareItem != null)
                                {
                                    if (!Gi.m_FlareItem.IsBurning())
                                    {
                                        Ammo = GameManager.GetInventoryComponent().GetNumFlares(Gi.m_FlareItem.m_Type);
                                    } else
                                    {
                                        Ammo = Convert.ToInt32(Gi.m_FlareItem.GetNormalizedBurnTimeLeft() * 100);
                                    }
                                }
                                if (Gi.m_MatchesItem != null)
                                {
                                    Ammo = GameManager.GetInventoryComponent().GetNumMatches();
                                }
                                if (Gi.m_TorchItem != null)
                                {
                                    if (Gi.m_TorchItem.IsBurning())
                                    {
                                        Ammo = 100 - Convert.ToInt32(Gi.m_TorchItem.GetBurnProgress() * 100);
                                    } else
                                    {
                                        Ammo = GameManager.GetInventoryComponent().GetNumTorches();
                                    }
                                }

                                if (Gi.m_KeroseneLampItem != null)
                                {
                                    Ammo = Convert.ToInt32(Gi.m_KeroseneLampItem.GetPercentFuelRemaining());
                                }
                                if (Gi.m_StoneItem != null)
                                {
                                    Ammo = GameManager.GetInventoryComponent().GetNumStones();
                                }
                                if (Gi.m_FlashlightItem != null)
                                {
                                    Ammo = Convert.ToInt32(Gi.m_FlashlightItem.GetNormalizedCharge() * 100);
                                }
                                if (Gi.m_SprayPaintCan != null)
                                {
                                    Ammo = Convert.ToInt32(Gi.GetNormalizedCondition() * 100);
                                }
                                if (Gi.m_NoiseMakerItem != null)
                                {
                                    if (Gi.m_NoiseMakerItem.IsWickLit())
                                    {
                                        Ammo = Convert.ToInt32(Gi.m_NoiseMakerItem.GetNormalizedWickTimeLeft() * 100);
                                    } else
                                    {
                                        Ammo = GameManager.GetInventoryComponent().GetNumNoiseMakers();
                                    }
                                }
                                TF2HUDRight.transform.GetChild(2).gameObject.SetActive(Ammo > 0);

                                TF2HUDRight.transform.GetChild(2).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = Ammo.ToString();
                            }
                        } else
                        {
                            TF2HUDRight.transform.GetChild(1).gameObject.SetActive(false);
                            TF2HUDRight.transform.GetChild(2).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}

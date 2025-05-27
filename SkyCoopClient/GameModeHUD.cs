using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public class GameModeHUD
    {
        public static GameObject s_DarkwalkerHUDClone;
        static HUDNowhereToHide s_HUD;

        public static List<UISprite> s_SideIcons = new List<UISprite>();
        public static List<UILabel> s_SideLables = new List<UILabel>();
        public static List<string> s_SideLablesPrefix = new List<string>() { "", "", "", "" };
        public static UILabel s_BottomLable = null;

        public static void Reintilize()
        {
            if (s_DarkwalkerHUDClone == null)
            {
                s_SideIcons.Clear();
                s_SideLables.Clear();
                s_SideLablesPrefix = new List<string>() { "", "", "", "" };
                Panel_HUD Panel = null;
                if (InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
                {
                    s_DarkwalkerHUDClone = UnityEngine.Object.Instantiate(Panel.m_NowhereToHide.gameObject, Panel.m_NowhereToHide.transform.parent);
                    s_DarkwalkerHUDClone.SetActive(true);
                    s_HUD = s_DarkwalkerHUDClone.GetComponent<HUDNowhereToHide>();
                    s_HUD.m_AfflictionRoot.SetActive(false);
                    s_HUD.m_EntityDistanceRoot.SetActive(false);
                    s_HUD.m_StartCountdownRoot.SetActive(false);
                    s_HUD.m_LureGlyphRoot.SetActive(false);
                    s_HUD.m_WardGlyphRoot.SetActive(false);
                    s_HUD.m_ToxicFogWarningRoot.SetActive(false);
                    s_HUD.transform.GetChild(4).gameObject.SetActive(false);
                    s_HUD.m_ToxicFogIndicatorLabel.gameObject.SetActive(false);
                    s_HUD.m_ToxicFogIndicatorLabel.text = "";
                    s_BottomLable = s_HUD.m_ToxicFogIndicatorLabel;


                    s_SideIcons.Add(FixSideIcon(s_HUD.m_WardGlyphRoot.transform));
                    s_SideIcons.Add(FixSideIcon(s_HUD.m_LureGlyphRoot.transform));
                    s_SideIcons.Add(FixSideIcon(s_HUD.transform.GetChild(4)));

                    s_SideLables.Add(GetSideLable(s_HUD.m_WardGlyphRoot.transform));
                    s_SideLables.Add(GetSideLable(s_HUD.m_LureGlyphRoot.transform));
                    s_SideLables.Add(GetSideLable(s_HUD.transform.GetChild(4)));
                    s_SideLables.Add(s_BottomLable);

                }
            }
        }

        public static UISprite FixSideIcon(Transform Root)
        {
            UITexture Tex = Root.GetChild(0).GetComponent<UITexture>();
            Root.GetChild(0).GetComponent<TweenAlpha>().enabled = false;
            if (Tex)
            {
                Tex.enabled = false; // Remove Icon
            }
            else
            {
                Root.GetChild(0).GetComponent<UISprite>().enabled = false;
            }
            
            Root.GetChild(2).gameObject.SetActive(false); // Hide progress bar

            Transform BGIcon = Root.GetChild(0).GetChild(0);
            UISprite Sprite = BGIcon.GetComponent<UISprite>();
            BGIcon.transform.rotation = Quaternion.identity;
            BGIcon.GetComponent<TweenAlpha>().enabled = false;
            Sprite.alpha = 1f;
            Sprite.color = Color.white;
            return Sprite;
        }

        public static void SetSideIcon(int SideIconIndex, string Icon)
        {
            if(SideIconIndex < 0 || SideIconIndex > s_SideIcons.Count-1)
            {
                return;
            }
            s_SideIcons[SideIconIndex].transform.parent.parent.gameObject.SetActive(true);
            s_SideIcons[SideIconIndex].gameObject.SetActive(true);
            s_SideIcons[SideIconIndex].spriteName = Icon;
        }

        public static UILabel GetSideLable(Transform Root)
        {
            UILabel Lable = Root.GetChild(1).GetComponent<UILabel>();
            UILocalize Loca = Lable.GetComponent<UILocalize>();
            if (Loca)
            {
                UnityEngine.Object.Destroy(Loca);
            }

            return Lable;
        }

        public static void SetSideLable(int SideLableIndex, string Text)
        {
            if (SideLableIndex < 0 || SideLableIndex > s_SideLables.Count - 1)
            {
                return;
            }
            s_SideLables[SideLableIndex].gameObject.SetActive(true);
            s_SideLables[SideLableIndex].text = s_SideLablesPrefix[SideLableIndex]+Text;
        }
        public static void SetSideLablePrefix(int SideLableIndex, string Text)
        {
            s_SideLablesPrefix[SideLableIndex] = Text;
        }

        public static void SetBottomLable(string Text)
        {
            s_BottomLable.gameObject.SetActive(true);
            s_BottomLable.text = Text;
        }
        public static void UpdateGameModeTimer(float TimeLeft)
        {
            if (s_HUD)
            {
                int Minutes = (int)TimeLeft / 60;
                int Seconds = (int)TimeLeft % 60;
                s_HUD.m_StartCountdownRoot.SetActive(true);
                UILabel Lable = s_HUD.m_StartCountdownRoot.transform.GetChild(0).GetComponent<UILabel>();
                UILocalize Loca = Lable.GetComponent<UILocalize>();
                if (Loca)
                {
                    UnityEngine.Object.Destroy(Loca);
                }

                Lable.text = "Time remaining";
                s_HUD.m_StartCountdownLabel.text = string.Format("{0:0}:{1:00}", Minutes, Seconds);
            }
        }
    }
}

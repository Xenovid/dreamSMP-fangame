using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkSender : MonoBehaviour
{
    public static void sendToTechno(websites website){
        switch(website){
            case websites.youtube:
                Application.OpenURL("https://www.youtube.com/user/technothepig");
                break;
            case websites.twitter:
                Application.OpenURL("https://twitter.com/Technothepig?ref_src=twsrc%5Egoogle%7Ctwcamp%5Eserp%7Ctwgr%5Eauthor");
                break;
            case websites.twitch:
                Application.OpenURL("https://www.twitch.tv/technoblade");
                break;
        }
    }

    public static void sendToTommy(websites website){
        switch(website){
            case websites.youtube:
                Application.OpenURL("https://www.youtube.com/channel/UC5p_l5ZeB_wGjO_yDXwiqvw");
                break;
            case websites.twitter:
                Application.OpenURL("https://twitter.com/tommyinnit");
                break;
            case websites.twitch:
                Application.OpenURL("https://www.twitch.tv/tommyinnit");
                break;
        }
    }

    public static void sendToWilbur(websites website){
        switch(website){
            case websites.youtube:
                Application.OpenURL("https://www.youtube.com/channel/UC1n_PfsVqxllCcnMPlxBIjw");
                break;
            case websites.twitter:
                Application.OpenURL("https://twitter.com/WilburSoot");
                break;
            case websites.twitch:
                Application.OpenURL("https://www.twitch.tv/wilbursoot");
                break;
        }
    }

}


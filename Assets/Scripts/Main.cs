using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;


public class Main : MonoBehaviour
{

    const string gif = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1497885550184&di=48b9289901fcd8c5dfa773873d9e6b00&imgtype=0&src=http%3A%2F%2Fimage.manyule.com%2Fbbs%2Fdata%2Fattachment%2Fforum%2F201508%2F04%2F160054selefffv3cgxfelx.gif";
    const string png = "http://172.16.3.231/pic/1.png";
    const string jpg = "http://172.16.3.231/pic/21.jpg";

    public GameObject m_imgObj;
    private string texname;
    private Texture2D m_Tex;
    public Text m_info;

    void Start()
    {
      //  StartCoroutine(DownloadHander());
    }
    

    IEnumerator DownloadHander()
    {
        WWW www = new WWW(gif);
        yield return www;
        File.WriteAllBytes(Application.temporaryCachePath + "/" + gif.GetHashCode(), www.bytes);
        ImgType type = Util.sington.GetTypeByBytes(www.bytes);
        print("type1:" + type);

        www = new WWW(png);
        yield return www;
        File.WriteAllBytes(Application.temporaryCachePath + "/" + png.GetHashCode(), www.bytes);
        type = Util.sington.GetTypeByBytes(www.bytes);
        print("type2:" + type);

        www = new WWW(jpg);
        yield return www;
        File.WriteAllBytes(Application.temporaryCachePath + "/" + jpg.GetHashCode(), www.bytes);
        type = Util.sington.GetTypeByBytes(www.bytes);
        print("type3:" + type);
        www.Dispose();
    }



    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 140, 40), "像素密度"))
        {
            int density = NativeBridge.sington.NGetDensity();
            m_info.text = "PPI:" + density;
        }
        if (GUI.Button(new Rect(10, 70, 140, 40), "运营商"))
        {
            string str= NativeBridge.sington.NCheckSIM();
            m_info.text = "运营商：" + str;
        }
        if (GUI.Button(new Rect(10, 130, 140, 40), "加载gif"))
        {
            StartCoroutine(GifHander());
        }
    }

    IEnumerator GifHander()
    {
        WWW www = new WWW(gif);
        yield return www;
        string path = Application.temporaryCachePath + "/" + gif.GetHashCode() + ".jpg";
        Debug.Log("save path:" + path);
        NativeBridge.sington.NToJPG(path, www.bytes);
        Debug.Log("trans jpg finish! path:" + path);

        m_Tex = new Texture2D(100, 100);
#if UNITY_EDITOR
        m_Tex = Resources.Load<Texture2D>("Icon-180");
        Debug.Log("txt is: " + (m_Tex == null));
#else
        m_Tex.LoadImage(File.ReadAllBytes(path));
#endif
        Sprite tempSprite = new Sprite();
        tempSprite = Sprite.Create(m_Tex, new Rect(0, 0, m_Tex.width, m_Tex.height), Vector2.zero);
        m_imgObj.GetComponent<Image>().sprite = tempSprite;
    }


}

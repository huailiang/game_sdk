using System.Text;


public enum ImgType
{
    JPG,
    PNG,
    GIF,
    TGA,
    BMP,
    ICO,
    None
};


public class Util 
{
    private static Util _s;

    public static Util sington
    {
        get
        {
            if (_s == null) _s = new Util();
            return _s;
        }
    }


    public ImgType GetTypeByBytes(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        string str = sb.ToString().ToUpper();

        ImgType type = ImgType.None;
        if (str.Equals("47494638")) type = ImgType.GIF;
        else if (str.Equals("89504E47")) type = ImgType.PNG;
        else if (str.Equals("00000200") || str.Equals("00001000")) type = ImgType.TGA;
        else if (str.StartsWith("ffd8")) type = ImgType.JPG;
        else if (str.StartsWith("424D")) type = ImgType.BMP;
        else if (str.Equals("00000100")) type = ImgType.ICO;
        return type;
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Vector4Float
{
    [Range(0, 1)]
    public float xMin;
    [Range(0, 1)]
    public float yMin;
    [Range(0, 1)]
    public float xMax;
    [Range(0, 1)]
    public float yMax;
}

public static class ToolUnit
{   
    /// <summary>
     /// 用于获取视频路径筛选的数组
     /// </summary>
     /// <summary>
     /// 视频路径
     /// </summary>
    private static string videoExtensions = "*.mp4|*.avi|*.mov|*.m4v";
    /// <summary>
    /// 用于获取表格路径筛选的数组
    /// </summary>
    private static string ExcelExtensions = "*.xlsx|*.xls|*.xlsm";
    /// <summary>
    /// 用于获取图片路径筛选的数组
    /// </summary>
    private static string pictureExtensions = "*.jpg|*.png|*.bmp";
    /// <summary>
    /// 寻找子物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetChild<T>(this GameObject obj, string name) where T : Component
    {
        var father = obj.GetComponentsInChildren<Transform>(true);
        GameObject Child = null;

        foreach (Transform child in father)
        {
            if (string.Equals(child.name, name))
            {
                Child = child.gameObject;
            }
        }

        if (Child == null)
        {
            return null;
        }

        T t = Child.GetComponent<T>();

        return t;
    }

    /// <summary>
    /// 寻找物体下第一个符合名称的子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name">子物体名字</param>
    /// <returns></returns>
    public static GameObject GetChild(this GameObject obj, string name)
    {
        Transform[] trans = obj.transform.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in trans)
        {
            if (string.Equals(child.name, name))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// 寻找物体下第一个符合名称的子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name">子物体名字</param>
    /// <returns></returns>
    public static Transform GetChild(this Transform obj, string name)
    {
        Transform[] trans = obj.transform.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in trans)
        {
            if (string.Equals(child.name, name))
            {
                return child;
            }
        }

        return null;
    }

    /// <summary>
    /// 获得字符串2 * index - 2起的2位值，仅限数字,起始值为1
    /// </summary>
    /// <param name="str"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    public static int GetIndex(this string str, int index)
    {
        string s = "";
        int value = 0;

        index = index == 0 ? 1 : index;

        if (index < str.Length)
        {
            s = str.Substring(2 * index - 2, 2);
        }

        int.TryParse(s, out value);

        return value;
    }

    /// <summary>
    /// 空格分割字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    public static string SplitIndex(this string str, int index)
    {
        string s = "";
        string[] splitStr = str.Split(' ');

        for (int i = 0; i < splitStr.Length; i++)
        {
            if (index == i)
            {
                s = splitStr[i];
            }
        }

        return s;
    }

    //从startIndex开始的number位往前挪动index位，起始值为0
    public static string[] MoveStrings(this string[] strs, int startIndex, int number, int index)
    {
        string[] strings = new string[strs.Length];

        Array.Copy(strs, strings, startIndex - index);
        Array.Copy(strs, startIndex, strings, startIndex - index, number);
        Array.Copy(strs, startIndex - index, strings, startIndex - index + number, index);
        Array.Copy(strs, startIndex + number, strings, startIndex + number, strs.Length - startIndex - number);

        return strings;
    }

    //array[]转换成string
    public static string ConvertString(this Array arr)
    {
        string s = "";

        foreach (var v in arr)
        {
            s += v.ToString() + " ";
        }

        return s;
    }

    /// <summary>
    /// 设置anchor
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="anchor">四维向量（MinX,MinY,MaxX,MaxY）</param>
    public static void SetAnchor(this Transform transform, Vector4 anchor)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(anchor.x, anchor.y);
        rect.anchorMax = new Vector2(anchor.z, anchor.w);
    }

    /// <summary>
    /// 设置anchor
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="anchor">MinX,MinY,MaxX,MaxY</param>
    public static void SetAnchor(this Transform transform, float xMin, float yMin, float xMax, float yMax)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(xMin, yMin);
        rect.anchorMax = new Vector2(xMax, yMax);
    }

    /// <summary>
    /// 设置offset
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="offset"></param>
    public static void SetOffset(this Transform transform, Vector4 offset)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(offset.x, offset.y);
        rect.offsetMax = new Vector2(offset.z, offset.w);
    }

    /// <summary>
    /// string转int
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static int ToInt(this string s)
    {
        int i = 0;
        if (!int.TryParse(s, out i))
        {
            Debug.Log("转换Int失败");
            i = int.MaxValue;
        }
        return i;
    }

    //没试过
    /// <summary>
    /// 生成随机数组
    /// </summary>
    /// <param name="a"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int[] RandomArray(this int[] a, int min, int max)
    {
        int[] arr = new int[max - min + 1];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = i + min;
        }

        //数组随机排列
        ///    Linq技术中的OrderBy方法排序，排序的依据是与原序列元素对应生成一个键值，Guid.NewGuid()方法返回的一个随机32位不重复的
        ///    Guid字符串。因为每次生成的Guid字符串大小不同，且大小与生成先后顺序无关，所以按这个兼职排序后的整数序列的顺序是随机的。
        arr = arr.OrderBy(c => Guid.NewGuid()).ToArray();

        a = arr;

        return a;
    }
    //string[]转换成string
    public static string GetString(this string[] strs)
    {
        string s = "";
        foreach (string ss in strs)
        {
            s += ss + " ";
        }
        return s;
    }
    
    //分割如1_2之类的字符串
    public static string GetFileName(this string str, int index)
    {
        string s = "";
        string[] strs = str.Split('.');
        
        if (index < strs.Length)
            s = strs[0];

        return s;
    }

    /// <summary>
    /// 向destTexture上绘制texture，相对位置为rect
    /// </summary>
    /// <param name="render"></param>
    /// <param name="texture"></param>
    /// <param name="rect">x:xMin, y:yMin, z:xMax, w:yMax</param>
    public static Texture2D DrawTexture(this Texture2D texture, Vector4Float pos, Vector4Float rect)
    {
        RenderTexture render = new RenderTexture(texture.width, texture.height, 24, RenderTextureFormat.ARGB32);

        Graphics.SetRenderTarget(render);

        GL.PushMatrix();
        GL.LoadOrtho();

        Material material = new Material(Shader.Find("UI/Default"));
        material.SetTexture("_MainTex", texture);
        material.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.TexCoord2(pos.xMin, pos.yMin);
        GL.Vertex3(rect.xMin, rect.yMin, 0);
        GL.TexCoord2(pos.xMax, pos.yMin);
        GL.Vertex3(rect.xMax, rect.yMin, 0);
        GL.TexCoord2(pos.xMax, pos.yMax);
        GL.Vertex3(rect.xMax, rect.yMax, 0);
        GL.TexCoord2(pos.xMin, pos.yMax);
        GL.Vertex3(rect.xMin, rect.yMax, 0);

        GL.End();
        GL.PopMatrix();

        return render.ToTexture2D();
    }

    /// <summary>
    /// 向destTexture上绘制texture，相对位置为rect
    /// </summary>
    /// <param name="destTexture"></param>
    /// <param name="texture"></param>
    /// <param name="rect">x:xMin, y:yMin, z:xMax, w:yMax</param>
    public static void DrawTexture(RenderTexture destTexture, Texture2D texture, Vector4Float rect)
    {
        Graphics.SetRenderTarget(destTexture);

        GL.PushMatrix();
        GL.LoadOrtho();

        Material material = new Material(Shader.Find("UI/Default"));
        material.SetTexture("_MainTex", texture);
        material.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(rect.xMin, rect.yMin, 0);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(rect.xMax, rect.yMin, 0);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(rect.xMax, rect.yMax, 0);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(rect.xMin, rect.yMax, 0);

        GL.End();
        GL.PopMatrix();
    }

    /// <summary>
    /// 合成图片
    /// </summary>
    /// <param name="bg">background 背景</param>
    /// <param name="fg">foreground 前景</param>
    /// <returns></returns>
    public static Texture2D OnCombineTexture(this Texture2D bg, Texture2D fg)
    {
        if (bg == null) return fg;
        if (fg == null) return bg;

        RenderTexture render = new RenderTexture(bg.width, bg.height, 24, RenderTextureFormat.ARGB32);

        Vector4Float v4 = new Vector4Float()
        {
            xMin = 0,
            yMin = 0,
            xMax = 1,
            yMax = 1,
        };

        DrawTexture(render, bg, v4);

        //定位
        //float width = TransUploadPanel.GetComponent<RectTransform>().rect.width;
        //float height = TransUploadPanel.GetComponent<RectTransform>().rect.height;
        //float widthDraw = ImgShowDraw.GetComponent<RectTransform>().rect.width;
        //float heightDraw = ImgShowDraw.GetComponent<RectTransform>().rect.height;

        //float x = (TransUploadPanel.position.x - width / 2 - (ImgShowDraw.transform.position.x - widthDraw / 2)) / ImgShowDraw.GetComponent<RectTransform>().rect.width;
        //float y = (TransUploadPanel.position.y - height / 2 - (ImgShowDraw.transform.position.y - heightDraw / 2)) / ImgShowDraw.GetComponent<RectTransform>().rect.height;
        //float z = (TransUploadPanel.position.x + width / 2 - (ImgShowDraw.transform.position.x - widthDraw / 2)) / ImgShowDraw.GetComponent<RectTransform>().rect.width;
        //float w = (TransUploadPanel.position.y + height / 2 - (ImgShowDraw.transform.position.y - heightDraw / 2)) / ImgShowDraw.GetComponent<RectTransform>().rect.height;

        //v4 = new Vector4Int()
        //{
        //    xMin = x,
        //    yMin = y,
        //    xMax = z,
        //    yMax = w,
        //};

        DrawTexture(render, fg, v4);

        fg = render.ToTexture2D();

        return fg;
    }

    /// <summary>
    /// RenderTexture转Texture2D
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    public static Texture2D ToTexture2D(this RenderTexture render)
    {
        if (render == null) return null;

        int width = render.width;
        int height = render.height;
        Texture2D t = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture.active = render;
        t.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        t.Apply();

        return t;
    }

    /// <summary>
    /// Texture转换成Texture2D
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Texture2D ToTexture2D(this Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    #region 图片处理

    /// <summary>
    /// 旋转图片180°
    /// </summary>
    /// <returns></returns>
    public static Texture2D RotateTexture(this Texture2D texture)
    {
        Color32[] original = texture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = texture.width;
        int h = texture.height;

        int iRotated, iOriginal;

        for (int i = 0; i < original.Length; i++)
        {
            iRotated = i;
            iOriginal = original.Length - i - 1;
            rotated[iRotated] = original[iOriginal];
        }

        Texture2D rotatedTexture = new Texture2D(w, h);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
    /// <summary>
    /// 获取指定路径下的视频路径数组
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetVideoFiles(string path)
    {
        List<string> temp = new List<string>();
        try
        {
            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + path);

            FileInfo[] files;

            string[] ImgType = videoExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }

        }
        catch
        {
            Debug.LogError("Videos文件夹不存在,请检查");
        }
        return FileSort(temp).ToArray();

    }
    /// <summary>
    /// 获取当前机器的IPv4的IP值
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIPv4()
    {
        string ipAddress = "";
        try
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("IP 获取失败");
        }
        return ipAddress;
    }
    /// <summary>
    /// 获取指定路径下的excel文件数组
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetExcelFiles(string path)
    {
        List<string> temp = new List<string>();
        try
        {

            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + path);

            FileInfo[] files;

            string[] ImgType = ExcelExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }
        }
        catch
        {
            Debug.LogError("excel文件夹不存在,请检查");
        }
        return FileSort(temp).ToArray();

    }

    /// <summary>
    /// 获取外部指定路径下的图片文件名称数组
    /// </summary>
    /// <param name="FoldPath"></param>
    /// <returns></returns>
    public static string[] GetTexturePath(string FoldPath)
    {

        List<string> temp = new List<string>();
        try
        {
            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + FoldPath);

            FileInfo[] files;

            string[] ImgType = pictureExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }
        }
        catch (Exception)
        {

            Debug.LogError("Photos文件夹不存在,请检查");
        }

        return FileSort(temp).ToArray();

    }

    /// <summary>
    /// 异步加载路径下的图片并且直接把他加载在RawImage上
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    public static async Task LoadByFSAsync(string path, RawImage image)
    {
        byte[] result;

        using (FileStream SourceStream = File.Open(path, FileMode.Open))
        {
            result = new byte[SourceStream.Length];
            await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
        }

        Texture2D tx = new Texture2D(2, 1);

        tx.LoadImage(result);

        float widthRatio = tx.width / image.rectTransform.sizeDelta.x;
        float heightRatio = tx.height / image.rectTransform.sizeDelta.y;

        if (widthRatio / heightRatio > 1.1)
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / widthRatio, tx.height / widthRatio);
        }
        else
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / heightRatio, tx.height / heightRatio);
        }

        image.texture = tx;
    }

    /// <summary>
    /// 同步加载路径下的图片并且直接把他加载在RawImage上
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    public static void LoadByFSSync(string path, RawImage image)
    {
        byte[] result;

        using (FileStream SourceStream = File.Open(path, FileMode.Open))
        {
            result = new byte[SourceStream.Length];
            SourceStream.Read(result, 0, (int)SourceStream.Length);
        }

        Texture2D tx = new Texture2D(2, 1);

        tx.LoadImage(result);

        float widthRatio = tx.width / image.rectTransform.sizeDelta.x;
        float heightRatio = tx.height / image.rectTransform.sizeDelta.y;

        if (widthRatio / heightRatio > 1.1)
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / widthRatio, tx.height / widthRatio);
        }
        else
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / heightRatio, tx.height / heightRatio);
        }

        image.texture = tx;
    }

    /// <summary>
    /// 加载外部图片文件转成Texture2D
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public static Texture2D LoadPicTexture(string Path)
    {
        if (File.Exists(Path))
        {
            FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;
            //创建Texture
            int width = 1080;
            int height = 1920;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(bytes);

            return texture;
        }

        return null;
    }
    /// <summary>
    /// 将所有路径下的外部图片文件转成Texture2D
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public static List<Texture2D> LoadPicTextures(string[] Path)
    {
        List<Texture2D> temp = new List<Texture2D>();
        foreach (var item in Path)
        {
            Texture2D tex = LoadPicTexture(item);
            temp.Add(tex);
        }
        return temp;
    }
    /// <summary>
    /// 将图片转换成精灵图
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public  static Sprite TextureToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    /// <summary>
    /// 将字符串根据里面的数字进行排序
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<string> FileSort(List<string> path)
    {
        string temp;
        for (int i = 0; i < path.Count - 1; i++)
        {
            for (int j = 0; j < path.Count - 1 - i; j++)
            {
                if (CustomSort(path[j], path[j + 1]))
                {
                    temp = path[j];
                    path[j] = path[j + 1];
                    path[j + 1] = temp;
                }
            }
        }
        return path;
    }

    /// <summary>
    /// 从字符串中获取数字并且判断大小
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static bool CustomSort(string str1, string str2)
    {
        int result1 = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(str1, @"[^0-9]+", ""));
        int result2 = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(str2, @"[^0-9]+", ""));
        if (result1 > result2)
            return true;
        else
            return false;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public  class ReadingExternalData
{
    /// <summary>
    /// ͳһ�첽����
    /// </summary>
    public static async void UniteFSAsync(string path)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(path);

        FileInfo[] files;

        string[] ImgType = "*.jpg|*.png|*.bmp".Split('|');

        Debug.Log(dirInfo);

        for (int j = 0; j < ImgType.Length; j++)
        {
            files = dirInfo.GetFiles(ImgType[j]);

            for (int i = 0; i < files.Length; i++)
            {


                Debug.Log(files[i].FullName);//�ļ�ȫ����

                

                //await LoadByFSAsync(files[i].FullName, raw);//����ͼƬ�ķ���
            }
        }
    }
    /// <summary>
    /// �첽����ͼƬ
    /// </summary>
    /// <param name="path">·��</param>
    /// <param name="image">Image����</param>
    /// <returns></returns>
    private static async Task LoadByFSAsync(string path, RawImage image)
    {
        byte[] result;

        using (FileStream SourceStream = File.Open(path, FileMode.Open))
        {
            result = new byte[SourceStream.Length];
            await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
        }

        Texture2D tx = new Texture2D(2, 1);

        tx.LoadImage(result);

        //�ȱ�������ͼƬ��С
        //float widthRatio = tx.width / img_width;
        //float heightRatio = tx.height / img_height;

        //if (widthRatio / heightRatio > 1.1)
        //{
        //    image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / widthRatio, tx.height / widthRatio);
        //}
        //else
        //{
        //    image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / heightRatio, tx.height / heightRatio);
        //}

        image.texture = tx;
    }
}

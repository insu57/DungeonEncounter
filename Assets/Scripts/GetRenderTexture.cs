using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class GetRenderTexture : MonoBehaviour
{
    [SerializeField] private Camera cam;
    //[SerializeField] private RenderTexture renderTexture;
    [SerializeField] private GameObject targetPrefab;
    private Sprite GetItemRenderTexture(GameObject itemPrefab)
    {
        Debug.Log("GetRenderTexture");
        RenderTexture renderTexture = new RenderTexture(256, 256, 24);
        cam.targetTexture = renderTexture;
        //cam.transform.position = itemPrefab.transform.position + Vector3.back * 1f + Vector3.up * -1f;
        //cam.transform.LookAt(itemPrefab.transform);

        RenderTexture.active = renderTexture;
        cam.Render();
        
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        
       
        return Sprite.Create(texture, new Rect(0, 0, renderTexture.width, renderTexture.height), Vector2.one * 0.5f);
    }

    private void SaveSprite(Sprite sprite, string path)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Sprite icon = GetItemRenderTexture(targetPrefab);
            SaveSprite(icon, Application.dataPath + "/Resources/"+targetPrefab.name+"_icon.png");
        }
    }
}

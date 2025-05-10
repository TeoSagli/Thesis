using GLTFast;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
public abstract class Puzzle : MonoBehaviour
{
    private PuzzleData puzzleData;

    private Vector3 bounds = Vector3.zero;

    public PuzzleData PuzzleData { get => puzzleData; set => puzzleData = value; }
    public Vector3 Bounds { get => bounds; set => bounds = value; }

    protected float CalculateOffsetForSocketCenter(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }
    protected Sprite LoadFromPath(string path, string name)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new(2, 2);
            tex.LoadImage(fileData);
            Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            s.name = name;
            return s;
        }
        else
        {
            Debug.Log("File not found!");
            return null;
        }
    }
    protected async Task<GameObject> LoadGLBFromBytes(string path, string name)
    {
        GltfImport gltf = new ();
        bool success = await gltf.Load(File.ReadAllBytes(path));

        if (success)
        {
            GameObject model = new (name);
            await gltf.InstantiateMainSceneAsync(model.transform); 
            foreach( var gameObj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if(gameObj.name == name && !gameObj.TryGetComponent<MeshFilter>(out _))
                {
                    Destroy(gameObj);
                }
            }
       
            model.SetActive(false);
            return model;
        }
        else
        {
            Debug.LogError("Failed to load GLTF from bytes.");
            return null;
        }
    }
    protected abstract void CalculateBounds();
}
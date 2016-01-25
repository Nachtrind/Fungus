using UnityEngine;

public class SlimeRenderer : MonoBehaviour
{
    int framesSkipped;
    public Material groundMaterial;

#if UNITY_EDITOR
    int prevSampleSize = 2;
#endif

    [SerializeField] Camera projectorCam;

    StandardGameSettings sgs;
    RenderTexture slimeTex;

    [Range(1, 4)] [Header("multiples of 128")] public int textureSize = 2;

    Material whiteMaterial;

    Texture2D slimeTexDirect;

    public bool useDirectMethod = false;

    void Start()
    {
        sgs = StandardGameSettings.Get;
        whiteMaterial = new Material(Shader.Find("Unlit/Color"));
        whiteMaterial.color = Color.green;
        CreateRenderTex();
    }

    void CreateRenderTex()
    {
        if (slimeTex != null)
        {
            slimeTex.DiscardContents();
            slimeTex.Release();
        }
        var sampleSize = 128*textureSize;
        slimeTex = new RenderTexture(sampleSize, sampleSize, 0);
    }

    void CreateRenderTexDirect()
    {
        var gg = AstarPath.active.astarData.gridGraph;
        slimeTexDirect = new Texture2D(gg.Width, gg.Depth);
    }

    void OnApplicationQuit()
    {
        groundMaterial.mainTexture = null;
        if (slimeTex)
        {
            slimeTex.DiscardContents();
            slimeTex.Release();
        }
    }


    bool previousState = false;
    void OnPreRender()
    {
        if (useDirectMethod != previousState)
        {
            previousState = useDirectMethod;
            if (useDirectMethod)
            {
                groundMaterial.shader = Shader.Find("Fungus/GroundShaderDirect");
            }
            else
            {
                groundMaterial.shader = Shader.Find("Fungus/GroundShader");
            }
        }
#if UNITY_EDITOR
        if (prevSampleSize != textureSize)
        {
            CreateRenderTex();
            prevSampleSize = textureSize;
        }
#endif
        if (framesSkipped < sgs.renderFrameSkip)
        {
            framesSkipped++;
            return;
        }
        framesSkipped = 0;
        if (useDirectMethod)
        {
            RenderSlimeDirect();
        }
        else
        {
            RenderSlimeFromCam();
        }
    }

    Color32 red = new Color32(255, 0, 0, 255);
    Color32 green = new Color32(0, 255, 0, 255);
    void RenderSlimeDirect()
    {
        if (slimeTexDirect == null)
        {
            CreateRenderTexDirect();
        }
        var gg = AstarPath.active.astarData.gridGraph;
        var cols = slimeTexDirect.GetPixels32();
        for (var i = 0; i < gg.nodes.Length; i++)
        {
            if (gg.nodes[i].Tag != SlimeHandler.slimeTag || !gg.nodes[i].Walkable)
            {
                cols[i] = red;
                continue;
            }
            cols[i] = green;
        }
        slimeTexDirect.SetPixels32(cols);
        slimeTexDirect.Apply();
        groundMaterial.mainTexture = slimeTexDirect;
    }

    void RenderSlimeFromCam()
    {
        if (slimeTex == null)
        {
            CreateRenderTex();
        }
        slimeTex.DiscardContents(true, true);
        var previousRT = RenderTexture.active;
        RenderTexture.active = slimeTex;
        var gg = AstarPath.active.astarData.gridGraph;
        whiteMaterial.SetPass(0);
        GL.Clear(true, true, Color.red);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.LoadProjectionMatrix(projectorCam.projectionMatrix);
        GL.MultMatrix(projectorCam.worldToCameraMatrix);
        GL.Begin(GL.QUADS);
        GL.Color(Color.green);
        for (var i = 0; i < gg.nodes.Length; i++)
        {
            if (gg.nodes[i].Tag != SlimeHandler.slimeTag || !gg.nodes[i].Walkable)
            {
                continue;
            }
            var bottomLeft = (Vector3) gg.nodes[i].position;
            bottomLeft.x -= gg.nodeSize*0.5f;
            bottomLeft.z -= gg.nodeSize*0.5f;
            GL.Vertex(bottomLeft);
            GL.Vertex3(bottomLeft.x, 0, bottomLeft.z + gg.nodeSize);
            GL.Vertex3(bottomLeft.x + gg.nodeSize, 0, bottomLeft.z + gg.nodeSize);
            GL.Vertex3(bottomLeft.x + gg.nodeSize, 0, bottomLeft.z);
        }
        GL.End();
        GL.PopMatrix();
        RenderTexture.active = previousRT;
        groundMaterial.SetMatrix("_projMatrix", projectorCam.projectionMatrix*projectorCam.worldToCameraMatrix);
        groundMaterial.mainTexture = slimeTex;
    }
}
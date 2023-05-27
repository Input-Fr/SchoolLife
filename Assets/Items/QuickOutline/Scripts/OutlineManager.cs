using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.QuickOutline.Scripts
{
  [DisallowMultipleComponent]

  public class OutlineManager : MonoBehaviour 
  {
    private static HashSet<Mesh> _registeredMeshes = new HashSet<Mesh>();

    public enum Mode 
    {
      OutlineAll,
      OutlineVisible,
      OutlineHidden,
      OutlineAndSilhouette,
      SilhouetteOnly
    }

    public Mode OutlineMode 
    {
      get { return outlineMode; }
      set 
      {
        outlineMode = value;
        _needsUpdate = true;
      }
    }

    public Color OutlineColor 
    {
      get { return outlineColor; }
      set 
      {
        outlineColor = value;
        _needsUpdate = true;
      }
    }

    public float OutlineWidth 
    {
      get { return outlineWidth; }
      set 
      {
        outlineWidth = value;
        _needsUpdate = true;
      }
    }

    [Serializable]
    private class ListVector3 
    {
      public List<Vector3> data;
    }

    [SerializeField]
    private Mode outlineMode;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 2f;

    [Header("Optional")]

    [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
                             + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    private bool precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] _renderers;
    private Material _outlineMaskMaterial;
    private Material _outlineFillMaterial;

    private bool _needsUpdate;

    void Awake() 
    {
      _renderers = GetComponentsInChildren<Renderer>();

      _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
      _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

      _outlineMaskMaterial.name = "OutlineMask (Instance)";
      _outlineFillMaterial.name = "OutlineFill (Instance)";

      LoadSmoothNormals();

      _needsUpdate = true;
    }

    void OnEnable() 
    {
      foreach (Renderer renderer in _renderers) 
      {
        List<Material> materials = renderer.sharedMaterials.ToList();

        materials.Add(_outlineMaskMaterial);
        materials.Add(_outlineFillMaterial);

        renderer.materials = materials.ToArray();
      }
    }

    void OnValidate() 
    {
      _needsUpdate = true;

      if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count) 
      {
        bakeKeys.Clear();
        bakeValues.Clear();
      }

      if (precomputeOutline && bakeKeys.Count == 0) 
      {
        Bake();
      }
    }

    void Update() 
    {
      if (_needsUpdate) 
      {
        _needsUpdate = false;

        UpdateMaterialProperties();
      }
    }

    void OnDisable() 
    {
      foreach (Renderer renderer in _renderers) 
      {
        List<Material> materials = renderer.sharedMaterials.ToList();

        materials.Remove(_outlineMaskMaterial);
        materials.Remove(_outlineFillMaterial);

        renderer.materials = materials.ToArray();
      }
    }

    void OnDestroy() 
    {
      Destroy(_outlineMaskMaterial);
      Destroy(_outlineFillMaterial);
    }

    void Bake() 
    {
      HashSet<Mesh> bakedMeshes = new HashSet<Mesh>();

      foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>()) 
      {
        if (!bakedMeshes.Add(meshFilter.sharedMesh)) 
        {
          continue;
        }

        List<Vector3> smoothNormals = SmoothNormals(meshFilter.sharedMesh);

        bakeKeys.Add(meshFilter.sharedMesh);
        bakeValues.Add(new ListVector3() { data = smoothNormals });
      }
    }

    void LoadSmoothNormals() 
    {
      foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>()) 
      {
        if (!_registeredMeshes.Add(meshFilter.sharedMesh)) 
        {
          continue;
        }

        int index = bakeKeys.IndexOf(meshFilter.sharedMesh);
        List<Vector3> smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

        meshFilter.sharedMesh.SetUVs(3, smoothNormals);

        Renderer renderer = meshFilter.GetComponent<Renderer>();

        if (renderer != null) 
        {
          CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
        }
      }

      foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>()) 
      {
        if (!_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh)) 
        {
          continue;
        }

        skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
        CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
      }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
      IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups = mesh.vertices
        .Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

      var smoothNormals = new List<Vector3>(mesh.normals);

      foreach (IGrouping<Vector3,KeyValuePair<Vector3,int>> group in groups) 
      {
        if (group.Count() == 1) 
        {
          continue;
        }

        Vector3 smoothNormal = Vector3.zero;

        foreach (KeyValuePair<Vector3,int> pair in group) 
        {
          smoothNormal += smoothNormals[pair.Value];
        }

        smoothNormal.Normalize();

        foreach (KeyValuePair<Vector3,int> pair in group) 
        {
          smoothNormals[pair.Value] = smoothNormal;
        }
      }

      return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials) 
    { 
      if (mesh.subMeshCount == 1) 
      {
        return;
      }

      if (mesh.subMeshCount > materials.Length) 
      {
        return;
      }

      mesh.subMeshCount++;
      mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    void UpdateMaterialProperties() 
    {
      _outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

      switch (outlineMode) 
      {
        case Mode.OutlineAll:
          _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
          _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
          _outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
          break;

        case Mode.OutlineVisible:
          _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
          _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
          _outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
          break;

        case Mode.OutlineHidden:
          _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
          _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
          _outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
          break;

        case Mode.OutlineAndSilhouette:
          _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
          _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
          _outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
          break;

        case Mode.SilhouetteOnly:
          _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
          _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
          _outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
          break;
      }
    }
  }
}

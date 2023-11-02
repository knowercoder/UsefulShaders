// MIT License

// Copyright (c) 2023 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MyLitCustomInspector : ShaderGUI {

    public enum SurfaceType {
        Opaque, TransparentBlend, TransparentCutout
    }
    
    public enum FaceRenderingMode {
        FrontOnly, NoCulling, DoubleSided
    }
    
    public enum BlendType {
        Alpha, Premultiplied, Additive, Multiply
    }
    
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader) {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if(newShader.name == "NedMakesGames/MyLit") {
            UpdateSurfaceType(material);
        }
    }
    
#if UNITY_2022_1_OR_NEWER
    public override void ValidateMaterial(Material material) {
        base.ValidateMaterial(material);
        UpdateSurfaceType(material);
    }
#endif

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {

        Material material = materialEditor.target as Material;
        var surfaceProp = BaseShaderGUI.FindProperty("_SurfaceType", properties, true);
        var blendProp = BaseShaderGUI.FindProperty("_BlendType", properties, true);
        var faceProp = BaseShaderGUI.FindProperty("_FaceRenderingMode", properties, true);
        
        EditorGUI.BeginChangeCheck();
        
#if UNITY_2022_1_OR_NEWER
        MaterialEditor.BeginProperty(surfaceProp);
#endif
        surfaceProp.floatValue = (int)(SurfaceType)EditorGUILayout.EnumPopup("Surface type", (SurfaceType)surfaceProp.floatValue);
#if UNITY_2022_1_OR_NEWER
        MaterialEditor.EndProperty();
#endif

#if UNITY_2022_1_OR_NEWER
        MaterialEditor.BeginProperty(blendProp);
#endif
        blendProp.floatValue = (int)(BlendType)EditorGUILayout.EnumPopup("Blend type", (BlendType)blendProp.floatValue);
#if UNITY_2022_1_OR_NEWER
        MaterialEditor.EndProperty();
#endif
        
#if UNITY_2022_1_OR_NEWER
        MaterialEditor.BeginProperty(faceProp);
#endif
        faceProp.floatValue = (int)(FaceRenderingMode)EditorGUILayout.EnumPopup("Face rendering mode", (FaceRenderingMode)faceProp.floatValue);
#if UNITY_2022_1_OR_NEWER
        MaterialEditor.EndProperty();
#endif
        
        if(EditorGUI.EndChangeCheck()) {
            UpdateSurfaceType(material);
        }

        base.OnGUI(materialEditor, properties);
    }

    private void UpdateSurfaceType(Material material) {
        SurfaceType surface = (SurfaceType)material.GetFloat("_SurfaceType");
        switch(surface) {
        case SurfaceType.Opaque:
            material.renderQueue = (int)RenderQueue.Geometry;
            material.SetOverrideTag("RenderType", "Opaque");
            break;
        case SurfaceType.TransparentCutout:
            material.renderQueue = (int)RenderQueue.AlphaTest;
            material.SetOverrideTag("RenderType", "TransparentCutout");
            break;
        case SurfaceType.TransparentBlend:
            material.renderQueue = (int)RenderQueue.Transparent;
            material.SetOverrideTag("RenderType", "Transparent");
            break;
        }

        BlendType blend = (BlendType)material.GetFloat("_BlendType");
        switch(surface) {
        case SurfaceType.Opaque:
        case SurfaceType.TransparentCutout:
            material.SetInt("_SourceBlend", (int)BlendMode.One);
            material.SetInt("_DestBlend", (int)BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            break;
        case SurfaceType.TransparentBlend:
            switch(blend) {
            case BlendType.Alpha:
                material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                break;
            case BlendType.Premultiplied:
                material.SetInt("_SourceBlend", (int)BlendMode.One);
                material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                break;
            case BlendType.Additive:
                material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DestBlend", (int)BlendMode.One);
                break;
            case BlendType.Multiply:
                material.SetInt("_SourceBlend", (int)BlendMode.Zero);
                material.SetInt("_DestBlend", (int)BlendMode.SrcColor);
                break;
            }
            material.SetInt("_ZWrite", 0);
            break;
        }
        if(surface == SurfaceType.TransparentBlend && blend == BlendType.Premultiplied) {
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        } else {
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        material.SetShaderPassEnabled("ShadowCaster", surface != SurfaceType.TransparentBlend);
        
        if(surface == SurfaceType.TransparentCutout) {
            material.EnableKeyword("_ALPHA_CUTOUT");
        } else {
            material.DisableKeyword("_ALPHA_CUTOUT");
        }
        
        FaceRenderingMode faceRenderingMode = (FaceRenderingMode)material.GetFloat("_FaceRenderingMode");
        if(faceRenderingMode == FaceRenderingMode.FrontOnly) {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
        } else {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }

        if(faceRenderingMode == FaceRenderingMode.DoubleSided) {
            material.EnableKeyword("_DOUBLE_SIDED_NORMALS");
        } else {
            material.DisableKeyword("_DOUBLE_SIDED_NORMALS");
        }
    }
}
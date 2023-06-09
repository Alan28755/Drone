using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplaceShader : ScriptableObject
{
    [MenuItem("Custom/Replace Shader")]
    static void Replace()
    {
        // 获取你想要替换的Shader
        Shader newShader = Shader.Find("Infrared");

        // 获取当前选中的所有对象
        Object[] objects = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

        foreach (GameObject obj in objects)
        {
            // 获取对象的Renderer组件
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer != null)
            {
                // 获取对象的所有材质
                Material[] materials = renderer.sharedMaterials;

                for (int i = 0; i < materials.Length; i++)
                {
                    // 替换Shader
                    materials[i].shader = newShader;
                }
            }
        }
    }
}

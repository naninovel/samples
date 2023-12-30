using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Naninovel
{
    /// <summary>
    /// Performs drawing of <see cref="SpineCharacter"/> to a <see cref="TransitionalRenderer"/>.
    /// </summary>
    public class SpineDrawer
    {
        private readonly Transform transform;
        private readonly MeshRenderer meshRenderer;
        private readonly MeshFilter meshFilter;
        private readonly RenderCanvas renderCanvas;
        private readonly List<Material> materials = new List<Material>();
        private readonly CommandBuffer commandBuffer = new CommandBuffer();
        private readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        private RenderTexture renderTexture;

        public SpineDrawer (SpineController controller)
        {
            meshRenderer = controller.MeshRenderer;
            meshRenderer.forceRenderingOff = true;
            renderCanvas = controller.RenderCanvas;
            meshFilter = controller.MeshFilter;
            transform = controller.transform;
            commandBuffer.name = $"Naninovel-DrawSpine-{transform.name}";

            // Align underlying game object with the render texture position.
            transform.localPosition += new Vector3(0, renderCanvas.Size.y / 2);
        }

        public void Dispose ()
        {
            if (renderTexture)
                RenderTexture.ReleaseTemporary(renderTexture);
        }

        public void DrawTo (TransitionalRenderer renderer, float pixelsPerUnit)
        {
            if (!meshRenderer.enabled) return;

            var drawDimensions = (Vector3)renderCanvas.Size * pixelsPerUnit;
            var drawPosition = transform.position + (Vector3)renderCanvas.Offset;
            var orthoMin = -drawDimensions / 2f + drawPosition * pixelsPerUnit;
            var orthoMax = drawDimensions / 2f + drawPosition * pixelsPerUnit;
            var orthoMatrix = Matrix4x4.Ortho(orthoMin.x, orthoMax.x, orthoMin.y, orthoMax.y, float.MinValue, float.MaxValue);
            var rotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(renderer.transform.localRotation));

            PrepareRenderTexture(drawDimensions, renderer);
            PrepareCommandBuffer(orthoMatrix);
            Draw(rotationMatrix, pixelsPerUnit);
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        private void PrepareRenderTexture (Vector2 drawDimensions, TransitionalRenderer renderer)
        {
            var requiredSize = new Vector2Int(Mathf.RoundToInt(drawDimensions.x), Mathf.RoundToInt(drawDimensions.y));
            if (CurrentTextureValid()) return;

            if (renderTexture)
                RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = RenderTexture.GetTemporary(requiredSize.x, requiredSize.y);
            renderer.MainTexture = renderTexture;

            bool CurrentTextureValid () => renderTexture && renderTexture.width == requiredSize.x && renderTexture.height == requiredSize.y;
        }

        private void PrepareCommandBuffer (Matrix4x4 orthoMatrix)
        {
            commandBuffer.Clear();
            commandBuffer.SetRenderTarget(renderTexture);
            commandBuffer.ClearRenderTarget(true, true, Color.clear);
            commandBuffer.SetProjectionMatrix(orthoMatrix);
        }

        private void Draw (Matrix4x4 rotationMatrix, float pixelsPerUnit)
        {
            var parent = transform.parent;
            var drawPosition = parent.TransformPoint(rotationMatrix // Compensate actor (parent game object) rotation.
                .MultiplyPoint3x4(parent.InverseTransformPoint(transform.position)));
            var drawTransform = Matrix4x4.TRS(drawPosition * pixelsPerUnit, parent.localRotation, parent.lossyScale * pixelsPerUnit);
            meshRenderer.GetPropertyBlock(propertyBlock);
            meshRenderer.GetSharedMaterials(materials);
            for (int i = 0; i < materials.Count; i++)
                commandBuffer.DrawMesh(meshFilter.sharedMesh, drawTransform, materials[i], i + meshRenderer.subMeshStartIndex, -1, propertyBlock);
        }
    }
}

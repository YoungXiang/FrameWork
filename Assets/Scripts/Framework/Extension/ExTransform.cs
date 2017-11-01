using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class ExTransform
    {
        public static Transform FindRecursively(this Transform trans, string name)
        {
            if (name.Equals(trans.name))
            {
                return trans;
            }

            Transform ret = null;
            if (trans.childCount > 0)
            {
                foreach (Transform child in trans)
                {
                    ret = FindRecursively(child, name);
                    if (ret != null) return ret;
                }
            }

            return ret;
        }

        public static void ResetTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        #region Transform Point Vector3
        public static Vector3 TransformPoint_Root(this Transform trans, Vector3 worldPos)
        {
            return trans.root.TransformPoint(worldPos);
        }
        public static Vector3 InverseTransformPoint_Root(this Transform trans, Vector3 localPos)
        {
            return trans.root.InverseTransformPoint(localPos);
        }
        public static Vector3 TransformDirection_Root(this Transform trans, Vector3 direction)
        {
            return trans.root.TransformDirection(direction);
        }
        public static Vector3 InverseTransformDirection_Root(this Transform trans, Vector3 direction)
        {
            return trans.root.InverseTransformDirection(direction);
        }

        #endregion

        public static Transform FindParent(this Transform trans, string parentName)
        {
            if (trans.parent == null) return null;

            if (trans.parent.name.Equals(parentName))
            {
                return trans.parent;
            }

            return trans.parent.FindParent(parentName);
        }

        public delegate bool ConditionDelegateEvent(Object A);
        public static Transform FindParent(this Transform trans, ConditionDelegateEvent del)
        {
            if (trans.parent == null) return null;
            // if Matches!
            if (del(trans.parent))
            {
                return trans.parent;
            }

            return trans.parent.FindParent(del);
        }

        public static void DestroyAllChildren(this Transform trans)
        {
            int childCount = trans.childCount;
            if (childCount <= 0)
            {
                return;
            }

            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(trans.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Reset transform position, rotation, scale; doesn't reset rectTransform's anchors.
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetTransformLocal(this Transform trans)
        {
            trans.localPosition = Vector3.zero; trans.localRotation = Quaternion.identity; trans.localScale = Vector3.one;
        }

        public static void CopyTransform(this Transform dst, Transform src)
        {
            dst.position = src.position; dst.rotation = src.rotation; dst.localScale = src.localScale;
        }

        public static void CopyLocalTransform(this Transform dst, Transform src)
        {
            dst.localPosition = src.localPosition; dst.localRotation = src.localRotation; dst.localScale = src.localScale;
        }

        // Euler Angle Helper
        // return the angle between -180 to 180
        public static Vector3 GetLocalEulerAngle(this Transform trans)
        {
            return new Vector3(EulerClamp(trans.localEulerAngles.x), EulerClamp(trans.localEulerAngles.y), EulerClamp(trans.localEulerAngles.z));
        }

        public static Bounds GetBounds(this RectTransform trans)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = trans.worldToLocalMatrix;
            Vector3[] corners = new Vector3[4];
            trans.GetWorldCorners(corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);

            return bounds;
        }

        public static float EulerClamp(float x) { return x > 180 ? x - 360 : x; }
    }
}
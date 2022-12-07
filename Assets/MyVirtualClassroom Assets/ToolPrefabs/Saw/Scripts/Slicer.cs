using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MyVirtualClassroom_Assets.ToolPrefabs.Saw.Scripts
{
    /// <summary>
    /// This class contain the static function to divid an object into two individual objects
    /// It should be called by the slicing object and send in a cutting plane based on the
    /// slicers tips position when entered and exit the object to be divided.
    /// </summary>
    public class Slicer
    {
        public static GameObject[] Slice(Plane aSlicingPlane, GameObject anObjectToSlice)
        {
            Mesh mesh = anObjectToSlice.GetComponent<MeshFilter>().mesh;
            var a = mesh.GetSubMesh(0);

            Sliceable sliceable = anObjectToSlice.GetComponent<Sliceable>();
            if (sliceable == null)
                throw new NotSupportedException("Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");

            SlicesMetadata slicesMeta = new SlicesMetadata(aSlicingPlane, mesh, sliceable.IsSolid, sliceable.ReverseWindTriangles, sliceable.ShareVerticies, sliceable.SmoothVerticices);

            GameObject positiveObject = CreateMeshGameObject(anObjectToSlice);
            positiveObject.name = "Plank_" + (SlicerSupportTools.SLICED_PART_COUNT);
            //positiveObject.name = String.Format("{0}_positive", anObjectToSlice.name);

            GameObject negativeObject = CreateMeshGameObject(anObjectToSlice);
            negativeObject.name = "Plank_" + (SlicerSupportTools.SLICED_PART_COUNT + 1);
            //negativeObject.name = String.Format("{0}_negative", anObjectToSlice.name);

            SlicerSupportTools.SLICED_PART_COUNT += 2;

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity);

            return new GameObject[] { positiveObject, negativeObject };
        }

        private static GameObject CreateMeshGameObject(GameObject anOriginalObject)
        {
            Material[] originalMaterials = anOriginalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshObject = new GameObject();
            Sliceable originalSliceable = anOriginalObject.GetComponent<Sliceable>();

            meshObject.AddComponent<MeshFilter>();
            meshObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWindTriangles = originalSliceable.ReverseWindTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            meshObject.GetComponent<MeshRenderer>().materials = originalMaterials;
            meshObject.transform.localScale = anOriginalObject.transform.localScale;
            meshObject.transform.rotation = anOriginalObject.transform.rotation;
            meshObject.transform.position = anOriginalObject.transform.position;

            meshObject.tag = anOriginalObject.tag;

            return meshObject;
        }

        private static void SetupCollidersAndRigidBodys(ref GameObject anObject, Mesh aMesh, bool anUseGravity)
        {
            MeshCollider meshCollider = anObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = aMesh;
            meshCollider.convex = true;

            var rb = anObject.AddComponent<Rigidbody>();
            rb.useGravity = anUseGravity;
        }
    }
}

/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using UnityEngine;

public partial class MeshLoader
{
	private static List<string> possibleMaterialPaths = new List<string>()
		{
			"",
			"/textures/",
			"../",
			"../materials/", "../materials/textures/",
			"../../materials/", "../../materials/textures/"
		};

	class MeshMaterialSet
	{
		private readonly int _materialIndex;
		private readonly Mesh _mesh;
		private Material _material;

		public MeshMaterialSet(in Mesh mesh, in int materialIndex)
		{
			_mesh = mesh;
			_materialIndex = materialIndex;
		}

		public int MaterialIndex => _materialIndex;

		public Material Material
		{
			get => _material;
			set => _material = value;
		}

		public Mesh Mesh => _mesh;
	}

	class MeshMaterialList
	{
		private List<MeshMaterialSet> meshMatList = new List<MeshMaterialSet>();

		public int Count => meshMatList.Count;

		public void Add(in MeshMaterialSet meshMatSet)
		{
			meshMatList.Add(meshMatSet);
		}

		public void SetMaterials(in List<Material> materials)
		{
			foreach (var meshMatSet in meshMatList)
			{
				meshMatSet.Material = materials[meshMatSet.MaterialIndex];
			}
		}

		public MeshMaterialSet Get(in int index)
		{
			return meshMatList[index];
		}
	}

	private static bool CheckFileSupport(in string fileExtension)
	{
		var isFileSupported = true;

		switch (fileExtension)
		{
			case ".dae":
			case ".obj":
			case ".stl":
				break;

			default:
				isFileSupported = false;
				break;
		}

		return isFileSupported;
	}

	private static Vector3 GetRotationByFileExtension(in string fileExtension, in string meshPath)
	{
		var eulerRotation = Vector3.zero;

		switch (fileExtension)
		{
			case ".dae":
			case ".obj":
			case ".stl":
				eulerRotation.Set(90f, -90f, 0f);
				break;

			default:
				break;
		}

		return eulerRotation;
	}

	private static Matrix4x4 ConvertAssimpMatrix4x4ToUnity(in Assimp.Matrix4x4 assimpMatrix)
	{
		return ConvertAssimpMatrix4x4ToUnity(assimpMatrix, Quaternion.identity);
	}

	private static Matrix4x4 ConvertAssimpMatrix4x4ToUnity(in Assimp.Matrix4x4 assimpMatrix, in Quaternion targetRotation)
	{
		assimpMatrix.Decompose(out var scaling, out var rotation, out var translation);
		var pos = new Vector3(translation.X, translation.Y, translation.Z);
		var q = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
		q *= targetRotation;
		var s = new Vector3(scaling.X, scaling.Y, scaling.Z);
		return Matrix4x4.TRS(pos, q, s);
	}

	private static readonly Assimp.AssimpContext importer = new Assimp.AssimpContext();

	private static readonly Assimp.LogStream logstream = new Assimp.LogStream(
		delegate (String msg, String userData)
		{
			Debug.Log(msg);
		});

	private static Assimp.Scene GetScene(in string targetPath, out Quaternion meshRotation)
	{
		meshRotation = Quaternion.identity;

		if (!File.Exists(targetPath))
		{
			Debug.LogError("File doesn't exist: " + targetPath);
			return null;
		}

		var colladaIgnoreConfig = new Assimp.Configs.ColladaIgnoreUpDirectionConfig(true);
		importer.SetConfig(colladaIgnoreConfig);

		// logstream.Attach();

		var fileExtension = Path.GetExtension(targetPath).ToLower();

		if (!CheckFileSupport(fileExtension))
		{
			Debug.LogWarning("Unsupported file extension: " + fileExtension + " -> " + targetPath);
			return null;
		}

		const Assimp.PostProcessSteps postProcessFlags =
			Assimp.PostProcessSteps.OptimizeGraph |
			Assimp.PostProcessSteps.OptimizeMeshes |
			Assimp.PostProcessSteps.CalculateTangentSpace |
			Assimp.PostProcessSteps.JoinIdenticalVertices |
			Assimp.PostProcessSteps.RemoveRedundantMaterials |
			Assimp.PostProcessSteps.Triangulate |
			Assimp.PostProcessSteps.SortByPrimitiveType |
			Assimp.PostProcessSteps.ValidateDataStructure |
			Assimp.PostProcessSteps.FindInvalidData |
			Assimp.PostProcessSteps.MakeLeftHanded;

		var scene = importer.ImportFile(targetPath, postProcessFlags);
		if (scene == null)
		{
			return null;
		}

		// Rotate meshes for Unity world since all 3D object meshes are oriented to right handed coordinates
		var eulerRotation = GetRotationByFileExtension(fileExtension, targetPath);
		meshRotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z);

		return scene;
	}
}

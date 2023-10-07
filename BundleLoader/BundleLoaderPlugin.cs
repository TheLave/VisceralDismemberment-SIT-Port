using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;

namespace Nexus.BundleLoader
{
	[BepInPlugin("com.pandahhcorp.bundleloader", "BundleLoader", "1.0.0")]
	public class BundleLoaderPlugin : BaseUnityPlugin
	{
		private Dictionary<String, AssetBundleCreateRequest> _loadedBundles;
		public static BundleLoaderPlugin Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
			String workingDirectory =
				Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "ssh", "Bundles");
			if (Directory.Exists(workingDirectory))
			{
				this._loadedBundles = Directory.GetFiles(workingDirectory).Where(f => f.ToLower().EndsWith(".bundle"))
					.ToDictionary(s => Path.GetFileNameWithoutExtension(s).ToLower(), AssetBundle.LoadFromFileAsync);
			}
			else
			{
				Directory.CreateDirectory(workingDirectory);
				this._loadedBundles = new Dictionary<String, AssetBundleCreateRequest>();
			}

			this.Logger.LogInfo($"Loaded {this._loadedBundles.Count} bundles...");
		}

		public Boolean IsLoading(String bundleName, out Boolean isFinished)
		{
			isFinished = false;
			if (!this._loadedBundles.TryGetValue(bundleName, out AssetBundleCreateRequest request))
			{
				return false;
			}

			isFinished = request.isDone;

			return true;
		}

		public AssetBundle GetAssetBundle(String bundleName)
		{
			if (this._loadedBundles.TryGetValue(bundleName, out AssetBundleCreateRequest request) && request.isDone)
			{
				return request.assetBundle;
			}

			return null;
		}

		public async Task<AssetBundle> GetAssetBundleAsync(String bundleName,
			CancellationToken cancellationToken = default)
		{
			if (!this._loadedBundles.TryGetValue(bundleName, out AssetBundleCreateRequest request))
			{
				return null;
			}

			while (!request.isDone && !cancellationToken.CanBeCanceled)
			{
				await Task.Yield();
			}

			return cancellationToken.IsCancellationRequested ? null : request.assetBundle;
		}

		public IEnumerator GetAssetBundleWaitForLoad(String bundleName)
		{
			if (!this._loadedBundles.TryGetValue(bundleName, out AssetBundleCreateRequest request))
			{
				yield break;
			}

			while (!request.isDone)
			{
				yield return null;
			}

			yield return request.assetBundle;
		}
	}
}
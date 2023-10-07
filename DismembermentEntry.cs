using BepInEx;
using Comfort.Common;
using EFT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Aki.Reflection.Patching;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DismembermentMod
{
    [BepInPlugin("com.servph.servphDismemberment", "SERVPH's Dismemberment", "1.0.0")]
    public class DismembermentEntry : BaseUnityPlugin
    {
        public static DismembermentEntry Instance { get; private set; }


        public void Awake()
        {
            new DismembermentPatch().Enable();
        }

        public void Update()
        {

        }




    }
}

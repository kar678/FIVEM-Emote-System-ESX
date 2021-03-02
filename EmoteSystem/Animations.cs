using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace EmoteSystem.Client
{
    public static class Animations
    {
        private static bool loadedAnimations = false;
        public static Dictionary<string, animationStruct> animDict;

        public static void LoadAnimations()
        {
            if (!loadedAnimations)
            {
                animDict = new Dictionary<string, animationStruct>();

                string jsonString = LoadResourceFile(GetCurrentResourceName(), "Animations.json");

                if (string.IsNullOrEmpty(jsonString))
                {
                    // Do not continue if the file is empty or it's null.
                    Debug.WriteLine("[EmoteSystem] An error occurred while loading the Animations file.");
                    return;
                }

                // Convert the json into an object.
                Newtonsoft.Json.Linq.JObject jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonString);

                int i = 0;

                Newtonsoft.Json.Linq.JArray Animations = (Newtonsoft.Json.Linq.JArray)jsonData["Animations"];

                foreach (var animation in Animations)
                {
                    animationStruct animationClass = new animationStruct(animation["animationdict"].ToString(), animation["animationname"].ToString(), animation["animationtype"].ToString());


                    animDict.Add(animation["animationcommandname"].ToString(), animationClass);

                    i++;
                }

                loadedAnimations = true;
            }
        }

    }
}

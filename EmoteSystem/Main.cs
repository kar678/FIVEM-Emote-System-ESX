using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace EmoteSystem.Client
{
    public class animationStruct
    {
        public string animDict, animName, animType;

        public animationStruct(string animationDict, string animationName, string animationType)
        {
            animDict = animationDict;
            animName = animationName;
            animType = animationType;
        }
    }
    
    public class Main : BaseScript
    {
        //Properties
        dynamic ESX;
        bool isDead = false;

        public Main()
        {
            //Event Handlers
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["esx:onPlayerDeath"] += new Action<object>(OnPlayerDeath);
            EventHandlers["esx:onPlayerSpawn"] += new Action<object>(OnPlayerSpawn);

            Exports.Add("playAnimationCommand", new Action<string>(PlayAnimationWithCommandName));
            Exports.Add("stopAllPedAnimations", new Action<int>(StopAllPedAnimations));

            Animations.LoadAnimations();
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            //ESX Setup
            TriggerEvent("esx:getSharedObject", new object[] { new Action<dynamic>(esx => { ESX = esx; }) });

            //Commands
            RegisterCommand("emote", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string arg = "";

                if (args.Count > 0)
                {
                    arg = args[0].ToString();
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Usage - /emote {emote name}" }
                    });

                    return;
                }

                animationStruct animation = null;

                try
                {
                    animation = Animations.animDict[arg];
                }
                catch(Exception e)
                {

                }

                if (animation != null && !isDead)
                {
                    if (animation.animType == "Animation")
                    {
                        StartPedAnimation(animation.animDict, animation.animName);
                    }
                    else if (animation.animType == "Scenario")
                    {
                        StartPedScenario(animation.animName);
                    }
                    else if (animation.animType == "Attitude")
                    {
                        StartPedAttitude(animation.animDict, animation.animName);
                    }
                }
				else
				{
                    if (isDead)
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            args = new[] { "[EmoteSystem]", "Can't play emotes while dead" }
                        });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            args = new[] { "[EmoteSystem]", "Couldn't play emote or emote is invalid" }
                        });
                    }
                }

            }), false);

            RegisterCommand("e", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string arg = "";

                if (args.Count > 0)
                {
                    arg = args[0].ToString();
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Usage - /e {emote name}" }
                    });

                    return;
                }

                animationStruct animation = null;

                try
                {
                    animation = Animations.animDict[arg.ToString()];
                }
                catch (Exception e)
                {

                }

                if (animation != null && !isDead)
                {
                    if (animation.animType == "Animation")
                    {
                        StartPedAnimation(animation.animDict, animation.animName);
                    }
                    else if (animation.animType == "Scenario")
                    {
                        StartPedScenario(animation.animName);
                    }
                    else if (animation.animType == "Attitude")
                    {
                        StartPedAttitude(animation.animDict, animation.animName);
                    }
                }
				else
				{
                    if (isDead)
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            args = new[] { "[EmoteSystem]", "Can't play emotes while dead" }
                        });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            args = new[] { "[EmoteSystem]", "Couldn't play emote or emote is invalid" }
                        });
                    }
				}

            }), false);

            RegisterCommand("cancelemote", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (!isDead)
                {
                    StopAllCurrentAnimations();
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Cannot cancel emotes while dead" }
                    });
                }
            }), false);

            RegisterCommand("c", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (!isDead)
                {
                    StopAllCurrentAnimations();
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Cannot cancel emotes while dead" }
                    });
                }
            }), false);
        }

        public void StopAllCurrentAnimations()
        {
            ClearPedTasks(PlayerPedId());
            ResetPedMovementClipset(PlayerPedId(), 0.0f);
        }

        public void StartPedAttitude(string clipSetLib, string clipSet)
        {
            
            RequestAnimSet(clipSetLib);

            if (HasAnimSetLoaded(clipSetLib))
            {
                SetPedMovementClipset(PlayerPedId(), clipSet, 1.0f);
            }
        }

        public void StartPedAnimation(string animLib, string anim)
        {
            RequestAnimDict(animLib);

            if(HasAnimDictLoaded(animLib))
            {
                //AnimationFlags flags = AnimationFlags.Loop | AnimationFlags.CancelableWithMovement;
                TaskPlayAnim(PlayerPedId(), animLib, anim, 8.0f, -8.0f, -1, 33, 0.0f, false, false, false);
            }
        }

        public void StartPedScenario(string scenario)
        {
            TaskStartScenarioInPlace(PlayerPedId(), scenario, 0, false);
        }

        public void OnPlayerDeath(object objectData)
        {
            isDead = true;
        }

        public void OnPlayerSpawn(object objectData)
        {
            isDead = false;
        }

#region Exports
        public void PlayAnimationWithCommandName(string commandName)
        {
            string arg = commandName;

            animationStruct animation = null;

            try
            {
                animation = Animations.animDict[arg.ToString()];
            }
            catch (Exception e)
            {

            }

            if (animation != null && !isDead)
            {
                if (animation.animType == "Animation")
                {
                    StartPedAnimation(animation.animDict, animation.animName);
                }
                else if (animation.animType == "Scenario")
                {
                    StartPedScenario(animation.animName);
                }
                else if (animation.animType == "Attitude")
                {
                    StartPedAttitude(animation.animDict, animation.animName);
                }
            }
            else
            {
                if (isDead)
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Can't play emotes while dead" }
                    });
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[EmoteSystem]", "Couldn't play emote or emote is invalid" }
                    });
                }
            }
        }

        public void StopAllPedAnimations(int ped)
        {
            ClearPedTasks(ped);
            ResetPedMovementClipset(ped, 0.0f);
        }

#endregion
    }
}

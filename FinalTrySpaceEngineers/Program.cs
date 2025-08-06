﻿using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;


namespace IngameScript
{
    public abstract partial class Program : MyGridProgram
    {
        private readonly CoreSystem _coreSystem;
        
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        // ReSharper disable once PublicConstructorInAbstractClass
        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            
            _coreSystem = new CoreSystem(program: this);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
            switch (updateSource)
            {
                case UpdateType.Update100:
                    _coreSystem.Update();
                    break;
                
                case UpdateType.Terminal:
                case UpdateType.Trigger:
                    _coreSystem.ExecuteCommand(argument);
                    break;
                
                case UpdateType.None:
                case UpdateType.Mod:
                case UpdateType.Script:
                case UpdateType.Update1:
                case UpdateType.Update10:
                case UpdateType.Once:
                case UpdateType.IGC:
                default:
                    break;
            }
        }
    }
}
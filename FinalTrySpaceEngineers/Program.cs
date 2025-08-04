using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public abstract partial class Program : MyGridProgram
    {
        private readonly List<IMyTerminalBlock> _allBlocks = new List<IMyTerminalBlock>();
        private const string DebugDisplayName = "[NEW BASE] Debug display";
        
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
            
            // _coreSystem = new CoreSystem(program: this);
        
        
            var debugText = string.Empty;
            GridTerminalSystem.GetBlocks(_allBlocks);
            var plcScreen = Me.GetSurface(0);
            if (_allBlocks.FirstOrDefault(block => block.CustomName == DebugDisplayName) == null)
                debugText += $"Не могу найти панель с именем {DebugDisplayName}\n";
            else
            {
                debugText += $"Дисплей для отладки с именем {DebugDisplayName} найден.\n";
                var debugScreen = GridTerminalSystem.GetBlockWithName(DebugDisplayName) as IMyTextPanel;
                debugScreen?.WriteText("Журнал срабатывания тревог:\n");
            }
        
            plcScreen.WriteText(debugText);

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
        }
    }
}
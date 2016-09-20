using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* Copyright 2016 Xenox Co.
*
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. You 
* may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software distributed 
* under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
* CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
* language governing permissions and limitations under the License.
*/
namespace CoreScorpion
{
    class Start
    {
        static void Main(string[] args)
        {
            string[] pOptions = null;
            
            if (args.Length == 0)
            {
                ConsoleHelper.Error("no input files!\nTry '{0}  -help' for more information.", 
                    Application.Constants.Name);
            }

            pOptions = ConsoleHelper.ProcessArgs(args);
            if(pOptions == null)
            {
                ConsoleHelper.Error("executable file not provided, see " + 
                    Application.Constants.Name + " -help for more details.");
            }

            if(!Application.Ready())
            {
                Application.Error("Application is not ready to start. Please make sure you" +
                    "have installed " + Application.Constants.ProductNameShort + " correctly.");
            }

            Globals.Init();
            if (ExeLoader.Load(pOptions))
            {
                if (!ScorpionVM.Startup(pOptions))
                {
                    ConsoleHelper.Error("exe \"" + Globals.Exe.header.name + "\" failed to start.");
                }
            }
            
            ConsoleHelper.Error("executable file: \"" + pOptions[0] + "\" could not be loaded.");
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BattleshipNeuralNet {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class NetSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static NetSettings defaultInstance = ((NetSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new NetSettings())));
        
        public static NetSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\usgtd-4pshxp3\\c$\\Users\\carlril\\Documents\\Battleship\\results.csv")]
        public string LogCSVPath {
            get {
                return ((string)(this["LogCSVPath"]));
            }
            set {
                this["LogCSVPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\usgtd-4pshxp3\\c$\\Users\\carlril\\Documents\\Battleship\\networks.dat")]
        public string NetworkSavePath {
            get {
                return ((string)(this["NetworkSavePath"]));
            }
            set {
                this["NetworkSavePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int TargetPoolSize {
            get {
                return ((int)(this["TargetPoolSize"]));
            }
            set {
                this["TargetPoolSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int NetworkAttempts {
            get {
                return ((int)(this["NetworkAttempts"]));
            }
            set {
                this["NetworkAttempts"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.05")]
        public double MutationRate {
            get {
                return ((double)(this["MutationRate"]));
            }
            set {
                this["MutationRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.05")]
        public double MutationEpsilon {
            get {
                return ((double)(this["MutationEpsilon"]));
            }
            set {
                this["MutationEpsilon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.5")]
        public double SurvivalRate {
            get {
                return ((double)(this["SurvivalRate"]));
            }
            set {
                this["SurvivalRate"] = value;
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stutter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Zachary Lewis")]
        public string AUTHOR {
            get {
                return ((string)(this["AUTHOR"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Copyright © 2012")]
        public string COPYRIGHT {
            get {
                return ((string)(this["COPYRIGHT"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        public int PhraseLength {
            get {
                return ((int)(this["PhraseLength"]));
            }
            set {
                this["PhraseLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int BlockLength {
            get {
                return ((int)(this["BlockLength"]));
            }
            set {
                this["BlockLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsTaskListVisible {
            get {
                return ((bool)(this["IsTaskListVisible"]));
            }
            set {
                this["IsTaskListVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsSoundEnabled {
            get {
                return ((bool)(this["IsSoundEnabled"]));
            }
            set {
                this["IsSoundEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DoesHideOnMinimize {
            get {
                return ((bool)(this["DoesHideOnMinimize"]));
            }
            set {
                this["DoesHideOnMinimize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsTaskValueVisible {
            get {
                return ((bool)(this["IsTaskValueVisible"]));
            }
            set {
                this["IsTaskValueVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsTaskProgressVisible {
            get {
                return ((bool)(this["IsTaskProgressVisible"]));
            }
            set {
                this["IsTaskProgressVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("default.sss")]
        public string LastTaskListFilename {
            get {
                return ((string)(this["LastTaskListFilename"]));
            }
            set {
                this["LastTaskListFilename"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Stutter")]
        public string APPLICATION_NAME {
            get {
                return ((string)(this["APPLICATION_NAME"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AreCompletedTasksVisible {
            get {
                return ((bool)(this["AreCompletedTasksVisible"]));
            }
            set {
                this["AreCompletedTasksVisible"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://docs.google.com/a/stutterapp.com/spreadsheet/viewform?pli=1&formkey=dEdnR" +
            "EpPXzJPNXdIYzdyMnJ6ZU1adlE6MQ")]
        public string BUG_REPORT_URI {
            get {
                return ((string)(this["BUG_REPORT_URI"]));
            }
        }
    }
}

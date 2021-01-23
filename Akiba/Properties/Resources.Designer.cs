﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Akiba.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Akiba.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] DefaultGameConfig {
            get {
                object obj = ResourceManager.GetObject("DefaultGameConfig", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The backup game configuration executable is missing. What did you do with it?.
        /// </summary>
        internal static string MessageBackupMissing {
            get {
                return ResourceManager.GetString("MessageBackupMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something is wrong with the YAML configuration file. Please double check its syntax and restart the game..
        /// </summary>
        internal static string MessageConfigurationError {
            get {
                return ResourceManager.GetString("MessageConfigurationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The game configuration executable is missing. What did you do with it?.
        /// </summary>
        internal static string MessageConfigUtilityMissing {
            get {
                return ResourceManager.GetString("MessageConfigUtilityMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The main game executable couldn&apos;t be located. Did you put the utility inside the game directory?.
        /// </summary>
        internal static string MessageGameLocation {
            get {
                return ResourceManager.GetString("MessageGameLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The installation was successful! Don&apos;t forget to edit the settings in {0} before starting the game..
        /// </summary>
        internal static string MessageSuccess {
            get {
                return ResourceManager.GetString("MessageSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It seems like {0} is already installed. Do you want to migrate from version {1} to {2}?.
        /// </summary>
        internal static string MessageUpgradeNotice {
            get {
                return ResourceManager.GetString("MessageUpgradeNotice", resourceCulture);
            }
        }
    }
}

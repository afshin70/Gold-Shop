﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gold.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ExceptionErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Gold.Resources.ExceptionErrorMessages", typeof(ExceptionErrorMessages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to مقدار پارامتر اسال شده نال است..
        /// </summary>
        public static string ArgumentNullException {
            get {
                return ResourceManager.GetString("ArgumentNullException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to مقدار {0} اسال شده نال است..
        /// </summary>
        public static string ArgumentNullException0 {
            get {
                return ResourceManager.GetString("ArgumentNullException0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to خطایی در برنامه رخ داده است.
        /// </summary>
        public static string Exception {
            get {
                return ResourceManager.GetString("Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to عملیات نامعتبر می باشد..
        /// </summary>
        public static string InvalidOperationException {
            get {
                return ResourceManager.GetString("InvalidOperationException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to عملیات لغو شده است..
        /// </summary>
        public static string OperationCanceledException {
            get {
                return ResourceManager.GetString("OperationCanceledException", resourceCulture);
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LegionKun.Base {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LegionKun.Base.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
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
        ///   Ищет локализованную строку, похожую на AIzaSyDIuH33zi6aod6jSHm31V1VIVKYIIGxvEo.
        /// </summary>
        internal static string ApiKeyToken {
            get {
                return ResourceManager.GetString("ApiKeyToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Data Source=KIRILL\SQL_LEGIONKUN;Initial Catalog=UserBanned;Integrated Security=True.
        /// </summary>
        internal static string ConnectionKey {
            get {
                return ResourceManager.GetString("ConnectionKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на NDU4Mjc2NjM2MDY0ODc0NDk3.DhZByw.dBlIP1itXd7XOjmVe59drPhkB7o.
        /// </summary>
        internal static string TestBotToken {
            get {
                return ResourceManager.GetString("TestBotToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на NDYwMTUyNTgzNzc2ODk0OTk3.DhAm7g.GSRCqXFiNo_oQQuv2Uhk770Rxbg.
        /// </summary>
        internal static string TokenBot {
            get {
                return ResourceManager.GetString("TokenBot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 4.35.
        /// </summary>
        internal static string VersionBot {
            get {
                return ResourceManager.GetString("VersionBot", resourceCulture);
            }
        }
    }
}

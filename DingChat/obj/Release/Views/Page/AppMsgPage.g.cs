﻿#pragma checksum "..\..\..\..\Views\Page\AppMsgPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1B752EE76DFD547D273639BA9FC85EFD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ResourceDictionary.Control;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace cn.lds.chatcore.pcw.Views.Page {
    
    
    /// <summary>
    /// AppMsgPage
    /// </summary>
    public partial class AppMsgPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\Views\Page\AppMsgPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Titel;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Views\Page\AppMsgPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSet;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\Views\Page\AppMsgPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ResourceDictionary.Control.DingScrollview ScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\Views\Page\AppMsgPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel StackMain;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DingChatExt;component/views/page/appmsgpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Page\AppMsgPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\..\Views\Page\AppMsgPage.xaml"
            ((cn.lds.chatcore.pcw.Views.Page.AppMsgPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Titel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.BtnSet = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\Views\Page\AppMsgPage.xaml"
            this.BtnSet.Click += new System.Windows.RoutedEventHandler(this.BtnSet_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ScrollViewer = ((ResourceDictionary.Control.DingScrollview)(target));
            return;
            case 5:
            this.StackMain = ((System.Windows.Controls.StackPanel)(target));
            
            #line 28 "..\..\..\..\Views\Page\AppMsgPage.xaml"
            this.StackMain.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.scrollViewer_MouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

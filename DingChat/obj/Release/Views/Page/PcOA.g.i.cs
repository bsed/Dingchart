﻿#pragma checksum "..\..\..\..\Views\Page\PcOA.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7AF3E0C44AB358CD2623AC8B6A2A0B4F"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ResourceDictionary.Converters;
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
using System.Windows.Interactivity;
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
using cn.lds.chatcore.pcw.Views.Control;


namespace cn.lds.chatcore.pcw.Views.Page {
    
    
    /// <summary>
    /// PcOA
    /// </summary>
    public partial class PcOA : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\Views\Page\PcOA.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Grid;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Views\Page\PcOA.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ListBoxLeft;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Views\Page\PcOA.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal cn.lds.chatcore.pcw.Views.Control.DingFrame ChatFrame;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Views\Page\PcOA.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SearchText;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\Views\Page\PcOA.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnAdd;
        
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
            System.Uri resourceLocater = new System.Uri("/DingChatExt;component/views/page/pcoa.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Page\PcOA.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 10 "..\..\..\..\Views\Page\PcOA.xaml"
            ((cn.lds.chatcore.pcw.Views.Page.PcOA)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\Views\Page\PcOA.xaml"
            ((cn.lds.chatcore.pcw.Views.Page.PcOA)(target)).MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseUp);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\..\Views\Page\PcOA.xaml"
            ((cn.lds.chatcore.pcw.Views.Page.PcOA)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.Page_Unloaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.ListBoxLeft = ((System.Windows.Controls.ListView)(target));
            return;
            case 4:
            this.ChatFrame = ((cn.lds.chatcore.pcw.Views.Control.DingFrame)(target));
            return;
            case 5:
            this.SearchText = ((System.Windows.Controls.TextBox)(target));
            
            #line 57 "..\..\..\..\Views\Page\PcOA.xaml"
            this.SearchText.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.SearchText_TextChanged);
            
            #line default
            #line hidden
            
            #line 58 "..\..\..\..\Views\Page\PcOA.xaml"
            this.SearchText.KeyUp += new System.Windows.Input.KeyEventHandler(this.SearchText_OnKeyUp);
            
            #line default
            #line hidden
            
            #line 59 "..\..\..\..\Views\Page\PcOA.xaml"
            this.SearchText.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.SearchText_OnPreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnAdd = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\..\Views\Page\PcOA.xaml"
            this.BtnAdd.Click += new System.Windows.RoutedEventHandler(this.BtnAdd_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\..\Views\LoadExistingModel.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A1EDA477601645914192299CB58BF7B31A70993662A87D55CB58DCC256D50141"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Gaant_Chart;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Gaant_Chart {
    
    
    /// <summary>
    /// LoadExistingModel
    /// </summary>
    public partial class LoadExistingModel : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\Views\LoadExistingModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox searchTxt;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Views\LoadExistingModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel modelNamesSP;
        
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
            System.Uri resourceLocater = new System.Uri("/Gaant Chart;component/views/loadexistingmodel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\LoadExistingModel.xaml"
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
            this.searchTxt = ((System.Windows.Controls.TextBox)(target));
            
            #line 31 "..\..\..\Views\LoadExistingModel.xaml"
            this.searchTxt.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.searchTxt_TextChanged);
            
            #line default
            #line hidden
            
            #line 32 "..\..\..\Views\LoadExistingModel.xaml"
            this.searchTxt.GotFocus += new System.Windows.RoutedEventHandler(this.searchTxt_GotFocus);
            
            #line default
            #line hidden
            
            #line 33 "..\..\..\Views\LoadExistingModel.xaml"
            this.searchTxt.LostFocus += new System.Windows.RoutedEventHandler(this.searchTxt_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.modelNamesSP = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            
            #line 57 "..\..\..\Views\LoadExistingModel.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btn_Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


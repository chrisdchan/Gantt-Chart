﻿#pragma checksum "..\..\..\Views\RegNewModel.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A3E53C67168EB3955B47F7510ADAA08910BC8D7629918C876D5AE7928A9DCBCB"
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
    /// RegNewModel
    /// </summary>
    public partial class RegNewModel : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\..\Views\RegNewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbModelID;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Views\RegNewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbDate;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Views\RegNewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCreateModel;
        
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
            System.Uri resourceLocater = new System.Uri("/Gaant Chart;component/views/regnewmodel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\RegNewModel.xaml"
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
            this.tbModelID = ((System.Windows.Controls.TextBox)(target));
            
            #line 38 "..\..\..\Views\RegNewModel.xaml"
            this.tbModelID.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbModelID_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbDate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.btnCreateModel = ((System.Windows.Controls.Button)(target));
            
            #line 57 "..\..\..\Views\RegNewModel.xaml"
            this.btnCreateModel.Click += new System.Windows.RoutedEventHandler(this.btnCreateModel_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 64 "..\..\..\Views\RegNewModel.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

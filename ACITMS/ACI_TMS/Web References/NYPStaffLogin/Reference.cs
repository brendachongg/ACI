﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace ACI_TMS.NYPStaffLogin {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="StaffLoginBinding", Namespace="http://WSStaffLogin/")]
    public partial class WSStaffLogin : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback getErrMsgOperationCompleted;
        
        private System.Threading.SendOrPostCallback authStaffPortalOperationCompleted;
        
        private System.Threading.SendOrPostCallback valLoginEncOperationCompleted;
        
        private System.Threading.SendOrPostCallback getLoginEmpNoOperationCompleted;
        
        private System.Threading.SendOrPostCallback valLoginBySchOperationCompleted;
        
        private System.Threading.SendOrPostCallback valLoginByOthersOperationCompleted;
        
        private System.Threading.SendOrPostCallback getStaffDeptOperationCompleted;
        
        private System.Threading.SendOrPostCallback getStaffInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback valLoginOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public WSStaffLogin() {
            this.Url = global::ACI_TMS.Properties.Settings.Default.ACI_TMS_NYPStaffLogin_WSStaffLogin;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event getErrMsgCompletedEventHandler getErrMsgCompleted;
        
        /// <remarks/>
        public event authStaffPortalCompletedEventHandler authStaffPortalCompleted;
        
        /// <remarks/>
        public event valLoginEncCompletedEventHandler valLoginEncCompleted;
        
        /// <remarks/>
        public event getLoginEmpNoCompletedEventHandler getLoginEmpNoCompleted;
        
        /// <remarks/>
        public event valLoginBySchCompletedEventHandler valLoginBySchCompleted;
        
        /// <remarks/>
        public event valLoginByOthersCompletedEventHandler valLoginByOthersCompleted;
        
        /// <remarks/>
        public event getStaffDeptCompletedEventHandler getStaffDeptCompleted;
        
        /// <remarks/>
        public event getStaffInfoCompletedEventHandler getStaffInfoCompleted;
        
        /// <remarks/>
        public event valLoginCompletedEventHandler valLoginCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getErrMsg() {
            object[] results = this.Invoke("getErrMsg", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getErrMsgAsync() {
            this.getErrMsgAsync(null);
        }
        
        /// <remarks/>
        public void getErrMsgAsync(object userState) {
            if ((this.getErrMsgOperationCompleted == null)) {
                this.getErrMsgOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetErrMsgOperationCompleted);
            }
            this.InvokeAsync("getErrMsg", new object[0], this.getErrMsgOperationCompleted, userState);
        }
        
        private void OngetErrMsgOperationCompleted(object arg) {
            if ((this.getErrMsgCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getErrMsgCompleted(this, new getErrMsgCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string authStaffPortal([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg1) {
            object[] results = this.Invoke("authStaffPortal", new object[] {
                        arg0,
                        arg1});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void authStaffPortalAsync(string arg0, string arg1) {
            this.authStaffPortalAsync(arg0, arg1, null);
        }
        
        /// <remarks/>
        public void authStaffPortalAsync(string arg0, string arg1, object userState) {
            if ((this.authStaffPortalOperationCompleted == null)) {
                this.authStaffPortalOperationCompleted = new System.Threading.SendOrPostCallback(this.OnauthStaffPortalOperationCompleted);
            }
            this.InvokeAsync("authStaffPortal", new object[] {
                        arg0,
                        arg1}, this.authStaffPortalOperationCompleted, userState);
        }
        
        private void OnauthStaffPortalOperationCompleted(object arg) {
            if ((this.authStaffPortalCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.authStaffPortalCompleted(this, new authStaffPortalCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int valLoginEnc([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg1) {
            object[] results = this.Invoke("valLoginEnc", new object[] {
                        arg0,
                        arg1});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void valLoginEncAsync(string arg0, string arg1) {
            this.valLoginEncAsync(arg0, arg1, null);
        }
        
        /// <remarks/>
        public void valLoginEncAsync(string arg0, string arg1, object userState) {
            if ((this.valLoginEncOperationCompleted == null)) {
                this.valLoginEncOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalLoginEncOperationCompleted);
            }
            this.InvokeAsync("valLoginEnc", new object[] {
                        arg0,
                        arg1}, this.valLoginEncOperationCompleted, userState);
        }
        
        private void OnvalLoginEncOperationCompleted(object arg) {
            if ((this.valLoginEncCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.valLoginEncCompleted(this, new valLoginEncCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getLoginEmpNo([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0) {
            object[] results = this.Invoke("getLoginEmpNo", new object[] {
                        arg0});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getLoginEmpNoAsync(string arg0) {
            this.getLoginEmpNoAsync(arg0, null);
        }
        
        /// <remarks/>
        public void getLoginEmpNoAsync(string arg0, object userState) {
            if ((this.getLoginEmpNoOperationCompleted == null)) {
                this.getLoginEmpNoOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetLoginEmpNoOperationCompleted);
            }
            this.InvokeAsync("getLoginEmpNo", new object[] {
                        arg0}, this.getLoginEmpNoOperationCompleted, userState);
        }
        
        private void OngetLoginEmpNoOperationCompleted(object arg) {
            if ((this.getLoginEmpNoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getLoginEmpNoCompleted(this, new getLoginEmpNoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int valLoginBySch([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg1, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg2) {
            object[] results = this.Invoke("valLoginBySch", new object[] {
                        arg0,
                        arg1,
                        arg2});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void valLoginBySchAsync(string arg0, string arg1, string arg2) {
            this.valLoginBySchAsync(arg0, arg1, arg2, null);
        }
        
        /// <remarks/>
        public void valLoginBySchAsync(string arg0, string arg1, string arg2, object userState) {
            if ((this.valLoginBySchOperationCompleted == null)) {
                this.valLoginBySchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalLoginBySchOperationCompleted);
            }
            this.InvokeAsync("valLoginBySch", new object[] {
                        arg0,
                        arg1,
                        arg2}, this.valLoginBySchOperationCompleted, userState);
        }
        
        private void OnvalLoginBySchOperationCompleted(object arg) {
            if ((this.valLoginBySchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.valLoginBySchCompleted(this, new valLoginBySchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int valLoginByOthers([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg1, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg2, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg3) {
            object[] results = this.Invoke("valLoginByOthers", new object[] {
                        arg0,
                        arg1,
                        arg2,
                        arg3});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void valLoginByOthersAsync(string arg0, string arg1, string arg2, string arg3) {
            this.valLoginByOthersAsync(arg0, arg1, arg2, arg3, null);
        }
        
        /// <remarks/>
        public void valLoginByOthersAsync(string arg0, string arg1, string arg2, string arg3, object userState) {
            if ((this.valLoginByOthersOperationCompleted == null)) {
                this.valLoginByOthersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalLoginByOthersOperationCompleted);
            }
            this.InvokeAsync("valLoginByOthers", new object[] {
                        arg0,
                        arg1,
                        arg2,
                        arg3}, this.valLoginByOthersOperationCompleted, userState);
        }
        
        private void OnvalLoginByOthersOperationCompleted(object arg) {
            if ((this.valLoginByOthersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.valLoginByOthersCompleted(this, new valLoginByOthersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getStaffDept([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0) {
            object[] results = this.Invoke("getStaffDept", new object[] {
                        arg0});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getStaffDeptAsync(string arg0) {
            this.getStaffDeptAsync(arg0, null);
        }
        
        /// <remarks/>
        public void getStaffDeptAsync(string arg0, object userState) {
            if ((this.getStaffDeptOperationCompleted == null)) {
                this.getStaffDeptOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStaffDeptOperationCompleted);
            }
            this.InvokeAsync("getStaffDept", new object[] {
                        arg0}, this.getStaffDeptOperationCompleted, userState);
        }
        
        private void OngetStaffDeptOperationCompleted(object arg) {
            if ((this.getStaffDeptCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getStaffDeptCompleted(this, new getStaffDeptCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] getStaffInfo([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0) {
            object[] results = this.Invoke("getStaffInfo", new object[] {
                        arg0});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void getStaffInfoAsync(string arg0) {
            this.getStaffInfoAsync(arg0, null);
        }
        
        /// <remarks/>
        public void getStaffInfoAsync(string arg0, object userState) {
            if ((this.getStaffInfoOperationCompleted == null)) {
                this.getStaffInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStaffInfoOperationCompleted);
            }
            this.InvokeAsync("getStaffInfo", new object[] {
                        arg0}, this.getStaffInfoOperationCompleted, userState);
        }
        
        private void OngetStaffInfoOperationCompleted(object arg) {
            if ((this.getStaffInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getStaffInfoCompleted(this, new getStaffInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://WSStaffLogin/", ResponseNamespace="http://WSStaffLogin/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int valLogin([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg1) {
            object[] results = this.Invoke("valLogin", new object[] {
                        arg0,
                        arg1});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void valLoginAsync(string arg0, string arg1) {
            this.valLoginAsync(arg0, arg1, null);
        }
        
        /// <remarks/>
        public void valLoginAsync(string arg0, string arg1, object userState) {
            if ((this.valLoginOperationCompleted == null)) {
                this.valLoginOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalLoginOperationCompleted);
            }
            this.InvokeAsync("valLogin", new object[] {
                        arg0,
                        arg1}, this.valLoginOperationCompleted, userState);
        }
        
        private void OnvalLoginOperationCompleted(object arg) {
            if ((this.valLoginCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.valLoginCompleted(this, new valLoginCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void getErrMsgCompletedEventHandler(object sender, getErrMsgCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getErrMsgCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getErrMsgCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void authStaffPortalCompletedEventHandler(object sender, authStaffPortalCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class authStaffPortalCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal authStaffPortalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void valLoginEncCompletedEventHandler(object sender, valLoginEncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class valLoginEncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal valLoginEncCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void getLoginEmpNoCompletedEventHandler(object sender, getLoginEmpNoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getLoginEmpNoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getLoginEmpNoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void valLoginBySchCompletedEventHandler(object sender, valLoginBySchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class valLoginBySchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal valLoginBySchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void valLoginByOthersCompletedEventHandler(object sender, valLoginByOthersCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class valLoginByOthersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal valLoginByOthersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void getStaffDeptCompletedEventHandler(object sender, getStaffDeptCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getStaffDeptCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getStaffDeptCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void getStaffInfoCompletedEventHandler(object sender, getStaffInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getStaffInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getStaffInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void valLoginCompletedEventHandler(object sender, valLoginCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class valLoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal valLoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591
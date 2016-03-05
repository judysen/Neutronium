﻿using Awesomium.Core;
using IntegratedTest;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests
{
    internal class AwesomiumWindowlessSharedJavascriptEngine : IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private readonly TaskCompletionSource<object> _EndTaskCompletionSource;
        private IWebView _CurrentWebView;

        public AwesomiumWindowlessSharedJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
            _EndTaskCompletionSource = new TaskCompletionSource<object>();
        }

        public void Init(string path = "javascript\\index.html")
        {
            InitAsync(path).Wait();
        }

        private async Task InitAsync(string ipath = "javascript\\index.html")
        {
            var taskLoaded = new TaskCompletionSource<object>();

            WebCore.QueueWork( () => 
            {
                _CurrentWebView = WebCore.CreateWebView(500, 500);
                ipath = ipath ?? "javascript\\index.html";
                _CurrentWebView.Source = new Uri(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath));
                WebView = new AwesomiumWebView(_CurrentWebView);
                var htmlWindowProvider = new AwesomiumTestHTMLWindowProvider(WebView, ipath);
                ViewEngine = new HTMLViewEngine(
                    htmlWindowProvider,
                    _JavascriptUIFrameworkManager
                );
                var viewReadyExecuter = new ViewReadyExecuter(_CurrentWebView, () => { taskLoaded.TrySetResult(null); });
                viewReadyExecuter.Do();
            });

            await taskLoaded.Task;
        }

        public HTMLViewEngine ViewEngine
        {
            get; private set;
        }
 
        public void Dispose()
        {
            WebCore.QueueWork(_CurrentWebView, () => _CurrentWebView.Dispose());      
        }

        public HTML.Core.JavascriptEngine.JavascriptObject.IWebView WebView
        {
            get; private set;
        }
    }
}
﻿using MVVM.HTML.Core.Infra;

namespace Tests.Infra.HTMLEngineTesterHelper.HtmlContext
{
    public class ConventionalTestHtmlProvider : ITestHtmlProvider 
    {
        public string GetHtlmPath(TestContext context) 
        {
            var relativePath = context.GetDescription();
            return BuildPath(relativePath);
        }

        private string BuildPath(string relative) 
        {
            return $"{GetType().Assembly.GetPath()}\\TestHtml\\{relative}";
        }
    }
}
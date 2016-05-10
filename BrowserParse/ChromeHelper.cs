using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace Hank.BrowserParse
{
    public enum BrowserType
    {
        Chrome,
        Firefox,
        IE
    }
    public class ChromeHelper
    {
        public string MonitorChrome(BrowserType browser)
        {
            if (browser == BrowserType.Chrome)
            {
                //"Chrome_WidgetWin_1"

                Process[] procsChrome = Process.GetProcessesByName("chrome");
                foreach (Process chrome in procsChrome)
                {
                    // the chrome process must have a window
                    if (chrome.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    //AutomationElement elm = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                    //         new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1"));
                    // find the automation element
                    AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);

                    // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
                    AutomationElement elmUrlBar = null;
                    try
                    {
                        // walking path found using inspect.exe (Windows SDK) for Chrome 29.0.1547.76 m (currently the latest stable)
                        var elm1 = elm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                        var elm2 = TreeWalker.ControlViewWalker.GetLastChild(elm1); // I don't know a Condition for this for finding :(
                        var elm3 = elm2.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
                        var elm4 = elm3.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                        elmUrlBar = elm4.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                    }
                    catch
                    {
                        // Chrome has probably changed something, and above walking needs to be modified. :(
                        // put an assertion here or something to make sure you don't miss it
                        continue;
                    }

                    // make sure it's valid
                    if (elmUrlBar == null)
                    {
                        // it's not..
                        continue;
                    }

                    // elmUrlBar is now the URL bar element. we have to make sure that it's out of keyboard focus if we want to get a valid URL
                    if ((bool)elmUrlBar.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
                    {
                        continue;
                    }

                    // there might not be a valid pattern to use, so we have to make sure we have one
                    AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                    if (patterns.Length == 1)
                    {
                        string ret = "";
                        try
                        {
                            ret = ((ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0])).Current.Value;
                        }
                        catch { }
                        if (ret != "")
                        {
                            // must match a domain name (and possibly "https://" in front)
                            if (Regex.IsMatch(ret, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
                            {
                                // prepend http:// to the url, because Chrome hides it if it's not SSL
                                if (!ret.StartsWith("http"))
                                {
                                    ret = "http://" + ret;
                                }
                                return ret;
                            }
                        }
                        continue;
                    }
                }

            }
            else if (browser == BrowserType.Firefox)
            {
                AutomationElement root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

                Condition toolBar = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
                new PropertyCondition(AutomationElement.NameProperty, "Browser tabs"));
                var tool = root.FindFirst(TreeScope.Children, toolBar);

                var tool2 = TreeWalker.ControlViewWalker.GetNextSibling(tool);

                var children = tool2.FindAll(TreeScope.Children, Condition.TrueCondition);

                //foreach (AutomationElement item in children)
                //{
                //    foreach (AutomationElement i in item.FindAll(TreeScope.Children, Condition.TrueCondition))
                //    {
                //        foreach (AutomationElement ii in i.FindAll(TreeScope.Children, Condition.TrueCondition))
                //        {
                //            if (ii.Current.LocalizedControlType == "document")
                //            {
                //                if (!ii.Current.BoundingRectangle.X.ToString().Contains("Infinity"))
                //                {
                //                    ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                //                    var activeUrl = activeTab.Current.Value;
                //                    return activeUrl;
                //                }
                //            }
                //        }
                //    }
                //}
            }
            return "";
        }
    }
}

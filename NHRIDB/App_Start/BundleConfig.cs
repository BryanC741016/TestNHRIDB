using System.Web;
using System.Web.Optimization;

namespace NHRIDB
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jqueryui.js",
                     "~/Scripts/jquery.AshAlom.gaugeMeter-2.0.0.min.js",
                     "~/Scripts/jquery.auto-complete.min.js",//https://goodies.pixabay.com/jquery/auto-complete/demo.html
                 "~/Scripts/ajax.js"
                     ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/sitjs").Include(
                      "~/Scripts/sitjs.js"));
            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/_layout.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/font_notosans.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/jquery.auto-complete.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/daterangepicker.css",
                       "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/site.css",
                        "~/Content/phone.css"));
            
        }
    }
}

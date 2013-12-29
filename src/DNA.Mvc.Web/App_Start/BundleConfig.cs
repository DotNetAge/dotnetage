//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Web;
using System.Web.Optimization;
using System.IO;

namespace DNA.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery.js")
                .Include("~/Scripts/jquery-ui*")
                .Include("~/Scripts/jquery.tmpl*")
                .Include("~/Scripts/jquery.mousewheel.js"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/scripts/taoui/unobtrusive.js",
                        "~/scripts/dna/unobtrusive.js")
                        );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            RegisterScriptBundles(bundles);
            RegisterCssBundles(bundles);

            bundles.Add(new ScriptBundle("~/scripts/ace_css")
.Include("~/scripts/ace/ace.js")
.Include("~/scripts/ace/mode-css.js")
.Include("~/scripts/ace/theme-chrome.js")
);
            //bundles.Add(new ScriptBundle("~/scripts/ace").Include("~/scripts/ace/ace.js")
            //    .Include("~/scripts/ace/mode-scss.js")
            //    .Include("~/scripts/ace/theme-chrome.js"));
            //RegisterContentListBundles(bundles);
        }

        public static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/taoui")
                .Include("~/scripts/taoui/comm.js")
                 .Include("~/scripts/taoui/base.js")
                //.Include("~/scripts/taoui/inputFilter.js")
                 .Include("~/scripts/taoui/textbox.js")
                 .Include("~/scripts/taoui/checkbox.js")
                 .Include("~/scripts/taoui/radio.js")
                 .Include("~/scripts/taoui/treeview.js")
                 .Include("~/scripts/taoui/panel.js")
                 .Include("~/scripts/taoui/datasrc.js")
                 .Include("~/scripts/taoui/listview.js")
                 .Include("~/scripts/taoui/listbox.js")
                 .Include("~/scripts/taoui/menu.js")
                 .Include("~/scripts/taoui/form.js")
                 .Include("~/scripts/taoui/button.js")
                 .Include("~/scripts/taoui/dialog.js")
                 .Include("~/scripts/taoui/tags.js")
                 .Include("~/scripts/taoui/slider.js")
                 .Include("~/scripts/taoui/accordion.js")
                 .Include("~/scripts/taoui/tabstrip.js")
                 .Include("~/scripts/taoui/overlay.js")
                 .Include("~/scripts/taoui/pager.js")
                 .Include("~/scripts/taoui/grid.js")
                 .Include("~/scripts/taoui/datasrcinfo.js")
                 .Include("~/scripts/taoui/rating.js")
                 .Include("~/scripts/taoui/colorpicker.js")
                 .Include("~/scripts/taoui/richtextbox.js")
                 .Include("~/scripts/taoui/dropdown.js")
                 .Include("~/scripts/taoui/editable.js")
                 .Include("~/scripts/taoui/pickers.js")
                 .Include("~/scripts/taoui/tooltip.js")
                 .Include("~/scripts/taoui/uploader.js")
                 .Include("~/scripts/taoui/autocomplete.js")
                 .Include("~/scripts/taoui/contentSlider.js")
                 .Include("~/scripts/taoui/roller.js")
                 .Include("~/scripts/taoui/dropbox.js")
                 .Include("~/scripts/taoui/photoviewer.js"));
            //.Include("~/scripts/taoui/unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/dna")
                .Include("~/scripts/dna/page.js",
                "~/scripts/dna/widget.js",
                "~/scripts/dna/layout.js",
                "~/scripts/dna/zone.js",
                //"~/scripts/dna/widget.js",
                "~/scripts/dna/comments.js",
                //"~/scripts/dna/followbutton.js",
                "~/scripts/dna/app.js"));
            //.Include("~/scripts/dna/unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/design").Include("~/scripts/jquery.farbtastic.mini.js", "~/scripts/dna/designers.js"));

            //Register the scripts in themes
            var themesPath = HttpContext.Current.Server.MapPath("~/content/themes/");
            var themes = Directory.GetDirectories(themesPath);
            foreach (var tpath in themes)
            {
                var dir = new DirectoryInfo(tpath);
                if (dir.Name.Equals("base", System.StringComparison.OrdinalIgnoreCase))
                    continue;
                if (Directory.Exists(Path.Combine(tpath, "scripts")))
                {
                    bundles.Add(new ScriptBundle(string.Format("~/bundles/themes/{0}", dir.Name.ToLower()))
                        .Include(string.Format("~/content/themes/{0}/scripts/*.js", dir.Name.ToLower())));
                }
            }
        }

        public static void RegisterCssBundles(BundleCollection bundles)
        {
            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/dashboard/css").Include("~/Content/css/dashboard.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                //"~/Content/themes/base/jquery.ui.accordion.css",
                //"~/Content/themes/base/jquery.ui.autocomplete.css",
                //"~/Content/themes/base/jquery.ui.button.css",
                //"~/Content/themes/base/jquery.ui.dialog.css",
                // "~/Content/themes/base/jquery.ui.slider.css",
                // "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/tao.ui.icons.css",
                        "~/Content/themes/base/tao.css",
                        "~/Content/themes/base/tao.ui.button.css",
                        "~/Content/themes/base/tao.ui.textbox.css",
                        "~/Content/themes/base/tao.ui.tag.css",
                        "~/Content/themes/base/tao.ui.colorpicker.css",
                        "~/Content/themes/base/tao.ui.progress.css",
                        "~/Content/themes/base/tao.ui.listbox.css",
                        "~/Content/themes/base/tao.ui.accordion.css",
                        "~/Content/themes/base/tao.ui.tabs.css",
                        "~/Content/themes/base/tao.ui.slider.css",
                        "~/Content/themes/base/tao.ui.panel.css",
                        "~/Content/themes/base/tao.ui.tree.css",
                        "~/Content/themes/base/tao.ui.grid.css",
                        "~/Content/themes/base/tao.ui.checkbox.css",
                        "~/Content/themes/base/tao.ui.radio.css",
                        "~/Content/themes/base/tao.ui.photoviewer.css",
                        "~/Content/themes/base/tao.ui.menu.css",
                        "~/Content/themes/base/tao.ui.scrollable.css",
                        "~/Content/themes/base/tao.ui.rte.css",
                        "~/Content/themes/base/tao.ui.roller.css",
                        "~/Content/themes/base/tao.ui.pager.css",
                        "~/Content/themes/base/tao.ui.dropbox.css",
                        "~/Content/themes/base/tao.ui.file.css",
                        "~/Content/themes/base/tao.ui.contentslider.css",
                        "~/Content/themes/base/tao.ui.dialog.css",
                //"~/Content/themes/base/tao.ui.video.css",
                        "~/Content/themes/base/tao.ui.rating.css",
                        "~/Content/themes/base/tao.ui.tooltip.css",
                        "~/Content/themes/base/site.css",
                        "~/Content/themes/base/page.layout.css",
                        "~/Content/themes/base/dna.widget.css",
                        "~/Content/themes/base/dna.cscode.css",
                        "~/Content/themes/base/dna.comments.css",
                        "~/Content/themes/base/dna.views.css",
                        "~/Content/themes/base/tao.ui.helpers.css"
                        ));

            //bundles.Add(new StyleBundle("~/Content/themes/default/css").Include("~/Content/themes/default/*.css"));
            // bundles.Add(new StyleBundle("~/Content/themes/base/css"));

            var themesPath = HttpContext.Current.Server.MapPath("~/content/themes/");
            var themes = Directory.GetDirectories(themesPath);
            foreach (var tpath in themes)
            {
                var dir = new DirectoryInfo(tpath);
                if (dir.Name.Equals("base", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                bundles.Add(new StyleBundle(string.Format("~/content/themes/{0}/css", dir.Name.ToLower())).Include(
                    string.Format("~/content/themes/{0}/jquery.ui*", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-font.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-font-768.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-font-600.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-font-400.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-widget-common.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-widget-header.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-widget-content.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-states-hover.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-states-active.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-states-disable.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/taoui-states-error.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-menu-pattern.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-menu-custom.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-pattern.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-colors.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-layouts.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-layouts-768.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-layouts-600.css", dir.Name.ToLower()),
                    string.Format("~/content/themes/{0}/dna-page-layouts-400.css", dir.Name.ToLower())
                  ));
            }
        }

        public static void RegisterContentListBundles(BundleCollection bundles)
        {
            var typePath = HttpContext.Current.Server.MapPath("~/content/types");
            var root = new DirectoryInfo(typePath);
            var typeDirs = root.GetDirectories();

            foreach (var sub in typeDirs)
            {
                var themePath = sub.FullName + (sub.FullName.EndsWith("\\") ? "" : "\\") + "theme";
                var themeDir = new DirectoryInfo(themePath);
                if (themeDir.Exists)
                {
                    var themes = themeDir.GetDirectories();
                    foreach (var theme in themes)
                    {
                        if (theme.Name.Equals("base", System.StringComparison.OrdinalIgnoreCase))
                        {
                            //Register base css
                            bundles.Add(new StyleBundle("~/Content/themes/base/" + sub.Name + "/css").Include(
                                "~/content/types/" + sub.Name + "/themes/base/*"
                                ));
                        }
                        else
                        {
                            //Register theme css
                            bundles.Add(new StyleBundle("~/Content/themes/" + theme.Name + "/css").Include(
                                "~/content/types/" + sub.Name + "/themes/" + theme.Name + "/*"
                                ));
                        }
                    }
                }
            }
        }
    }
}
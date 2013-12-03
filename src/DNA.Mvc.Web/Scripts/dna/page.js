/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {

    $.resolveUrl = function (url) {
        var _appPath = "/";

        if (window.appPath)
            _appPath = window.appPath;

        if ($("body").data("root"))
            _appPath = $("body").data("root");

        if (url.startsWith("~/"))
            return url.replace("~/", _appPath);
    };

    $.res = function (key, def) {
        return (res != undefined && res[key]) ? res[key] : def;
    };

    $.widget("dna.page", {
        options: {
            id: 0,
            container: ".d-page",
            change: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._layoutchanged = false;
            this._contentchanged = false;
            this._layoutmpl = null;
            this._mode = false;
            this._widgetsloaded = false;

            // $("body").on("layoutchange", function (event, ui) {
            //self._layoutchanged = ui.layout;
            //self._layoutmpl = ui.tmpl;
            //self.saveLayouts();
            // }).on("click", function () {
            // $(".d-widget.d-state-active").isActive(false);
            // });

            if (opts.change)
                el.bind(eventPrefix + "change", opts.change);
            //          el.taoTooltip();
            //  this.getTopLayout().css("height","auto");
            //this._attachContext();
            return el;
        },
        applyfor: function (t) {
            $.loading();
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/pages/applyfor/" + this.options.id), { scope: t }, function () {
                $.loading("hide");
            });
        },
        savestyle: function (v) {
            $.loading();
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/pages/savestyle/" + this.options.id), { text: v }, function () {
                $.loading("hide");
                var sheet = $("#inpagestyle", document);
                if (sheet.length == 0)
                    sheet = $("<style id='inpagestyle' type='text/css' />");
                sheet.text(v);
            });
        },
        design: function (mode) {

            if (mode == false) {
                if (this._mode == "contents") this._unloadzones();
                $(document).off("mouseup")
                                     .off("mousemove");
            } else {
                this.loadwidgets();
            }
            this._mode = mode;
            $(document).trigger("designpage", mode);
        },
        saveLayouts: function () {
            var tmpdiv = $("<div/>").appendTo("body").hide(),
                widgets = $(".d-widget").appendTo(tmpdiv),
                layoutContainer = $(".d-state-design");

            $("[data-role=layout]", layoutContainer).layout("destroy");

            var _copy = layoutContainer.clone(false),
                _layoutHtml = "";

            _copy.removeAttr("style");
            $(".d-widget-zone-design", _copy).removeClass("d-widget-zone-design");
            $(".d-layout", _copy).removeClass("d-layout");
            $(".ui-sortable", _copy).removeClass("ui-sortable");
            $(".ui-resizable", _copy).removeClass("ui-resizable");
            $(".ui-droppable", _copy).removeClass("ui-droppable");
            $(".d-state-selected", _copy).removeClass("d-state-selected");
            _layoutHtml = _copy.html();
            //console.log(_copy.html());
            _copy.remove();

            $("body>.d-page").remove();
            $(".d-state-design>.d-page").appendTo("body").removeAttr("style");

            $("[data-role=widgetzone]").each(function (i, zone) {
                $(zone).widgetZone();
                $(".d-widget[data-zone=" + $(zone).attr("id") + "]").appendTo(zone);
            });

            $("body>footer").appendTo("body");
            $("body>.d-state-design").remove();
            $.closePanels();

            //Get the lost widgets
            if (tmpdiv.children().length) {
                var lastZone = $("[data-role=widgetzone]:last"),
                    lastZoneID = lastZone.attr("id");

                tmpdiv.children().each(function (i, lw) {
                    //move widget to new zone
                    $(lw).appendTo(lastZone).attr("data-zone", lastZone.attr("id"));
                });
                tmpdiv.children().appendTo(lastZone);
            }

            //Save the layout
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/pages/applylayout/" + $("body").data("id") + "?locale=" + $("body").attr("lang")), {
                name: "",  // self._layoutmpl ? self._layoutmpl : "",
                data: _layoutHtml, // _layoutHtml ? encodeURI(_layoutHtml) : "",
                dir: "lrt",  // $('#page_direction').taoRadios('option', 'value'),
                viewmode: "" // $('#page_viewMode').taoRadios('option', 'value')
            }, function () {
            });
        },
        setPageHtml: function (val, dir) {
            return $.post($.resolveUrl("~/api/" + $("body").data("web") + "/pages/applylayout/" + $("body").data("id") + "?locale=" + $("body").attr("lang")), {
                name: "",
                data: val,
                dir: dir ? dir : "lrt",
                viewmode: ""
            });
        },
        designLayouts: function () {
            var layoutContainer = $("<div/>").addClass("d-state-design")
                                                       .appendTo("body")
                                                       .height($(document).height())
                                                       .width($(document).width() - 342)
                                                       .css("z-index", $.topMostIndex());

            $(".d-page").clone(false)
                                .appendTo(layoutContainer);

            $("footer", layoutContainer).remove();

            $("[data-role=widget]", layoutContainer).remove();
            $("[data-role=layout]", layoutContainer).layout();
            $.closePanels();
            $("#layouttools_panel").taoPanel("open");
            this.enableLayoutTmpls();
        },
        enableLayoutTmpls: function () {
            $(".d-layout-tmpl").draggable({
                appendTo: "body",
                cursor: "move",
                cursorAt: { "left": 30, "top": 15 },
                helper: function () {
                    //console.log($(this));
                    var _shadowCopy = $(this).clone();
                    _shadowCopy.children("span").remove();
                    return $("<div/>").addClass("d-layout-tmpl-helper").html("<span>Add </span>" + _shadowCopy.html() + " <span style='margin-right:5px;'>to</span><span data-name='targetholder'> ... </span>").css("z-index", $.topMostIndex());
                }
            });
        },
        cancelLayouts: function () {
            $("body>.d-state-design").remove();
            $.closePanels();
        },
        loadTmpl: function (layoutId) {
            ///<summary>Load page layout view template from server</summary>
            var self = this;
            $.loading();
            $(this.options.container).load($.resolveUrl("~/dynamicui/layout/" + layoutId + "?locale=" + $("body").attr("lang")), function () {
                $.loading("hide");
                self._layoutmpl = layoutId;

                self.design("layouts");
                //self._triggerEvent("change");
                $("body").trigger("layoutchange");
                self._layoutchanged = false;
            });
        },
        loadwidgets: function () {
            var self = this;
            ///<summary>Load all widgets then enabled the widgetzones and widgets</summary>
            //$("body").blockUI();
            $.get($.resolveUrl("~/api/" + $("body").data("web") + "/widgets?id=" + $("body").data("id")), function (data) {
                // $("body").unblockUI();
                var zones = $("[data-role='widgetzone']");
                zones.each(function (i, n) {
                    var zone = $(n),
                        zoneID = zone.attr("id"),
                        widgetsInzone = [];

                    $.each(data, function (i, w) {
                        if (w.zone == zoneID)
                            widgetsInzone.push(w);
                    });

                    var sortedWidgets = widgetsInzone.sort(function (w1, w2) {
                        return w1.pos > w2.pos ? 1 : -1;
                    });

                    zone.empty();
                    $.each(sortedWidgets, function (i, sw) {
                        $("<div id='widget_" + sw.id + "'/>").attr("data-role", "widget").appendTo(zone).widget(sw);
                    });
                    zone.widgetZone();
                });
                self._widgetsloaded = true;
            });
            //this._contextDlg();
            return this;
        },
        getTopLayout: function () {
            var layouts = $("[data-role='layout']"), _topLayout = null;
            _topLayout = $("[data-role=layout]:first", $("body"));
            return _topLayout;
        },
        styleChanged: function () {
            this._layoutchanged = true;
            $("#btnSavePageChange").isDisable(false);
        },
        reset: function () {
            $.loading();
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/pages/reset/" + this.options.id), { includeLayout: true }, function () {
                location.reload();
            });
        },
        _unloadlayouts: function () {
            $("body").removeClass("d-state-design");
            $("[data-role='layout']").layout("destroy");
            $("[data-role='widgetzone']").off("click");
        },
        _unloadzones: function () {
            $("[data-role='widgetzone']").widgetZone("destroy");
            $("[data-role='widget']").widget("destroy");
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        destroy: function () {
            this.element.unbind(this.widgetEventPrefix + "change");
            $("body").off("layoutchange").off("click");
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);
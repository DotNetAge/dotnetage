function change_device() {
    var name = $("#devices_menu").find("[data-group=devices][data-checked]").data("name"),
        mode = $("#devices_menu").find("[data-group=dir][data-checked]").data("val");

    if (name == "pc") {
        $("#preview_frame").css({
            top: "0px",
            left: "0px",
            width: "100%",
            height: "100%",
            "background": "none",
            "background-color": "#fff",
            "padding": "0px",
            "margin": "0px"
        });

        $("#designer_toolbar").animate({ "margin-left": "0px" });
        $("body").css("overflow", "hidden");
    }

    if (name == "pad") {
        if (mode == "portrait") {
            $("#preview_frame").css({
                "padding-top": "138px",
                "padding-left": "42px",
                "padding-right": "42px",
                "padding-bottom": "42px",
                "height": "929px",
                "width": "768px",
                "margin": "auto",
                "top": "0px",
                "left": "0px",
                "background-image": "url(/content/images/ipad-portrait.png)",
                "background-color": "#000"
            });
            $("#designer_toolbar").animate({ "margin-left": "852px" });
            $("body").css("overflow", "auto");
        }
        else {
            $("#preview_frame").css({
                "padding-top": "138px",
                "padding-left": "42px",
                "padding-right": "42px",
                "padding-bottom": "42px",
                "width": "1024px",
                "height": "672px",
                "margin": "auto",
                "background-image": "url(/content/images/ipad-landscape.png)",
                "background-color": "#000"
            }).position({
                of: $("body"),
                at: "center top",
                my: "center top",
                offset: "0 10px"
            });
            $("#designer_toolbar").animate({ "margin-left": "0px" });
        }

        $("body").css({ "background-color": "#000" });
    }

    if (name == "phone") {
        if (mode == "portrait") {
            $("#preview_frame").css({
                "padding-top": "198px",
                "padding-left": "24px",
                "padding-right": "22px",
                "padding-bottom": "161px",
                "height": "357px",
                "width": "320px",
                "margin": "auto",
                "top": "0px",
                "left": "0px",
                "background-image": "url(/content/images/iphone-portrait.png)",
                "background-color": "#000"
            }).position({
                of: $("body"),
                at: "center top",
                my: "center top",
                offset: "0 10px"
            });
            $("#designer_toolbar").animate({ "margin-left": "0px" });
        }
        else {
            $("#preview_frame").css({
                "padding-top": "101px",
                "padding-left": "118px",
                "padding-right": "116px",
                "padding-bottom": "56px",
                "width": "480px",
                "height": "208px",
                "margin": "auto",
                "background-image": "url(/content/images/iphone-landscape.png)",
                "background-color": "#000"
            }).position({
                of: $("body"),
                at: "center top",
                my: "center top",
                offset: "0 20px"
            });
            $("#designer_toolbar").animate({ "margin-left": "0px" });
        }

        $("body").css({ "background-color": "#000" });
    }
}

function _setglobalfont(val, key) {
    var _import = "";

    if (key)
        _import = "@import url(http://fonts.googleapis.com/css?family=" + key + ");\t\n";

    $("#dna-font", _getdoc()).text(_import + "body,.d-ui-widget, .d-ui-widget-header, .d-ui-widget-content, .d-content-text, .ui-widget, .ui-widget-content,.d-input > input, .d-picker > input, .d-button, .d-pager, .d-checkbox, .d-radio,.d-rte textarea { font-family:" + val + "; }");
}

function _loadtaoui_colors(_name) {
    var doc = _getdoc();
    $.get("/builder/loadcss?name=" + _name, function (css) {
        $.each(css.styles, function (i, _style) {
            var _name = "";
            if (_style.name == "widgets_theme")
                _name = "taoui-widget-common";
            if (_style.name == "header_theme")
                _name = "taoui-widget-header";
            if (_style.name == "content_theme")
                _name = "taoui-widget-content";
            if (_style.name == "hover_states_theme")
                _name = "taoui-states-hover";
            if (_style.name == "active_states_theme")
                _name = "taoui-states-active";
            if (_style.name == "disable_states_theme")
                _name = "taoui-states-disable";
            if (_style.name == "error_states_theme")
                _name = "taoui-states-error";

            $("#" + _name, doc).html(_style.value);
        });
        //$("#taoui_theme", _getdoc()).text(data);
        // + '\t\n' + 'a,a:visited,a:link,.d-link {color:' + item.find('[data-name=link]').css('background-color') + '}' $(".d-ui-widget-content", _getdoc()).css("color");
    });

}

function _getdoc() { return $("#preview_frame")[0].contentWindow.document; }

function _setsitepattern(val) {
    var doc = _getdoc();
    $.ajax(val).done(function (htm) {
        $("#dna-page-pattern", doc).html(htm);
        //$("#preview_frame")[0].contentWindow._refresh();
    });
}

function _setmenupattern(val) {
    var menu_stylesheet = $("#dna-menu-pattern", _getdoc());
    menu_stylesheet.load(val, function () {
        var text = menu_stylesheet.text(),
        styleText = text.replace(/\/\*#name#\*\//g, ".d-mainmenu");
        menu_stylesheet.text(styleText);

        // $("#preview_frame")[0].contentWindow._refresh();
        _readmenustyle();
    });
}

function _readmenustyle() {
    $("#menus_panel .d-style-editor").each(function (i, editor) {
        editor.refresh();
    });
}

function __loadthemefromlocal() {
    if (window.localStorage) {
        var themeArgStr = window.localStorage.getItem("dna_custom_theme");

        if (themeArgStr) {
            var themeArgs = $.parseJSON(themeArgStr);
            $.each(themeArgs, function (i, sheetObj) {
                $("#" + sheetObj.id, _getdoc()).text(sheetObj.text);
            });
            // $("#preview_frame")[0].contentWindow._refresh();
            _load_layout_colors();
        }
    }
}

function __savethemetolocal() {
    if (window.localStorage) {
        var customstyles = [];

        $("style[id]", _getdoc()).each(function (i, sheet) {
            var name = $(sheet).attr("id");
            customstyles.push({ id: name, text: $(sheet).text() });
        });

        window.localStorage.setItem("dna_custom_theme", JSON.stringify(customstyles));
    }
}

function __createSectionTools(sectionDat) {
    //$.closePanels();
    sectionDat.suffix = _getMediaSuffix();
    var sui = $(sectionDat.tmpl ? ("#" + sectionDat.tmpl) : "#layout_tmpl").tmpl(sectionDat).appendTo("body").css({ right: "-400px" });

    sui.taoPanel({
        autoRelease: true,
        opened: true
        //,close: function () { // $('#layout_panel').taoPanel('open');}
    });
    sui.taoUI();
    sui.unobtrusive_editors();
}

function __openpanel(name) {
    $.closePanels();
    $("#" + name).taoPanel("open");
}

function _open_fontsettings(fkey, fName) {
    var fontPane = $("#fontsettingpane"),
        cbFamily = $("#font_family"),
     _animate = function () {
         $("#fontsettingpane").css({ "z-index": $.topMostIndex() + 1 }).stop(true, false).animate({ "left": "0px" }, 200);
         $("#font_preview_container").width($("body").width() - 300).css({ "z-index": $.topMostIndex() + 1 }).stop(true, false).animate({ "left": "300px" }, 200);
     },
     _doc = $("#preview_frame")[0].contentWindow.document;
    //console.log("Key:"+fkey+" Name:"+fName);
    //fkey = cbFamily.taoComboBox("option", "text"),
    //fName = cbFamily.taoComboBox("option", "value");

    if (!fkey && !fName) {
        // cbFamily.taoComboBox("open");
        return;
    }

    if (fontPane.length == 0) {
        $.loading();
        var fd = $("#font_tmpl").tmpl({ suffix: _getMediaSuffix() }).appendTo("body").hide();
        fd.height($("body").height());
        fd.css({ "z-index": $.topMostIndex() });
        $("#btnfontsettings>span").removeClass("d-icon-cog").addClass("d-icon-loading");

        $("#font_preview").attr("src", "/builder/fonts")
                                       .bind("load", function () {
                                           fontPane = $("#fontsettingpane");
                                           var previewDoc = $("#font_preview")[0].contentWindow.document;
                                           if (fkey) {
                                               $("<link/>").attr("href", "http://fonts.googleapis.com/css?family=" + fkey)
                                                   .attr("rel", "stylesheet")
                                                   .attr("type", "text/css")
                                                   .prependTo($("head", previewDoc));
                                           }

                                           fd.taoUI();
                                           $("#dna-font", previewDoc).text($("#dna-font", _doc).text());

                                           //$("#btnfontsettings>span").removeClass("d-icon-loading").addClass("d-icon-cog");
                                           fd.show();
                                           fontPane.unobtrusive_editors();
                                           _animate();
                                           $.loading("hide");
                                       });
    } else {
        $.loading();
        var previewDoc = $("#font_preview")[0].contentWindow.document;
        fontPane = $("#fontsettingpane");

        if (fkey) {
            $("<link/>").attr("href", "http://fonts.googleapis.com/css?family=" + fkey)
               .attr("rel", "stylesheet")
               .attr("type", "text/css")
               .prependTo($("head", previewDoc));
        }
        $("#dna-font" + _getMediaSuffix(), previewDoc).text($("#dna-font" + _getMediaSuffix(), _doc).text());
        $(".d-style-editor", fontPane).each(function (i, editor) { editor.refresh(); });
        _animate();
        $.loading("hide");
    }
}

function _close_fontsettings() {
    $("#fontsettingpane").stop(true, false).animate({ "left": "-300px" }, 200);
    $("#font_preview_container").stop(true, false).animate({ "left": $("body").width() + "px" }, 200);
}

function _save_fontsettings() {
    $("#dna-font" + _getMediaSuffix(), _getdoc()).text($("#dna-font" + _getMediaSuffix(), $("#font_preview")[0].contentWindow.document).text());
    _close_fontsettings();
}

function _open_taouisettings() {
    var _animate = function () {
        $("#taoui_builder").stop(true, false).css({ "z-index": $.topMostIndex() + 1 })
                                       .animate({ "left": "0px" }, 200);

        $("#taoui_preview_container").width($("body").width() - 300)
                                                           .css({ "z-index": $.topMostIndex() + 1 })
                                                           .stop(true, false)
                                                           .animate({ "left": "300px" }, 200);
    },
        builderPane = $("#taoui_builder");

    if (builderPane.length == 0) {
        var tt = $("#taoui_tmpl").tmpl({}).appendTo("body").hide(),
            _frame = $("#taoui_preview");

        $.loading();
        _frame.bind("load", function () {
            tt.height($("body").height())
              .css({ "z-index": $.topMostIndex() })
              .show();

            var doc = _getdoc(),
                tao_doc = _frame[0].contentWindow.document,
                head = $("head", tao_doc);
            $("style[id$='theme']", tao_doc).remove();

            $("<style id=\"taoui-widget-common\" type=\"text/css\" />").appendTo(head);
            $("<style id=\"taoui-widget-header\" type=\"text/css\" />").appendTo(head);
            $("<style id=\"taoui-widget-content\" type=\"text/css\" />").appendTo(head);

            $("<style id=\"taoui-states-hover\" type=\"text/css\" />").appendTo(head);
            $("<style id=\"taoui-states-active\" type=\"text/css\" />").appendTo(head);
            $("<style id=\"taoui-states-disable\" type=\"text/css\" />").appendTo(head);
            $("<style id=\"taoui-states-error\" type=\"text/css\" />").appendTo(head);

            var taoui_styles = $("style[id^='taoui']", tao_doc);
            taoui_styles.each(function (i, _ts) {
                $(_ts).html($("#" + $(_ts).attr("id"), doc).html());
            });
            //$("#taoui_theme", tao_doc).text($("#taoui_theme", doc).text());
            $("#dna-font", tao_doc).html($("#dna-font", doc).html());

            tt.taoUI();
            tt.unobtrusive_editors();

            //$("#btntaouisettings>span").removeClass("d-icon-loading").addClass("d-icon-cog");

            _animate();
            $.loading("hide");
        }).attr("src", "/builder/colors");

    } else {
        $.loading();
        window.setTimeout(function () {
            var doc = $("#preview_frame")[0].contentWindow.document,
                   tao_doc = $("#taoui_preview")[0].contentWindow.document;

            $("#dna-font", tao_doc).html($("#dna-font", doc).html());

            var taoui_styles = $("style[id^='taoui']", tao_doc);
            taoui_styles.each(function (i, _ts) {
                $(_ts).html($("#" + $(_ts).attr("id"), doc).html());
            });

            $(".d-style-editor", builderPane).each(function (i, editor) {
                editor.refresh();
            });
            $.loading("hide");
            _animate();
        }, 200);
    }
}

function _save_taouisettings() {
    var doc = $("#preview_frame")[0].contentWindow.document,
               tao_doc = $("#taoui_preview")[0].contentWindow.document;

    //$("#taoui_theme", doc).text($("#taoui_theme", tao_doc).text());
    var taoui_styles = $("style[id$=taoui]", tao_doc);
    taoui_styles.each(function (i, _ts) {
        var _id = $(_ts).attr("id");
        $("#" + _id, doc).html($("#" + _id, tao_doc).html());
    });

    _close_taouisettings();
}

function _close_taouisettings() {
    $("#taoui_builder").stop(true, false)
                                   .animate({ "left": "-300px" }, 200);

    $("#taoui_preview_container").width($("body").width() - 300)
                                                    .stop(true, false)
                                                    .animate({ "left": $("body").width() + "px" }, 200);
}

function _setLayoutPickerColor(name, cls, color) {
    var palette_span = $("#colorpickers").find("[data-name=" + name + "]"),
        palette_picker = palette_span.next(),
        layout_preview = $("#layoutcolor_preview").find("[data-name=" + name + "]");;

    if (layout_preview.length)
        layout_preview.css("background", color);

    if (color == "transparent" || color == "")
        palette_span.css({
            "background-image": color ? "url(/content/images/img_transparent.png)" : "url(/content/images/img_nocolor.png)",
            "background-color": "transparent"
        });
    else
        palette_span.css({
            "background-image": "none",
            "background-color": color
        });
    //palette_span.css("background-color", color);
    palette_picker.taoColorpicker("setColor", color);
}

function _selectLayoutColors(item) {
    //console.clear();
    //reset colors
    $("#colorpickers").find("[data-name]").css({
        "background-image": "url(/content/images/img_nocolor.png)",
        "background-color": "transparent"
    });

    $(item).find("[data-name]").each(function (i, cn) {
        var _name = $(cn).data("name"),
            _color = $(cn).data("color"),// $(cn).css("background-color"),
            _span = $("#colorpickers").find("[data-name=" + _name + "]"),
            _cls = _span.length ? _span.data("class") : "";

        //console.log(_color);
        if (_cls) {
            _setLayoutPickerColor(_name, _cls, _color);
            var prop = "background-color";
            if (_name == "link" || _name == "color")
                prop = "color";

            _set_layout_color(_cls, prop, _color);
        }
    });
}

function _set_css_property(target, sheet, name, val) {
    var custom_style = $("#" + sheet, _getdoc()),
        editing_text = custom_style.text(),
        regex = new RegExp(target.replace(".", "\.") + " \{(.+?)\}"),
            _val = name + ":" + val + ";";

    if (editing_text.match(regex)) {
        var hRegx = new RegExp(name + ":(.+?);"),
        results = editing_text.match(regex),
        phCss = results[1];

        if (phCss && phCss.match(hRegx)) {
            phCss = phCss.replace(hRegx, _val);
            editing_text = editing_text.replace(regex, target + " {" + phCss + "}");
        } else
            editing_text = editing_text.replace(regex, target + " {" + (phCss ? (phCss.endsWith(";") ? phCss : (phCss + ";")) : "") + _val + "}");
    }
    else
        editing_text += target + " {" + _val + "}\n";
    console.log(custom_style);
    custom_style.text(editing_text);
}

function _set_palette_background(ele, color) {
    if (color == 'transparent') {
        $(ele).prev().css('background-image', 'url(/content/images/img_transparent.png)');
    } else {
        if (color == "")
            $(ele).prev().css('background-image', 'url(/content/images/img_nocolor.png)');
        else
            $(ele).prev().css('background-image', 'none');
    }
}

function _set_layout_color(target, prop, val) {
    _set_css_property(target, "layout_colors", prop, val);
}

function _load_layout_colors() {
    _setLayoutPickerColor("body", "body", $("body", _getdoc()).css("background-color"));
    _setLayoutPickerColor("page", ".d-page", $(".d-page", _getdoc()).css("background-color"));
    _setLayoutPickerColor("header", ".d-page-header", $(".d-page-header", _getdoc()).css("background-color"));
    _setLayoutPickerColor("sitetools", ".d-sitetools", $(".d-sitetools", _getdoc()).css("background-color"));
    _setLayoutPickerColor("nav", ".d-page-nav", $(".d-page-nav", _getdoc()).css("background-color"));
    _setLayoutPickerColor("content", ".d-page-content", $(".d-page-content", _getdoc()).css("background-color"));
    _setLayoutPickerColor("footer", ".d-page-footer", $(".d-page-footer", _getdoc()).css("background-color"));
}

function _pub_to_clound() {
    var customstyles = [],
        holder = $("#themes_holder"),
        colorsholder = $("#thumbcolors_holder"),
        form = $("#publish_form");

    holder.empty();
    $("style[id]", _getdoc()).each(function (i, sheet) {
        var name = $(sheet).attr("id"),
            input = $("<input type='hidden'/>").attr("name", name).val($(sheet).text()).appendTo(holder);
    });

    $("[name=mode]", form).val($("body", _getdoc()).data("mode"));

    colorsholder.empty();
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.sitetools.color").val($.rgb2hex($(".d-sitetools", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.sitetools.bgcolor").val($.rgb2hex($(".d-sitetools", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.header.color").val($.rgb2hex($(".d-page-header", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.header.bgcolor").val($.rgb2hex($(".d-page-header", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.nav.color").val($.rgb2hex($(".d-page-nav", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.nav.bgcolor").val($.rgb2hex($(".d-page-nav", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.content.color").val($.rgb2hex($(".d-page-content", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.content.bgcolor").val($.rgb2hex($(".d-page-content", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.footer.color").val($.rgb2hex($(".d-page-footer", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.footer.bgcolor").val($.rgb2hex($(".d-page-footer", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.page.color").val($.rgb2hex($(".d-page", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.page.bgcolor").val($.rgb2hex($(".d-page", _getdoc()).css("background-color")));

    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.body.color").val($.rgb2hex($("body", _getdoc()).css("color")));
    $("<input type='hidden' />").appendTo(colorsholder).attr("name", "tc.body.bgcolor").val($.rgb2hex($("body", _getdoc()).css("background-color")));

    form.submit();
}

function _getMediaSuffix(name) {
    var width = $("body").width();
    if (width <= 768 && width > 600)
        return "-768";
    if (width <= 600 && width > 400)
        return "-600";
    if (width <= 400)
        return "-400";
    return "";
}

function prepare_style_elements(configs, curTheme) {
    var _doc = _getdoc(), head = $("head", _doc);
    $("<style id=\"dna-font\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"dna-font-768\" type=\"text/css\"  media=\"screen and (max-width: 768px)\"/>").appendTo(head);
    $("<style id=\"dna-font-600\" type=\"text/css\"  media=\"screen and (max-width: 600px)\"/>").appendTo(head);
    $("<style id=\"dna-font-400\" type=\"text/css\"  media=\"screen and (max-width: 400px)\"/>").appendTo(head);

    $("<style id=\"taoui-widget-common\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"taoui-widget-header\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"taoui-widget-content\" type=\"text/css\" />").appendTo(head);

    $("<style id=\"taoui-states-hover\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"taoui-states-active\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"taoui-states-disable\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"taoui-states-error\" type=\"text/css\" />").appendTo(head);

    $("<style id=\"dna-menu-pattern\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"dna-menu-custom\" type=\"text/css\" />").appendTo(head);

    $("<style id=\"dna-page-pattern\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"dna-page-colors\" type=\"text/css\" />").appendTo(head);

    $("<style id=\"dna-page-layouts\" type=\"text/css\" />").appendTo(head);
    $("<style id=\"dna-page-layouts-768\" type=\"text/css\" media=\"screen and (max-width: 768px)\" />").appendTo(head);
    $("<style id=\"dna-page-layouts-600\" type=\"text/css\" media=\"screen and (max-width: 600px)\" />").appendTo(head);
    $("<style id=\"dna-page-layouts-400\" type=\"text/css\" media=\"screen and (max-width: 400px)\" />").appendTo(head);

    $("[data-role=layout],.d-sitetools", _getdoc()).on("click", function (e) {
        //console.log(e.target);
        if ($(e.target).hasClass("ui-resizable"))
            return;

        if (e.target.tagName != "A") {
            e.stopPropagation();
            e.preventDefault();
            var _title = $(this).data("label");
            if (!_title)
                $(this).attr("title");

            var _selector = $(this).attr("class") ? "." + $(this).attr("class") : null;

            if (!_selector && $(this).attr("id"))
                _selector = "#" + $(this).attr("id");

            // console.log();

            if (_selector) {
                $.loading("Parsing element...");
                window.setTimeout(function () {
                    __createSectionTools({ title: _title, selector: _selector, tmpl: "widget_layout_tmpl" });
                    $.loading("hide");
                }, 200);
            }
        }
    });

    var curCssLinks = $('link[href^="' + curTheme + '"]', _doc);
    //console.log(curCssLinks);
    if (curCssLinks.length)
        curCssLinks.remove();

    for (var key in configs) {
        if ($("#" + key, _doc).length) {
            $("#" + key, _doc).html(configs[key]);
        }
    }

    //$(".d-mainmenu",_doc).resizable({
    //    handle: ["s"],
    //    resize: function (e, ui) {
    //        console.log(ui.size);
    //        _set_css_property(".d-mainmenu.d-menu > .d-item > a", "custom_menu" + _getMediaSuffix(), "line-height:" + ui.size.height + "px");
    //        _set_css_property(".d-mainmenu", "custom_menu" + _getMediaSuffix(), "height:" + ui.size.height + "px");
    //    }
    //,stop: function () {
    //    _refresh();
    //    $("#mainmenu_holder").css("width", "auto");
    //}
    //});
}

//(function ($, window, document, undefined) {
//    themeBuilder = function () { };
//    themeBuilder.prototype = {
//    };
//})(jQuery, window, document);
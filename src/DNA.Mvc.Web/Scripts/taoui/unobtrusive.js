/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($, window, document, undefined) {

    $.fn.taoUI = function () {

        $("img[data-preload=true]").each(function (i, imgEle) {
            var img = $(imgEle);
            img.wrap("<div/>");
            var wrapper = img.parent();
            wrapper.addClass("d-loading").width(img.width).height(img.height);
            var newImg = new Image();
            img.css({ opacity: 0 });
            //console.log(wrapper.width());
            var maxWidth = wrapper.width();
            $(newImg).attr("style", img.attr("style"))
                  .css({ opacity: 0 })
                  .attr("src", img.attr("src"))
                  .bind("load", function () {
                      $(this).animate({ "opacity": "1" }, 500);
                      wrapper.after($(this));
                      $(this).prev().remove();
                      $(img).removeAttr("data-preload");
                      $(img).hide();
                  });
        });

        if ($.fn.taoDataSource)
            $("[data-role='datasource']", this).taoDataSource();

        if ($.fn.taoPager)
            $("[data-role='pager']", this).taoPager();

        if ($.fn.taoDataSourceInfo)
            $("[data-role='datasourceinfo']", this).taoDataSourceInfo();

        if ($.fn.taoForm)
            $("[data-role='form'],form[data-source]", this).taoForm();

        if ($.fn.taoTextbox)
            $("input[type='text'][data-role!='none'],input[type='search'][data-role!='none'],input[type='number'][data-role!='none'],input[type='email'][data-role!='none'],input[type='tel'][data-role!='none'],input[type='url'][data-role!='none'],input[type='password'][data-role!='none'],[data-role='textbox'],[role='textbox']", this).taoTextbox();

        if ($.fn.taoVideo)
            $("[data-role='video']", this).taoVideo();

        if ($.fn.taoProgressbar)
            $("[data-role='progressbar']", this).taoProgressbar();

        if ($.fn.taoSwitcher)
            $("[data-role='switcher']", this).taoSwitcher();

        if ($.fn.taoCheckbox)
            $("input[type='checkbox'][data-role!='none'][data-role!='button'],input[data-role='checkbox']", this).taoCheckbox();

        if ($.fn.taoRadios)
            $("[data-role='radios']", this).taoRadios();

        if ($.fn.taoRadio)
            $("input[type='radio'][data-role!='none'][data-role!='button'],input[data-role='radio']", this).taoRadio();

        if ($.fn.taoTags)
            $("[data-role='tags']", this).taoTags();

        if ($.fn.taoRoller)
            $("[data-role='roller']", this).taoRoller();

        if ($.fn.taoContentSlider)
            $("[data-role='content-slider']", this).taoContentSlider();

        $("textarea:not([data-role])").addClass("d-ui-widget d-ui-widget-content d-textarea")
        .each(function (i, ta) { $(ta).hover(function () { if (!$(this).isDisable()) $(this).isHover(true); }, function () { if (!$(this).isDisable()) $(this).isHover(false); }).bind("focus", function () { if (!$(this).isDisable()) $(this).isActive(true); }).bind("blur", function () { if (!$(this).isDisable()) $(this).isActive(false); }); });

        if ($.fn.taoDropbox)
            $("[data-role='dropbox']", this).taoDropbox();

        if ($.fn.taoTreeview)
            $("[data-role='tree']", this).taoTreeview();

        if ($.fn.taoPanel)
            $("[data-role='panel']", this).taoPanel();

        if ($.fn.taoGrid)
            $("[data-role='grid']", this).taoGrid();

        if ($.fn.taoListview)
            $("[data-role='listview']", this).taoListview();

        if ($.fn.taoListbox)
            $("[data-role='listbox'],[role='listbox']", this).taoListbox();

        //if ($.fn.imageNavigator)
        //    $("[data-role='imgnav']", this).imageNavigator();

        if ($.fn.taoButton)
            $("button[data-role!='none'],[data-role='button'],[role='button'],a[role='link'],input[type='submit'],input[type='reset'],input[type='button']", this).taoButton();

        if ($.fn.taoButtonGroup)
            $("[data-role='buttons']", this).taoButtonGroup();

        if ($.fn.taoMenu)
            $("[data-role='menu'],[data-role='menubar'],[data-role='toolbar']", this).taoMenu();

        //if ($.fn.taoToolbar)
        //    $("[data-role='toolbar']", this).taoToolbar();

        if ($.fn.taoRating)
            $("[data-role='rating']", this).taoRating();

        if ($.fn.taoColorpicker)
            $("[data-role='color']", this).taoColorpicker();

        if ($.fn.taoEditor)
            $("[data-role='editor']", this).taoEditor();

        if ($.fn.taoDropdown)
            $("[data-role='dropdown']", this).taoDropdown();

        if ($.fn.taoComboBox)
            $("[data-role='combobox'],[role='combobox']", this).taoComboBox();

        if ($.fn.taoDatepicker)
            $("[data-role='picker'][type='date'],[data-role='datetime']", this).taoDatepicker();

        if ($.fn.taoColorDropdown)
            $("[data-role='picker'][type='color']", this).taoColorDropdown();

        if ($.fn.taoTimepicker)
            $("[data-role='time']", this).taoTimepicker();

        if ($.fn.taoUploader)
            $("[data-role='uploader']", this).taoUploader();

        if ($.fn.taoUploadInfo)
            $("[data-role='uploadinfo']", this).taoUploadInfo();

        if ($.fn.taoEditable)
            $("[data-editable=true]", this).taoEditable();

        if ($.fn.taoAutoComplete)
            $("[data-autocomplete]", this).taoAutoComplete();

        if ($.fn.taoSlider)
            $("[data-role='slider']", this).taoSlider();

        if ($.fn.draggable) {
            var _draggs = $("[data-draggable='true'][data-role!='panel'][data-role!='content-slider']", this);
            _draggs.each(function (i, _dag) {
                var _d = $(_dag);
                _d.draggable({
                    handle: _d.data("handle") ? _d.data("handle") : false,
                    helper: _d.data("helper") ? _d.data("helper") : "original",
                    addClasses: _d.data("add-classes") != undefined ? _d.dataBool("add-classes") : true,
                    appendTo: _d.data("append-to") ? _d.data("append-to") : "parent",
                    axis: _d.data("axis") ? _d.data("axis") : false,
                    cancel: _d.data("cancel") ? _d.data("cancel") : ":input,option",
                    connectToSortable: _d.data("connect-to") ? _d.data("connect-to") : false,
                    containment: _d.data("containment") ? _d.data("containment") : false,
                    cursor: _d.data("cursor") ? _d.data("cursor") : "auto",
                    cursorAt: _d.data("cursor-at") ? _d.data("cursor-at") : false,
                    delay: _d.data("delay") != undefined ? _d.dataInt("delay") : 0,
                    distance: _d.data("distance") != undefined ? _d.dataInt("distance") : 1,
                    grid: _d.data("grid") ? eval(_d.data("grid")) : false,
                    iframeFix: _d.data("iframe-fix") != undefined ? _d.dataBool("iframe-fix") : false,
                    opacity: _d.data("opacity") != undefined ? _d.dataFloat("opacity") : false,
                    refreshPositions: _d.data("refresh-pos") != undefined ? _d.dataBool("refresh-pos") : false,
                    revert: _d.data("revert") ? _d.data("revert") : false,
                    revertDuration: _d.data("revert-dur") ? _d.dataInt("revert-dur") : 500,
                    scope: _d.data("scope") ? _d.data("scope") : "default",
                    scrollSenitivity: _d.data("scroll-senitivity") != undefined ? _d.dataInt("scroll-senitivity") : 20,
                    scrollSpeed: _d.data("scroll-speed") != undefined ? _d.dataInt("scroll-speed") : 20,
                    snap: _d.data("snap") != undefined ? _d.dataBool("snap") : false,
                    snapMode: _d.data("snap-mode") ? _d.data("snap-mode") : "both",
                    snapTolerance: _d.data("snap-tolerance") != undefined ? _d.dataInt("snap-tolerance") : 20,
                    stack: _d.data("stack") ? _d.data("stack") : false,
                    zIndex: _d.data("zindex") != undefined ? _d.dataInt("zindex") : false,
                    start: _d.data("drag-start") ? new Function("event", "ui", _d.data("drag-start")) : null,
                    stop: _d.data("drag-stop") ? new Function("event", "ui", _d.data("drag-stop")) : null,
                    drag: _d.data("drag") ? new Function("event", "ui", _d.data("drag")) : null
                });
            });
        }

        if ($.fn.resizable) {
            var _resizes = $("[data-resizable='true'][data-role!='dialog'][data-role!='none'][data-role!='editor']", this);
            _resizes.each(function (i, _resize) {
                var r = $(_resize);
                r.resizable({
                    alsoResize: r.data("also-resize") ? r.data("also-resize") : false,
                    animate: r.data("animate") != undefined ? r.dataBool("animate") : false,
                    animateDuration: r.data("animate-dur") != undefined ? r.dataInt("animate-dur") : "slow",
                    animateEasing: r.data("animate-easing") ? r.data("animate-easing") : "swing",
                    aspectRatio: r.data("aspect-ratio") != undefined ? r.dataFloat("aspect-ratio") : false,
                    cancel: r.data("cancel") ? r.data("cancel") : ":input,option",
                    delay: r.data("delay") != undefined ? r.dataInt("delay") : 0,
                    distance: r.data("distance") != undefined ? r.dataInt("distance") : 1,
                    grid: r.data("grid") ? eval(r.data("grid")) : false,
                    containment: r.data("containment") ? r.data("containment") : false,
                    ghost: r.data("ghost") != undefined ? r.dataBool("ghost") : false,
                    handles: r.data("handles") ? r.data("handles") : "e,s,se",
                    helper: r.data("helper") ? r.data("helper") : false,
                    maxHeight: r.data("max-height") != undefined ? r.dataInt("max-height") : null,
                    minHeight: r.data("min-height") != undefined ? r.dataInt("min-height") : 10,
                    maxWidth: r.data("max-width") != undefined ? r.dataInt("max-width") : null,
                    minHeight: r.data("min-width") != undefined ? r.dataInt("min-width") : 10,
                    start: r.data("resize-start") ? new Function("event", "ui", r.data("resize-start")) : null,
                    stop: r.data("resize-stop") ? new Function("event", "ui", r.data("resize-stop")) : null,
                    resize: r.data("resize") ? new Function("event", "ui", r.data("resize")) : null
                });
            });
        }

        $("[data-inline='true']", this).addClass("d-inline");

        $("span[data-icon]", this).each(function (i, icon) {
            var _icon = $(icon).data("icon");
            $(icon).addClass(_icon.startsWith("d-icon") ? _icon : ("d-icon-" + _icon)).css("margin-right", "5px");
            if ("large" == $(icon).data("icon-size")) {
                $(icon).css({
                    "margin-right": "10px",
                    "font-size": "2em"
                });
            }
        });

        $("[data-position]", this).each(function (i, ele) {
            $(ele).css("position", $(ele).data("position"));
        });

        if ($.fn.taoAccordion)
            $("[data-role='accordion']", this).taoAccordion();

        if ($.fn.taoTabs)
            $("[data-role='tabs']", this).taoTabs();

        $("ul[data-display=mobile-list]:not(.d-mobile-list)", this).each(function (i, ml) {
            $(ml).mobilelist();
        });

        $("ul[data-display=button-list]:not(.d-button-list)", this).each(function (i, ml) {
            $(ml).buttonlist();
        });

        $("[data-rel=mailto]", this).on("click", function () {
            var callback = null;
            if ($(this).data("callback"))
                callback = new Function($(this).data("callback"));

            var dlg = $.mailto($(this).data("to"), $(this).attr("title"), function () {
                dlg.taoPanel("close");
                if ($.isFunction(callback))
                    callback();
            });
        });

        $("[data-rel=login]", this).on("click", function () {
            var callback = null;
            if ($(this).data("login-success"))
                callback = new Function($(this).data("login-success"));

            $.login().done(function () {
                if ($.isFunction(callback))
                    callback();
            });
        });

        $("[data-rel=link]", this).each(function (i, link) {
            $(link).unbind("click").click(function (event) {
                event.preventDefault(); event.stopPropagation();
                if ($(link).attr("href") && $(link).attr("data-link-to") == undefined && $(link).attr("href").startsWith("#")) {
                    $(link).attr("data-link-to", $(link).attr("href"));
                    $(link).attr("href", "javascript:void(0);");
                }

                var linkResult = $.linkDialog($(link).data("link-title"), $(link).data("link-to"));
                if ($(link).data("link-select")) {
                    var selectFunc = new Function("data", $(link).data("link-select"));
                    linkResult.done(function (data) {
                        selectFunc(data);
                    });
                }
            });
        });

        $("[data-rel=file]", this).each(function (i, rff) {
            $(rff).unbind("click")
                   .bind("click", function () {
                       var _refs = $(this),
                           _filter = _refs.data('file-filter') ? _refs.data('file-filter') : "",
                           _path = _refs.data("file-path") ? _refs.data("file-path") : "",
                           _valTo = _refs.data("file-to") ? _refs.datajQuery("file-to") : null,
                           _title = _refs.attr("title") ? _refs.attr("title") : "",
                       _selected = _refs.data("file-select") ? new Function("file", _refs.data("file-select")) : null;

                       $.fileDialog(_title, _path, _filter)
                         .done(function (fileUrl) {
                             if (fileUrl && _valTo && _valTo.length)
                                 _valTo.val(fileUrl).trigger("change");
                             if ($.isFunction(_selected)) {
                                 _selected(fileUrl);
                             }
                         });
                   });
        });

        $("[data-rel=folder]", this).each(function (i, fl) {
            $(fl).unbind("click").click(function (event) {
                event.preventDefault();
                event.stopPropagation();
                var folderResult = $.folderDialog($(fl).data("folder-title"), $(fl).data("folder-path"), $(fl).data("folder-to"), $(fl).dataBool("folder-readonly"));
                if ($(fl).data("folder-select")) {
                    var selectFunc = new Function("url", $(fl).data("folder-select"));
                    folderResult.done(function (url) {
                        selectFunc(url);
                    });
                }
            });
        });

        $("[data-role=dialog]", this).each(function (i, dlg) {
            $(dlg).taoDialog();
        });

        $("a[data-rel=dialog]", this).on("click", function (event) {
            event.preventDefault();
            event.stopPropagation();

            var href = $(this).attr("href");
            if (href.startsWith("#")) {
                if ($(href).length)
                    $(href).taoDialog("open");
            } else {

                var self = $(this),
                    id = "dynamic_dialog_" + $(".d-dialog").length + 1,
                 dlgHolder = $("<div id=\"" + id + "\"/>").appendTo("body");

                $(this).attr("href", "#" + id);

                for (var i = 0; i < this.attributes.length; i++) {
                    var attr = this.attributes.item(i);
                    if (attr.name.startsWith("data-dialog-"))
                        dlgHolder.attr(attr.name.replace("data-dialog-", "data-"), attr.value);
                }

                dlgHolder.taoDialog({ url: href });

                if (dlgHolder.taoDialog("option", "cache") == false) {
                    dlgHolder.data("href", href);
                    dlgHolder.bind("taoDialogclose", function () {
                        self.attr("href", href);
                    });
                }

            }
        });

        $("a[data-rel=popup]", this).each(function (i, link) {
            var _rel = $(link).attr("href"), _target = null;

            if (_rel.startsWith("#")) {
                _target = $(_rel);
            } else {
                _target = $("<ul/>").appendTo("body").width($(link).data("popup-width") ? $(link).data("popup-width") : 120).hide();
                $.get(_rel, function (data) {
                    $.each(data, function (i, dat) {
                        var _item = $("<li/>").appendTo(_target),
                            _prop = $(link).attr("data-popup-label") ? $(link).attr("data-popup-label") : "title";

                        for (var prop in dat)
                            _item.attr("data-" + prop, dat[prop]);

                        if (dat[_prop])
                            $("<a/>").text(dat[_prop])
                                           .attr("href", "javascript:void(0);")
                                           .appendTo(_item);

                    });

                    _target.taoMenu({
                        type: "vertical",
                        itemClick: function (event, ui) {
                            $(link).taoButton("option", "label", ui.item.text());
                            if ($(link).attr("data-popup-command")) {
                                var cmd = new Function("item", $(link).attr("data-popup-command"));
                                if ($.isFunction(cmd))
                                    cmd(ui.item);
                            }
                        }
                    });
                });
            }

            if (_target.length) {
                _target.appendTo("body")
                           .css({ "position": "absolute" })
                           .hide();

                if (_target.hasClass("d-menu")) {
                    _target.bind("taoMenuitemClick", function (event, ui) {
                        $(link).taoButton("option", "label", ui.item.text());
                        if ($(link).attr("data-popup-command")) {
                            var cmd = new Function("item", $(link).attr("data-popup-command"));
                            if ($.isFunction(cmd))
                                cmd(ui.item);
                        }
                    });
                }
            }

            $("body").on("click", function () { _target.hide(); });

            $(link).attr("href", "javascript:void(0);")
                      .on("click", function (event) {
                          event.preventDefault();
                          event.stopPropagation();
                          if (_target.isVisible()) {
                              _target.hide();
                          } else {
                              _target.show()
                                         .position({
                                             of: $(link),
                                             my: "left top",
                                             at: "left bottom"
                                         });
                          }
                      });
        });

        $("a[data-rel=panel]", this).on("click", function (event) {
            event.preventDefault();
            event.stopPropagation();
            $.closePanels();

            var href = $(this).attr("href");

            if (href == undefined) return;
            if (href.startsWith("#")) {
                if ($(href).length) {
                    $(href).taoPanel("open");
                }
            } else {
                var self = $(this),
                   id = "dynamic_panel_" + $(".d-panel").length + 1,
                panel = $("<div id=\"" + id + "\"/>").appendTo("body");

                for (var i = 0; i < this.attributes.length; i++) {
                    var attr = this.attributes.item(i);
                    if (attr.name.startsWith("data-panel-"))
                        panel.attr(attr.name.replace("data-panel-", "data-"), attr.value);
                }
                //var contentUrl = $(this).data("url");
                //if (this.tagName == "A" && $(this).attr("href") && $(this).attr("href") != "#" && $(this).attr("href") != "javascript:void(0);") {
                contentUrl = $(this).attr("href");
                $(this).attr("href", "#" + id);

                if ($(this).closest(".d-panel").length && !$(this).attr("data-panel-return") && !panel.attr("data-return")) {
                    var parentPanel = $(this).closest(".d-panel"),
                    parentPanelID = parentPanel.attr("id");
                    if (parentPanelID)
                        panel.attr("data-return", "#" + parentPanelID);
                }
                //}

                if ($(this).data("panel-display") == "overlay")
                    panel.css("position", "fixed");

                panel.taoPanel({
                    contentUrl: contentUrl,
                    opened: true
                });

                if (panel.taoPanel("option", "autoRelease") == true) {
                    panel.data("href", href);
                    panel.bind("taoPanelclose", function () {
                        self.attr("href", panel.data("href"));
                    });
                }
            }
        });

        $("input[data-auto-select=true],input[data-auto-select=true]", this).live("focus", function () { $(this).select(); });

        if ($.fn.taoPhotoViewer) {
            $("[data-viewer=true]").each(function (i, _pv) {
                $(_pv).on("click", function (event) {
                    event.preventDefault();
                    if (_pv.tagName == "A") {
                        var src = $(this).attr("href");
                        $(_pv).taoPhotoViewer({ src: src });
                    } else
                        $(_pv).taoPhotoViewer();
                });

            });
        }

        if ($.fn.followButton)
            $("[data-role='follow']", this).followButton();

        if ($.fn.commentList)
            $("[data-role='comments']", this).commentList();

        //if ($.fn.quickComment)
        //    $("[data-role='commenteditor']", this).quickComment();

        if ($.fn.commentbox)
            $("[data-role='commentbox']", this).commentbox();

        //if ($.fn.taoTooltip)
        //    $("[title][data-role!='dialog']:not(.d-state-disable):not([disabled=disabled]),[data-tooltip-url]:not(.d-state-disable):not([disabled=disabled]),[data-tooltip-content]:not(.d-state-disable):not([disabled=disabled])", this).taoTooltip();

        $("img").error(function () {
            $(this).hide();
        });

        //$("[data-role='codeviewer']").each(function (i, viewer) {
        //    //console.log($(viewer).text());
        //});

        $("[data-corner]").each(function (i, ele) {
            var corner = $(ele).data("corner");
            if (corner == "true" && corner != undefined)
                $(ele).addClass("d-corner")
            else {
                var cv = parseInt(corner);
                if (cv != NaN) {
                    $(ele).css({
                        "border-radius": cv + "px",
                        "-webkit-border-radius": cv + "px",
                        "-moz-border-radius": cv + "px"
                    });
                }
            }
        });

        $("[data-code-file]").each(function (i, code) {
            $(this).load($(this).data("code-file") + "?format=code");
        });

        $("[data-rel=code]").each(function (i, code) {
            var lang = "html", title = "View source", codeText = "View source", content = $(this).html();
            if (content) {
                if ($(this).data("code-lang")) lang = $(this).data("code-lang");
                if ($(this).data("code-title")) title = $(this).data("code-title");
                if ($(this).data("code-button")) codeText = $(this).data("code-button");
                var btn = $("<a/>").appendTo("body").text(codeText);
                $(this).after(btn);
                btn.taoButton();
                btn.click(function () {
                    $.editCode(title, content, null, lang);
                });
            }
        });

        $("[data-shadow]").each(function (i, ele) {
            var _shadow = $(ele).data("shadow");
            if (_shadow == "true" && _shadow != undefined)
                $(ele).addClass("d-shadow d-shadow-around")
            else
                $(ele).addClass("d-shadow d-shadow-" + _shadow);
        });
    };

})(jQuery, window, document);

$(function () {
    if (!window.$T)
        window.$T = {};
    $T.init = function (_parent) {
        if (_parent)
            $(_parent).taoUI();
        else
            $("body").taoUI();
    };

    if ($("body").data("taoui-unobtrusive") != false)
        $T.init();

    if ($("body").data("scroll-helper") != false)
        $(document).bind("scroll", function (event) {
            if ($(".d-overlay").length)
                return;

            var scrollHelper = $("body .d-scroller");
            if (scrollHelper.length == 0) {
                scrollHelper = $("<div/>").addClass("d-ui-widget d-scroller d-tran-fast").append($("<span/>").addClass("d-icon-arrow-up-2")).appendTo("body");
                scrollHelper.click(function () {
                    $("body,html").animate({ scrollTop: "0px" }, 500);
                }).height(0);
            }

            if ($(document).scrollTop()) {
                scrollHelper.css("zIndex", $.topMostIndex());
                if (scrollHelper.height() < 5)
                    scrollHelper.height(50);
            } else {
                if (scrollHelper.height() > 5)
                    scrollHelper.height(0);
            }

            //Add scroll fixed
            var _pins = $("[data-auto-fixed='true']");

            if (_pins.length) {
                var bodyHeight = $("body").height(), st = $(document).scrollTop();
                //Find the current fixed
                var _pin = $(".d-fixed-flag[data-auto-fixed='true']"),
                    _createPlaceholder = function (el) {
                        _removePlaceholder();
                        var holder = $("<div/>").appendTo("body").addClass("d-fixed").addClass("d-auto-fixed-holder");
                        $(el).clone().appendTo(holder);
                    },
                    _removePlaceholder = function () {
                        if ($(".d-auto-fixed-holder").length)
                            $(".d-auto-fixed-holder").remove();
                    };

                if (_pin.length) {
                    //The page has a pinner
                    var t = _pin.dataInt("fixed-top");
                    if (st < t) {
                        _pin.removeClass("d-fixed-flag").removeAttr("data-fixed-top");
                        _removePlaceholder();
                    }
                    else {
                        var nextIndex = $("[data-auto-fixed='true']").index(_pin) + 1;
                        nextPin = $("[data-auto-fixed='true']").get(nextIndex);

                        if (nextPin != undefined && nextPin != null) {
                            var nextTop = $(nextPin).position().top;

                            if (nextTop < (bodyHeight + st)) {
                                if (st > nextTop) {
                                    _pin.removeClass("d-fixed-flag").removeAttr("data-fixed-top").css("top", "0px");
                                    $(nextPin).addClass("d-fixed-flag").data("fixed-top", nextTop);
                                    _createPlaceholder($(nextPin));
                                }
                                else {
                                    _removePlaceholder();
                                }
                            }
                        }
                        else
                            _removePlaceholder();

                    }
                } else { //Nothing on pin
                    //1.Find the first pinner
                    var _pin = _pins.first(), t = _pin.position().top;
                    if (st >= t) {
                        _pin.addClass("d-fixed-flag").data("fixed-top", t);
                        _createPlaceholder(_pin);
                    }
                }
            }

        });

    $("body").bind("click", function () {
        $(".d-picker.d-state-active").each(function (i, pd) {
            if ($.isFunction(pd.close))
                pd.close();
        });
    });
});
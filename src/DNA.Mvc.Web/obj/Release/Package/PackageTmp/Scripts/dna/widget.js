(function ($) {
    $.widget("dna.widget", {
        options: {
            id: null,
            title: null,
            link: null,
            icon: null,
            contentUrl: null,
            closable: true,
            isStatic: false,
            expanded: true,
            cssText: null,
            headerCssText: null,
            bodyCssText: null,
            pos: 0,
            showHeader: true,
            showBorder: true,
            transparent: false,
            viewMode: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            el.addClass("d-ui-widget d-widget");
            el.attr("data-zone", el.parent().attr("id"));
            if (!opts.expanded)
                el.addClass("d-widget-collapsed");
            var _header = $(".d-widget-header", el);


            if (_header.length == 0) {
                _header = $("<div/>").addClass("d-ui-widget-header d-h3 d-widget-header").appendTo(el);
                var headLink = $("<a/>").appendTo(_header)
                                                         .addClass("d-widget-title-link")
                                                         .attr("href", "javascript:void(0);"),
                    headerText = $("<span/>").attr("contenteditable", "true")
                                                                 .addClass("d-widget-title-text")
                                                                 .text(opts.title ? opts.title : "")
                                                                 .appendTo(headLink)
                                                                 .bind("keydown", function (e) {
                                                                     if (e.keyCode == 13) {
                                                                         if ($(this).text() != opts.title)
                                                                             $(this).trigger("change");
                                                                         return false;
                                                                     }
                                                                 })
                                                                 .bind("blur", function () {
                                                                     if ($(this).text() != opts.title)
                                                                         $(this).trigger("change");
                                                                 })
                                                                 .bind("change", function () {
                                                                     var curTitle = $(this).text();
                                                                     opts.title = curTitle;
                                                                     self.settings({ title: opts.title });
                                                                     ////console.log(opts.title + " header change");
                                                                 });

                if (opts.icon)

                    if (opts.icon.startsWith("d-icon"))
                        $("<span/>").prependTo(_header).addClass(opts.icon).addClass("d-widget-icon");
                    else
                        $("<img/>").prependTo(_header).attr("src", opts.icon).addClass("d-widget-icon");
            }


            var _body = $(".d-widget-body", el);

            if (_body.length == 0)
                _body = $("<div/>").addClass("d-ui-widget-content d-widget-body").appendTo(el);

            _body.ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
                if (ajaxSettings.url == opts.contentUrl) {
                    $("<div/>").text("There is error occour during widget load, the error message is :" + thrownError).addClass("d-state-error")
                                      .appendTo(_body);
                }
            });

            if (opts.cssText) el.attr("style", opts.cssText);
            if (opts.headerCssText) _header.attr("style", opts.headerCssText);
            if (opts.bodyCssText) _body.attr("style", opts.bodyCssText);

            if (!opts.showHeader)
                _header.addClass("d-widget-header-hide");
            else
                _header.removeClass("d-widget-header-hide")

            if (!opts.showBorder)
                el.addClass("noborder");

            if (opts.transparent)
                el.addClass("d-transparent");

            if (opts.expanded)
                self.refresh();

            el.bind("click", function () {
                //if (!el.isActive()) {
                $(".d-widget.d-state-active").isActive(false);
                el.isActive(true);
                self.openTools();
                //} else {}
            }).bind("dblclick", function () {
                self.openPrefs();
            });

            //new feature for DNA3
            if (opts.viewMode == "floating")
                self.float();
            else {
                if (!opts.showHeader)
                    self.showHeader(false);
            }

            if (!opts.expanded)
                _header.show();

            return el;
        },
        changestyle: function () {
            var self = this, opts = this.options, _body = $(".d-widget-body", this.element), el = this.element;

            if (self.designer) {
                $(".d-panel[data-opened=true]").not(self.designer).each(function (i, pane) {
                    $(pane).taoPanel("close");
                });

                self.designer.taoPanel("open");

            } else {
                var panel = $("<div/>").appendTo("body").attr("data-pos", "right").css("position", "fixed"),
                    heading = $("<h3/>").text($.res("changestyle", "Change style")).appendTo(panel),
                body = $("<div/>").appendTo(panel);

                panel.taoPanel({
                    contentUrl: $.resolveUrl("~/widget/designer/" + this.options.id + "?locale=" + $("body").attr("lang")),
                    display: "overlay",
                    opened: true,
                    load: function () {
                        panel.unobtrusive_editors();
                    }
                });

                $(".d-panel[data-opened=true]").not(panel).each(function (j, pane) {
                    $(pane).taoPanel("close");
                });

                self.designer = panel;
            }
            self.closeTools();
        },
        showHeader: function (val) {
            var _header = $(".d-widget-header", this.element);//,
            //_verbs = $(".d-widget-verbs", this.element);
            if (val) {
                _header.show();
                //_header.append(_verbs);
            }
            else {
                _header.hide();
                //_verbs.appendTo(this.element);
            }
        },
        bingfront: function () {
            this.element.css("z-index", $.topMostIndex());
            this.savestyle();
        },
        sendback: function () {
            this.element.css("z-index", 2);
            this.savestyle();
        },
        savestyle: function () {
            var _body = $(".d-widget-body", this.element),
                _header = $(".d-widget-header", this.element);
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/applystyle"), {
                id: this.options.id,
                box: this.element.attr("style") ? this.element.attr("style") : "",
                body: _body.attr("style") ? _body.attr("style") : "",
                header: _header.attr("style") ? _header.attr("style") : ""
            });
        },
        float: function () {
            var opts = this.options, self = this, el = this.element;

            var _header = $(".d-widget-header", el),
                _verbs = $(".d-widget-verbs", el),
                _body = $(".d-widget-body", el);

            _header.hide();
            if (opts.viewMode != "floating") {
                self.settings({ viewMode: "floating" });
                opts.viewMode = "floating";
            }

            var _savestyle = function () { $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/applystyle"), { id: opts.id, box: el.attr("style") }); }

            el.addClass("d-widget-viewmode-floating")
               .draggable({
                   stop: _savestyle,
                   handle: '.d-widget-drag-handler'
               })
               .resizable({ stop: _savestyle, ghost: true, handles: 's,w,n,e' });

            if (_verbs.length) {
                _verbs.appendTo(el);
            }

            if (self.tools) {
                self.tools.remove();
                self.tools = null;
                self.openTools();
            }

        },
        restore: function () {
            var opts = this.options, self = this, el = this.element;

            if (opts.viewMode == "floating") {
                var _header = $(".d-widget-header", el),
                    _verbs = $(".d-widget-verbs", el),
                    _body = $(".d-widget-body", el);

                _header.show();

                el.css({ "left": "0px", "top": "0px", "position": "relative", "width": "auto" });
                $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/applystyle"), { id: opts.id, box: el.attr("style") }, function () {
                    self.settings({ viewMode: "" });
                });

                el.removeClass("d-widget-viewmode-floating")
                   .draggable('destroy')
                   .resizable('destroy');

                if (_verbs.length)
                    _verbs.appendTo(_header);

                opts.viewMode = "";

                if (self.tools) {
                    self.tools.remove();
                    self.tools = null;
                    self.openTools();
                }
            }
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("id")) opts.id = el.data("id");

            if (el.data("title"))
                opts.title = el.data("title");

            if (el.data("link")) opts.link = el.data("link");
            if (el.data("icon")) opts.icon = el.data("icon");
            if (el.data("content-url")) opts.contentUrl = el.data("content-url");
            if (el.data("style")) opts.cssText = el.data("style");
            if (el.data("header-style")) opts.headerCssText = el.data("header-style");
            if (el.data("body-style")) opts.bodyCssText = el.data("body-style");
            if (el.data("closable") != undefined) opts.closable = el.dataBool("closable");
            if (el.data("static") != undefined) opts.isStatic = el.dataBool("static");
            if (el.data("expanded") != undefined) opts.expanded = el.dataBool("expanded");
            if (el.data("show-header") != undefined) opts.showHeader = el.dataBool("show-header");
            if (el.data("show-border") != undefined) opts.showBorder = el.dataBool("show-border");
            if (el.data("pos") != undefined) opts.pos = el.dataInt("pos");
            return this;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        toggle: function () {
            var el = this.element, opts = this.options, self = this;
            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/toggle"), { id: this.options.id }, function () {
                if (opts.expanded)
                    el.addClass("d-widget-collapsed");
                else {
                    el.removeClass("d-widget-collapsed");
                    var _body = $(".d-widget-body", el);
                    if (_body.children().length == 0)
                        self.refresh();
                }
                opts.expanded = !opts.expanded;

                if (!opts.expanded) {
                    self.showHeader(true);
                } else {
                    self.showHeader(opts.showHeader);
                }
            });
        },
        del: function () {
            var el = this.element;
            $.ajax({
                url: $.resolveUrl("~/api/" + $("body").data("web") + "/widgets/remove"),
                type: "post",
                data: { id: this.options.id },
                error: function (jqXHR, textStatus, errorThrown) {
                    $.err(errorThrown);
                }
            }).done(function (data) {
                if (data.error) {
                    $.err(data.error);
                    return;
                }
                el.remove();
            });

        },
        prefs: function (values, keepstates) {
            var self = this, opts = this.options,
               _body = $(".d-widget-body", this.element),
               el = this.element,
               userPrefsForm = $("form.d-widget-prefs", _body);

            if (values != undefined) {
                if (values.jquery) {
                    var allvalues = { id: this.options.id };
                    $(":input[name]", values).each(function (i, n) {
                        var inputField = $(n);
                        allvalues[inputField.attr("name")] = inputField.val();
                    });

                    $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/apply"), allvalues, function () {
                        if (!keepstates)
                            self.refresh();
                    });
                } else {
                    if (values.id == undefined)
                        values.id = this.options.id;
                    $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/apply"), values, function () {
                        if (!keepstates)
                            self.refresh();
                    });
                }
                return el;
            } else {
                return userPrefsForm.serialize();
            }
        },
        settings: function (values) {
            var self = this, opts = this.options, _params = {
                id: opts.id,
                title: opts.title,
                link: opts.link,
                iconUrl: opts.icon,
                showHeader: opts.showHeader,
                showBorder: opts.showBorder
            };
            if (values != undefined) {
                //Save settings to remote
                _params.noroles = true;
                $.extend(_params, values);
                $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/save"), _params);
                self.setData(_params);
            } else
                return _params;
        },
        openSettings: function () {
            var self = this, opts = this.options, _body = $(".d-widget-body", this.element), el = this.element;

            if (self.settingPanel) {
                $(".d-panel[data-opened=true]").not(self.settingPanel).each(function (i, pane) {
                    $(pane).taoPanel("close");
                });

                self.settingPanel.taoPanel("open");

            } else {
                var panel = $("<div/>").appendTo("body").attr("data-pos", "right").css("position", "fixed"),
                    heading = $("<h3/>").text((res != undefined && res["widget-settings"]) ? res["widget-settings"] : "Widget settings").appendTo(panel),
                body = $("<div/>").appendTo(panel);

                panel.taoPanel({
                    contentUrl: $.resolveUrl("~/widget/settings/" + this.options.id + "?locale=" + $("body").attr("lang")),
                    display: "overlay",
                    opened: true
                });

                $(".d-panel[data-opened=true]").not(panel).each(function (j, pane) {
                    $(pane).taoPanel("close");
                });

                self.settingPanel = panel;
            }
            self.closeTools();
        },
        openPrefs: function () {
            var self = this, opts = this.options,
                _body = $(".d-widget-body", this.element),
                el = this.element,
                userPrefsForm = self._prefsForm();

            if (userPrefsForm.length) {
                var panel = $("<div/>").attr("data-pos", "right").appendTo("body").css("position", "fixed");
                $("<h3/>").text((res != undefined && res["prefs"]) ? res["prefs"] : "User preferences").appendTo(panel);

                var _b = $("<div/>").appendTo(panel).append(userPrefsForm.show()),
                    autosave = userPrefsForm.dataBool("auto-save"),
                    btnContainer = $("<div/>").css("padding", "10px").addClass("d-form-buttons-holder").appendTo(userPrefsForm);

                if (!autosave) {
                    var btn = $("<button/>").text((res != undefined && res.save) ? res.save : "Save")
                                             .attr("data-inline", false)
                                             .addClass("d-state-disable")
                                             .appendTo(btnContainer)
                                             .taoButton();

                    if (!userPrefsForm.attr("data-ajax")) {
                        //Autosave context
                        //set current widget context
                        window.widget = el.find("iframe.d-widget-frame").get(0).contentWindow.widget;
                        userPrefsForm.taoUI();
                        btn.bind("click", function (event) {
                            event.stopPropagation();
                            event.preventDefault();
                            var _prefData = {};
                            // userPrefsForm.serialize();

                            $("[name]", userPrefsForm).each(function (i, cb) {
                                if ($(cb).attr("type") == "checkbox") {
                                    if ($(cb).val().toLowerCase == "false" || $(cb).attr("checked") == undefined)
                                        _prefData[$(cb).attr("name")] = false;
                                } else
                                    _prefData[$(cb).attr("name")] = $(cb).val();
                            });

                            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/apply/" + opts.id),
                                _prefData, function () {
                                    $.closePanels();
                                    userPrefsForm.remove();
                                    self.refresh();
                                });

                        });
                    }
                }

                var btnDel = $("<a/>").text((res != undefined && res.del) ? res.del : "Delete")
                                             .attr("data-inline", false)
                                             .appendTo(btnContainer)
                                             .click(function () {
                                                 self.del();
                                                 panel.remove();
                                             })
                                             .taoButton();

                var btnSettings = $("<a/>").text((res != undefined && res.settings) ? res.settings : "Open settings")
                             .attr("data-inline", false)
                             .attr("data-icon-left", "d-icon-settings")
                             .appendTo(btnContainer)
                             .click(function () {
                                 self.openSettings();
                                 panel.remove();
                             })
                             .taoButton();

                self.closeTools();
                $.closePanels();

                panel.taoPanel({
                    opened: true,
                    autoRelease: true,
                    display: "overlay",
                    close: function () {
                        if (btnContainer)
                            btnContainer.remove();
                        if (userPrefsForm && !autosave)
                            userPrefsForm.prependTo(_body).hide();
                        panel.remove();
                    }
                });

                if (!autosave)
                    userPrefsForm.change(function () { btn.isDisable(false); });
            }

            return el;
        },
        hasPrefs: function () {
            var prefsForm = this._prefsForm();
            if (prefsForm.length == 0) return false;

            return !prefsForm.dataBool("hidden");
        },
        refresh: function () {
            var opts = this.options, _body = $(".d-widget-body", this.element), self = this;
            if (opts.contentUrl) {
                $.ajax(opts.contentUrl)
                  .done(function (htm) {
                      _body.html(htm).taoUI();
                      var _f = _body.find("iframe.d-widget-frame");
                      if (_f.length > 0) { //for client widget
                          $(_f).bind("load", function () {
                              var _w = this.contentWindow.widget;
                              _w._triggerEvent("design");
                          });
                      }
                  });
            }
        },
        openTools: function () {
            var opts = this.options, self = this, el = this.element, _isfloat = function () {
                return opts.viewMode == "floating";
            },
            _cmds = [
                { title: "Options", icon: "d-icon-cog", name: "openPrefs", hidden: !this.hasPrefs() },
                { title: "Settings", icon: "d-icon-settings", name: "openSettings", hidden: false },
               // { title: "Style", icon: "d-icon-paint-format", name: "changestyle", close: true },
                { title: "Delete", icon: "d-icon-trash", name: "del", hidden: opts.isStatic },
                {
                    title: _isfloat() ? "Restore" : "Float",
                    icon: _isfloat() ? "d-icon-pushpin" : "d-icon-external-link",
                    name: _isfloat() ? "restore" : "float",
                    hidden: false
                },
                { title: "Toggle", icon: "d-icon-gallery", name: "toggle", hidden: false }
            ];

            if (!self.tools)
                self.tools = self._createTools(_cmds);

            if (!self.tools.hasClass("d-state-opened")) {

                $(".d-widget").not(el).each(function (i, _widget) {
                    $(_widget).widget("closeTools");
                });

                $.closePanels();

                self.tools
                      .height(0)
                      .css("z-index", $.topMostIndex() + 1)
                      .stop(true, false)
                      .animate({ height: "65px" }, 200, function () {
                          self.tools.addClass("d-state-opened");
                      });
            }

        },
        closeTools: function () {
            var self = this, el = this.element;
            if (self.tools && self.tools.hasClass("d-state-opened")) {
                self.tools.animate({ height: "0px" }, 200, function () {
                    self.tools.removeClass("d-state-opened");
                });
            }
        },
        //design: function () {
        //    var dlg = $("<div/>").appendTo("body"), el = this.element, self = this,
        //        _body = $(".d-widget-body", this.element),
        //        _header = $(".d-widget-header", this.element);

        //    //$("body").blockUI();
        //    dlg.load($.resolveUrl("~/theme/uidesigner?locale=") + $("body").attr("lang"), function () {

        //        var box_css = el.attr("style") ? el.attr("style") : "",
        //            body_css = _body.attr("style") ? _body.attr("style") : "",
        //            header_css = _header.attr("style") ? _header.attr("style") : "",
        //         radios = $("<div/>").css("padding", "5px")
        //                                          .attr("data-value", "body")
        //                                          .attr("data-inline", "true")
        //                                          .prependTo(dlg);

        //        //.prependTo($(".ui-dialog-buttonpane", dlg.closest(".ui-dialog")));
        //        $("<input type='radio' value='widget' />").attr("data-label", $.res("box", "Box")).appendTo(radios);
        //        if (!_header.hasClass("d-widget-header-hide"))
        //            $("<input type='radio' value='header' />").attr("data-label", $.res("header", "Header")).appendTo(radios);

        //        $("<input type='radio' value='body'/>").attr("data-label", $.res("body", "Body")).appendTo(radios);


        //        dlg.taoUI();
        //        radios.taoRadios({
        //            change: function (event, ui) {
        //                var targetEl = _body;
        //                if (ui.value == "widget") targetEl = el;
        //                if (ui.value == "body") targetEl = _body;
        //                if (ui.value == "header") targetEl = _header;
        //                designElement(targetEl);
        //            }
        //        });

        //        $("body").unblockUI();
        //        var _undo = true, btns = {};
        //        btns[$.res("apply", "Apply")] = function () {
        //            dlg.closest(".ui-dialog").blockUI();
        //            $.post($.resolveUrl("~/api/" + $("body").data("web") + "/widgets/applystyle"),
        //                {
        //                    id: self.options.id,
        //                    box: el.attr("style") ? el.attr("style") : "",
        //                    body: _body.attr("style") ? _body.attr("style") : "",
        //                    header: _header.attr("style") ? _header.attr("style") : ""
        //                },
        //                function () {
        //                    dlg.closest(".ui-dialog").unblockUI();
        //                    _undo = false;
        //                    dlg.dialog("close");
        //                });
        //        };

        //        dlg.dialog({
        //            title: $.res("design", "Design") + " " + self.options.title,
        //            width: 430,
        //            resizable: false,
        //            modal: false,
        //            position: { of: document, at: "left top", my: "left top" },
        //            buttons: btns,
        //            close: function () {
        //                if (_undo) {
        //                    //resetElementStyle();
        //                    el.attr("style", box_css);
        //                    _body.attr("style", body_css);
        //                    _header.attr("style", header_css);
        //                }
        //                var filedlg = $("#design_filedlg");
        //                if (filedlg.length)
        //                    filedlg.remove();
        //                dlg.dialog("destroy").remove();
        //            },
        //            open: function () {
        //                //radios.taoRadios();
        //                designElement(_body);
        //            }
        //        });
        //    });
        //},
        disable: function () {
            this.widget().isDisable(true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            return this;
        },
        move: function (zid, pos) {
            $.ajax({
                type: "POST",
                url: $.resolveUrl("~/api/" + $("body").data("web") + "/widgets/moveto"),
                data: {
                    id: this.options.id,
                    zoneID: zid,
                    pos: pos
                },
                error: function (response) {
                    $.err(response.statusText, response.responseText);
                }
            });
        },
        setData: function (data) {
            //only set the data value to ui 
            if (data) {
                this._setTitle(data.title);
                this._setLink(data.link);
                this._setIcon(data.icon);

                if (data.cssText)
                    this.element.attr("style", data.cssText);

                if (data.headerCssText)
                    $(".d-widget-header", this.element).attr("style", data.headerCssText);

                if (data.bodyCssText)
                    $(".d-widget-body", this.element).attr("style", data.bodyCssText);

                if (data.transparent)
                    this.element.addClass("d-transparent");
                else
                    this.element.removeClass("d-transparent");

                if (data.showBorder)
                    this.element.addClass("noborder");
                else
                    this.element.removeClass("noborder");

                if (!this.options.expanded)
                    this._setShowHeader(true);
                else
                    this._setShowHeader(data.showHeader);
            }
            return this.element;
        },
        _createTools: function (cmds) {
            var footer = $("<div/>").addClass("d-ui-widget d-footer-tools").appendTo("body"),
                list = $("<ul/>").appendTo(footer), self = this;
            $.each(cmds, function (i, cmd) {
                if (!cmd.hidden) {
                    var _verb = $("<li/>").appendTo(list)
                                                       .append($("<span/>").addClass(cmd.icon))
                                                       .append($("<div/>").text(cmd.title))
                                                       .click(function () {
                                                           if ($.isFunction(self[cmd.name]))
                                                               self[cmd.name]();
                                                       });
                }
            });
            list.taoListview();
            return footer;
        },
        _prefsForm: function () {
            var _body = $(".d-widget-body", this.element), self = this;



            if ($("form.d-widget-prefs", _body).length)
                return $("form.d-widget-prefs", _body);

            var _frame = _body.find("iframe.d-widget-frame");

            if (_frame.length) {
                var _form = $("[data-role='widget-prefs']", _frame[0].contentWindow.document);
                if (_form.length)
                    return _form;
            }

            return [];
        },
        _setTitle: function (val) {
            var self = this, opts = this.options, el = this.element;
            var titleEl = $(".d-widget-title-text", el);

            if (titleEl.length && titleEl.text() != val && val != undefined) {
                titleEl.text(val);
                opts.title = val;
                el.attr("data-title", val);
            }
            return el;
        },
        _setLink: function (val) {
            var self = this, opts = this.options, el = this.element;
            var linkEl = $(".d-widget-title-link", el);

            //if (linkEl.length && linkEl.attr("href") != val) {
            //    if (val)
            //        linkEl.attr("href", val);
            //    else
                    linkEl.attr("href", "javascript:void(0);");
            //}

            if (val != undefined) {
                opts.link = val;
                el.attr("data-link", val);
            }

            return el;
        },
        _setIcon: function (val) {
            var self = this, opts = this.options, el = this.element, linkEl = $(".d-widget-title-link", el);
            var iconEl = linkEl.children(".d-widget-icon");
            if (iconEl.length)
                iconEl.remove();

            if (val) {
                if (opts.icon.startsWith("d-icon"))
                    $("<span/>").prependTo(linkEl).addClass(val).addClass("d-widget-icon");
                else
                    $("<img/>").prependTo(linkEl).attr("src", val).addClass("d-widget-icon");
            }

            opts.icon = val ? val : "";
            el.attr("data-icon", val ? val : "");

            return el;
        },
        _setShowHeader: function (val) {
            var self = this, opts = this.options, el = this.element;
            //if (val)
            //    wHeader.removeClass("d-widget-header-hide");
            //else
            //    wHeader.addClass("d-widget-header-hide");
            this.showHeader(val);
            opts.showHeader = val;
            el.attr("data-show-header", val);
            return el;
        },
        _setOption: function (key, value) {
            if (key == "title")
                return this._setTitle(value);

            if (key == "link")
                return this._setLink(value);

            if (key == "icon")
                return this._setIcon(value);

            if (key == "showHeader")
                return this._setShowHeader(value);

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            var el = this.element;
            //el.removeClass("d-widget");
            //var verbs = $(".d-widget-verbs", el);
            //if (verbs.length)
            //    verbs.remove();
            //var _body = $(".d-widget-body", this.element);
            //console.log($(".d-widget-frame", this.element).length);
            //console.log(_body.length);
            //if ($(".d-widget-frame", this.element).length > 0)
            //    _body.load(this.options.contentUrl);

            if (this.tools) {
                this.tools.remove();
                this.tools = null;
            }

            if (this.designer) {
                this.designer.remove();
                this.designer = null;
            }

            if (this.settingPanel) {
                this.settingPanel.remove();
                this.settingPanel = null;
            }

            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
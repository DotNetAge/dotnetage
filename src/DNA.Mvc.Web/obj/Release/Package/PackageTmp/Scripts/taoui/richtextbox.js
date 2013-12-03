(function ($) {
    NodeInfo = function (element) {
        this.element = $(element);
    }

    NodeInfo.prototype = {
        is: function (format) {
            var methodname = "is" + format[0].toUpperCase() + format.substr(1);
            if (this[methodname])
                return this[methodname]();
            return false;
        },
        hasClass: function (cls) { return this.element.hasClass(cls); },
        isBold: function () {
            if (this.isIn("strong") || this.isIn("b"))
                return true;
            var weight = this.element.css("font-weight");
            if (weight == "bold") return true;

            if (parseInt(weight) && parseInt(weight) > 400)
                return true;
            return false;
        },
        isItalic: function () { return (this.isIn("i") || this.hasStyle("font-style", "italic")) ? true : false; },
        isUnderline: function () { return (this.isIn("u") || this.hasStyle("text-decoration", "underline")) ? true : false; },
        isOverline: function () { return (this.hasStyle("text-decoration", "overline")) ? true : false; },
        isStrikeThrough: function () { return (this.hasStyle("text-decoration", "line-through")) ? true : false; },
        isLink: function () { return (this.isTag("A") || this.isIn("a")) ? true : false; },
        isJustifyLeft: function () { return (this.hasStyle("text-align", "left")) ? true : false; },
        isJustifyCenter: function () { return (this.isIn("center") || this.hasStyle("text-align", "center")) ? true : false; },
        isJustifyRight: function () { return (this.hasStyle("text-align", "right")) ? true : false; },
        isJustifyNone: function () { return (!this.isJustifyCenter() && !this.isJustifyLeft() && !this.isJustifyRight()) ? true : false; },
        isOrderList: function () { return this.isIn("ol") ? true : false; },
        isUnorderList: function () { return this.isIn("ul") ? true : false; },
        isSub: function () { return this.isIn("sub") ? true : false; },
        isSup: function () { return this.isIn("sup") ? true : false; },
        getFontFarmily: function () { return this.element.css("font-family") ? this.element.css("font-family") : "inherit"; },
        getForeColor: function () { return this.element.css("color") ? this.element.css("color") : "inherit"; },
        getBackgroundColor: function () { return this.element.css("background-color") ? this.element.css("background-color") : "inherit"; },
        getFontSize: function () { return this.element.css("font-size") ? this.element.css("font-size") : "inherit"; },
        isTag: function (_tagName) { return this.getTagName() == _tagName ? true : false; },
        isIn: function (_tagName) {
            if (this.element[0].tagName.toLowerCase() == _tagName)
                return true;
            else
                return this.element.closest(_tagName).length ? true : false;
        },
        hasStyle: function (_name, _value) {
            if (this.element.css(_name)) {
                return this.element.css(_name).toString().toLocaleLowerCase().indexOf(_value) > -1;
            }
            else
                return false;
        },
        getTagName: function () { return this.element[0].tagName; }
    };

    $.widget("dna.taoEditor", {
        options: {
            showcode: true,
            width: "auto",
            height: 300,
            //editorStyle: "font-family:Verdana, Arial, Helvetica;font-size:12pt;background:#fff;",
            //documentStyle: false,
            select: null,
            normalized: null,
            autoEncode: false,
            toolbar: null,
            resizable: true,
            htmlText: "Design",
            codeText: "Html",
            editing: null,//Returns the editing element
            toolpanes: [
               [
               { title: "Bold", "tooltip-position": "top", icon: "d-icon-bold", role: "checkbox", command: "setBold" },
               { title: "Italic", "tooltip-position": "top", icon: "d-icon-italic", role: "checkbox", command: "setItalic" },
               { title: "Underline", "tooltip-position": "top", icon: "d-icon-underline", role: "checkbox", command: "setUnderline" },
               { title: "Strike through", "tooltip-position": "top", icon: "d-icon-strikethrough", role: "checkbox", command: "setStrikeThrough" },
               { text: "-" },
               { title: "Fore color", "tooltip-position": "top", html: "<input data-role='picker' type='color' style='width:60px' data-icon='d-icon-font-2'/>" },
               { title: "Background color", "tooltip-position": "top", html: "<input data-role='picker' type='color' style='width:60px' data-icon='d-icon-paint-format'/>" }
               ],
               [
                    { title: "Justify left", "tooltip-position": "top", icon: "d-icon-align-left", command: "setJustifyLeft", role: "radio", group: "justify" },
                    { title: "Justify center", "tooltip-position": "top", icon: "d-icon-align-center", command: "setJustifyCenter", role: "radio", group: "justify" },
                    { title: "Justify right", "tooltip-position": "top", icon: "d-icon-align-right", command: "setJustifyRight", role: "radio", group: "justify" },
                    { title: "Justify full", "tooltip-position": "top", icon: "d-icon-align-justify", command: "setJustifyFull", role: "radio", group: "justify" },
                    { text: "-" },
                    { title: "Indent", "tooltip-position": "top", icon: "d-icon-indent-right", command: "setIndent" },
                    { title: "Outdent", "tooltip-position": "top", icon: "d-icon-indent-left", command: "setOutdent" },
                    { title: "Ordered list", "tooltip-position": "top", icon: "d-icon-list-ol", command: "insertOrderedList" },
                    { title: "Unordered list", "tooltip-position": "top", icon: "d-icon-list-ul", command: "insertUnorderedList" }
               ]
            ]
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element,
            wrapper = $("<div/>").addClass("d-reset d-ui-widget d-rte"), panes = $("<div/>").addClass("d-rte-panes").appendTo(wrapper),
            editorWrapper = $("<div/>").addClass("d-ui-widget-content d-rte-editor").appendTo(wrapper);
            this.wrapper = wrapper;
            this._unobtrusive();

            if (opts.toolbar) {
                var toolbar = $(opts.toolbar);
                if (toolbar.length) {
                    toolbar.appendTo(panes);
                    this.tools = toolbar;
                    $("li[data-cmd]", toolbar).on("click", function (event) {
                        event.preventDefault();
                        var _cmd = $(this).data("cmd"), _opt = $(this).data("cmd-param"), _optval = null;

                        if (_opt) {
                            try {
                                var func = new Function(_opt);
                                _optval = func();
                            } catch (e) {
                                console.log(e);
                            }
                        }

                        if (_cmd && self[_cmd])
                            self[_cmd](_optval);
                    });
                }
            } else {
                $.each(opts.toolpanes, function (i, pane) {
                    $("<ul/>").appendTo(panes).taoMenu({
                        type: "toolbar",
                        itemClick: function (event, ui) {
                            var cmd = $(ui.item).data("command");
                            if (self[cmd])
                                self[cmd]();
                        },
                        datasource: pane
                    });
                });
            }

            el.before(wrapper);

            if (opts.editorStyle)
                el.attr("style", opts.editorStyle);

            if (el.attr("rows")) {
                var rows = parseInt(el.attr("rows"));
                editorWrapper.height((rows * 24) + "px");
            }
            else {
                if (opts.height)
                    editorWrapper.height(opts.height);
            }

            if (opts.width != "auto" && opts.width > 0)
                editorWrapper.width(opts.width);

            this.height = editorWrapper.innerHeight();
            this.width = editorWrapper.innerWidth();

            var tabs = $("<div/>").addClass("d-rte-tabs").appendTo(wrapper),
             //_iframe = $("<iframe src='javascript:void(0);' frameborder='0' style='width:100%;'></iframe>"),
             _htmlEditor = $("<div/>").attr("contenteditable", true).addClass("d-rte-editor-html"),
                htmlWS = this.addWorkspace("html", _htmlEditor, opts.htmlText, function () {
                    self.enableToolbars();
                    //_htmlEditor.html($.browser.msie ? el.text() : el.val());
                    _htmlEditor.html(el.val()).focus();
                    //self._openHtml($.browser.msie ? el.text() : el.val());
                }),
                textWS = this.addWorkspace("text", el, opts.codeText, function () {
                    // $.browser.msie ? el.val(self._getHtml()) : el.text(self._getHtml());
                    var _htmlText = _htmlEditor.html();
                    //$.browser.msie ? el.val(_htmlText) : el.text(_htmlText);
                    //if (_htmlText)
                    //_htmlText = self.htmlEncode(_htmlText);
                    el.val(_htmlText).focus();
                    self.disableToolbars();
                });

            _htmlEditor.on("mouseup", function (e) {
                //var range = document.createTextRange();
                opts.editing = e.target;
                if ($(this).text()) {
                    //console.log($(this).selectionStart);
                    var range = self.getSelectionRange();
                    //console.log($(this).range);
                    if (window.getSelection) {
                        //console.log("Mark this range");
                        //console.log(range);
                        $(this).data("range-start", range.startOffset);
                        $(this).data("range-end", range.endOffset);
                    }
                    else {
                        //ie: 
                    }
                }
            }).on("click", function () {
                self._parseElement();
            })
               .on("keypress", function () {
                   self._parseElement();
               });

            if (el.attr("placeholder"))
                _htmlEditor.attr("data-placeholder", el.attr("placeholder"));

            if (!opts.showcode)
                tabs.hide();

            if (opts.resizable) {
                wrapper.resizable({
                    alsoResize: editorWrapper,
                    //  autoHide: true,
                    // containment:"parent",
                    handles: "n,s",
                    resize: function (event, ui) {
                        self.resizeWorkspaces();
                    }
                });
            }

            var _form = el.closest("form");
            if (_form.length) {
                _form.on("submit", function () {
                    if (self.curMode == "html") {
                        var htmlText = self.getEditor().html();
                        if (opts.autoEncode)
                            $.browser.msie ? el.val(self.htmlEncode(htmlText)) : el.text(self.htmlEncode(htmlText));
                        else
                            $.browser.msie ? el.val(htmlText) : el.text(htmlText);
                    }
                });
            }

            this.setWorkspace("html");

            if (!el.parent().isVisible()) {
                window.setTimeout(function () {
                    self.setWorkspace(self.curMode);
                }, 500);
            }

            if (opts.select)
                el.bind(eventPrefix + "select", opts.select);
        },
        getSelectionRange: function () {
            var sel;
            if (window.getSelection) { //for ff/chrome
                sel = window.getSelection();
                if (sel.rangeCount) {
                    return sel.getRangeAt(0);
                }
            } else if (document.selection) {
                return document.selection.createRange();
            }
            return null;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            //if (el.data("editor-style")) opts.editorStyle = el.data("editor-style");
            //if (el.data("embded-style") != undefined) opts.documentStyle = el.dataBool("embded-style");
            if (el.data("encoded") != undefined) opts.autoEncode = el.dataBool("encoded");
            if (el.data("toolbar")) opts.toolbar = el.datajQuery("toolbar");
            if (el.data("showcode") != undefined) opts.showcode = el.dataBool("showcode");
            if (el.data("text-html")) opts.htmlText = el.data("text-html");
            if (el.data("text-code")) opts.codeText = el.data("text-code");
            if (el.data("height") != undefined) opts.height = el.data("height");
            if (el.data("width") != undefined) opts.width = el.data("width");
            if (el.data("select"))
                opts.select = new Function("event", "ui", el.data("select"));
        },
        _parseElement: function () {
            var nodeInfo = null;

            if (window.getSelection) {
                //ff
                var sel = window.getSelection().getRangeAt(0);
                if (sel.startContainer) {
                    if (sel.startContainer.localName) {
                        nodeInfo = new NodeInfo(sel.startContainer);
                    } else {
                        //text node
                        if (sel.startContainer.parentNode)
                            nodeInfo = new NodeInfo(sel.startContainer.parentNode);
                    }
                }
            }
            else {
                if (document.selection) {
                    var range = document.selection.createRange();
                    if (range.parentElement) {
                        nodeInfo = new NodeInfo(range.parentElement);
                    }
                }
            }

            if (nodeInfo) {
                //try parse format
                if (this.tools) {
                    var checks = ["bold", "italic", "underline", "overline", "strikeThrough"],
                        justifyGroups = ["justifyLeft", "justifyCenter", "justifyRight", "justifyFull"],
                        orderGroups = ["insertOrderedList", "insertUnorderedList"];

                    for (var i = 0; i < checks.length; i++) {
                        var cmd = checks[i],
                        cmdEles = $("[data-cmd=" + cmd + "]", this.tools);
                        if (cmdEles.length) {
                            var cmdVal = nodeInfo.is(cmd);
                            cmdEles.attr("data-checked", cmdVal).isActive(cmdVal);
                        }
                    }

                    for (var j = 0; j < justifyGroups.length; j++) {
                        var cmd = justifyGroups[j];
                        cmdEles = $("[data-cmd=" + cmd + "]", this.tools);
                        if (cmdEles.length) {
                            var groupName = cmdEles.attr("data-group");
                            if (groupName) {
                                var cmdVal = nodeInfo.is(cmd);
                                if (cmdVal) {
                                    cmdEles.siblings("[data-group=" + groupName + "]").attr("checked", false).isActive(false);
                                    cmdEles.attr("data-checked", cmdVal).isActive(cmdVal);
                                    break;
                                }
                            }
                        }
                    }

                    for (var k = 0; k < orderGroups.length; k++) {
                        var cmd = orderGroups[k];
                        cmdEles = $("[data-cmd=" + cmd + "]", this.tools);
                        if (cmdEles.length) {
                            var groupName = cmdEles.attr("data-group");
                            if (groupName) {
                                var cmdVal = nodeInfo.is(cmd);
                                if (cmdVal) {
                                    cmdEles.siblings("[data-group=" + groupName + "]").attr("checked", false).isActive(false);
                                    cmdEles.attr("data-checked", cmdVal).isActive(cmdVal);
                                    break;
                                }
                            }
                        }
                    }
                }

                this._triggerEvent("select", { element: nodeInfo.element, info: nodeInfo });
            }
        },
        resizeWorkspaces: function () {
            var editorWrapper = $(".d-rte-editor", this.wrapper);
            this.height = editorWrapper.height();
            this.width = editorWrapper.width();
            var self = this,
                overlay = $(".d-rte-toolbar-overlay", this.wrapper),
                panes = $(".d-rte-panes", this.wrapper),
                wps = $(".workspace", this.wrapper).each(function (i, n) {
                    var h = self.height,
                        w = self.width;
                    if (h)
                        $(n).children().height(h);
                    if (w)
                        $(n).children().css("width", "inherit");

                        $(n).children("textarea").width(w);
                });

            if (overlay.length) {
                overlay.height(panes.height())
                if (panes.width())
                    overlay.width(panes.width());
            }


        },
        addWorkspace: function (name, element, txt, onactived) {
            var self = this, wsp = $("<div/>").addClass("workspace " + name).appendTo($(".d-rte-editor", this.wrapper));
            wsp.append(element);
            if ($.isFunction(onactived))
                wsp.bind("ws_active", onactived);

            if (this.height)
                element.height(this.height);

           // if (this.width)
                //element.width(this.width);
            element.css("width","inherit");
            var tab = $("<span/>").addClass("d-ui-widget d-rte-tab-" + name)
                                               .appendTo($(".d-rte-tabs", this.wrapper))
                                               .click(function () {
                                                   if (!$(this).isActive()) {
                                                       self.setWorkspace(name);
                                                   }
                                               }).text(txt);
            return wsp;
        },
        setWorkspace: function (name) {
            var self = this, opts = this.options, el = this.element, cur_ws = $(".workspace." + name, this.wrapper);
            $(".workspace", this.wrapper).isActive(false);
            cur_ws.isActive(true);
            $(".d-rte-tabs .d-state-active", this.wrapper).isActive(false);
            $(".d-rte-tab-" + name, this.wrapper).isActive(true);
            this.curMode = name;
            cur_ws.trigger("ws_active");
            self.resizeWorkspaces();
        },
        mode: function (name) {
            this.setWorkspace(name);
        },
        enableToolbars: function () {
            var overlay = $(".d-rte-toolbar-overlay", this.wrapper);
            if (overlay.length)
                overlay.remove();
        },
        disableToolbars: function () {
            var panes = $(".d-rte-panes", this.wrapper),
                overlay = $("<div/>").addClass("d-rte-toolbar-overlay").appendTo(panes);

            overlay.height(panes.height())
                         .width(panes.width());
            //.css({
            //    "background-color": this.element.css("background-color")
            //});
        },
        getEditor: function () { return $(".d-rte-editor-html", this.wrapper); },
        bold: function () { this.runCmd("Bold"); },
        italic: function () { this.runCmd("Italic"); },
        underline: function () { this.runCmd("Underline"); },
        strikeThrough: function () { this.runCmd("StrikeThrough"); },
        justifyLeft: function () { this.runCmd("JustifyLeft"); },
        justifyCenter: function () { this.runCmd("JustifyCenter"); },
        justifyRight: function () { this.runCmd("JustifyRight"); },
        justifyFull: function () { this.runCmd("Justifyfull"); },
        fontName: function (_name) {
            this.runCmd("fontName", _name);
        },
        fontSize: function (_size) {
            this.runCmd("fontSize", _size)
        },
        insertOrderedList: function () { this.runCmd("InsertOrderedList"); },
        insertUnorderedList: function () { this.runCmd("InsertUnorderedList"); },
        indent: function () { this.runCmd("Indent"); },
        outdent: function () { this.runCmd("Outdent"); },
        setUppercase: function () {
            var ele = this.options.editing;
            if (ele) {
                if (!$(ele).hasClass("d-rte-editor-html"))
                    $(ele).css("text-transform", "uppercase");
            }
        },
        setLowercase: function () {
            var ele = this.options.editing;
            if (ele) {
                if (!$(ele).hasClass("d-rte-editor-html"))
                    $(ele).css("text-transform", "lowercase");
            }
        },
        foreColor: function (_color) { this.runCmd("forecolor", _color); },
        setBackgroundColor: function (_color) { this.runCmd("hilitecolor", _color); },
        superscript: function (val) { this.runCmd("Superscript", val); },
        subscript: function (val) { this.runCmd("Subscript", val); },
        insertHorizontalRule: function () { this.runCmd("InsertHorizontalRule"); },
        insertParagraph: function (val) { this.runCmd("InsertParagraph", val); },
        insertDate: function () { this.runCmd("insertHTML", "<span>" + (new Date()).toDateString() + "</span>"); },
        insertTime: function () { this.runCmd("insertHTML", "<span>" + (new Date()).toLocaleTimeString() + "</span>"); },
        insertLink: function (url) {
            if (!url)
                url = window.prompt("Please supply url address");
            if (url)
                document.execCommand("CreateLink", true, url);
        },
        unlink: function () { this.runCmd("Unlink"); },
        insertBlock: function (patern) {
            var txt = window.getSelection().toString(),
                results = patern.replace(/\{0\}/, txt);

            if (window.getSelection) {
                //Firefox
                var sel=window.getSelection();
                if (sel.rangeCount) {
                    var _range = sel.getRangeAt(0);
                    _range.deleteContents();
                    _range.insertNode($(results)[0]);
                }

            } else {
                if (document.selection) {
                    var sel = document.selection.createRange();
                    sel.pasteHTML(results);
                }
            }

        },
        formatBlock: function (format) {
            this.runCmd("FormatBlock", format);
        },
        removeFormat: function () { this.runCmd("RemoveFormat"); },
        copy: function () { this.runCmd("Copy"); },
        cut: function () { this.runCmd("Cut"); },
        paste: function () { this.runCmd("Paste"); },
        print: function () { this.runCmd("Print"); },
        runCmd: function (cmd, opt) {
            this.execCmd(cmd, opt);
        },
        execCmd: function (command, option) {
            if (option == 'removeFormat') {
                command = option;
                option = null;
            }

            //
            try {
                //fix the selection lost issuse
                //if (this.isIE() && this.curSelection)
                //this.curSelection.select();
                //this.getEditor().focus();
                document.execCommand(command, false, option);
            }
            catch (e) {
                alert(e);
                //alert("You browser does not support this command.");
            }

        },
        isDesignModeSupported: function () {
            return (document.designMode && document.execCommand) ? true : false;
        },
        isGecko: function () { return (navigator.userAgent.indexOf('Gecko') != -1) ? true : false; },
        isIE: function () { return $.browser.msie; },
        isHtmlEncode: function (strHtml) {
            if (strHtml.search(/&amp;/g) != -1 || strHtml.search(/&lt;/g) != -1 || strHtml.search(/&gt;/g) != -1)
                return true;
            else
                return false;
        },
        htmlEncode: function (strHtml) {
            if (strHtml)
                return strHtml.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
            else
                return "";
        },
        htmlDecode: function (strHtml) {
            if (this.isHtmlEncode(strHtml))
                return strHtml.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
            return strHtml;
        },
        insertLink: function (_url, _target) {
            if (_url && _url != 'http://') {
                if (this.curSelection)
                    this.runCmd('insertHTML', '<a href="' + _url + '" target="' + _target + '">' + this.curSelection.text);
                else this.runCmd('createLink', _url);
            }
        },
        formatBlock: function (format) {
            this.runCmd("FormatBlock", format);
        },
        insertHtml: function (_html) {
            this.getEditor().focus();
            this.runCmd("insertHTML",_html);
        },
        insertVideo: function (_url, _type) {
            this.getEditor().focus();
            var placeHolder = $("<div/>"),
                videoEle = $("<video/>").attr("controls", "controls").appendTo(placeHolder)
                .attr("autoplay", "autoplay")
                .attr("preload", "auto");
            //.attr("onloadedmetadata", "if ($(this).width()>$(this).parent().width()) $(this).width($(this).parent().width());");
            var src = $("<source/>").attr("src", _url).attr("type", _type).appendTo(videoEle);
            this.runCmd("insertHTML", placeHolder.html());
        },
        insertImage: function (_url, _alt) {
            this.getEditor().focus();
            if (_url && _url != 'http://')
                this.insertBlock('<div class="d-shadow-around" style="float:left;padding:5px;margin:10px;background:#fff;"><img src="' + _url + '" alt="' + (_alt ? _alt : "") + '" /></div>');
                //this.runCmd('insertHTML', '<div class="d-shadow-around" style="float:left;padding:5px;margin:10px;background:#fff;"><img src="' + _url + '" alt="' + (_alt ? _alt : "") + '" /></div>');
        },
        _setOption: function (key, value) {
            if (key == "height") {
                var _panes = $(".d-rte-panes", this.wrapper),
                    _container = $(".d-rte-editor", this.wrapper),
                    _tabs = $(".d-rte-tabs", this.wrapper);
                _container.height(value - (_panes.outerHeight(true) + _tabs.outerHeight(true)));
                this.options.height = value;
                this.resizeWorkspaces();
            }
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        },
        widget: function () {
            return this.wrapper;
        }
    });
})(jQuery);
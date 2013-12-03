/*!  
** Copyright (c) 2013 Ray Liang (http://www.dotnetage.com)
** Dual licensed under the MIT and GPL licenses:
** http://www.opensource.org/licenses/mit-license.php
**/

(function ($) {
    $.widget("dna.taoEditable", {
        options: {
            placeholder: null,
            toolbar: null,
            select: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            el.attr("contenteditable", "true");
            var timer = null,
                _setTimer = function () {
                    _clearTimer();
                    timer = window.setTimeout(function () {
                        if (self.tools)
                            self.tools.fadeOut();
                        timer = null;
                    }, 5000);
                },
                _clearTimer = function () {
                    if (timer) {
                        window.clearTimeout(timer);
                        timer = null;
                    }
                };

            this.orgVal =$.trim(el.html());
            this.ischanged = false;

            this._createToolbars();

            el.bind("click", function (e) {
                e.stopPropagation();
                self._parseElement();
                $(this).focus();
            })
               .bind("dblclick", function (e) {
                   e.stopPropagation();
               })
               .bind("keypress", function (e) {
                   if (!self.tools.isVisible())
                       self.tools.fadeIn();
                   self._parseElement();

                   var _c = e.charCode;
                   if ((_c == undefined) || (_c == 0)) _c = e.keyCode;

                   var ignores = [8, 9, 16, 17, 18, 19, 20, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43];
                   if ($.inArray(_c, ignores) == -1) {
                       if (self.orgVal !=$.trim(el.html()))
                           el.trigger("change");
                   }
               })
               .bind("focus", function () {
                   self.tools.css({
                       // "position": "fixed",
                       "top": "0px",
                       "left": "0px"
                   }).fadeIn();
                   _clearTimer();
               })
               .bind("blur", function () {
                   _setTimer();
                   if (self.orgVal != el.html())
                       el.trigger("change");

               });

            if (self.tools)
                self.tools.bind("mouseover", function () {
                    _clearTimer();
                }).bind("mouseleave", function () { _setTimer(); });

            if (opts.select)
                el.bind(eventPrefix + "select", opts.select);

            $("body").bind("click", function () {
                if (self.tools && self.tools.isVisible()) {
                    self.tools.fadeOut();
                }
            });

            return el;
        },
        _trackChanges: function () { },
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
        _createToolbars: function () {
            var self = this, opts = this.options, el = this.element, toolArgs = [
               [
               { title: "Bold", "tooltip-position": "top", icon: "d-icon-bold", role: "checkbox", cmd: "bold" },
               { title: "Italic", "tooltip-position": "top", icon: "d-icon-italic", role: "checkbox", cmd: "italic" },
               { title: "Underline", "tooltip-position": "top", icon: "d-icon-underline", role: "checkbox", cmd: "underline" },
               { title: "Strike through", "tooltip-position": "top", icon: "d-icon-strikethrough", role: "checkbox", cmd: "strikeThrough" },
               { text: "-" },
               { title: "Fore color", "tooltip-position": "top", html: "<input data-role='picker' type='color' style='width:80px' data-icon='d-icon-font-2'/>" },
               { title: "Background color", "tooltip-position": "top", html: "<input data-role='picker' type='color' style='width:80px' data-icon='d-icon-paint-format'/>" }
               ],
               [
                    { title: "Justify left", "tooltip-position": "top", icon: "d-icon-align-left", cmd: "justifyLeft", role: "radio", group: "justify" },
                    { title: "Justify center", "tooltip-position": "top", icon: "d-icon-align-center", cmd: "justifyCenter", role: "radio", group: "justify" },
                    { title: "Justify right", "tooltip-position": "top", icon: "d-icon-align-right", cmd: "justifyRight", role: "radio", group: "justify" },
                    { title: "Justify full", "tooltip-position": "top", icon: "d-icon-align-justify", cmd: "justifyFull", role: "radio", group: "justify" },
                    { text: "-" },
                    { title: "Indent", "tooltip-position": "top", icon: "d-icon-indent-right", cmd: "indent" },
                    { title: "Outdent", "tooltip-position": "top", icon: "d-icon-indent-left", cmd: "outdent" },
                    { title: "Ordered list", "tooltip-position": "top", icon: "d-icon-list-ol", cmd: "insertOrderedList", role: "radio", group: "ordered" },
                    { title: "Unordered list", "tooltip-position": "top", icon: "d-icon-list-ul", cmd: "insertUnorderedList", role: "radio", group: "ordered" }
               ]
            ];


            if (opts.toolbar) {
                var toolbar = $(opts.toolbar);
                if (toolbar.length) {
                    this.tools = toolbar;
                    this.internalTools = false;
                    this.tools.appendTo("body")
                          .bind("click", function (e) {
                              e.stopPropagation();
                          });
                    
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
                var panes = $("<div/>").addClass("d-ui-widget d-rte-panes").css({
                    position: "fixed",
                    "padding": "5px",
                    "width": "100%"
                }).appendTo("body")
                    .bind("click", function (e) {
                    e.stopPropagation();
                });

                $.each(toolArgs, function (i, pane) {
                    $("<ul/>").addClass("d-inline")
                                    .appendTo(panes)
                                    .taoMenu({
                                        type: "toolbar",
                                        itemClick: function (event, ui) {
                                            var cmd = $(ui.item).data("cmd");
                                            if (self[cmd])
                                                self[cmd]();
                                        },
                                        datasource: pane
                                    });
                });
                this.tools = panes;
                this.internalTools = true;
            }

            if (this.tools && this.tools.length)
                this.tools.hide();



            return el;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.attr("placeholder"))
                opts.placeholder = el.attr("placeholder");

            if (el.data("placeholder"))
                opts.placeholder = el.data("placeholder");

            if (el.data("toolbar"))
                opts.toolbar = el.datajQuery("toolbar");

            if (el.data("select"))
                opts.select = new Function("event", "ui", el.data("select"));

        },
        discard: function () {
            var el = this.element;
            el.html(this.orgVal);
            this.ischanged = false;
        },
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
                var sel = window.getSelection().getRangeAt(0);
                sel.deleteContents();
                sel.insertNode($(results)[0]);
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
            try {
                document.execCommand(command, false, option);
            }
            catch (e) {
                alert(e);
            }

        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        destroy: function () {
            this.element.attr("contenteditable", false)
                                .removeClass("d-editable");
            if (this.tools) {
                if (this.internalTools)
                    this.tools.remove();
                else {
                    this.element.after(this.tools);
                    $("li[data-cmd]", this.tools).off("click");
                }
            }

            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
(function ($) {
    $.widget("dna.layout", {
        options: {
            label: null,
            focuszoneHolder: false,
            columns: null,
            //remove: null,
            //create: null,
            //change: null,
            deletable: true,
            designable: true,
            pinnable: false,
            extensible: true,
            autoFixed: false
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            el.addClass("d-layout");//.disableSelection();
            var _isContainer = el.parents("[data-role='layout']").length == 0;

            if (!_isContainer) {
                // el.wrap("<div/>");
                //var wrapper = el.parent().addClass("d-layout-wrapper");
                //,_header = $("<div/>").prependTo(wrapper).addClass("header");
                //$("<span/>").addClass("dragHandler ui-icon ui-icon-grip-dotted-vertical").appendTo(_header);

                //var _layoutTitle = $("<span/>").addClass("title")
                //    .text(opts.label ? opts.label : "")
                //    .appendTo(_header)
                //    .attr("contenteditable", "true")
                //    .bind("focus", function () {
                //        $(this).select();
                //        wrapper.addClass("d-state-highlight");
                //    })
                //    .bind("blur", function () {
                //        wrapper.removeClass("d-state-highlight");
                //    })
                //    .bind("keydown", function (e) {
                //        if (e.keyCode == 13) {
                //            $(this).trigger("change");
                //            return false;
                //        }
                //    })
                //    .bind("change", function () {
                //        //console.log($(this).text());
                //        if ($(this).text()) {
                //            el.attr("data-label", $(this).text());
                //            $("body").trigger("layoutchange", { layout: true });
                //        } else
                //            $(this).text(el.attr("data-label"));
                //    });

                //var _tools = $("<div/>").addClass("tools")
                //                                       .appendTo(_header);

                //this.wrapper = wrapper;
                //this.header = _header;

                // self._buildLayoutTools(_tools);
                var columnsHolder = $(">.d-layout-columns", el);
                var tools = $("<ul/>").appendTo(el).addClass("d-layout-tools");
                tools.taoMenu({
                    type: "toolbar",
                    datasource: [
                        { icon: "d-icon-move dragHandler" },
                        {
                            icon: "d-icon-cog", cmd: "settings", click: function () { }
                        },
                        {
                            icon: "d-icon-cross-3", click: function () {
                                $.confirm($.res("dellayoutconfirm", "Are you sure delete this layout and embded layouts?"))
                                  .done(function () {
                                      el.remove();
                                  });
                            }
                        }
                    ]
                });
                self.layoutTools = tools;
                el.bind("click", function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    $(".d-layout", el.closest(".d-state-design")).not(this).removeClass("d-state-selected");
                    $(this).toggleClass("d-state-selected");
                    $(this).children(".d-layout-tools").css({ "z-index": $.topMostIndex() });

                    if (opts.columns < 3) {
                        $("[data-cmd=settings]", tools).remove();
                        tools.css("width", "auto");
                    }
                });

                if (opts.extensible) {
                    if (opts.columns) {
                        if (columnsHolder.length == 0) {
                            columnsHolder = $("<div/>").addClass("d-layout-columns").appendTo(el);
                            columnsHolder.addClass("d-layout-columns-" + (el.data("class") ? el.data("class") : opts.columns))

                            for (var i = 0; i < opts.columns; i++) {
                                $("<div/>").addClass("d-layout-column")
                                                  .appendTo(columnsHolder);
                            }

                            //$.each(opts.columns, function (i, n) {
                            //    var column = $("<div/>").appendTo(columnsHolder)
                            //                                             .attr("data-width", n)
                            //                                             .addClass("d-layout-column");
                            //    if (n.endsWith("%")) {
                            //        var flex = (parseFloat(n.replace("%", "")) / 10).toFixed(2);
                            //        column.css({
                            //            "-webkit-box-flex": flex,
                            //            "-ms-box-flex": flex,
                            //            "-moz-box-flex": flex
                            //        });
                            //    }
                            //    else {
                            //        if (n.endsWith("px") || n.endsWith("em")) {
                            //            column.css("width", n);
                            //        } else {
                            //            column.css("width", n + "px");
                            //        }
                            //    }
                            //});
                        }
                    }
                }

                var _cols = $(">.d-layout-columns>.d-layout-column", el);
                //self._initZones(el);
                if (el.children(".d-widget-zone").length)
                    self._initColumn(el);
                else
                    _cols.each(function (i, c) {
                        self._initColumn($(c));
                        //if (i < _cols.length - 1) {
                        //    $(c).resizable({ handles: "e" });
                        //}
                    });

            }
            else {
                // var parentHeight = el.parent().height();
                //  el.height(parentHeight);
                self._initDragAndDrop(el);
            }

            this._initCommands();
        },
        _initColumn: function (column) {
            this._initDragAndDrop(column);
            this._initZones(column);
        },
        _initZones: function (column) {
            var self = this, opts = this.options,
                el = this.element,
                _zone = $(">.d-widget-zone", column);

            if (opts.focuszoneHolder && _zone.length == 0) {
                var _id = self._genZoneID();
                _zone = $("<div id='" + _id + "' />").attr("data-role", "widgetzone")
                                                                          .prependTo(column)
                                                                          .addClass("d-layout d-widget-zone");
                //_zone.widgetZone();
            }

            //this._buildTitleEditorTo(_zone);

            //self._buildTitleEditorTo(theZone);
        },
        _initCommands: function () {
            var self = this, el = this.element;
            if (this.wrapper) {
                var _context = {
                    title: "Layout : " + this.options.label,
                    commands: [
                        { title: "Change style", name: "changestyle" }
                    ],
                    verbs: [
                        { title: "Bing to front", icon: "ui-icon ui-icon-help", name: "bingfront" },
                        { title: "Send to back", icon: "ui-icon ui-icon-help", name: "sendback" },
                        { title: "Delete", icon: "ui-icon ui-icon-trash", name: "del", close: true }
                    ]
                };

                //create context menu
                this.wrapper.bind("contextpopup", function (e, ui) {
                    e.stopPropagation();
                    e.returns = _context;
                }).bind("command", function (e, cmd) {
                    e.stopPropagation();
                    if ($.isFunction(self[cmd]))
                        self[cmd]();
                });
            }
        },
        del: function () {
            //  $.notify(this.options.label);
            this.element.remove();
        },
        //_buildLayoutTools: function (_tools) {
        //    var self = this, el = this.element, opts = this.options;

        //    if (opts.pinnable) {
        //        var pinner = $("<span/>").addClass("ui-icon ui-icon-pin-w")
        //                 .appendTo(_tools)
        //                 .click(function () {
        //                     if (opts.autoFixed) {
        //                         pinner.removeClass("ui-icon-pin-s").addClass("ui-icon-pin-w");
        //                         el.attr("data-auto-fixed", "false");
        //                         opts.autoFixed = false;
        //                     }
        //                     else {
        //                         pinner.removeClass("ui-icon-pin-w").addClass("ui-icon-pin-s");
        //                         el.attr("data-auto-fixed", "true");
        //                         opts.autoFixed = true;
        //                     }
        //                     self._triggerEvent("change");
        //                 });

        //        if (opts.autoFixed)
        //            pinner.removeClass("ui-icon-pin-w").addClass("ui-icon-pin-s");
        //    }

        //    if (opts.designable) {
        //        $("<span/>").addClass("ui-icon ui-icon-image")
        //                             .appendTo(_tools)
        //                             .click(function () {
        //                                 if (window.uiDesigner)
        //                                     window.uiDesigner.dialog("close");

        //                                 var dlg = $("<div/>").appendTo("body");
        //                                 $("body").blockUI();
        //                                 dlg.load($.resolveUrl("~/theme/uidesigner?locale=") + $("body").attr("lang"), function () {
        //                                     dlg.taoUI();
        //                                     $("body").unblockUI();
        //                                     var _undo = true, _btns = {};

        //                                     _btns[$.res("apply", "Apply")] = function () {
        //                                         _undo = false;
        //                                         dlg.dialog("close");
        //                                         $("body").page("styleChanged");
        //                                     };

        //                                     _btns[$.res("cancel", "Cancel")] = function () { dlg.dialog("close"); };

        //                                     dlg.dialog({
        //                                         title: $.res("design-layout", "Design layout"),
        //                                         width: 430,
        //                                         resizable: false,
        //                                         modal: false,
        //                                         open: function () {
        //                                             designElement(el);
        //                                         },
        //                                         close: function () {
        //                                             if (_undo) resetElementStyle();

        //                                             var filedlg = $("#design_filedlg");
        //                                             if (filedlg.length)
        //                                                 filedlg.remove();
        //                                             dlg.remove();
        //                                         },
        //                                         buttons: _btns
        //                                     });
        //                                     window.uiDesigner = dlg;
        //                                 });
        //                             });
        //    }

        //    if (opts.deletable) {
        //        $("<span/>").addClass("ui-icon ui-icon-close")
        //                             .appendTo(_tools)
        //                             .click(function () {
        //                                 $.confirm($.res("dellayoutconfirm", "Are you sure delete this layout and embded layouts?"))
        //                                   .done(function () {
        //                                       //self._triggerEvent("change");
        //                                       //self._triggerEvent("remove");
        //                                       //el.parent().removeClass("divided");
        //                                       el.remove();
        //                                       $("body").trigger("layoutchange");
        //                                   });
        //                             });
        //    }
        //},
        _initDragAndDrop: function (el) {
            var self = this;
            el.sortable({
                items: el.parents("[data-role='layout']").length == 0 ? ".d-layout" : ".d-layout,.d-widget-zone", //el.parents("[data-role='layout']").length == 0 ? ".d-layout-wrapper" : ".d-layout-wrapper,.d-widget-zone",
                //handle: ".dragHandler",
                connectWith: ".d-layout",
                cancel: ":input,.d-widget-zone",
                revert: 100,
                placeholder: "placeholder",
                forceHelperSize: true,
                dropOnEmpty: true,
                forcePlaceholderSize: true,
                cursorAt: { top: 20 },
                helper: function (event, item) {
                    var widgetCount = $(".d-widget", item);
                    var txt = $.res("move", "Move") + " <strong>" + item.children(".header").text() + "and " + widgetCount.length + " widgets</strong> " + $.res("to", "to") + " ...<span></span>";
                    //console.log(txt);
                    return $("<div/>").addClass("d-layout-sortable-helper").html(txt);
                },
                start: function (event, ui) {
                    ui.item.hide();
                    //$(".d-widget").hide();
                    //$(".d-layout-wrapper.d-state-highlight").removeClass("d-state-highlight");
                    //$(".d-widget-zone").addClass("d-state-activated");
                },
                stop: function (event, ui) {
                    //ui.item.removeClass("d-state-hover").show();
                    //$(".d-widget").show();
                    //$(".d-widget-zone").removeClass("d-state-activated");
                },
                update: function (event, ui) {
                    $("body").trigger("layoutchange", { layout: true });
                    //self._triggerEvent("change");
                }
            }).droppable({
                hoverClass: "d-state-hover",
                greedy: true,
                accept: ".d-layout-tmpl",
                drop: function (event, ui) {
                    var _item = ui.draggable,
                        _cols = _item.data("cols"),
                        _title = _item.data("title"),
                        layoutCount = $("[data-role=layout]").length,
                        layoutIndex = layoutCount;

                    while ($("#layout" + layoutIndex).length)
                        layoutIndex++;

                    var _nl = $("<div id=\"layout" + layoutIndex + "\" />").appendTo(el)
                                       .attr("data-role", "layout")
                                       .attr("data-zone-holder", "true")
                                       .attr("data-label", _title)
                                       .attr("data-cols", _item.data("cols"))
                                       .attr("data-class", _item.data("class") ? _item.data("class") : null)
                                       .layout();
                    _nl.addClass("d-page-part").removeAttr("data-zone-holder");
                },
                over: function (event, ui) {
                    $("[data-name=targetholder]", ui.helper).text(el.attr("data-label"));
                }
            });
        },
        _unobtrusive: function () {
            var opts = this.options, el = this.element;
            if (el.data("label"))
                opts.label = el.data("label");
            if (el.data("zone-holder") != undefined)
                opts.focuszoneHolder = el.dataBool("zone-holder");
            if (el.data("cols"))
                opts.columns = eval(el.data("cols"));
            if (el.data("auto-fixed") != undefined)
                opts.autoFixed = el.dataBool("auto-fixed");
            if (el.data("deletable") != undefined)
                opts.deletable = el.dataBool("deletable")
            if (el.data("design") != undefined)
                opts.designable = el.dataBool("design")
            if (el.data("extensible") != undefined)
                opts.deletable = el.dataBool("extensible")
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        _genZoneID: function () {
            var allzones = $("data-role['widgetzone']"),
            index = allzones.length;
            while ($("#zone" + index).length > 0)
                index++;
            return "zone" + index;
        },
        destroy: function () {
            //$(".d-widget-zone>.title", this.element).remove();
            //$(".header", this.element).remove();
            if (this.layoutTools)
                this.layoutTools.remove();

            $(".ui-droppable", this.element).droppable("destroy");
            $(".ui-sortable", this.element).sortable("destroy");
            $(".ui-resizable", this.element).resizable("destroy");
            $(this.element).sortable("destroy");
            $(".d-state-hover", this.element).removeClass("d-state-hover");
            $(".d-state-selected", this.element).removeClass("d-state-selected");
            //if (this.wrapper) {
            //    this.wrapper.after(this.element);
            //    this.wrapper.remove();
            //}
            //$("body").removeClass("d-state-design");
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
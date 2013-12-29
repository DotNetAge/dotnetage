/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoDialog", {
        options: {
            modal: true,
            width: 300,
            height: 0,
            title: "",
            position: "center center",
            draggable: true,
            resizable: false,
            autoOpen: true,
            fullscreen: false,
            url: null,
            open: null,
            close: null,
            load: null,
            dragstart: null,
            dragstop: null,
            drag: null,
            closeButton: true,
            cache: true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            if (opts.load)
                el.bind(eventPrefix + "load", opts.load);

            if (opts.open)
                el.bind(eventPrefix + "open", opts.open);

            if (opts.close)
                el.bind(eventPrefix + "close", opts.close);

            if (opts.drag)
                el.bind(eventPrefix + "drag", opts.drag);

            if (opts.dragstart)
                el.bind(eventPrefix + "dragstart", opts.dragstart);

            if (opts.dragstop)
                el.bind(eventPrefix + "dragstop", opts.dragstop);

            if (opts.autoOpen)
                this.open();
            else {
                if (!opts.url)
                    el.hide();
            }
            return el;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el[0].tagName == "A" && el.attr("href") && !el.attr("href").startsWith("javascript") && el.attr("href") != "#") {
                opts.url = el.attr("href");
                el.attr("href", "javascript:void(0);");
            }

            if (el.attr("title")) {
                opts.title = el.attr("title");
                el.removeAttr("title");
            } else {
                if (el.data("title"))
                    opts.title = el.data("title");
            }

            if (el.data("pos") != undefined) opts.position = el.data("pos");
            if (el.data("width") != undefined) opts.width = el.dataInt("width");
            if (el.data("height") != undefined) opts.height = el.dataInt("height");
            if (el.data("modal") != undefined) opts.modal = el.dataBool("modal");
            if (el.data("fullscreen") != undefined) opts.fullscreen = el.dataBool("fullscreen");
            if (el.data("opened") != undefined) opts.autoOpen = el.dataBool("opened");
            if (el.data("close-btn") != undefined) opts.closeButton = el.dataBool("close-btn");
            if (el.data("cache") != undefined) opts.cache = el.dataBool("cache");
            if (el.data("load")) opts.load = new Function("event", el.data("load"));
            if (el.data("open")) opts.open = new Function("event", el.data("open"));
            if (el.data("close")) opts.close = new Function("event", el.data("close"));
            if (el.data("draggable") != undefined) opts.draggable = el.dataBool("draggable");
            if (el.data("resizable") != undefined) opts.resizable = el.dataBool("resizable");
            if (el.data("padding") != undefined) el.css("padding", el.data("padding"));
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        _createDialog: function () {
            var el = this.element, self = this, opts = this.options;
            var dlg = $("<div/>").addClass("d-ui-widget d-dialog").appendTo("body"),
                header = $("<h3/>").addClass("d-ui-widget-header d-dialog-header").appendTo(dlg);
            this.header = header;
            $("<span/>").addClass("d-inline").text(opts.title).appendTo(header);
            if (opts.closeButton) {
                var closer = $("<span/>").addClass("d-inline d-dialog-close d-icon-cross-3")
                    .appendTo(header)
                    .click(function () { self.close(); });
            }

            el.addClass("d-ui-widget-content d-dialog-content").appendTo(dlg).show();
            var tabIndex = 0;
            $("[tabIndex]").each(function (i, ti) {
                if ($(ti).attr("tabIndex") > tabIndex)
                    tabIndex = $(ti).attr("tabIndex");
            });

            dlg.attr("tabIndex", tabIndex)
                 .css({ "z-index": $.topMostIndex() + 1 })
                 .bind("keypress", function (event) { if (event.keyCode == 27) { self.close(); } });

            this.dialog = dlg.show();
            var enableCloseButton = function (container) {
                $("[data-rel='close']", container).bind("click", function () { self.close(); });
            };

            if (opts.url) {
                var loader = $("<div/>").appendTo(el).css("padding", "20px;");
                $("<span/>").addClass("d-icon-loading d-inline").appendTo(loader);
                $("<span/>").text("Loading ...").appendTo(loader);
                //this.dialog.hide();
                
                self._setposition();
                $.ajax(opts.url)
                  .done(function (htm) {
                      el.css("opacity", "0");
                      el.html(htm);
                      //el.unobtrusive_ajax();
                      el.taoUI();
                      self._setsize();
                      self._setposition();
                      el.css("opacity", "1");
                      //el.fadeIn();
                      //self.dialog.fadeIn();
                      enableCloseButton(el);
                      self._triggerEvent("load");
                  });
            } else {
                self._setsize();
                self._setposition();
                enableCloseButton(el);
            }

            if (opts.draggable) {
                dlg.draggable({
                    handle: ".d-dialog-header",
                    iframeFix: true,
                    start: function (event, ui) {
                        self._triggerEvent("dragstart", ui);
                    },
                    stop: function (event, ui) {
                        self._triggerEvent("dragstop", ui);
                    },
                    drag: function (event, ui) {
                        self._triggerEvent("drag", ui);
                    }
                });
            }

            if (opts.resizable)
                dlg.resizable();

            return dlg;
        },
        open: function () {
            var self = this, opts = this.options;
            //console.log(opts);
            if (opts.modal && !this.overlay) {
                self.overlay = $("<div/>").addClass("d-overlay")
                                                          .css({ "z-index": $.topMostIndex() })
                                                          .height($(document).height())
                                                          .appendTo("body");
            }

            if (this.dialog) {
                this.dialog.show()
                this._setposition();
            }
            else {
                var dlg = this._createDialog();
                self.dialog = dlg;
            }
            this._triggerEvent("open");
        },
        close: function () {

            if (this.overlay) {
                this.overlay.remove();
                this.overlay = null;
            }

            this.dialog.hide();

            if (this.options.fullscreen) 
                $("body").css("overflow", this.overflow);

            this._triggerEvent("close");

            if (!this.options.cache) {
                this.element.unbind();
                this.dialog.remove();
            }
        },
        _setsize: function () {
            var opts = this.options, _size = { width: window.innerWidth, height: window.innerHeight };
            if (!opts.fullscreen) {
                if (opts.width < _size.width)
                    _size.width = opts.width;

                if (opts.height < _size.height)
                    _size.height = opts.height;
                else
                    _size.height = 0;
            }
            //else
            //    this.dialog.css({
            //        "max-height": (_size.height - this.header.outerHeight(true)) + "px",
            //        "overfloat":"auto"
            //    });


            this.dialog.width(_size.width);

            if (_size.height)
                this.dialog.height(_size.height);
        },
        _setposition: function () {
            var opts = this.options;
            if (opts.fullscreen){
                this.overflow= $("body").css("overflow");
                $("body").css("overflow", "hidden");
            }

            this.dialog.css({ "z-index": $.topMostIndex() + 1 })
                .stop(true, false)
                .position({
                    of: $(window),
                    at: opts.position,
                    offset:opts.fullscreen ? (window.scrollX+"px "+window.scrollY+"px") : "0px 0px",
                    my: "center center"
                }).focus();
        },
        destroy: function () {
            var self = this, el = this.element, opts = this.options;
            if (self.overlay)
                self.overlay.remove();

            if (self.dialog)
                self.dialog.remove();

            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
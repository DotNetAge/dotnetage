(function ($) {
    $.widget("dna.taoPanel", {
        options: {
            //draggable: false,
            collapsable: true,
            icon: null,
            link:null,
            //resizable: false,
            closable: false,
            display: "static", //possible values: static | push | overlay | dialog (modal dialog) | window (no modal)
            autoOpen: true, // not avaliable for static
            autoRelease: false, // not avaliable for static
            autoLoad: true,
            dismissible: true,
            position: "left",
            returnTo:null,
            //shadow: null, // values: hover peel blend popout
            collapsed: false,
            opened: false,
            contentUrl: null,
            fillHeight: true,
            icons: {
                collapsed: "d-icon-caret-down",
                expanded: "d-icon-caret-up"
            },
            width: 300,
            load: null,
            expand: null,
            collapse: null,
            open: null,
            close: null
        },
        _create: function () {
            var opts = this.options, el = this.element, self = this, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();

            el.addClass("d-reset d-ui-widget d-panel")
               .addClass("d-panel-" + opts.display);

            if ("static" != opts.display) {
                if ($(window).width() <= 420) {
                    this.options.width = $(window).width()
                }
                opts.collapsable = false;
                opts.collapsed = false;
                //opts.closable = true;
                el.addClass("d-panel-" + opts.position);

                if (el.parent()[0].tagName != "body")
                    el.prependTo("body");
            }

            self.header = $(">header,>h3", el).first();

            if (self.header.length == 0 && el.attr("title")) {
                el.wrapInner("<div/>");
                self.header = $("<h3/>").text(el.attr("title")).prependTo(el);
                el.removeAttr("title");
            }

            if (self.header.length) {
                var _title = self.header.text();
                self.header.wrapInner("<a href=\"javascript:void(0);\" class='d-panel-header-link'/>");
                self.header.addClass("d-ui-widget-header d-panel-header");
                var emptyLink = self.header.children("a:first");
                //emptyLink.attr("title", _title);
                emptyLink.wrapInner("<span class='d-header-text'/>");

                if (opts.icon) {
                    var leftIcon=$("<span/>").addClass("d-inline")
                        .css({ "margin-right": "5px" })
                        .addClass(opts.icon)
                        .prependTo(emptyLink);
                    if (opts.link) {
                        leftIcon.click(function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            window.location = opts.link;
                        });
                    }
                }

                if (opts.returnTo) {
                    var returnBtn = $("<a/>").attr("data-icon-left", "d-icon-angle-left").css({
                        "display": "inline-block",
                        "position": "absolute"
                    })
                                                             .prependTo(self.header)
                                                             .click(function () {
                                                                 $(opts.returnTo).taoPanel("open");
                                                                 self.close();
                                                             }).taoButton();
                    returnBtn.next().css("margin-left","40px");
                }
                
                if (opts.closable) {
                    $("<span/>").addClass("d-icon-holder d-close-icon-holder")
                        .addClass("d-icon-cross-3")
                        .appendTo(emptyLink)
                        .click(function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            self.close();
                        });
                }

                if (opts.collapsable) {
                    var collapseHandler = function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        self.expand(!opts.collapsed);
                    };

                    $("<span/>").addClass("d-icon-holder d-toggle-icon-holder")
                        .addClass(opts.collapsed ? opts.icons.collapsed : opts.icons.expanded)
                        .appendTo(emptyLink)
                        .click(collapseHandler);

                    if (el.attr("id") != undefined)
                        $("a[data-rel=\"collapse\"][href=\"#" + el.attr("id") + "\"]").on("click", collapseHandler);
                }
            }

            var _body = $(">div", el).addClass("d-ui-widget-content");
            if (_body.length == 0)
                _body = $("<div/>").addClass("d-ui-widget-content").appendTo(el);

            self.body = _body;

            // if (opts.autoLoad)
            // self._load();

            if (opts.collapsable && opts.collapsed)
                _body.hide();

            if (opts.load)
                el.bind(eventPrefix + "load", opts.load);

            if (opts.open)
                el.bind(eventPrefix + "open", opts.open);

            if (opts.close)
                el.bind(eventPrefix + "close", opts.close);

            if (opts.collapse)
                el.bind(eventPrefix + "collapse", opts.collapse);

            if (opts.expand)
                el.bind(eventPrefix + "expand", opts.expand);

            if (self.header.length) {
                self.header.bind("click", function () {
                    if (opts.collapsable && !opts.closable && !opts.draggable) {
                        $(".d-toggle-icon-holder", self.header).click();
                    }
                });
            }

            //window.addEventListener("resize",$.proxy(this, this._winResize),false);
            $(window).on("resize", $.proxy(this._winResize, this));
            if ("static" != opts.display) {
                if (opts.opened) {
                    opts.opened = false;
                    self.open();
                }
                else
                    el.hide();

                $("a[data-rel=close]", el).on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    self.close();
                });

                if (el.attr("id") != undefined) {
                    $("a[data-rel=close][href=\"#" + el.attr("id") + "\"]").on("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        self.close();
                    });

                    $("a[data-rel=open][href=\"#" + el.attr("id") + "\"]").on("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        $(".d-panel[data-opened=true]").not(el).each(function (i, pane) {
                            $(pane).taoPanel("close");
                        });

                        self.open();
                    });
                }

            } else
                this._fillHeight(opts.fillHeight);
        },
        _unobtrusive: function () {
            var $t = this.element, opts = this.options;
            if ($t.data("icon")) opts.icon = $t.data("icon");
            if ($t.data("link")) opts.link = $t.data("link");
            if ($t.data("return")) opts.returnTo = $t.data("return");

            if ($t.data("collapsed-icon")) opts.icons.collapsed = $t.data("collapsed-icon");
            if ($t.data("expanded-icon")) opts.icons.expanded = $t.data("expanded-icon");
            if ($t.data("pos")) opts.position = $t.data("pos");
            if ($t.data("display")) opts.display = $t.data("display");
            if ($t.data("fill") != undefined) opts.fillHeight = $t.dataBool("fill");
            if ($t.data("width") != undefined) opts.width = $t.data("width");
            if ($t.data("collapsable") != undefined) opts.collapsable = $t.dataBool("collapsable");
            if ($t.data("closable") != undefined) opts.closable = $t.dataBool("closable");
            if ($t.data("collapsed") != undefined) opts.collapsed = $t.dataBool("collapsed");
            if ($t.data("auto-load") != undefined) opts.autoLoad = $t.dataBool("auto-load");
            if ($t.data("dismissible") != undefined) opts.dismissible = $t.dataBool("dismissible");
            //if ($t.data("auto-open") != undefined) opts.autoOp = $t.dataBool("auto-open");
            if ($t.data("opened") != undefined) opts.opened = $t.dataBool("opened");
            if ($t.data("auto-release") != undefined) opts.autoRelease = $t.dataBool("auto-release");
            if ($t.data("load")) opts.load = new Function("event", $t.data("load"));
            if ($t.data("open")) opts.open = new Function("event", $t.data("open"));
            if ($t.data("close")) opts.close = new Function("event", $t.data("close"));
            if ($t.data("expand")) opts.expand = new Function("event", "ui", $t.data("expand"));
            if ($t.data("collapse")) opts.collapse = new Function("event", "ui", $t.data("collapse"));

            
            //if ($t.data("always-open")
            if ($t.data("url"))
                opts.contentUrl = $t.data("url");

            if ($t.data("title"))
                $t.attr("title", $t.data("title"));

            if ($t.data("closable") == undefined && opts.display != "static") 
                opts.closable = true;

            if ($t.data("fill") == undefined && opts.display == "static")
                opts.fillHeight = false;

            return this;
        },
        _load: function (callback) {
            var opts = this.options, el = this.element, self = this;

            if (opts.contentUrl) {

                if (self.header.length) {
                    var headerLink = self.header.children(".d-panel-header-link");

                    $(">a[data-icon-left='d-icon-angle-left']", self.header).hide();

                    $(".d-icon-holder", headerLink).hide();
                    var loadingEle = $("<span/>").addClass("d-icon-holder d-icon-loading").appendTo(headerLink);
                }

                var _body = self.body;

                $.ajax(opts.contentUrl)
                  .done(function (htm) {
                      _body.html(htm);
                      _body.unobtrusive_ajax();
                      _body.taoUI();
                      _body.show();

                      if (self.header.length) {
                          loadingEle.remove();
                          $(">a[data-icon-left='d-icon-angle-left']", self.header).show();
                          $(".d-icon-holder", headerLink).show();
                      }

                      self.loaded = true;
                      self._triggerEvent("load");

                      if ($.isFunction(callback))
                          callback();

                      el.attr("data-opened", true);

                      $(".d-button[data-rel=close]", el).on("click", function (e) {
                          e.preventDefault();
                          e.stopPropagation();
                          self.close();
                      });
                  })
                  .fail(function (jqXHR, textStatus) {
                      $.err("Load failed: " + textStatus);
                  });

            }
        },
        _winResize: function () {
            var self = this, opts = this.options, el = this.element;

            if (this.dismisshelper)
                $(this.dismisshelper).css({ "width": ($(window).width() - el.width()) + "px" });

            if (opts.opened || "static" == opts.display)
                this._fillHeight(opts.fillHeight);

            if ("static" != opts.display && opts.position == "right" && opts.opened)
                el.stop(true, false).position(this._getPos());
        },
        _setOption: function (key, value) {
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _setsize: function () { },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        _fillHeight: function (val) {
            if (val) {
                var self = this, el = this.element;
                el.height($(window).height());
                self.body.height(el.innerHeight() - $(">.d-panel-header", el).outerHeight(true));
            }
        },
        _getPos: function () {
            var opts = this.options, el = this.element, self = this, w = el.width();
            return {
                of: $("body"),
                at: opts.position + " top",
                my: opts.position + " top",
                using: function (using) {
                    using.top = 0;

                    if (opts.display == "reveal") {
                        using["z-index"] = -1;
                        $(this).show().css(using);
                    } else
                        $(this).stop(true, false).animate(using, 200);

                    if (opts.display != "overlay") {
                        $("body").data("margin", $("body").css("margin"))
                                        .stop(true, false);

                        if (opts.position == "left")
                            $("body").animate({ "margin-left": w + "px" }, 200);
                        else
                            $("body").animate({ "margin-left": "-" + w + "px", "margin-right": w + "px" }, 200);
                    }
                }
            };
        },
        expand: function (val) {
            var el = this.element, opts = this.options;
            if (opts.collapsable) {

                //if ((!opts.collapsed && val) || (opts.collapsed && !val))
                //    return el;

                this.header.find(".d-toggle-icon-holder")
                                   .toggleClass(opts.icons.collapsed)
                                   .toggleClass(opts.icons.expanded)

                if (opts.contentUrl && !this.loaded)
                    this._load();

                this.body.toggle();

                this._triggerEvent(val ? "expand" : "collapse");
                opts.collapsed = opts.collapsed;
            }
            return el;
        },
        open: function () {
            var opts = this.options, el = this.element, self = this;

            if ("static" != opts.display) {
                if (opts.opened)
                    return el;

                el.width(opts.width);
                var w = el.width();

                el.css({
                    "opacity": 0,
                    "z-index": $.topMostIndex() + 1,
                    "top": "0px"
                   , "left": opts.position == "left" ? ((-w) + "px") : ($("body").width() + w) + "px"
                }).show();

                self._fillHeight(opts.fillHeight);
                el.show()
                   .stop(true, false)
                   .css("opacity", 1)
                   .position(this._getPos());
            }

            opts.opened = true;
            el.attr("data-opened", true);
            this._triggerEvent("open");

            if (opts.contentUrl && !self.loaded)
                self._load();

            if (opts.dismissible) {
                this.dismisshelper = $("<div/>").appendTo("body")
                                                                    .height($(document).height())
                                                                    .css({
                                                                        "opacity": 0,
                                                                        "width": ($(window).width() - el.width()) + "px",
                                                                        "background": "#000",
                                                                        "position": "absolute",
                                                                        "top": "0px"
                                                                    }).click(function () {
                                                                        self.close();
                                                                    });
                if (opts.position == "left")
                    this.dismisshelper.css({ left: el.width() + "px" });
                else
                    this.dismisshelper.css({ left: "0px" });
                //   this.dismisshelper.css({ left: "-" + el.width() + "px" });
            }

            return el;
        },
        close: function () {
            var el = this.element, opts = this.options, self = this;
            if (!opts.closable)
                return;

            if ("static" != opts.display && opts.opened) {
                this._triggerEvent("close");
                opts.opened = false;

                var width = el.outerWidth(true),
                    left = (opts.position == "left" || opts.position == null) ? -width : $(document).width() + width;

                if ("reveal" != opts.display)
                    el.stop(true, false).animate({ left: (left / 16) + "em" }, 200, function () { el.hide(); });

                if ("overlay" != opts.display) {
                    $("body").stop(true, false).animate({ "margin-left": "0px", "margin-right": "0px" }, function () {
                        $("body").css("margin", $("body").data("margin"));
                        if ("reveal" == opts.display)
                            el.hide();
                    });
                }
            }

            if (this.dismisshelper) {
                this.dismisshelper.remove();
                this.dismisshelper = null;
            }

            if (opts.autoRelease)
                el.remove();
        },
        destroy: function () {
            $(window).off("resize", $.proxy(this._winResize, this));
            if (this.dismisshelper) {
                this.dismisshelper.remove();
                this.dismisshelper = null;
            }
            return $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);